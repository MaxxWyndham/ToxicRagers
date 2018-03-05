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

        bool bNoSortAlpha;

        [Required]
        public string DiffuseColour
        {
            get => GetFile("diffuse");
            set => fileNames.Add("diffuse", value);
        }

        [Required]
        public string NormalMap
        {
            get => GetFile("normal");
            set => fileNames.Add("normal", value);
        }

        [Required]
        public string SpecMap
        {
            get => GetFile("specular");
            set => fileNames.Add("specular", value);
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
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "NormalMap").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "SpecMap").FirstOrDefault();
            XElement sort = xml.Descendants("NoSortAlpha").FirstOrDefault();

            if (diff != null) { DiffuseColour = diff.Attribute("FileName").Value; }
            if (norm != null) { NormalMap = norm.Attribute("FileName").Value; }
            if (spec != null) { SpecMap = spec.Attribute("FileName").Value; }
            if (sort != null) { bNoSortAlpha = (sort.Attribute("Value").Value.ToLower() == "true"); }
        }
    }
}