using System;

namespace ToxicRagers.Helpers
{
    public class ColourBlock
    {
        public static int FloatToInt(float a, int limit)
        {
            // use ANSI round-to-zero behaviour to get round-to-nearest
            int i = (int)(a + 0.5f);

            // clamp to the limit
            if (i < 0)
                i = 0;
            else if (i > limit)
                i = limit;

            // done
            return i;
        }

        static int FloatTo565(Vector3 colour)
        {
            // get the components in the correct range
            int r = FloatToInt(31.0f * colour.X, 31);
            int g = FloatToInt(63.0f * colour.Y, 63);
            int b = FloatToInt(31.0f * colour.Z, 31);

            // pack into a single value
            return (r << 11) | (g << 5) | b;
        }

        static void WriteColourBlock(int a, int b, byte[] indices, ref byte[] block, int offset)
        {
            // write the endpoints
            block[offset + 0] = (byte)(a & 0xff);
            block[offset + 1] = (byte)(a >> 8);
            block[offset + 2] = (byte)(b & 0xff);
            block[offset + 3] = (byte)(b >> 8);

            // write the indices
            for (int i = 0; i < 4; ++i)
            {
                block[offset + 4 + i] = (byte)(indices[(4 * i) + 0] | (indices[(4 * i) + 1] << 2) | (indices[(4 * i) + 2] << 4) | (indices[(4 * i) + 3] << 6));
            }
        }

        public static void WriteColourBlock3(Vector3 start, Vector3 end, byte[] indices, ref byte[] block, int offset)
        {
            // get the packed values
            int a = FloatTo565(start);
            int b = FloatTo565(end);

            // remap the indices
            byte[] remapped = new byte[16];
            if (a <= b)
            {
                // use the indices directly
                for (int i = 0; i < 16; ++i)
                    remapped[i] = indices[i];
            }
            else
            {
                // swap a and b
                int t = a;
                a = b;
                b = t;
                for (int i = 0; i < 16; ++i)
                {
                    if (indices[i] == 0)
                        remapped[i] = 1;
                    else if (indices[i] == 1)
                        remapped[i] = 0;
                    else
                        remapped[i] = indices[i];
                }
            }

            // write the block
            WriteColourBlock(a, b, remapped, ref block, offset);
        }

        public static void WriteColourBlock4(Vector3 start, Vector3 end, byte[] indices, ref byte[] block, int offset)
        {
            // get the packed values
            int a = FloatTo565(start);
            int b = FloatTo565(end);

            // remap the indices
            byte[] remapped = new byte[16];
            if (a < b)
            {
                // swap a and b
                int t = a;
                a = b;
                b = t;
                for (int i = 0; i < 16; ++i)
                    remapped[i] = (byte)((indices[i] ^ 0x1) & 0x3);
            }
            else if (a == b)
            {
                // use index 0
                for (int i = 0; i < 16; ++i)
                    remapped[i] = 0;
            }
            else
            {
                // use the indices directly
                for (int i = 0; i < 16; ++i)
                    remapped[i] = indices[i];
            }

            // write the block
            WriteColourBlock(a, b, remapped, ref block, offset);
        }
    }
}
