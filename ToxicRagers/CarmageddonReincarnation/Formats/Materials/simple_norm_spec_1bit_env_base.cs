using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_spec_1bit_env_base : MT2
    {
        // Uses:
        // DoubleSided, CastsShadows, FogEnabled, Walkable, Panickable
        // NeedsWorldLightDir, NeedsVertexColour
        // AlphaCutOff, Multiplier, ReflectionMultiplier

        string normal;
        string specular;

        Vector3 transmissiveFactor;

        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        public string Spec_Map
        {
            get => specular;
            set => specular = value;
        }

        public float TransmissiveFactor
        {
            get => transmissiveFactor.X;
            set => transmissiveFactor.X = value;
        }

        public simple_norm_spec_1bit_env_base() { }

        public simple_norm_spec_1bit_env_base(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            XElement tran = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "TransmissiveFactor").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (tran != null) { transmissiveFactor = ReadConstant(tran); }
        }
    }
}