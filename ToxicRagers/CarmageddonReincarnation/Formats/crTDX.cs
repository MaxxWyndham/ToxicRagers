using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
            Unknown4 = 4,
            Unknown8 = 8,
            Unknown64 = 64,
            Unknown128 = 128,
            Unknown256 = 256,
            ExtraData = 512,
            Unknown1024 = 1024,
            Unknown16384 = 16384
        }

        int width;
        int height;
        Flags flags;

        public TDX()
            : base()
        {
            this.extension = "TDX";
        }

        public static TDX Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            TDX tdx = new TDX();

            tdx.Name = fi.Name.Replace(fi.Extension, "");

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0x02)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid TDX file", path);
                    return null;
                }

                tdx.width = (int)br.ReadUInt16();
                tdx.height = (int)br.ReadUInt16();
                int mipCount = (int)br.ReadUInt16();
                tdx.flags = (Flags)br.ReadUInt32();
                tdx.Format = (D3DFormat)br.ReadUInt32();

                if (tdx.flags.HasFlag(Flags.ExtraData))
                {
                    int extraDataLength = (int)br.ReadUInt32();
                    int extraDataType = br.ReadUInt16();

                    switch (extraDataType)
                    {
                        case 0:
                            /* font */
                            Logger.LogToFile(Logger.LogLevel.Info, "Skipped {0} bytes of extra data", extraDataLength);
                            br.ReadBytes(extraDataLength - 2);
                            break;

                        case 1:
                            /* animation */
                            Logger.LogToFile(Logger.LogLevel.Info, "Skipped {0} bytes of extra data", extraDataLength);
                            br.ReadBytes(extraDataLength - 2);
                            break;

                        case 3:
                            /* vt dictionary */
                            int textureType = br.ReadUInt16(); // 2 = Diffuse, 3 = Normal, 4 = Specular

                            br.ReadUInt32(); // 0xdeadbeef

                            int sheetWidth = (int)br.ReadUInt32();
                            int sheetHeight = (int)br.ReadUInt32();
                            int mipLevels = (int)br.ReadUInt32();
                            int tileSizeNoPadding = (int)br.ReadUInt32();
                            int tilePadding = (int)br.ReadUInt32();

                            br.ReadUInt32(); // 0xdeadbeef

                            int fileCount = (int)br.ReadUInt32();

                            for (int i = 0; i < fileCount; i++)
                            {
                                int x = (int)br.ReadUInt32();
                                int y = (int)br.ReadUInt32();
                                int w = (int)br.ReadUInt32();
                                int h = (int)br.ReadUInt32();
                                string file = br.ReadNullTerminatedString();
                                br.ReadByte(); // padding?
                            }

                            br.ReadUInt32(); // 0xdeadbeef

                            int indexCount = (int)br.ReadUInt32();

                            for (int i = 0; i < indexCount; i++)
                            {
                                int row = (int)br.ReadUInt32();
                                int col = (int)br.ReadUInt32();
                                int level = (int)br.ReadUInt32();
                                uint hash = br.ReadUInt32();

                                string tileName = string.Format("{0:x8}", hash);
                                string zadTileName = string.Format("{0}/{1}_{2}.tdx", tileName.Substring(0, 2), tileName, (textureType == 2 ? "D" : (textureType == 3 ? "N" : "S")));
                            }

                            br.ReadUInt32(); // 0xdeadbeef

                            fileCount = (int)br.ReadUInt32();

                            for (int i = 0; i < fileCount; i++)
                            {
                                string file = br.ReadNullTerminatedString();
                                int timestamp = (int)br.ReadUInt32();
                                br.ReadUInt32(); // padding?
                            }

                            br.ReadUInt32(); // 0xdeadbeef
                            br.ReadUInt32(); // padding?
                            br.ReadUInt32(); // 0xdeadbeef
                            break;

                        default:
                            throw new NotImplementedException(string.Format("Unknown Extra Data flag: {0}", extraDataType));
                    }
                }

                for (int i = 0; i < mipCount; i++)
                {
                    var mip = new MipMap();
                    mip.Width = tdx.width >> i;
                    mip.Height = tdx.height >> i;

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

        public int GetMipLevelForSize(int maxDimension)
        {
            for (int i = 0; i < this.MipMaps.Count; i++)
            {
                if (this.MipMaps[i].Width <= maxDimension || this.MipMaps[i].Height <= maxDimension)
                {
                    return i;
                }
            }

            return 0;
        }

        public Bitmap Decompress(int mipLevel = 0, bool bSuppressAlpha = false)
        {
            var mip = this.MipMaps[mipLevel];

            Bitmap b = new Bitmap(mip.Width, mip.Height, PixelFormat.Format32bppArgb);
            Squish.SquishFlags flags = 0;
            bool bNotCompressed = false;

            switch (this.Format)
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

                default:
                    throw new NotImplementedException(string.Format("Can't decompress: {0}", this.Format));
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

            var bmpdata = b.LockBits(new Rectangle(0, 0, mip.Width, mip.Height), ImageLockMode.ReadWrite, (bSuppressAlpha ? PixelFormat.Format32bppRgb : b.PixelFormat));
            System.Runtime.InteropServices.Marshal.Copy(dest, 0, bmpdata.Scan0, dest.Length);
            b.UnlockBits(bmpdata);

            return b;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                bw.Write(new byte[] { 0, 2 });

                bw.Write((short)this.MipMaps[0].Width);
                bw.Write((short)this.MipMaps[0].Height);
                bw.Write((short)this.MipMaps.Count);
                bw.Write((int)this.flags);
                bw.WriteString(this.ShortFormat);

                for (int i = 0; i < this.MipMaps.Count; i++) { bw.Write(this.MipMaps[i].Data); }
            }
        }

        public void SaveAsDDS(string path)
        {
            DDS dds = new DDS();
            dds.Width = this.MipMaps[0].Width;
            dds.Height = this.MipMaps[0].Height;
            dds.Format = this.Format;
            dds.Data = this.MipMaps[0].Data;
            dds.Save(path);
        }
    }
}
