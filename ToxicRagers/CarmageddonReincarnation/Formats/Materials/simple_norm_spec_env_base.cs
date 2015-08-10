using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_spec_env_base : MT2
    {
        // Uses:
        // DoubleSided, Translucent, Walkable, Panickable, CastsShadows
        // NeedsVertexColour, NeedsWorldSpaceVertexNormal, NeedsWorldEyePos, NeedsWorldVertexPos, NeedsWorldLightDir, NeedsLightingSpaceVertexNormal, NeedsLocalCubeMap, NeedsSeperateObjectColour
        // Multiplier, Emissive_Colour, EmissiveLight, ReflectionBluryness, ReflectionMultiplier, Fresnel_R0

        string normal;
        string specular;
        string cubeMap;
        bool bDirectionSet;
        Single directionAngle;

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

        public bool DirectionSet
        {
            get { return bDirectionSet; }
            set { bDirectionSet = value; }
        }

        public Single DirectionAngle
        {
            get { return directionAngle; }
            set { directionAngle = value; }
        }

        public simple_norm_spec_env_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            var cube = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "EnvironmentCube").FirstOrDefault();
            var dset = xml.Descendants("DirectionSet").FirstOrDefault();
            var dang = xml.Descendants("DirectionAngle").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (norm != null) { normal = norm.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (cube != null) { cubeMap = cube.Attribute("FileName").Value; }
            if (dset != null) { bDirectionSet = (dset.Attribute("Value").Value.ToLower() == "true"); }
            if (dang != null) { directionAngle = dang.Attribute("Value").Value.ToSingle(); }
        }
    }
}
