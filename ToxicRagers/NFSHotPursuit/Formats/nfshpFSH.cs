using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.Core.Formats;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.NFSHotPursuit.Formats
{
    public class FSH
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public List<FSHRecord> Contents { get; set; }

        public FSH()
        {
            Contents = new List<FSHRecord>();
        }

        public static FSH Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            FileInfo fi = new FileInfo(path);
            FSH fsh = new FSH()
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Location = Path.GetDirectoryName(path)
            };

            using (BinaryReader br = new BinaryReader(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadByte() != 0x53 || // S
                    br.ReadByte() != 0x48 || // H
                    br.ReadByte() != 0x50 || // P
                    br.ReadByte() != 0x49)   // I
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid FSH file", path);
                    return null;
                }

                br.ReadUInt32();        // filesize
                int imageCount = (int)br.ReadUInt32();
                br.ReadString(4);       // tag

                for (int i = 0; i < imageCount; i++)
                {
                    FSHRecord fish = new FSHRecord
                    {
                        Tag = br.ReadString(4),
                        Offset = (int)br.ReadUInt32()
                    };

                    fsh.Contents.Add(fish);
                }
            }

            return fsh;
        }

        public void Extract(FSHRecord record, string destination)
        {
            if (!Directory.Exists(destination)) { Directory.CreateDirectory(destination); }

            FSHI fish = new FSHI();

            using (BinaryReader br = new BinaryReader(new FileStream(Path.Combine(Location, $"{Name}.fsh"), FileMode.Open)))
            {
                br.BaseStream.Seek(record.Offset, SeekOrigin.Begin);

                while (true)
                {
                    long position = br.BaseStream.Position;
                    byte section = br.ReadByte();
                    int offset = (br.ReadByte() << 0 | br.ReadByte() << 8 | br.ReadByte() << 16);
                    long nextSection = position + offset;

                    switch (section)
                    {
                        case 0x60: // DXT1
                        case 0x61: // DXT3
                            fish.Format = (FSHI.ImageFormat)section;

                            int dataSize;
                            fish.Width = (int)br.ReadUInt16();
                            fish.Height = (int)br.ReadUInt16();
                            br.ReadUInt16();
                            br.ReadUInt16();
                            br.ReadUInt16();
                            br.ReadUInt16();

                            switch (section)
                            {
                                case 0x60:
                                    dataSize = (((fish.Width + 3) / 4) * ((fish.Height + 3) / 4)) * 8;
                                    fish.Data = br.ReadBytes(dataSize);
                                    break;

                                case 0x61:
                                    dataSize = (((fish.Width + 3) / 4) * ((fish.Height + 3) / 4)) * 16;
                                    fish.Data = br.ReadBytes(dataSize);
                                    break;
                            }
                            break;

                        case 0x69:
                            br.ReadUInt16();
                            br.ReadUInt16();
                            br.ReadBytes(8);
                            br.ReadNullTerminatedString();
                            break;

                        case 0x70:
                            fish.Name = br.ReadNullTerminatedString();
                            fish.Save(Path.Combine(destination, fish.Name));
                            break;

                        default:
                            Console.WriteLine($"Unknown section: {section}");
                            return;
                    }

                    br.BaseStream.Seek(nextSection, SeekOrigin.Begin);

                    if (offset == 0) { return; }
                }
            }
        }
    }

    public class FSHRecord
    {
        public string Tag { get; set; }

        public int Offset { get; set; }
    }

    public class FSHI
    {
        public enum ImageFormat
        {
            DXT1 = 0x60,
            DXT3 = 0x61
        }

        public ImageFormat Format { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }

        public void Save(string path)
        {
            DDS dds = new DDS
            {
                Width = Width,
                Height = Height,
                Name = Name,
            };

            switch (Format)
            {
                case ImageFormat.DXT1:
                    dds.Format = D3DFormat.DXT1;
                    break;

                case ImageFormat.DXT3:
                    dds.Format = D3DFormat.DXT3;
                    break;
            }

            dds.MipMaps.Add(new MipMap
            {
                Width = Width,
                Height = Height
            });

            dds.MipMaps[0].Data = Data;

            dds.Save(path);
        }
    }
}