using System;
using ToxicRagers.Carmageddon2.Formats;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class c2Map
    {
        public void Load(string path)
        {
            c2MapTXT.Load(path, this);
        }
    }
}
