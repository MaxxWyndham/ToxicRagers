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
        private ZADType type = ZADType.Archive;

        public string Name => name;
        public string Extension => extension;
        public List<ZADEntry> Contents { get; } = new List<ZADEntry>();
        public bool IsVT => type == ZADType.VirtualTexture;

        public ZADType Type
        {
            get => type;
            set => type = value;
        }

        public uint CentralDirectoryOffset => (uint)Contents.Sum(e => e.EntrySize + e.Padding);

        public ZAD()
        {
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

			return Load(fi.OpenRead(), path);
		}

        public static ZAD Load(Stream stream, string path)
        {
            //Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            ZAD zad = new ZAD()
            {
                name = Path.GetFileNameWithoutExtension(path),
                extension = Path.GetExtension(path),
                location = Path.GetDirectoryName(path) + "\\"
            };

            using (BinaryReader br = new BinaryReader(stream))
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

                ushort recordCount = br.ReadUInt16();
                if (recordCount != br.ReadUInt16())
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Something has gone horribly wrong!  Aborting");
                    return null;
                }

                uint directorySize = br.ReadUInt32();
                uint directoryOffset = br.ReadUInt32();

                br.ReadUInt16();

                br.BaseStream.Seek(directoryOffset, SeekOrigin.Begin);

                using (BinaryMemoryStream ms = new BinaryMemoryStream(br.ReadBytes((int)directorySize)))
                {
                    for (int i = 0; i < recordCount; i++)
                    {
                        zad.Contents.Add(new ZADEntry(ms));
                    }
                }

                /* The padding calculations below serve no real purpose, they're just used for debugging VTs */

                ZADEntry lastEntry = null;

                foreach (ZADEntry entry in zad.Contents.OrderBy(e => e.Offset))
                {
                    if (lastEntry != null)
                    {
                        lastEntry.Padding = entry.Offset - lastEntry.Offset - lastEntry.EntrySize;
                    }

                    lastEntry = entry;
                }

                lastEntry.Padding = (uint)(directoryOffset - lastEntry.Offset - lastEntry.EntrySize);
            }

            return zad;
        }

        public bool Contains(string file)
        {
            return Contents.Any(e => e.Name == file);
        }

        public void RemoveEntry(ZADEntry entry)
        {
            int indexOfEntry = Contents.IndexOf(entry);
            int oldContentsCount = Contents.Count;
            uint centralDirectoryOffset = CentralDirectoryOffset;
            //ZADEntry nextEntry = indexOfEntry == contents.Count ? null : contents[indexOfEntry + 1];
            Contents.Remove(entry);
            using (FileStream fs = new FileStream(Path.Combine(location, $"{name}{extension}"), FileMode.Open))
            {
                Logger.LogToFile(Logger.LogLevel.Info, "\tFile Size Before Removal:\t\t{0}", fs.Length);
                Logger.LogToFile(Logger.LogLevel.Info, "\tEntryOffset:\t\t\t\t\t\t{0}", entry.Offset);
                uint dataSize = entry.EntrySize + entry.Padding;
                Logger.LogToFile(Logger.LogLevel.Info, "\tEntrySize:\t\t\t\t\t\t{0}\n\tEntryPadding:\t\t\t\t\t{1}\n\tDataSize:\t\t\t\t\t\t{2}", entry.EntrySize, entry.Padding, dataSize);
                Logger.LogToFile(Logger.LogLevel.Info, "\tEntryOffset+DataSize:\t\t\t{0}\n\tNextEntryOffset:\t\t\t\t{1}", entry.Offset + dataSize, Contents[indexOfEntry + 1].Offset);

                uint nextEntryOffset = entry.Offset + dataSize;
                uint foundOffset = Contents[indexOfEntry + 1].Offset;
                ZADEntry foundEntry = null;
                int firstIndexAfterEntry = 0;

                for (int i = 1; i < Contents.Count; i++)
                {
                    if (Contents[i].Offset >= nextEntryOffset && Contents[i].Offset <= foundOffset)
                    {
                        foundOffset = Contents[i].Offset;
                        foundEntry = Contents[i];
                    }

                    if (Contents[i].Offset > entry.Offset)
                    {
                        if (firstIndexAfterEntry == 0) { firstIndexAfterEntry = i; }
                        Contents[i].Offset -= dataSize;
                    }
                }

                if (foundEntry != null) { Logger.LogToFile(Logger.LogLevel.Info, "\tFoundEntryOffset:\t\t\t{0}\n\tFoundEntryName:\t\t\t\t\t{1}\n\tFoundEntrySize:\t\t\t\t\t{2}", foundOffset, foundEntry.Name, foundEntry.EntrySize + foundEntry.Padding); }
                Logger.LogToFile(Logger.LogLevel.Info, "\tCalculatedNextEntryOffset:\t\t{0}", nextEntryOffset);
                fs.Seek(nextEntryOffset, SeekOrigin.Begin);
                byte[] data = new byte[fs.Length - nextEntryOffset];
                fs.Read(data, 0, (int)(fs.Length - nextEntryOffset));
                fs.Seek(entry.Offset, SeekOrigin.Begin);
                fs.SetLength(entry.Offset);
                fs.Write(data, 0, data.Length);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Seek((int)CentralDirectoryOffset, SeekOrigin.Begin);
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
            int indexOfEntry = Contents.IndexOf(entry);
            int oldContentsCount = Contents.Count;
            uint centralDirectoryOffset = CentralDirectoryOffset;

            using (FileStream fs = new FileStream(Path.Combine(location, $"{name}{extension}"), FileMode.Open))
            {
                uint dataSize = entry.EntrySize + entry.Padding;
                entry.Data = newData;
                entry.Padding = 0;

                if (Type == ZADType.VirtualTexture)
                {
                    while (entry.Data.Length > entry.Padding) { entry.Padding += 4096; }
                    entry.Padding -= (uint)entry.Data.Length;
                }

                uint newDataSize = entry.EntrySize + entry.Padding;
                uint nextEntryOffset = entry.Offset + dataSize;
                uint foundOffset = Contents[indexOfEntry + 1].Offset;
                ZADEntry foundEntry = null;
                int firstIndexAfterEntry = 0;

                for (int i = 1; i < Contents.Count; i++)
                {
                    if (indexOfEntry == i) { continue; }

                    if (Contents[i].Offset >= nextEntryOffset && Contents[i].Offset <= foundOffset)
                    {
                        foundOffset = Contents[i].Offset;
                        foundEntry = Contents[i];
                    }

                    if (Contents[i].Offset > entry.Offset)
                    {
                        if (firstIndexAfterEntry == 0) { firstIndexAfterEntry = i; }
                        Contents[i].Offset -= dataSize;
                        Contents[i].Offset += newDataSize;
                    }
                }

                fs.Seek(nextEntryOffset, SeekOrigin.Begin);
                byte[] data = new byte[fs.Length - nextEntryOffset];
                fs.Read(data, 0, (int)(fs.Length - nextEntryOffset));
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
                    bw.Seek((int)CentralDirectoryOffset, SeekOrigin.Begin);
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
                bw.Seek((int)CentralDirectoryOffset, SeekOrigin.Begin);

                ZADEntry entry = new ZADEntry()
                {
                    CompressionMethod = (type == ZADType.Archive ? CompressionMethods.Deflate : CompressionMethods.LZ4),
                    Name = name,
                    Offset = (uint)bw.BaseStream.Position,
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
                    entry.Padding -= (uint)entry.Data.Length;
                    bw.Write(new byte[entry.Padding]);
                }

                Contents.Add(entry);

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
            bw.Seek((int)CentralDirectoryOffset, SeekOrigin.Begin);

            ZADEntry directory = new ZADEntry()
            {
                Name = path.Replace(root, "").Replace("\\", "/") + "/",
                Offset = (uint)bw.BaseStream.Position
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
                Contents.Add(directory);
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
                    Offset = (uint)bw.BaseStream.Position,
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
                    entry.Padding -= (uint)entry.Data.Length;
                    bw.Write(new byte[entry.Padding]);
                }

                Contents.Add(entry);
            }
        }

        private void writeCentralDirectory(BinaryWriter bw)
        {
            int centralDirectorySize = (int)bw.BaseStream.Position;

            foreach (ZADEntry entry in Contents)
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
            bw.Write((short)Contents.Count);
            bw.Write((short)Contents.Count);
            bw.Write(centralDirectorySize);
            bw.Write(CentralDirectoryOffset);
            bw.Write((short)0);     // Comment length
        }

        public void Extract(ZADEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            file.Name = file.Name.Replace("/", "\\");

            string fullDestination = Path.Combine(destination, file.Name);

            if (file.SizeUncompressed == 0)
            {
                if (!Directory.Exists(fullDestination)) { Directory.CreateDirectory(fullDestination); }
            }
            else
            {
                string folder = Path.GetDirectoryName(fullDestination);
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }

                using (BinaryWriter bw = new BinaryWriter(new FileStream(fullDestination, FileMode.Create)))
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
                    bfs.Read(buff, 0, (int)file.SizeUncompressed);
                    break;

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
                        bfs.Read(buff, 0, (int)file.SizeUncompressed);
                    }
                    else
                    {
	                    var compressedData = bfs.ReadBytes((int)file.SizeCompressed);

						using (MemoryStream ms = new MemoryStream(compressedData))
                        using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                        {
	                        int offset = 0;
	                        int sizeLeft = (int)file.SizeUncompressed;
	                        
	                        while (sizeLeft > 0)
	                        {
		                        int bytesRead = ds.Read(buff, offset, sizeLeft);
		                        offset += bytesRead;
		                        sizeLeft = (int)file.SizeUncompressed - offset;
	                        }
                        }
                    }
                    break;

                case CompressionMethods.LZ4:
                    using (MemoryStream ms = new MemoryStream(bfs.ReadBytes((int)file.SizeCompressed)))
                    using (LZ4Decompress lz4 = new LZ4Decompress(ms))
                    {
                        lz4.Read(buff, 0, (int)file.SizeCompressed);
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

        public string Name { get; set; }

        public uint Offset { get; set; }

        public uint Padding { get; set; }

        public byte[] CRC { get; private set; } = new byte[] { 0, 0, 0, 0 };

        public byte[] Data
        {
            get => data;
            set
            {
                data = value;
                SizeUncompressed = (uint)data.Length;
                CRC = (new CRC32()).Hash(data);

                if (SizeUncompressed > 0)
                {
                    switch (CompressionMethod)
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
                    CompressionMethod = CompressionMethods.None;
                }

                SizeCompressed = (uint)data.Length;
            }
        }

        public uint SizeCompressed { get; private set; }
        public uint SizeUncompressed { get; private set; }

        public uint EntrySize
        {
            get
            {
                if (CompressionMethod == CompressionMethods.LZ4)
                {
                    return SizeCompressed;
                }
                else
                {
                    return (uint)(Name.Length + SizeCompressed + 0x1e);
                }
            }
        }

        public CompressionMethods CompressionMethod { get; set; }

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
            CompressionMethod = (CompressionMethods)br.ReadUInt16();
            br.ReadUInt16();    // file last modified time
            br.ReadUInt16();    // file last modified date
            br.ReadUInt32();    // crc32
            SizeCompressed = br.ReadUInt32();
            SizeUncompressed = br.ReadUInt32();
            int nameLength = br.ReadUInt16();
            int extraLength = br.ReadUInt16();
            br.ReadUInt16();    // file comment length
            br.ReadUInt32();    // disk number
            br.ReadUInt16();    // internal file attributes
            br.ReadUInt16();    // external file attributes
            Offset = br.ReadUInt32();
            Name = br.ReadString(nameLength);
            br.ReadBytes(extraLength);
        }
    }
}