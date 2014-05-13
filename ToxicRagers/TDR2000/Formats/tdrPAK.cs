using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class PAK
    {
        string name;
        string location;
        List<PAKEntry> contents;

        public string Name { get { return name; } }
        public List<PAKEntry> Contents { get { return contents; } }

        public PAK()
        {
            contents = new List<PAKEntry>();
        }

        public static PAK Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            PAK pak = new PAK();

            pak.name = Path.GetFileNameWithoutExtension(path);
            pak.location = Path.GetDirectoryName(path) + "\\";

            char[] filename = new char[255];
            bool[] rollback = new bool[255];
            byte code = 0;
            int i = 0;
            string sBase = "";

            using (BinaryReader brDir = new BinaryReader(fi.OpenRead()))
            {
                i = 0;

                while (brDir.BaseStream.Position < brDir.BaseStream.Length)
                {
                    do
                    {
                        filename[i] = (char)brDir.ReadByte();
                        code = brDir.ReadByte();

                        if (code == 136) { sBase = new string(filename, 0, i); }
                        if (code == 192) { rollback[i] = true; }

                        i++;
                    }
                    while ((code & 8) != 8);

                    var entry = new PAKEntry();
                    entry.Name = new string(filename, 0, i);
                    entry.Offset = (int)brDir.ReadUInt32();
                    entry.Size = (int)brDir.ReadUInt32();
                    pak.contents.Add(entry);

                    switch (code)
                    {
                        case 72:
                            // do nothing
                            break;

                        case 136:
                            i = sBase.Length;
                            break;

                        case 200:
                            rollback[i - 1] = true;
                            break;

                        default:
                            i = Array.LastIndexOf(rollback, true);
                            if (i > -1) { rollback[i] = false; }
                            break;
                    }
                }
            }

            return pak;
        }

        public void Extract(PAKEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
            {
                using (FileStream fs = new FileStream(this.location + this.name + ".pak", FileMode.Open))
                {
                    fs.Seek(file.Offset, SeekOrigin.Begin);

                    if (file.Size >= 8)
                    {
                        if ((fs.ReadByte() ^ fs.ReadByte()) == 122 && (fs.ReadByte() ^ fs.ReadByte()) == 14)
                        {
                            int a, b, c, d;
                            a = fs.ReadByte();
                            b = fs.ReadByte();
                            c = fs.ReadByte();
                            d = fs.ReadByte();

                            a = 1 * (a ^ d);
                            b = 256 * (b ^ d);
                            c = 65536 * (c ^ d);
                            d = 16777216 * (d ^ d);
                            file.Size = (a + b + c + d);

                            // Compressed!
                            fs.Seek(2, SeekOrigin.Current);
                            byte[] buff = new byte[file.Size];

                            using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
                            {
                                ds.Read(buff, 0, file.Size);
                                bw.Write(buff);
                                buff = null;
                            }
                        }
                        else
                        {
                            // Uncompressed!
                            fs.Seek(file.Offset, SeekOrigin.Begin);
                            byte[] buff = new byte[file.Size];
                            fs.Read(buff, 0, file.Size);
                            bw.Write(buff);
                            buff = null;
                        }
                    }
                }
            }
        }
    }

    public class PAKEntry
    {
        string name;
        int offset;
        int size;

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

        public int Size
        {
            get { return size; }
            set { size = value; }
        }
    }
}
