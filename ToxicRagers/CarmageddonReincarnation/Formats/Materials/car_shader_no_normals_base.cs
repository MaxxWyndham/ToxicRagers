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

        string normals;
        string decals;
        string decalsSpec;

        public string Normal_Map
        {
            get { return normals; }
            set { normals = value; }
        }

        public string Decals
        {
            get { return decals; }
            set { decals = value; }
        }

        public string DecalsSpec
        {
            get { return decalsSpec; }
            set { decalsSpec = value; }
        }

        public car_shader_no_normals_base(XElement xml)
            : base(xml)
        {
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var deca = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Decals").FirstOrDefault();
            var decs = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DecalsSpec").FirstOrDefault();

            if (norm != null) { normals = norm.Attribute("FileName").Value; }
            if (deca != null) { decals = deca.Attribute("FileName").Value; }
            if (decs != null) { decalsSpec = decs.Attribute("FileName").Value; }
        }
    }
}
