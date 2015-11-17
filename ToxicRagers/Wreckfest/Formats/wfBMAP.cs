using System;
using System.IO;

using ToxicRagers.Compression.LZ4;
using ToxicRagers.Helpers;

namespace ToxicRagers.Wreckfest.Formats
{
    public class BMAP
    {
        public static BMAP Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            BMAP bmap = new BMAP();

            var buff = new byte[33554432];
            int size = 0;
            int sizeSoFar = 0;

            using (var bw = new BinaryWriter(new FileStream(@"d:\bmap3.1.raw", FileMode.Create)))
            {
                using (var bfs = new BinaryFileStream(path, FileMode.Open))
                {
                    bfs.Seek(0xC, SeekOrigin.Begin);

                    while (bfs.Position < bfs.Length)
                    {
                        int length = (int)bfs.ReadUInt32();

                        Console.WriteLine("Pass size: {0}  Position: {1}", length, bfs.Position);

                        using (var ms = new MemoryStream(bfs.ReadBytes(length)))
                        using (var lz4 = new LZ4Decompress(ms))
                        {
                            size = lz4.Read(buff, sizeSoFar, buff.Length);
                        }

                        bw.Write(buff, sizeSoFar, size);

                        sizeSoFar += size;
                    }
                }
            }

            return bmap;
        }
    }
}
