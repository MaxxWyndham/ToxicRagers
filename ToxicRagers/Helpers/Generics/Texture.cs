using System;
using System.Collections.Generic;

namespace ToxicRagers.Helpers
{
    public abstract class Texture
    {
        string name;
        protected string extension;
        D3DFormat format;
        List<MipMap> mipMaps = new List<MipMap>();

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual D3DFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        public string ShortFormat
        {
            get
            {
                switch (format)
                {
                    case D3DFormat.A8R8G8B8:
                        return "8888";

                    default:
                        return format.ToString().Substring(0, 4);
                }
            }
        }

        public List<MipMap> MipMaps { get { return mipMaps; } }
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
    }
}
