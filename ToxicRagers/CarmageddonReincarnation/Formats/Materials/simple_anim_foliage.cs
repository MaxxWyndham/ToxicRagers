using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_anim_foliage : MT2
    {
        public simple_anim_foliage() { }

        public simple_anim_foliage(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_anim_foliage
            {

            };
        }
    }
}