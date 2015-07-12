using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    // WAD file used by some Stainless games

    public class WAD
    {
        string name;
        string location;
        int entryCount;
        List<WADEntry> contents;

        Version version;
        int flags;

        public string Name { get { return name; } }
        public List<WADEntry> Contents { get { return contents; } }

        public WAD()
        {
            contents = new List<WADEntry>();
        }

        public static WAD Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            WAD wad = new WAD();

            wad.name = Path.GetFileNameWithoutExtension(path);
            wad.location = Path.GetDirectoryName(path) + "\\";

            using (var br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0x34 ||
                    br.ReadByte() != 0x12)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid WAD file", path);
                    return null;
                }

                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                wad.version = new Version(major, minor);
                wad.flags = (int)br.ReadUInt32();

                int xmlLength = (int)br.ReadUInt32();
                if (xmlLength > 0)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Unexpected data discovered.  Aborting");
                    return null;
                }

                int namesLength = (int)br.ReadUInt32();
                int endOfNamesBlock = (int)br.BaseStream.Position + namesLength;

                while (br.BaseStream.Position < endOfNamesBlock)
                {
                    var entry = new WADEntry();
                    entry.Name = br.ReadString();
                    wad.contents.Add(entry);
                }
            }

            return wad;
        }

        public void Extract(WADEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (var bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
            {
                using (var fs = new FileStream(this.location + this.name + ".wad", FileMode.Open))
                {
                    fs.Seek(file.Offset, SeekOrigin.Begin);

                    var buff = new byte[file.Size];
                    fs.Read(buff, 0, file.Size);
                    bw.Write(buff);
                    buff = null;
                }
            }
        }
    }

    public class WADEntry
    {
        string name;
        int offset;
        int size;

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

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
