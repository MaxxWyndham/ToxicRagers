using System;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class LIGHT
    {
        enum LightType
        {
            Point = 0,
            Directional = 1,
            Spot = 2
        }

        LightType type;
        string name;
        Matrix3D transform;
        Single r;
        Single g;
        Single b;
        Single intensity;

        public static LIGHT Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            //Logger.LogToFile("{0}", Path);
            LIGHT light = new LIGHT();

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadUInt32() != 3)
                {
                    Logger.LogToFile("{0} isn't a valid LIGHT file", path);
                    return null;
                }

                light.type = (LightType)br.ReadUInt32();

                //Logger.LogToFile("Light type: {0}", light.type);

                light.r = br.ReadSingle();
                light.g = br.ReadSingle();
                light.b = br.ReadSingle();
                light.intensity = br.ReadSingle();
                //Logger.LogToFile("{0}", br.ReadSingle());    //2
                //Logger.LogToFile("{0}", br.ReadSingle());    //0
                //Logger.LogToFile("{0}", br.ReadSingle());    //120

                Logger.LogToFile("{17}\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{16}", light.intensity, br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadUInt32(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), light.type, path.Replace(@"G:\Carmageddon_Reincarnation\2015-02-01\Content\", ""), br.ReadUInt32(), br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                return null;

                int nameLength = (int)br.ReadUInt32();
                if (nameLength > 0)
                {
                    light.name = br.ReadString(nameLength);
                    Logger.LogToFile("Light name: {0}", light.name);
                }

                if (br.BaseStream.Position != br.BaseStream.Length) { Logger.LogToFile("Incomplete"); }
            }

            return light;
        }
    }
}
