using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.DestructionDerbyRaw.Formats
{
    public class SPR : ITexture
    {
        public string Name { get; set; }

        public string Extension => "spr";

        public List<MipMap> MipMaps { get; set; }

        public D3DFormat Format { get; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<Color> Colours { get; set; } = new List<Color>();

        public byte[] Data { get; set; }

        public static SPR Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            SPR spr = new()
            {
                Name = Path.GetFileNameWithoutExtension(path)
            };

            using (BinaryReader br = new(fi.OpenRead()))
            {
                br.ReadUInt16();    // ?
                byte mode = br.ReadByte();
                br.ReadByte();      // ?
                br.ReadUInt16();    // ?
                br.ReadUInt16();    // ?

                int colours = mode == 1 ? 16 : 256;

                for (int i = 0; i < colours; i++)
                {
                    spr.Colours.Add(ColorHelper.PSX5551ToColor(br.ReadUInt16(), ColorHelper.ChannelOrder.BGR, i == 0));
                }

                br.ReadUInt16();    // ?
                spr.Width = br.ReadUInt16();
                spr.Height = br.ReadUInt16();
                br.ReadUInt16();    // ?

                spr.Data = new byte[spr.Width * spr.Height];

                int pointer = 0;

                for (int y = 0; y < spr.Height; y++)
                {
                    for (int x = 0; x < spr.Width; x++)
                    {
                        byte i = br.ReadByte();

                        if (mode == 1)
                        {
                            spr.Data[pointer++] = (byte)(i & 0xf);
                            spr.Data[pointer++] = (byte)(i >> 4);
                        }
                        else
                        {
                            spr.Data[pointer++] = i;
                        }

                        if (mode == 1) { x++; }
                    }
                }
            }

            return spr;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp = new(Width, Height, PixelFormat.Format32bppArgb);
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            using (MemoryStream nms = new())
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Color c = Colours[Data[y * Width + x]];
                        nms.WriteByte(c.B);
                        nms.WriteByte(c.G);
                        nms.WriteByte(c.R);
                        nms.WriteByte(c.A);
                    }
                }

                byte[] contentBuffer = new byte[nms.Length];
                nms.Position = 0;
                nms.Read(contentBuffer, 0, contentBuffer.Length);

                Marshal.Copy(contentBuffer, 0, bmpdata.Scan0, contentBuffer.Length);
            }

            bmp.UnlockBits(bmpdata);

            return bmp;
        }
    }
}
