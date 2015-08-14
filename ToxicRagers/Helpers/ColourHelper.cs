using System;
using System.Drawing;

namespace ToxicRagers.Helpers
{
    public static class ColourHelper
    {
        public static Color R8G8B8ToColour(int i)
        {
            int a = 255;
            int r = (i & 0xFF0000) >> 16;
            int g = (i & 0xFF00) >> 8;
            int b = (i & 0xFF);

            return Color.FromArgb(a, r, g, b);
        }

        public static Color R5G6B5ToColour(int i)
        {
            int a = 255 << 24;
            int r = (i & 0xF800) << 8;
            int g = (i & 0x7E0) << 5;
            int b = (i & 0x1F) << 3;

            return Color.FromArgb(r | g | b | a);
        }

        public static Color A4R4G4B4ToColour(int i)
        {
            int a = (i & 0xF000) >> 12;
            int r = (i & 0xF00) >> 8;
            int g = (i & 0xF0) >> 4;
            int b = (i & 0xF);

            return Color.FromArgb(a * 17, r * 17, g * 17, b * 17);
        }
    }
}
