using System;
using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_additive_base : MT2
    {
        // Uses:
        // CastsShadows, DoubleSided
        // NeedsSeperateObjectColour, NeedsVertexColour
        // Multiplier

        public simple_additive_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
        }
    }
}
