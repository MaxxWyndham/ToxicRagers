using System.Collections.Generic;
using System.Diagnostics;

namespace ToxicRagers.Compression.Huffman
{
    [DebuggerDisplay("Frequency {frequency} Symbol {symbol}")]
    public class Node
    {
        Node left = null;
        Node right = null;
        byte symbol;
        int frequency;

        public byte Symbol
        {
            get => symbol;
            set => symbol = value;
        }

        public int Frequency
        {
            get => frequency;
            set => frequency = value;
        }

        public Node Left
        {
            get => left;
            set => left = value;
        }

        public Node Right
        {
            get => right;
            set => right = value;
        }

        public List<bool> Traverse(byte v, List<bool> data)
        {
            if (left == null && right == null)
            {
                return (symbol == v ? (data.Count == 0 ? new List<bool> { false } : data) : null);
            }
            else
            {
                List<bool> l = null;
                List<bool> r = null;

                if (left != null)
                {
                    List<bool> lbranch = new List<bool>(data) { false };
                    l = left.Traverse(v, lbranch);
                }

                if (right != null)
                {
                    List<bool> rbranch = new List<bool>(data) { true };
                    r = right.Traverse(v, rbranch);
                }

                return (l ?? r);
            }
        }
    }
}