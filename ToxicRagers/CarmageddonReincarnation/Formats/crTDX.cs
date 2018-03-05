using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

using ToxicRagers.CarmageddonReincarnation.VirtualTextures;
using ToxicRagers.Core.Formats;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

using Squish;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class TDX : Texture
    {
        [Flags]
        public enum Flags
        {
            Palettised4bit = 1,
            Palettised8bit = 2,
            Alpha1bit = 4,
            AlphaNbit = 8,
            AlphaOnly = 16,
            Compressed = 32,
            DoNotCompress = 64,
            DoNot16bit = 128,
            DoNotDownsample = 256,
            ExtraData = 512,
            DoNotMipmap = 1024,
            Swizzled = 2048,
            FileIsJpeg = 4096,
            CubeMap = 8192,
            sRGB = 16384
        }

        public enum ExtraDataTypes
        {
            Font,
            Animation,
            VTMap
        }

        int width;
        int height;
        Flags flags;
        ExtraDataTypes extraDataType;
        TDXExtraData extraData;

        public ExtraDataTypes ExtraDataType => extraDataType;
        public TDXExtraData ExtraData => extraData;

        public void SetFlags(Flags flags)
        {
            this.flags = flags;
        }

        public TDX()
            : base()
        {
            extension = "TDX";
        }

        public static TDX Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            TDX tdx = new TDX();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path))) { tdx = Load(ms, Path.GetFileNameWithoutExtension(path)); }

            return tdx;
        }

        public static TDX Load(Stream stream, string name = null)
        {
            TDX tdx = new TDX { Name = name };

            using (BinaryReader br = new BinaryReader(stream))
            {
                if (br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0x02)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "This isn't a valid TDX");
                    return null;
                }

                tdx.width = br.ReadUInt16();
                tdx.height = br.ReadUInt16();
                int mipCount = br.ReadUInt16();
                tdx.flags = (Flags)br.ReadUInt32();
                tdx.Format = (D3DFormat)br.ReadUInt32();

                if (tdx.flags.HasFlag(Flags.ExtraData))
                {
                    int extraDataLength = (int)br.ReadUInt32();
                    int extraDataType = br.ReadUInt16();

                    switch (extraDataType)
                    {
                        case 0:
                            tdx.extraDataType = ExtraDataTypes.Font;
                            tdx.extraData = new FontDefinition(br.ReadBytes(extraDataLength - 2));
                            break;

                        case 1:
                            tdx.extraDataType = ExtraDataTypes.Animation;
                            tdx.extraData = new AnimationDefinition(br.ReadBytes(extraDataLength - 2));
                            break;

                        case 3:
                            tdx.extraDataType = ExtraDataTypes.VTMap;
                            tdx.extraData = new VTMap(br.ReadBytes(extraDataLength - 2));
                            break;

                        default:
                            throw new NotImplementedException(string.Format("Unknown Extra Data flag: {0}", extraDataType));
                    }
                }

                for (int i = 0; i < mipCount; i++)
                {
                    MipMap mip = new MipMap()
                    {
                        Width = tdx.width >> i,
                        Height = tdx.height >> i
                    };

                    switch (tdx.Format)
                    {
                        case D3DFormat.A8R8G8B8:
                            mip.Data = br.ReadBytes(mip.Width * mip.Height * 4);
                            break;

                        case D3DFormat.A8:
                            mip.Data = br.ReadBytes(mip.Width * mip.Height);
                            break;

                        case D3DFormat.DXT1:
                            mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 8);
                            break;

                        case D3DFormat.ATI2:
                        case D3DFormat.DXT5:
                            mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 16);
                            break;

                        default:
                            Logger.LogToFile(Logger.LogLevel.Error, "Unknown format: {0}", tdx.Format);
                            return null;
                    }

                    tdx.MipMaps.Add(mip);
                }
            }

            return tdx;
        }

        public static TDX LoadFromBitmap(Bitmap asset, string name, D3DFormat format)
        {
            TDX tdx = null;

            if (tdx == null)
            {
                tdx = new TDX();

                Bitmap b = asset;
                SquishFlags flags = Squish.SquishFlags.kDxt1;

                tdx.Name = name;
                tdx.Format = format;

                switch (tdx.Format)
                {
                    case D3DFormat.DXT1:
                        flags = Squish.SquishFlags.kDxt1;
                        break;

                    case D3DFormat.DXT5:
                        flags = Squish.SquishFlags.kDxt5;
                        break;
                }

                List<Bitmap> mipBitmaps = GenerateMips(b, b.Width, b.Height);

                foreach (Bitmap mb in mipBitmaps)
                {
                    MipMap mip = new MipMap()
                    {
                        Width = mb.Width,
                        Height = mb.Height
                    };

                    byte[] data = new byte[mb.Width * mb.Height * 4];
                    byte[] dest = new byte[Squish.Squish.GetStorageRequirements(mb.Width, mb.Height, flags | Squish.SquishFlags.kColourIterativeClusterFit | Squish.SquishFlags.kWeightColourByAlpha)];

                    int ii = 0;
                    for (int y = 0; y < mb.Height; y++)
                    {
                        for (int x = 0; x < mb.Width; x++)
                        {
                            Color p = mb.GetPixel(x, y);
                            data[ii + 0] = p.R;
                            data[ii + 1] = p.G;
                            data[ii + 2] = p.B;
                            data[ii + 3] = p.A;

                            ii += 4;
                        }
                    }

                    if (format == D3DFormat.ATI2)
                    {
                        dest = BC5Unorm.Compress(data, (ushort)mb.Width, (ushort)mb.Height, GetMipSize(format, (ushort)mb.Width, (ushort)mb.Height));
                    }
                    else
                    {
                        Squish.Squish.CompressImage(data, mb.Width, mb.Height, ref dest, flags | Squish.SquishFlags.kColourClusterFit);
                    }

                    mip.Data = dest;

                    tdx.MipMaps.Add(mip);
                }
            }

            return tdx;
        }

        public static int GetMipSize(D3DFormat format, ushort width, ushort height)
        {
            width = Math.Max((ushort)4, width);
            height = Math.Max((ushort)4, height);

            if (format == D3DFormat.DXT1)
            {
                return
                    (((width + 3) / 4) *
                     ((height + 3) / 4)) * 8;
            }

            if (format == D3DFormat.DXT5 ||
                format == D3DFormat.ATI2)
            {
                return
                    (((width + 3) / 4) *
                     ((height + 3) / 4)) * 16;
            }

            return width * height * 4;
        }

        public static List<Bitmap> GenerateMips(Bitmap b, int width, int height)
        {
            List<Bitmap> mips = new List<Bitmap>();
            int currentWidth = width / 2;
            int currentHeight = height / 2;
            mips.Add(b);
            int i = 1;
            while (currentWidth > 1 && currentHeight > 1)
            {
                Bitmap mipimage = new Bitmap(currentWidth, currentHeight);
                RectangleF srcRect = new RectangleF(0, 0, width, height);
                RectangleF destRect = new RectangleF(0, 0, currentWidth, currentHeight);
                Graphics grfx = Graphics.FromImage(mipimage);
                grfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                grfx.DrawImage(b, destRect, srcRect, GraphicsUnit.Pixel);
                mips.Add(mipimage);
                i++;

                currentHeight /= 2;
                currentWidth /= 2;
            }
            return mips;
        }

        public int GetMipLevelForSize(int maxDimension)
        {
            for (int i = 0; i < MipMaps.Count; i++)
            {
                if (MipMaps[i].Width <= maxDimension || MipMaps[i].Height <= maxDimension)
                {
                    return i;
                }
            }

            return 0;
        }

        public Bitmap Decompress(int mipLevel = 0, bool bSuppressAlpha = false)
        {
            MipMap mip = MipMaps[mipLevel];

            Bitmap b = new Bitmap(mip.Width, mip.Height, PixelFormat.Format32bppArgb);
            byte[] dest = Decompress(mip, bSuppressAlpha);

            BitmapData bmpdata = b.LockBits(new Rectangle(0, 0, mip.Width, mip.Height), ImageLockMode.ReadWrite, (bSuppressAlpha ? PixelFormat.Format32bppRgb : b.PixelFormat));
            Marshal.Copy(dest, 0, bmpdata.Scan0, dest.Length);
            b.UnlockBits(bmpdata);

            return b;
        }

        public byte[] Decompress(MipMap mip, bool bSuppressAlpha = false)
        {
            SquishFlags flags = 0;
            bool bNotCompressed = false;
            bool ATI2 = false;

            switch (Format)
            {
                case D3DFormat.DXT1:
                    flags = SquishFlags.kDxt1;
                    break;

                case D3DFormat.DXT5:
                    flags = SquishFlags.kDxt5;
                    break;

                case D3DFormat.A8R8G8B8:
                    bNotCompressed = true;
                    break;

                case D3DFormat.ATI2:
                    ATI2 = true;
                    break;

                default:
                    throw new NotImplementedException(string.Format("Can't decompress: {0}", Format));
            }

            byte[] dest = new byte[mip.Width * mip.Height * 4];
            byte[] data = mip.Data;

            if (bNotCompressed)
            {
                for (uint i = 0; i < data.Length - 4; i += 4)
                {
                    dest[i + 0] = data[i + 2];
                    dest[i + 1] = data[i + 1];
                    dest[i + 2] = data[i + 0];
                    dest[i + 3] = data[i + 3];
                }
            }
            else if (ATI2)
            {
                return DecompressATI2(data, (uint)mip.Width, (uint)mip.Height, !bSuppressAlpha);
            }
            else
            {
                Squish.Squish.DecompressImage(dest, mip.Width, mip.Height, ref data, flags);

                for (uint i = 0; i < dest.Length - 4; i += 4)
                {
                    byte r = dest[i + 0];
                    dest[i + 0] = dest[i + 2];
                    dest[i + 2] = r;
                }
            }
            return dest;
        }

        private Bitmap makeBitmapFromATI2(byte[] blocks, uint width, uint height, bool keepAlpha)
        {
            byte[] buffer = DecompressATI2(blocks, width, height, keepAlpha);
            Bitmap bitmap = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
            Rectangle area = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(area, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(buffer, 0, data.Scan0, (int)(width * height * 4));
            bitmap.UnlockBits(data);
            return bitmap;
        }

        public byte[] DecompressATI2(byte[] blocks, uint width, uint height, bool keepAlpha)
        {
            byte[] redBuffer = new byte[width * height];
            byte[] greenBuffer = new byte[width * height];

            for (int row = 0, col = 0, j = 0; j < blocks.Length; j += 16, col += 4)
            {
                byte[] redColours = BC5Unorm.CalcColours(blocks[j], blocks[j + 1]);
                byte[] greenColours = BC5Unorm.CalcColours(blocks[j + 8], blocks[j + 9]);

                uint[] redIndices = BC5Unorm.GetIndices(blocks[j + 7], blocks[j + 6], blocks[j + 5], blocks[j + 4], blocks[j + 3], blocks[j + 2]);
                uint[] greenIndices = BC5Unorm.GetIndices(blocks[j + 15], blocks[j + 14], blocks[j + 13], blocks[j + 12], blocks[j + 11], blocks[j + 10]);

                if (col > width)
                {
                    col -= (int)width;
                    row += 4;
                }

                /* To test: see if this spits out the same numbers as below */
                //int[] offsets = new int[]
                //{
                //    (int)(2 * row * width + col + width),
                //    (int)(3 * row * width + col + width),
                //    (int)(1 * row * width + col),
                //    (int)(1 * row * width + col + width)
                //};

                //for (int i = 0; i < 4; i++)
                //{
                //    for (int k = 0; k < 4; k++)
                //    {
                //        redBuffer[  offsets[i] + k] = redColours[    redIndices[i * 4 + k]];
                //        greenBuffer[offsets[i] + k] = greenColours[greenIndices[i * 4 + k]];
                //    }
                //}

                redBuffer[row * width + col + width * 2 + 0] = redColours[redIndices[0]];
                redBuffer[row * width + col + width * 2 + 1] = redColours[redIndices[1]];
                redBuffer[row * width + col + width * 2 + 2] = redColours[redIndices[2]];
                redBuffer[row * width + col + width * 2 + 3] = redColours[redIndices[3]];

                redBuffer[row * width + col + width * 3 + 0] = redColours[redIndices[4]];
                redBuffer[row * width + col + width * 3 + 1] = redColours[redIndices[5]];
                redBuffer[row * width + col + width * 3 + 2] = redColours[redIndices[6]];
                redBuffer[row * width + col + width * 3 + 3] = redColours[redIndices[7]];

                redBuffer[row * width + col + 0] = redColours[redIndices[8]];
                redBuffer[row * width + col + 1] = redColours[redIndices[9]];
                redBuffer[row * width + col + 2] = redColours[redIndices[10]];
                redBuffer[row * width + col + 3] = redColours[redIndices[11]];

                redBuffer[row * width + col + width + 0] = redColours[redIndices[12]];
                redBuffer[row * width + col + width + 1] = redColours[redIndices[13]];
                redBuffer[row * width + col + width + 2] = redColours[redIndices[14]];
                redBuffer[row * width + col + width + 3] = redColours[redIndices[15]];

                greenBuffer[row * width + col + width * 2 + 0] = greenColours[greenIndices[0]];
                greenBuffer[row * width + col + width * 2 + 1] = greenColours[greenIndices[1]];
                greenBuffer[row * width + col + width * 2 + 2] = greenColours[greenIndices[2]];
                greenBuffer[row * width + col + width * 2 + 3] = greenColours[greenIndices[3]];

                greenBuffer[row * width + col + width * 3 + 0] = greenColours[greenIndices[4]];
                greenBuffer[row * width + col + width * 3 + 1] = greenColours[greenIndices[5]];
                greenBuffer[row * width + col + width * 3 + 2] = greenColours[greenIndices[6]];
                greenBuffer[row * width + col + width * 3 + 3] = greenColours[greenIndices[7]];

                greenBuffer[row * width + col + 0] = greenColours[greenIndices[8]];
                greenBuffer[row * width + col + 1] = greenColours[greenIndices[9]];
                greenBuffer[row * width + col + 2] = greenColours[greenIndices[10]];
                greenBuffer[row * width + col + 3] = greenColours[greenIndices[11]];

                greenBuffer[row * width + col + width + 0] = greenColours[greenIndices[12]];
                greenBuffer[row * width + col + width + 1] = greenColours[greenIndices[13]];
                greenBuffer[row * width + col + width + 2] = greenColours[greenIndices[14]];
                greenBuffer[row * width + col + width + 3] = greenColours[greenIndices[15]];
            }

            byte[] buffer = new byte[width * height * 4];

            for (uint i = 0, j = 0; i < width * height * 4; i += 4, j++)
            {
                //Console.WriteLine("Filling Buffer Pixel " + (i / 4));
                buffer[i + 2] = redBuffer[j];
                buffer[i + 1] = greenBuffer[j];
                float redTmp = redBuffer[j] / 255;
                float greenTmp = greenBuffer[j] / 255;
                //redTmp = redTmp * 2 - 1;
                //greenTmp = greenTmp * 2 - 1;
                double blueTmp = Math.Sqrt((redTmp * redTmp + greenTmp * greenTmp));
                blueTmp = (blueTmp + 1) * 0.5;
                buffer[i] = 255;// (byte)(blueTmp * 255);
                buffer[i + 3] = 255;
            }

            return buffer;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create)) { Save(fs); }
        }

        public void Save(Stream s)
        {
            byte[] data = Save(this);
            s.Write(data, 0, data.Length);
        }

        public static byte[] Save(TDX tdx)
        {
            byte[] buffer;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(new byte[] { 0, 2 });

                bw.Write((short)tdx.MipMaps[0].Width);
                bw.Write((short)tdx.MipMaps[0].Height);
                bw.Write((short)tdx.MipMaps.Count);
                bw.Write((int)tdx.flags);
                bw.WriteString(tdx.ShortFormat);

                for (int i = 0; i < tdx.MipMaps.Count; i++) { bw.Write(tdx.MipMaps[i].Data); }

                bw.Flush();
                ms.Flush();

                buffer = ms.ToArray();
            }

            return buffer;
        }

        public static explicit operator DDS(TDX tdx)
        {
            DDS dds = new DDS()
            {
                Width = tdx.MipMaps[0].Width,
                Height = tdx.MipMaps[0].Height,
                Format = tdx.Format
            };

            foreach (MipMap mip in tdx.MipMaps)
            {
                dds.MipMaps.Add(new MipMap { Width = mip.Width, Height = mip.Height, Data = mip.Data });
            }

            return dds;
        }
    }

    public abstract class TDXExtraData { }

    public class FontDefinition : TDXExtraData
    {
        public FontDefinition(byte[] data)
        {

        }
    }

    public class AnimationDefinition : TDXExtraData
    {
        public AnimationDefinition(byte[] data)
        {

        }
    }
}