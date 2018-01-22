using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_shader_glass : MT2
    {
        public car_shader_glass() { }

        public car_shader_glass(XElement xml)
            : base(xml)
        {
            coreDefaults = new car_shader_glass()
            {

            };
        }
    }
}