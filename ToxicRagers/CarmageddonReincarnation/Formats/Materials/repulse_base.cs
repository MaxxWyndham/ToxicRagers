using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class repulse_base : MT2
    {
        public repulse_base() { }

        public repulse_base(XElement xml)
            : base(xml)
        {
        }
    }
}