using System;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    public class FBX
    {
        public static FBX Load(string path)
        {
            // All these (int) casts are messy
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            FBX fbx = new FBX();

            using (var br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadString(18) == "Kaydara FBX Binary")
                {
                    Logger.LogToFile("Binary FBX detected, unsupported!");
                    return null;
                }
            }

            using (var sr = new StreamReader(fi.OpenRead())) 
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.StartsWith(";")) { continue; } // Comment
                }
            }

            return fbx;
        }
    }
}
