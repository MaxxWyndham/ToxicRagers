using System.Collections.Generic;
using System.IO;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTMapEntry
    {
        public Dictionary<string, VTMapTile> Tiles { get; set; } = new Dictionary<string, VTMapTile>();

        public VTMap Map { get; set; }

        public int TimeStamp { get; set; }

        public int Unknown2 { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Column { get; set; }

        public int Row { get; set; }

        public string FileName { get; set; }

        public int NumTiles => Width / 120 * (Height / 120);

        public int GetWidth(int page)
        {
            if (page == 0) { return Width; }

            int divisor = 1;
            for (int i = 1; i <= page; i++, divisor *= 2) { }
            return Width / divisor;
        }

        public int GetHeight(int page)
        {
            if (page == 0) { return Height; }

            int divisor = 1;
            for (int i = 1; i <= page; i++, divisor *= 2) { }
            return Height / divisor;
        }

        public int GetPadding(int pageWidth)
        {
            switch (pageWidth)
            {
                case 2048:
                    return 72;

                case 1024:
                    return 36;

                case 512:
                    return 20;

                case 256:
                    return 12;

                case 128:
                    return 8;

                default:
                    if (pageWidth < 128) { return 4; }
                    return 0;
            }
        }

        public override string ToString()
        {
            return Path.GetFileName(FileName);
        }
    }
}