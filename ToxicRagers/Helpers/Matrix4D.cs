using System;

namespace ToxicRagers.Helpers
{
    public class Matrix4D
    {
        public float M11; public float M12; public float M13; public float M14;
        public float M21; public float M22; public float M23; public float M24;
        public float M31; public float M32; public float M33; public float M34;
        public float M41; public float M42; public float M43; public float M44;

        public static Matrix4D Identity => new Matrix4D(1.0f, 0, 0, 0, 0, 1.0f, 0, 0, 0, 0, 1.0f, 0, 0, 0, 0, 1.0f);

        public float Determinant =>
            (M11 * M22 * M33 * M44) - (M11 * M22 * M34 * M43) + (M11 * M23 * M34 * M42) - (M11 * M23 * M32 * M44) +
            (M11 * M24 * M32 * M43) - (M11 * M24 * M33 * M42) - (M12 * M23 * M34 * M41) + (M12 * M23 * M31 * M44) -
            (M12 * M24 * M31 * M43) + (M12 * M24 * M33 * M41) - (M12 * M21 * M33 * M44) + (M12 * M21 * M34 * M43) +
            (M13 * M24 * M31 * M42) - (M13 * M24 * M32 * M41) + (M13 * M21 * M32 * M44) - (M13 * M21 * M34 * M42) +
            (M13 * M22 * M34 * M41) - (M13 * M22 * M31 * M44) - (M14 * M21 * M32 * M43) + (M14 * M21 * M33 * M42) -
            (M14 * M22 * M33 * M41) + (M14 * M22 * M31 * M43) - (M14 * M23 * M31 * M42) + (M14 * M23 * M32 * M41);

        public Matrix4D(
            float M11, float M12, float M13, float M14,
            float M21, float M22, float M23, float M24,
            float M31, float M32, float M33, float M34,
            float M41, float M42, float M43, float M44)
        {
            this.M11 = M11; this.M12 = M12; this.M13 = M13; this.M14 = M14;
            this.M21 = M21; this.M22 = M22; this.M23 = M23; this.M24 = M24;
            this.M31 = M31; this.M32 = M32; this.M33 = M33; this.M34 = M34;
            this.M41 = M41; this.M42 = M42; this.M43 = M43; this.M44 = M44;
        }

        public Matrix4D Clone()
        {
            return new Matrix4D(M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
        }

        public static Matrix4D LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 z = (eye - target).Normalised;
            Vector3 x = Vector3.Cross(up, z).Normalised;
            Vector3 y = Vector3.Cross(z, x).Normalised;

            Matrix4D result = Identity;

            result.M11 = x.X;
            result.M12 = y.X;
            result.M13 = z.X;

            result.M21 = x.Y;
            result.M22 = y.Y;
            result.M23 = z.Y;

            result.M31 = x.Z;
            result.M32 = y.Z;
            result.M33 = z.Z;

            result.M41 = -((x.X * eye.X) + (x.Y * eye.Y) + (x.Z * eye.Z));
            result.M42 = -((y.X * eye.X) + (y.Y * eye.Y) + (y.Z * eye.Z));
            result.M43 = -((z.X * eye.X) + (z.Y * eye.Y) + (z.Z * eye.Z));

            return result;
        }

        public Matrix4D Inverted()
        {
            Matrix4D m = Clone();

            if (m.Determinant != 0)
            {
                m.Invert();
            }

            return m;
        }

        public void Normalise()
        {
            float determinant = Determinant;

            M11 /= determinant;
            M12 /= determinant;
            M13 /= determinant;
            M14 /= determinant;

            M21 /= determinant;
            M22 /= determinant;
            M23 /= determinant;
            M24 /= determinant;

            M31 /= determinant;
            M32 /= determinant;
            M33 /= determinant;
            M34 /= determinant;

            M41 /= determinant;
            M42 /= determinant;
            M43 /= determinant;
            M44 /= determinant;
        }

        public Matrix4D Normalised()
        {
            Matrix4D m = Clone();

            m.Normalise();

            return m;
        }

        public void Invert()
        {
            Invert(this);
        }

        public Vector3 ExtractTranslation()
        {
            return new Vector3(M41, M42, M43);
        }

        public Vector3 ExtractScale()
        {
            return new Vector3(new Vector3(M11, M12, M13).Length, new Vector3(M21, M22, M23).Length, new Vector3(M31, M32, M33).Length);
        }

