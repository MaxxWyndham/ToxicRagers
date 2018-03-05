using System.Collections.Generic;
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

        string cubeMap;
        bool bDirectionSet;
        float directionAngle;

        [Required]
        public string DiffuseColour
        {
            get => GetFile("diffuse");
            set => fileNames.Add("diffuse", value);
        }

        [Required]
        public string Normal_Map
        {
            get => GetFile("normal");
            set => fileNames.Add("normal", value);
        }

        [Required]
        public string Spec_Map
        {
            get => GetFile("specular");
            set => fileNames.Add("specular", value);
        }

        public string EnvironmentCube
        {
            get => cubeMap;
            set => cubeMap = value;
        }

        public bool DirectionSet
        {
            get => bDirectionSet;
            set => bDirectionSet = value;
        }

        public float DirectionAngle
        {
            get => directionAngle;
            set => directionAngle = value;
        }

        public simple_norm_spec_env_base() { }

        public simple_norm_spec_env_base(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement norm = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            XElement cube = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "EnvironmentCube").FirstOrDefault();
            XElement dset = xml.Descendants("DirectionSet").FirstOrDefault();
            XElement dang = xml.Descendants("DirectionAngle").FirstOrDefault();

            if (diff != null) { DiffuseColour = diff.Attribute("FileName").Value; }
            if (norm != null) { Normal_Map = norm.Attribute("FileName").Value; }
            if (spec != null) { Spec_Map = spec.Attribute("FileName").Value; }
            if (cube != null) { cubeMap = cube.Attribute("FileName").Value; }
            if (dset != null) { bDirectionSet = (dset.Attribute("Value").Value.ToLower() == "true"); }
            if (dang != null) { directionAngle = dang.Attribute("Value").Value.ToSingle(); }
        }

        protected override void initDefaultValues()
        {
            Translucent = Troolean.False;

            NeedsWorldSpaceVertexNormal = Troolean.True;
            NeedsWorldEyePos = Troolean.True;
            NeedsWorldVertexPos = Troolean.True;
            //NEEDS_PER_PIXEL_DIFFUSE_LIGHTING = 1,
            //NEEDS_PER_PIXEL_DIFFUSE_COLOUR = 1,
            //NEEDS_PER_PIXEL_SPECULAR_LIGHTING = 1,
            //NEEDS_PER_PIXEL_EMISSIVE_LIGHTING = 1,
            NeedsLocalCubeMap = Troolean.True;
            //NEEDS_PER_PIXEL_AMBIENT_LIGHTING = 1,
            //NEEDS_AMBIENT_LIGHT = 1,
            NeedsLightingSpaceVertexNormal = Troolean.True;
            //NEEDS_TANGENT_FRAME = 1,
            ReceivesShadows = Troolean.True;
            FogEnabled = Troolean.True;
            //NEEDS_SPECULAR_MASK = 1,
            NeedsSeperateObjectColour = Troolean.True;

            TextureCoordSources = new List<TextureCoordSource>
            {
                new TextureCoordSource
                {
                    Alias = "Tex0",
                    UVStream = 0
                }
            };

            Samplers = new List<Sampler>
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
            };
        }
    }
}