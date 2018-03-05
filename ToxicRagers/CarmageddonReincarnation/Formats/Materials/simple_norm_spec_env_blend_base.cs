using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_spec_env_blend_base : MT2
    {
        // Uses:
        // Walkable, Sitable, Translucent, Panickable
        // NeedsLocalCubeMapm NeedsWorldLightDir
        // ReflectionBluryness, Fresnel_R0, ReflectionMultiplier

        string substance2;

        string normal;
        string specular;
        string diffuse2;
        string normal2;
        string specular2;
        string blend;
        string cubeMap;

        Vector3 blendFactor;
        Vector3 fallOff;
        Vector3 blendUVSlot;
        Vector3 layer1UVSlot;
        Vector3 layer2UVSlot;

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

        public string DiffuseColour2
        {
            get => diffuse2;
            set => diffuse2 = value;
        }

        public string Normal_Map2
        {
            get => normal2;
            set => normal2 = value;
        }

        public string Spec_Map2
        {
            get => specular2;
            set => specular2 = value;
        }

        public string Substance2
        {
            get => substance2;
            set => substance2 = value;
        }

        public string EnvironmentCube
        {
            get => cubeMap;
            set => cubeMap = value;
        }

        public string BlendMap
        {
            get => blend;
            set => blend = value;
        }

        public float BlendFactor
        {
            get => blendFactor.X;
            set => blendFactor.X = value;
        }

        public float Falloff
        {
            get => fallOff.X;
            set => fallOff.X = value;
        }

        public float Layer1UVSlot
        {
            get => layer1UVSlot.X;
            set => layer1UVSlot.X = value;
        }

        public float Layer2UVSlot
        {
            get => layer2UVSlot.X;
            set => layer2UVSlot.X = value;
        }

        public float BlendUVSlot
        {
            get => blendUVSlot.X;
            set => blendUVSlot.X = value;
        }

        public simple_norm_spec_env_blend_base() { }

        public simple_norm_spec_env_blend_base(XElement xml)
            : base(xml)
        {
            XElement subs = xml.Descendants("Substance2").FirstOrDefault();

            if (subs != null) { substance2 = subs.Attribute("Name").Value; }

            XElement dif1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement nor1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement spe1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            XElement dif2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour2").FirstOrDefault();
            XElement nor2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map2").FirstOrDefault();
            XElement spe2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map2").FirstOrDefault();
            XElement blen = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "BlendMap").FirstOrDefault();
            XElement cube = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "EnvironmentCube").FirstOrDefault();

            if (dif1 != null) { diffuse = dif1.Attribute("FileName").Value; }
            if (nor1 != null) { normal = nor1.Attribute("FileName").Value; }
            if (spe1 != null) { specular = spe1.Attribute("FileName").Value; }
            if (dif2 != null) { diffuse2 = dif2.Attribute("FileName").Value; }
            if (nor2 != null) { normal2 = nor2.Attribute("FileName").Value; }
            if (spe2 != null) { specular2 = spe2.Attribute("FileName").Value; }
            if (blen != null) { blend = blen.Attribute("FileName").Value; }
            if (cube != null) { cubeMap = cube.Attribute("FileName").Value; }

            XElement blnf = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "BlendFactor").FirstOrDefault();
            XElement fall = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Falloff").FirstOrDefault();
            XElement bluv = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "BlendUVSlot").FirstOrDefault();
            XElement l1uv = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Layer1UVSlot").FirstOrDefault();
            XElement l2uv = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Layer2UVSlot").FirstOrDefault();

            if (blnf != null) { blendFactor = ReadConstant(blnf); }
            if (fall != null) { fallOff = ReadConstant(fall); }
            if (bluv != null) { blendUVSlot = ReadConstant(bluv); }
            if (l1uv != null) { layer1UVSlot = ReadConstant(l1uv); }
            if (l2uv != null) { layer2UVSlot = ReadConstant(l2uv); }
        }
    }
}