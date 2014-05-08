using System;
using System.IO;
using System.Text;

namespace ToxicRagers.CarmageddonReincarnation.Helpers
{
    public class DocumentParser : BinaryReader
    {
        long position;

        public DocumentParser(string path) : base(new FileStream(path, FileMode.Open), Encoding.ASCII) { }

        public string SkipToNextSection()
        {
            string s = ReadNextLine();

            while (s != null && !s.StartsWith("[")) { s = ReadNextLine(); }

            return s;
        }

        public string ReadNextLine()
        {
            this.position = this.BaseStream.Position;
            string s = "";
            bool bRead = true;

            while (bRead)
            {
                if (this.BaseStream.Position == this.BaseStream.Length) { break; }

                int c = this.PeekChar();

                switch (c)
                {
                    case '<':
                    case '[':
                        bRead = (s.Trim().Length == 0);
                        break;

                    case '>':
                    case ']':
                        s += this.ReadChar();
                        bRead = false;
                        break;

                    case 10:
                    case 13:
                        bRead = (s.Trim().Length == 0);
                        break;
                }

                if (bRead) { s += this.ReadChar(); }
            }

            if (s.IndexOf("/") > -1) { s = s.Substring(0, s.IndexOf("/")).Trim(); } else { s = s.Trim(); }

            return (s.Length > 0 ? s : null);
        }

        public void Rewind()
        {
            this.BaseStream.Seek(position, SeekOrigin.Begin);
        }
    }
}
