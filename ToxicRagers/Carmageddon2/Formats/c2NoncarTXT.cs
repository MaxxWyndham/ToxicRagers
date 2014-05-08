using System;
using ToxicRagers.Carmageddon2.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class Noncar
    {
        public static Noncar Load(string path)
        {
            Noncar noncar = new Noncar();

            using (var doc = new DocumentParser(path))
            {
            }

            return noncar;
        }
    }
}
