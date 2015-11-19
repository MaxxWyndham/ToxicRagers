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
    public class crVTMap
    {
        public enum VTMapType
        {
            Unknown = 1,
            DiffuseMap =  2,
            NormalMap = 3,
            SpecularMap = 4
        }
        VTMapType type;
        int width;
        int height;
        int pageCount;
        int tileSize;
        int tilePadding;
        int textureCount;
        int numberOfTiles;
        List<crVTMapEntry> entries = new List<crVTMapEntry>();
        Dictionary<string, crVTMapTileTDX> tilesByName = new Dictionary<string, crVTMapTileTDX>();
        List<crVTMapPage> tilesPages = new List<crVTMapPage>();

        public Dictionary<string, crVTMapTileTDX> TilesByName
        {
            get { return tilesByName; }
            set { tilesByName = value; }
        }
        public List<crVTMapPage> TilesPages
        {
            get { return tilesPages; }
            set { tilesPages = value; }
        }
        public List<crVTMapEntry> Entries
        {
            get { return entries; }
            set { entries = value; }
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
        public VTMapType Type
        {
            get { return type; }
            set { type = value; }
        }
        public int PageCount
        {
            get { return pageCount; }
            set { pageCount = value; }
        }
        public int TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }
        public int TilePadding
        {
            get { return tilePadding; }
            set { tilePadding = value; }
        }
        public int NumberOfTiles
        {
            get { return numberOfTiles; }
            set { numberOfTiles = value; }
        }

        public int TextureCount
        {
            get { return textureCount; }
            set { textureCount = value; }
        }

        public crVTMap(byte[] buff)
        {

            using (MemoryStream ms = new MemoryStream(buff))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    type = (VTMapType)br.ReadInt16();
                    if (!IsDeadBeef(br))
                    {
                        Logger.LogToFile(Logger.LogLevel.Error, "OH FUCK IT'S NOT DEADBEEF #1!\nPosition: {0}", br.BaseStream.Position);
                        return;
                    }
                    width = br.ReadInt32();
                    height = br.ReadInt32();
                    PageCount = br.ReadInt32();
                    TileSize = br.ReadInt32();
                    TilePadding = br.ReadInt32();


                    if (!IsDeadBeef(br))
                    {
                        Logger.LogToFile(Logger.LogLevel.Error, "OH FUCK IT'S NOT DEADBEEF #2!\nPosition: {0}", br.BaseStream.Position);
                        return;
                    }
                    textureCount = br.ReadInt32();

                    for (int i = 0; i < textureCount; i++)
                    {
                        crVTMapEntry entry = new crVTMapEntry();
                        entry.Row = br.ReadInt32();
                        entry.Column = br.ReadInt32();
                        entry.Width = br.ReadInt32();
                        entry.Height = br.ReadInt32();
                        StringBuilder sb = new StringBuilder();
                        byte letter = br.ReadByte();
                        bool foundNull = false;
                        while (letter != 0)
                        {
                            sb.Append((char)letter);
                            letter = br.ReadByte();
                        }
                        
                        br.ReadByte();

                        entry.FileName = sb.ToString();
                        entries.Add(entry);
                    }

                    if (!IsDeadBeef(br))
                    {
                        Logger.LogToFile(Logger.LogLevel.Error, "OH FUCK IT'S NOT DEADBEEF #3!\nPosition: {0}", br.BaseStream.Position);
                        return;
                    }
                    numberOfTiles = br.ReadInt32();
                    int j = 0;
                    for (int i = 0; i < numberOfTiles; i++)
                    {
                        crVTMapTile tile = new crVTMapTile();
                        tile.Column = br.ReadInt32();
                        tile.Row = br.ReadInt32();
                        tile.Page = br.ReadInt32();
                        
                        tile.TileName = br.ReadBytes(4);
                        //Logger.LogToFile("\tTile {0}\n\t\t FileName: {1}\n\t\tUnknown1: {2}\n\t\tUnknown2: {3}\n\t\tUnknown3: {4}", j, BitConverter.ToString( tile.TileName), tile.Unknown1, tile.Unknown2, tile.Unknown3);
                        j++;
                        if (!TilesByName.ContainsKey(tile.TileNameString))
                        {
                            crVTMapTileTDX tileTDX = new crVTMapTileTDX();
                            tileTDX.TileName = tile.TileName;
                            tileTDX.Coords.Add(tile);
                            TilesByName.Add(tile.TileNameString, tileTDX);
                        }
                        else
                        {
                            //Logger.LogToFile("Tile #{0} \"{1}\" already in TilesByName", i, tile.TileNameString);
                            TilesByName[tile.TileNameString].Coords.Add(tile);
                        }
                        if (tile.Page >= TilesPages.Count)
                        {
                            for (int x = TilesPages.Count; x <= tile.Page; x++)
                            {
                                TilesPages.Add(new crVTMapPage() { PageNumber = tile.Page });
                            }
                        }
                        TilesPages[tile.Page].AddTile(tile);
                    }
                    if (!IsDeadBeef(br))
                    {
                        Logger.LogToFile(Logger.LogLevel.Error, "OH FUCK IT'S NOT DEADBEEF #4!\nPosition: {0}", br.BaseStream.Position);
                        return;
                    }
                    int anotherTextureCount = br.ReadInt32();
                    List<int> unknownValues = new List<int>();
                    for (int i = 0; i < anotherTextureCount; i++)
                    {

                        StringBuilder sb = new StringBuilder();
                        byte letter = br.ReadByte();
                        bool foundNull = false;
                        while (letter != 0)
                        {
                            sb.Append((char)letter);
                            letter = br.ReadByte();
                        }

                        if (entries[i].FileName == sb.ToString())
                        {
                            entries[i].TimeStamp = br.ReadInt32();
                            entries[i].Unknown2 = br.ReadInt32();
                            if (!unknownValues.Contains(entries[i].TimeStamp)) unknownValues.Add(entries[i].TimeStamp);
                        }
                        else
                        {
                            for (int k = 0; k < entries.Count; k++)
                            {
                                if (entries[k].FileName == sb.ToString())
                                {
                                    entries[k].TimeStamp = br.ReadInt32();
                                    entries[k].Unknown2 = br.ReadInt32();
                                    if (!unknownValues.Contains(entries[k].TimeStamp)) unknownValues.Add(entries[k].TimeStamp);
                                    break;
                                }
                            }
                        }
                        
                    }
                }
            }
        }
        public bool IsDeadBeef(BinaryReader br)
        {
            var ef = br.ReadByte();
            var be = br.ReadByte();
            var ad = br.ReadByte();
            var de = br.ReadByte();
            return ef == 0xEF && be == 0xBE && ad == 0xAD && de == 0xDE;
        }
        public int GetWidth(int page)
        {
            return TilesPages[page].Tiles.Count > 0 ? TilesPages[page].Width : Width;
            
        }
        public int GetHeight(int page)
        {
            return TilesPages[page].Tiles.Count > 0 ? TilesPages[page].Height : Height;
            
        }
        public void LogToConsole()
        {
            Logger.LogToFile(Logger.LogLevel.Debug, "Logging a VT Map:\n\tType: {0}\n\tPageCount: {1}\n\tTileSize: {2}\n\tTilePadding: {3}\n\tNumber Of Tiles: {4}\n\tWidth: {5}\n\tHeight: {6}\n\tTextureCount: {7}\n\tEntries.Count: {8}", type, pageCount, TileSize, tilePadding, numberOfTiles, width, height, textureCount, entries.Count);
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var entry in entries)
            {
                sb.AppendFormat("\tEntry {0}:\n\t\tColumn: {1}\n\t\tRow: {2}\n\t\tWidth: {3}\n\t\tHeight:{4}\n\t\tName: {5}\n\t\tTimestamp: {6}\n\t\tUnknown 2: {7}\n", i, entry.Column, entry.Row, entry.Width, entry.Height, entry.FileName, entry.TimeStamp, entry.Unknown2);
                i++;
            }
            Logger.LogToFile(Logger.LogLevel.Debug, sb.ToString());
            int j = 0;
            sb.Clear();
            foreach (var keyval in TilesByName)
            {
                for (int k = 0; k < keyval.Value.Coords.Count; k++)
                {
                    var tile = keyval.Value.Coords[k];
                    sb.AppendFormat("\tTile {0}\n\t\t FileName: {1}\n\t\tColumn: {2}\n\t\tRow: {3}\n\t\tPage: {4}\n", j, tile.TileNameString, tile.Column, tile.Row, tile.Page);
                    j++;
                }
            }
            Logger.LogToFile(Logger.LogLevel.Debug, sb.ToString());
        }
    }
}
