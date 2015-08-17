using System;
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
            get { return normal; }
            set { normal = value; }
        }

        public string Decals
        {
            get { return decal; }
            set { decal = value; }
        }

        public string DecalsSpec
        {
            get { return decalSpec; }
            set { decalSpec = value; }
        }

        public car_shader_no_normals_base(XElement xml)
            : base(xml)
        {
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var deca = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Decals").FirstOrDefault();
            var decs = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DecalsSpec").FirstOrDefault();

            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (deca != null) { decal = deca.Attribute("FileName").Value; }
            if (decs != null) { decalSpec = decs.Attribute("FileName").Value; }
        }
    }
}
