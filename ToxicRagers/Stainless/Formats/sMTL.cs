using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;
using ToxicRagers.CarmageddonReincarnation.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class MTL : Material
    {
        string name;
        List<string> textureNames = new List<string>();
        Version version;

        public string Name { get { return name; } }
        public List<string> Textures { get { return textureNames; } }

        public static MTL Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            MTL mtl = new MTL();

            mtl.name = fi.Name.Replace(fi.Extension, "");

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                mtl.version = new Version(major, minor);

                Logger.LogToFile("MTL v{0}.{1}", major, minor);

                int textureCount = (int)br.ReadUInt32();
                Logger.LogToFile("Texture count: {0}", textureCount);
                for (int i = 0; i < textureCount; i++)
                {
                    int nameLength = (int)br.ReadUInt32();
                    int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                    mtl.textureNames.Add(br.ReadString(nameLength));
                    br.ReadBytes(padding);

                    Logger.LogToFile("Added texture \"{0}\" of length {1}, padding of {2}", mtl.textureNames[mtl.textureNames.Count - 1], nameLength, padding);

                    br.ReadBytes(30);
                }
            }

            return mtl;
        }
    }
}
