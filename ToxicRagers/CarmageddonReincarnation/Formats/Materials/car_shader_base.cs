using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_shader_base : MT2
    {
        // Uses:
        // ReflectionBluryness, Fresnel_R0

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

        public car_shader_base() { }

        public car_shader_base(XElement xml)
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