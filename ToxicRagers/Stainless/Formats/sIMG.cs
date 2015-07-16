using System;
using System.Collections.Generic;
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
        int width;
        int height;
        List<Plane> planes = new List<Plane>();

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

        public IMG()
            : base()
        {
            this.extension = "IMG";
        }

        public static IMG Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            IMG img = new IMG();

            img.Name = fi.Name.Replace(fi.Extension, "");

            // TO DO

            return img;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                Parallel.ForEach(
                    this.planes,
                    p =>
                    {
                        p.Compress(Plane.CompressionMethod.Huffman);
                    }
                );

                bw.WriteString("IMAGEMAP");
                bw.Write(new byte[] { 0x1, 0x1 }); // version 1.1
                bw.Write((byte)(BasicFlags.Compressed | BasicFlags.DisableDownSample | BasicFlags.DisableMipMaps));
                bw.Write((byte)(AdvancedFlags.Huffman | AdvancedFlags.DontAutoJPEG));
                bw.Write(6);
                bw.Write(16 + this.planes[0].DataSize + this.planes[1].DataSize + this.planes[2].DataSize + this.planes[3].DataSize);
                bw.Write((short)this.width);
                bw.Write((short)this.height);
                bw.Write(0x64);

                for (int i = 3; i >= 0; i--) { bw.Write(this.planes[i].DataSize); }

                for (int i = 3; i >= 0; i--)
                {
                    bw.Write(planes[i].Output.ToArray());
                }

                //for (int i = 3; i >= 0; i--)
                //{
                //    for (int j = 0; j < planes[i].Data.Count; j++)
                //    {
                //        bw.Write(planes[i].Data[j].Count);
                //        bw.Write(planes[i].Data[j].Colour);
                //    }
                //}
            }
        }

        public void ImportFromBitmap(Bitmap bitmap)
        {
            width = bitmap.Width;
            height = bitmap.Height;

            byte[] iB = new byte[width * height * 4];

            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bmpdata.Scan0, iB, 0, bmpdata.Stride * bmpdata.Height);
            bitmap.UnlockBits(bmpdata);

            for (int i = 0; i < 4; i++)
            {
                planes.Add(new Plane(i));
                planes[i].Data = iB.ToList().Every(4, i).ToArray();
            }

            //for (int i = 0; i < iB.Length; i += 4)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        if (i == 0)
            //        {
            //            planes.Add(new Plane());
            //            planes[j].Data.Add(new ColourCount { Colour = iB[i + j], Count = 1 });
            //        }
            //        else
            //        {
            //            if (planes[j].LastEntry.Colour == iB[i + j] && planes[j].LastEntry.Count < 127)
            //            {
            //                planes[j].LastEntry.Count++;
            //            }
            //            else
            //            {
            //                //planes[j].Add(new ColourCount { Colour = iB[i + j], Count = 1 });
            //                planes[j].Data.Add(new ColourCount { Colour = iB[i + j], Count = 1 });
            //            }
            //        }
            //    }
            //}
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
            get { return true; } //return data.Count / data.Sum(cc => cc.Length) > 50; }
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
            get
            {
                switch (compressionMethod)
                {
                    case CompressionMethod.RLE:
                        return data.Length * 2;

                    case CompressionMethod.Huffman:
                        return output.Length;

                    case CompressionMethod.LIC:
                        return -1;

                    default:
                        return data.Length;
                }
            }
        }

        public ColourCount LastEntry
        {
            get { return new ColourCount(); }// return data[data.Count - 1]; }
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
                case CompressionMethod.Huffman:
                    compressHuffman();
                    break;
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

            //Console.WriteLine(string.Join(string.Empty, output.Cast<bool>().Select(bit => bit ? "1" : "0")));
        }
    }

    public class ColourCount
    {
        public byte Colour { get; set; }
        public byte Count { get; set; }
    }
}
