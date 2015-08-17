using System;
using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_base : MT2
    {
        // Uses:
        // Walkable
        // EmissiveFactor

        public string DiffuseColour
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        public simple_base() : base() { }

        public simple_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
        }
    }
}
