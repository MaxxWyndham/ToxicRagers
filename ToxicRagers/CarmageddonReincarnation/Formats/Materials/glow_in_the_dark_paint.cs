using System;
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
            get { return normal; }
            set { normal = value; }
        }

        public glow_in_the_dark_paint(XElement xml)
            : base(xml)
        {
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();

            if (norm != null) { normal = norm.Attribute("FileName").Value; }

            var diff = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var spec = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecularColour").FirstOrDefault();
            var ambi = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "AmbientLight").FirstOrDefault();

            if (diff != null) { diffuseColour = ReadConstant(diff); }
            if (spec != null) { specularColour = ReadConstant(spec); }
            if (ambi != null) { ambientLight = ReadConstant(ambi); }
        }
    }
}
