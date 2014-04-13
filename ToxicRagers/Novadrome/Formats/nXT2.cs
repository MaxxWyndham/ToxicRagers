using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Novadrome.Formats
{
    public class XT2 : Texture
    {
        int dataSize;
        int width;
        int height;
        D3DBaseTexture header;
        string textureName;

        public string Texture { get { return textureName; } }

        public override string Format
        {
            get
            {
                switch (header.DataFormat)
                {
                    case D3DFormat.A8R8G8B8:
                        return "8888";

                    case D3DFormat.DXT1:
                        return "DXT1";

                    case D3DFormat.DXT2:
                        return "DXT3";
                }

                return null;
            }
        }

        public static XT2 Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Console.WriteLine("{0}", Path);
            XT2 xt2 = new XT2();

            xt2.Name = fi.Name.Replace(fi.Extension, "");

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead()))
            {
                Console.WriteLine("Always 0 : {0}", br.ReadUInt32());

                int magic = (int)br.ReadUInt32();

                xt2.dataSize = (int)br.ReadUInt32();
                Console.WriteLine("Always 52 : {0}", br.ReadUInt32());
                Console.WriteLine("Always 0 : {0}", br.ReadUInt16());
                xt2.width = br.ReadUInt16();
                xt2.height = br.ReadUInt16();
                Console.WriteLine("{0} {1}", br.ReadUInt16(), br.ReadUInt16());

                xt2.header = new D3DBaseTexture(br);

                Console.WriteLine("{0}x{1} :: {2} :: {3} :: {4} :: {5}", xt2.width, xt2.height, xt2.header.DataFormat, xt2.header.Endian, xt2.dataSize, magic);

                int W = 0;
                int H = 0;
                int TexelPitch = 0;
                int DataSize = 0;
                int OutSize = 0;
                int w = xt2.width;
                int h = xt2.height;

                switch (xt2.header.DataFormat)
                {
                    case D3DFormat.A8R8G8B8:
                        W = w;
                        H = h;
                        TexelPitch = 4;
                        DataSize = w * h * 4;
                        break;

                    case D3DFormat.DXT1:
                        W = w / 4;
                        H = h / 4;
                        TexelPitch = 8;
                        DataSize = w * h / 2;
                        break;

                    case D3DFormat.DXT2:
                        W = w / 4;
                        H = h / 4;
                        TexelPitch = 16;
                        DataSize = W * H;
                        break;
                }

                if (H % 128 != 0) { H = H + (128 - H % 128); }

                switch (xt2.header.DataFormat)
                {
                    case D3DFormat.A8R8G8B8:
                        OutSize = W * H * TexelPitch;
                        break;

                    case D3DFormat.DXT1: 
                        OutSize = W * H * TexelPitch;
                        break;

                    case D3DFormat.DXT2:
                        DataSize = w * H * 2;
                        OutSize = W * H * TexelPitch;
                        break;
                }

                byte[] data = new byte[DataSize];
                byte[] outdata = new byte[OutSize];

                Array.Copy(br.ReadBytes(DataSize), data, DataSize);

                int step = (xt2.header.Endian == 1 ? 2 : 4);
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

                var mip = new MipMap();
                mip.Width = xt2.width;
                mip.Height = xt2.height;
                mip.Data = data;
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

            alignedWidth = (w + 31) &~ 31;
            logBpp = (TexelPitch >> 2) + ((TexelPitch >> 1) >> (TexelPitch >> 2));
            Macro = ((x >> 5) + (y >> 5) * (alignedWidth >> 5)) << (logBpp + 7);
            Micro = (((x & 7) + ((y & 6) << 2)) << logBpp);
            Offset = Macro + ((Micro &~ 15) << 1) + (Micro & 15) + ((y & 8) << (3 + logBpp)) + ((y & 1) << 4);
            return (((Offset &~ 511) << 3) + ((Offset & 448) << 2) + (Offset & 63) + ((y & 16) << 7) + (((((y & 8) >> 2) + (x >> 3)) & 3) << 6)) >> logBpp;
        }
    }
}
