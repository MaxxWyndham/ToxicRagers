using System;

namespace ToxicRagers.Helpers
{
    public class CRC32
    {
        private uint[] table = new uint[256];
        private uint result = 0xffffffff;

        public CRC32()
            : this(0xedb88320)
        {
        }

        public CRC32(uint polynomial)
        {
            for (uint i = 0; i < 256; i++)
            {
                uint crc32 = i;

                for (int j = 8; j > 0; j--)
                {
                    if ((crc32 & 1) == 1)
                    {
                        crc32 = (crc32 >> 1) ^ polynomial;
                    }
                    else
                    {
                        crc32 >>= 1;
                    }
                }
                table[i] = crc32;
            }
        }

        public byte[] Hash(Byte[] array)
        {
            int start = 0;
            int size = array.Length;
            int end = start + size;

            result = 0xffffffff;

            for (int i = start; i < end; i++)
            {
                result = (result >> 8) ^ table[array[i] ^ (result & 0xff)];
            }

            result = ~result;

            return BitConverter.GetBytes(result);
        }
    }
}
