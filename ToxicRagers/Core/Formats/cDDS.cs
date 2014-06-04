using System;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    public class DDS : Texture
    {
        [Flags]
        public enum Flags
        {
            Caps = 1,
            Height = 2,
            Width = 4,
            Pitch = 8,
            PixelFormat = 4096,
            MipMapCount = 131072,
            LinearSize = 524288,
            DepthTexture = 8388608
        }

        int height;
        int width;
        int depth = 0;
        byte[] data;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path + ".dds", FileMode.Create)))
            {
                bw.Write(new byte[] { 0x44, 0x44, 0x53, 0x20 });    // 'DDS '
                bw.Write(124);
                bw.Write((int)(Flags.Caps | Flags.Height | Flags.Width | Flags.PixelFormat | Flags.LinearSize));

                bw.Write(this.height);
                bw.Write(this.width);
                bw.Write(this.data.Length);
                bw.Write(this.depth);
                bw.Write(this.MipMaps.Count);

                for (int i = 0; i < 11; i++) { bw.Write(0); }

                // PixelFormat
                bw.Write(32);
                switch (Format)
                {
                    case D3DFormat.DXT1:
                    case D3DFormat.DXT5:
                        bw.Write(4);
                        break;
                }
                bw.Write(Format.ToString().ToCharArray());
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);

                bw.Write(4096);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);

                bw.Write(this.data);
            }
        }
    }
}
