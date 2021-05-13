using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class TWT
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public List<TWTEntry> Contents { get; set; } = new List<TWTEntry>();

        public static TWT Create(string path)
        {
            TWT twt = new TWT
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Location = Path.GetDirectoryName(path)
            };

            twt.Save();

            return twt;
        }

        public static TWT Load(string path)
        {
            TWT twt;

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            {
                twt = Load(ms);
            }

            twt.Name = Path.GetFileNameWithoutExtension(path);
            twt.Location = Path.GetDirectoryName(path);

            return twt;
        }

        public static TWT Load(Stream stream)
        {
            TWT twt = new TWT();

            using (BinaryReader br = new BinaryReader(stream, Encoding.Default))
            {
                br.ReadInt32();     // length

                int fileCount = br.ReadInt32();

                for (int i = 0; i < fileCount; i++)
                {
                    twt.Contents.Add(new TWTEntry
                    {
                        Length = br.ReadInt32(),
                        Name = br.ReadString(0x34)
                    });
                }

                for (int i = 0; i < fileCount; i++)
                {
                    twt.Contents[i].Data = br.ReadBytes(twt.Contents[i].Length);

                    if (twt.Contents[i].Length % 4 > 0) { br.ReadBytes(4 - (twt.Contents[i].Length % 4)); }
                }
            }

            return twt;
        }

        public void Save()
        {
            Save(Path.Combine(Location, $"{Name}.twt"));
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(0);
                bw.Write(Contents.Count);

                foreach (TWTEntry entry in Contents)
                {
                    bw.Write(entry.Length);
                    bw.WriteString(entry.Name);
                    bw.Write((byte)0);
                    for (int i = entry.Name.Length + 1; i < 52; i++) { bw.Write((byte)205); }
                }

                foreach (TWTEntry entry in Contents)
                {
                    bw.Write(entry.Data);

                    if (entry.Length % 4 != 0)
                    {
                        for (int i = entry.Length % 4; i < 4; i++) { bw.Write((byte)205); }
                    }
                }

                bw.BaseStream.Seek(0, SeekOrigin.Begin);
                bw.Write((int)bw.BaseStream.Length);
            }
        }

        public byte[] Extract(TWTEntry entry)
        {
            return entry.Data;
        }

        public void Extract(TWTEntry entry, string destination)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(destination, entry.Name), FileMode.Create)))
            {
                bw.Write(entry.Data);
            }
        }

        public void Add(string path)
        {
            Contents.Add(TWTEntry.FromFile(path));

            Save();
        }
    }

    public class TWTEntry
    {
        public int Length { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }

        public static TWTEntry FromFile(string path)
        {
            return new TWTEntry
            {
                Name = Path.GetFileName(path),
                Length = (int)new FileInfo(path).Length,
                Data = File.ReadAllBytes(path)
            };
        }
    }
}
