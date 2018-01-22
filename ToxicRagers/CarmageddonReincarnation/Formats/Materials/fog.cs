using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class fog : MT2
    {
        // Uses:
        // Unpickable

        Vector3 mainColour;
        Vector3 minDistance;
        Vector3 maxDistance;

        public Vector3 MainColour
        {
            get => mainColour;
            set => mainColour = value;
        }

        public float Min_distance
        {
            get => minDistance.X;
            set => minDistance.X = value;
        }

        public float Max_distance
        {
            get => maxDistance.X;
            set => maxDistance.X = value;
        }

        public fog() { }

        public fog(XElement xml)
            : base(xml)
        {
            coreDefaults = new fog
            {

            };

            XElement main = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "MainColour").FirstOrDefault();
            XElement mind = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Min_distance").FirstOrDefault();
            XElement maxd = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Max_distance").FirstOrDefault();

            if (main != null) { mainColour = ReadConstant(main); }
            if (mind != null) { minDistance = ReadConstant(mind); }
            if (maxd != null) { maxDistance = ReadConstant(maxd); }
        }
    }
}