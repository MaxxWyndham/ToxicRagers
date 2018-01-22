using System.IO;
using System.Text;

using ToxicRagers.Helpers;

using unluacNet;
using unluac.decompile;
using unluac.parse;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class LOL
    {
        string document = "";

        public string Document
        {
            get => document;
            set => document = value;
        }

        public static LOL Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            LOL lol = new LOL();

            byte[] data = File.ReadAllBytes(path);

            if (data[0] == 0x1b &&
                data[1] == 0x4c &&
                data[2] == 0x75 &&
                data[3] == 0x61 &&
                data[4] == 0x51)
            {
                StringBuilder sb = new StringBuilder();

                data[5] = 0;

                ByteBuffer buffer = new ByteBuffer(data);
                BHeader header = new BHeader(buffer);

                Decompiler d = new Decompiler(header.function.parse(buffer, header));
                d.decompile();
                d.print(new Output((s) => { sb.Append(s); }, () => { sb.Append("\r\n"); }));

                lol.document = sb.ToString();
            }
            else
            {
                lol.document = Encoding.Default.GetString(data);
            }

            return lol;
        }

        public byte[] ReadAllBytes()
        {
            return Encoding.ASCII.GetBytes(document);
        }
    }
}