using System;

namespace ToxicRagers.Helpers
{
    public class Matrix3D
    {
        public Single M11;
        public Single M12;
        public Single M13;
        public Single M21;
        public Single M22;
        public Single M23;
        public Single M31;
        public Single M32;
        public Single M33;
        public Single M41;
        public Single M42;
        public Single M43;

        public static Matrix3D Identity { get { return new Matrix3D(1.0f, 0, 0, 0, 1.0f, 0, 0, 0, 1.0f, 0, 0, 0); } }

        public Matrix3D(Single M11, Single M12, Single M13, Single M21, Single M22, Single M23, Single M31, Single M32, Single M33, Single M41, Single M42, Single M43)
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
            this.M11 = 1.0f;
            this.M12 = 0;
            this.M13 = 0;
            this.M21 = 0;
            this.M22 = 1.0f;
            this.M23 = 0;
            this.M31 = 0;
            this.M32 = 0;
            this.M33 = 1.0f;
            this.M41 = Position.X;
            this.M42 = Position.Y;
            this.M43 = Position.Z;
        }

        public Matrix3D(Vector3 row1, Vector3 row2, Vector3 row3, Vector3 row4)
        {
            this.M11 = row1.X;
            this.M12 = row1.Y;
            this.M13 = row1.Z;
            this.M21 = row2.X;
            this.M22 = row2.Y;
            this.M23 = row2.Z;
            this.M31 = row3.X;
            this.M32 = row3.Y;
            this.M33 = row3.Z;
            this.M41 = row4.X;
            this.M42 = row4.Y;
            this.M43 = row4.Z;
        }

        static Single toRads = (Single)Math.PI / 180;

        public static Matrix3D CreateRotationX(Single Degrees)
        {
            Degrees *= toRads;

            Matrix3D m = Matrix3D.Identity;
            m.M22 = (Single)Math.Cos(Degrees);
            m.M23 = (Single)Math.Sin(Degrees);
            m.M32 = -(Single)Math.Sin(Degrees);
            m.M33 = (Single)Math.Cos(Degrees);
            return m;
        }

        public static Matrix3D CreateRotationY(Single Degrees)
        {
            Degrees *= toRads;

            Matrix3D m = Matrix3D.Identity;
            m.M11 = (Single)Math.Cos(Degrees);
            m.M13 = -(Single)Math.Sin(Degrees);
            m.M31 = (Single)Math.Sin(Degrees);
            m.M33 = (Single)Math.Cos(Degrees);
            return m;
        }

        public static Matrix3D CreateRotationZ(Single Degrees)
        {
            Degrees *= toRads;

            Matrix3D m = Matrix3D.Identity;
            m.M11 = (Single)Math.Cos(Degrees);
            m.M12 = (Single)Math.Sin(Degrees);
            m.M21 = -(Single)Math.Sin(Degrees);
            m.M22 = (Single)Math.Cos(Degrees);
            return m;
        }

        public static Matrix3D CreateScale(Single scale)
        {
            return CreateScale(scale, scale, scale);
        }

        public static Matrix3D CreateScale(Single x, Single y, Single z)
        {
            Matrix3D m = Matrix3D.Identity;
            m.M11 = x;
            m.M22 = y;
            m.M33 = z;
            return m;
        }

        public Vector3 Position
        {
            get { return new Vector3(M41, M42, M43); }
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }

        public Single Scale
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
            var p = x.Position + y.Position;

            return new Matrix3D(
                (x.M11 * y.M11) + (x.M12 * y.M21) + (x.M13 * y.M31), (x.M11 * y.M12) + (x.M12 * y.M22) + (x.M13 * y.M32), (x.M11 * y.M13) + (x.M12 * y.M23) + (x.M13 * y.M33),
                (x.M21 * y.M11) + (x.M22 * y.M21) + (x.M23 * y.M31), (x.M21 * y.M12) + (x.M22 * y.M22) + (x.M23 * y.M32), (x.M21 * y.M13) + (x.M22 * y.M23) + (x.M23 * y.M33),
                (x.M31 * y.M11) + (x.M32 * y.M21) + (x.M33 * y.M31), (x.M31 * y.M12) + (x.M32 * y.M22) + (x.M33 * y.M32), (x.M31 * y.M13) + (x.M32 * y.M23) + (x.M33 * y.M33),
                p.X, p.Y, p.Z
            );
        }

        public override string ToString()
        {
            return "{ {M11:" + M11 + " M12:" + M12 + " M13:" + M13 + "} {M21:" + M21 + " M22:" + M22 + " M23:" + M23 + "} {M31:" + M31 + " M32:" + M32 + " M33:" + M33 + "} {M41:" + M41 + " M42:" + M42 + " M43:" + M43 + "} }";
        }
    }
}
