using System;
using System.Drawing;

namespace ToxicRagers.Helpers
{
    public class Colour
    {
        public float R { get; set; }

        public float G { get; set; }

        public float B { get; set; }

        public float A { get; set; }

        public static Colour White => new Colour(1, 1, 1, 1);

        public Colour(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Colour FromArgb(float a, float r, float g, float b)
        {
            return new Colour(r, g, b, a);
        }

        public static Colour Random()
        {
            KnownColor[] knownColourNames = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            Color c = Color.FromKnownColor(knownColourNames[ToxicRagers.Random.Next(knownColourNames.Length)]);

            return new Colour(c.R, c.G, c.B, c.A);
        }
    }
}
