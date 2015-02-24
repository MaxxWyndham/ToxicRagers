using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Helpers
{
    public class DocumentParser
    {
        byte[] data;
        int position;
        bool bFiddled = false;

        public bool EOF
        {
            get { return position == data.Length; }
        }

        public bool Fiddled
        {
            get { return bFiddled; }
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(data);
        }

        public DocumentParser(string path)
        {
            using (var br = new BinaryReader(File.OpenRead(path))) {
                if (br.PeekChar() == '@')
                {
                    List<byte> processed = new List<byte>();

                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        var s = IWantToFiddle(ReadLine(br, 256));
                        if (s != null)
                        {
                            processed.AddRange(s);
                            processed.Add(13);
                            processed.Add(10);
                        }
                    }

                    data = new byte[processed.Count];
                    processed.CopyTo(data);

                    bFiddled = true;
                }
                else
                {
                    data = br.ReadBytes((int)br.BaseStream.Length);
                }
            }
        }

        private byte[] ReadLine(BinaryReader br, int buffSize)
        {
            var b = new byte[buffSize];
            int i = 0;

            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                byte c = br.ReadByte();

                if (c == '\r') { continue; }
                if (c == '\n') { break; }

                b[i++] = c;
            }

            var r = new byte[i];
            Array.Copy(b, r, i);

            return r;
        }

        private byte[] IWantToFiddle(byte[] s)
        {
            if (s[0] != '@') { return null; }

            byte[] d = new byte[s.Length - 1];

            var seeds = new byte[] { 
                0x6C, 0x1B, 0x99, 0x5F, 
                0xB9, 0xCD, 0x5F, 0x13,
                0xCB, 0x04, 0x20, 0x0E,
                0x5E, 0x1C, 0xA1, 0x0E
            };

            var commentSeeds = new byte[] { 
                0x67, 0xA8, 0xD6, 0x26,
                0xB6, 0xDD, 0x45, 0x1B,
                0x32, 0x7E, 0x22, 0x13,
                0x15, 0xC2, 0x94, 0x37
            };

            var key = seeds;

            Array.Copy(s, 1, d, 0, d.Length);
            int len = d.Length;
            int seed = len % 16;

            for (int i = 0; i < len; i++)
            {
                if (i >= 3 && d[i - 1] == '/' && d[i - 2] == '/')
                {
                    key = commentSeeds;
                }

                if (d[i] == '\t') { d[i] = 159; }

                byte c = (byte)(d[i] - 32);

                if ((c & 0x80) != 0x80)
                {
                    d[i] = (byte)((c ^ (key[seed] & 127)) + 32);
                }
                seed = (seed + 7) % 16;

                if (d[i] == 159) { d[i] = 9; }
            }

            return d;
        }

        public string ReadLine()
        {
            string r = "";

            while (r.Length == 0)
            {
                int commentPosition = -1;
                int read = 0;

                while (position < data.Length)
                {
                    byte c = data[position++];

                    read++;

                    if (commentPosition == -1 && read > 1 && data[position - 1] == '/' && data[position - 2] == '/') { commentPosition = read - 2; }
                    if (c == '\r') { continue; }
                    if (c == '\n') { break; }
                }

                r = Encoding.ASCII.GetString(data, position - read, (commentPosition > -1 ? commentPosition : read)).Trim();
            }

            return r;
        }

        public string PeekLine()
        {
            int p = position;
            string s = ReadLine();
            position = p;

            return s;
        }

        public int ReadInt()
        {
            return ReadLine().ToInt();
        }

        public Single ReadSingle()
        {
            return ReadLine().ToSingle();
        }

        public string[] ReadStrings()
        {
            return ReadLine().Split(',');
        }

        public int[] ReadInts()
        {
            var s = ReadStrings();
            var i = new int[s.Length];

            for (int j = 0; j < i.Length; j++)
            {
                i[j] = s[j].ToInt();
            }

            return i;
        }

        public Vector2 ReadVector2()
        {
            return Vector2.Parse(ReadLine());
        }

        public Vector3 ReadVector3()
        {
            return Vector3.Parse(ReadLine());
        }

        public Vector4 ReadVector4()
        {
            return Vector4.Parse(ReadLine());
        }
    }
}
