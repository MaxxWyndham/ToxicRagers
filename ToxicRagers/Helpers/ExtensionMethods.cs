using System;
using System.IO;

namespace ToxicRagers.Helpers
{
    public static class ExtensionMethods
    {
        public static string ReadString(this BinaryReader br, int length)
        {
            return new string(br.ReadChars(length));
        }
    }
}