        public Quaternion ExtractRotation(bool rowNormalize = true)
        {
            Vector3 row0 = new Vector3(M11, M12, M13);
            Vector3 row1 = new Vector3(M21, M22, M23);
            Vector3 row2 = new Vector3(M31, M32, M33);

            if (rowNormalize)
            {
                row0 = row0.Normalised;
                row1 = row1.Normalised;
                row2 = row2.Normalised;
            }

            Quaternion q = Quaternion.Identity;
            double trace = 0.25 * (row0[0] + row1[1] + row2[2] + 1.0);

            if (trace > 0)
            {
                double sq = Math.Sqrt(trace);

                q.W = (float)sq;
                sq = 1.0 / (4.0 * sq);
                q.X = (float)((row1[2] - row2[1]) * sq);
                q.Y = (float)((row2[0] - row0[2]) * sq);
                q.Z = (float)((row0[1] - row1[0]) * sq);
            }
            else if (row0[0] > row1[1] && row0[0] > row2[2])
            {
                double sq = 2.0 * Math.Sqrt(1.0 + row0[0] - row1[1] - row2[2]);

                q.X = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row2[1] - row1[2]) * sq);
                q.Y = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[0] + row0[2]) * sq);
            }
            else if (row1[1] > row2[2])
            {
                double sq = 2.0 * Math.Sqrt(1.0 + row1[1] - row0[0] - row2[2]);

                q.Y = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row2[0] - row0[2]) * sq);
                q.X = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[1] + row1[2]) * sq);
            }
            else
            {
                double sq = 2.0 * Math.Sqrt(1.0 + row2[2] - row0[0] - row1[1]);

                q.Z = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row1[0] - row0[1]) * sq);
                q.X = (float)((row2[0] + row0[2]) * sq);
                q.Y = (float)((row2[1] + row1[2]) * sq);
            }

            q.Normalise();

            return q;
        }

        public static Matrix4D CreateScale(float s)
        {
            return new Matrix4D(
                s, 0, 0, 0,
                0, s, 0, 0,
                0, 0, s, 0,
                0, 0, 0, 1);
        }

        public static Matrix4D CreateScale(float x, float y, float z)
        {
            return new Matrix4D(
                x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1);
        }

        public static Matrix4D CreateScale(Vector3 v)
        {
            return new Matrix4D(
                v.X, 0, 0, 0,
                0, v.Y, 0, 0,
                0, 0, v.Z, 0,
                0, 0, 0, 1);
        }

        public static Matrix4D CreateTranslation(float x, float y, float z)
        {
            return new Matrix4D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                x, y, z, 1);
        }

        public static Matrix4D CreateTranslation(Vector3 v)
        {
            return new Matrix4D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                v.X, v.Y, v.Z, 1);
        }

        public static Matrix4D CreateRotationX(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new Matrix4D(
                1, 0, 0, 0,
                0, cos, sin, 0,
                0, -sin, cos, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4D CreateRotationY(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new Matrix4D(
                cos, 0, -sin, 0,
                0, 1, 0, 0,
                sin, 0, cos, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4D CreateOrthographic(float width, float height, float depthNear, float depthFar)
        {
            return CreateOrthographicOffCentre(-width / 2, width / 2, -height / 2, height / 2, depthNear, depthFar);
        }

        public static Matrix4D CreateOrthographicOffCentre(float left, float right, float bottom, float top, float depthNear, float depthFar)
        {
            Matrix4D result = Identity;

            float invRL = 1.0f / (right - left);
            float invTB = 1.0f / (top - bottom);
            float invFN = 1.0f / (depthFar - depthNear);

            result.M11 = 2 * invRL;
            result.M22 = 2 * invTB;
            result.M33 = -2 * invFN;

            result.M41 = -(right + left) * invRL;
            result.M42 = -(top + bottom) * invTB;
            result.M43 = -(depthFar + depthNear) * invFN;

            return result;
        }

        public static Matrix4D CreatePerspectiveFieldOfView(float fovy, float aspect, float depthNear, float depthFar)
        {
            float maxY = depthNear * (float)Math.Tan(0.5f * fovy);
            float minY = -maxY;
            float minX = minY * aspect;
            float maxX = maxY * aspect;

            return CreatePerspectiveOffCentre(minX, maxX, minY, maxY, depthNear, depthFar);
        }

        public static Matrix4D CreatePerspectiveOffCentre(float left, float right, float bottom, float top, float depthNear, float depthFar)
        {
            float x = 2.0f * depthNear / (right - left);
            float y = 2.0f * depthNear / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(depthFar + depthNear) / (depthFar - depthNear);
            float d = -(2.0f * depthFar * depthNear) / (depthFar - depthNear);

            return new Matrix4D(
                x, 0, 0, 0,
                0, y, 0, 0,
                a, b, c, -1,
                0, 0, d, 0);
        }

        public static Matrix4D CreateFromQuaternion(Quaternion q)
        {
            q.ToAxisAngle(out Vector3 axis, out float angle);

            return CreateFromAxisAngle(axis, angle);
        }

        public static Matrix4D CreateFromAxisAngle(Vector3 axis, float angle)
        {
            axis.Normalise();
            float axisX = axis.X;
            float axisY = axis.Y;
            float axisZ = axis.Z;

            float cos = (float)Math.Cos(-angle);
            float sin = (float)Math.Sin(-angle);
            float t = 1.0f - cos;

            float tXX = t * axisX * axisX;
            float tXY = t * axisX * axisY;
            float tXZ = t * axisX * axisZ;
            float tYY = t * axisY * axisY;
            float tYZ = t * axisY * axisZ;
            float tZZ = t * axisZ * axisZ;

            float sinX = sin * axisX;
            float sinY = sin * axisY;
            float sinZ = sin * axisZ;

            return new Matrix4D(
                tXX + cos, tXY - sinZ, tXZ + sinY, 0,
                tXY + sinZ, tYY + cos, tYZ - sinX, 0,
                tXZ - sinY, tYZ + sinX, tZZ + cos, 0,
                0, 0, 0, 1);
        }

        public Matrix4D ClearTranslation()
        {
            Matrix4D m = Clone();

            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;

            return m;
        }

        public Matrix4D ClearScale()
        {
            Matrix4D m = Clone();
            Vector3 x = new Vector3(m.M11, m.M12, m.M13).Normalised;
            Vector3 y = new Vector3(m.M21, m.M22, m.M23).Normalised;
            Vector3 z = new Vector3(m.M31, m.M32, m.M33).Normalised;

            m.M11 = x.X; m.M12 = x.Y; m.M13 = x.Z;
            m.M21 = y.X; m.M22 = y.Y; m.M23 = y.Z;
            m.M31 = z.X; m.M32 = z.Y; m.M33 = z.Z;

            return m;
        }

        public Matrix4D ClearRotation()
        {
            Matrix4D m = Clone();
            float x = new Vector3(m.M11, m.M12, m.M13).Length;
            float y = new Vector3(m.M21, m.M22, m.M23).Length;
            float z = new Vector3(m.M31, m.M32, m.M33).Length;

            m.M11 = x; m.M12 = 0; m.M13 = 0;
            m.M21 = 0; m.M22 = y; m.M23 = 0;
            m.M31 = 0; m.M32 = 0; m.M33 = z;

            return m;
        }

        public static Matrix4D Invert(Matrix4D m)
        {
            float d = m.Determinant;

            if (d == 0) { return m; }

            float invd = 1.0f / d;
            Matrix4D t = Identity;

            float b0 = (m.M31 * m.M42) - (m.M32 * m.M41);
            float b1 = (m.M31 * m.M43) - (m.M33 * m.M41);
            float b2 = (m.M34 * m.M41) - (m.M31 * m.M44);
            float b3 = (m.M32 * m.M43) - (m.M33 * m.M42);
            float b4 = (m.M34 * m.M42) - (m.M32 * m.M44);
            float b5 = (m.M33 * m.M44) - (m.M34 * m.M43);

            float a0 = (m.M11 * m.M22) - (m.M12 * m.M21);
            float a1 = (m.M11 * m.M23) - (m.M13 * m.M21);
            float a2 = (m.M14 * m.M21) - (m.M11 * m.M24);
            float a3 = (m.M12 * m.M23) - (m.M13 * m.M22);
            float a4 = (m.M14 * m.M22) - (m.M12 * m.M24);
            float a5 = (m.M13 * m.M24) - (m.M14 * m.M23);

            float d11 = (m.M22 * b5) + (m.M23 * b4) + (m.M24 * b3);
            float d12 = (m.M21 * b5) + (m.M23 * b2) + (m.M24 * b1);
            float d13 = (m.M21 * -b4) + (m.M22 * b2) + (m.M24 * b0);
            float d14 = (m.M21 * b3) + (m.M22 * -b1) + (m.M23 * b0);

            float d21 = (m.M12 * b5) + (m.M13 * b4) + (m.M14 * b3);
            float d22 = (m.M11 * b5) + (m.M13 * b2) + (m.M14 * b1);
            float d23 = (m.M11 * -b4) + (m.M12 * b2) + (m.M14 * b0);
            float d24 = (m.M11 * b3) + (m.M12 * -b1) + (m.M13 * b0);

            float d31 = (m.M42 * a5) + (m.M43 * a4) + (m.M44 * a3);
            float d32 = (m.M41 * a5) + (m.M43 * a2) + (m.M44 * a1);
            float d33 = (m.M41 * -a4) + (m.M42 * a2) + (m.M44 * a0);
            float d34 = (m.M41 * a3) + (m.M42 * -a1) + (m.M43 * a0);

            float d41 = (m.M32 * a5) + (m.M33 * a4) + (m.M34 * a3);
            float d42 = (m.M31 * a5) + (m.M33 * a2) + (m.M34 * a1);
            float d43 = (m.M31 * -a4) + (m.M32 * a2) + (m.M34 * a0);
            float d44 = (m.M31 * a3) + (m.M32 * -a1) + (m.M33 * a0);

            t.M11 = d11 * invd;
            t.M12 = -(d21 * invd);
            t.M13 = d31 * invd;
            t.M14 = -(d41 * invd);

            t.M21 = -(d12 * invd);
            t.M22 = d22 * invd;
            t.M23 = -(d32 * invd);
            t.M24 = d42 * invd;

            t.M31 = d13 * invd;
            t.M32 = -(d23 * invd);
            t.M33 = d33 * invd;
            t.M34 = -(d43 * invd);

            t.M41 = -(d14 * invd);
            t.M42 = d24 * invd;
            t.M43 = -(d34 * invd);
            t.M44 = d44 * invd;

            return t;
        }

        public Vector3 Right()
        {
            return new Vector3(M11, M12, M13);
        }

        public Vector3 Up()
        {
            return new Vector3(M21, M22, M23);
        }

        public Vector3 Forward()
        {
            return -new Vector3(M31, M32, M33);
        }

        public static Matrix4D operator *(Matrix4D x, Matrix4D y)
        {
            return new Matrix4D(
                (x.M11 * y.M11) + (x.M12 * y.M21) + (x.M13 * y.M31) + (x.M14 * y.M41),
                (x.M11 * y.M12) + (x.M12 * y.M22) + (x.M13 * y.M32) + (x.M14 * y.M42),
                (x.M11 * y.M13) + (x.M12 * y.M23) + (x.M13 * y.M33) + (x.M14 * y.M43),
                (x.M11 * y.M14) + (x.M12 * y.M24) + (x.M13 * y.M34) + (x.M14 * y.M44),

                (x.M21 * y.M11) + (x.M22 * y.M21) + (x.M23 * y.M31) + (x.M24 * y.M41),
                (x.M21 * y.M12) + (x.M22 * y.M22) + (x.M23 * y.M32) + (x.M24 * y.M42),
                (x.M21 * y.M13) + (x.M22 * y.M23) + (x.M23 * y.M33) + (x.M24 * y.M43),
                (x.M21 * y.M14) + (x.M22 * y.M24) + (x.M23 * y.M34) + (x.M24 * y.M44),

                (x.M31 * y.M11) + (x.M32 * y.M21) + (x.M33 * y.M31) + (x.M34 * y.M41),
                (x.M31 * y.M12) + (x.M32 * y.M22) + (x.M33 * y.M32) + (x.M34 * y.M42),
                (x.M31 * y.M13) + (x.M32 * y.M23) + (x.M33 * y.M33) + (x.M34 * y.M43),
                (x.M31 * y.M14) + (x.M32 * y.M24) + (x.M33 * y.M34) + (x.M34 * y.M44),

                (x.M41 * y.M11) + (x.M42 * y.M21) + (x.M43 * y.M31) + (x.M44 * y.M41),
                (x.M41 * y.M12) + (x.M42 * y.M22) + (x.M43 * y.M32) + (x.M44 * y.M42),
                (x.M41 * y.M13) + (x.M42 * y.M23) + (x.M43 * y.M33) + (x.M44 * y.M43),
                (x.M41 * y.M14) + (x.M42 * y.M24) + (x.M43 * y.M34) + (x.M44 * y.M44)
            );
        }

        public override string ToString()
        {
            return string.Format("{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} }}", M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
        }
    }
}