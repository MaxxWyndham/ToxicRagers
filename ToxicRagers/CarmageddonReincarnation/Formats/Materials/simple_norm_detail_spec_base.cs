using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_detail_spec_base : MT2
    {
        public simple_norm_detail_spec_base() { }

        public simple_norm_detail_spec_base(XElement xml)
            : base(xml)
        {
        }
    }
}