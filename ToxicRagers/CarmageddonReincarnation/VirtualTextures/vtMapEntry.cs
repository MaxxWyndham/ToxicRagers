using System.Collections.Generic;
using System.IO;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTMapEntry
    {
        int column;
        int row;
        int width;
        int height;
        string fileName;

        int timeStamp;
        int unknown2;
        Dictionary<string, VTMapTile> tiles = new Dictionary<string, VTMapTile>();

        public Dictionary<string, VTMapTile> Tiles
        {
            get => tiles;
            set => tiles = value;
        }

        public int TimeStamp
        {
            get => timeStamp;
            set => timeStamp = value;
        }

        public int Unknown2
        {
            get => unknown2;
            set => unknown2 = value;
        }

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

        public int Column
        {
            get => column;
            set => column = value;
        }

        public int Row
        {
            get => row;
            set => row = value;
        }

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        public int NumTiles => (width / 120) * (height / 120);

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