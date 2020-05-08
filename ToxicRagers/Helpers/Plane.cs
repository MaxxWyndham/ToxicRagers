namespace ToxicRagers.Helpers
{
    public class Plane
    {
        public Vector3 Point;
        public Vector3 Normal;
        public float Distance;

        public Plane() { }

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

        public static Plane FromPoints(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Plane p = new Plane();

            float a1 = p1.X - p0.X;
            float b1 = p1.Y - p0.Y;
            float c1 = p1.Z - p0.Z;
            float a2 = p2.X - p0.X;
            float b2 = p2.Y - p0.Y;
            float c2 = p2.Z - p0.Z;

            p.Normal = new Vector3(b1 * c2 - b2 * c1,
                                   a2 * c1 - a1 * c2,
                                   a1 * b2 - b1 * a2);
            p.Distance = (-p.Normal.X * p0.X - p.Normal.Y * p0.Y - p.Normal.Z * p0.Z);

            return p;
        }

        public float SignedDistToPoint(Vector3 p)
        {
            return (Vector3.Dot(Normal, p) - Distance) / Vector3.Dot(Normal, Normal);
        }

        public override string ToString()
        {
            return $"{{P: {Point} N: {Normal} D:{Distance}}}";
        }
    }
}