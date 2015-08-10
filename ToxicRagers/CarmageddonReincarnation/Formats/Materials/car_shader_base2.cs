using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_shader_base2 : MT2
    {
        // Uses:
        // DoubleSided

        string normal;
        string specular;
        string diffuse2;
        string normal2;
        string specular2;
        string blend;

        Vector3 blendFactor;
        Vector3 fallOff;
        Vector3 blendUVSlot;
        Vector3 layer1UVSlot;
        Vector3 layer2UVSlot;

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

        public string DiffuseColour2
        {
            get { return diffuse2; }
            set { diffuse2 = value; }
        }

        public string Normal_Map2
        {
            get { return normal2; }
            set { normal2 = value; }
        }

        public string Spec_Map2
        {
            get { return specular2; }
            set { specular2 = value; }
        }

        public string BlendMap
        {
            get { return blend; }
            set { blend = value; }
        }

        public Single BlendFactor
        {
            get { return blendFactor.X; }
            set { blendFactor.X = value; }
        }

        public Single Falloff
        {
            get { return fallOff.X; }
            set { fallOff.X = value; }
        }

        public Single Layer1UVSlot
        {
            get { return layer1UVSlot.X; }
            set { layer1UVSlot.X = value; }
        }

        public Single Layer2UVSlot
        {
            get { return layer2UVSlot.X; }
            set { layer2UVSlot.X = value; }
        }

        public Single BlendUVSlot
        {
            get { return blendUVSlot.X; }
            set { blendUVSlot.X = value; }
        }

        public car_shader_base2(XElement xml)
            : base(xml)
        {
            var dif1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var nor1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var spe1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            var dif2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour2").FirstOrDefault();
            var nor2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map2").FirstOrDefault();
            var spe2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map2").FirstOrDefault();
            var blen = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "BlendMap").FirstOrDefault();

            if (dif1 != null) { diffuse = dif1.Attribute("FileName").Value; }
            if (nor1 != null) { normal = nor1.Attribute("FileName").Value; }
            if (spe1 != null) { specular = spe1.Attribute("FileName").Value; }
            if (dif2 != null) { diffuse2 = dif2.Attribute("FileName").Value; }
            if (nor2 != null) { normal2 = nor2.Attribute("FileName").Value; }
            if (spe2 != null) { specular2 = spe2.Attribute("FileName").Value; }
            if (blen != null) { blend = blen.Attribute("FileName").Value; }

            var blnf = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "BlendFactor").FirstOrDefault();
            var fall = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Falloff").FirstOrDefault();
            var bluv = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "BlendUVSlot").FirstOrDefault();
            var l1uv = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Layer1UVSlot").FirstOrDefault();
            var l2uv = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Layer2UVSlot").FirstOrDefault();

            if (blnf != null) { blendFactor = ReadConstant(blnf); }
            if (fall != null) { fallOff = ReadConstant(fall); }
            if (bluv != null) { blendUVSlot = ReadConstant(bluv); }
            if (l1uv != null) { layer1UVSlot = ReadConstant(l1uv); }
            if (l2uv != null) { layer2UVSlot = ReadConstant(l2uv); }
        }
    }
}
