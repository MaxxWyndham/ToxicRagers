using System;
using System.ComponentModel;

namespace ToxicRagers.Helpers
{
    public class StringArrayConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string[])) { return true; }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is string[])
            {
                string[] s = value as string[];
                return string.Join(",", s);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(null);
            return pdc;
        }
    }
}
