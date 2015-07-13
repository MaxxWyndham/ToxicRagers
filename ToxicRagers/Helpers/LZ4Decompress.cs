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

            return pos;
        }
    }
}
