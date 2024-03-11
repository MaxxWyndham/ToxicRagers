using System.Collections.Generic;
using System.IO;
using ToxicRagers.Compression.LZSS;
using ToxicRagers.Helpers;
using static unluac.decompile.expression.TableLiteral;

namespace ToxicRagers.DestructionDerbyRaw.Formats
{
    public class PTH
    {
        public List<PTHEntry> Contents { get; set; } = new List<PTHEntry>();

        public bool Compressed { get; internal set; }

        public int Size { get; internal set; }

        public string DataPath { get; internal set; }

        public byte[] Data { get; internal set; }

        public static PTH Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            PTH pth = new()
            {
                DataPath = Path.ChangeExtension(path, "dat")
            };

            using (BinaryReader br = new(fi.OpenRead()))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    pth.Contents.Add(new PTHEntry { Name = br.ReadNullTerminatedString(), Size = (int)br.ReadUInt32(), Offset = (int)br.ReadUInt32() });
                }

                pth.Size = pth.Contents.Sum(c => c.Size);
                pth.Compressed = pth.Size > new FileInfo(pth.DataPath).Length;
            }

            if (pth.Compressed)
            {
                int pointer = 0;

                using (FileStream fs = File.OpenRead(pth.DataPath))
                using (BinaryReader br = new(fs))
                {
                    bool loop = true;
                    uint length = br.ReadUInt32();

                    pth.Data = new byte[length];

                    do
                    {
                        uint flags = 0;
                        for (int i = 0; i < 4; i++) { flags += (uint)(br.ReadByte() << (3 - i) * 8); }
                        byte mode = (byte)(flags & 0x3);

                        for (int i = 0; i < 30; i++)
                        {
                            if ((flags >> (30 - i + 1) & 0x1) == 1)
                            {
                                int offset = pointer;
                                byte len = br.ReadByte();
                                int dist = br.ReadByte();

                                switch (mode)
                                {
                                    case 0:
                                        dist += (len & 0x3f) << 8;
                                        len = (byte)((len & 0xf8) >> 6);
                                        break;

                                    case 1:
                                        dist += (len & 0x1f) << 8;
                                        len = (byte)((len & 0xf8) >> 5);
                                        break;

                                    case 2:
                                        dist += (len & 0xf) << 8;
                                        len = (byte)((len & 0xf8) >> 4);
                                        break;

                                    case 3:
                                        dist += (len & 0x7) << 8;
                                        len = (byte)((len & 0xf8) >> 3);
                                        break;
                                }

                                len += 3;

                                int odist = dist;

                                for (int j = 0; j < len; j++)
                                {
                                    pth.Data[pointer++] = pth.Data[offset - dist - 1];
                                    if (--dist < 0) { dist = odist; }
                                }
                            }
                            else
                            {
                                pth.Data[pointer++] = br.ReadByte();
                            }

                            if (pointer == length)
                            {
                                loop = false;
                                break;
                            }
                        }
                    } while (loop);
                }
            } 
            else
            {
                pth.Data = File.ReadAllBytes(pth.DataPath);
            }

            return pth;
        }
        public byte[] Extract(PTHEntry entry)
        {
            return Data[entry.Offset..(entry.Offset + entry.Size)];
        }
    }

    public class PTHEntry
    {
        public string Name { get; set; }

        public int Size { get; set; }

        public int Offset { get; set; }
    }
}