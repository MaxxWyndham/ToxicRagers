using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using ToxicRagers.Helpers;

namespace ToxicRagers.BurnoutParadise.Formats
{
    public class BUNDLE
    {
        string name;
        string extension;
        string location;
        int version;
        int size;
        int flags;

        List<BUNDLEEntry> contents;

        public string Name => name;
        public string Extension => extension;
        public List<BUNDLEEntry> Contents => contents;

        public BUNDLE()
        {
            contents = new List<BUNDLEEntry>();
        }

        public static BUNDLE Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            BUNDLE bundle = new BUNDLE()
            {
                name = Path.GetFileNameWithoutExtension(path),
                extension = Path.GetExtension(path),
                location = Path.GetDirectoryName(path) + "\\"
            };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 98 ||
                    br.ReadByte() != 110 ||
                    br.ReadByte() != 100 ||
                    br.ReadByte() != 50)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid BUNDLE file", path);
                    return null;
                }

                bundle.version = (int)br.ReadInt32();
                br.ReadInt32();
                bundle.size = (int)br.ReadInt32();
                int fileCount = (int)br.ReadInt32();
                int tableOffset = (int)br.ReadInt32();
                int headerOffset = (int)br.ReadInt32();
                int bodyOffset = (int)br.ReadInt32();
                int dataEnd = (int)br.ReadInt32();
                br.ReadInt32();
                bundle.flags = (int)br.ReadInt32();
                br.ReadInt32();

                for (int i = 0; i < fileCount; i++)
                {
                    BUNDLEEntry entry = new BUNDLEEntry { Name = i.ToString("00000") };

                    Console.WriteLine("{0}:", entry.Name);
                    br.ReadInt32(); // CRC
                    Console.WriteLine("{0} == 0", br.ReadInt32());
                    br.ReadInt32(); // LinkID
                    Console.WriteLine("{0} == 0", br.ReadInt32());
                    entry.HeaderSize = ((int)br.ReadInt32() << 8) >> 8;
                    entry.DataSize = ((int)br.ReadInt32() << 8) >> 8;
                    Console.WriteLine("{0} == 0", br.ReadInt32());
                    entry.HeaderSizeCompressed = (int)br.ReadInt32();
                    entry.DataSizeCompressed = (int)br.ReadInt32();
                    Console.WriteLine("{0} == 0", br.ReadInt32());
                    entry.HeaderOffset = headerOffset + (int)br.ReadInt32();
                    entry.DataOffset = bodyOffset + (int)br.ReadInt32();
                    Console.WriteLine("{0} == 0", br.ReadInt32());
                    Console.WriteLine("{0} == 0", br.ReadInt32());
                    entry.Type = br.ReadInt16();
                    entry.Count = br.ReadInt16();
                    Console.WriteLine("{0} == 0", br.ReadInt32());

                    bundle.Contents.Add(entry);
                }
            }

            return bundle;
        }

        public void Extract(BUNDLEEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (MemoryStream msOutput = new MemoryStream())
            {
                if (file.HeaderSize > 0)
                {
                    using (BinaryReader br = new BinaryReader(new FileStream(location + name + extension, FileMode.Open)))
                    {
                        br.BaseStream.Seek(file.HeaderOffset + 2, SeekOrigin.Begin);

                        using (MemoryStream ms = new MemoryStream(br.ReadBytes(file.HeaderSizeCompressed - 2)))
                        using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                        {
                            byte[] data = new byte[file.HeaderSize];
                            ds.Read(data, 0, file.HeaderSize);
                            msOutput.Write(data, 0, data.Length);
                        }
                    }
                }

                if (file.DataSize > 0)
                {
                    using (BinaryReader br = new BinaryReader(new FileStream(location + name + extension, FileMode.Open)))
                    {
                        br.BaseStream.Seek(file.DataOffset + 2, SeekOrigin.Begin);

                        using (MemoryStream ms = new MemoryStream(br.ReadBytes(file.DataSizeCompressed - 2)))
                        using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                        {
                            byte[] data = new byte[file.DataSize];
                            ds.Read(data, 0, file.DataSize);
                            msOutput.Write(data, 0, data.Length);
                        }
                    }
                }

                Console.WriteLine("{0}\t{1}\t{2}\t{3}", file.Type, file.Name, file.HeaderSize, file.DataSize);
                msOutput.Flush();
                msOutput.WriteTo(new FileStream(destination + "\\" + file.Type + "-" + file.Name, FileMode.Create));
            }
        }
    }

    public class BUNDLEEntry
    {
        string name;

        int type;
        int count;
        int headerSize;
        int dataSize;
        int headerSizeCompressed;
        int dataSizeCompressed;
        int headerOffset;
        int dataOffset;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int HeaderSize
        {
            get => headerSize;
            set => headerSize = value;
        }

        public int HeaderSizeCompressed
        {
            get => headerSizeCompressed;
            set => headerSizeCompressed = value;
        }

        public int DataSize
        {
            get => dataSize;
            set => dataSize = value;
        }

        public int DataSizeCompressed
        {
            get => dataSizeCompressed;
            set => dataSizeCompressed = value;
        }

        public int HeaderOffset
        {
            get => headerOffset;
            set => headerOffset = value;
        }

        public int DataOffset
        {
            get => dataOffset;
            set => dataOffset = value;
        }

        public int Type
        {
            get => type;
            set => type = value;
        }

        public int Count
        {
            get => count;
            set => count = value;
        }
    }
}