using System;
using System.Drawing;

namespace ToxicRagers.Helpers
{
    public static class ColourHelper
    {
        public static Color R8G8B8ToColour(int i)
        {
            int a = 0;
            int r = (i & 0xFF0000) >> 16;
            int g = (i & 0xFF00) >> 8;
            int b = (i & 0xFF);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
