namespace ToxicRagers.Helpers
{
    public class Plane
    {
        public Vector3 Point;
        public Vector3 Normal;
        public float Distance;

        public Plane(Vector3 n, float d)
        {
            Point = Vector3.Zero;
            Normal = n;
            Distance = d;
        }

        public Plane(Vector3 p, Vector3 n, float d)
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