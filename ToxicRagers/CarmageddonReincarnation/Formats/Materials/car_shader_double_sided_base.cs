using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_shader_double_sided_base : MT2
    {
        // Uses:
        // DoubleSided

        string diffuseS1_1;
        string normalS1_1;
        string specularS1_1;
        string normalS1_2;
        string specularS1_2;
        string diffuseS2;
        string normalS2;
        string specularS2;
        string blend;

        Vector3 blendFactor = Vector3.Zero;
        Vector3 fallOff = Vector3.Zero;
        Vector3 blendUVSlot = Vector3.Zero;
        Vector3 layer1UVSlot = Vector3.Zero;
        Vector3 layer2UVSlot = Vector3.Zero;

        [Required]
        public string Side1_DiffuseColour1
        {
            get { return diffuseS1_1; }
            set { diffuseS1_1 = value; }
        }

        [Required]
        public string Side1_Normal_Map1
        {
            get { return normalS1_1; }
            set { normalS1_1 = value; }
        }

        [Required]
        public string Side1_Spec_Map1
        {
            get { return specularS1_1; }
            set { specularS1_1 = value; }
        }

        [Required]
        public string Side1_DiffuseColour2
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        [Required]
        public string Side1_Normal_Map2
        {
            get { return normalS1_2; }
            set { normalS1_2 = value; }
        }

        [Required]
        public string Side1_Spec_Map2
        {
            get { return specularS1_2; }
            set { specularS1_2 = value; }
        }

        // <Texture Alias="Side1_Decal_DiffuseColour" FileName="spatter_X"/>
	    // <Texture Alias="Side1_Decal_Normal_Map" FileName="spatter_X"/>
	    // <Texture Alias="Side1_Decal_Spec_Map" FileName="spatter_X"/>

        [Required]
        public string Side2_DiffuseColour
        {
            get { return diffuseS2; }
            set { diffuseS2 = value; }
        }

        [Required]
        public string Side2_Normal_Map
        {
            get { return normalS2; }
            set { normalS2 = value; }
        }

        [Required]
        public string Side2_Spec_Map
        {
            get { return specularS2; }
            set { specularS2 = value; }
        }

        [Required]
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

        [Ignore]
        public new string DiffuseColour
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        public car_shader_double_sided_base() { }

        public car_shader_double_sided_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new car_shader_double_sided_base
            {
                Translucent = Troolean.False,
                ReceivesShadows = Troolean.True,
                FogEnabled = Troolean.True,
                NeedsWorldSpaceVertexNormal = Troolean.True,
                NeedsWorldEyePos = Troolean.True,
                NeedsWorldVertexPos = Troolean.True,
                NeedsVertexColour = Troolean.True,
                NeedsLocalCubeMap = Troolean.True,
                NeedsLightingSpaceVertexNormal = Troolean.True,
                //NEEDS_PER_PIXEL_DIFFUSE_LIGHTING = 1,
                //NEEDS_PER_PIXEL_DIFFUSE_COLOUR = 1,
                //NEEDS_PER_PIXEL_SPECULAR_LIGHTING = 1,
                //NEEDS_PER_PIXEL_EMISSIVE_LIGHTING = 1,
                //NEEDS_PER_PIXEL_AMBIENT_LIGHTING = 1,
                //NEEDS_AMBIENT_LIGHT = 1,
                //NEEDS_TANGENT_FRAME = 1,
                //NEEDS_SPECULAR_MASK = 1,
                BlendMap = "Paint_B",

                TextureCoordSources =
                {
                    new TextureCoordSource
                    {
                        Alias = "tex0",
                        UVStream = 0
                    },
                    new TextureCoordSource
                    {
                        Alias = "tex1",
                        UVStream = 1
                    }
                },

                Samplers =
                {
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_DiffuseColour1",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.DiffuseAlbedo,
                        sRGBRead = true
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_NormalMap1",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.TangentSpaceNormals
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_SpecMap1",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.SpecColour,
                        UsageAlpha = Sampler.Usage.SpecPower
                    },

                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_DiffuseColour2",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.DiffuseAlbedo,
                        sRGBRead = true
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_NormalMap2",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.TangentSpaceNormals
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_SpecMap2",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.SpecColour,
                        UsageAlpha = Sampler.Usage.SpecPower
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_Decal_DiffuseColour",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.DiffuseAlbedo,
                        sRGBRead = true
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_Decal_Normal_Map",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.TangentSpaceNormals
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side1_Decal_Spec_Map",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.SpecColour,
                        UsageAlpha = Sampler.Usage.SpecPower
                    },

                    new Sampler 
                    {
                        Alias = "SAMPLER_Side2_DiffuseColour",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.DiffuseAlbedo,
                        sRGBRead = true
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side2_NormalMap",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.TangentSpaceNormals
                    },
                    new Sampler 
                    {
                        Alias = "SAMPLER_Side2_SpecMap",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                        UsageRGB = Sampler.Usage.SpecColour,
                        UsageAlpha = Sampler.Usage.SpecPower
                    },

                    new Sampler 
                    {
                        Alias = "SAMPLER_BlendMap",
                        MinFilter = Sampler.Filter.Anisotropic,
                        MipFilter = Sampler.Filter.Anisotropic,
                        MagFilter = Sampler.Filter.Anisotropic,
                    }
                }
            };

            var diff11 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_DiffuseColour1").FirstOrDefault();
            var norm11 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Normal_Map1").FirstOrDefault();
            var spec11 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Spec_Map1").FirstOrDefault();
            var diff12 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_DiffuseColour2").FirstOrDefault();
            var norm12 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Normal_Map2").FirstOrDefault();
            var spec12 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Spec_Map2").FirstOrDefault();
            var diff21 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side2_DiffuseColour").FirstOrDefault();
            var norm21 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side2_Normal_Map").FirstOrDefault();
            var spec21 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side2_Spec_Map").FirstOrDefault();
            var blen = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "BlendMap").FirstOrDefault();

            if (diff11 != null) { diffuseS1_1 = diff11.Attribute("FileName").Value; }
            if (norm11 != null) { normalS1_1 = norm11.Attribute("FileName").Value; }
            if (spec11 != null) { specularS1_1 = spec11.Attribute("FileName").Value; }

            if (diff12 != null) { diffuse = diff12.Attribute("FileName").Value; }
            if (norm12 != null) { normalS1_2 = norm12.Attribute("FileName").Value; }
            if (spec12 != null) { specularS1_2 = spec12.Attribute("FileName").Value; }

            if (diff21 != null) { diffuseS2 = diff21.Attribute("FileName").Value; }
            if (norm21 != null) { normalS2 = norm21.Attribute("FileName").Value; }
            if (spec21 != null) { specularS2 = spec21.Attribute("FileName").Value; }

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
