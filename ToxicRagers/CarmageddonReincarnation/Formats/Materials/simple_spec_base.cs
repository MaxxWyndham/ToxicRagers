using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_spec_base : MT2
    {
        // Uses:
        // NeedsWorldSpaceVertexNormal, NeedsWorldEyePos, NeedsWorldVertexPos, NeedsWorldLightDir
        // Fresnel_R0, ReflectionMultiplier

        string specular;

        public string Spec_Map
        {
            get => specular;
            set => specular = value;
        }

        public simple_spec_base() { }

        public simple_spec_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_spec_base
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
        }
    }
}