using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class glow_simple_norm_spec_env_base_A : MT2
    {
        // Uses:
        // DoubleSided, CastsShadows, Unpickable, FogEnabled
        // NeedsWorldLightDir
        // Multiplier, EmissiveFactor

        string normal;
        string specular;

        bool bNoSortAlpha;

        public string Normal_Map
        {
            get { return normal; }
            set { normal = value; }
        }

        public string Spec_Map
        {
            get { return specular; }
            set { specular = value; }
        }

        public bool NoSortAlpha
        {
            get { return bNoSortAlpha; }
            set { bNoSortAlpha = value; }
        }

        public glow_simple_norm_spec_env_base_A(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            var sort = xml.Descendants("NoSortAlpha").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (sort != null) { bNoSortAlpha = (sort.Attribute("Value").Value.ToLower() == "true"); }
        }
    }
}
