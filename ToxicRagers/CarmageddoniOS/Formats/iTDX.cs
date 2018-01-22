using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace ToxicRagers.CarmageddoniOS.Formats
{
    public class TDX
    {
        public static int LIMIT_COORD(int val, int size, bool tiles)
        {
            if (tiles)
            {
                return WRAP_COORD(val, size);
            }
            else
            {
                //return PVRT_CLAMP(val, 0, size - 1);
                throw new NotImplementedException("PVRT_CLAMP");
            }
        }

        public static int WRAP_COORD(int val, int size)
        {
            return (val & (size - 1));
        }

        public static uint TwiddleUV(uint YSize, uint XSize, uint YPos, uint XPos)
        {
            uint MinDimension;
            uint MaxValue;
            uint SrcBitPos, DstBitPos, Twiddled;
            int ShiftCount;

            Debug.Assert(YPos < YSize);
            Debug.Assert(XPos < XSize);

            Debug.Assert(Number_is_power_2(YSize));
            Debug.Assert(Number_is_power_2(XSize));

            if (YSize < XSize)
            {
                MinDimension = YSize;
                MaxValue = XPos;
            }
            else
            {
                MinDimension = XSize;
                MaxValue = YPos;
            }

            SrcBitPos = 1;
            DstBitPos = 1;
            Twiddled = 0;
            ShiftCount = 0;

            while (SrcBitPos < MinDimension)
            {
                if ((YPos & SrcBitPos) > 0)
                {
                    Twiddled |= DstBitPos;
                }

                if ((XPos & SrcBitPos) > 0)
                {
                    Twiddled |= (DstBitPos << 1);
                }

                SrcBitPos <<= 1;
                DstBitPos <<= 2;
                ShiftCount++;
            }

            MaxValue >>= ShiftCount;
            Twiddled |= (MaxValue << (2 * ShiftCount));

            return Twiddled;
        }

        public static bool Number_is_power_2(uint imp)
        {
            if (imp == 0) { return false; }

            uint minus1 = imp - 1;
            return ((imp | minus1) == (imp ^ minus1)) ? true : false;
        }

        public static void ProcessTDX(string PathIn, string PathOut)
        {
            string pathIn = PathIn.Substring(0, PathIn.LastIndexOf("\\") + 1);
            string fileIn = PathIn.Replace(pathIn, "");

            if (!Directory.Exists(PathOut)) { Directory.CreateDirectory(PathOut); }

            BinaryReader br = new BinaryReader(new FileStream(pathIn + fileIn, FileMode.Open));

            br.ReadUInt16();    // 512, standard header
            int width = br.ReadInt16();
            int height = br.ReadInt16();
            int mipmaps = br.ReadUInt16();
            int flags = (int)br.ReadUInt32();
            int type = (int)br.ReadUInt32();
            int x = 0, y = 0;

            uint pixel = 0;

            //Console.Write("\t{0}", br.BaseStream.Length);
            //Console.Write("\t{0}\t{1}\t{2}", width, height, mipmaps);
            //for (int i = 0; i < 32; i++)
            //{
            //    Console.Write("\t{0}", ((flags & (1 << i)) > 0 ? 1 : 0));
            //}
            //Console.Write("\t{0}", type);
            //Console.WriteLine();

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (type == 13)
            {
                // PVRTC
                byte[] rawData = br.ReadBytes((width * height) / 2);

                bool AssumeImageTiles = true;
                int BLK_X_2BPP = 8;
                int BLK_X_4BPP = 4;
                int BLK_X_SIZE = (true) ? BLK_X_2BPP : BLK_X_4BPP; // is_2bpp
                int BLK_Y_SIZE = 4;
                //int BLK_X_MAX = 8;
                int blkX, blkY;
                int blkXp1, blkYp1;
                int blkXDim, blkYDim;

                //AMTC_BLOCK_STRUCT[,] pBlocks = new AMTC_BLOCK_STRUCT[2, 2];

                blkXDim = Math.Max(2, width / BLK_X_SIZE);
                blkYDim = Math.Max(2, height / BLK_Y_SIZE);

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        blkX = (x - BLK_X_SIZE / 2);
                        blkY = (y - BLK_Y_SIZE / 2);

                        blkX = LIMIT_COORD(blkX, width, AssumeImageTiles);
                        blkY = LIMIT_COORD(blkY, height, AssumeImageTiles);

                        blkX /= BLK_X_SIZE;
                        blkY /= BLK_Y_SIZE;

                        blkXp1 = LIMIT_COORD(blkX + 1, blkXDim, AssumeImageTiles);
                        blkYp1 = LIMIT_COORD(blkY + 1, blkYDim, AssumeImageTiles);

                        //pBlocks[0, 0] = input_buf + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkY, (uint)blkX);
                        //pBlocks[0, 1] = input_buf + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkY, (uint)blkXp1);
                        //pBlocks[1, 0] = input_buf + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkYp1, (uint)blkX);
                        //pBlocks[1, 1] = input_buf + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkYp1, (uint)blkXp1);

                        Console.WriteLine("0, 0 = " + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkY, (uint)blkX));
                        Console.WriteLine("0, 1 = " + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkY, (uint)blkXp1));
                        Console.WriteLine("1, 0 = " + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkYp1, (uint)blkX));
                        Console.WriteLine("1, 1 = " + TwiddleUV((uint)blkYDim, (uint)blkXDim, (uint)blkYp1, (uint)blkXp1));
                    }
                }
            }
            else
            {
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        switch (type)
                        {
                            case 16:
                            case 17:
                            case 19:
                                pixel = br.ReadUInt16();
                                break;

                            case 18:
                                pixel = br.ReadUInt32();
                                break;

                            case 21:
                                pixel = (uint)((br.ReadByte() << 0) | (br.ReadByte() << 8) | (br.ReadByte() << 16));
                                break;

                            default:
                                pixel = br.ReadUInt16();
                                break;
                        }

                        Color c = pixelToRGB(pixel, type);
                        bmp.SetPixel(x, y, c);
                    }
                }
            }

            bmp.Save(PathOut + "\\" + fileIn.ToLower().Replace("tdx", "tif"), System.Drawing.Imaging.ImageFormat.Tiff);

            br.Close();
        }

        private static Color pixelToRGB(uint i, int format)
        {
            // Note to idiot self, values are passed in backwards.
            int r = 0;
            int g = 0;
            int b = 0;
            int a = 0;

            switch (format)
            {
                case 16:
                    //R4G4B4A4
                    r = (int)((i & 0xF000) >> 12) << 4;
                    g = (int)((i & 0xF00) >> 8) << 4;
                    b = (int)((i & 0xF0) >> 4) << 4;
                    a = (int)((i & 0xF) >> 0) << 4;
                    break;

                case 17:
                    //R5G5B6
                    r = (int)((i & 0xF800) >> 11) << 3;
                    g = (int)((i & 0x7C0) >> 6) << 3;
                    b = (int)(i & 0x3F) << 2;
                    a = 255;
                    break;

                case 18:
                    //R8G8B8A8
                    a = (int)(i >> 24);
                    b = (int)((i & 0xFF0000) >> 16);
                    g = (int)((i & 0xFF00) >> 8);
                    r = (int)((i & 0xFF) >> 0);
                    break;

                case 19:
                    //R5G6B5
                    r = (int)((i & 0xF800) >> 11) << 3;
                    g = (int)((i & 0x7E0) >> 5) << 2;
                    b = (int)(i & 0x1F) << 3;
                    a = 255;
                    break;

                case 21:
                    b = (int)((i & 0xFF0000) >> 16);
                    g = (int)((i & 0xFF00) >> 8);
                    r = (int)((i & 0xFF) >> 0);
                    a = 255;
                    break;

            }

            return Color.FromArgb(a, r, g, b);
        }
    }
}