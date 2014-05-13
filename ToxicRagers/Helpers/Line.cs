using System;

namespace ToxicRagers.Helpers
{
    public class Line
    {
        public Vector3 Point0;
        public Vector3 Point1;

        public Line()
        {
            Point0 = Vector3.Zero;
            Point1 = Vector3.Zero;
        }

        public Line(Vector3 p0, Vector3 p1)
        {
            Point0 = p0;
            Point1 = p1;
        }

        public Vector3 Direction
        {
            get { return (Point1 - Point0).Normalised; }
        }

        public override string ToString()
        {
            return "{P0: " + Point0.ToString() + " P1: " + Point1.ToString() + "}";
        }
    }
}
