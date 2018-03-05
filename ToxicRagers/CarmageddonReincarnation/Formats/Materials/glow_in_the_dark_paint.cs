using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class glow_in_the_dark_paint : MT2
    {
        string normal;
        Vector3 diffuseColour;
        Vector3 specularColour;
        Vector3 ambientLight;

        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        public Vector3 Diffuse_Colour
        {
            get => diffuseColour;
            set => diffuseColour = value;
        }

        public Vector3 SpecularColour
        {
            get => specularColour;
            set => specularColour = value;
        }

        public Vector3 AmbientLight
        {
            get => ambientLight;
            set => ambientLight = value;
        }

        public glow_in_the_dark_paint() { }

        public glow_in_the_dark_paint(XElement xml)
            : base(xml)
        {
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();

            if (norm != null) { normal = norm.Attribute("FileName").Value; }

            XElement diff = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement spec = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecularColour").FirstOrDefault();
            XElement ambi = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "AmbientLight").FirstOrDefault();

            if (diff != null) { diffuseColour = ReadConstant(diff); }
            if (spec != null) { specularColour = ReadConstant(spec); }
            if (ambi != null) { ambientLight = ReadConstant(ambi); }
        }
    }
}