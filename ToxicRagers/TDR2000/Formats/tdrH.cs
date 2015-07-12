using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class H
    {
        Dictionary<int, string> definitions;

        public Dictionary<int, string> Definitions { get { return definitions; } }

        public H()
        {
            definitions = new Dictionary<int, string>();
        }

        public static H Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            H h = new H();

            string[] lines;

            using (var sr = new StreamReader(fi.OpenRead())) { lines = sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
            lines = lines.Select(l => l.Trim()).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#define"))
                {
                    var parts = lines[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    h.definitions[int.Parse(parts[2])] = parts[1];
                }
            }

            return h;
        }
    }
}
