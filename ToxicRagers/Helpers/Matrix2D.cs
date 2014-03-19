using System;

namespace ToxicRagers.Helpers
{
    public class Matrix2D : IEquatable<Matrix2D>
    {
        public Single M11;
        public Single M12;
        public Single M21;
        public Single M22;
        public Single M31;
        public Single M32;

        public static Matrix2D Identity { get { return new Matrix2D(1.0f, 0, 0, 1.0f, 0, 0); } }

        public Matrix2D(Single m11, Single m12, Single m21, Single m22, Single m31, Single m32)
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
            return (this.M11 == other.M11 && this.M12 == other.M12 && this.M21 == other.M21 && this.M22 == other.M22 && this.M31 == other.M31 && this.M32 == other.M32);
        }
    }
}
