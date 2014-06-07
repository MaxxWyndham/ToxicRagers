using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ToxicRagers.Core.Formats;
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

        public static TDX Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            TDX tdx = new TDX();

            tdx.Name = fi.Name.Replace(fi.Extension, "");

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 0 ||
                    br.ReadByte() != 2)
                {
                    Logger.LogToFile("{0} isn't a valid TDX file", Path);
                    return null;
                }

                tdx.width = (int)br.ReadUInt16();
                tdx.height = (int)br.ReadUInt16();
                int mipCount = (int)br.ReadUInt16();
                tdx.flags = (Flags)br.ReadUInt32();
                tdx.Format = (D3DFormat)br.ReadUInt32();

                if ((tdx.flags & Flags.ExtraData) == Flags.ExtraData)
                {
                    // Frames list usually
                    br.ReadBytes((int)br.ReadUInt32());
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

                        case D3DFormat.DXT1:
                            mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 8);
                            break;

                        case D3DFormat.ATI2:
                        case D3DFormat.DXT5:
                            mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 16);
                            break;

                        default:
                            Logger.LogToFile("Unknown format: {0}", tdx.Format);
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
