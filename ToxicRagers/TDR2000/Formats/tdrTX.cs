using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class TX : Material
    {
        string name;
        string texture;
        string normalMap;
        int version;
        int levels;
        int flags;
        int width;
        int height;

        public string Name { get { return name; } }
        public override string FileName { get { return texture; } }

        public static TX Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            TX tx = new TX();

            tx.name = Path.GetFileNameWithoutExtension(path);

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 84 ||
                    br.ReadByte() != 84 ||
                    br.ReadByte() != 69 ||
                    br.ReadByte() != 88)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid TX file", path);
                    return null;
                }

                tx.version = br.ReadInt32();
                tx.levels = br.ReadInt32();
                tx.flags = br.ReadInt32();
                br.ReadByte();
                br.ReadByte();
                br.ReadByte();
                int nmlen = br.ReadByte(); if (nmlen > 0) { tx.normalMap = br.ReadString(nmlen - 1); }
                if (tx.version >= 5) { br.ReadBytes(8); }
                tx.texture = br.ReadString(br.ReadByte() - 1);
                tx.width = br.ReadInt16();
                tx.height = br.ReadInt16();

                tx.texture += string.Format("_{0}x{1}_32", tx.width, tx.height);

                Logger.LogToFile(Logger.LogLevel.Debug, "TX v{0}", tx.version);
            }

            return tx;
        }
    }
}
