using System;
using System.IO;
using System.Text;

namespace ToxicRagers.Helpers
{
    class BEBinaryReader : BinaryReader
    {
        #region Constructors
        public BEBinaryReader(Stream input)
            : base(input)
        {
        }

        public BEBinaryReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }
        #endregion

        public override ushort ReadUInt16()
        {
            byte[] bytes = ReadBytes(2);
            return (ushort)(bytes[0] << 8 | bytes[1]);
        }

        public override uint ReadUInt32()
        {
            byte[] bytes = ReadBytes(4);
            UInt32[] ints = new UInt32[4];

            ints[0] = bytes[0];
            ints[1] = bytes[1];
            ints[2] = bytes[2];
            ints[3] = bytes[3];

            ints[0] <<= 24;
            ints[1] <<= 16;
            ints[2] <<= 8;
            ints[3] <<= 0;

            return (ints[0] | ints[1] | ints[2] | ints[3]);
        }

        public override float ReadSingle()
        {
            byte[] bytes = ReadBytes(4);
            byte[] reverse = new byte[4];
            reverse[0] = bytes[3];
            reverse[1] = bytes[2];
            reverse[2] = bytes[1];
            reverse[3] = bytes[0];
            return System.BitConverter.ToSingle(reverse, 0);
        }

        public string ReadStringOfLength(int length)
        {
            Char[] chars = ReadChars(length);

            string sout = "";
            for (int i = 0; i < chars.Length - 1; i++)
            {
                sout += chars[i];
            }

            return sout;
        }

        public override string ReadString()
        {
            string sout = "";
            int j = 0;

            while (1 == 1)
            {
                char s = ReadChar();
                if ((byte)s == 0) { j++; break; }
                sout += (s + "");
            }

            return sout;
        }

        public string[] ReadStrings(int count)
        {
            string[] sout = new string[count];
            int j = 0;

            while (j < count)
            {
                char s = ReadChar();
                if ((byte)s == 0) { j++; continue; }
                sout[j] += (s + "");
            }

            return sout;
        }
    }
}
