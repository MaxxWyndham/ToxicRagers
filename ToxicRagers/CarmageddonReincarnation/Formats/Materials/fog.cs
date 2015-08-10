using System;
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
            get { return mainColour; }
            set { mainColour = value; }
        }

        public Single Min_distance
        {
            get { return minDistance.X; }
            set { minDistance.X = value; }
        }

        public Single Max_distance
        {
            get { return maxDistance.X; }
            set { maxDistance.X = value; }
        }

        public fog(XElement xml)
            : base(xml)
        {
            var main = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "MainColour").FirstOrDefault();
            var mind = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Min_distance").FirstOrDefault();
            var maxd = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Max_distance").FirstOrDefault();

            if (main != null) { mainColour = ReadConstant(main); }
            if (mind != null) { minDistance = ReadConstant(mind); }
            if (maxd != null) { maxDistance = ReadConstant(maxd); }
        }
    }
}
