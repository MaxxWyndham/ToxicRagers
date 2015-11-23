using System;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTMapTile
    {
        int column;
        int row;
        int page;
        uint hash;
        string tileName;
        string zadTileName;

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

        public int Page
        {
            get { return page; }
            set { page = value; }
        }

        public uint Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        public string TileName
        {
            get { return tileName; }
            set { tileName = value; }
        }

        public string ZadTileName
        {
            get { return zadTileName; }
            set { zadTileName = value; }
        }

        public VTMapTileTDX TDXTile
        {
            get;
            set;
        }
    }
}