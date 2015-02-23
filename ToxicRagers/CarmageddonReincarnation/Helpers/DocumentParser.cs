using System;
using System.IO;
using System.Text;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Helpers
{
    public class DocumentParser : IDisposable
    {
        BinaryReader br;
        long position;
        static string lastLine;

        public DocumentParser(string path)
        {
            br = new BinaryReader(new FileStream(path, FileMode.Open), Encoding.ASCII);
        }

        public bool NextLineIsASection()
        {
            string s = ReadNextLine();
            bool bIsSection = (s != null && s.StartsWith("["));
            Rewind();

            return bIsSection;
        }

        public bool EOF()
        {
            string s = ReadNextLine();
            bool bIsEOF = (s == null);
            Rewind();

            return bIsEOF;
        }

        public string SkipToNextSection()
        {
            string s = null;

            while (!NextLineIsASection()) { s = ReadNextLine(); }
            s = ReadNextLine();

            return s;
        }

        public string ReadFirstLine()
        {
            // keeps reading lines until it reaches a valid one or EOF
            string s = ReadNextLine();
            while (s == null) { s = ReadNextLine(); }
            return s;
        }

        public string ReadNextLine()
        {
            this.position = this.br.BaseStream.Position;
            string s = "";
            bool bRead = true;

            while (bRead)
            {
                if (this.br.BaseStream.Position == this.br.BaseStream.Length) { break; }

                int c = this.br.PeekChar();

                switch (c)
                {
                    case '<':
                    case '[':
                        bRead = (s.Length == 0);
                        break;

                    case '>':
                    case ']':
                        s += this.br.ReadChar();
                        bRead = false;
                        break;

                    case 10:
                    case 13:
                        bRead = (s.Length == 0);
                        break;

                    case '/':
                        if (s.Length == 1 && s[0] == '/')
                        {
                            s = "";
                            while (this.br.ReadChar() != 10) { }
                        }
                        break;
                }

                if (bRead) { s += this.br.ReadChar(); }
                if (s.Trim().Length == 0) { s = s.Trim(); }
            }

            if (s.IndexOf("/") > -1) { s = s.Substring(0, s.IndexOf("/")).Trim(); } else { s = s.Trim(); }

            lastLine = (s.Length > 0 ? s : null);

            return lastLine;
        }

        public int ReadInt()
        {
            int i;
            string s = ReadNextLine();

            if (int.TryParse(s, out i))
            {
                return i;
            }

            throw new ArithmeticException(string.Format("Expected an int, received {0}", s));
        }

        public float ReadFloat()
        {
            return Convert.ToSingle(ReadNextLine(), ToxicRagers.Culture);
        }

        public Vector3 ReadVector3()
        {
            return Vector3.Parse(ReadNextLine());
        }

        public string[] ReadStringArray(int expectedLength = -1)
        {
            var a = ReadNextLine().Split(' ');

            if (expectedLength == -1)
            {
                return a;
            }
            else
            {
                if (a.Length != expectedLength)
                {
                    return null;
                }
                else
                {
                    return a;
                }
            }
        }

        public void Rewind()
        {
            this.br.BaseStream.Seek(position, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            br.Close();
        }
    }
}
