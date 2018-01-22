using System;

namespace ToxicRagers.Helpers
{
    public class Vector4
    {
        private float _x;
        private float _y;
        private float _z;
        private float _w;

        public float X => _x;
        public float Y => _y;
        public float Z => _z;
        public float W => _w;

        public Vector4(float n)
        {
            _x = n;
            _y = n;
            _z = n;
            _w = n;
        }

        public Vector4(float X, float Y, float Z, float W)
        {
            _x = X;
            _y = Y;
            _z = Z;
            _w = W;
        }

        public static Vector4 Min(Vector4 v1, Vector4 v2)
        {
            return new Vector4(
                Math.Min(v1.X, v2.X),
                Math.Min(v1.Y, v2.Y),
                Math.Min(v1.Z, v2.Z),
                Math.Min(v1.W, v2.W)
            );
        }

        public static Vector4 Max(Vector4 v1, Vector4 v2)
        {
            return new Vector4(
                Math.Max(v1.X, v2.X),
                Math.Max(v1.Y, v2.Y),
                Math.Max(v1.Z, v2.Z),
                Math.Max(v1.W, v2.W)
            );
        }

        public static Vector4 Truncate(Vector4 v)
        {
            return new Vector4(
                (float)(v.X > 0.0f ? Math.Floor(v.X) : Math.Ceiling(v.X)),
                (float)(v.Y > 0.0f ? Math.Floor(v.Y) : Math.Ceiling(v.Y)),
                (float)(v.Z > 0.0f ? Math.Floor(v.Z) : Math.Ceiling(v.Z)),
                (float)(v.W > 0.0f ? Math.Floor(v.W) : Math.Ceiling(v.W))
            );
        }

        public Vector4 SplatX() { return new Vector4(_x); }
        public Vector4 SplatY() { return new Vector4(_y); }
        public Vector4 SplatZ() { return new Vector4(_z); }
        public Vector4 SplatW() { return new Vector4(_w); }

        public static Vector4 Reciprocal(Vector4 v)
        {
            return new Vector4(
                    1.0f / v.X,
                    1.0f / v.Y,
                    1.0f / v.Z,
                    1.0f / v.W
            );
        }

        public static bool CompareAnyLessThan(Vector4 left, Vector4 right)
        {
            return left._x < right._x
                    || left._y < right._y
                    || left._z < right._z
                    || left._w < right._w;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(_x, _y, _z);
        }

        public static Vector4 operator +(Vector4 x, Vector4 y)
        {
            return new Vector4(x._x + y._x, x._y + y._y, x._z + y._z, x._w + y._w);
        }

        public static Vector4 operator -(Vector4 x, Vector4 y)
        {
            return new Vector4(x._x - y.X, x._y - y.Y, x._z - y.Z, x._w - y.W);
        }

        public static Vector4 operator *(Vector4 x, Vector4 y)
        {
            return new Vector4(x._x * y._x, x._y * y._y, x._z * y._z, x._w * y._w);
        }

        public static Vector4 MultiplyAdd(Vector4 a, Vector4 b, Vector4 c)
        {
            return a * b + c;
        }

        public static Vector4 NegativeMultiplySubtract(Vector4 a, Vector4 b, Vector4 c)
        {
            return c - a * b;
        }

        public static Vector4 Parse(string v)
        {
            v = v.Replace(" ", "");
            string[] s = v.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return new Vector4(s[0].ToSingle(), s[1].ToSingle(), s[2].ToSingle(), s[3].ToSingle());
        }

        public override string ToString()
        {
            return string.Format("{{X: {0,15:F9} Y: {1,15:F9} Z: {2,15:F9} W: {3,15:F9} }}", _x, _y, _z, _w);
        }
    }
}