using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ToxicRagers.Compression.Huffman;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public enum CompressionMethod
    {
        None = 0,
        RLE = 1,
        Huffman = 2,
        LIC = 3
    }

    public class IMG : ITexture
    {
        [Flags]
        public enum BasicFlags : byte
        {
            DisableCompressInTextureMemory = 0x01,
            Compressed = 0x02,
            OneBitAlpha = 0x04,
            Disable16bit = 0x08,
            AttachedDataSize = 0x10,
            DisableDownSample = 0x20,
            DisableMipMaps = 0x40,
            IsCubemap = 0x80
        }

        [Flags]
        public enum AdvancedFlags : byte
        {
            Huffman = 0x01,
            LIC = 0x02,
            UnknownCompression = 0x04,
            CrushToJPEG = 0x08,
            DontAutoJPEG = 0x10,
            SRGB = 0x20
        }

        public enum PlaneFormat
        {
            Format1planeARGB = 0,
            Format4planeXRGB = 5,
            Format4planeARGB = 6
        }

        Version version;
        BasicFlags basicFlags;
        AdvancedFlags advancedFlags;
        PlaneFormat planeFormat;

        int width;
        int height;
        ConcurrentBag<Plane> planes = new ConcurrentBag<Plane>();

        public string Name { get; set; }

        public string Extension { get; } = "img";

        public List<MipMap> MipMaps { get; set; } = new List<MipMap>();

        public D3DFormat Format { get; }

        public static IMG Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            IMG img = new IMG() { Name = Path.GetFileNameWithoutExtension(path) };

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                if (br.ReadByte() != 0x49 || // I
                    br.ReadByte() != 0x4D || // M
                    br.ReadByte() != 0x41 || // A
                    br.ReadByte() != 0x47 || // G
                    br.ReadByte() != 0x45 || // E
                    br.ReadByte() != 0x4D || // M
                    br.ReadByte() != 0x41 || // A
                    br.ReadByte() != 0x50)   // P
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid IMG file", path);
                    return null;
                }

                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                img.version = new Version(major, minor);
                img.basicFlags = (BasicFlags)br.ReadByte();
                img.advancedFlags = (AdvancedFlags)br.ReadByte();
                img.planeFormat = (PlaneFormat)br.ReadUInt32();
                int fileSize = (int)br.ReadUInt32();
                img.width = br.ReadUInt16();
                img.height = br.ReadUInt16();

                Logger.LogToFile(Logger.LogLevel.Info, "{0} : {1} : {2} : {3}", img.version, img.basicFlags, img.advancedFlags, img.planeFormat);

                if (img.version.Minor == 1) { br.ReadUInt32(); }

                int planeCount = (img.planeFormat == PlaneFormat.Format1planeARGB ? 1 : 4);

                for (int i = 0; i < planeCount; i++)
                {
                    img.planes.Add(new Plane(i) { Output = new byte[br.ReadUInt32()] });
                }

                for (int i = 0; i < img.planes.Count; i++)
                {
                    Plane plane = img.planes.First(p => p.Index == i);
                    plane.Output = br.ReadBytes(plane.Output.Length);
                    if (img.basicFlags.HasFlag(BasicFlags.Compressed))
                    {
                        plane.Decompress((img.advancedFlags.HasFlag(AdvancedFlags.Huffman) ? CompressionMethod.Huffman : CompressionMethod.RLE));
                    }
                }
            }

            return img;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                BasicFlags basic = 0;
                AdvancedFlags compression = 0;
                int dataSize = 0;

                if (planeFormat == PlaneFormat.Format1planeARGB)
                {
                    dataSize = planes.First().Output.Length;
                }
                else
                {
                    basic = BasicFlags.Compressed;

                    if (planes.Any(p => p.Method == CompressionMethod.RLE && p.PoorCompression))
                    {
                        Parallel.ForEach(
                            planes,
                            p =>
                            {
                                p.Compress(CompressionMethod.Huffman);
                            }
                        );

                        compression = AdvancedFlags.Huffman;
                    }
                    else if (planes.Any(p => p.Method == CompressionMethod.Huffman))
                    {
                        compression = AdvancedFlags.Huffman;
                    }

                    dataSize = (planes.Count * 4) + planes.Sum(p => p.DataSize);
                }

                bw.WriteString("IMAGEMAP");
                bw.Write(new byte[] { 0x1, 0x1 }); // version 1.1
                bw.Write((byte)(basic | BasicFlags.DisableDownSample | BasicFlags.DisableMipMaps));
                bw.Write((byte)(compression | AdvancedFlags.DontAutoJPEG));
                bw.Write((int)planeFormat);
                bw.Write(dataSize);
                bw.Write((short)width);
                bw.Write((short)height);
                bw.Write(0x64);

                if (planeFormat != PlaneFormat.Format1planeARGB)
                {
                    foreach (Plane plane in planes.OrderByDescending(p => p.Index))
                    {
                        bw.Write(plane.DataSize);
                    }
                }

                foreach (Plane plane in planes.OrderByDescending(p => p.Index))
                {
                    bw.Write(plane.Output);
                }
            }
        }

        public void ImportFromBitmap(Bitmap bitmap, CompressionMethod compression = CompressionMethod.RLE)
        {
            if (bitmap.Width <= 32 || bitmap.Height <= 32) { bitmap = bitmap.Resize(64, 64); }

            width = bitmap.Width;
            height = bitmap.Height;

            int planeCount = 4;// Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            if (bitmap.Width < 8 && bitmap.Height < 8) { planeCount = 1; }

            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] iB = new byte[4 * bmpdata.Width * bmpdata.Height];
            Marshal.Copy(bmpdata.Scan0, iB, 0, bmpdata.Stride * bmpdata.Height);
            bitmap.UnlockBits(bmpdata);

            Parallel.For(0, planeCount,
                i =>
                //for (int i = 0; i < planeCount; i++)
                {
                    Plane plane = new Plane(i) { Data = iB.ToList().Every(planeCount, i).ToArray() };
                    plane.Compress(planeCount > 1 ? compression : CompressionMethod.None);
                    planes.Add(plane);
                }
            );

            switch (planeCount)
            {
                case 1:
                    planeFormat = PlaneFormat.Format1planeARGB;
                    break;

                case 3:
                    planeFormat = PlaneFormat.Format4planeXRGB;
                    break;

                case 4:
                    planeFormat = PlaneFormat.Format4planeARGB;
                    break;
            }
        }

        public Bitmap ExportToBitmap()
        {
            Bitmap bmp = new Bitmap(width, height);
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte[] oB = new byte[4 * width * height];

            foreach (Plane plane in planes.OrderBy(p => p.Index))
            {
                if (plane.Data == null) { continue; }

                for (int i = 0; i < plane.Data.Length; i++)
                {
                    oB[(i * 4) + (3 - plane.Index)] = plane.Data[i];
                }
            }

            Marshal.Copy(oB, 0, bmpdata.Scan0, bmpdata.Stride * bmpdata.Height);
            bmp.UnlockBits(bmpdata);

            return bmp;
        }
    }

    public class Plane
    {
        int index;
        byte[] data;
        CompressionMethod compressionMethod = CompressionMethod.None;
        byte[] output;

        public bool PoorCompression => output.Length / (data.Length * 1.0f) > 0.5f;

        public CompressionMethod Method
        {
            get => compressionMethod;
            set => compressionMethod = value;
        }

        public byte[] Data
        {
            get => data;
            set => data = value;
        }

        public byte[] Output
        {
            get => output;
            set => output = value;
        }

        public int DataSize => output.Length;
        public int Index => index;

        public Plane(int index)
        {
            this.index = index;
        }

        public void Compress(CompressionMethod method)
        {
            switch (method)
            {
                case CompressionMethod.None:
                    reorderData();
                    break;

                case CompressionMethod.RLE:
                    compressRLE();
                    break;

                case CompressionMethod.Huffman:
                    compressHuffman();
                    break;
            }
        }

        private void reorderData()
        {
            compressionMethod = CompressionMethod.None;

            output = new byte[data.Length];

            for (int i = 0; i < data.Length; i += 4)
            {
                output[i + 0] = data[i + 3];
                output[i + 1] = data[i + 2];
                output[i + 2] = data[i + 1];
                output[i + 3] = data[i + 0];
            }
        }

        private void compressRLE()
        {
            compressionMethod = CompressionMethod.RLE;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                byte lastColour = 0;
                int count = 0;

                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != lastColour || count == 127)
                    {
                        if (i > 0)
                        {
                            bw.Write((byte)count);
                            bw.Write(lastColour);
                        }

                        lastColour = data[i];
                        count = 0;
                    }

                    count++;
                }

                output = ms.ToArray();
            }
        }

        private void compressHuffman()
        {
            compressionMethod = CompressionMethod.Huffman;

            Tree tree = new Tree();

            tree.BuildTree(data);

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                byte[] huffmanTable = tree.ToByteArray();

                bw.Write((byte)0x42); // B
                bw.Write((byte)0x54); // T
                bw.Write((byte)0x54); // T
                bw.Write((byte)0x42); // B
                bw.Write(32);
                bw.Write(1573120);
                bw.Write(huffmanTable.Length);
                bw.Write((short)4);
                bw.Write((short)1);
                bw.Write(tree.LeafCount);
                bw.Write(1);
                bw.Write(8);
                bw.Write(huffmanTable);
                bw.Write(tree.Encode(data));

                output = ms.ToArray();
            }
        }

        public void Decompress(CompressionMethod method)
        {
            switch (method)
            {
                case CompressionMethod.None:
                    //deorderData();
                    break;

                case CompressionMethod.RLE:
                    decompressRLE();
                    break;

                case CompressionMethod.Huffman:
                    decompressHuffman();
                    break;
            }
        }

        private void decompressRLE()
        {
            compressionMethod = CompressionMethod.RLE;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                for (int i = 0; i < output.Length; i += 2)
                {
                    int count = output[i + 0];
                    byte colour = output[i + 1];

                    for (int j = 0; j < count; j++)
                    {
                        bw.Write(colour);
                    }
                }

                data = ms.ToArray();
            }
        }

        private void decompressHuffman()
        {
            compressionMethod = CompressionMethod.Huffman;

            Tree tree = new Tree();

            using (MemoryStream msIn = new MemoryStream(output))
            using (BinaryReader br = new BinaryReader(msIn))
            using (MemoryStream msOut = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(msOut))
            {
                br.ReadBytes(4);    // BTTB
                br.ReadUInt32();    // 32
                br.ReadUInt32();    // 1573120
                int huffmanTableLength = (int)br.ReadUInt32();
                br.ReadUInt16();    // 4
                br.ReadUInt16();    // 1
                int leafCount = (int)br.ReadUInt32();
                br.ReadUInt32();    // 1
                br.ReadUInt32();    // 8

                tree.FromByteArray(br.ReadBytes(huffmanTableLength), leafCount);
                bw.Write(tree.Decode(br.ReadBytes(output.Length - (int)br.BaseStream.Position)));

                data = msOut.ToArray();
            }
        }
    }
}