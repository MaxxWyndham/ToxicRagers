using System.Collections.Generic;

using ToxicRagers.Helpers;

namespace ToxicRagers.Generics
{
    public abstract class Texture
    {
        public string Name { get; set; }

        public string Extension { get; protected set; }

        public List<MipMap> MipMaps { get; private set; } = new List<MipMap>();

        public virtual D3DFormat Format { get; protected set; }

        public string ShortFormat
        {
            get
            {
                switch (Format)
                {
                    case D3DFormat.A8R8G8B8:
                        return "8888";

                    default:
                        return Format.ToString().Substring(0, 4);
                }
            }
        }

        public void SetFormat(D3DFormat format)
        {
            Format = format;
        }
    }

    public class MipMap
    {
        public int Width { get; set; }

        public int Height { get; set; }
        
        public byte[] Data { get; set; }
    }
}