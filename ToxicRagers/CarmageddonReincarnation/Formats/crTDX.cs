using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class TDX
    {
        string name;
        int width;
        int height;
        string format;
        public List<MipMap> mipMaps = new List<MipMap>();

        public string Name { get { return name; } }

        public static TDX Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            TDX tdx = new TDX();

            tdx.name = fi.Name.Replace(fi.Extension, "");

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
                int flags = (int)br.ReadUInt32();
                tdx.format = br.ReadString(4);

                for (int i = 0; i < mipCount; i++)
                {
                    var mip = new MipMap();
                    mip.Width = tdx.width >> i;
                    mip.Height = tdx.height >> i;
                    mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 16);

                    if (tdx.format == "DXT5") { mip.Decompress(); }
                    tdx.mipMaps.Add(mip);
                }
            }

            return tdx;
        }
    }

    public class MipMap
    {
        int width;
        int height;
        byte[] data;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public void Decompress()
        {
        }
    }
}
