using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_shader_no_normals_base : MT2
    {
        // Uses:
        // DoubleSided
        // ReflectionBluryness, ReflectionMultiplier, Fresnel_R0

        string normal;
        string decal;
        string decalSpec;

        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        public string Decals
        {
            get => decal;
            set => decal = value;
        }

        public string DecalsSpec
        {
            get => decalSpec;
            set => decalSpec = value;
        }

        public car_shader_no_normals_base() { }

        public car_shader_no_normals_base(XElement xml)
            : base(xml)
        {
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement deca = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Decals").FirstOrDefault();
            XElement decs = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DecalsSpec").FirstOrDefault();

            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (deca != null) { decal = deca.Attribute("FileName").Value; }
            if (decs != null) { decalSpec = decs.Attribute("FileName").Value; }
        }
    }
}