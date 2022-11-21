using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using ToxicRagers.CarmageddonReincarnation.VirtualTextures;
using ToxicRagers.Core.Formats;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

using Squish;
using ToxicRagers.Wreckfest.Formats;

namespace ToxicRagers.Stainless.Formats
{
    public class TDX : ITexture
    {
        [Flags]
        public enum TDXFlags
        {
            Palettised4bit = 0x1,
            Palettised8bit = 0x2,
            Alpha1bit = 0x4,
            AlphaNbit = 0x8,
            AlphaOnly = 0x10,
            Compressed = 0x20,
            DoNotCompress = 0x40,
            DoNot16bit = 0x80,
            DoNotDownsample = 0x100,
            ExtraData = 0x200,
            DoNotMipmap = 0x400,
            Swizzled = 0x800,
            FileIsJpeg = 0x1000,
            CubeMap = 0x2000,
            sRGB = 0x4000
        }

        public enum ExtraDataTypes
        {
            Font,
            Animation,
            VTMap
        }

        public string Name { get; set; }

        public string Extension { get; } = "tdx";

        public List<MipMap> MipMaps { get; set; } = new List<MipMap>();

        public D3DFormat Format { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public TDXFlags Flags { get; set; }

        public ExtraDataTypes ExtraDataType { get; set; }

        public TDXExtraData ExtraData { get; set; }

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

                tdx.Width = br.ReadUInt16();
                tdx.Height = br.ReadUInt16();
                int mipCount = br.ReadUInt16();
                tdx.Flags = (TDXFlags)br.ReadUInt32();
                tdx.Format = (D3DFormat)br.ReadUInt32();

                if (tdx.Flags.HasFlag(TDXFlags.ExtraData))
                {
                    int extraDataLength = (int)br.ReadUInt32();
                    int extraDataType = br.ReadUInt16();

                    switch (extraDataType)
                    {
                        case 0:
                            tdx.ExtraDataType = ExtraDataTypes.Font;
                            tdx.ExtraData = new FontDefinition(br.ReadBytes(extraDataLength - 2));
                            break;

                        case 1:
                            tdx.ExtraDataType = ExtraDataTypes.Animation;
                            tdx.ExtraData = new AnimationDefinition(br.ReadBytes(extraDataLength - 2));
                            break;

                        case 3:
                            tdx.ExtraDataType = ExtraDataTypes.VTMap;
                            tdx.ExtraData = new VTMap(br.ReadBytes(extraDataLength - 2));
                            break;

                        default:
                            throw new NotImplementedException($"Unknown Extra Data flag: {extraDataType}");
                    }
                }

                for (int i = 0; i < mipCount; i++)
                {
                    MipMap mip = new MipMap()
                    {
                        Width = tdx.Width >> i,
                        Height = tdx.Height >> i
                    };

                    switch (tdx.Format)
                    {
                        case D3DFormat.PVRTC4:
                            mip.Data = br.ReadBytes(mip.Width * mip.Height / 2);
                            break;

                        case D3DFormat.R4G4B4A4:
                        case D3DFormat.R5G5B6:
                        case D3DFormat.R5G6B5:
                            mip.Data = br.ReadBytes(mip.Width * mip.Height * 2);
                            break;

                        case D3DFormat.A8B8G8R8:
                            mip.Data = br.ReadBytes(mip.Width * mip.Height * 4);
                            break;

                        case D3DFormat.A8R8G8B8:
                            mip.Data = br.ReadBytes(mip.Width * mip.Height * (tdx.Flags.HasFlag(TDXFlags.AlphaNbit) ? 4 : 3));
                            break;

                        case D3DFormat.A8:
                            mip.Data = br.ReadBytes(mip.Width * mip.Height);
                            break;

                        case D3DFormat.DXT1:
                            mip.Data = br.ReadBytes((mip.Width + 3) / 4 * ((mip.Height + 3) / 4) * 8);
                            break;

                        case D3DFormat.ATI2:
                        case D3DFormat.DXT5:
                            mip.Data = br.ReadBytes((mip.Width + 3) / 4 * ((mip.Height + 3) / 4) * 16);
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
                SquishFlags flags = SquishFlags.kDxt1;

                tdx.Name = name;
                tdx.Format = format;

                switch (tdx.Format)
                {
                    case D3DFormat.DXT1:
                        flags = SquishFlags.kDxt1;
                        break;

                    case D3DFormat.DXT5:
                        flags = SquishFlags.kDxt5;
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
                    byte[] dest = new byte[Squish.Squish.GetStorageRequirements(mb.Width, mb.Height, flags | SquishFlags.kColourIterativeClusterFit | SquishFlags.kWeightColourByAlpha)];

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
                        Squish.Squish.CompressImage(data, mb.Width, mb.Height, dest, flags | SquishFlags.kColourClusterFit, true);
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
            List<Bitmap> mips = new List<Bitmap>() { b };
            int currentWidth = width / 2;
            int currentHeight = height / 2;
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

        public Color[] GetData(int mipLevel = 0)
        {
			MipMap mip = MipMaps[mipLevel];
			int x, y;

			//Bitmap bmp = new Bitmap(mip.Width, mip.Height, PixelFormat.Format32bppArgb);
			byte[] data;
			byte[] dest;
			Color[] outputData = new Color[mip.Width * mip.Height];
			switch (Format)
			{
				case D3DFormat.PVRTC4:
					data = mip.Data;
					dest = new byte[mip.Width * mip.Height * 4];

					PVTRC.Decompress(data, false, mip.Width, mip.Height, true, dest);

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 4;

							Color c = Color.FromArgb(dest[offset + 3], dest[offset + 0], dest[offset + 1], dest[offset + 2]);
							outputData[x + y * mip.Width] = c;
							//bmp.SetPixel(x, y, c);
						}
					}
					break;

				case D3DFormat.R4G4B4A4:
					data = mip.Data;

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 2;
							int pixel = BitConverter.ToUInt16(data, offset);
                            outputData[x + y * mip.Width] = ColorHelper.R4G4B4A4ToColor(pixel);
							//bmp.SetPixel(x, y, ColorHelper.R4G4B4A4ToColor(pixel));
						}
					}
					break;

				case D3DFormat.R5G5B6:
					data = mip.Data;

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 2;
							int pixel = BitConverter.ToUInt16(data, offset);
                            outputData[x + y * mip.Width] = ColorHelper.R5G5B6ToColor(pixel);
							//bmp.SetPixel(x, y, ColorHelper.R5G5B6ToColor(pixel));
						}
					}
					break;

