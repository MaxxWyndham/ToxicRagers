using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Core.Formats;
using ToxicRagers.Helpers;

namespace ToxicRagers.DoubleStealSecondClash.Formats
{
    public class XPR
    {
        public enum RecordType : uint
        {
            Texture = 262145,
            Model = 2147483648,
            EOF = 4294967295
        }

        int size;
        int dataOffset;
        string name;
        string location;
        List<XPREntry> contents;

        public string Name { get { return name; } }
        public List<XPREntry> Contents { get { return contents; } }

        public XPR()
        {
            contents = new List<XPREntry>();
        }

        public static XPR Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            XPR xpr = new XPR();

            xpr.name = Path.GetFileNameWithoutExtension(path);
            xpr.location = Path.GetDirectoryName(path) + "\\";

            using (var br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0x58 ||    // X
                    br.ReadByte() != 0x50 ||    // P
                    br.ReadByte() != 0x52 ||    // R
                    br.ReadByte() != 0x30)      // 0
                {
                    Logger.LogToFile("{0} isn't a valid XPR file", path);
                    return null;
                }

                xpr.size = (int)br.ReadInt32();
                xpr.dataOffset = (int)br.ReadInt32();

                bool bLoop = true;

                while (bLoop)
                {
                    RecordType type = (RecordType)br.ReadUInt32();

                    switch (type)
                    {
                        case RecordType.Texture:
                            int offset = (int)br.ReadInt32();
                            br.ReadInt32(); // 0x0
                            int settings = (int)br.ReadInt32();
                            br.ReadInt32(); // 0x0

                            xpr.contents.Add(
                                new XPREntry
                                {
                                    Type = type,
                                    Name = offset.ToString("00000000"),
                                    Offset = xpr.dataOffset + offset,
                                    Flags = settings
                                }
                            );
                            break;

                        case RecordType.Model:

                            break;

                        case RecordType.EOF:
                            bLoop = false;
                            break;

                        default:
                            throw new NotImplementedException(string.Format("Unknown type: {0}", type));
                    }
                }

                br.BaseStream.Seek(xpr.dataOffset, SeekOrigin.Begin);
            }

            return xpr;
        }

        public void Extract(XPREntry file, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            using (var fs = new FileStream(this.location + this.name + ".xpr", FileMode.Open))
            {
                fs.Seek(file.Offset, SeekOrigin.Begin);

                switch (file.Type)
                {
                    case RecordType.Texture:
                        ExtractTexture(fs, destination + "\\" + file.Name, file.Flags);
                        break;

                        //using (var bw = new BinaryWriter(new FileStream(destination + "\\" + file.Name, FileMode.Create)))
                        //{

                        //}
                }
            }
        }

        public void ExtractTexture(FileStream fs, string path, int Flags)
        {
            DDS dds = new DDS();
            int[] dimensionLookup = new int[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 };

            int size = 0;
            int u = (Flags & 0x00F00000) >> 20;
            int v = (Flags & 0x0F000000) >> 24;
            int f = (Flags & 0x0000FF00) >> 8;

            switch (f)
            {
                case 12: // DXT1
                    size = (dimensionLookup[u] * dimensionLookup[v]) / 2;
                    dds.Format = D3DFormat.DXT1;
                    break;

                case 15: // DXT5
                    size = dimensionLookup[u] * dimensionLookup[v];
                    dds.Format = D3DFormat.DXT5;
                    break;

                default:
                    throw new NotImplementedException(string.Format("Unknown texture format: {0}", f));
            }

            dds.Width = dimensionLookup[u];
            dds.Height = dimensionLookup[v];
            dds.Data = new byte[size];
            fs.Read(dds.Data, 0, size);

            dds.Save(path);
        }
    }

    public class XPREntry
    {
        string name;
        XPR.RecordType type;
        int offset;
        int flags;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public XPR.RecordType Type
        {
            get { return type; }
            set { type = value; }
        }

        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public int Flags
        {
            get { return flags; }
            set { flags = value; }
        }
    }
}
