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
            get => tilesByName;
            set => tilesByName = value;
        }

        public List<VTMapPage> TilesPages
        {
            get => tilesPages;
            set => tilesPages = value;
        }

        public List<VTMapEntry> Entries
        {
            get => entries;
            set => entries = value;
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

        public VTMapType Type
        {
            get => type;
            set => type = value;
        }

        public int PageCount
        {
            get => pageCount;
            set => pageCount = value;
        }

        public int TileSize
        {
            get => tileSize;
            set => tileSize = value;
        }

        public int TilePadding
        {
            get => tilePadding;
            set => tilePadding = value;
        }

        public int NumberOfTiles
        {
            get => numberOfTiles;
            set => numberOfTiles = value;
        }

        public int TextureCount
        {
            get => textureCount;
            set => textureCount = value;
        }

        public VTMap(byte[] buff)
        {
            using (MemoryStream ms = new MemoryStream(buff))
            using (BinaryReader br = new BinaryReader(ms))
            {
                type = (VTMapType)br.ReadInt16();

                if (!IsDeadBeef(br))
                {
                    //Logger.LogToFile(Logger.LogLevel.Error, "Unexpected data at {0:x2}", br.BaseStream.Position);
                    return;
                }

                width = br.ReadInt32();
                height = br.ReadInt32();
                pageCount = br.ReadInt32();
                tileSize = br.ReadInt32();
                tilePadding = br.ReadInt32();

                if (!IsDeadBeef(br))
                {
                    //Logger.LogToFile(Logger.LogLevel.Error, "Unexpected data at {0:x2}", br.BaseStream.Position);
                    return;
                }

                textureCount = br.ReadInt32();

                for (int i = 0; i < textureCount; i++)
                {
                    VTMapEntry entry = new VTMapEntry()
                    {
                        Row = br.ReadInt32(),
                        Column = br.ReadInt32(),
                        Width = br.ReadInt32(),
                        Height = br.ReadInt32(),
                        FileName = br.ReadNullTerminatedString(),
                        Map = this
                    };

                    br.ReadByte();

                    entries.Add(entry);
                }

                if (!IsDeadBeef(br))
                {
                    //Logger.LogToFile(Logger.LogLevel.Error, "Unexpected data at {0:x2}", br.BaseStream.Position);
                    return;
                }

                numberOfTiles = br.ReadInt32();

                for (int i = 0; i < numberOfTiles; i++)
                {
                    VTMapTile tile = new VTMapTile()
                    {
                        Column = br.ReadInt32(),
                        Row = br.ReadInt32(),
                        Page = br.ReadInt32(),

                        Hash = br.ReadUInt32()
                    };

                    tile.TileName = string.Format("{0:x8}", tile.Hash);
                    tile.ZadTileName = string.Format("{0}/{1}_{2}.tdx", tile.TileName.Substring(0, 2), tile.TileName, type.ToString().Substring(0, 1));

                    if (!TilesByName.ContainsKey(tile.TileName.ToUpper())) { TilesByName.Add(tile.TileName.ToUpper(), new VTMapTileTDX { TileName = tile.TileName }); }

                    TilesByName[tile.TileName.ToUpper()].Coords.Add(tile);

                    if (tile.Page >= TilesPages.Count)
                    {
                        for (int x = TilesPages.Count; x <= tile.Page; x++)
                        {
                            TilesPages.Add(new VTMapPage { PageNumber = tile.Page });
                        }
                    }

                    TilesPages[tile.Page].AddTile(tile);
                }

                if (!IsDeadBeef(br))
                {
                    //Logger.LogToFile(Logger.LogLevel.Error, "Unexpected data at {0:x2}", br.BaseStream.Position);
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
                        if (!unknownValues.Contains(entries[i].TimeStamp)) { unknownValues.Add(entries[i].TimeStamp); }
                    }
                    else
                    {
                        for (int k = 0; k < entries.Count; k++)
                        {
                            if (entries[k].FileName == fileName)
                            {
                                entries[k].TimeStamp = br.ReadInt32();
                                entries[k].Unknown2 = br.ReadInt32();
                                if (!unknownValues.Contains(entries[k].TimeStamp)) { unknownValues.Add(entries[k].TimeStamp); }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public bool IsDeadBeef(BinaryReader br)
        {
            return (br.ReadByte() == 0xef &&
                    br.ReadByte() == 0xbe &&
                    br.ReadByte() == 0xad &&
                    br.ReadByte() == 0xde);
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
            return;
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int j = 0;

            //Logger.LogToFile(Logger.LogLevel.Debug, "Logging a VT Map:\n\tType: {0}\n\tPageCount: {1}\n\tTileSize: {2}\n\tTilePadding: {3}\n\tNumber Of Tiles: {4}\n\tWidth: {5}\n\tHeight: {6}\n\tTextureCount: {7}\n\tEntries.Count: {8}", type, pageCount, TileSize, tilePadding, numberOfTiles, width, height, textureCount, entries.Count);

            foreach (VTMapEntry entry in entries)
            {
                sb.AppendFormat("\tEntry {0}:\n\t\tColumn: {1}\n\t\tRow: {2}\n\t\tWidth: {3}\n\t\tHeight:{4}\n\t\tName: {5}\n\t\tTimestamp: {6}\n\t\tUnknown 2: {7}\n", i, entry.Column, entry.Row, entry.Width, entry.Height, entry.FileName, entry.TimeStamp, entry.Unknown2);
                i++;
            }

            //Logger.LogToFile(Logger.LogLevel.Debug, sb.ToString());
            sb.Clear();

            foreach (KeyValuePair<string, VTMapTileTDX> keyval in TilesByName)
            {
                for (int k = 0; k < keyval.Value.Coords.Count; k++)
                {
                    VTMapTile tile = keyval.Value.Coords[k];
                    sb.AppendFormat("\tTile {0}\n\t\t FileName: {1}\n\t\tColumn: {2}\n\t\tRow: {3}\n\t\tPage: {4}\n", j, tile.TileName, tile.Column, tile.Row, tile.Page);
                    j++;
                }
            }

            //Logger.LogToFile(Logger.LogLevel.Debug, sb.ToString());
        }
    }
}