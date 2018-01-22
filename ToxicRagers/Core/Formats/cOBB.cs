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

        public string Name => name;
        public List<OBBEntry> Contents => contents;

        public OBB()
        {
            contents = new List<OBBEntry>();
        }

        public static OBB Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            OBB obb = new OBB()
            {
                name = Path.GetFileNameWithoutExtension(path),
                location = Path.GetDirectoryName(path) + "\\"
            };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                br.ReadUInt32();        // 0x01000000 OBB version?

                obb.entryCount = (int)br.ReadUInt32();

                for (int i = 0; i < obb.entryCount; i++)
                {
                    OBBEntry entry = new OBBEntry()
                    {
                        Name = br.ReadString((int)br.ReadUInt32()),
                        Offset = (int)br.ReadUInt32(),
                        Size = (int)br.ReadUInt32()
                    };

                    obb.Contents.Add(entry);
                }

                br.ReadUInt32();        // 0xFFFFFFFF Header terminator?
            }

            return obb;
        }

        public void Extract(OBBEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
            using (FileStream fs = new FileStream(location + name + ".obb", FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);

                byte[] buff = new byte[file.Size];
                fs.Read(buff, 0, file.Size);
                bw.Write(buff);
                buff = null;
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
            get => offset;
            set => offset = value;
        }

        public int Size
        {
            get => size;
            set => size = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }
    }
}