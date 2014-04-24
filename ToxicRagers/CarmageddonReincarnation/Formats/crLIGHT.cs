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

        public static LIGHT Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            LIGHT light = new LIGHT();

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadUInt32() != 3)
                {
                    Logger.LogToFile("{0} isn't a valid LIGHT file", Path);
                    return null;
                }

                light.type = (LightType)br.ReadUInt32();

                Logger.LogToFile("Light type: {0}", light.type);

                Logger.LogToFile("{0}", br.ReadSingle());    //0.8000001
                Logger.LogToFile("{0}", br.ReadSingle());    //0.0509804
                Logger.LogToFile("{0}", br.ReadSingle());    //0
                Logger.LogToFile("{0}", br.ReadSingle());    //1
                Logger.LogToFile("{0}", br.ReadSingle());    //2
                Logger.LogToFile("{0}", br.ReadSingle());    //0
                Logger.LogToFile("{0}", br.ReadSingle());    //120

                light.transform = new Matrix3D(
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                              );    // This may not actually be a matrix :/

                Logger.LogToFile("Transform: {0}", light.transform.ToString());

                Logger.LogToFile("{0}", br.ReadUInt32());    // 1
                Logger.LogToFile("{0} {1} {2} {3}", br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());    // 0 0 0 255
                Logger.LogToFile("{0}", br.ReadSingle());    //1
                Logger.LogToFile("{0}", br.ReadSingle());    //1
                Logger.LogToFile("{0}", br.ReadSingle());    //0
                Logger.LogToFile("{0}", br.ReadSingle());    //0

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
