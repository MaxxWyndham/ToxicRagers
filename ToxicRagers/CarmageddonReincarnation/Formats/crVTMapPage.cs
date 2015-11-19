using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class crVTMapPage
    {
        List<List<crVTMapTile>> tiles = new List<List<crVTMapTile>>();
        int maxTilesX = 0;
        int maxTilesY = 0;

        public int PageNumber
        {
            get;
            set;
        }
        public int MaxTilesX { get { return maxTilesX; } set { maxTilesX = value; } }
        public int MaxTilesY { get { return maxTilesY; } set { maxTilesY = value; } }
        public List<List<crVTMapTile>> Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }
        public int Width
        {
            get { return (MaxTilesX + 1) * 120; }
        }
        public int Height
        {
            get { return (MaxTilesY + 1) * 120; }
        }
        public void AddTile(crVTMapTile tile)
        {
            if (Tiles.Count <= tile.Row)
            {
                for (int x = Tiles.Count; x <= tile.Row; x++)
                {
                    Tiles.Add(new List<crVTMapTile>());
                }
            }
            if (Tiles[tile.Row].Count <= tile.Column)
            {
                for (int x = Tiles[tile.Row].Count; x <= tile.Column; x++)
                {
                    Tiles[tile.Row].Add(null);
                }
            }
            if (Tiles[tile.Row][tile.Column] == null)
            {
                Tiles[tile.Row].Insert(tile.Column, tile);
            }
            else
            {
                //Logger.LogToFile("Tile already exists at [{0}, {1}] in Tiles Page #{2} \"{3}\" ( other tile: {4})", tile.Row, tile.Column, PageNumber, tile.TileNameString, Tiles[tile.Row][tile.Column].TileNameString);
            }
            if (tile.Row > MaxTilesY) MaxTilesY = tile.Row;
            if (tile.Column > MaxTilesX) MaxTilesX = tile.Column;


        }
    }
}
