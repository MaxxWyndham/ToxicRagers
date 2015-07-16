using System;
using System.Collections.Generic;
using System.Linq;

namespace ToxicRagers.Compression.Huffman
{
    public class Tree
    {
        List<Node> nodes = new List<Node>();
        Node root;

        FrequencyTable frequencies = new FrequencyTable();
        
        int bitCount = 0;
        int leafCount = 0;

        public FrequencyTable FrequencyTable
        {
            get { return frequencies; }
            set { frequencies = value; }
        }

        public int LeafCount
        {
            get { return (leafCount * 2) + 1; }
        }

        public void BuildTree(byte[] source = null)
        {
            nodes.Clear();

            if (source != null) { frequencies.Import(source); }

            foreach (KeyValuePair<byte, int> symbol in frequencies.Frequencies)
            {
                nodes.Add(new Node { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ThenBy(node => node.Symbol).ToList();

                if (orderedNodes.Count >= 2)
                {
                    List<Node> takenNodes = orderedNodes.Take(2).ToList();

                    Node parent = new Node
                    {
                        Frequency = takenNodes[0].Frequency + takenNodes[1].Frequency,
                        Left = takenNodes[0],
                        Right = takenNodes[1]
                    };

                    nodes.Remove(takenNodes[0]);
                    nodes.Remove(takenNodes[1]);
                    nodes.Add(parent);

                    leafCount++;
                }
            }

            root = nodes.FirstOrDefault();
        }

        public byte[] ToByteArray() {
            List<bool> encodedDictionary = new List<bool>();
            processNode(root, encodedDictionary);
            return boolListToByteArray(encodedDictionary);
        }

        private void processNode(Node node, List<bool> data)
        {
            if (node.Left == null && node.Right == null)
            {
                if (data.Count == 0) { data.Add(true); data.Add(false); }
                data.Add(true); data.Add(true);

                byte mask = 0x80;

                for (int i = 0; i < 8; i++)
                {
                    data.Add((node.Symbol & mask) == mask);
                    mask >>= 1;
                }

                data.Add(false); data.Add(false);
            }
            else
            {
                data.Add(true); data.Add(false);

                if (node.Left != null) { processNode(node.Left, data); }
                if (node.Right != null) { processNode(node.Right, data); }
            }
        }

        public byte[] Encode(byte[] source)
        {
            List<bool> encodedSource = new List<bool>();

            Dictionary<byte, List<bool>> tree = new Dictionary<byte, List<bool>>();

            for (int i = 0; i < 256; i++)
            {
                var list = root.Traverse((byte)i, new List<bool>());
                if (list != null) { tree.Add((byte)i, list); }
            }

            encodedSource.AddRange(
                source.SelectMany(
                    character => tree[character]
                ).ToList()
            );

            bitCount = encodedSource.Count;

            return boolListToByteArray(encodedSource);
        }

        private static byte[] boolListToByteArray(List<bool> list)
        {
            byte[] bytes = new byte[list.Count / 8 + (list.Count % 8 == 0 ? 0 : 1)];

            int b = 0;
            int j = 7;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i]) { bytes[b] |= (byte)(1 << j); }

                j--;

                if (j < 0)
                {
                    j = 7;
                    b++;
                }
            }

            return bytes;
        }
    }
}
