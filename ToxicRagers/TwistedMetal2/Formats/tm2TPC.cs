using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;
using ToxicRagers.PSX.Formats;

namespace ToxicRagers.TwistedMetal2.Formats
{
    public class TPC
    {
        public List<TIM> Textures { get; set; } = new List<TIM>();

        public static TPC Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            TPC tpc = new TPC();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                if (br.ReadByte() != 0x54 || // T
                    br.ReadByte() != 0x43 || // C
                    br.ReadByte() != 0x50 || // P
                    br.ReadByte() != 0x4d || // M
                    br.ReadByte() != 0x43 || // C
                    br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0xf1 ||
                    br.ReadByte() != 0xf1 ||
                    br.ReadByte() != 0x58 ||
                    br.ReadByte() != 0x34)
                {
                    // this can't all be magic?
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid TPC file", path);
                    return null;
                }

                uint textureCount = br.ReadUInt32();

                for (int i = 0; i < textureCount; i++)
                {
                    br.ReadUInt32(); // block size

                    tpc.Textures.Add(TIM.Load(ms, channelOrder: ColorHelper.ChannelOrder.RGB));
                }
            }

            return tpc;
        }
    }
}
