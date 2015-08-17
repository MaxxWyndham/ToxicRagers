using System;
using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class glass_base : MT2
    {
        // Uses:
        // Walkable, CastsShadows, DoubleSided
        // Multiplier, EmissiveLight, ReflectionMultiplier

        string normal;
        string specular;
        string cubeMap;

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

        public glass_base() { }

        public glass_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new glass_base
            {
                Translucent = TriStateBool.True,
                DoubleSided = TriStateBool.False,

                FogEnabled = TriStateBool.True,
                NeedsWorldSpaceVertexNormal = TriStateBool.True,
                NeedsWorldEyePos = TriStateBool.True,
                NeedsWorldVertexPos = TriStateBool.True,
                //NEEDS_TANGENT_FRAME = 1,
                //NEEDS_PER_PIXEL_SPECULAR_LIGHTING = 1,
                NeedsLocalCubeMap = TriStateBool.True,
                //NEEDS_SPECULAR_MASK = 1,
                ReceivesShadows = TriStateBool.True,
                //IS_THIN_GLASS = 1,

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
