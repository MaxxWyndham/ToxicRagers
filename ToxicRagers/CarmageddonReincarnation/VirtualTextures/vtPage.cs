using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using ToxicRagers.Helpers;
using ToxicRagers.Stainless.Formats;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTPage
    {
        public static List<VTPage> Pages = new List<VTPage>();

        public int maxTilesToStitch = 20000;

        public VTMap Map { get; set; }

        public int PageNum { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<List<VTMapTileTDX>> Tiles { get; set; } = new List<List<VTMapTileTDX>>();

        public int TileSize => Map.TileSize;

        public int TilePadding => Map.TilePadding;

        public VTPage(int w, int h, int page, VTMap map)
        {
            Map = map;

            while (Pages.Count < page + 1) { Pages.Add(null); }

            Pages[page] = this;
            PageNum = page;
            Width = w;
            Height = h;
            int oldnumTilesX = (int)Math.Ceiling((float)Width / TileSize);
            int oldnumTilesY = (int)Math.Ceiling((float)Height / TileSize);
            int numTilesX = oldnumTilesX > maxTilesToStitch ? maxTilesToStitch : oldnumTilesX;
            int numTilesY = oldnumTilesY > maxTilesToStitch ? maxTilesToStitch : oldnumTilesY;
            if (numTilesX < oldnumTilesX) { Width = numTilesX * TileSize; }
            if (numTilesY < oldnumTilesY) { Height = numTilesY * TileSize; }
            //tiles = Enumerable.Repeat(Enumerable.Repeat<crTDXVTMapTileTDX>(null, (int)Math.Ceiling((float)width / 128)).ToList(), (int)Math.Ceiling((float)height / 128)).ToList();
            for (int i = 0; i < numTilesY; i++)
            {
                Tiles.Add(Enumerable.Repeat<VTMapTileTDX>(null, numTilesX).ToList());
                for (int j = 0; j < numTilesX; j++)
                {
                    Tiles[i].Add(null);
                }
            }
        }

        public int GetDivisor()
        {
            if (PageNum == 0) { return 1; }
            int divisor = 1;
            for (int i = 1; i <= PageNum; i++, divisor *= 2) { }
            return divisor;
        }

        public List<VTMapTile> GetTiles(VTMapEntry textureEntry)
        {
            List<VTMapTile> output = new List<VTMapTile>();

            int divisor = GetDivisor();
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor;
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor;
            int tileRow = (int)Math.Floor(yPos / (float)TileSize);
            int tileCol = (int)Math.Floor(xPos / (float)TileSize);

            tileRow = tileRow < 0 ? 0 : tileRow;
            tileCol = tileCol < 0 ? 0 : tileCol;

            int numTilesX = (int)Math.Ceiling(textureEntry.GetWidth(PageNum) / (float)TileSize);
            int numTilesY = (int)Math.Ceiling(textureEntry.GetHeight(PageNum) / (float)TileSize);

            for (int row = tileRow; row < Tiles.Count && row <= tileRow + numTilesY; row++)
            {
                int rowStart = row * TileSize;
                int rowEnd = (row + 1) * TileSize;

                for (int col = tileCol; col < Tiles[row].Count && col <= tileCol + numTilesX; col++)
                {
                    if (Tiles[row][col] == null) { continue; }

                    int colStart = col * TileSize;
                    int colEnd = (col + 1) * TileSize;

                    if (Tiles[row][col].Texture == null) { Tiles[row][col].GetTextureFromZAD(); }
                    if (Tiles[row][col].Texture == null) { continue; }

                    output.Add((from tile in Tiles[row][col].Coords where tile.Row == row && tile.Column == col select tile).First());
                }
            }

            return output;
        }

        public List<VTMapTileTDX> ImportTexture(Bitmap image, VTMapEntry textureEntry)
        {
            List<VTMapTileTDX> output = new List<VTMapTileTDX>();

            int divisor = GetDivisor();
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor;
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor;
            int tileRow = (int)Math.Floor(yPos / (float)TileSize);
            int tileCol = (int)Math.Floor(xPos / (float)TileSize);

            tileRow = tileRow < 0 ? 0 : tileRow;
            tileCol = tileCol < 0 ? 0 : tileCol;

            int numTilesX = (int)Math.Ceiling(textureEntry.GetWidth(PageNum) / (float)TileSize);
            int numTilesY = (int)Math.Ceiling(textureEntry.GetHeight(PageNum) / (float)TileSize);

            int bitmapheight = textureEntry.GetHeight(PageNum);
            int bitmapwidth = textureEntry.GetWidth(PageNum);

            for (int row = tileRow; row < Tiles.Count && row <= tileRow + numTilesY; row++)
            {
                int rowStart = row * TileSize;
                int rowEnd = (row + 1) * TileSize;

                for (int col = tileCol; col < Tiles[row].Count && col <= tileCol + numTilesX; col++)
                {
                    if (Tiles[row][col] == null) { continue; }

                    int colStart = col * TileSize;
                    int colEnd = (col + 1) * TileSize;

                    if (Tiles[row][col].Texture == null) { Tiles[row][col].GetTextureFromZAD(); }
                    if (Tiles[row][col].Texture == null) { continue; }

                    Bitmap decompressed = Tiles[row][col].Texture.GetBitmap();

                    int firstPixelRow = rowStart < yPos ? yPos - rowStart : 0;
                    int lastPixelRow = TileSize + TilePadding + TilePadding;

                    int firstPixelCol = colStart < xPos ? xPos - colStart : 0;
                    int lastPixelCol = TileSize + TilePadding + TilePadding;

                    for (int y = firstPixelRow; y < lastPixelRow; y++)
                    {
                        if ((row * TileSize + (y - TilePadding)) - (yPos) >= bitmapheight + TilePadding) { break; }

                        for (int x = firstPixelCol; x < lastPixelCol; x++)
                        {
                            if ((col * TileSize + (x - TilePadding)) - xPos >= bitmapwidth + TilePadding) { break; }

                            int originalX = (col * TileSize + (x - TilePadding)) - xPos;
                            int originalY = (row * TileSize + (y - TilePadding)) - (yPos);

                            if (originalX < 0)
                            {
                                originalX = 0;
                            }
                            else if (originalX >= bitmapwidth)
                            {
                                originalX = bitmapwidth - 1;
                            }

                            if (originalY < 0)
                            {
                                originalY = 0;
                            }
                            else if (originalY >= bitmapheight)
                            {
                                originalY = bitmapheight - 1;
                            }

                            Color colour = image.GetPixel(originalX, originalY);
                            decompressed.SetPixel(x, y, colour);
                        }
                    }

                    TDX newTileTDX = TDX.LoadFromBitmap(decompressed, Tiles[row][col].Texture.Name, Tiles[row][col].Texture.Format);
                    newTileTDX.Flags = TDX.TDXFlags.AlphaNbit | TDX.TDXFlags.DoNot16bit;
                    Tiles[row][col].Texture = newTileTDX;
                    output.Add(Tiles[row][col]);
                }
            }

            return output;
        }

        public void SaveTexture(VTMapEntry textureEntry, string outputPath)
        {
            SaveTexture(textureEntry, outputPath, ImageFormat.Png);
        }

        public void SaveTexture(VTMapEntry textureEntry, string outputPath, ImageFormat format)
        {
            SaveTexture(textureEntry, outputPath, false, false, true, format);
        }

        public void SaveTexture(VTMapEntry textureEntry, string outputPath, bool SaveTDX, bool SaveTGA, bool SaveOther, ImageFormat format)
        {
            int divisor = GetDivisor();
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor;
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor;
            int tileRow = (int)Math.Floor(yPos / (float)TileSize);
            int tileCol = (int)Math.Floor(xPos / (float)TileSize);

            tileRow = tileRow < 0 ? 0 : tileRow;
            tileCol = tileCol < 0 ? 0 : tileCol;

            int numTilesX = (int)Math.Ceiling(textureEntry.GetWidth(PageNum) / (float)TileSize);
            int numTilesY = (int)Math.Ceiling(textureEntry.GetHeight(PageNum) / (float)TileSize);

            int bitmapheight = textureEntry.GetHeight(PageNum);
            int bitmapwidth = textureEntry.GetWidth(PageNum);
            Bitmap stitched = new Bitmap(bitmapwidth, bitmapheight);

            D3DFormat guessedFormat = D3DFormat.DXT5;
            bool formatGuessed = false;

            for (int row = tileRow; row < Tiles.Count && row <= tileRow + numTilesY; row++)
            {
                int rowStart = row * TileSize;
                int rowEnd = (row + 1) * TileSize;

                for (int col = tileCol; col < Tiles[row].Count && col <= tileCol + numTilesX; col++)
                {
                    if (Tiles[row][col] == null) { continue; }

                    int colStart = col * TileSize;
                    int colEnd = (col + 1) * TileSize;

                    if (Tiles[row][col].Texture == null) { Tiles[row][col].GetTextureFromZAD(); }
                    if (Tiles[row][col].Texture == null) { continue; }
                    if (!formatGuessed) { guessedFormat = Tiles[row][col].Texture.Format; }

                    Bitmap decompressed = Tiles[row][col].Texture.GetBitmap();

                    int firstPixelRow = rowStart < yPos ? yPos - rowStart : TilePadding;
                    int lastPixelRow = TileSize + TilePadding;

                    int firstPixelCol = colStart < xPos ? xPos - colStart : TilePadding;
                    int lastPixelCol = TileSize + TilePadding;

                    for (int y = firstPixelRow; y < lastPixelRow; y++)
                    {
                        if ((row * TileSize + (y - TilePadding)) - (yPos) >= bitmapheight) { break; }
                        for (int x = firstPixelCol; x < lastPixelCol; x++)
                        {
                            if ((col * TileSize + (x - TilePadding)) - xPos >= bitmapwidth) { break; }

                            Color pixelColour = decompressed.GetPixel(x, y);

                            stitched.SetPixel((col * TileSize + (x - TilePadding)) - xPos, (row * TileSize + (y - TilePadding)) - (yPos), pixelColour);
                        }
                    }
                }
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputPath))) { Directory.CreateDirectory(Path.GetDirectoryName(outputPath)); }

            if (SaveTDX)
            {
                TDX newtdx = TDX.LoadFromBitmap(stitched, Path.GetFileNameWithoutExtension(outputPath), guessedFormat);
                newtdx.Save(outputPath);
            }

            if (SaveTGA)
            {
                if (File.Exists(outputPath)) { File.Delete(outputPath); }

                using (FileStream stream = new FileStream(outputPath, FileMode.OpenOrCreate))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    int tgaWidth = bitmapwidth;
                    int tgaHeight = bitmapheight;
                    int tgaTileRowCount = Tiles.Count - 1;

                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)2);
                    writer.Write((short)0);
                    writer.Write((short)0);
                    writer.Write((byte)0);
                    writer.Write((short)0);
                    writer.Write((short)0);
                    writer.Write((short)bitmapwidth);
                    writer.Write((short)bitmapheight);
                    writer.Write((byte)32);
                    writer.Write((byte)0);

                    byte[] raw = new byte[stitched.Width * stitched.Height * 4];

                    BitmapData data = stitched.LockBits(new Rectangle(0, 0, stitched.Width, stitched.Height), ImageLockMode.ReadOnly, stitched.PixelFormat);
                    Marshal.Copy(data.Scan0, raw, 0, data.Stride * data.Height);
                    stitched.UnlockBits(data);

                    // important to use the BitmapData object's Width and Height
                    // properties instead of the Bitmap's.
                    for (int x = data.Height - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < data.Width; y++)
                        {
                            int columnOffset = x * data.Stride + y * 4;
                            byte B = raw[columnOffset + 0];
                            byte G = raw[columnOffset + 1];
                            byte R = raw[columnOffset + 2];
                            byte A = raw[columnOffset + 3];
                            writer.Write(B);
                            writer.Write(G);
                            writer.Write(R);
                            writer.Write(A);
                        }
                    }
                }
            }

            if (SaveOther)
            {
                using (FileStream str = File.OpenWrite(outputPath))
                {
                    stitched.Save(str, format);
                }
            }

            stitched.Dispose();
        }

        public void SaveTextureTGA(VTMapEntry textureEntry, string outputPath)
        {
            int divisor = GetDivisor();
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor;
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor;
            int tileRow = (int)Math.Floor(yPos / (float)TileSize);
            int tileCol = (int)Math.Floor(xPos / (float)TileSize);
            tileRow = tileRow < 0 ? 0 : tileRow;
            tileCol = tileCol < 0 ? 0 : tileCol;
            int numTilesX = (int)Math.Ceiling(textureEntry.GetWidth(PageNum) / (float)TileSize);
            int numTilesY = (int)Math.Ceiling(textureEntry.GetHeight(PageNum) / (float)TileSize);

            int bitmapheight = textureEntry.GetHeight(PageNum);
            int bitmapwidth = textureEntry.GetWidth(PageNum);
            Bitmap stitched = new Bitmap(bitmapwidth, bitmapheight);

            string tileprefix = Path.GetFullPath(outputPath) + Path.GetFileNameWithoutExtension(outputPath);

            if (!Directory.Exists(Path.GetDirectoryName(outputPath))) { Directory.CreateDirectory(Path.GetDirectoryName(outputPath)); }
            if (File.Exists(outputPath)) { File.Delete(outputPath); }
            int pos;

            using (FileStream stream = new FileStream(outputPath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                int tgaWidth = Width;
                int tgaHeight = Height;
                int tgaTileRowCount = Tiles.Count - 1;

                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)2);
                writer.Write((short)0);
                writer.Write((short)0);
                writer.Write((byte)0);
                writer.Write((short)0);
                writer.Write((short)0);
                writer.Write((short)bitmapwidth);
                writer.Write((short)bitmapheight);
                writer.Write((byte)32);
                writer.Write((byte)0);

                pos = (int)writer.BaseStream.Position;

                List<byte[]> rowTiles = new List<byte[]>();

                for (int row = tileRow + numTilesY; row >= tileRow && row >= 0; row--)
                {
                    rowTiles.Clear();
                    int rowStart = row * (TileSize + TilePadding + TilePadding) * tgaWidth * 4;

                    int tgaTileColCount = Tiles[tileRow].Count;

                    for (int y = (TileSize + TilePadding + TilePadding) - 1; y >= 0; y--)
                    {

                        for (int col = tileCol; col < Tiles[row].Count && col <= tileCol + numTilesX; col++)
                        {
                            if (y == (TileSize + TilePadding + TilePadding) - 1)
                            {
                                if (Tiles[row][col] == null)
                                {
                                    rowTiles.Add(Enumerable.Repeat((byte)0, 4 * (TileSize + TilePadding + TilePadding) * (TileSize + TilePadding + TilePadding)).ToArray());
                                }
                                else
                                {
                                    if (Tiles[row][col].Texture == null) { Tiles[row][col].GetTextureFromZAD(); }

                                    // TODO: fix this
                                    //rowTiles.Add(Tiles[row][col].Texture.Decompress(Tiles[row][col].Texture.MipMaps[0]));
                                }
                            }
                            int colStart = col * (TileSize + TilePadding + TilePadding) * 4 + rowStart;
                            byte[] decompressed = rowTiles[col];

                            for (int x = 0; x < (TileSize + TilePadding + TilePadding); x++)
                            {
                                int pixel = y * (TileSize + TilePadding + TilePadding) * 4 + x * 4;

                                writer.Write((byte)decompressed[pixel + 0]);
                                writer.Write((byte)decompressed[pixel + 1]);
                                writer.Write((byte)decompressed[pixel + 2]);
                                writer.Write((byte)decompressed[pixel + 3]);

                            }
                        }
                    }
                }
            }
        }

        public void SaveTGA(string outputPath, bool limitSize = false, int maxTileLimit = 100)
        {
            if (File.Exists(outputPath)) { File.Delete(outputPath); }
            int pos;

            using (FileStream stream = new FileStream(outputPath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                int tgaWidth = Width;
                int tgaHeight = Height;
                int tgaTileRowCount = Tiles.Count - 1;
                if (limitSize)
                {
                    if (maxTileLimit * (TileSize + TilePadding + TilePadding) < tgaWidth) { tgaWidth = maxTileLimit * (TileSize + TilePadding + TilePadding); }
                    if (maxTileLimit * (TileSize + TilePadding + TilePadding) < tgaHeight)
                    {
                        tgaTileRowCount = maxTileLimit;
                        tgaHeight = maxTileLimit * (TileSize + TilePadding + TilePadding);
                    }
                }
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)2);
                writer.Write((short)0);
                writer.Write((short)0);
                writer.Write((byte)0);
                writer.Write((short)0);
                writer.Write((short)0);
                writer.Write((short)tgaWidth);
                writer.Write((short)tgaHeight);
                writer.Write((byte)32);
                writer.Write((byte)0);

                pos = (int)writer.BaseStream.Position;

                List<byte[]> rowTiles = new List<byte[]>();

                for (int tileRow = tgaTileRowCount - 1; tileRow >= 0; tileRow--)
                {
                    rowTiles.Clear();
                    int rowStart = tileRow * (TileSize + TilePadding + TilePadding) * tgaWidth * 4;

                    int tgaTileColCount = Tiles[tileRow].Count;
                    if (tgaWidth < Width) { tgaTileColCount = maxTileLimit; }

                    for (int y = (TileSize + TilePadding + TilePadding) - 1; y >= 0; y--)
                    {
                        for (int tileCol = 0; tileCol < tgaTileColCount; tileCol++)
                        {
                            if (y == (TileSize + TilePadding + TilePadding) - 1)
                            {
                                if (Tiles[tileRow][tileCol] == null)
                                {
                                    rowTiles.Add(Enumerable.Repeat((byte)0, 4 * (TileSize + TilePadding + TilePadding) * (TileSize + TilePadding + TilePadding)).ToArray());
                                }
                                else
                                {
                                    if (Tiles[tileRow][tileCol].Texture == null) { Tiles[tileRow][tileCol].GetTextureFromZAD(); }

                                    // TODO: fix this
                                    //rowTiles.Add(Tiles[tileRow][tileCol].Texture.Decompress(Tiles[tileRow][tileCol].Texture.MipMaps[0]));
                                }
                            }

                            int colStart = tileCol * (TileSize + TilePadding + TilePadding) * 4 + rowStart;
                            byte[] decompressed = rowTiles[tileCol];

                            for (int x = 0; x < (TileSize + TilePadding + TilePadding); x++)
                            {
                                int pixel = y * (TileSize + TilePadding + TilePadding) * 4 + x * 4;

                                writer.Write((byte)decompressed[pixel + 0]);
                                writer.Write((byte)decompressed[pixel + 1]);
                                writer.Write((byte)decompressed[pixel + 2]);
                                writer.Write((byte)decompressed[pixel + 3]);
                            }
                        }
                    }
                }
            }
        }

        public Bitmap MergeTiles()
        {
            int bitmapheight = (TileSize + TilePadding + TilePadding) * (maxTilesToStitch < Tiles[0].Count ? maxTilesToStitch : Tiles.Count);
            int bitmapwidth = (TileSize + TilePadding + TilePadding) * (maxTilesToStitch < Tiles.Count ? maxTilesToStitch : Tiles.Count);
            Bitmap stitched = new Bitmap(bitmapwidth, bitmapheight);

            for (int tileCol = 0; tileCol < Tiles.Count; tileCol++)
            {
                int colStart = tileCol * (TileSize + TilePadding + TilePadding);
                if (colStart >= (TileSize + TilePadding + TilePadding) * maxTilesToStitch) { break; }

                for (int tileRow = 0; tileRow < Tiles[tileCol].Count; tileRow++)
                {
                    if (Tiles[tileCol][tileRow] == null) { continue; }

                    int rowStart = tileRow * (TileSize + TilePadding + TilePadding);
                    if (rowStart >= (TileSize + TilePadding + TilePadding) * maxTilesToStitch) { break; }
                    if (Tiles[tileCol][tileRow].Texture == null) { Tiles[tileCol][tileRow].GetTextureFromZAD(); }
                    Bitmap decompressed = Tiles[tileCol][tileRow].Texture.GetBitmap();

                    for (int y = 0; y < decompressed.Height; y++)
                    {
                        for (int x = 0; x < decompressed.Width; x++)
                        {
                            int oldPixel = y * decompressed.Height + x;
                            int newPixel = ((rowStart + y) * ((TileSize + TilePadding + TilePadding) * maxTilesToStitch) + colStart + x);
                            Color pixelColour = decompressed.GetPixel(x, y);

                            stitched.SetPixel(colStart + x, rowStart + y, pixelColour);
                        }
                    }
                }
            }

            return stitched;
        }

        public override string ToString()
        {
            return $"Page {PageNum}";
        }
    }
}