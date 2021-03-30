using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.Novadrome.Formats
{
    public class XT2 : ITexture
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public D3DBaseTexture Header { get; set; }

        public string Name { get; set; }

        public string Extension { get; } = "xt2";

        public List<MipMap> MipMaps { get; set; } = new List<MipMap>();

        public D3DFormat Format { get; }

        public static XT2 Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Console.WriteLine("{0}", path);
            XT2 xt2 = new XT2() { Name = fi.Name.Replace(fi.Extension, "") };

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead()))
            {
                Console.WriteLine("Always 0 : {0}", br.ReadUInt32());

                int magic = (int)br.ReadUInt32();

                br.ReadUInt32();    // datasize
                Console.WriteLine("Always 52 : {0}", br.ReadUInt32());
                Console.WriteLine("Always 0 : {0}", br.ReadUInt16());
                xt2.Width = br.ReadUInt16();
                xt2.Height = br.ReadUInt16();
                Console.WriteLine("{0} {1}", br.ReadUInt16(), br.ReadUInt16());

                xt2.Header = new D3DBaseTexture(br);

                int W = 0;
                int H = 0;
                int TexelPitch = 0;
                int DataSize = 0;
                int OutSize = 0;
                int w = xt2.Width;
                int h = xt2.Height;

                switch (xt2.Header.DataFormat)
                {
                    //case D3DFormat.X360_A8R8G8B8:
                    //    W = w;
                    //    H = h;
                    //    TexelPitch = 4;
                    //    DataSize = w * h * 4;
                    //    break;

                    //case D3DFormat.X360_DXT1:
                    //    W = w / 4;
                    //    H = h / 4;
                    //    TexelPitch = 8;
                    //    DataSize = w * h / 2;
                    //    break;

                    //case D3DFormat.X360_DXT2:
                    //    W = w / 4;
                    //    H = h / 4;
                    //    TexelPitch = 16;
                    //    DataSize = W * H;
                    //    break;
                }

                if (H % 128 != 0) { H += 128 - H % 128; }

                switch (xt2.Header.DataFormat)
                {
                    //case D3DFormat.X360_A8R8G8B8:
                    //    OutSize = W * H * TexelPitch;
                    //    break;

                    //case D3DFormat.X360_DXT1:
                    //    OutSize = W * H * TexelPitch;
                    //    break;

                    //case D3DFormat.X360_DXT2:
                    //    DataSize = w * H * 2;
                    //    OutSize = W * H * TexelPitch;
                    //    break;
                }

                byte[] data = new byte[DataSize];
                byte[] outdata = new byte[OutSize];

                Array.Copy(br.ReadBytes(DataSize), data, DataSize);

                int step = xt2.Header.Endian == 1 ? 2 : 4;
                for (int i = 0; i < data.Length; i += step)
                {
                    for (int j = 0; j < step / 2; j++)
                    {
                        byte tmp = data[i + j];
                        data[i + j] = data[i + step - j - 1];
                        data[i + step - j - 1] = tmp;
                    }
                }

                for (int y = 0; y < H; y++)
                {
                    for (int x = 0; x < W; x++)
                    {
                        int offset = Xbox360ConvertTextureAddress(x, y, W, TexelPitch);
                        if (offset * TexelPitch < data.Length) { Array.Copy(data, offset * TexelPitch, outdata, (x + y * W) * TexelPitch, TexelPitch); }
                    }
                }

                MipMap mip = new MipMap
                {
                    Width = xt2.Width,
                    Height = xt2.Height,
                    Data = data
                };

                xt2.MipMaps.Add(mip);
            }

            return xt2;
        }

        public static int Xbox360ConvertTextureAddress(int x, int y, int w, int TexelPitch)
        {
            int alignedWidth;
            int logBpp;
            int Macro;
            int Micro;
            int Offset;

            alignedWidth = (w + 31) & ~31;
            logBpp = (TexelPitch >> 2) + ((TexelPitch >> 1) >> (TexelPitch >> 2));
            Macro = ((x >> 5) + (y >> 5) * (alignedWidth >> 5)) << (logBpp + 7);
            Micro = (((x & 7) + ((y & 6) << 2)) << logBpp);
            Offset = Macro + ((Micro & ~15) << 1) + (Micro & 15) + ((y & 8) << (3 + logBpp)) + ((y & 1) << 4);
            return (((Offset & ~511) << 3) + ((Offset & 448) << 2) + (Offset & 63) + ((y & 16) << 7) + (((((y & 8) >> 2) + (x >> 3)) & 3) << 6)) >> logBpp;
        }
    }
}