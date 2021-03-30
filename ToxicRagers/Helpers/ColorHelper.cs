using System.Drawing;

namespace ToxicRagers.Helpers
{
    public static class ColorHelper
    {
        public enum ChannelOrder
        {
            BGR,
            RGB
        }

        public static Color R8G8B8ToColor(int i)
        {
            int a = 255;
            int r = (i & 0xFF0000) >> 16;
            int g = (i & 0xFF00) >> 8;
            int b = (i & 0xFF);

            return Color.FromArgb(a, r, g, b);
        }

        public static Color A8B8G8R8ToColor(uint i)
        {
            int a = (int)(i >> 24);
            int b = (int)(i & 0xFF0000) >> 16;
            int g = (int)(i & 0xFF00) >> 8;
            int r = (int)(i & 0xFF) >> 0;

            return Color.FromArgb(a, r, g, b);
        }

        public static Color R5G6B5ToColor(int i)
        {
            int a = 255 << 24;
            int r = (i & 0xF800) << 8;
            int g = (i & 0x7E0) << 5;
            int b = (i & 0x1F) << 3;

            return Color.FromArgb(r | g | b | a);
        }

        public static Color A4R4G4B4ToColor(int i)
        {
            int a = (i & 0xF000) >> 12;
            int r = (i & 0xF00) >> 8;
            int g = (i & 0xF0) >> 4;
            int b = (i & 0xF);

            return Color.FromArgb(a * 17, r * 17, g * 17, b * 17);
        }

        public static Color R4G4B4A4ToColor(int i)
        {
            int r = ((i & 0xF000) >> 12) << 4;
            int g = ((i & 0xF00) >> 8) << 4;
            int b = ((i & 0xF0) >> 4) << 4;
            int a = ((i & 0xF) >> 0) << 4;

            return Color.FromArgb(a, r, g, b);
        }

        public static Color R5G5B6ToColor(int i)
        {
            int r = ((i & 0xF800) >> 11) << 3;
            int g = ((i & 0x7C0) >> 6) << 3;
            int b = (i & 0x3F) << 2;
            int a = 255;

            return Color.FromArgb(a, r, g, b);
        }

        public static Color PSX5551ToColor(int pixel, ChannelOrder order = ChannelOrder.BGR, bool useTransparency = false)
        {
            int alpha = 255;
            int special = (pixel & 0x8000) >> 15;
            int r = ((pixel & 0x7c00) >> 10) << 3;
            int g = ((pixel & 0x03e0) >> 5) << 3;
            int b = (pixel & 0x001f) << 3;

            if (useTransparency && r == 0 && g == 0 && b == 0) { alpha = 0; }

            if (order == ChannelOrder.BGR)
            {
                int x = r;
                r = b;
                b = x;
            }

            return Color.FromArgb(alpha, r, g, b);
        }
    }
}