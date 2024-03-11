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
            using (Stream stream = fi.OpenRead())
            {
                return Load(stream, Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path) + "\\");
            }
        }

        public static OBB Load(Stream stream, string name, string location)
        {
            OBB obb = new()
            {
                name = name,
                location = location
            };

            using (BinaryReader br = new BinaryReader(stream))
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

        public void Save(string path)
        {
            FileInfo fi = new FileInfo(path);

            using (BinaryWriter writer = new BinaryWriter(fi.OpenWrite()))
            {

                writer.Write(1); // Flags?
                writer.Write(Contents.Count); // NumFiles?
                foreach (var entry in Contents)
                {
                    writer.Write(entry.Name.Length);
                    writer.Write(entry.Name.ToCharArray());
                    writer.Write(0); //Offset - fill this in later!
                    writer.Write(0); //Size - also fill in later

                }
                writer.Write(new byte[] { 255, 255, 255, 255 });   // padding?
                foreach (OBBEntry entry in Contents)
                {
                    entry.Offset = (int)writer.BaseStream.Position;
                    byte[] data = entry.GetData();
                    entry.Size = data.Length;
                    writer.Write(data);
                }

                // We have the size and offsets of the entries now, so go back and fill them in
                writer.Seek(8, SeekOrigin.Begin);
                foreach (var entry in Contents)
                {
                    // might as well write the name again to save faffing with seeking - Lazy Trent
                    writer.Write(entry.Name.Length);
                    writer.Write(entry.Name.ToCharArray());
                    writer.Write(entry.Offset);
                    writer.Write(entry.Size);

                }
            }
        }

        public void Extract(OBBEntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
            using (FileStream fs = new FileStream(location + name + (name.EndsWith(".obb") == false ? ".obb" : ""), FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);

                byte[] buff = new byte[file.Size];
                fs.Read(buff, 0, file.Size);
                bw.Write(buff);
                buff = null;
            }
        }

        public byte[] ExtractToStream(OBBEntry file)
        {

            using (FileStream fs = new FileStream(location + name + (name.EndsWith(".obb") == false ? ".obb" : ""), FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);

                byte[] buff = new byte[file.Size];
                fs.Read(buff, 0, file.Size);
                return buff;
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
        public virtual byte[] GetData()
        {
            throw new NotImplementedException();
        }
    }

    public class OBBEntry_File : OBBEntry
    {
        public string Path { get; set; }

        public override byte[] GetData()
        {
            return File.ReadAllBytes(Path);
        }
    }

    public class OBBEntry_Bytes : OBBEntry
    {
        public byte[] Data { get; set; }

        public override byte[] GetData()
        {
            return Data;
        }
    }
}