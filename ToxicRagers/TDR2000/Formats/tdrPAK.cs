using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class PAK
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public List<PAKEntry> Contents { get; set; } = new List<PAKEntry>();

        public static PAK Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            PAK pak = new PAK
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Location = Path.GetDirectoryName(path)
            };

            char[] filename = new char[255];
            bool[] rollback = new bool[255];
            byte code;
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

                    PAKEntry entry = new PAKEntry()
                    {
                        Name = new string(filename, 0, i),
                        Offset = (int)brDir.ReadUInt32(),
                        Size = (int)brDir.ReadUInt32()
                    };
                    pak.Contents.Add(entry);

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

            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path.Combine(destination, file.Name), FileMode.Create)))
            using (FileStream fs = new FileStream(Path.Combine(Location, $"{Name}.pak"), FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);
                Console.WriteLine($"\tExtracting {file.Name}");
                if (file.Size >= 8)
                {
                    if ((fs.ReadByte() ^ fs.ReadByte()) == 122 && (fs.ReadByte() ^ fs.ReadByte()) == 14)
                    {
                        int a, b, c, d;
                        a = fs.ReadByte();
                        b = fs.ReadByte();
                        c = fs.ReadByte();
                        d = fs.ReadByte();
                        Console.WriteLine($"\t\t{a} - {b} - {c} - {d}");

                        a = 1 * (a ^ d);
                        b = 256 * (b ^ d);
                        c = 65536 * (c ^ d);
                        d = 16777216 * (d ^ d);
                        file.Size = a + b + c + d;

                        // Compressed!
                        //fs.Seek(2, SeekOrigin.Current);
                        Console.WriteLine($"\t\t{fs.ReadByte()} - {fs.ReadByte()}");
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
                    }
                }
            }
        }
    }

    public class PAKEntry
    {
        public string Name { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }
    }
}