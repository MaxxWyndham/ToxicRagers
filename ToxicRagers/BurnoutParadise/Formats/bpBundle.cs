using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using ToxicRagers.Helpers;

namespace ToxicRagers.BurnoutParadise.Formats
{
    public class BUNDLE
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string Extension { get; set; }

        public List<BUNDLEEntry> Contents { get; set; } = new List<BUNDLEEntry>();

        public int Version { get; set; }

        public int Size { get; private set; }

        public int Flags { get; set; }

        public static BUNDLE Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            BUNDLE bundle = new BUNDLE
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Extension = Path.GetExtension(path),
                Location = Path.GetDirectoryName(path)
            };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0x62 ||
                    br.ReadByte() != 0x6e ||
                    br.ReadByte() != 0x64 ||
                    br.ReadByte() != 0x32)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid BUNDLE file", path);
                    return null;
                }

                bundle.Version = br.ReadInt32();
                br.ReadInt32();
                bundle.Size = br.ReadInt32();
                int fileCount = br.ReadInt32();
                br.ReadInt32();         // table offset
                int headerOffset = br.ReadInt32();
                int bodyOffset = br.ReadInt32();
                br.ReadInt32();         // data end
                br.ReadInt32();
                bundle.Flags = br.ReadInt32();
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
                    using (FileStream fs = new FileStream(Path.Combine(Location, $"{Name}{Extension}"), FileMode.Open))
                    using (BinaryReader br = new BinaryReader(fs))
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
                    using (FileStream fs = new FileStream(Path.Combine(Location, $"{Name}{Extension}"), FileMode.Open))
                    using (BinaryReader br = new BinaryReader(fs))
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
        public string Name { get; set; }

        public int HeaderSize { get; set; }

        public int HeaderSizeCompressed { get; set; }

        public int DataSize { get; set; }

        public int DataSizeCompressed { get; set; }

        public int HeaderOffset { get; set; }

        public int DataOffset { get; set; }

        public int Type { get; set; }

        public int Count { get; set; }
    }
}