using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_base : MT2
    {
        // Uses:
        // EmissiveLight, Translucent, Walkable, Panickable, Sitable, DoubleSided, ReflectionMultiplier

        string normal;
        Vector3 fresnelR0;
        Vector3 reflectionBluryness;
        Vector3 specColour;
        Vector3 specPower;
        Vector3 emissiveColour;

        public string NormalMap
        {
            get { return normal; }
            set { normal = value; }
        }

        public Single Fresnel_R0
        {
            get { return fresnelR0.X; }
            set { fresnelR0.X = value; }
        }

        public Single ReflectionBluryness
        {
            get { return reflectionBluryness.X; }
            set { reflectionBluryness.X = value; }
        }

        public Vector3 SpecColour
        {
            get { return specColour; }
            set { specColour = value; }
        }

        public Single SpecPower
        {
            get { return specPower.X; }
            set { specPower.X = value; }
        }

        public Vector3 Emissive_Colour
        {
            get { return emissiveColour; }
            set { emissiveColour = value; }
        }

        public simple_norm_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var fres = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Fresnel_R0").FirstOrDefault();
            var refl = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "ReflectionBluryness").FirstOrDefault();
            var scol = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecColour").FirstOrDefault();
            var spow = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "SpecPower").FirstOrDefault();
            var emmi = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Emissive_Colour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (fres != null) { fresnelR0 = ReadConstant(fres); }
            if (refl != null) { reflectionBluryness = ReadConstant(refl); }
            if (scol != null) { specColour = ReadConstant(scol); }
            if (spow != null) { specPower = ReadConstant(spow); }
            if (emmi != null) { emissiveColour = ReadConstant(emmi); }
        }
    }
}
