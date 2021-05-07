using System;
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

        int dataStart;

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
            TWT twt = new TWT
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Location = Path.GetDirectoryName(path)
            };

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs, Encoding.Default))
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

                twt.dataStart = (int)br.BaseStream.Position;
            }

            return twt;
        }

        public void Save()
        {
            using (FileStream fs = new FileStream(Path.Combine(Location, $"{Name}.twt"), FileMode.Create))
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
                    bw.Write(File.ReadAllBytes(entry.SourceFile));

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
            using (FileStream fs = new FileStream(Path.Combine(Location, $"{Name}.twt"), FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                int offset = dataStart;

                for (int i = 0; i < Contents.Count; i++)
                {
                    if (Contents[i] == entry)
                    {
                        break;
                    }
                    else
                    {
                        offset += entry.Length;

                        if (entry.Length % 4 > 0) { offset += 4 - (entry.Length % 4); }
                    }
                }

                br.BaseStream.Seek(offset, SeekOrigin.Begin);

                return br.ReadBytes(entry.Length);
            }
        }

        public void Extract(TWTEntry entry, string destination)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(destination, entry.Name), FileMode.Create)))
            {
                bw.Write(Extract(entry));
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

        public string SourceFile { get; set; }

        public static TWTEntry FromFile(string path)
        {
            return new TWTEntry
            {
                Name = Path.GetFileName(path),
                Length = (int)new FileInfo(path).Length,
                SourceFile = path
            };
        }
    }
}
