using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

using ToxicRagers.Generics;
using ToxicRagers.Helpers;
using static ToxicRagers.Helpers.ColorHelper;

namespace ToxicRagers.PSX.Formats
{
    public class TIM : ITexture
    {
        public enum TIMMode
        {
            Format16bppA1R5G5B5 = 2,
            Format4bppPalette = 8,
            Format8bppPalette = 9
        }

        [Flags]
        public enum TIMFlags
        {
            Transparent = 1
        }

        public List<TIMPalette> Palettes = new List<TIMPalette>();

        public byte[] Data { get; set; }

        public int OriginX { get; set; }

        public int OriginY { get; set; }

        public int DataWidth { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public TIMMode Mode { get; set; }

        public TIMFlags Flags { get; set; }

        public ChannelOrder ChannelOrder { get; set; }

        public string Name { get; set; }

        public string Extension { get; } = "tim";

        public List<MipMap> MipMaps { get; set; } = new List<MipMap>();

        public D3DFormat Format { get; }

        public static TIM Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            TIM tim = new TIM();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path))) { tim = Load(ms, Path.GetFileNameWithoutExtension(path)); }

            return tim;
        }

        public static TIM Load(Stream stream, string name = null, ChannelOrder channelOrder = ChannelOrder.BGR)
        {
            TIM tim = new TIM();

            using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                void ReadPalettes()
                {
                    ushort paletteOriginX = br.ReadUInt16();
                    ushort paletteOriginY = br.ReadUInt16();
                    ushort paletteEntryCount = br.ReadUInt16();
                    ushort paletteCount = br.ReadUInt16();

                    for (int i = 0; i < paletteCount; i++)
                    {
                        TIMPalette palette = new TIMPalette();

                        for (int j = 0; j < paletteEntryCount; j++)
                        {
                            palette.Colours.Add(PSX5551ToColor(br.ReadUInt16(), channelOrder, tim.Flags.HasFlag(TIMFlags.Transparent)));
                        }

                        tim.Palettes.Add(palette);
                    }
                }

                if (br.ReadByte() != 0x10 ||
                    br.ReadByte() != 0x0 ||
                    br.ReadByte() != 0x0 ||
                    br.ReadByte() != 0x0)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid TIM file", name);
                    return null;
                }

                uint flags = br.ReadUInt32();

                tim.Mode = (TIMMode)(flags & 0x0f);
                tim.Flags = (TIMFlags)((flags & 0xf0) >> 4);
                tim.ChannelOrder = channelOrder;

                switch (tim.Mode)
                {
                    case TIMMode.Format16bppA1R5G5B5:
                        br.ReadUInt16(); // ??
                        br.ReadUInt16(); // ??
                        tim.OriginX = br.ReadUInt16();
                        tim.OriginY = br.ReadUInt16();
                        tim.Width = tim.DataWidth = br.ReadUInt16();
                        tim.Height = br.ReadUInt16();
                        tim.Data = br.ReadBytes(tim.Width * tim.Height * 2);
                        break;

                    case TIMMode.Format4bppPalette:
                        br.ReadUInt16(); // length of palette
                        br.ReadUInt16(); // ??

                        ReadPalettes();

                        br.ReadUInt16(); // length of image
                        br.ReadUInt16(); // ??
                        tim.OriginX = br.ReadUInt16();
                        tim.OriginY = br.ReadUInt16();
                        tim.DataWidth = br.ReadUInt16();
                        tim.Height = br.ReadUInt16();
                        tim.Width = tim.DataWidth * 4;
                        tim.Data = br.ReadBytes(tim.DataWidth * tim.Height * 2);
                        break;

                    case TIMMode.Format8bppPalette:
                        br.ReadUInt16(); // length of palette
                        br.ReadUInt16(); // ??

                        ReadPalettes();

                        br.ReadUInt16(); // length of image
                        br.ReadUInt16(); // ??
                        tim.OriginX = br.ReadUInt16();
                        tim.OriginY = br.ReadUInt16();
                        tim.DataWidth = br.ReadUInt16();
                        tim.Height = br.ReadUInt16();
                        tim.Width = tim.DataWidth * 2;
                        tim.Data = br.ReadBytes(tim.DataWidth * tim.Height * 2);
                        break;

                    default:
                        throw new NotImplementedException($"Unknown mode is unknown!  {tim.Mode}");
                        //return null;
                }
            }

            return tim;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            using (MemoryStream ms = new MemoryStream(Data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        switch (Mode)
                        {
                            case TIMMode.Format16bppA1R5G5B5:
                                bmp.SetPixel(x, y, PSX5551ToColor(br.ReadUInt16(), ChannelOrder, Flags.HasFlag(TIMFlags.Transparent)));
                                break;

                            case TIMMode.Format4bppPalette:
                                byte pixels = br.ReadByte();
                                bmp.SetPixel(x + 1, y, Palettes[0].Colours[(pixels & 0xf0) >> 4]);
                                bmp.SetPixel(x + 0, y, Palettes[0].Colours[pixels & 0x0f]);
                                x++;
                                break;

                            case TIMMode.Format8bppPalette:
                                bmp.SetPixel(x, y, Palettes[0].Colours[br.ReadByte()]);
                                break;
                        }
                    }
                }
            }

            return bmp;
        }
    }

    public class TIMPalette
    {
        public List<Color> Colours { get; set; } = new List<Color>();
    }
}
