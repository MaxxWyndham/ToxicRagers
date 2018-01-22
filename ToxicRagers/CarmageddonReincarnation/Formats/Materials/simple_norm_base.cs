using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_base : MT2
    {
        // Uses:
        // Emissive_Colour, EmissiveLight, Translucent, Walkable, Panickable, Sitable, DoubleSided, ReflectionBluryness, ReflectionMultiplier, Fresnel_R0

        string normal;
        Vector3 specColour;
        Vector3 specPower;

        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        public Vector3 SpecColour
        {
            get => specColour;
            set => specColour = value;
        }

        public float SpecPower
        {
            get => specPower.X;
            set => specPower.X = value;
        }

        public simple_norm_base() { }

        public simple_norm_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_norm_base
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement scol = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecColour").FirstOrDefault();
            XElement spow = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecPower").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (scol != null) { specColour = ReadConstant(scol); }
            if (spow != null) { specPower = ReadConstant(spow); }
        }
    }
}