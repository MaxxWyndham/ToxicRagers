using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ToxicRagers.Helpers
{
    public static class ExtensionMethods
    {
        public static void Write(this BinaryWriter br, Vector2 v)
        {
            br.Write(v.X);
            br.Write(v.Y);
        }

        public static void Write(this BinaryWriter br, Vector3 v)
        {
            br.Write(v.X);
            br.Write(v.Y);
            br.Write(v.Z);
        }

        public static void Write(this BinaryWriter br, Vector4 v)
        {
            br.Write(v.X);
            br.Write(v.Y);
            br.Write(v.Z);
            br.Write(v.W);
        }

        // Rather specific versions of Read and WriteString here.  Possibly move inside cFBX.cs
        public static string ReadPropertyString(this BinaryReader br, int length)
        {
            byte[] c = br.ReadBytes(length);
            int l = length;

            for (int i = 0; i < length - 1; i++)
            {
                if (c[i + 0] == 0 && c[i + 1] == 1)
                {
                    c[i + 0] = 58; // :
                    c[i + 1] = 58; // :
                    break;
                }
            }

            return Encoding.UTF8.GetString(c);
        }

        public static void WritePropertyString(this BinaryWriter bw, string s)
        {
            char[] c = s.ToCharArray();
            int length = c.Length;

            for (int i = 0; i < length - 1; i++)
            {
                if (c[i + 0] == ':' && c[i + 1] == ':')
                {
                    c[i + 0] = (char)0;
                    c[i + 1] = (char)1;
                    break;
                }
            }

            bw.Write(c);
        }

        public static string ReadString(this BinaryReader br, int length)
        {
            if (length == 0) { return null; }

            char[] c = br.ReadChars(length);
            int l = length;

            for (int i = 0; i < length; i++)
            {
                if (c[i] == 0)
                {
                    l = i;
                    break;
                }
            }

            return new string(c, 0, l);
        }

        public static string[] ReadStrings(this BinaryReader br, int count)
        {
            string[] r = new string[count];
            int i = 0;

            while (i < count)
            {
                char c = br.ReadChar();
                if (c == 0)
                {
                    i++;
                }
                else
                {
                    r[i] += c;
                }
            }

            return r;
        }

        public static string ReadNullTerminatedString(this BinaryReader br)
        {
            string r = "";
            char c = (char)0;

            do
            {
                c = br.ReadChar();
                if (c > 0) { r += c; }
            } while (c > 0);

            return r;
        }

        public static void WriteString(this BinaryWriter bw, string s)
        {
            bw.Write(s.ToCharArray());
        }

        public static void WriteBytes(this BinaryWriter bw, int count, byte b = 0)
        {
            byte[] buff = new byte[count];
            if (b != 0) { for (int i = 0; i < buff.Length; i++) { buff[i] = b; } }

            bw.Write(buff);
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static float ToSingle(this string s)
        {
            return float.Parse(s, ToxicRagers.Culture);
        }

        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static T ToEnumWithDefault<T>(this string value, T defaultValue, bool ignoreCase = true) where T : struct
        {
            if (Enum.TryParse<T>(value, ignoreCase, out T o))
            {
                return o;
            }
            else
            {
                return defaultValue;
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
                if (startIndex == -1) { break; }

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }

        public static string ToFormattedString(this byte[] ba)
        {
            int count = ba.Length;
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int i = 0; i < count; i++) { hex.AppendFormat("0x{0:x2}{1}", ba[i], (i + 1 < count ? ", " : "")); }
            return hex.ToString();
        }

        public static string ToFormattedString<T>(this T a)
            where T : IEnumerable
        {
            return string.Join(",", a);
        }

        public static string ToHex(this byte[] ba)
        {
            int count = ba.Length;
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int i = 0; i < count; i++) { hex.AppendFormat("{0:x2}{1}", ba[i], (i + 1 < count ? " " : "")); }
            return hex.ToString();
        }

        public static IEnumerable<T> Every<T>(this List<T> list, int step, int start = 0)
        {
            for (int i = start; i < list.Count; i += step)
            {
                yield return list[i];
            }
        }

        public static Bitmap Resize(this Bitmap b, int width, int height)
        {
            if (b.PixelFormat == PixelFormat.Format8bppIndexed) { return b; }

            Graphics g = Graphics.FromImage(b);
            Bitmap d = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(d))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(b, new Rectangle(0, 0, width, height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return d;
        }

        public static string Indent(this string lua)
        {
            string[] lines = lua.Split('\r', '\n').Where(s => s != string.Empty).ToArray();
            string indent = "";

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].EndsWith("}") || lines[i].EndsWith("},")) { indent = indent.Substring(0, indent.Length - 2); }

                lines[i] = $"{indent}{lines[i]}";

                if (lines[i].EndsWith("{")) { indent += "  "; }
            }

            return string.Join("\r\n", lines);
        }
    }
}