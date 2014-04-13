using System;
using System.Collections.Generic;

namespace ToxicRagers.Helpers
{
    public abstract class Texture
    {
        string name;
        string format;
        List<MipMap> mipMaps = new List<MipMap>();

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual string Format
        {
            get { return format; }
            set { format = value; }
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
