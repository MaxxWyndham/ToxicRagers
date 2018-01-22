using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_spec_env_unlit_base : MT2
    {
        // Uses:
        // ReflectionMultiplier

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

        public simple_norm_spec_env_unlit_base() { }

        public simple_norm_spec_env_unlit_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_norm_spec_env_unlit_base
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