using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_translucent_body_base : MT2
    {
        public car_translucent_body_base() { }

        public car_translucent_body_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new car_translucent_body_base()
            {

            };
        }
    }
}