using System;
using System.IO;

namespace ToxicRagers.Carmageddon.Helpers
{
    public class DocumentWriter : IDisposable
    {
        public int CommentIndent { get; set; } = 8;

        public char CommentIndentCharacter { get; set; } = '\t';

        public char IndentCharacter { get; set; } = '\t';

        public int TabWidth { get; set; } = 8;

        private StreamWriter BaseStream { get; set; }

        private int CurrentIndent { get; set; } = 0;

        public DocumentWriter(string path)
        {
            BaseStream = new StreamWriter(path);
        }

        public void IncreaseIndent()
        {
            CurrentIndent++;
        }

        public void DecreaseIndent()
        {
            CurrentIndent--;
        }

        public void WriteLine()
        {
            BaseStream.WriteLine();
        }

        public void WriteLine(string value, string comment = null)
        {
            value = $"{new string(IndentCharacter, CurrentIndent)}{value}";
            BaseStream.Write(value);

            if (comment != null)
            {
                int indentOffset = CommentIndent - (value.Length / (CommentIndentCharacter == '\t' ? TabWidth : 1)) - CurrentIndent;
                
                BaseStream.Write($"{new string(CommentIndentCharacter, indentOffset)}// {comment}");
            }

            BaseStream.WriteLine();
        }

        public void WriteSection(string sectionHeading, string content, string comment = null)
        {
            sectionHeading = $"{new string(IndentCharacter, CurrentIndent)}[{sectionHeading}]";

            BaseStream.Write(sectionHeading);

            if (comment != null)
            {
                int indentOffset = CommentIndent - (sectionHeading.Length / (CommentIndentCharacter == '\t' ? TabWidth : 1)) - CurrentIndent;

                BaseStream.Write($"{new string(CommentIndentCharacter, indentOffset)}// {comment}");
            }

            BaseStream.WriteLine();
            BaseStream.Write(content);

            BaseStream.WriteLine();
            BaseStream.WriteLine();
        }

        public void Dispose()
        {
            BaseStream.Dispose();
        }
    }
}
