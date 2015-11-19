using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class crVTMapTile
    {
        int column;
        int row;
        int page;
        byte[] tileName = new byte[4];


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
        public byte[] TileName
        {
            get { return tileName; }
            set { tileName = value; }
        }
        public string TileNameString
        {
            get { return BitConverter.ToString(new byte[] { tileName[3], tileName[2], tileName[1], tileName[0] }).Replace("-", string.Empty); }
        }
        public crVTMapTileTDX TDXTile
        {
            get;
            set;
        }
    }
}
