using System;
using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class glass_base : MT2
    {
        // Uses:
        // Multiplier, Walkable, CastsShadows, DoubleSided, TextureCoordSource, Sampler, EmissiveLight, ReflectionMultiplier

        string normal;
        string specular;
        string cubeMap;

        public string Normal_Map
        {
            get { return normal; }
            set { normal = value; }
        }

        public string Spec_Map
        {
            get { return specular; }
            set { specular = value; }
        }

        public string EnvironmentCube
        {
            get { return cubeMap; }
            set { cubeMap = value; }
        }

        public glass_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            var cube = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "EnvironmentCube").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (cube != null) { cubeMap = cube.Attribute("FileName").Value; }
        }

        public override string ToString()
        {
            return string.Format("DiffuseColour: {0} Normal_Map: {1} Spec_Map: {2} EnvironmentCube: {3} Multiplier: {4}", diffuse, normal, specular, cubeMap, multiplier);
        }
    }
}
