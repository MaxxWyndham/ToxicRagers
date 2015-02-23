using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Vigilante82ndOffense.Formats
{
    public class BIN
    {
        List<int> aOffsets = new List<int>();

        public static BIN Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            BIN bin = new BIN();

            using (var br = new BinaryReader(fi.OpenRead()))
            {
                int aCount = (int)br.ReadUInt32();
                int aOffset = (int)br.ReadUInt32();
                int bCount = (int)br.ReadUInt32();
                int bOffset = (int)br.ReadUInt32();
                int textureCount = (int)br.ReadUInt32();
                int textureOffset = (int)br.ReadUInt32();
                
                int cCount = (int)br.ReadUInt32();
                for (int i = 0; i < cCount; i++)
                {
                    br.ReadBytes(36);
                }

                Debug.Assert(br.BaseStream.Position == aOffset);

                for (int i = 0; i < aCount; i++)
                {
                    bin.aOffsets.Add((int)br.ReadUInt32());
                }

                Debug.Assert((int)br.ReadUInt32() + aOffset == bOffset);

                for (int i = 0; i < aCount; i++)
                {
                    br.BaseStream.Seek(aOffset + bin.aOffsets[i], SeekOrigin.Begin);

                    Logger.LogToFile("###### {0,0:D4} ######", i);
                    Logger.LogToFile("U: {0}", br.ReadUInt32());
                    int ux = (int)br.ReadUInt32();
                    Logger.LogToFile("{0}\t\t// Number of entries in second section", ux);
                    Logger.LogToFile("U: {0}", br.ReadUInt32());
                    int v = (int)br.ReadUInt32();
                    Logger.LogToFile("{0}\t\t// Entry count", v);
                    int offset1 = (int)br.ReadUInt32();
                    Logger.LogToFile("{0}\t\t// Offset to first section", offset1);
                    Logger.LogToFile("U: {0}", br.ReadUInt32());
                    Logger.LogToFile("{0}\t// End of first section", br.ReadUInt32());
                    int offset2 = (int)br.ReadUInt32();
                    Logger.LogToFile("{0}\t// Offset to second section", offset2);
                    Logger.LogToFile("U: {0}", br.ReadUInt32());
                    Logger.LogToFile("U: {0}", br.ReadUInt32());
                    Logger.LogToFile("U: {0}", br.ReadSingle());

                    br.BaseStream.Seek(aOffset + bin.aOffsets[i] + offset1, SeekOrigin.Begin);
                    for (int j = 0; j < v; j++)
                    {
                        Logger.LogToFile("\tPosition: X: {0} Y: {1} Z: {2} Normal: X: {3} Y: {4} Z: {5}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }

                    br.BaseStream.Seek(aOffset + bin.aOffsets[i] + offset2, SeekOrigin.Begin);
                    for (int j = 0; j < ux; j++)
                    {
                        Logger.LogToFile("###### {0,0:D4}.{1,0:D4} ######", i, j);
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0}", br.ReadUInt32());
                        Logger.LogToFile("\t{0} {1} {2} {3}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        Logger.LogToFile("\t{0} {1} {2} {3}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        Logger.LogToFile("\t{0} {1} {2} {3}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        Logger.LogToFile("\t{0} {1}", br.ReadSingle(), br.ReadSingle());
                        Logger.LogToFile("\t{0} {1} {2} {3}", br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                        Logger.LogToFile("\t{0} {1} {2} {3}", br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                    }
                }

                br.BaseStream.Seek(bOffset, SeekOrigin.Begin);

                for (int i = 0; i < bCount; i++)
                {
                    br.ReadUInt32();    // bOffset + this value = start of entry
                }

                Debug.Assert((int)br.ReadUInt32() + bOffset == textureOffset);
                br.BaseStream.Seek(textureOffset, SeekOrigin.Begin);

                for (int i = 0; i < textureCount; i++)
                {
                    br.ReadUInt32();    // textureOffset + this value = start of entry
                }

                Debug.Assert((int)br.ReadUInt32() + textureOffset == br.BaseStream.Length);
            }

            return bin;
        }
    }
}
