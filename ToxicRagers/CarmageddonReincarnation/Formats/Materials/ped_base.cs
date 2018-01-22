using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class ped_base : MT2
    {
        public ped_base() { }

        public ped_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new ped_base
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
        }
    }
}