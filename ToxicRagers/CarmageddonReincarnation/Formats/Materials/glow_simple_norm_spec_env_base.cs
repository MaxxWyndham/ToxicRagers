using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class glow_simple_norm_spec_env_base : MT2
    {
        // Uses:
        // Multiplier, DoubleSided, TextureCoordSource, Panickable, FogEnabled, NeedsWorldLightDir

        string normal;
        string specular;

        Vector3 emissiveFactor;

        public string NormalMap
        {
            get { return normal; }
            set { normal = value; }
        }

        public string SpecMap
        {
            get { return specular; }
            set { specular = value; }
        }

        public Single EmissiveFactor
        {
            get { return emissiveFactor.X; }
            set { emissiveFactor.X = value; }
        }

        public glow_simple_norm_spec_env_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            var emmi = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "EmissiveFactor").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (emmi != null) { emissiveFactor = ReadConstant(emmi); }
        }

        public override string ToString()
        {
            return string.Format("DiffuseColour: {0} Normal_Map: {1} Spec_Map: {2}", diffuse, normal, specular);
        }
    }
}
