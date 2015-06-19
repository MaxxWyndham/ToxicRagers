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
        byte[] data;

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

            //using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            //{
            //    if (br.ReadByte() != 0x00 ||
            //        br.ReadByte() != 0x02)
            //    {
            //        Logger.LogToFile("{0} isn't a valid TDX file", path);
            //        return null;
            //    }

            //    tdx.width = (int)br.ReadUInt16();
            //    tdx.height = (int)br.ReadUInt16();
            //    int mipCount = (int)br.ReadUInt16();
            //    tdx.flags = (Flags)br.ReadUInt32();
            //    tdx.Format = (D3DFormat)br.ReadUInt32();

            //    if (tdx.flags.HasFlag(Flags.ExtraData))
            //    {
            //        int extraDataLength = (int)br.ReadUInt32();

            //        Logger.LogToFile("Skipped {0} bytes of extra data", extraDataLength);
            //        br.ReadBytes(extraDataLength);

            //        //Logger.LogToFile("{0}", br.ReadUInt16());
            //        //Logger.LogToFile("{0}", br.ReadUInt16());

            //        //br.ReadUInt32(); // 0xDEADBEEF
            //        //Logger.LogToFile("DEADBEEF");

            //        //Logger.LogToFile("{0}", br.ReadUInt32());
            //        //Logger.LogToFile("{0}", br.ReadUInt32());
            //        //Logger.LogToFile("{0}", br.ReadUInt32());
            //        //Logger.LogToFile("{0}", br.ReadUInt32());
            //        //Logger.LogToFile("{0}", br.ReadUInt32());

            //        //br.ReadUInt32(); // 0xDEADBEEF
            //        //Logger.LogToFile("DEADBEEF");

            //        //int fileCount = (int)br.ReadUInt32();

            //        //for (int i = 0; i < fileCount; i++)
            //        //{
            //        //    int x = (int)br.ReadUInt32();
            //        //    int y = (int)br.ReadUInt32();
            //        //    int w = (int)br.ReadUInt32();
            //        //    int h = (int)br.ReadUInt32();

            //        //    string file = br.ReadNullTerminatedString();

            //        //    byte b = br.ReadByte();

            //        //    Logger.LogToFile("{0}\t{1}\t{2}\t{3}\t{5}\t{4}", x, y, w, h, file, b);
            //        //}

            //        //br.ReadUInt32(); // 0xDEADBEEF
            //        //Logger.LogToFile("DEADBEEF");

            //        //int indexCount = (int)br.ReadUInt32();

            //        //for (int i = 0; i < indexCount; i++)
            //        //{
            //        //    int x = (int)br.ReadUInt32();
            //        //    int y = (int)br.ReadUInt32();
            //        //    int z = (int)br.ReadUInt32();
            //        //    uint h = br.ReadUInt32();

            //        //    Logger.LogToFile("{0}\t{1}\t{2}\t{3}\t:{3:x2}", x, y, z, h);
            //        //}

            //        //br.ReadUInt32(); // 0xDEADBEEF
            //        //Logger.LogToFile("DEADBEEF");

            //        //fileCount = (int)br.ReadUInt32();

            //        //for (int i = 0; i < fileCount; i++)
            //        //{
            //        //    string file = br.ReadNullTerminatedString();
            //        //    int a = (int)br.ReadUInt32();
            //        //    int b = (int)br.ReadUInt32();

            //        //    Logger.LogToFile("{0}\t{1}\t{2}", a, b, file);
            //        //}

            //        //br.ReadUInt32(); // 0xDEADBEEF
            //        //Logger.LogToFile("DEADBEEF");

            //        //Logger.LogToFile("{0}", br.ReadUInt32());

            //        //br.ReadUInt32(); // 0xDEADBEEF
            //        //Logger.LogToFile("DEADBEEF");
            //    }

            //    for (int i = 0; i < mipCount; i++)
            //    {
            //        var mip = new MipMap();
            //        mip.Width = tdx.width >> i;
            //        mip.Height = tdx.height >> i;

            //        switch (tdx.Format)
            //        {
            //            case D3DFormat.A8R8G8B8:
            //                mip.Data = br.ReadBytes(mip.Width * mip.Height * 4);
            //                break;

            //            case D3DFormat.A8:
            //                mip.Data = br.ReadBytes(mip.Width * mip.Height);
            //                break;

            //            case D3DFormat.DXT1:
            //                mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 8);
            //                break;

            //            case D3DFormat.ATI2:
            //            case D3DFormat.DXT5:
            //                mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 16);
            //                break;

            //            default:
            //                Logger.LogToFile("Unknown format: {0}", tdx.Format);
            //                return null;
            //        }

            //        tdx.MipMaps.Add(mip);
            //    }
            //}

            return img;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                bw.WriteString("IMAGEMAP");
                bw.Write(new byte[] { 0x0, 0x1, 0x4b, 0x0 });
                bw.Write(1);
                bw.Write(this.data.Length); // Filesize, we'll update this later
                bw.Write((short)this.width);
                bw.Write((short)this.height);
                bw.Write(this.data);
            }
        }

        public void ImportFromBitmap(Bitmap bitmap)
        {
            width = bitmap.Width;
            height = bitmap.Height;

            byte[] iB = new byte[width * height * 4];
            byte[] oB = new byte[iB.Length * 2];

            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bmpdata.Scan0, iB, 0, bmpdata.Stride * bmpdata.Height);
            bitmap.UnlockBits(bmpdata);

            byte[] lastColour = new byte[4];
            byte count = 0;
            int j = 0;

            for (int i = 0; i < iB.Length; i += 4)
            {
                byte[] colour = new byte[4];

                colour[0] = iB[i + 0];
                colour[1] = iB[i + 1];
                colour[2] = iB[i + 2];
                colour[3] = iB[i + 3];

                if (!colour.SequenceEqual(lastColour) || count == 127)
                {
                    if (i > 0)
                    {
                        oB[j++] = count;
                        oB[j++] = lastColour[3];
                        oB[j++] = lastColour[2];
                        oB[j++] = lastColour[1];
                        oB[j++] = lastColour[0];
                    }

                    Array.Copy(colour, lastColour, 4);
                    count = 0;
                }

                count++;
            }

            oB[j++] = count;
            oB[j++] = lastColour[3];
            oB[j++] = lastColour[2];
            oB[j++] = lastColour[1];
            oB[j++] = lastColour[0];

            data = new byte[j];
            Array.Copy(oB, 0, data, 0, data.Length);
        }
    }
}
