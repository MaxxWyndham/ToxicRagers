using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class glow_simple_norm_spec_env_base : MT2
    {
        // Uses:
        // DoubleSided, Panickable, FogEnabled
        // NeedsWorldLightDir, NeedsWorldSpaceVertexNormal, NeedsWorldEyePos, NeedsWorldVertexPos, NeedsLightingSpaceVertexNormal, NeedsVertexColour;
        // Multiplier, EmissiveFactor

        [Required]
        public string DiffuseColour
        {
            get => GetFile("diffuse");
            set => fileNames.Add("diffuse", value);
        }

        [Required]
        public string Normal_Map
        {
            get => GetFile("normal");
            set => fileNames.Add("normal", value);
        }

        [Required]
        public string Spec_Map
        {
            get => GetFile("specular");
            set => fileNames.Add("specular", value);
        }

        public glow_simple_norm_spec_env_base() { }

        public glow_simple_norm_spec_env_base(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();

            if (diff != null) { DiffuseColour = diff.Attribute("FileName").Value; }
            if (norm != null) { Normal_Map = norm.Attribute("FileName").Value; }
            if (spec != null) { Spec_Map = spec.Attribute("FileName").Value; }
        }
    }
}