using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class BSF : Dictionary<string, string>
    {
        public static BSF Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, $"BSF loading {path}");

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

        // With thanks to ManIkWeet (PR#4)
        public void Save(string path)
        {
            var fileInfo = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, $"BSF saving {path}");

            using (BinaryWriter writer = new BinaryWriter(File.Create(fileInfo.FullName), Encoding.Unicode))
            {
                writer.Write((byte)0x42); // B
                writer.Write((byte)0x5a); // Z
                writer.Write((byte)0x42); // B
                writer.Write((byte)0x54); // T

                //no clue what these mean
                writer.Write((ushort)1);
                writer.Write((ushort)2);
                writer.Write((uint)16);
                writer.Write((uint)0);

                foreach (var kvp in this)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;
                    writer.Write((byte)0); //all entries seem prefixed with a 0 byte
                    writer.Write((byte)key.Length);
                    writer.Write((short)value.Length); //odd, not ushort? possible mistake in the read function? depends on endianness
                    writer.Write(key.ToCharArray());
                    writer.Write(value.ToCharArray());
                }
            }
        }
    }
}