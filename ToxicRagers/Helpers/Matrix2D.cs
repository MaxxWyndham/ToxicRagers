using System;

namespace ToxicRagers.Helpers
{
    public class Matrix2D : IEquatable<Matrix2D>
    {
        public float M11;
        public float M12;
        public float M21;
        public float M22;
        public float M31;
        public float M32;

        public static Matrix2D Identity => new Matrix2D(1.0f, 0, 0, 1.0f, 0, 0);

        public Matrix2D(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            M31 = m31;
            M32 = m32;
        }

        public override string ToString()
        {
            return "{ {M11:" + M11 + " M12:" + M12 + "} {M21:" + M21 + " M22:" + M22 + "} {M31:" + M31 + " M32:" + M32 + "} }";
        }

        public static bool operator ==(Matrix2D x, Matrix2D y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Matrix2D x, Matrix2D y)
        {
            return !x.Equals(y);
        }

        public bool Equals(Matrix2D other)
        {
            return (M11 == other.M11 && M12 == other.M12 && M21 == other.M21 && M22 == other.M22 && M31 == other.M31 && M32 == other.M32);
        }

        public override bool Equals(object obj)
        {
            Matrix2D m = obj as Matrix2D;

            if (m == null) { return false; }
            return (this == m);
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() ^
                   M12.GetHashCode() ^
                   M21.GetHashCode() ^
                   M22.GetHashCode() ^
                   M31.GetHashCode() ^
                   M32.GetHashCode();
        }
    }
}