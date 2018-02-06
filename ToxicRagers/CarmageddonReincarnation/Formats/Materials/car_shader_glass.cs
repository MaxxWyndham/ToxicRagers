using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_shader_glass : MT2
    {
        string normal;
        string specular;

        [Required]
        public string DiffuseColour
        {
            get => diffuse;
            set => diffuse = value;
        }

        [Required]
        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        [Required]
        public string Spec_Map
        {
            get => specular;
            set => specular = value;
        }

        public car_shader_glass() { }

        public car_shader_glass(XElement xml)
            : base(xml)
        {
            coreDefaults = new car_shader_glass()
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