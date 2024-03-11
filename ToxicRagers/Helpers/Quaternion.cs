using System;

namespace ToxicRagers.Helpers
{
    public class Quaternion
    {
        public enum RotationOrder
        {
            OrderXYZ,
            OrderXZY,
            OrderYZX,
            OrderYXZ,
            OrderZXY,
            OrderZYX,
            OrderSphericXYZ
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

        public float Length => (float)Math.Sqrt((W * W) + new Vector3(X, Y, Z).LengthSquared);

        public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);

        public Quaternion() { }

        public Quaternion (Vector3 xyz, float w)
        {
            X = xyz.X;
            Y = xyz.Y;
            Z = xyz.Z;
            W = w;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Quaternion FromAxisAngle(Vector3 axis, float angle)
        {
            if (axis.LengthSquared == 0.0f) { return Identity; }

            Quaternion q = Identity;

            angle *= 0.5f;
            axis.Normalise();
            axis *= (float)Math.Sin(angle);

            q.X = axis.X;
            q.Y = axis.Y;
            q.Z = axis.Z;
            q.W = (float)Math.Cos(angle);

            q.Normalise();

            return q;
        }

        public void ToAxisAngle(out Vector3 axis, out float angle)
        {
            if (Math.Abs(W) > 1.0f) { Normalise(); }

            angle = 2.0f * (float)Math.Acos(W);

            float den = (float)Math.Sqrt(1.0 - (W * W));

            if (den > 0.0001f)
            {
                axis = new Vector3(X, Y, Z) / den;
            }
            else
            {
                axis = Vector3.UnitX;
            }
        }

        public Vector3 ToEuler(RotationOrder order)
        {
            Vector3 r = Vector3.Zero;

            float r11, r12, r21, r31, r32;
            r11 = r12 = r21 = r31 = r32 = 0;

            switch (order)
            {
                case RotationOrder.OrderXYZ:
                    r11 = -2.0f * (X * Y - W * Z); // was r31
                    r21 = 2.0f * (X * Z + W * Y);
                    r31 = -2.0f * (Y * Z - W * X); // was r11
                    r12 = W * W + X * X - Y * Y - Z * Z; // was r32
                    r32 = W * W - X * X - Y * Y + Z * Z; // was r12
                    break;

                case RotationOrder.OrderYZX:
                    r11 = -2.0f * (X * Z - W * Y);
                    r12 = W * W + X * X - Y * Y - Z * Z;
                    r21 = 2.0f * (X * Y + W * Z);
                    r31 = -2.0f * (Y * Z - W * X);
                    r32 = W * W - X * X + Y * Y - Z * Z;
                    break;
            }

            r.X = (float)Maths.RadiansToDegrees(Math.Atan2(r31, r32));
            r.Y = (float)Maths.RadiansToDegrees(Math.Asin(r21));
            r.Z = (float)Maths.RadiansToDegrees(Math.Atan2(r11, r12));

            return r;
        }

        public void Normalise()
        {
            float scale = 1.0f / Length;
            X *= scale;
            Y *= scale;
            Z *= scale;
            W *= scale;
        }

        public static Quaternion Parse(string q)
        {
            q = q.Replace(" ", "");
            string[] s = q.Split(new char[] { ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            return new Quaternion(s[0].ToSingle(), s[1].ToSingle(), s[2].ToSingle(), s[3].ToSingle());
        }

        public static Quaternion operator *(Quaternion x, Quaternion y)
        {
            Vector3 left = new Vector3(x.X, x.Y, x.Z);
            Vector3 right = new Vector3(y.X, y.Y, y.Z);

            return new Quaternion(
                (y.W * left) + (x.W * right) + Vector3.Cross(left, right),
                (x.W * y.W) - Vector3.Dot(left, right)
            );
        }
    }
}
