using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_spec_env_base_A : MT2
    {
        // Uses:
        // DoubleSided, Walkable, Panickable, CastsShadows
        // NeedsVertexColour
        // Multiplier, ReflectionBluryness, ReflectionMultiplier, Fresnel_R0, EmissiveFactor

        string normal;
        string specular;

        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        public string Spec_Map
        {
            get => specular;
            set => specular = value;
        }

        public simple_norm_spec_env_base_A() { }

        public simple_norm_spec_env_base_A(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_norm_spec_env_base_A
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();

            if (diff != null) { diffuse = (diff.Attributes("FileName").Any() ? diff.Attribute("FileName").Value : diff.Attribute("Movie").Value); }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
        }
    }
}