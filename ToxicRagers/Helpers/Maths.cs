using System;

namespace ToxicRagers.Helpers
{
    public static class Maths
    {
        public const float Pi = 3.1415927f;

        public const float TwoPi = Pi * 2;

        public const float PiOver4 = Pi / 4;

        public static float DegreesToRadians(float degrees)
        {
            const float degToRad = (float)Math.PI / 180.0f;

            return degrees * degToRad;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDeg = 180.0 / Math.PI;

            return radians * radToDeg;
        }
    }
}
