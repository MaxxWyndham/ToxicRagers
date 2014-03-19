using System;

namespace ToxicRagers.Helpers
{
    public class Plane
    {
        public Vector3 Point;
        public Vector3 Normal;
        public Single Distance;

        public Plane(Vector3 n, Single d)
        {
            Point = Vector3.Zero;
            Normal = n;
            Distance = d;
        }

        public Plane(Vector3 p, Vector3 n, Single d)
        {
            Point = p;
            Normal = n;
            Distance = d;
        }

        public override string ToString()
        {
            return "{P: " + Point + " N: " + Normal + " D:" + Distance + "}";
        }
    }
}
