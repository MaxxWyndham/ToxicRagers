using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class MTL
    {
        string name;
        string textureName;

        public string Name { get { return name; } }
        public string Texture { get { return textureName; } }

        public static MTL Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            MTL mtl = new MTL();

            mtl.name = fi.Name.Replace(fi.Extension, "");

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 1 ||
                    br.ReadByte() != 5)
                {
                    Logger.LogToFile("{0} isn't a valid MTL file", Path);
                    return null;
                }

                int textureCount = (int)br.ReadUInt32();
                int nameLength = (int)br.ReadUInt32();
                int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                mtl.textureName = br.ReadString(nameLength);
                br.ReadBytes(padding);

                Logger.LogToFile("Name: \"{0}\" of length {1}, padding of {2}", mtl.Texture, nameLength, padding);
            }

            return mtl;
        }
    }
}
