using System;
using System.Drawing;

namespace ToxicRagers.Helpers
{
    public class Colour
    {
        public float R { get; }

        public float G { get; }

        public float B { get; }

        public float A { get; }

        public float H { get; }

        public float S { get; }

        public float L { get; }

        public static Colour Black => new Colour(0, 0, 0, 1);

        public static Colour White => new Colour(1, 1, 1, 1);

        public Colour(float r, float g, float b, float a)
        {
            if (r > 1f) { throw new ArgumentOutOfRangeException("r", "r is expected in the 0.0 to 1.0f range"); }
            if (g > 1f) { throw new ArgumentOutOfRangeException("g", "g is expected in the 0.0 to 1.0f range"); }
            if (b > 1f) { throw new ArgumentOutOfRangeException("b", "b is expected in the 0.0 to 1.0f range"); }
            if (a > 1f) { throw new ArgumentOutOfRangeException("a", "a is expected in the 0.0 to 1.0f range"); }

            R = r;
            G = g;
            B = b;
            A = a;

            //float min = Math.Min(r, Math.Min(g, b));
            //float max = Math.Max(r, Math.Max(g, b));
            //string maxChannel = max == r ? "R" : max == g ? "G" : "B";

            //L = (min + max) / 2f;

            //if (min != max)
            //{
            //    if (L <= 0.5f)
            //    {
            //        S = (max - min) / (max + min);
            //    }
            //    else
            //    {
            //        S = (max - min) / (2f - max - min);
            //    }

            //    switch (maxChannel)
            //    {
            //        case "R":
            //            H = (g - b) / (max - min) + (g < b ? 6 : 0);
            //            break;

            //        case "G":
            //            H = (b - r) / (max - min) + 2;
            //            break;

            //        case "B":
            //            H = (r - g) / (max - min) + 4;
            //            break;
            //    }

            //    //H *= 60;
            //}
        }

        public static Colour FromArgb(float a, float r, float g, float b)
        {
            return new Colour(r, g, b, a);
        }

        public static Colour FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Colour(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static Colour FromRgb(byte r, byte g, byte b)
        {
            return new Colour(r / 255.0f, g / 255.0f, b / 255.0f, 1);
        }

        public static Colour FromColor(Color c)
        {
            return new Colour(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        }

        public static Colour Random()
        {
            KnownColor[] knownColourNames = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            Color c = Color.FromKnownColor(knownColourNames[ToxicRagers.Random.Next(knownColourNames.Length)]);

            return Colour.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}
