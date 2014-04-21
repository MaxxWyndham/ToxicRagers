using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

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

        public void Save(string Path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path, FileMode.Create)))
            {
                bw.Write(new byte[] { 0, 2 });

                bw.Write((short)this.MipMaps[0].Width);
                bw.Write((short)this.MipMaps[0].Height);
                bw.Write((short)this.MipMaps.Count);
                bw.Write((int)this.flags);
                bw.Write(this.ShortFormat);

                for (int i = 0; i < this.MipMaps.Count; i++) { bw.Write(this.MipMaps[i].Data); }
            }
        }
    }
}