				case D3DFormat.A8B8G8R8:
					data = mip.Data;

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 4;
							uint pixel = BitConverter.ToUInt32(data, offset);
                            outputData[x + y * mip.Width] = ColorHelper.A8B8G8R8ToColor(pixel);
							//bmp.SetPixel(x, y, ColorHelper.A8B8G8R8ToColor(pixel));
						}
					}
					break;

				case D3DFormat.R5G6B5:
					data = mip.Data;

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 2;
							int pixel = BitConverter.ToUInt16(data, offset);
                            outputData[x + y * mip.Width] = ColorHelper.R5G6B5ToColor(pixel);
							//bmp.SetPixel(x, y, ColorHelper.R5G6B5ToColor(pixel));
						}
					}
					break;

				case D3DFormat.A8R8G8B8:
					data = mip.Data;

					int pixelSize = Flags.HasFlag(TDXFlags.AlphaNbit) ? 4 : 3;

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * pixelSize;
							int a = pixelSize == 4 ? data[offset + 3] : 255;

							Color c = Color.FromArgb(a, data[offset + 0], data[offset + 1], data[offset + 2]);
							outputData[x + y * mip.Width] = c;
							//bmp.SetPixel(x, y, c);
						}
					}
					break;

				case D3DFormat.DXT1:
					data = mip.Data;
					dest = new byte[mip.Width * mip.Height * 4];

					Squish.Squish.DecompressImage(dest, mip.Width, mip.Height, data, SquishFlags.kDxt1);

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 4;

							Color c = Color.FromArgb(dest[offset + 3], dest[offset + 0], dest[offset + 1], dest[offset + 2]);
                            outputData[x + y * mip.Width] = c;
							//bmp.SetPixel(x, y, c);
						}
					}
					break;

				case D3DFormat.DXT5:
					data = mip.Data;
					dest = new byte[mip.Width * mip.Height * 4];

					Squish.Squish.DecompressImage(dest, mip.Width, mip.Height, data, SquishFlags.kDxt5);

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 4;

							Color c = Color.FromArgb(dest[offset + 3], dest[offset + 0], dest[offset + 1], dest[offset + 2]);
                            outputData[x + y * mip.Width] = c;
							//bmp.SetPixel(x, y, c);
						}
					}
					break;

				case D3DFormat.ATI2:
					data = mip.Data;

					dest = BC5Unorm.Decompress(data, (uint)mip.Width, (uint)mip.Height);

					for (y = 0; y < mip.Height; y++)
					{
						for (x = 0; x < mip.Width; x++)
						{
							int offset = (x + y * mip.Width) * 4;

							Color c = Color.FromArgb(dest[offset + 3], dest[offset + 2], dest[offset + 1], dest[offset + 0]);
                            outputData[x + y * mip.Width] = c;
							//bmp.SetPixel(x, y, c);
						}
					}
					break;
			}

			return outputData;
        }
        public Bitmap GetBitmap(int mipLevel = 0)
        {
            MipMap mip = MipMaps[mipLevel];
            int x, y;

            Bitmap bmp = new Bitmap(mip.Width, mip.Height, PixelFormat.Format32bppArgb);
            byte[] data;
            byte[] dest;

            switch (Format)
            {
                case D3DFormat.PVRTC4:
                    data = mip.Data;
                    dest = new byte[mip.Width * mip.Height * 4];

                    PVTRC.Decompress(data, false, mip.Width, mip.Height, true, dest);

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 4;

                            Color c = Color.FromArgb(dest[offset + 3], dest[offset + 0], dest[offset + 1], dest[offset + 2]);
                            bmp.SetPixel(x, y, c);
                        }
                    }
                    break;

                case D3DFormat.R4G4B4A4:
                    data = mip.Data;

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 2;
                            int pixel = BitConverter.ToUInt16(data, offset);
                            bmp.SetPixel(x, y, ColorHelper.R4G4B4A4ToColor(pixel));
                        }
                    }
                    break;

                case D3DFormat.R5G5B6:
                    data = mip.Data;

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 2;
                            int pixel = BitConverter.ToUInt16(data, offset);
                            bmp.SetPixel(x, y, ColorHelper.R5G5B6ToColor(pixel));
                        }
                    }
                    break;

                case D3DFormat.A8B8G8R8:
                    data = mip.Data;

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 4;
                            uint pixel = BitConverter.ToUInt32(data, offset);
                            bmp.SetPixel(x, y, ColorHelper.A8B8G8R8ToColor(pixel));
                        }
                    }
                    break;

                case D3DFormat.R5G6B5:
                    data = mip.Data;

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 2;
                            int pixel = BitConverter.ToUInt16(data, offset);
                            bmp.SetPixel(x, y, ColorHelper.R5G6B5ToColor(pixel));
                        }
                    }
                    break;

                case D3DFormat.A8R8G8B8:
                    data = mip.Data;

                    int pixelSize = Flags.HasFlag(TDXFlags.AlphaNbit) ? 4 : 3;

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * pixelSize;
                            int a = pixelSize == 4 ? data[offset + 3] : 255;

                            Color c = Color.FromArgb(a, data[offset + 0], data[offset + 1], data[offset + 2]);
                            bmp.SetPixel(x, y, c);
                        }
                    }
                    break;

                case D3DFormat.DXT1:
                    data = mip.Data;
                    dest = new byte[mip.Width * mip.Height * 4];

                    Squish.Squish.DecompressImage(dest, mip.Width, mip.Height, data, SquishFlags.kDxt1);

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 4;

                            Color c = Color.FromArgb(dest[offset + 3], dest[offset + 0], dest[offset + 1], dest[offset + 2]);
                            bmp.SetPixel(x, y, c);
                        }
                    }
                    break;

                case D3DFormat.DXT5:
                    data = mip.Data;
                    dest = new byte[mip.Width * mip.Height * 4];

                    Squish.Squish.DecompressImage(dest, mip.Width, mip.Height, data, SquishFlags.kDxt5);

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 4;

                            Color c = Color.FromArgb(dest[offset + 3], dest[offset + 0], dest[offset + 1], dest[offset + 2]);
                            bmp.SetPixel(x, y, c);
                        }
                    }
                    break;

                case D3DFormat.ATI2:
                    data = mip.Data;

                    dest = BC5Unorm.Decompress(data, (uint)mip.Width, (uint)mip.Height);

                    for (y = 0; y < mip.Height; y++)
                    {
                        for (x = 0; x < mip.Width; x++)
                        {
                            int offset = (x + y * mip.Width) * 4;

                            Color c = Color.FromArgb(dest[offset + 3], dest[offset + 2], dest[offset + 1], dest[offset + 0]);
                            bmp.SetPixel(x, y, c);
                        }
                    }
                    break;
            }

            return bmp;
        }

        //public int GetMipLevelForSize(int maxDimension)
        //{
        //    for (int i = 0; i < MipMaps.Count; i++)
        //    {
        //        if (MipMaps[i].Width <= maxDimension || MipMaps[i].Height <= maxDimension)
        //        {
        //            return i;
        //        }
        //    }

        //    return 0;
        //}

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
                bw.Write((int)tdx.Flags);
                bw.Write((int)tdx.Format);

                for (int i = 0; i < tdx.MipMaps.Count; i++) { bw.Write(tdx.MipMaps[i].Data); }

                bw.Flush();
                ms.Flush();

                buffer = ms.ToArray();
            }

            return buffer;
        }

        public static explicit operator DDS(TDX tdx)
        {
            DDS dds = new DDS
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
        public FontDefinition(byte[] _)
        {

        }
    }

    public class AnimationDefinition : TDXExtraData
    {
        public AnimationDefinition(byte[] _)
        {

        }
    }
}