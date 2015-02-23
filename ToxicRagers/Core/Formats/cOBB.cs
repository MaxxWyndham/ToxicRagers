using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    // Android Opaque Binary Blob

    public class OBB
    {
        string name;
        string location;
        int entryCount;
        List<OBBEntry> contents;

        public string Name { get { return name; } }
        public List<OBBEntry> Contents { get { return contents; } }

        public OBB()
        {
            contents = new List<OBBEntry>();
        }

        public static OBB Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            OBB obb = new OBB();

            obb.name = Path.GetFileNameWithoutExtension(path);
            obb.location = Path.GetDirectoryName(path) + "\\";

            using (var br = new BinaryReader(fi.OpenRead()))
            {
                br.ReadUInt32();        // 0x01000000 OBB version?

                obb.entryCount = (int)br.ReadUInt32();

                for (int i = 0; i < obb.entryCount; i++)
                {
                    OBBEntry entry = new OBBEntry();

                    entry.Name = br.ReadString((int)br.ReadUInt32());
                    entry.Offset = (int)br.ReadUInt32();
                    entry.Size = (int)br.ReadUInt32();

                    obb.Contents.Add(entry);
                }

                br.ReadUInt32();        // 0xFFFFFFFF Header terminator?
            }

            return obb;
        }

        public void Extract(OBBEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (var bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
            {
                using (var fs = new FileStream(this.location + this.name + ".obb", FileMode.Open))
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

    public class OBBEntry
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
