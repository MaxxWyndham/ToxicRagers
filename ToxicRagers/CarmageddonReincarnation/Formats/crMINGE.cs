using System;
using System.Collections.Generic;
using System.IO;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class MINGE
    {
        public enum ModType
        {
            Vehicle,
            Level,
            Wheels
        }

        int mingeVersion = 1;
        Version modVersion = new Version(1, 0);
        ModType modType = ModType.Vehicle;
        string name;
        string author;
        string website;
        List<string> images = new List<string>();
        List<string> requirements = new List<string>();

        public int MingeVersion
        {
            get { return mingeVersion; }
        }

        public Version ModVersion
        {
            get { return modVersion; }
            set { modVersion = value; }
        }

        public ModType Type
        {
            get { return modType; }
            set { modType = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public string Website
        {
            get { return website; }
            set { website = value; }
        }

        public List<string> Images
        {
            get { return images; }
            set { images = value; }
        }

        public List<string> Requirements
        {
            get { return requirements; }
            set { requirements = value; }
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("[MingeVersion]");
                sw.WriteLine(mingeVersion);

                sw.WriteLine();

                sw.WriteLine("[Version]");
                sw.WriteLine(modVersion);

                sw.WriteLine();

                sw.WriteLine("[Type]");
                sw.WriteLine(modType);

                sw.WriteLine();

                if (!string.IsNullOrEmpty(name))
                {
                    sw.WriteLine("[Name]");
                    sw.WriteLine(name);
                    sw.WriteLine();
                }

                if (!string.IsNullOrEmpty(author))
                {
                    sw.WriteLine("[Author]");
                    sw.WriteLine(author);
                    sw.WriteLine();
                }

                if (!string.IsNullOrEmpty(website))
                {
                    sw.WriteLine("[Website]");
                    sw.WriteLine(website);
                    sw.WriteLine();
                }

                if (images.Count > 0)
                {
                    sw.WriteLine("[Images]");
                    foreach (string image in images) { sw.WriteLine(image); }
                    sw.WriteLine();
                }

                if (requirements.Count > 0)
                {
                    sw.WriteLine("[Required]");
                    foreach (string requirement in requirements) { sw.WriteLine(requirement); }
                }
            }
        }
    }
}
