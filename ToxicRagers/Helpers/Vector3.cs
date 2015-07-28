using System;
using System.ComponentModel;

namespace ToxicRagers.Helpers
{
    [TypeConverterAttribute(typeof(Vector3Converter))]
    public class Vector3 : IEquatable<Vector3>
    {
        private Single _x;
        private Single _y;
        private Single _z;

        public Single X { get { return _x; } set { _x = value; } }
        public Single Y { get { return _y; } set { _y = value; } }
        public Single Z { get { return _z; } set { _z = value; } }

        public Vector3(Single x, Single y, Single z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public static Vector3 Up
        {
            get { return new Vector3(0, 1, 0); }
        }

        public static Vector3 Zero
        {
            get { return new Vector3(0, 0, 0); }
        }

        public Vector3 Normalised
        {
            get { this.Normalise(); return this; }
        }

        public override string ToString()
        {
            return string.Format("{{X: {0,15:F9} Y: {1,15:F9} Z: {2,15:F9} }}", _x, _y, _z);
        }

        public Single Sum()
        {
            return _x + _y + _z;
        }

        public static Vector3 TransformVector(Vector3 v, Matrix3D m)
        {
            return v * m;
        }

        public static Vector3 Parse(string v)
        {
            v = v.Replace(" ", "");
            string[] s = v.Split(new char[] { ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            return new Vector3(s[0].ToSingle(), s[1].ToSingle(), s[2].ToSingle());
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
		        v1.Y * v2.Z - v1.Z * v2.Y,
		        v1.Z * v2.X - v1.X * v2.Z,
		        v1.X * v2.Y - v1.Y * v2.X
            );
        }

        public static Single Distance(Vector3 v1, Vector3 v2)
        {
            return (Single)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
        }

        public static Single Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public Single Length
        {
            get { return (Single)Math.Sqrt(this.LengthSquared); }
        }

        public Single LengthSquared
        {
            get { return this._x * this._x + this._y * this._y + this._z * this._z; }
        }

        public void Normalise()
        {
            this._x /= this.Length;
            this._y /= this.Length;
            this._z /= this.Length;
        }

        public static Vector3 operator *(Single y, Vector3 x) { return x * y; }

        public static Vector3 operator *(Vector3 x, Single y)
        {
            return new Vector3(x._x * y, x._y * y, x._z * y);
        }

        public static Vector3 operator *(Vector3 x, Vector3 y)
        {
            return new Vector3(x._x * y.X, x._y * y.Y, x._z * y.Z);
        }

        public static Vector3 operator -(Vector3 x, Vector3 y)
        {
            return new Vector3(x._x - y.X, x._y - y.Y, x._z - y.Z);
        }

        public static Vector3 operator -(Vector3 x)
        {
            return new Vector3(-x._x, -x._y, -x._z);
        }

        public static Vector3 operator +(Vector3 x, Vector3 y)
        {
            return new Vector3(x._x + y.X, x._y + y.Y, x._z + y.Z);
        }

        public static Vector3 operator /(Vector3 x, Single y)
        {
            return new Vector3(x._x / y, x._y / y, x._z / y);
        }

        public static Vector3 operator *(Vector3 x, Matrix3D y)
        {
            Vector3 r = new Vector3(0, 0, 0);

            r.X = (x.X * y.M11) + (x.Y * y.M21) + (x.Z * y.M31);
            r.Y = (x.X * y.M12) + (x.Y * y.M22) + (x.Z * y.M32);
            r.Z = (x.X * y.M13) + (x.Y * y.M23) + (x.Z * y.M33);
            r += y.Position;

            return r;
        }

        public static bool operator ==(Vector3 x, Vector3 y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Vector3 x, Vector3 y)
        {
            return !x.Equals(y);
        }

        public bool Equals(Vector3 other)
        {
            return (this.X == other.X && this.Y == other.Y && this.Z == other.Z);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }
            return Equals(obj as Vector3);
        }
    }

    public class Vector3Converter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Vector3)) { return true; }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is Vector3)
            {
                Vector3 v = value as Vector3;

                return v.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
