using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Formats
{
    public class PIX
    {
        public static int[] RawPalette = { 16711900, 6095619, 9634306, 13172993, 16711680, 16728128, 16744319, 16760767, 4002056, 7214854, 10362372, 13575170, 16722432, 16736321, 16684930, 16698819, 4332553, 7416327, 10565893, 13649666, 16733440, 16743482, 16687732, 16697774, 5911309, 8603146, 11360519, 14052355, 16744192, 16752189, 16694395, 16702392, 4862728, 7819270, 10841604, 13798146, 16754688, 16760384, 16766081, 16771777, 4865801, 7824647, 10848773, 13807618, 16766208, 16703299, 16640645, 16577736, 5394954, 8224008, 11118853, 13947907, 16776960, 16711244, 16711319, 16645603, 4608526, 6978827, 9349383, 11654148, 14024448, 14745158, 15400331, 16121041, 2966528, 5010176, 7053824, 9031680, 11075328, 12648263, 14220941, 15793876, 5131591, 7367765, 9538404, 11774322, 13944960, 14669212, 15328184, 16052436, 1591296, 2586112, 3580672, 4575488, 5570048, 8257085, 10943867, 13630904, 402944, 2640929, 4813377, 7051362, 9223810, 10736538, 12314802, 13827530, 16917, 28965, 41269, 53316, 65364, 4259711, 8388265, 12582612, 17185, 1925439, 3899228, 5807226, 7715479, 9554605, 11393732, 13232858, 15143, 27719, 40296, 52872, 65448, 4194238, 8323027, 12451817, 10016, 934457, 1793362, 2717802, 3576707, 5810330, 8109490, 10343113, 13878, 26728, 39835, 52685, 65535, 4194046, 8388350, 12516861, 10031, 21347, 32407, 43723, 54783, 4186110, 8382974, 12514301, 9786, 18283, 26525, 35022, 43263, 3980799, 7983615, 11921151, 6712, 12906, 18844, 25037, 30975, 3248895, 6532607, 9750527, 60, 109, 158, 206, 255, 3816191, 7566335, 11382271, 8072704, 9652509, 11232571, 12746840, 14326645, 15120283, 15979712, 16773350, 2230272, 4988432, 7746848, 10439472, 13197632, 13270115, 13342341, 13414824, 5321499, 6503453, 7751199, 8933153, 10115107, 12282666, 14515505, 16683064, 6176801, 7620633, 9064466, 10442506, 11886338, 13468725, 15116904, 16699291, 3745551, 5061411, 6443064, 7758924, 9074784, 10588282, 12102036, 13615534, 2827800, 3946022, 5064244, 6182465, 7300687, 9537649, 11708818, 13945780, 5423, 1189439, 2438991, 3623006, 4807022, 6911883, 8951463, 11056324, 1315860, 2171169, 3026478, 3947324, 4802889, 5658198, 6513507, 7368816, 8289662, 9144971, 10000280, 10855589, 11711154, 12632000, 13487309, 14342618, 0, 1119273, 2237752, 3355976, 4474455, 5592934, 6711413, 7829893, 8948116, 10066595, 11185074, 12303553, 13421777, 14540256, 15658735, 16777215 };
        static Color[] palette;
        List<PIXIE> pixies;

        public List<PIXIE> Pixies
        {
            get => pixies;
            set => pixies = value;
        }

        public static Color[] Palette
        {
            get
            {
                if (PIX.palette == null)
                {
                    palette = new Color[RawPalette.Length];
                    for (int i = 0; i < palette.Length; i++)
                    {
                        palette[i] = ColourHelper.R8G8B8ToColour(RawPalette[i]);
                    }
                }

                return palette;
            }
        }

        public PIX()
        {
            pixies = new List<PIXIE>();
        }

        public static PIX Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            FileInfo fi = new FileInfo(path);
            PIX pix = new PIX();

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadUInt32() != 0x12 ||
                    br.ReadUInt32() != 0x08 ||
                    br.ReadUInt32() != 0x02 ||
                    br.ReadUInt32() != 0x02)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid PIX file", path);
                    return null;
                }

                PIXIE pixelmap = new PIXIE();

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int tag = (int)br.ReadUInt32();
                    int length = (int)br.ReadUInt32();

                    switch (tag)
                    {
                        case 0x03:
                            pixelmap = new PIXIE()
                            {
                                Format = (PIXIE.PixelmapFormat)br.ReadByte(),
                                RowSize = br.ReadUInt16(),
                                Width = br.ReadUInt16(),
                                Height = br.ReadUInt16(),
                                HalfWidth = br.ReadUInt16(),
                                HalfHeight = br.ReadUInt16(),
                                Name = br.ReadString()
                            };
                            break;

                        case 0x21:
                            pixelmap.PixelCount = (int)br.ReadUInt32();
                            pixelmap.PixelSize = (int)br.ReadUInt32();
                            pixelmap.SetData(br.ReadBytes(pixelmap.DataLength));
                            break;

                        case 0x00:
                            pix.pixies.Add(pixelmap);
                            break;

                        case 0x3d:
                            pixelmap = new PIXIE()
                            {
                                Format = (PIXIE.PixelmapFormat)br.ReadByte(),
                                RowSize = br.ReadUInt16(),
                                Width = br.ReadUInt16(),
                                Height = br.ReadUInt16()
                            };
                            br.ReadBytes(6);
                            pixelmap.Name = br.ReadString();
                            break;

                        default:
                            Logger.LogToFile(Logger.LogLevel.Error, "Unknown PIX tag: {0} ({1:x2})", tag, br.BaseStream.Position);
                            return null;
                    }
                }
            }

            return pix;
        }
    }

    public class PIXIE
    {
        public enum PixelmapFormat
        {
            C1_8bit = 3,
            C2_16bit = 5,
            C2_16bitAlpha = 18
        }

        string name;
        PixelmapFormat format;
        int width;
        int height;
        int halfWidth;
        int halfHeight;
        int rowSize;
        int pixelCount;
        int pixelSize;
        byte[] data;

        public PixelmapFormat Format
        {
            get => format;
            set => format = value;
        }

        public int Width
        {
            get => width;
            set => width = value;
        }

        public int Height
        {
            get => height;
            set => height = value;
        }

        public int HalfWidth
        {
            get => halfWidth;
            set => halfWidth = value;
        }

        public int HalfHeight
        {
            get => halfHeight;
            set => halfHeight = value;
        }

        public int RowSize
        {
            get => rowSize;
            set => rowSize = value;
        }

        public int ActualRowSize
        {
            get
            {
                if (pixelCount != height * width || rowSize > width)
                {
                    return rowSize / pixelSize;
                }
                else
                {
                    return rowSize;
                }
            }
        }

        public int PixelCount
        {
            get => pixelCount;
            set => pixelCount = value;
        }

        public int PixelSize
        {
            get => pixelSize;
            set => pixelSize = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int DataLength => height * ActualRowSize * pixelSize;

        public void SetData(byte[] data)
        {
            this.data = data;
        }

        public Color GetColourAtPixel(int x, int y)
        {
            switch (format)
            {
                case PixelmapFormat.C1_8bit:
                    return PIX.Palette[data[x + y * ActualRowSize]];

                case PixelmapFormat.C2_16bit:
                    return ColourHelper.R5G6B5ToColour((data[x + (y * rowSize)] << 8) | data[x + (y * rowSize) + 1]);

                case PixelmapFormat.C2_16bitAlpha:
                    return ColourHelper.A4R4G4B4ToColour(data[x + (y * rowSize)] << 8 | data[x + (y * rowSize) + 1]);
            }

            return Color.Pink;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            {
                BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                using (MemoryStream nms = new MemoryStream())
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < ActualRowSize; x++)
                        {
                            Color c = GetColourAtPixel(x * pixelSize, y);
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
}