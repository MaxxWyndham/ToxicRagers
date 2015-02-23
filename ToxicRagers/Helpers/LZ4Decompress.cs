using System;
using System.IO;

namespace ToxicRagers.Helpers
{
    class LZ4Decompress : BinaryReader
    {
        // http://fastcompression.blogspot.co.uk/2011/05/lz4-explained.html

        public LZ4Decompress(Stream input)
            : base(input)
        {
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            int pos = 0;

            while (true)
            {
                byte token = this.ReadByte();
                int literalsLength = (token & 0xF0) >> 4;
                int matchLength = (token & 0x0F) + 4;

                if (literalsLength == 15)
                {
                    byte lengthToAdd = 255;

                    while (lengthToAdd == 255)
                    {
                        lengthToAdd = this.ReadByte();
                        literalsLength += lengthToAdd;
                    }
                }

                for (int i = 0; i < literalsLength; i++) { buffer[index + pos++] = this.ReadByte(); }

                if (this.BaseStream.Position == this.BaseStream.Length) { break; }

                int offset = this.ReadUInt16();

                if (matchLength == 19)
                {
                    byte matchToAdd = 255;

                    while (matchToAdd == 255)
                    {
                        matchToAdd = this.ReadByte();
                        matchLength += matchToAdd;
                    }
                }

                for (int i = 0; i < matchLength; i++)
                {
                    buffer[index + pos + i] = buffer[index + pos - offset + i];
                }

                pos += matchLength;
            }

            //int r, flags;
            //int i, j, k;
            //byte c;

            //for (i = 0; i < N - F; i++) { g_ring_buffer[i] = 0x20; }

            //r = N - F;

            //for (flags = 0; ; flags >>= 1)
            //{
            //    if ((flags & 0x100) == 0)
            //    {
            //        if (this.BaseStream.Position == this.BaseStream.Length) { break; }
            //        c = this.ReadByte();

            //        flags = c | 0xFF00;
            //    }

            //    if ((flags & 1) == 1)
            //    {
            //        if (this.BaseStream.Position == this.BaseStream.Length) { break; }
            //        c = this.ReadByte();
            //        buffer[index++] = c;
            //        g_ring_buffer[r] = c;
            //        r = (r + 1) & (N - 1);
            //    }
            //    else
            //    {
            //        if (this.BaseStream.Position == this.BaseStream.Length) { break; }
            //        i = this.ReadByte();
            //        if (this.BaseStream.Position == this.BaseStream.Length) { break; }
            //        j = this.ReadByte();

            //        i |= ((j & 0xF0) << 4);
            //        j = (j & 0x0F) + THRESHOLD;

            //        for (k = 0; k <= j; k++)
            //        {
            //            c = g_ring_buffer[(i + k) & (N - 1)];
            //            buffer[index++] = c;
            //            g_ring_buffer[r] = c;
            //            r = (r + 1) & (N - 1);
            //        }
            //    }

            //    if (index >= buffer.Length) { break; }
            //}

            return 0;
        }
    }
}
