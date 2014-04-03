using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class CNT
    {
        string name;
        string nodeName;
        string modelName;
        Matrix3D transform;
        List<CNT> childNodes = new List<CNT>();

        public string Name { get { return name; } }
        public string NodeName { get { return (nodeName == null ? modelName : nodeName); } }
        public string Model { get { return modelName; } }
        public List<CNT> Children { get { return childNodes; } }

        public static CNT Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            CNT cnt;

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 69 ||
                    br.ReadByte() != 35 ||
                    br.ReadByte() != 0 ||
                    br.ReadByte() != 4)
                {
                    Logger.LogToFile("{0} isn't a valid CNT file", Path);
                    return null;
                }

                cnt = Load(br);

                Logger.LogToFile("{0} :: {1}", br.BaseStream.Position.ToString("X"), br.BaseStream.Length.ToString("X"));
                if (br.BaseStream.Position != br.BaseStream.Length) { Logger.LogToFile("Still has data remaining"); }
            }

            return cnt;
        }

        private static CNT Load(BinaryReader br)
        {
            // The Load(BinaryReader) version skips the header check and is used for recursive loading
            CNT cnt = new CNT();

            Logger.LogToFile("{0}", br.BaseStream.Position.ToString("X"));

            int nameLength = (int)br.ReadUInt32();
            int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

            cnt.name = br.ReadString(nameLength);
            br.ReadBytes(padding);

            Logger.LogToFile("Name: \"{0}\" of length {1}, padding of {2}", cnt.Name, nameLength, padding);

            if (br.ReadByte() == 12)
            {
                Logger.LogToFile("This is a \"12\" file");
                br.ReadByte();
            }

            Logger.LogToFile("This is usually 0: {0}", br.ReadSingle());

            cnt.transform = new Matrix3D(
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                            );

            string section = br.ReadString(4);
            switch (section)
            {
                case "LITg":
                    int lightType = (int)br.ReadUInt32();

                    switch (lightType)
                    {
                        case 2: // Bounding box?
                            Logger.LogToFile("Light type: {0}", lightType);

                            for (int i = 0; i < 26; i++)
                            {
                                Logger.LogToFile("{0}] {1}", i, br.ReadSingle());
                            }

                            break;

                        case 3:
                            Logger.LogToFile("Light type: {0}", lightType);
                            break;

                        default:
                            Logger.LogToFile("Unknown light type!  I've never seen a light like {0} before", lightType);
                            break;
                    }

                    nameLength = (int)br.ReadUInt32();
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    string lightName = br.ReadString(nameLength);
                    br.ReadBytes(padding);

                    Logger.LogToFile("LITg: \"{0}\" of length {1}, padding of {2}", lightName, nameLength, padding);
                    break;

                case "EMT2":
                    Logger.LogToFile("EMT2, skipping 650 bytes");
                    br.ReadBytes(650);
                    break;

                case "MODL":
                case "SKIN":
                    nameLength = (int)br.ReadUInt32();
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    cnt.modelName = br.ReadString(nameLength);
                    br.ReadBytes(padding);

                    Logger.LogToFile("{0}: \"{1}\" of length {2}, padding of {3}", section, cnt.modelName, nameLength, padding);
                    break;

                case "VFXI":
                    nameLength = (int)br.ReadUInt32();

                    string effectName = br.ReadString(nameLength);

                    Logger.LogToFile("VXFI: \"{0}\" of length {1}, padding of {2}", effectName, nameLength, 0);
                    break;

                case "NULL":
                    break;

                default:
                    Logger.LogToFile("Unknown section \"{0}\"; Aborting", section);
                    return null;
            }

            int childNodes = (int)br.ReadUInt32();

            for (int i = 0; i < childNodes; i++)
            {
                Logger.LogToFile("Loading child {0} of {1}", (i + 1), childNodes);
                cnt.childNodes.Add(Load(br));
            }

            br.ReadUInt32();    // Terminator

            return cnt;
        }
    }
}
