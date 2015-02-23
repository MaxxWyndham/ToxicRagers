using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public ZAD()
        {
            contents = new List<ZADEntry>();
        }

        public static ZAD Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
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
                    Logger.LogToFile("{0} isn't a valid ZAD file", path);
                    return null;
                }

                br.ReadUInt32();

                int recordCount = br.ReadUInt16();
                if (recordCount != br.ReadUInt16())
                {
                    Logger.LogToFile("Something has gone horribly wrong!  Aborting");
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
                                    Logger.LogToFile("{0} has a malformed header", (bfs.Position - 4).ToString("X"));
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
        }

        public int SizeCompressed
        {
            get { return sizeCompressed; }
        }

        public int SizeUncompressed
        {
            get { return sizeUncompressed; }
        }

        public int CompressionMethod
        {
            get { return compressionMethod; }
        }

        public ZADEntry(BinaryMemoryStream br)
        {
            if (br.ReadByte() != 0x50 ||
                br.ReadByte() != 0x4B ||
                br.ReadByte() != 0x01 ||
                br.ReadByte() != 0x02)
            {
                Logger.LogToFile("{0} isn't a valid ZAD entry", (br.Position - 4).ToString("X"));
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
