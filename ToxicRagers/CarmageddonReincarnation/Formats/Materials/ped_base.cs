using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class ped_base : MT2
    {
        [Required]
        public string DiffuseColour
        {
            get => GetFile("diffuse");
            set => fileNames.Add("diffuse", value);
        }

        public ped_base() { }

        public ped_base(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { DiffuseColour = diff.Attribute("FileName").Value; }
        }
    }
}