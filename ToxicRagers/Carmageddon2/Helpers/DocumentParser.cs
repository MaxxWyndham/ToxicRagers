using System;
using System.IO;
using System.Text;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class DocumentParser : BinaryReader
    {
        public DocumentParser(string Path) : base(new FileStream(Path, FileMode.Open), Encoding.ASCII) { }


    }
}
