using System.IO;
using System.Text;

namespace ToxicRagers.Helpers
{
    public class BinaryMemoryStream : MemoryStream
    {
        public BinaryMemoryStream(byte[] buffer) : base(buffer) { }

        public byte[] ReadBytes(int count)
        {
            byte[] b = new byte[count];
            Read(b, 0, count);
            return b;
        }

        public char[] ReadChars(int count)
        {
            return Encoding.ASCII.GetString(ReadBytes(count)).ToCharArray();
        }

        public ushort ReadUInt16()
        {
            byte[] bytes = ReadBytes(2);
            return (ushort)(bytes[1] << 8 | bytes[0]);
        }

        public uint ReadUInt32()
        {
            byte[] bytes = ReadBytes(4);
            return (uint)(bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24);
        }

        public string ReadString(int length)
        {
            char[] c = ReadChars(length);
            int l = length;

            for (int i = 0; i < length; i++)
            {
                if (c[i] == 0)
                {
                    l = i;
                    break;
                }
            }

            return new string(c, 0, l);
        }
    }
}