using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_spec_1bit_env_base : MT2
    {
        // Uses:
        // DoubleSided, CastsShadows, FogEnabled, NeedsWorldLightDir, NeedsVertexColour, Walkable, Panickable, Multiplier, ReflectionMultiplier

        string normal;
        string specular;

        Vector3 alphaCutOff;
        Vector3 transmissiveFactor;

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

        public Single AlphaCutOff
        {
            get { return alphaCutOff.X; }
            set { alphaCutOff.X = value; }
        }

        public Single TransmissiveFactor
        {
            get { return transmissiveFactor.X; }
            set { transmissiveFactor.X = value; }
        }

        public simple_norm_spec_1bit_env_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            var alph = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "AlphaCutOff").FirstOrDefault();
            var tran = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "TransmissiveFactor").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (alph != null) { alphaCutOff = ReadConstant(alph); }
            if (tran != null) { transmissiveFactor = ReadConstant(tran); }
        }
    }
}
