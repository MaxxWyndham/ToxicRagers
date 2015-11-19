using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ToxicRagers.Stainless.Formats;
using ToxicRagers.Core.Formats;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class crVTMapEntry
    {
        int column;
        int row;
        int width;
        int height;
        string fileName;

        int timeStamp;
        int unknown2;
        Dictionary<string, crVTMapTile> tiles = new Dictionary<string, crVTMapTile>();

        public Dictionary<string, crVTMapTile> Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }
        public int TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }
        public int Unknown2
        {
            get { return unknown2; }
            set { unknown2 = value; }
        }
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
        public int Column
        {
            get { return column; }
            set { column = value; }
        }
        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public int NumTiles
        {
            get { return (width / 120) * (height / 120); }
        }
        public int GetWidth(int page)
        {
            if (page == 0) return Width;
            int divisor = 1;
            for (int i = 1; i <= page; i++, divisor *= 2) { }
            return Width / divisor;
        }
        public int GetHeight(int page)
        {
            if (page == 0) return Height;
            int divisor = 1;
            for (int i = 1; i <= page; i++, divisor *= 2) { }
            return Height / divisor;
        }
        public int GetPaddingX(int page)
        {
            int pageWidth = GetWidth(page);
            if (pageWidth == 2048) return 72;
            else if (pageWidth == 1024) return 36;
            else if (pageWidth == 512) return 20;
            else if (pageWidth == 256) return 12;
            else if (pageWidth == 128) return 8;
            else if (pageWidth < 128) return 4;
            else return 0;
        }
        public int GetPaddingY(int page)
        {
            int pageWidth = GetHeight(page);
            if (pageWidth == 2048) return 72;
            else if (pageWidth == 1024) return 36;
            else if (pageWidth == 512) return 20;
            else if (pageWidth == 256) return 12;
            else if (pageWidth == 128) return 8;
            else if (pageWidth < 128) return 4;
            else return 0;
        }
        public override string ToString()
        {
            return Path.GetFileName(FileName);
        }
    }
}
