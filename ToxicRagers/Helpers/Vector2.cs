using System;
using System.ComponentModel;
using System.Globalization;

namespace ToxicRagers.Helpers
{
    [TypeConverterAttribute(typeof(Vector2Converter))]
    public class Vector2 : IEquatable<Vector2>
    {
        Single _x;
        Single _y;

        public Vector2(Single x, Single y)
        {
            _x = x;
            _y = y;
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

        public static Vector2 Zero
        {
            get { return new Vector2(0, 0); }
        }

        public static Vector2 Parse(string v)
        {
            v = v.Replace(" ", "");
            CultureInfo culture = new CultureInfo("en-GB");

            string[] s = v.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return new Vector2(Convert.ToSingle(s[0], culture), Convert.ToSingle(s[1], culture));
        }

        public override string ToString()
        {
            return "{X:" + _x + " Y:" + _y + "}";
        }

        public static Vector2 operator *(Vector2 x, float y)
        {
            return new Vector2(x._x * y, x._y * y);
        }

        public static bool operator ==(Vector2 x, Vector2 y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Vector2 x, Vector2 y)
        {
            return !x.Equals(y);
        }

        public bool Equals(Vector2 other)
        {
            return (this.X == other.X && this.Y == other.Y);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }
            return Equals(obj as Vector2);
        }

        public Single Length { get { return (Single)Math.Sqrt(Dot(this, this)); } }
        public Vector2 Normalised { get { return new Vector2(this.X / this.Length, this.Y / this.Length); } }

        public static Single Dot(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
    }

    public class Vector2Converter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Vector2)) { return true; }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is Vector2)
            {
                Vector2 v = value as Vector2;

                return v.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
