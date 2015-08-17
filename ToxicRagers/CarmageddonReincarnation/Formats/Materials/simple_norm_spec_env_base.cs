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

        [Required]
        public string DiffuseColour
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        [Required]
        public string Normal_Map
        {
            get { return normal; }
            set { normal = value; }
        }

        [Required]
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

        public simple_norm_spec_env_base() { }

        public simple_norm_spec_env_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_norm_spec_env_base
            {
                Translucent = TriStateBool.False,

                NeedsWorldSpaceVertexNormal = TriStateBool.True,
                NeedsWorldEyePos = TriStateBool.True,
                NeedsWorldVertexPos = TriStateBool.True,
                //NEEDS_PER_PIXEL_DIFFUSE_LIGHTING = 1,
                //NEEDS_PER_PIXEL_DIFFUSE_COLOUR = 1,
                //NEEDS_PER_PIXEL_SPECULAR_LIGHTING = 1,
                //NEEDS_PER_PIXEL_EMISSIVE_LIGHTING = 1,
                NeedsLocalCubeMap = TriStateBool.True,
                //NEEDS_PER_PIXEL_AMBIENT_LIGHTING = 1,
                //NEEDS_AMBIENT_LIGHT = 1,
                NeedsLightingSpaceVertexNormal = TriStateBool.True,
                //NEEDS_TANGENT_FRAME = 1,
                ReceivesShadows = TriStateBool.True,
                FogEnabled = TriStateBool.True,
                //NEEDS_SPECULAR_MASK = 1,
                NeedsSeperateObjectColour = TriStateBool.True,

                TextureCoordSources =
                {
                    new TextureCoordSource
                    {
                        Alias = "Tex0",
                        UVStream = 0
                    }
                },

                Samplers =
                {
                    new Sampler 
                    {
                        Alias = "SAMPLER_DiffuseColour",
                        UsageRGB = Sampler.Usage.DiffuseAlbedo,
                        sRGBRead = true
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_NormalMap",
                        UsageRGB = Sampler.Usage.TangentSpaceNormals
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_SpecMap",
                        UsageRGB = Sampler.Usage.SpecColour,
                        UsageAlpha = Sampler.Usage.SpecPower
                    }
                }
            };

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
