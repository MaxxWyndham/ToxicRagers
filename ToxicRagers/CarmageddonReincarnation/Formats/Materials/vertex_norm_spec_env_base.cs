using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class vertex_norm_spec_env_base : MT2
    {
        // Uses:
        // Walkable
        // NeedsVertexColour, NeedsWorldSpaceVertexNormal, NeedsWorldEyePos, NeedsWorldVertexPos, NeedsWorldLightDir, NeedsLightingSpaceVertexNormal
        // EmissiveFactor

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

        public vertex_norm_spec_env_base() { }

        public vertex_norm_spec_env_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new vertex_norm_spec_env_base
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
        }
    }
}