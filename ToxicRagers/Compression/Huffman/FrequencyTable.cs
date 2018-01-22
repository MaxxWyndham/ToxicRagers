using System.Collections.Generic;
using System.Linq;

namespace ToxicRagers.Compression.Huffman
{
    public class FrequencyTable
    {
        Dictionary<byte, int> frequencyTable = new Dictionary<byte, int>();

        public Dictionary<byte, int> Frequencies => frequencyTable;

        public void Clear()
        {
            frequencyTable.Clear();
        }

        public void Import(byte[] data)
        {
            data.GroupBy(b => b)
                .ToDictionary(g => g.Key, g => g.Count())
                .ToList()
                .ForEach(x => frequencyTable[x.Key] = x.Value);
        }

        public void Add(byte value, int count)
        {
            if (!frequencyTable.ContainsKey(value))
            {
                frequencyTable.Add(value, count);
            }
            else
            {
                frequencyTable[value] += count;
            }
        }
    }
}