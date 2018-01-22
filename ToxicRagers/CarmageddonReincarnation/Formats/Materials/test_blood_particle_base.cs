using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class test_blood_particle_base : MT2
    {
        public test_blood_particle_base() { }

        public test_blood_particle_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new test_blood_particle_base
            {

            };
        }
    }
}