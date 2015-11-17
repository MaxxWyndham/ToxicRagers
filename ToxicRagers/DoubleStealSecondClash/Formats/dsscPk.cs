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

        public string Name { get { return name; } }
        public List<PKEntry> Contents { get { return contents; } }

        public PK()
        {
            contents = new List<PKEntry>();
        }

        public static PK Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            PK pk = new PK();

            pk.name = Path.GetFileNameWithoutExtension(path);
            pk.location = Path.GetDirectoryName(path) + "\\";

            using (BinaryReader brDir = new BinaryReader(fi.OpenRead()))
            {
                while (true)
                {
                    string name = brDir.ReadString(20);

                    if (name == "") { break; }

                    pk.contents.Add(
                        new PKEntry {
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

            using (var bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
            {
                using (var fs = new FileStream(this.location + this.name + ".pk", FileMode.Open))
                {
                    fs.Seek(file.Offset, SeekOrigin.Begin);

                    if (file.Unknown != 0)
                    {
                        using (var ds = new LSZZDecompress(fs))
                        {
                            var buff = new byte[file.Unknown];
                            ds.Read(buff, 0, file.Size);
                            bw.Write(buff);
                            buff = null;
                        }
                    }
                    else
                    {
                        var buff = new byte[file.Size];
                        fs.Read(buff, 0, file.Size);
                        bw.Write(buff);
                        buff = null;
                    }
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
            get { return name; }
            set { name = value; }
        }

        public int Unknown
        {
            get { return unknown; }
            set { unknown = value; }
        }

        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }
    }
}
