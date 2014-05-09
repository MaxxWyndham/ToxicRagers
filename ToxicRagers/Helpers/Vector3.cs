using System;
using System.ComponentModel;
using System.Globalization;

namespace ToxicRagers.Helpers
{
    [TypeConverterAttribute(typeof(Vector3Converter))]
    public class Vector3 : IEquatable<Vector3>
    {
        private Single _x;
        private Single _y;
        private Single _z;

        public Vector3(Single n)
        {
            _x = n;
            _y = n;
            _z = n;
        }

        public Vector3(Single x, Single y, Single z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Single X
        {
            get { return _x; }
            set { _x = value; }
        }

        public Single Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Single Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public static Vector3 Up
        {
            get { return new Vector3(0, 1, 0); }
        }

        public static Vector3 Zero
        {
            get { return new Vector3(0, 0, 0); }
        }

        public override string ToString()
        {
            return "{X:" + _x + " Y:" + _y + " Z:" + _z + "}";
        }

        public string ToBBoxString()
        {
            return _x + "," + _y + "," + _z;
        }

        public Single Sum()
        {
            return _x + _y + _z;
        }

        public static Vector3 Parse(string v)
        {
            v = v.Replace(" ", "");
            string[] s = v.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            CultureInfo culture = new CultureInfo("en-GB");
            return new Vector3(Convert.ToSingle(s[0], culture), Convert.ToSingle(s[1], culture), Convert.ToSingle(s[2], culture));
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            // Cross Product
            return new Vector3((v1.Y * v2.Z) - (v1.Z * v2.Y), (v1.Z * v2.X) - (v1.X * v2.Z), (v1.X * v2.Y) - (v1.Y * v2.X));
        }

        public static Single Distance(Vector3 v1, Vector3 v2)
        {
            return (Single)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
        }

        public static Single Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Single Magnitude(Vector3 v)
        {
            return (Single)Math.Sqrt(Dot(v, v));
        }

        public static Single LengthSquared(Vector3 v)
        {
            return Dot(v, v);
        }

        public static Vector3 Min(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                Math.Min(v1.X, v2.X), 
                Math.Min(v1.Y, v2.Y), 
                Math.Min(v1.Z, v2.Z)
                );
        }

        public static Vector3 Max(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                Math.Max(v1.X, v2.X), 
                Math.Max(v1.Y, v2.Y), 
                Math.Max(v1.Z, v2.Z)
                );
        }

        public static Vector3 Truncate(Vector3 v)
        {
            return new Vector3(
                (float)(v.X > 0.0f ? Math.Floor(v.X) : Math.Ceiling(v.X)),
                (float)(v.Y > 0.0f ? Math.Floor(v.Y) : Math.Ceiling(v.Y)),
                (float)(v.Z > 0.0f ? Math.Floor(v.Z) : Math.Ceiling(v.Z))
            );
        }

        public Single Length
        {
            get
            {
                return Magnitude(this);
            }
        }

        [BrowsableAttribute(false)]
        public Vector3 Normalise
        {
            get
            {
                return new Vector3(this.X / this.Length, this.Y / this.Length, this.Z / this.Length);
            }
        }

        //Public Function ToNonCarString() As String
        //    Return Format(_x, "0.000000") & ", " & Format(_y, "0.000000") & ", " & Format(_z, "0.000000")
        //End Function

        //Public Shared Operator +(ByVal x As Vector3, ByVal y As Vector3) As Vector3
        //    Return New Vector3(x._x + y._x, x._y + y._y, x._z + y._z)
        //End Operator

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
