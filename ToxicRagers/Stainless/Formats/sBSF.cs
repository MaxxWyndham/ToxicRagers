using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class BSF : Dictionary<string, string>
    {
        public static BSF Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, $"{path}");

            BSF bsf = new BSF();

            using (BinaryReader br = new BinaryReader(fi.OpenRead(), Encoding.Unicode))
            {
                if (br.ReadByte() != 0x42 || // B
                    br.ReadByte() != 0x5a || // Z
                    br.ReadByte() != 0x42 || // B
                    br.ReadByte() != 0x54)   // T
                {
                    Logger.LogToFile(Logger.LogLevel.Error, $"{path} isn't a valid BSF file");
                    return null;
                }

                br.ReadUInt16();
                br.ReadUInt16();
                br.ReadUInt32();
                br.ReadUInt32();

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    br.ReadByte();
                    byte keyLength = br.ReadByte();
                    short valLength = br.ReadInt16();

                    bsf.Add(new string(br.ReadChars(keyLength), 0, keyLength), new string(br.ReadChars(valLength), 0, valLength));
                }
            }

            return bsf;
        }
    }
}