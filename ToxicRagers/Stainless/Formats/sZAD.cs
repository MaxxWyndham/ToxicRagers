using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using ToxicRagers.Compression.LZ4;
using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public enum ZADType
    {
        Archive,
        VirtualTexture
    }

    public enum CompressionMethods
    {
        None = 0,
        Deflate = 0x8,
        LZ4 = 0x38
    }

    public class ZAD
    {
        private string name;
        private string extension;
        private string location;
        private List<ZADEntry> contents;
        private ZADType type = ZADType.Archive;

        public string Name => name;
        public string Extension => extension;
        public List<ZADEntry> Contents => contents;
        public bool IsVT => type == ZADType.VirtualTexture;

        public ZADType Type
        {
            get => type;
            set => type = value;
        }

        public int CentralDirectoryOffset => contents.Sum(e => e.EntrySize + e.Padding);

        public ZAD()
        {
            contents = new List<ZADEntry>();
        }

        public ZAD(string name)
            : this()
        {
            this.name = name;
        }

        public static ZAD Create(string path)
        {
            return Create(path, ZADType.Archive);
        }

        public static ZAD Create(string path, ZADType type)
        {
            ZAD zad = new ZAD(Path.GetFileNameWithoutExtension(path))
            {
                extension = Path.GetExtension(path),
                type = type,
                location = Path.GetDirectoryName(path)
            };

            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                zad.writeCentralDirectory(bw);
            }

            return zad;
        }

        public static ZAD Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            //Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            ZAD zad = new ZAD()
            {
                name = Path.GetFileNameWithoutExtension(path),
                extension = Path.GetExtension(path),
                location = Path.GetDirectoryName(path) + "\\"
            };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0x50 ||
                    br.ReadByte() != 0x4B)
                {
                    zad.type = ZADType.VirtualTexture;
                }

                br.BaseStream.Seek(-22, SeekOrigin.End);

                if (br.ReadByte() != 0x50 ||
                    br.ReadByte() != 0x4B ||
                    br.ReadByte() != 0x05 ||
                    br.ReadByte() != 0x06)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid ZAD file", path);
                    return null;
                }

                br.ReadUInt32();

                int recordCount = br.ReadUInt16();
                if (recordCount != br.ReadUInt16())
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Something has gone horribly wrong!  Aborting");
                    return null;
                }

                int directorySize = (int)br.ReadUInt32();
                int directoryOffset = (int)br.ReadUInt32();

                br.ReadUInt16();

                br.BaseStream.Seek(directoryOffset, SeekOrigin.Begin);

                using (BinaryMemoryStream ms = new BinaryMemoryStream(br.ReadBytes(directorySize)))
                {
                    for (int i = 0; i < recordCount; i++)
                    {
                        zad.contents.Add(new ZADEntry(ms));
                    }
                }

                /* The padding calculations below serve no real purpose, they're just used for debugging VTs */

                ZADEntry lastEntry = null;

                foreach (ZADEntry entry in zad.contents.OrderBy(e => e.Offset))
                {
                    if (lastEntry != null)
                    {
                        lastEntry.Padding = entry.Offset - lastEntry.Offset - lastEntry.EntrySize;
                    }

                    lastEntry = entry;
                }

                lastEntry.Padding = directoryOffset - lastEntry.Offset - lastEntry.EntrySize;
            }

            return zad;
        }

        public void RemoveEntry(ZADEntry entry)
        {
            int indexOfEntry = contents.IndexOf(entry);
            int oldContentsCount = contents.Count;
            int centralDirectoryOffset = CentralDirectoryOffset;
            //ZADEntry nextEntry = indexOfEntry == contents.Count ? null : contents[indexOfEntry + 1];
            contents.Remove(entry);
            using (FileStream fs = new FileStream(Path.Combine(location, $"{name}{extension}"), FileMode.Open))
            {
                Logger.LogToFile(Logger.LogLevel.Info, "\tFile Size Before Removal:\t\t{0}", fs.Length);
                Logger.LogToFile(Logger.LogLevel.Info, "\tEntryOffset:\t\t\t\t\t\t{0}", entry.Offset);
                int dataSize = entry.EntrySize + entry.Padding;
                Logger.LogToFile(Logger.LogLevel.Info, "\tEntrySize:\t\t\t\t\t\t{0}\n\tEntryPadding:\t\t\t\t\t{1}\n\tDataSize:\t\t\t\t\t\t{2}", entry.EntrySize, entry.Padding, dataSize);
                Logger.LogToFile(Logger.LogLevel.Info, "\tEntryOffset+DataSize:\t\t\t{0}\n\tNextEntryOffset:\t\t\t\t{1}", entry.Offset + dataSize, contents[indexOfEntry + 1].Offset);

                int nextEntryOffset = entry.Offset + dataSize;
                int foundOffset = contents[indexOfEntry + 1].Offset;
                ZADEntry foundEntry = null;
                int firstIndexAfterEntry = 0;

                for (int i = 1; i < contents.Count; i++)
                {
                    if (contents[i].Offset >= nextEntryOffset && contents[i].Offset <= foundOffset)
                    {
                        foundOffset = contents[i].Offset;
                        foundEntry = contents[i];
                    }

                    if (contents[i].Offset > entry.Offset)
                    {
                        if (firstIndexAfterEntry == 0) { firstIndexAfterEntry = i; }
                        contents[i].Offset -= dataSize;
                    }
                }

                if (foundEntry != null) { Logger.LogToFile(Logger.LogLevel.Info, "\tFoundEntryOffset:\t\t\t{0}\n\tFoundEntryName:\t\t\t\t\t{1}\n\tFoundEntrySize:\t\t\t\t\t{2}", foundOffset, foundEntry.Name, foundEntry.EntrySize + foundEntry.Padding); }
                Logger.LogToFile(Logger.LogLevel.Info, "\tCalculatedNextEntryOffset:\t\t{0}", nextEntryOffset);
                fs.Seek(nextEntryOffset, SeekOrigin.Begin);
                byte[] data = new byte[fs.Length - nextEntryOffset];
                fs.Read(data, 0, (int)fs.Length - nextEntryOffset);
                fs.Seek(entry.Offset, SeekOrigin.Begin);
                fs.SetLength(entry.Offset);
                fs.Write(data, 0, data.Length);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Seek(CentralDirectoryOffset, SeekOrigin.Begin);
                    fs.SetLength(CentralDirectoryOffset);
                    writeCentralDirectory(bw);
                    Logger.LogToFile(Logger.LogLevel.Info, "\tFile Size After Removal:\t\t{0}", fs.Length);
                }

            }
        }

        public void ReplaceEntryFromFile(ZADEntry entry, string file)
        {
            ReplaceEntryFromBuffer(entry, File.ReadAllBytes(file));
        }

        public void ReplaceEntryFromBuffer(ZADEntry entry, byte[] newData)
        {
            int indexOfEntry = contents.IndexOf(entry);
            int oldContentsCount = contents.Count;
            int centralDirectoryOffset = CentralDirectoryOffset;

            using (FileStream fs = new FileStream(Path.Combine(location, $"{name}{extension}"), FileMode.Open))
            {
                int dataSize = entry.EntrySize + entry.Padding;
                entry.Data = newData;
                entry.Padding = 0;

                if (Type == ZADType.VirtualTexture)
                {
                    while (entry.Data.Length > entry.Padding) { entry.Padding += 4096; }
                    entry.Padding -= entry.Data.Length;
                }

                int newDataSize = entry.EntrySize + entry.Padding;
                int nextEntryOffset = entry.Offset + dataSize;
                int foundOffset = contents[indexOfEntry + 1].Offset;
                ZADEntry foundEntry = null;
                int firstIndexAfterEntry = 0;

                for (int i = 1; i < contents.Count; i++)
                {
                    if (indexOfEntry == i) { continue; }

                    if (contents[i].Offset >= nextEntryOffset && contents[i].Offset <= foundOffset)
                    {
                        foundOffset = contents[i].Offset;
                        foundEntry = contents[i];
                    }

                    if (contents[i].Offset > entry.Offset)
                    {
                        if (firstIndexAfterEntry == 0) { firstIndexAfterEntry = i; }
                        contents[i].Offset -= dataSize;
                        contents[i].Offset += newDataSize;
                    }
                }

                fs.Seek(nextEntryOffset, SeekOrigin.Begin);
                byte[] data = new byte[fs.Length - nextEntryOffset];
                fs.Read(data, 0, (int)fs.Length - nextEntryOffset);
                fs.Seek(entry.Offset, SeekOrigin.Begin);

                fs.SetLength(entry.Offset);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    if (type == ZADType.Archive)
                    {
                        bw.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 });
                        bw.Write((short)0x14);  // Version needed to extract
                        bw.Write((short)0);     // Flags
                        bw.Write((short)entry.CompressionMethod);
                        bw.Write(0);            // DateTime
                        bw.Write(entry.CRC);
                        bw.Write(entry.SizeCompressed);
                        bw.Write(entry.SizeUncompressed);
                        bw.Write(entry.Name.Length);
                        bw.Write(entry.Name.ToCharArray());
                    }
                    bw.Write(entry.Data);

                    if (type == ZADType.VirtualTexture)
                    {
                        bw.Write(new byte[entry.Padding]);
                    }

                    fs.Write(data, 0, data.Length);
                    bw.Seek(CentralDirectoryOffset, SeekOrigin.Begin);
                    fs.SetLength(CentralDirectoryOffset);
                    writeCentralDirectory(bw);
                }
            }
        }

        public void AddEntryFromFile(string file, string folder)
        {
            AddEntryFromBuffer(File.ReadAllBytes(file), Path.Combine(folder, Path.GetFileName(file)));
        }

        public void AddEntryFromBuffer(byte[] data, string name)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(location, $"{name}{extension}"), FileMode.Open)))
            {
                bw.Seek(CentralDirectoryOffset, SeekOrigin.Begin);

                ZADEntry entry = new ZADEntry()
                {
                    CompressionMethod = (type == ZADType.Archive ? CompressionMethods.Deflate : CompressionMethods.LZ4),
                    Name = name,
                    Offset = (int)bw.BaseStream.Position,
                    Data = data
                };

                if (type == ZADType.Archive)
                {
                    bw.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 });
                    bw.Write((short)0x14);  // Version needed to extract
                    bw.Write((short)0);     // Flags
                    bw.Write((short)entry.CompressionMethod);
                    bw.Write(0);            // DateTime
                    bw.Write(entry.CRC);
                    bw.Write(entry.SizeCompressed);
                    bw.Write(entry.SizeUncompressed);
                    bw.Write(entry.Name.Length);
                    bw.Write(entry.Name.ToCharArray());
                }
                bw.Write(entry.Data);

                if (type == ZADType.VirtualTexture)
                {
                    while (entry.Data.Length > entry.Padding) { entry.Padding += 4096; }
                    entry.Padding -= entry.Data.Length;
                    bw.Write(new byte[entry.Padding]);
                }

                contents.Add(entry);

                writeCentralDirectory(bw);
            }
        }

        public void AddDirectory(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(location, $"{name}{extension}"), FileMode.Open)))
            {
                addDirectory(bw, path, path.Substring(0, path.IndexOf(Path.GetFileName(path))));

                writeCentralDirectory(bw);
            }
        }

        private void addDirectory(BinaryWriter bw, string path, string root)
        {
            bw.Seek(CentralDirectoryOffset, SeekOrigin.Begin);

            ZADEntry directory = new ZADEntry()
            {
                Name = path.Replace(root, "").Replace("\\", "/") + "/",
                Offset = (int)bw.BaseStream.Position
            };

            /* Write entry (ZAD.VTs don't record folder info as specific entries) */
            if (type == ZADType.Archive)
            {
                bw.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 });
                bw.Write((short)0x0a);  // Version needed to extract
                bw.Write((short)0);     // Flags
                bw.Write((short)CompressionMethods.None);
                bw.Write(0);            // DateTime
                bw.Write(0);            // CRC
                bw.Write(0);            // Compressed filesize
                bw.Write(0);            // Uncompressed filesize
                bw.Write(directory.Name.Length);
                bw.Write(directory.Name.ToCharArray());
                contents.Add(directory);
            }

            foreach (string folder in Directory.GetDirectories(path))
            {
                addDirectory(bw, folder, root);
            }

            foreach (string file in Directory.GetFiles(path))
            {
                if (string.Compare(Path.GetExtension(file), ".zad", true) == 0) { continue; }
                if (string.Compare(Path.GetExtension(file), ".zip", true) == 0) { continue; }
                if (string.Compare(Path.GetExtension(file), ".img", true) == 0) { continue; }

                ZADEntry entry = new ZADEntry()
                {
                    CompressionMethod = (type == ZADType.Archive ? CompressionMethods.Deflate : CompressionMethods.LZ4),
                    Name = directory.Name + Path.GetFileName(file),
                    Offset = (int)bw.BaseStream.Position,
                    Data = File.ReadAllBytes(file)
                };

                if (type == ZADType.Archive)
                {
                    bw.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 });
                    bw.Write((short)0x14);  // Version needed to extract
                    bw.Write((short)0);     // Flags
                    bw.Write((short)entry.CompressionMethod);
                    bw.Write(0);            // DateTime
                    bw.Write(entry.CRC);
                    bw.Write(entry.SizeCompressed);
                    bw.Write(entry.SizeUncompressed);
                    bw.Write(entry.Name.Length);
                    bw.Write(entry.Name.ToCharArray());
                }
                bw.Write(entry.Data);

                if (type == ZADType.VirtualTexture)
                {
                    while (entry.Data.Length > entry.Padding) { entry.Padding += 4096; }
                    entry.Padding -= entry.Data.Length;
                    bw.Write(new byte[entry.Padding]);
                }

                contents.Add(entry);
            }
        }

        private void writeCentralDirectory(BinaryWriter bw)
        {
            int centralDirectorySize = (int)bw.BaseStream.Position;

            foreach (ZADEntry entry in contents)
            {
                /* Write end of dictionary */
                bw.Write(new byte[] { 0x50, 0x4b, 0x01, 0x02 });
                bw.Write((byte)0x3f);   // Version made by
                bw.Write((byte)0);      // Host OS
                bw.Write((byte)0x0a);   // Minimum version to extract
                bw.Write((byte)0);      // Target OS
                bw.Write((short)(entry.CompressionMethod == CompressionMethods.LZ4 ? 0x100 : 0x0)); // Flags
                bw.Write((short)entry.CompressionMethod);
                bw.Write(0);            // DateTime
                bw.Write(entry.CRC);
                bw.Write(entry.SizeCompressed);
                bw.Write(entry.SizeUncompressed);
                bw.Write((short)entry.Name.Length);
                bw.Write((short)36);    // Extra field length
                bw.Write((short)0);     // Comment length
                bw.Write((short)0);     // Disk number
                bw.Write((short)0);     // Internal file attributes
                bw.Write(0x20);         // External file attributes
                bw.Write(entry.Offset); // Relative offset from start
                bw.Write(entry.Name.ToCharArray());
                bw.Write((short)0x0a);
                bw.Write((short)0x20);
                bw.Write(0);
                bw.Write((short)0x01);
                bw.Write((short)0x18);
                bw.Write(DateTime.Now.Ticks);
                bw.Write(DateTime.Now.Ticks);
                bw.Write(DateTime.Now.Ticks);
            }

            centralDirectorySize = (int)bw.BaseStream.Position - centralDirectorySize;

            // Write end of dictionary
            bw.Write(new byte[] { 0x50, 0x4b, 0x05, 0x06 });
            bw.Write((short)0);     // Disk number
            bw.Write((short)0);     // Disk number with central directory
            bw.Write((short)contents.Count);
            bw.Write((short)contents.Count);
            bw.Write(centralDirectorySize);
            bw.Write(CentralDirectoryOffset);
            bw.Write((short)0);     // Comment length
        }

        public void Extract(ZADEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            file.Name = file.Name.Replace("/", "\\");

            if (file.SizeUncompressed == 0)
            {
                if (!Directory.Exists(destination + file.Name)) { Directory.CreateDirectory(destination + file.Name); }
            }
            else
            {
                string folder = Path.GetDirectoryName(destination + file.Name);
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }

                using (BinaryWriter bw = new BinaryWriter(new FileStream(destination + file.Name, FileMode.Create)))
                {
                    bw.Write(ExtractToBuffer(file));
                }
            }
        }

        public byte[] ExtractToBuffer(ZADEntry file)
        {
            file.Name = file.Name.Replace("/", "\\");

            if (file.SizeUncompressed == 0) { return null; }

            using (BinaryFileStream bfs = new BinaryFileStream(Path.Combine(location, $"{name}{extension}"), FileMode.Open))
            {
                bfs.Seek(file.Offset, SeekOrigin.Begin);

                return Decompress(file, bfs);
            }
        }

        public byte[] Decompress(ZADEntry file, BinaryFileStream bfs)
        {
            byte[] buff = new byte[file.SizeUncompressed];

            switch (file.CompressionMethod)
            {
                case CompressionMethods.None:
                case CompressionMethods.Deflate:
                    if (bfs.ReadByte() != 0x50 ||
                        bfs.ReadByte() != 0x4B ||
                        bfs.ReadByte() != 0x03 ||
                        bfs.ReadByte() != 0x04)
                    {
                        Logger.LogToFile(Logger.LogLevel.Error, "{0:x2} has a malformed header", (bfs.Position - 4));
                        return null;
                    }

                    bfs.ReadUInt16();    // version needed to extract    
                    bfs.ReadUInt16();    // flags
                    bfs.ReadUInt16();    // compression method
                    bfs.ReadUInt16();    // file last modified time
                    bfs.ReadUInt16();    // file last modified date
                    bfs.ReadUInt32();    // crc32
                    bfs.ReadUInt32();    // compressed size
                    bfs.ReadUInt32();    // uncompressed size
                    int nameLength = bfs.ReadUInt16();
                    int extraLength = bfs.ReadUInt16();
                    bfs.ReadString(nameLength);
                    bfs.ReadBytes(extraLength);

                    if (file.CompressionMethod == CompressionMethods.None)
                    {
                        bfs.Read(buff, 0, file.SizeUncompressed);
                    }
                    else
                    {
                        using (MemoryStream ms = new MemoryStream(bfs.ReadBytes(file.SizeCompressed)))
                        using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                        {
                            ds.Read(buff, 0, file.SizeUncompressed);
                        }
                    }
                    break;

                case CompressionMethods.LZ4:
                    using (MemoryStream ms = new MemoryStream(bfs.ReadBytes(file.SizeCompressed)))
                    using (LZ4Decompress lz4 = new LZ4Decompress(ms))
                    {
                        lz4.Read(buff, 0, file.SizeCompressed);
                    }
                    break;

                default:
                    throw new NotImplementedException(string.Format("Unknown CompressionMethod: {0}", file.CompressionMethod));
            }

            return buff;
        }
    }

    public class ZADEntry
    {
        byte[] data;

        byte[] crc = new byte[] { 0, 0, 0, 0 };
        int sizeCompressed;
        int sizeUncompressed;
        int offset;
        int padding;
        string name;
        CompressionMethods compressionMethod;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Offset
        {
            get => offset;
            set => offset = value;
        }

        public int Padding
        {
            get => padding;
            set => padding = value;
        }

        public byte[] CRC => crc;

        public byte[] Data
        {
            get => data;
            set
            {
                data = value;
                sizeUncompressed = data.Length;
                crc = (new CRC32()).Hash(data);

                if (sizeUncompressed > 0)
                {
                    switch (compressionMethod)
                    {
                        case CompressionMethods.Deflate:
                            using (MemoryStream ms = new MemoryStream())
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
                            {
                                ds.Write(data, 0, data.Length);
                                ds.Flush();
                                ds.Close();

                                data = ms.ToArray();
                            }
                            break;

                        case CompressionMethods.LZ4:
                            using (MemoryStream ms = new MemoryStream())
                            using (LZ4Compress lz4 = new LZ4Compress(ms))
                            {
                                lz4.Write(data, 0, data.Length);
                                lz4.Flush();
                                lz4.Close();

                                data = ms.ToArray();
                            }
                            break;
                    }
                }
                else
                {
                    compressionMethod = CompressionMethods.None;
                }

                sizeCompressed = data.Length;
            }
        }

        public int SizeCompressed => sizeCompressed;
        public int SizeUncompressed => sizeUncompressed;

        public int EntrySize
        {
            get
            {
                if (compressionMethod == CompressionMethods.LZ4)
                {
                    return sizeCompressed;
                }
                else
                {
                    return name.Length + sizeCompressed + 0x1e;
                }
            }
        }

        public CompressionMethods CompressionMethod
        {
            get => compressionMethod;
            set => compressionMethod = value;
        }

        public ZADEntry()
        {
        }

        public ZADEntry(BinaryMemoryStream br)
        {
            if (br.ReadByte() != 0x50 ||
                br.ReadByte() != 0x4B ||
                br.ReadByte() != 0x01 ||
                br.ReadByte() != 0x02)
            {
                Logger.LogToFile(Logger.LogLevel.Error, "{0:x2} isn't a valid ZAD entry", (br.Position - 4));
                return;
            }

            br.ReadByte();      // version made by
            br.ReadByte();      // Host OS
            br.ReadByte();      // version needed to extract    
            br.ReadByte();      // target os
            int flags = br.ReadUInt16();
            compressionMethod = (CompressionMethods)br.ReadUInt16();
            br.ReadUInt16();    // file last modified time
            br.ReadUInt16();    // file last modified date
            br.ReadUInt32();    // crc32
            sizeCompressed = (int)br.ReadUInt32();
            sizeUncompressed = (int)br.ReadUInt32();
            int nameLength = br.ReadUInt16();
            int extraLength = br.ReadUInt16();
            br.ReadUInt16();    // file comment length
            br.ReadUInt32();    // disk number
            br.ReadUInt16();    // internal file attributes
            br.ReadUInt16();    // external file attributes
            offset = (int)br.ReadUInt32();
            name = br.ReadString(nameLength);
            br.ReadBytes(extraLength);
        }
    }
}