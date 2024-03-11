using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.UltimateRacePro.Formats
{
    public class BankFile
    {
        private Version version;
        private int flags;

        public string Name { get; set; }
        public string Location { get; set; }
        public List<BankEntry> Contents { get; set; }

        public BankFile()
        {
            Contents = new List<BankEntry>();
        }

        public static BankFile Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            BankFile bank = new BankFile()
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Location = Path.GetDirectoryName(path)
            };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadNullTerminatedString() != "Bank file v1.13")
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid bank file", path);
                    return null;
                }

                br.BaseStream.Seek(0x100, SeekOrigin.Begin);

                uint fileCount = br.ReadUInt32();

                for (int i = 0; i < fileCount; i++)
                {
                    BankEntry entry = new BankEntry
                    {
                    };

                    br.ReadBytes(0x100);

                    bank.Contents.Add(entry);
                }

                long dataStart = br.BaseStream.Position;

                br.BaseStream.Seek(-4, SeekOrigin.End);

                uint count = br.ReadUInt32();

                br.BaseStream.Seek(dataStart, SeekOrigin.Begin);

                for (int i = 0; i < count; i++)
                {
                    uint length = br.ReadUInt32();

                    //Console.WriteLine($"{length}");

                    File.WriteAllBytes($@"D:\ComTest\output\{i}", br.ReadBytes((int)length));
                }

                Console.WriteLine($"{br.BaseStream.Position:x2} :: {br.BaseStream.Length:x2}");
            }

            return bank;
        }

        public void Extract(BankEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(destination, file.Name), FileMode.Create)))
            using (FileStream fs = new FileStream(Path.Combine(Location, $"{Name}.viv"), FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);

                byte[] buff = new byte[file.Size];
                fs.Read(buff, 0, file.Size);
                bw.Write(buff);
                buff = null;
            }
        }
    }

    public class BankEntry
    {
        public int Offset { get; set; }

        public int Size { get; set; }

        public string Name { get; set; }
    }
}
