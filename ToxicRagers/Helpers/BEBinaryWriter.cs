using System;
using System.IO;
using System.Text;

namespace ToxicRagers.Helpers
{
    class BEBinaryWriter : BinaryWriter
    {
        #region Constructors
        public BEBinaryWriter()
            : base()
        {
        }

        public BEBinaryWriter(Stream output)
            : base(output)
        {
        }

        public BEBinaryWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {
        }
        #endregion

        public void WriteByte(byte b)
        {
            Write(b);
        }

        public void WriteInt16(int i)
        {
            byte[] b = BitConverter.GetBytes((short)i);
            Array.Reverse(b);
            Write(BitConverter.ToInt16(b, 0));
        }

        public void WriteInt32(int i)
        {
            byte[] b = BitConverter.GetBytes(i);
            Array.Reverse(b);
            Write(BitConverter.ToInt32(b, 0));
        }

        public void WriteSingle(float s)
        {
            byte[] b = BitConverter.GetBytes(s);
            Array.Reverse(b);
            Write(BitConverter.ToSingle(b, 0));
        }
    }
}