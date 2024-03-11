using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.NFSHotPursuit.Formats
{
    public class VIV
    {
        private Version version;
        private int flags;

        public string Name { get; set; }
        public string Location { get; set; }
        public List<VIVEntry> Contents { get; set; }

        public VIV()
        {
            Contents = new List<VIVEntry>();
        }

        public static VIV Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            VIV viv = new VIV()
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Location = Path.GetDirectoryName(path) + "\\"
            };

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0x42 || // B
                    br.ReadByte() != 0x49 || // I
                    br.ReadByte() != 0x47 || // G
                    br.ReadByte() != 0x46)   // F
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid VIV file", path);
                    return null;
                }

                int size = (int)br.ReadUInt32();
                int fileCount = (int)br.ReadUInt32();
                int headerSize = (int)br.ReadUInt32();

                for (int i = 0; i < fileCount; i++)
                {
                    VIVEntry entry = new VIVEntry
                    {
                        Offset = (int)br.ReadUInt32(),
                        Size = (int)br.ReadUInt32(),
                        Name = br.ReadNullTerminatedString()
                    };

                    viv.Contents.Add(entry);
                }
            }

            return viv;
        }

        public void Extract(VIVEntry file, string destination)
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

    public class VIVEntry
    {
        public int Offset { get; set; }

        public int Size { get; set; }

        public string Name { get; set; }
    }
}
