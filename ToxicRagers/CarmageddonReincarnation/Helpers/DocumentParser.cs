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
        public int LineNum { get; set; }

        public long Position
        {
            get => position;
            set
            {
                br.BaseStream.Seek(value, SeekOrigin.Begin);
                position = value;
            }
        }

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
            position = br.BaseStream.Position;
            string s = "";
            bool bRead = true;

            while (bRead)
            {
                if (br.BaseStream.Position == br.BaseStream.Length) { break; }

                int c = br.PeekChar();

                switch (c)
                {
                    case '<':
                    case '[':
                        bRead = (s.Length == 0);
                        break;

                    case '>':
                    case ']':
                        s += br.ReadChar();
                        bRead = false;
                        break;

                    case 10:
                        if (s.Length == 0) { LineNum++; }
                        bRead = (s.Length == 0);
                        break;

                    case 13:
                        bRead = (s.Length == 0);
                        break;

                    case '/':
                        if (s.Length == 1 && s[0] == '/')
                        {
                            s = "";
                            while (br.ReadChar() != 10) { }
                        }
                        break;
                }

                if (bRead) { s += br.ReadChar(); }
                if (s.Trim().Length == 0) { s = s.Trim(); }
            }

            if (s.IndexOf("/") > -1) { s = s.Substring(0, s.IndexOf("/")).Trim(); } else { s = s.Trim(); }

            lastLine = (s.Length > 0 ? s : null);
            LineNum++;

            return lastLine;
        }

        public int ReadInt()
        {
            string s = ReadNextLine();

            if (int.TryParse(s, out int i))
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

        public Colour ReadColour()
        {
            return Colour.Parse(ReadNextLine());
        }

        public string[] ReadStringArray(int expectedLength = -1)
        {
            string[] a = ReadNextLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
            br.BaseStream.Seek(position, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            br.Close();
        }
    }
}