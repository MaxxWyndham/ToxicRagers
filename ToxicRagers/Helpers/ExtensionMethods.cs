using System;
using System.Collections.Generic;
using System.IO;

namespace ToxicRagers.Helpers
{
    public static class ExtensionMethods
    {
        public static string ReadString(this BinaryReader br, int length)
        {
            return new string(br.ReadChars(length));
        }

        public static void WriteString(this BinaryWriter bw, string s)
        {
            bw.Write(s.ToCharArray());
        }

        public static string ReadNextLine(this StreamReader sr)
        {
            if (sr.EndOfStream) { return null; }

            string s = sr.ReadLine();

            if (s.IndexOf("/") > -1) { s = s.Substring(0, s.IndexOf("/")).Trim(); } else { s = s.Trim(); }

            if (s.Length == 0)
            {
                return sr.ReadNextLine();
            }
            else
            {
                return s;
            }
        }

        public static void Add(this Dictionary<string, string> d, string[] kvp)
        {
            d[kvp[0]] = string.Join(" ", kvp, 1, kvp.Length - 1);
        }

        public static string ToName(this byte[] b)
        {
            string s = "";

            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] == 0) { break; }
                s += (char)b[i];
            }

            return s;
        }

        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            int startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }
    }
}
