using System.Linq;
using System.Xml.Linq;

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

        [Required]
        public string DiffuseColour
        {
            get => diffuse;
            set => diffuse = value;
        }

        [Required]
        public string NormalMap
        {
            get => normal;
            set => normal = value;
        }

        [Required]
        public string SpecMap
        {
            get => specular;
            set => specular = value;
        }

        public bool NoSortAlpha
        {
            get => bNoSortAlpha;
            set => bNoSortAlpha = value;
        }

        public glow_simple_norm_spec_env_base_A() { }

        public glow_simple_norm_spec_env_base_A(XElement xml)
            : base(xml)
        {
            coreDefaults = new glow_simple_norm_spec_env_base_A
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "NormalMap").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "SpecMap").FirstOrDefault();
            XElement sort = xml.Descendants("NoSortAlpha").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (sort != null) { bNoSortAlpha = (sort.Attribute("Value").Value.ToLower() == "true"); }
        }
    }
}