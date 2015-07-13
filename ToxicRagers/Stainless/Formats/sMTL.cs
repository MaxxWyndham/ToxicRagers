using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class MTL : Material
    {
        string name;
        List<string> textureNames = new List<string>();
        Version version;

        public string Name { get { return name; } }
        public List<string> Textures { get { return textureNames; } }

        public static MTL Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            MTL mtl = new MTL();

            mtl.name = Path.GetFileNameWithoutExtension(path);

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                mtl.version = new Version(major, minor);

                Logger.LogToFile(Logger.LogLevel.Debug, "MTL v{0}.{1}", major, minor);

                int textureCount = (int)br.ReadUInt32();
                Logger.LogToFile(Logger.LogLevel.Debug, "Texture count: {0}", textureCount);
                for (int i = 0; i < textureCount; i++)
                {
                    if (mtl.version.Major == 4 && mtl.version.Minor == 7)
                    {
                        mtl.textureNames.Add(br.ReadBytes(16).ToName());
                        br.ReadBytes(16);
                    }
                    else
                    {
                        int nameLength = (int)br.ReadUInt32();
                        int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength;

                        mtl.textureNames.Add(br.ReadString(nameLength));
                        br.ReadBytes(padding);

                        Logger.LogToFile(Logger.LogLevel.Debug, "Added texture \"{0}\" of length {1}, padding of {2}", mtl.textureNames[mtl.textureNames.Count - 1], nameLength, padding);
                    }

                    br.ReadBytes(30);
                }
            }

            return mtl;
        }
    }
}
