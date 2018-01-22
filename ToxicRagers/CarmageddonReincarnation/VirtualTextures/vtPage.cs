using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using ToxicRagers.CarmageddonReincarnation.Formats;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.VirtualTextures
{
    public class VTPage
    {
        public static List<VTPage> Pages = new List<VTPage>();
        public int maxTilesToStitch = 20000;
        int pageNum;
        int width;
        int height;

        List<List<VTMapTileTDX>> tiles = new List<List<VTMapTileTDX>>();

        VTMap map;

        public VTMap Map
        {
            get => map;
            set => map = value;
        }

        public int PageNum
        {
            get => pageNum;
            set => pageNum = value;
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

        public List<List<VTMapTileTDX>> Tiles
        {
            get => tiles;
            set => tiles = value;
        }

        public int TileSize => Map.TileSize;
        public int TilePadding => Map.TilePadding;

        public VTPage(int w, int h, int page, VTMap map)
        {
            this.map = map;

            while (Pages.Count < page + 1) { Pages.Add(null); }

            Pages[page] = this;
            PageNum = page;
            Width = w;
            Height = h;
            int oldnumTilesX = (int)Math.Ceiling((float)width / TileSize);
            int oldnumTilesY = (int)Math.Ceiling((float)height / TileSize);
            int numTilesX = oldnumTilesX > maxTilesToStitch ? maxTilesToStitch : oldnumTilesX;
            int numTilesY = oldnumTilesY > maxTilesToStitch ? maxTilesToStitch : oldnumTilesY;
            if (numTilesX < oldnumTilesX) { Width = numTilesX * TileSize; }
            if (numTilesY < oldnumTilesY) { Height = numTilesY * TileSize; }
            //tiles = Enumerable.Repeat(Enumerable.Repeat<crTDXVTMapTileTDX>(null, (int)Math.Ceiling((float)width / 128)).ToList(), (int)Math.Ceiling((float)height / 128)).ToList();
            for (int i = 0; i < numTilesY; i++)
            {
                tiles.Add(Enumerable.Repeat<VTMapTileTDX>(null, numTilesX).ToList());
                for (int j = 0; j < numTilesX; j++)
                {
                    tiles[i].Add(null);
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
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor; //(int)Math.Floor(((double)textureEntry.Column / Pages[0].Width) * Width);
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor; // (int)Math.Floor(((double)textureEntry.Row / Pages[0].Height) * Height);
            int tileRow = (int)Math.Floor(yPos / (float)TileSize);
            int tileCol = (int)Math.Floor(xPos / (float)TileSize);
            tileRow = tileRow < 0 ? 0 : tileRow;
            tileCol = tileCol < 0 ? 0 : tileCol;
            int numTilesX = (int)Math.Ceiling(textureEntry.GetWidth(PageNum) / (float)TileSize);
            int numTilesY = (int)Math.Ceiling(textureEntry.GetHeight(PageNum) / (float)TileSize);

            for (int row = tileRow; row < tiles.Count && row <= tileRow + numTilesY; row++)
            {

                int rowStart = row * TileSize;
                int rowEnd = (row + 1) * TileSize;
                //if (yPos + bitmapheight < rowStart) break;
                //if (yPos >= rowEnd) continue;
                //if (colStart >= 128 * maxTilesToStitch) break;
                for (int col = tileCol; col < tiles[row].Count && col <= tileCol + numTilesX; col++)
                {
                    if (tiles[row][col] == null) { continue; }
                    //tiles[tileCol][tileRow].Texture.SaveAsDDS(@"E:\Games\Steam\SteamApps\common\Carmageddon_Reincarnation\ZAD_VT\outskirts\output\" + "tile_" + tileCol + "_" + tileRow);
                    int colStart = col * TileSize;
                    int colEnd = (col + 1) * TileSize;

                    //if (xPos >= colEnd) continue;
                    //if (xPos + bitmapwidth < colStart) break;
                    //if (rowStart >= 128 * maxTilesToStitch) break;

                    if (tiles[row][col].Texture == null) { tiles[row][col].GetTextureFromZAD(); }
                    if (tiles[row][col].Texture == null) { continue; }

                    output.Add((from tile in tiles[row][col].Coords where tile.Row == row && tile.Column == col select tile).First());
                    //stiched.UnlockBits(bmd);
                }
            }

            return output;
        }

        public List<VTMapTileTDX> ImportTexture(Bitmap image, VTMapEntry textureEntry)
        {
            List<VTMapTileTDX> output = new List<VTMapTileTDX>();

            int divisor = GetDivisor();
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor; //(int)Math.Floor(((double)textureEntry.Column / Pages[0].Width) * Width);
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor; // (int)Math.Floor(((double)textureEntry.Row / Pages[0].Height) * Height);
            int tileRow = (int)Math.Floor(yPos / (float)TileSize);
            int tileCol = (int)Math.Floor(xPos / (float)TileSize);
            tileRow = tileRow < 0 ? 0 : tileRow;
            tileCol = tileCol < 0 ? 0 : tileCol;
            int numTilesX = (int)Math.Ceiling(textureEntry.GetWidth(PageNum) / (float)TileSize);
            int numTilesY = (int)Math.Ceiling(textureEntry.GetHeight(PageNum) / (float)TileSize);

            int bitmapheight = textureEntry.GetHeight(PageNum);
            int bitmapwidth = textureEntry.GetWidth(PageNum);

            for (int row = tileRow; row < tiles.Count && row <= tileRow + numTilesY; row++)
            {
                int rowStart = row * TileSize;
                int rowEnd = (row + 1) * TileSize;
                //if (yPos + bitmapheight < rowStart) break;
                //if (yPos >= rowEnd) continue;
                //if (colStart >= 128 * maxTilesToStitch) break;
                for (int col = tileCol; col < tiles[row].Count && col <= tileCol + numTilesX; col++)
                {
                    //if (row == 70 && col == 67)
                    {
                        //Logger.LogToFile("this is row 70, col 67");
                    }

                    if (tiles[row][col] == null) { continue; }
                    //tiles[tileCol][tileRow].Texture.SaveAsDDS(@"E:\Games\Steam\SteamApps\common\Carmageddon_Reincarnation\ZAD_VT\outskirts\output\" + "tile_" + tileCol + "_" + tileRow);
                    int colStart = col * TileSize;
                    int colEnd = (col + 1) * TileSize;

                    if (tiles[row][col].Texture == null) { tiles[row][col].GetTextureFromZAD(); }
                    if (tiles[row][col].Texture == null) { continue; }

                    Bitmap decompressed = tiles[row][col].Texture.Decompress(0, false);

                    int firstPixelRow = rowStart < yPos ? yPos - rowStart : 0;// TilePadding;
                    int lastPixelRow = TileSize + TilePadding + TilePadding;// rowEnd >= yPos + bitmapheight ? rowEnd - (yPos + bitmapheight) : 124;

                    int firstPixelCol = colStart < xPos ? xPos - colStart : 0;// TilePadding;
                    int lastPixelCol = TileSize + TilePadding + TilePadding;// colEnd > xPos + bitmapwidth ? colEnd - (xPos + bitmapwidth) : 124;

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
                    TDX newTileTDX = TDX.LoadFromBitmap(decompressed, tiles[row][col].Texture.Name, tiles[row][col].Texture.Format);
                    newTileTDX.SetFlags(TDX.Flags.Unknown8 | TDX.Flags.Unknown128);
                    tiles[row][col].Texture = newTileTDX;
                    output.Add(tiles[row][col]);

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
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor; //(int)Math.Floor(((double)textureEntry.Column / Pages[0].Width) * Width);
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor; // (int)Math.Floor(((double)textureEntry.Row / Pages[0].Height) * Height);
            int tileRow = (int)Math.Floor(yPos / (float)TileSize);
            int tileCol = (int)Math.Floor(xPos / (float)TileSize);
            tileRow = tileRow < 0 ? 0 : tileRow;
            tileCol = tileCol < 0 ? 0 : tileCol;
            int numTilesX = (int)Math.Ceiling(textureEntry.GetWidth(PageNum) / (float)TileSize);
            int numTilesY = (int)Math.Ceiling(textureEntry.GetHeight(PageNum) / (float)TileSize);

            int bitmapheight = textureEntry.GetHeight(PageNum);
            int bitmapwidth = textureEntry.GetWidth(PageNum);
            Bitmap stitched = new Bitmap(bitmapwidth, bitmapheight);

            //string tileprefix = Path.GetFullPath(outputPath) + Path.GetFileNameWithoutExtension(outputPath);
            //if (textureEntry.FileName == "Data_Core\\Content\\Levels\\outskirts\\accessories\\sw_Museum_Moon\\sw_Museum_Moon_S")
            {
                //Console.WriteLine("This is sw_Museum_moon");
            }

            D3DFormat guessedFormat = D3DFormat.DXT5;
            bool formatGuessed = false;
            for (int row = tileRow; row < tiles.Count && row <= tileRow + numTilesY; row++)
            {
                int rowStart = row * TileSize;
                int rowEnd = (row + 1) * TileSize;
                //if (yPos + bitmapheight < rowStart) break;
                //if (yPos >= rowEnd) continue;
                //if (colStart >= 128 * maxTilesToStitch) break;
                for (int col = tileCol; col < tiles[row].Count && col <= tileCol + numTilesX; col++)
                {
                    //if (row == 70 && col == 67)
                    {
                        //Logger.LogToFile("this is row 70, col 67");
                    }

                    if (tiles[row][col] == null) { continue; }
                    //tiles[tileCol][tileRow].Texture.SaveAsDDS(@"E:\Games\Steam\SteamApps\common\Carmageddon_Reincarnation\ZAD_VT\outskirts\output\" + "tile_" + tileCol + "_" + tileRow);
                    int colStart = col * TileSize;
                    int colEnd = (col + 1) * TileSize;

                    //if (xPos >= colEnd) continue;
                    //if (xPos + bitmapwidth < colStart) break;
                    //if (rowStart >= 128 * maxTilesToStitch) break;

                    if (tiles[row][col].Texture == null) { tiles[row][col].GetTextureFromZAD(); }
                    if (tiles[row][col].Texture == null) { continue; }
                    if (!formatGuessed) { guessedFormat = tiles[row][col].Texture.Format; }
                    Bitmap decompressed = tiles[row][col].Texture.Decompress(0, false);
                    //decompressed.Save(tileprefix + "_tile_" + row + "_" + col + ".png");

                    int firstPixelRow = rowStart < yPos ? yPos - rowStart : TilePadding;
                    int lastPixelRow = TileSize + TilePadding;// rowEnd >= yPos + bitmapheight ? rowEnd - (yPos + bitmapheight) : 124;

                    int firstPixelCol = colStart < xPos ? xPos - colStart : TilePadding;
                    int lastPixelCol = TileSize + TilePadding;// colEnd > xPos + bitmapwidth ? colEnd - (xPos + bitmapwidth) : 124;
                    //BitmapData bmd = stiched.LockBits(new Rectangle(colStart, rowStart, 128, 128), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    for (int y = firstPixelRow; y < lastPixelRow/*rowPair.Value.MipMaps[0].Height*/; y++)
                    {
                        if ((row * TileSize + (y - TilePadding)) - (yPos) >= bitmapheight) { break; }
                        for (int x = firstPixelCol; x < lastPixelCol /*rowPair.Value.MipMaps[0].Width*/; x++)
                        {
                            if ((col * TileSize + (x - TilePadding)) - xPos >= bitmapwidth) { break; }
                            //int oldPixel = y * 128 + x;
                            //int newPixel = ((row * 120 + y-8) * Width - (yPos) * Width) + ((col * 120 + x-4) - xPos); //((rowStart + y) * (128 * maxTilesToStitch) + colStart + x);
                            Color pixelColour = decompressed.GetPixel(x, y);

                            stitched.SetPixel((col * TileSize + (x - TilePadding)) - xPos, (row * TileSize + (y - TilePadding)) - (yPos), pixelColour);
                            /*tdx.MipMaps[0].Data[newPixel] = pixelColour.R;
                            tdx.MipMaps[0].Data[newPixel+1] = pixelColour.G;
                            tdx.MipMaps[0].Data[newPixel+2] = pixelColour.B;
                            tdx.MipMaps[0].Data[newPixel+3] = pixelColour.A; //rowPair.Value.MipMaps[0].Data[oldPixel];*/
                        }
                    }
                    //stiched.UnlockBits(bmd);
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
                    int tgaTileRowCount = tiles.Count - 1;

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
                    BitmapData data = stitched.LockBits(new Rectangle(0, 0, stitched.Width, stitched.Height), ImageLockMode.ReadOnly, stitched.PixelFormat);
                    unsafe
                    {
                        // important to use the BitmapData object's Width and Height
                        // properties instead of the Bitmap's.
                        for (int x = data.Height - 1; x >= 0; x--)
                        {
                            byte* row = (byte*)data.Scan0 + (x * data.Stride);
                            for (int y = 0; y < data.Width; y++)
                            {
                                int columnOffset = y * 4;
                                byte B = row[columnOffset];
                                byte G = row[columnOffset + 1];
                                byte R = row[columnOffset + 2];
                                byte alpha = row[columnOffset + 3];
                                writer.Write(B);
                                writer.Write(G);
                                writer.Write(R);
                                writer.Write(alpha);
                            }
                        }
                    }
                    stitched.UnlockBits(data);
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
            int xPos = PageNum == 0 ? textureEntry.Column : textureEntry.Column / divisor; //(int)Math.Floor(((double)textureEntry.Column / Pages[0].Width) * Width);
            int yPos = PageNum == 0 ? textureEntry.Row : textureEntry.Row / divisor; // (int)Math.Floor(((double)textureEntry.Row / Pages[0].Height) * Height);
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
                int tgaTileRowCount = tiles.Count - 1;

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

                //writer.Seek(pos, SeekOrigin.Begin);
                for (int row = tileRow + numTilesY; row >= tileRow && row >= 0; row--)
                {
                    rowTiles.Clear();
                    int rowStart = row * (TileSize + TilePadding + TilePadding) * tgaWidth * 4;

                    int tgaTileColCount = tiles[tileRow].Count;

                    //if (colStart >= 128 * maxTilesToStitch) break;
                    for (int y = (TileSize + TilePadding + TilePadding) - 1; y >= 0; y--)
                    {

                        for (int col = tileCol; col < tiles[row].Count && col <= tileCol + numTilesX; col++)
                        {
                            if (y == (TileSize + TilePadding + TilePadding) - 1)
                            {
                                if (tiles[row][col] == null)
                                {
                                    rowTiles.Add(Enumerable.Repeat((byte)0, 4 * (TileSize + TilePadding + TilePadding) * (TileSize + TilePadding + TilePadding)).ToArray());
                                }
                                else
                                {
                                    //if (rowStart >= 128 * maxTilesToStitch) break;
                                    if (tiles[row][col].Texture == null) { tiles[row][col].GetTextureFromZAD(); }
                                    rowTiles.Add(tiles[row][col].Texture.Decompress(tiles[row][col].Texture.MipMaps[0]));
                                }
                            }
                            int colStart = col * (TileSize + TilePadding + TilePadding) * 4 + rowStart;
                            //for (int y = 0; y < tiles[tileRow][tileCol].Texture.MipMaps[0].Height; y++)
                            {
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

            //stitched.Save(File.OpenWrite(outputPath), format);
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
                int tgaTileRowCount = tiles.Count - 1;
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

                //writer.Seek(pos, SeekOrigin.Begin);
                for (int tileRow = tgaTileRowCount - 1; tileRow >= 0; tileRow--)
                {
                    rowTiles.Clear();
                    int rowStart = tileRow * (TileSize + TilePadding + TilePadding) * tgaWidth * 4;

                    int tgaTileColCount = tiles[tileRow].Count;
                    if (tgaWidth < Width) { tgaTileColCount = maxTileLimit; }
                    //if (colStart >= 128 * maxTilesToStitch) break;
                    for (int y = (TileSize + TilePadding + TilePadding) - 1; y >= 0; y--)
                    {
                        for (int tileCol = 0; tileCol < tgaTileColCount; tileCol++)
                        {
                            if (y == (TileSize + TilePadding + TilePadding) - 1)
                            {
                                if (tiles[tileRow][tileCol] == null)
                                {
                                    rowTiles.Add(Enumerable.Repeat((byte)0, 4 * (TileSize + TilePadding + TilePadding) * (TileSize + TilePadding + TilePadding)).ToArray());
                                }
                                else
                                {
                                    //if (rowStart >= 128 * maxTilesToStitch) break;
                                    if (tiles[tileRow][tileCol].Texture == null) { tiles[tileRow][tileCol].GetTextureFromZAD(); }
                                    rowTiles.Add(tiles[tileRow][tileCol].Texture.Decompress(tiles[tileRow][tileCol].Texture.MipMaps[0]));
                                }
                            }

                            int colStart = tileCol * (TileSize + TilePadding + TilePadding) * 4 + rowStart;
                            //for (int y = 0; y < tiles[tileRow][tileCol].Texture.MipMaps[0].Height; y++)
                            {
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
        }

        public Bitmap MergeTiles()
        {
            /*TDX tdx = new TDX();
            tdx.Format = D3DFormat.A8R8G8B8;
            tdx.MipMaps.Add(new MipMap());
            tdx.MipMaps[0].Width = Width;
            tdx.MipMaps[0].Height = Height;
            tdx.MipMaps[0].Data = new byte[Width * Height * 4];
            for (int i = 0; i < tdx.MipMaps[0].Data.Length; i++ )
            {
                tdx.MipMaps[0].Data[i] = 255;
            }*/
            int bitmapheight = (TileSize + TilePadding + TilePadding) * (maxTilesToStitch < tiles[0].Count ? maxTilesToStitch : tiles.Count);
            int bitmapwidth = (TileSize + TilePadding + TilePadding) * (maxTilesToStitch < tiles.Count ? maxTilesToStitch : tiles.Count);
            Bitmap stitched = new Bitmap(bitmapwidth, bitmapheight);

            for (int tileCol = 0; tileCol < tiles.Count; tileCol++)
            {
                int colStart = tileCol * (TileSize + TilePadding + TilePadding);
                if (colStart >= (TileSize + TilePadding + TilePadding) * maxTilesToStitch) { break; }

                for (int tileRow = 0; tileRow < tiles[tileCol].Count; tileRow++)
                {
                    if (tiles[tileCol][tileRow] == null) { continue; }
                    //tiles[tileCol][tileRow].Texture.SaveAsDDS(@"E:\Games\Steam\SteamApps\common\Carmageddon_Reincarnation\ZAD_VT\outskirts\output\" + "tile_" + tileCol + "_" + tileRow);
                    int rowStart = tileRow * (TileSize + TilePadding + TilePadding);
                    if (rowStart >= (TileSize + TilePadding + TilePadding) * maxTilesToStitch) { break; }
                    if (tiles[tileCol][tileRow].Texture == null) { tiles[tileCol][tileRow].GetTextureFromZAD(); }
                    Bitmap decompressed = tiles[tileCol][tileRow].Texture.Decompress(0);
                    //BitmapData bmd = stiched.LockBits(new Rectangle(colStart, rowStart, 128, 128), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    for (int y = 0; y < decompressed.Height/*rowPair.Value.MipMaps[0].Height*/; y++)
                    {
                        for (int x = 0; x < decompressed.Width /*rowPair.Value.MipMaps[0].Width*/; x++)
                        {
                            int oldPixel = y * decompressed.Height + x;
                            int newPixel = ((rowStart + y) * ((TileSize + TilePadding + TilePadding) * maxTilesToStitch) + colStart + x);
                            Color pixelColour = decompressed.GetPixel(x, y);

                            stitched.SetPixel(colStart + x, rowStart + y, pixelColour);
                            /*tdx.MipMaps[0].Data[newPixel] = pixelColour.R;
                            tdx.MipMaps[0].Data[newPixel+1] = pixelColour.G;
                            tdx.MipMaps[0].Data[newPixel+2] = pixelColour.B;
                            tdx.MipMaps[0].Data[newPixel+3] = pixelColour.A; //rowPair.Value.MipMaps[0].Data[oldPixel];*/
                        }
                    }
                    //stiched.UnlockBits(bmd);
                }
            }
            return stitched;
        }

        public override string ToString()
        {
            return "Page " + PageNum;
        }
    }
}