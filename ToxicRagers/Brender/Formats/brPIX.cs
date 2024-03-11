using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Brender.Formats
{
    public class PIX
    {
        public static Game GamePalette { get; } = new Game();

        public List<PIXIE> Pixies { get; set; } = new List<PIXIE>();

        public static PIX Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            PIX pix = new PIX();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            {
                pix = Load(ms);
            }

            return pix;
        }

        public static PIX Load(Stream stream)
        {
            PIX pix = new PIX();

            using (BEBinaryReader br = new BEBinaryReader(stream, Encoding.Default))
            {
                if (br.ReadUInt32() != 0x12 ||
                    br.ReadUInt32() != 0x08 ||
                    br.ReadUInt32() != 0x02 ||
                    br.ReadUInt32() != 0x02)
                {
                    Logger.LogToFile(Logger.LogLevel.Warning, "This PIX file is missing the standard header");

                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                }

                Stack<PIXIE> pixies = new Stack<PIXIE>();

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    if (br.BaseStream.Position + 8 > br.BaseStream.Length)
                    {
                        Logger.LogToFile(Logger.LogLevel.Error, $"This PIX is malformed.  The last {br.BaseStream.Length - br.BaseStream.Position} bytes are redundant and should be removed");
                        break;
                    }

                    int tag = (int)br.ReadUInt32();
                    br.ReadUInt32();        // length

                    switch (tag)
                    {
                        case 0x03:  // 3
                            pixies.Push(new PIXIE()
                            {
                                Format = (PIXIE.PixelmapFormat)br.ReadByte(),
                                RowSize = br.ReadUInt16(),
                                Width = br.ReadUInt16(),
                                Height = br.ReadUInt16(),
                                HalfWidth = br.ReadUInt16(),
                                HalfHeight = br.ReadUInt16(),
                                Name = br.ReadString()
                            });
                            break;

                        case 0x21:  // 33
                            pixies.Peek().PixelCount = (int)br.ReadUInt32();
                            pixies.Peek().PixelSize = (int)br.ReadUInt32();
                            pixies.Peek().SetData(br.ReadBytes(pixies.Peek().DataLength));
                            break;

                        case 0x22:  // 34
                        case 0x00:  // 0
                            pix.Pixies.Add(pixies.Pop());
                            break;

                        case 0x3d:
                            pixies.Push(new PIXIE
                            {
                                Format = (PIXIE.PixelmapFormat)br.ReadByte(),
                                RowSize = br.ReadUInt16(),
                                Width = br.ReadUInt16(),
                                Height = br.ReadUInt16()
                            });
                            br.ReadBytes(6);
                            pixies.Peek().Name = br.ReadString();
                            break;

                        default:
                            Logger.LogToFile(Logger.LogLevel.Error, "Unknown PIX tag: {0} ({1:x2})", tag, br.BaseStream.Position);
                            return null;
                    }
                }
            }

            return pix;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BEBinaryWriter bw = new BEBinaryWriter(fs))
            {
                bw.WriteInt32(0x12);
                bw.WriteInt32(0x8);
                bw.WriteInt32(0x2);
                bw.WriteInt32(0x2);

                foreach (PIXIE pixie in Pixies)
                {
                    bw.WriteInt32(0x3);
                    bw.WriteInt32(12 + pixie.Name.Length);
                    bw.WriteByte((byte)pixie.Format);
                    bw.WriteInt16(pixie.RowSize);
                    bw.WriteInt16(pixie.Width);
                    bw.WriteInt16(pixie.Height);
                    bw.WriteInt16(pixie.HalfWidth);
                    bw.WriteInt16(pixie.HalfHeight);
                    bw.WriteString(pixie.Name.ToUpper());
                    bw.WriteByte(0);

                    bw.WriteInt32(0x21);
                    bw.WriteInt32(8 + pixie.RowSize * pixie.Height);
                    bw.WriteInt32(pixie.PixelCount);
                    bw.WriteInt32(pixie.PixelSize);
                    bw.Write(pixie.Data);

                    bw.WriteInt32(0x0);
                    bw.WriteInt32(0);
                }
            }
        }

        public void RebuildPalette(PIXIE pixie)
        {
            if (pixie.Format != PIXIE.PixelmapFormat.Palette) { return; }

            for (int i = 0; i < 256; i++)
            {
                GamePalette[i] = Colour.FromArgb(
                    255,
                    pixie.Data[i * 4 + 1],
                    pixie.Data[i * 4 + 2],
                    pixie.Data[i * 4 + 3]
                );
            }
        }
    }

    public class PIXIE
    {
        public enum PixelmapFormat
        {
            Indexed8bit = 3,
            C2_16bit = 5,
            Palette = 7,
            BGR555 = 11,
            C2_16bitAlpha = 18
        }

        public byte[] Data { get; private set; }

        public PixelmapFormat Format { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int HalfWidth { get; set; }

        public int HalfHeight { get; set; }

        public int RowSize { get; set; }

        public int ActualRowSize
        {
            get
            {
                if (PixelCount != Height * Width || RowSize > Width)
                {
                    return RowSize / PixelSize;
                }
                else
                {
                    return RowSize;
                }
            }
        }

        public int PixelCount { get; set; }

        public int PixelSize { get; set; }

        public string Name { get; set; }

        public int DataLength
        {
            get
            {
                if (Format == PixelmapFormat.Palette)
                {
                    return 4 * 256;
                }
                else
                {
                    return Height * ActualRowSize * PixelSize;
                }
            }
        }

        public void SetData(byte[] data)
        {
            Data = data;
        }

        public Color GetColourAtPixel(int x, int y)
        {
            switch (Format)
            {
                case PixelmapFormat.Indexed8bit:
                    return PIX.GamePalette[Data[x + y * ActualRowSize]].ToColor();

                case PixelmapFormat.C2_16bit:
                    return ColorHelper.R5G6B5ToColor(Data[x + y * RowSize] << 8 | Data[x + y * RowSize + 1]);

                case PixelmapFormat.C2_16bitAlpha:
                    return ColorHelper.A4R4G4B4ToColor(Data[x + y * RowSize] << 8 | Data[x + y * RowSize + 1]);

                case PixelmapFormat.Palette:
                    return Color.FromArgb(
                        255,
                        Data[y * 4 + x + 1],
                        Data[y * 4 + x + 2],
                        Data[y * 4 + x + 3]);
            }

            return Color.Pink;
        }

        public static PIXIE FromBitmap(PixelmapFormat format, Bitmap bitmap)
        {
            PIXIE pixie = new PIXIE
            {
                Format = format,
                Width = bitmap.Width,
                Height = bitmap.Height,
                HalfWidth = bitmap.Width / 2,
                HalfHeight = bitmap.Height / 2,
                RowSize = bitmap.Width
            };

            if (pixie.Width % 2 != 0 || pixie.HalfWidth % 2 != 0)
            {
                pixie.RowSize = (pixie.HalfWidth + (pixie.HalfWidth % 2 == 0 ? 2 : 1)) * 2;
            }

            pixie.PixelCount = pixie.RowSize * pixie.Height;
            pixie.PixelSize = 1;

            byte[] data = new byte[pixie.RowSize * pixie.Height];
            Dictionary<Colour, byte> lut = new Dictionary<Colour, byte>();

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < pixie.Width; x++)
                {
                    Colour c = Colour.FromColor(bitmap.GetPixel(x, y));

                    if (!lut.TryGetValue(c, out byte index))
                    {
                        index = (byte)PIX.GamePalette.GetNearestPixelIndex(c);
                        lut.Add(c, index);
                    }

                    data[y * pixie.RowSize + x] = index;
                }
            }

            pixie.SetData(data);

            return pixie;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            using (MemoryStream nms = new MemoryStream())
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < ActualRowSize; x++)
                    {
                        Color c = GetColourAtPixel(x * PixelSize, y);
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