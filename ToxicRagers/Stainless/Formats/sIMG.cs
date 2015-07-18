using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ToxicRagers.Compression.Huffman;
using ToxicRagers.Core.Formats;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class IMG : Texture
    {
        Version version;
        BasicFlags basicFlags;
        AdvancedFlags advancedFlags;
        PixelFormat pixelFormat;

        int width;
        int height;
        ConcurrentBag<Plane> planes = new ConcurrentBag<Plane>();

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

        public enum PixelFormat
        {
            Format24bitRGB = 5,
            Format32bitARGB = 6
        }

        public IMG()
            : base()
        {
            this.extension = "IMG";
        }

        public static IMG Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            IMG img = new IMG();

            img.Name = Path.GetFileNameWithoutExtension(path);

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
                img.pixelFormat = (PixelFormat)br.ReadUInt32();
                int fileSize = (int)br.ReadUInt32();
                img.width = br.ReadUInt16();
                img.height = br.ReadUInt16();

                Logger.LogToFile(Logger.LogLevel.Info, "{0} : {1} : {2} : {3}", img.version, img.basicFlags, img.advancedFlags, img.pixelFormat);

                if (img.version.Minor == 1) { br.ReadUInt32(); }
            }

            return img;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                AdvancedFlags compression = 0;

                if (this.planes.Any(p => p.PoorCompression))
                {
                    Parallel.ForEach(
                        this.planes,
                        p =>
                        {
                            p.Compress(Plane.CompressionMethod.Huffman);
                        }
                    );

                    compression = AdvancedFlags.Huffman;
                }

                bw.WriteString("IMAGEMAP");
                bw.Write(new byte[] { 0x1, 0x1 }); // version 1.1
                bw.Write((byte)(BasicFlags.Compressed | BasicFlags.DisableDownSample | BasicFlags.DisableMipMaps));
                bw.Write((byte)(compression | AdvancedFlags.DontAutoJPEG));
                bw.Write(6);
                bw.Write(16 + this.planes.Sum(p => p.DataSize));
                bw.Write((short)this.width);
                bw.Write((short)this.height);
                bw.Write(0x64);

                foreach (var plane in this.planes.OrderByDescending(p => p.Index))
                {
                    bw.Write(plane.DataSize);
                }

                foreach (var plane in this.planes.OrderByDescending(p => p.Index))
                {
                    bw.Write(plane.Output);
                }
            }
        }

        public void ImportFromBitmap(Bitmap bitmap)
        {
            width = bitmap.Width;
            height = bitmap.Height;

            byte[] iB = new byte[width * height * 4];

            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(bmpdata.Scan0, iB, 0, bmpdata.Stride * bmpdata.Height);
            bitmap.UnlockBits(bmpdata);

            Parallel.For(0, 4,
                i =>
                {
                    var plane = new Plane(i);
                    plane.Data = iB.ToList().Every(4, i).ToArray();
                    plane.Compress(Plane.CompressionMethod.RLE);
                    planes.Add(plane);
                }
            );
        }
    }

    public class Plane
    {
        public enum CompressionMethod
        {
            None = 0,
            RLE = 1,
            Huffman = 2,
            LIC = 3
        }

        int index;
        byte[] data;
        CompressionMethod compressionMethod = CompressionMethod.RLE;
        byte[] output;

        public bool PoorCompression
        {
            get { return output.Length / (data.Length * 1.0f) > 0.5f; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public byte[] Output
        {
            get { return output; }
        }

        public int DataSize
        {
            get { return output.Length; }
        }

        public int Index { get { return index; } }

        public Plane(int index)
        {
            this.index = index;
        }

        public void Compress(CompressionMethod method)
        {
            switch (method)
            {
                case CompressionMethod.RLE:
                    compressRLE();
                    break;

                case CompressionMethod.Huffman:
                    compressHuffman();
                    break;
            }
        }

        private void compressRLE()
        {
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

                output = ms.GetBuffer();
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
                bw.Write(tree.ToByteArray());
                bw.Write(tree.Encode(data));

                output = ms.GetBuffer();
            }
        }
    }
}
