using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class skybox_base : MT2
    {
        // Uses:
        // FogEnabled
        // Multiplier

        public skybox_base() { }

        public skybox_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new skybox_base
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
        }
    }
}