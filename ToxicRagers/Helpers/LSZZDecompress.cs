using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ToxicRagers.Helpers
{
    class LSZZDecompress : BinaryReader
    {
        // "Based" on the code found here:
        // http://my.execpc.com/~geezer/code/lzss.c

        const int N = 4096;
        const int F = 16;
        const int THRESHOLD = 2;
        static byte[] g_ring_buffer = new byte[N + F - 1];

        public LSZZDecompress(Stream input)
            : base(input)
        {
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            int r, flags;
            int i, j, k;
            byte c;

            for (i = 0; i < N - F; i++) { g_ring_buffer[i] = 0x20; }

            r = N - F;

            for (flags = 0; ; flags >>= 1)
            {
                if ((flags & 0x100) == 0)
                {
                    if (this.BaseStream.Position == this.BaseStream.Length) { break; }
                    c = this.ReadByte();

                    flags = c | 0xFF00;
                }

                if ((flags & 1) == 1)
                {
                    if (this.BaseStream.Position == this.BaseStream.Length) { break; }
                    c = this.ReadByte();
                    buffer[index++] = c;
                    g_ring_buffer[r] = c;
                    r = (r + 1) & (N - 1);
                }
                else
                {
                    if (this.BaseStream.Position == this.BaseStream.Length) { break; }
                    i = this.ReadByte();
                    if (this.BaseStream.Position == this.BaseStream.Length) { break; }
                    j = this.ReadByte();

                    i |= ((j & 0xF0) << 4);
                    j = (j & 0x0F) + THRESHOLD;

                    for (k = 0; k <= j; k++)
                    {
                        c = g_ring_buffer[(i + k) & (N - 1)];
                        buffer[index++] = c;
                        g_ring_buffer[r] = c;
                        r = (r + 1) & (N - 1);
                    }
                }

                if (index >= buffer.Length) { break; }
            }

            return count;
        }
    }
}
