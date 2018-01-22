using System.Collections.Generic;
using System.IO;

using ToxicRagers.Compression.LZSS;
using ToxicRagers.Helpers;

namespace ToxicRagers.DoubleStealSecondClash.Formats
{
    public class PK
    {
        string name;
        string location;
        List<PKEntry> contents;

        public string Name => name;
        public List<PKEntry> Contents => contents;

        public PK()
        {
            contents = new List<PKEntry>();
        }

        public static PK Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);

            PK pk = new PK()
            {
                name = Path.GetFileNameWithoutExtension(path),
                location = Path.GetDirectoryName(path) + "\\"
            };

            using (BinaryReader brDir = new BinaryReader(fi.OpenRead()))
            {
                while (true)
                {
                    string name = brDir.ReadString(20);

                    if (name == "") { break; }

                    pk.contents.Add(
                        new PKEntry
                        {
                            Name = name,
                            Unknown = brDir.ReadInt32(),
                            Offset = brDir.ReadInt32(),
                            Size = brDir.ReadInt32()
                        }
                    );
                }
            }

            return pk;
        }

        public void Extract(PKEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
            using (FileStream fs = new FileStream(location + name + ".pk", FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);

                if (file.Unknown != 0)
                {
                    using (LSZZDecompress ds = new LSZZDecompress(fs))
                    {
                        byte[] buff = new byte[file.Unknown];
                        ds.Read(buff, 0, file.Size);
                        bw.Write(buff);
                        buff = null;
                    }
                }
                else
                {
                    byte[] buff = new byte[file.Size];
                    fs.Read(buff, 0, file.Size);
                    bw.Write(buff);
                    buff = null;
                }
            }
        }
    }

    public class PKEntry
    {
        string name;
        int unknown;
        int offset;
        int size;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Unknown
        {
            get => unknown;
            set => unknown = value;
        }

        public int Offset
        {
            get => offset;
            set => offset = value;
        }

        public int Size
        {
            get => size;
            set => size = value;
        }
    }
}