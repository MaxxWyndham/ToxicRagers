using System.Collections.Generic;

using ToxicRagers.Helpers;

namespace ToxicRagers.Generics
{
    public abstract class Texture
    {
        string name;
        protected string extension;
        D3DFormat format;
        protected List<MipMap> mipMaps = new List<MipMap>();

        public string Name
        {
            get => name;
            set => name = value;
        }

        public virtual D3DFormat Format
        {
            get => format;
            set => format = value;
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

        public List<MipMap> MipMaps => mipMaps;
    }

    public class MipMap
    {
        int width;
        int height;
        byte[] data;

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

        public byte[] Data
        {
            get => data;
            set => data = value;
        }
    }
}