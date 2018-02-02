using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_base : MT2
    {
        // Uses:
        // Walkable
        // EmissiveFactor

        public string DiffuseColour
        {
            get => diffuse;
            set => diffuse = value;
        }

        public simple_base() { }

        public simple_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_base
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
        }
    }
}