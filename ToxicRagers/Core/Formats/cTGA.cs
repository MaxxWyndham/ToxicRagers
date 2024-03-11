using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ToxicRagers.Core.Formats
{
    public class TGA
    {
        public enum ImageType
        {
            None = 0,
            ColourMapped = 1,
            TrueColour = 2,
            BlackandWhite = 3,
            ColourMappedRLE = 9,
            TrueColourRLE = 10,
            BlackandWhiteRLE = 11
        }
        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ImageType Type { get; set; }

        public byte PixelDepth { get; set; }

        public byte[] Data { get; set; }

        public static TGA Load(string path)
        {
            TGA tga;

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path))) { tga = Load(ms, Path.GetFileNameWithoutExtension(path)); }

            return tga;
        }

        public static TGA Load(Stream stream, string name = null)
        {
            TGA tga = new TGA
            {
                Name = name
            };

            using (BinaryReader br = new BinaryReader(stream))
            {
                byte idLength = br.ReadByte();
                byte colourMapType = br.ReadByte();
                tga.Type = (ImageType)br.ReadByte();

                if (idLength > 0) { throw new NotImplementedException("No support for TGA files with ID sections!"); }
                if (colourMapType == 0) { br.ReadBytes(5); } else { throw new NotImplementedException("No support for TGA files with ColourMaps!"); }

                int xOrigin = br.ReadInt16();
                int yOrigin = br.ReadInt16();
                tga.Width = br.ReadInt16();
                tga.Height = br.ReadInt16();
                tga.PixelDepth = br.ReadByte();
                byte imageDescriptor = br.ReadByte();
                byte size = (byte)(tga.PixelDepth / 8);

                switch (tga.Type)
                {
                    case ImageType.TrueColourRLE:
                        tga.Data = br.ReadBytes((int)br.BaseStream.Length - 13);
                        break;

                    default:
                        tga.Data = br.ReadBytes(tga.Width * tga.Height * size);
                        break;

                }
            }

            return tga;
        }

        public static TGA FromBitmap(Bitmap bitmap)
        {
            TGA tga = new TGA
            {
                Width = bitmap.Width,
                Height = bitmap.Height,
                Type = ImageType.TrueColour,
                PixelDepth = 32
            };

            byte[] raw = new byte[bitmap.Width * bitmap.Height * 4];

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(data.Scan0, raw, 0, data.Stride * data.Height);
            bitmap.UnlockBits(data);

            tga.Data = new byte[raw.Length];

            using (MemoryStream ms = new MemoryStream(tga.Data))
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                for (int y = data.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        int offset = y * data.Stride + (x * 4);

                        bw.Write(raw[offset + 0]);
                        bw.Write(raw[offset + 1]);
                        bw.Write(raw[offset + 2]);
                        bw.Write(raw[offset + 3]);
                    }
                }
            }

            return tga;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create)) { Save(fs); }
        }

        public void Save(Stream stream)
        {
            byte[] data = Save(this);
            stream.Write(data, 0, data.Length);
        }

        public static byte[] Save(TGA tga)
        {
            byte[] buffer;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((byte)0);          // ID length
                bw.Write((byte)0);          // Colourmap type
                bw.Write((byte)tga.Type);
                bw.Write((short)0);         // Colourmap first entry
                bw.Write((short)0);         // Colourmap length
                bw.Write((byte)0);          // Colourmap entry size
                bw.Write((short)0);         // X origin
                bw.Write((short)0);         // Y origin
                bw.Write((short)tga.Width);
                bw.Write((short)tga.Height);
                bw.Write(tga.PixelDepth);
                bw.Write((byte)0);          // ImageDescriptor

                bw.Write(tga.Data);

                bw.Flush();
                ms.Flush();

                buffer = ms.ToArray();
            }

            return buffer;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            BitmapData bmpdata;
            PixelFormat format = PixelFormat.Format32bppArgb;
            byte size = (byte)(PixelDepth / 8);

            if (size == 3) { format = PixelFormat.Format24bppRgb; }

            bmpdata = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, format);

            switch (Type)
            {
                case ImageType.TrueColourRLE:
                    const int rawSection = 127;
                    int step = 0;
                    int bpCount;
                    int currentPixel = 0;
                    int pixelCount = Width * Height;
                    byte[] colorBuffer = new byte[size];
                    byte chunkHeader;

                    using (MemoryStream nms = new MemoryStream())
                    {
                        while (currentPixel < pixelCount)
                        {
                            chunkHeader = Data[step];
                            step++;

                            if (chunkHeader <= rawSection)
                            {
                                chunkHeader++;
                                bpCount = size * chunkHeader;
                                nms.Write(Data, step, bpCount);
                                step += bpCount;

                                currentPixel += chunkHeader;
                            }
                            else
                            {
                                chunkHeader -= rawSection;
                                Array.Copy(Data, step, colorBuffer, 0, size);
                                step += size;
                                for (uint j = 0; j < chunkHeader; j++) { nms.Write(colorBuffer, 0, size); }
                                currentPixel += chunkHeader;
                            }
                        }

                        byte[] contentBuffer = new byte[nms.Length];
                        nms.Position = 0;
                        nms.Read(contentBuffer, 0, contentBuffer.Length);

                        Marshal.Copy(contentBuffer, 0, bmpdata.Scan0, contentBuffer.Length);
                    }
                    break;

                default:
                    Marshal.Copy(Data, 0, bmpdata.Scan0, Data.Length);
                    break;
            }

            bmp.UnlockBits(bmpdata);

            return bmp;
        }
    }
}
