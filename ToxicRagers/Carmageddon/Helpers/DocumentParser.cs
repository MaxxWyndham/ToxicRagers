using System;
using System.IO;
using System.Text;

namespace ToxicRagers.Carmageddon.Helpers
{
    public class DocumentParser : IDisposable
    {
        BinaryReader br;

        //using (var sr = new BinaryReader(File.OpenRead(@"D:\Carmageddon Installations\Carma\DATA\TEXT.TXT"))) 
        //{
        //    while (sr.BaseStream.Position != sr.BaseStream.Length)
        //    {
        //        IWantToFiddle(ReadLine(sr, 256));
        //    }
        //}

        public DocumentParser(string path)
        {
            br = new BinaryReader(new FileStream(path, FileMode.Open), Encoding.ASCII);
        }

        private byte[] ReadLine(BinaryReader br, int buffSize)
        {
            var b = new byte[buffSize];
            int i = 0;

            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                byte c = br.ReadByte();

                if (c == '\r') { continue; }
                if (c == '\n') { break; }

                b[i++] = c;
            }

            var r = new byte[i];
            Array.Copy(b, r, i);

            return r;
        }

        private void IWantToFiddle(byte[] s)
        {
            if (s[0] != '@') { return; }

            byte[] d = new byte[s.Length - 1];

            var seeds = new byte[] { 
                0x6C, 0x1B, 0x99, 0x5F, 
                0xB9, 0xCD, 0x5F, 0x13,
                0xCB, 0x04, 0x20, 0x0E,
                0x5E, 0x1C, 0xA1, 0x0E
            };

            Array.Copy(s, 1, d, 0, d.Length);
            int len = d.Length;
            int seed = len % 16;

            for (int i = 0; i < len; i++)
            {
                if (d[i] == '\t') { d[i] = 159; }

                byte c = (byte)(d[i] - 32);

                if ((c & 0x80) != 0x80)
                {
                    d[i] = (byte)((c ^ (seeds[seed] & 127)) + 32);
                }
                seed = (seed + 7) % 16;

                if (d[i] == 159) { d[i] = 9; }
            }

            Console.WriteLine("{0} becomes {1}", System.Text.Encoding.ASCII.GetString(s), System.Text.Encoding.ASCII.GetString(d));

            //var freqs = File.ReadAllBytes(@"D:\Carmageddon Installations\Carma\DATA\TEXT.TXT")
            //        .OrderBy(c => c)
            //        .GroupBy(c => c)
            //        .ToDictionary(g => (byte)g.Key, g => g.Count());

            //foreach (var e in freqs)
            //{
            //    Console.WriteLine("{0}\t{1}\t{2}", e.Key, (char)e.Key, e.Value);
            //}
        }

        public void Dispose()
        {
            br.Close();
        }
    }
}
