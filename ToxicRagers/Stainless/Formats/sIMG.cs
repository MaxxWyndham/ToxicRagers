using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using ToxicRagers.Core.Formats;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class IMG : Texture
    {
        int width;
        int height;
        List<ColourCount>[] planes = new List<ColourCount>[4];

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
            Logger.LogToFile("{0}", path);
            IMG img = new IMG();

            img.Name = fi.Name.Replace(fi.Extension, "");

            // TO DO

            return img;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                bw.WriteString("IMAGEMAP");
                bw.Write(new byte[] { 0x1, 0x1 }); // version 1.1
                bw.Write((byte)(BasicFlags.Compressed | BasicFlags.DisableDownSample | BasicFlags.DisableMipMaps));
                bw.Write((byte)AdvancedFlags.DontAutoJPEG);
                bw.Write(6);
                bw.Write(16 + (this.planes[0].Count * 2) + (this.planes[1].Count * 2) + (this.planes[2].Count * 2) + (this.planes[3].Count * 2));
                bw.Write((short)this.width);
                bw.Write((short)this.height);
                bw.Write(0x64);

                for (int i = 3; i >= 0; i--) { bw.Write(this.planes[i].Count * 2); }

                for (int i = 3; i >= 0; i--)
                {
                    for (int j = 0; j < planes[i].Count; j++)
                    {
                        bw.Write(planes[i][j].Count);
                        bw.Write(planes[i][j].Colour);
                    }
                }
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

            for (int i = 0; i < iB.Length; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == 0)
                    {
                        planes[j] = new List<ColourCount> { new ColourCount { Colour = iB[i + j], Count = 1 } };
                    }
                    else
                    {
                        var lastColour = planes[j][planes[j].Count - 1];

                        if (lastColour.Colour == iB[i + j] && lastColour.Count < 127)
                        {
                            planes[j][planes[j].Count - 1].Count++;
                        }
                        else
                        {
                            planes[j].Add(new ColourCount { Colour = iB[i + j], Count = 1 });
                        }
                    }
                }
            }
        }
    }

    public class ColourCount
    {
        public byte Colour { get; set; }
        public byte Count { get; set; }
    }
}
