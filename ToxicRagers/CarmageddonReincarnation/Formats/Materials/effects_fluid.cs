using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class effects_fluid : MT2
    {
        // Uses:

        string normal;
        string normal2;
        string noise;
        string reflect2D;

        Vector3 specColour;
        Vector3 specPower;

        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        public string Normal_Map2
        {
            get => normal2;
            set => normal2 = value;
        }

        public string Noise
        {
            get => noise;
            set => noise = value;
        }

        public string Reflect_2d
        {
            get => reflect2D;
            set => reflect2D = value;
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

        public effects_fluid() { }

        public effects_fluid(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement nor1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement nor2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map2").FirstOrDefault();
            XElement nois = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Noise").FirstOrDefault();
            XElement refl = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Reflect_2d").FirstOrDefault();
            XElement scol = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecColour").FirstOrDefault();
            XElement spow = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecPower").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (nor1 != null) { normal = nor1.Attribute("FileName").Value; }
            if (nor2 != null) { normal2 = nor2.Attribute("FileName").Value; }
            if (nois != null) { noise = nois.Attribute("FileName").Value; }
            if (refl != null) { reflect2D = refl.Attribute("FileName").Value; }
            if (scol != null) { specColour = ReadConstant(scol); }
            if (spow != null) { specPower = ReadConstant(spow); }
        }
    }
}