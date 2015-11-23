using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.CarmageddonReincarnation.Formats;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTMap : TDXExtraData
    {
        public enum VTMapType
        {
            Unknown = 1,
            DiffuseMap = 2,
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
        List<VTMapEntry> entries = new List<VTMapEntry>();
        Dictionary<string, VTMapTileTDX> tilesByName = new Dictionary<string, VTMapTileTDX>();
        List<VTMapPage> tilesPages = new List<VTMapPage>();

        public Dictionary<string, VTMapTileTDX> TilesByName
        {
            get { return tilesByName; }
            set { tilesByName = value; }
        }
        public List<VTMapPage> TilesPages
        {
            get { return tilesPages; }
            set { tilesPages = value; }
        }
        public List<VTMapEntry> Entries
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

        public VTMap(byte[] buff)
        {
            using (MemoryStream ms = new MemoryStream(buff))
            using (BinaryReader br = new BinaryReader(ms))
            {
                type = (VTMapType)br.ReadInt16();

                if (!IsDeadBeef(br))
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "OH FUCK IT'S NOT DEADBEEF #1!\nPosition: {0:x2}", br.BaseStream.Position);
                    return;
                }

                width = br.ReadInt32();
                height = br.ReadInt32();
                pageCount = br.ReadInt32();
                tileSize = br.ReadInt32();
                tilePadding = br.ReadInt32();

                if (!IsDeadBeef(br))
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "OH FUCK IT'S NOT DEADBEEF #2!\nPosition: {0:x2}", br.BaseStream.Position);
                    return;
                }

                textureCount = br.ReadInt32();

                for (int i = 0; i < textureCount; i++)
                {
                    VTMapEntry entry = new VTMapEntry();
                    entry.Row = br.ReadInt32();
                    entry.Column = br.ReadInt32();
                    entry.Width = br.ReadInt32();
                    entry.Height = br.ReadInt32();
                    entry.FileName = br.ReadNullTerminatedString();
                    br.ReadByte();

                    entries.Add(entry);
                }

                if (!IsDeadBeef(br))
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "OH FUCK IT'S NOT DEADBEEF #3!\nPosition: {0:x2}", br.BaseStream.Position);
                    return;
                }

                numberOfTiles = br.ReadInt32();
                int j = 0;
                for (int i = 0; i < numberOfTiles; i++)
                {
                    VTMapTile tile = new VTMapTile();
                    tile.Column = br.ReadInt32();
                    tile.Row = br.ReadInt32();
                    tile.Page = br.ReadInt32();

                    tile.Hash = br.ReadUInt32();
                    tile.TileName = string.Format("{0:x8}", tile.Hash);
                    tile.ZadTileName = string.Format("{0}/{1}_{2}.tdx", tile.TileName.Substring(0, 2), tile.TileName, type.ToString().Substring(0, 1));

                    //Logger.LogToFile("\tTile {0}\n\t\t FileName: {1}\n\t\tUnknown1: {2}\n\t\tUnknown2: {3}\n\t\tUnknown3: {4}", j, BitConverter.ToString( tile.TileName), tile.Unknown1, tile.Unknown2, tile.Unknown3);
                    j++;
                    if (!TilesByName.ContainsKey(tile.TileName))
                    {
                        VTMapTileTDX tileTDX = new VTMapTileTDX();
                        tileTDX.TileName = tile.TileName;
                        tileTDX.Coords.Add(tile);
                        TilesByName.Add(tile.TileName, tileTDX);
                    }
                    else
                    {
                        //Logger.LogToFile("Tile #{0} \"{1}\" already in TilesByName", i, tile.TileNameString);
                        TilesByName[tile.TileName].Coords.Add(tile);
                    }
                    if (tile.Page >= TilesPages.Count)
                    {
                        for (int x = TilesPages.Count; x <= tile.Page; x++)
                        {
                            TilesPages.Add(new VTMapPage() { PageNumber = tile.Page });
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
                    string fileName = br.ReadNullTerminatedString();

                    if (entries[i].FileName == fileName)
                    {
                        entries[i].TimeStamp = br.ReadInt32();
                        entries[i].Unknown2 = br.ReadInt32();
                        if (!unknownValues.Contains(entries[i].TimeStamp)) unknownValues.Add(entries[i].TimeStamp);
                    }
                    else
                    {
                        for (int k = 0; k < entries.Count; k++)
                        {
                            if (entries[k].FileName == fileName)
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

        public bool IsDeadBeef(BinaryReader br)
        {
            return (br.ReadByte() == 0xde &&
                    br.ReadByte() == 0xad &&
                    br.ReadByte() == 0xbe &&
                    br.ReadByte() == 0xef);
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
                    sb.AppendFormat("\tTile {0}\n\t\t FileName: {1}\n\t\tColumn: {2}\n\t\tRow: {3}\n\t\tPage: {4}\n", j, tile.TileName, tile.Column, tile.Row, tile.Page);
                    j++;
                }
            }
            Logger.LogToFile(Logger.LogLevel.Debug, sb.ToString());
        }
    }
}