using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.Stainless.Formats;

namespace ToxicRagers.MaxTools
{
    public class MaxMDLProcessor
    {
        public MDL[] LoadMDLs(String[] files)
        {
            ConcurrentBag<MDL> output = new ConcurrentBag<MDL>();

            Parallel.For(0, files.Length, (i) => {
                output.Add(MDL.Load(files[i]));
            });

            return output.ToArray();
        }
    }
}
