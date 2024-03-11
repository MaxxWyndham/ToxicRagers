using System;

namespace ToxicRagers.Helpers
{
    public class Matrix3D
    {
        public float M11;
        public float M12;
        public float M13;
        public float M21;
        public float M22;
        public float M23;
        public float M31;
        public float M32;
        public float M33;
        public float M41;
        public float M42;
        public float M43;

        public static Matrix3D Identity => new Matrix3D(1.0f, 0, 0, 0, 1.0f, 0, 0, 0, 1.0f, 0, 0, 0);

        public Matrix3D(float M11, float M12, float M13, float M21, float M22, float M23, float M31, float M32, float M33, float M41, float M42, float M43)
        {
            this.M11 = M11;
            this.M12 = M12;
            this.M13 = M13;
            this.M21 = M21;
            this.M22 = M22;
            this.M23 = M23;
            this.M31 = M31;
            this.M32 = M32;
            this.M33 = M33;
            this.M41 = M41;
            this.M42 = M42;
            this.M43 = M43;
        }

        public Matrix3D(Vector3 Position)
        {
            M11 = 1.0f;
            M12 = 0;
            M13 = 0;
            M21 = 0;
            M22 = 1.0f;
            M23 = 0;
            M31 = 0;
            M32 = 0;
            M33 = 1.0f;
            M41 = Position.X;
            M42 = Position.Y;
            M43 = Position.Z;
        }

        public Matrix3D(Vector3 row1, Vector3 row2, Vector3 row3, Vector3 row4)
        {
            M11 = row1.X;
            M12 = row1.Y;
            M13 = row1.Z;
            M21 = row2.X;
            M22 = row2.Y;
            M23 = row2.Z;
            M31 = row3.X;
            M32 = row3.Y;
            M33 = row3.Z;
            M41 = row4.X;
            M42 = row4.Y;
            M43 = row4.Z;
        }

        static float toRads = (float)Math.PI / 180;

        public static Matrix3D CreateRotationX(float Degrees)
        {
            Degrees *= toRads;

            Matrix3D m = Matrix3D.Identity;
            m.M22 = (float)Math.Cos(Degrees);
            m.M23 = (float)Math.Sin(Degrees);
            m.M32 = -(float)Math.Sin(Degrees);
            m.M33 = (float)Math.Cos(Degrees);
            return m;
        }

        public static Matrix3D CreateRotationY(float Degrees)
        {
            Degrees *= toRads;

            Matrix3D m = Matrix3D.Identity;
            m.M11 = (float)Math.Cos(Degrees);
            m.M13 = -(float)Math.Sin(Degrees);
            m.M31 = (float)Math.Sin(Degrees);
            m.M33 = (float)Math.Cos(Degrees);
            return m;
        }

        public static Matrix3D CreateRotationZ(float Degrees)
        {
            Degrees *= toRads;

            Matrix3D m = Matrix3D.Identity;
            m.M11 = (float)Math.Cos(Degrees);
            m.M12 = (float)Math.Sin(Degrees);
            m.M21 = -(float)Math.Sin(Degrees);
            m.M22 = (float)Math.Cos(Degrees);
            return m;
        }

        public static Matrix3D CreateScale(float scale)
        {
            return CreateScale(scale, scale, scale);
        }

        public static Matrix3D CreateScale(float x, float y, float z)
        {
            Matrix3D m = Matrix3D.Identity;
            m.M11 = x;
            m.M22 = y;
            m.M33 = z;
            return m;
        }

        public Vector3 Position
        {
            get => new Vector3(M41, M42, M43);
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }

        public float Scale
        {
            set
            {
                M11 = value;
                M22 = value;
                M33 = value;
            }
        }

        public static Matrix3D operator *(Matrix3D x, Matrix3D y)
        {
            Vector3 p = x.Position + y.Position;

            return new Matrix3D(
                (x.M11 * y.M11) + (x.M12 * y.M21) + (x.M13 * y.M31), (x.M11 * y.M12) + (x.M12 * y.M22) + (x.M13 * y.M32), (x.M11 * y.M13) + (x.M12 * y.M23) + (x.M13 * y.M33),
                (x.M21 * y.M11) + (x.M22 * y.M21) + (x.M23 * y.M31), (x.M21 * y.M12) + (x.M22 * y.M22) + (x.M23 * y.M32), (x.M21 * y.M13) + (x.M22 * y.M23) + (x.M23 * y.M33),
                (x.M31 * y.M11) + (x.M32 * y.M21) + (x.M33 * y.M31), (x.M31 * y.M12) + (x.M32 * y.M22) + (x.M33 * y.M32), (x.M31 * y.M13) + (x.M32 * y.M23) + (x.M33 * y.M33),
                p.X, p.Y, p.Z
            );
        }

        public static explicit operator Matrix3D(Matrix4D m)
        {
            return new Matrix3D(m.M11, m.M12, m.M13, m.M21, m.M22, m.M23, m.M31, m.M32, m.M33, m.M41, m.M42, m.M43);
        }

        public override bool Equals(object obj)
        {
            Matrix3D other = (Matrix3D)obj;

            if (other == null) { return false; }

            return M11 == other.M11 && M12 == other.M12 && M13 == other.M13 &&
                   M21 == other.M21 && M22 == other.M22 && M23 == other.M23 &&
                   M31 == other.M31 && M32 == other.M32 && M33 == other.M33 &&
                   M41 == other.M41 && M42 == other.M42 && M43 == other.M43;
        }

        public override string ToString()
        {
            return "{ {M11:" + M11 + " M12:" + M12 + " M13:" + M13 + "} {M21:" + M21 + " M22:" + M22 + " M23:" + M23 + "} {M31:" + M31 + " M32:" + M32 + " M33:" + M33 + "} {M41:" + M41 + " M42:" + M42 + " M43:" + M43 + "} }";
        }
    }
}