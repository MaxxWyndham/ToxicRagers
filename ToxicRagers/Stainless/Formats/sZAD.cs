using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public enum ZADType
    {
        Archive,
        VirtualTexture
    }

    public class ZAD
    {
        string name;
        string location;
        int entryCount;
        List<ZADEntry> contents;
        ZADType type = ZADType.Archive;

        Version version;
        int flags;

        public string Name { get { return name; } }
        public List<ZADEntry> Contents { get { return contents; } }
        public bool IsVT { get { return type == ZADType.VirtualTexture; } }

        public int CentralDirectoryOffset
        {
            get { return contents.Sum(e => e.EntrySize); }
        }

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
            ZAD zad = new ZAD(Path.GetFileNameWithoutExtension(path));
            zad.location = Path.GetDirectoryName(path);

            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                zad.WriteCentralDirectory(bw);
            }

            return zad;
        }

        public static ZAD Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            ZAD zad = new ZAD();

            zad.name = Path.GetFileNameWithoutExtension(path);
            zad.location = Path.GetDirectoryName(path) + "\\";

            using (var br = new BinaryReader(fi.OpenRead()))
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

                using (var ms = new BinaryMemoryStream(br.ReadBytes(directorySize)))
                {
                    for (int i = 0; i < recordCount; i++)
                    {
                        zad.contents.Add(new ZADEntry(ms));
                    }
                }
            }

            return zad;
        }

        public void AddDirectory(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(this.location, this.name + ".zad"), FileMode.Open)))
            {
                AddDirectory(bw, path, path.Substring(0, path.IndexOf(Path.GetFileName(path))));

                this.WriteCentralDirectory(bw);
            }
        }

        private void AddDirectory(BinaryWriter bw, string path, string root)
        {
            bw.Seek(this.CentralDirectoryOffset, SeekOrigin.Begin);

            ZADEntry directory = new ZADEntry();
            directory.Name = path.Replace(root, "").Replace("\\", "/") + "/";

            // Write entry
            directory.Offset = (int)bw.BaseStream.Position;
            bw.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04 });
            bw.Write((short)0x14);  // Version needed to extract
            bw.Write((short)0);     // Flags
            bw.Write((short)0);     // Compression method (0 == stored)
            bw.Write(0);            // DateTime
            bw.Write(0);            // CRC
            bw.Write(0);            // Compressed filesize
            bw.Write(0);            // Uncompressed filesize
            bw.Write(directory.Name.Length);
            bw.Write(directory.Name.ToCharArray());
            this.contents.Add(directory);

            foreach (var folder in Directory.GetDirectories(path))
            {
                AddDirectory(bw, folder, root);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                if (string.Compare(Path.GetExtension(file), ".zad", true) == 0) { continue; }

                ZADEntry entry = new ZADEntry();
                entry.Data = File.ReadAllBytes(file);
                entry.Offset = (int)bw.BaseStream.Position;
                entry.Name = directory.Name + Path.GetFileName(file);

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
                bw.Write(entry.Data);

                this.contents.Add(entry);
            }
        }

        private void WriteCentralDirectory(BinaryWriter bw)
        {
            int centralDirectorySize = (int)bw.BaseStream.Position;

            foreach (ZADEntry entry in contents)
            {
                // Write end of dictionary
                bw.Write(new byte[] { 0x50, 0x4b, 0x01, 0x02 });
                bw.Write((byte)0x3f);   // Version made by
                bw.Write((byte)0);      // Host OS
                bw.Write((byte)0x14);   // Minimum version to extract
                bw.Write((byte)0);      // Target OS
                bw.Write((short)0);     // Flags
                bw.Write((short)entry.CompressionMethod);
                bw.Write(0);            // DateTime
                bw.Write(entry.CRC);
                bw.Write(entry.SizeCompressed);
                bw.Write(entry.SizeUncompressed);
                bw.Write((short)entry.Name.Length);
                bw.Write((short)0);     // Extra field length
                bw.Write((short)0);     // Comment length
                bw.Write((short)0);     // Disk number
                bw.Write((short)0);     // Internal file attributes
                bw.Write(0x20);         // External file attributes
                bw.Write(entry.Offset); // Relative offset from start
                bw.Write(entry.Name.ToCharArray());
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

                using (var bw = new BinaryWriter(new FileStream(destination + file.Name, FileMode.Create)))
                {
                    using (var bfs = new BinaryFileStream(this.location + this.name + ".zad", FileMode.Open))
                    {
                        bfs.Seek(file.Offset, SeekOrigin.Begin);

                        var buff = new byte[file.SizeUncompressed];

                        switch (file.CompressionMethod)
                        {
                            case 0: // no compression
                            case 8: // deflate
                                if (bfs.ReadByte() != 0x50 ||
                                    bfs.ReadByte() != 0x4B ||
                                    bfs.ReadByte() != 0x03 ||
                                    bfs.ReadByte() != 0x04)
                                {
                                    Logger.LogToFile(Logger.LogLevel.Error, "{0:x2} has a malformed header", (bfs.Position - 4));
                                    return;
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

                                if (file.CompressionMethod == 0)
                                {
                                    bfs.Read(buff, 0, file.SizeUncompressed);
                                }
                                else
                                {
                                    using (var ms = new MemoryStream(bfs.ReadBytes(file.SizeCompressed)))
                                    using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                                    {
                                        ds.Read(buff, 0, file.SizeUncompressed);
                                    }
                                }
                                break;

                            case 56:
                                using (var ms = new MemoryStream(bfs.ReadBytes(file.SizeCompressed)))
                                using (var lz4 = new LZ4Decompress(ms))
                                {
                                    lz4.Read(buff, 0, file.SizeUncompressed);
                                }
                                break;

                            default:
                                throw new NotImplementedException(string.Format("Unknown CompressionMethod: {0}", file.CompressionMethod));
                        }

                        bw.Write(buff);
                        buff = null;
                    }
                }
            }
        }
    }

    public class ZADEntry
    {
        byte[] data;

        byte[] crc = new byte[] { 0, 0, 0, 0 };
        int sizeCompressed;
        int sizeUncompressed;
        int offset;
        string name;
        int compressionMethod;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public byte[] CRC
        {
            get { return crc; }
        }

        public byte[] Data
        {
            get { return data; }
            set
            {
                data = value;
                sizeUncompressed = data.Length;
                crc = (new CRC32()).Hash(data);

                if (sizeUncompressed > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
                    {
                        ds.Write(data, 0, data.Length);
                        ds.Flush();
                        ds.Close();

                        data = ms.ToArray();
                    }

                    compressionMethod = 8;
                }

                sizeCompressed = data.Length;
            }
        }

        public int SizeCompressed
        {
            get { return sizeCompressed; }
        }

        public int SizeUncompressed
        {
            get { return sizeUncompressed; }
        }

        public int EntrySize
        {
            get { return name.Length + sizeCompressed + 0x1e; }
        }

        public int CompressionMethod
        {
            get { return compressionMethod; }
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

            br.ReadUInt16();    // version made by
            br.ReadUInt16();    // version needed to extract    
            int flags = br.ReadUInt16();
            compressionMethod = br.ReadUInt16();
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
