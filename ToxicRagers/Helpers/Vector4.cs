using System;
using System.ComponentModel;

namespace ToxicRagers.Helpers
{
    [TypeConverter(typeof(Vector4Converter))]
    public class Vector4 : IEquatable<Vector4>
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;

                    case 1:
                        return Y;

                    case 2:
                        return Z;

                    case 3:
                        return W;

                    default:
                        throw new InvalidOperationException();
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;

                    case 1:
                        Y = value;
                        break;

                    case 2:
                        Z = value;
                        break;

                    case 3:
                        W = value;
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public Vector4() { }

        public Vector4(float n)
        {
            X = n;
            Y = n;
            Z = n;
            W = n;
        }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4 Zero => new Vector4(0, 0, 0, 0);

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

        public Vector4 SplatX() { return new Vector4(X); }

        public Vector4 SplatY() { return new Vector4(Y); }

        public Vector4 SplatZ() { return new Vector4(Z); }

        public Vector4 SplatW() { return new Vector4(W); }

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
            return left.X < right.X ||
                   left.Y < right.Y ||
                   left.Z < right.Z ||
                   left.W < right.W;
        }

        public static Vector4 Transform(Vector4 v, Matrix4D m)
        {
            Transform(ref v, ref m, out Vector4 result);

            return result;
        }

        public static void Transform(ref Vector4 v, ref Matrix4D m, out Vector4 result)
        {
            result = new Vector4(
                (v.X * m.M11) + (v.Y * m.M21) + (v.Z * m.M31) + (v.W * m.M41),
                (v.X * m.M12) + (v.Y * m.M22) + (v.Z * m.M32) + (v.W * m.M42),
                (v.X * m.M13) + (v.Y * m.M23) + (v.Z * m.M33) + (v.W * m.M43),
                (v.X * m.M14) + (v.Y * m.M24) + (v.Z * m.M34) + (v.W * m.M44));
        }

        public static Vector4 operator +(Vector4 x, Vector4 y)
        {
            return new Vector4(x.X + y.X, x.Y + y.Y, x.Z + y.Z, x.W + y.W);
        }

        public static Vector4 operator -(Vector4 x, Vector4 y)
        {
            return new Vector4(x.X - y.X, x.Y - y.Y, x.Z - y.Z, x.W - y.W);
        }

        public static Vector4 operator *(Vector4 x, Vector4 y)
        {
            return new Vector4(x.X * y.X, x.Y * y.Y, x.Z * y.Z, x.W * y.W);
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
            string[] s = v.Replace(" ", "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return new Vector4(s[0].ToSingle(), s[1].ToSingle(), s[2].ToSingle(), s[3].ToSingle());
        }

        public override string ToString()
        {
            return string.Format("{{X: {0,15:F9} Y: {1,15:F9} Z: {2,15:F9} W: {3,15:F9} }}", X, Y, Z, W);
        }

        public bool Equals(Vector4 other)
        {
            return (X == other.X && Y == other.Y && Z == other.Z && W == other.W);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static explicit operator Vector4(Vector3 v)
        {
            return new Vector4(v.X, v.Y, v.Z, 0);
        }
    }

    public class Vector4Converter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Vector3)) { return true; }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Vector4)
            {
                Vector4 v = value as Vector4;

                return v.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}