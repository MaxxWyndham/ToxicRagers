using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_base : MT2
    {
        // Uses:
        // Walkable
        // EmissiveFactor

        [Required]
        public string DiffuseColour
        {
            get => GetFile("diffuse");
            set => fileNames.Add("diffuse", value);
        }

        public simple_base() { }

        public simple_base(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { DiffuseColour = diff.Attribute("FileName").Value; }
        }
    }
}