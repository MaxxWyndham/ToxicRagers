using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class car_shader_double_sided_base : MT2
    {
        // Uses:
        // DoubleSided

        string blend;

        Vector3 blendFactor = Vector3.Zero;
        Vector3 fallOff = Vector3.Zero;
        Vector3 blendUVSlot = Vector3.Zero;
        Vector3 layer1UVSlot = Vector3.Zero;
        Vector3 layer2UVSlot = Vector3.Zero;

        [Ignore]
        public string DiffuseColour
        {
            get => diffuse;
            set => diffuse = value;
        }

        [Required]
        public string Side1_DiffuseColour1
        {
            get => GetFile("diffuses11");
            set => fileNames.Add("diffuses11", value);
        }

        [Required]
        public string Side1_Normal_Map1
        {
            get => GetFile("normals11");
            set => fileNames.Add("normals11", value);
        }

        [Required]
        public string Side1_Spec_Map1
        {
            get => GetFile("speculars11");
            set => fileNames.Add("speculars11", value);
        }

        [Required]
        public string Side1_DiffuseColour2
        {
            get => GetFile("diffuse");
            set => fileNames.Add("diffuse", value);
        }

        [Required]
        public string Side1_Normal_Map2
        {
            get => GetFile("normals12");
            set => fileNames.Add("normals12", value);
        }

        [Required]
        public string Side1_Spec_Map2
        {
            get => GetFile("speculars12");
            set => fileNames.Add("speculars12", value);
        }

        // <Texture Alias="Side1_Decal_DiffuseColour" FileName="spatter_X"/>
        // <Texture Alias="Side1_Decal_Normal_Map" FileName="spatter_X"/>
        // <Texture Alias="Side1_Decal_Spec_Map" FileName="spatter_X"/>

        [Required]
        public string Side2_DiffuseColour
        {
            get => GetFile("diffuses2");
            set => fileNames.Add("diffuses2", value);
        }

        [Required]
        public string Side2_Normal_Map
        {
            get => GetFile("normals2");
            set => fileNames.Add("normals2", value);
        }

        [Required]
        public string Side2_Spec_Map
        {
            get => GetFile("speculars2");
            set => fileNames.Add("speculars2", value);
        }

        [Required]
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

        public car_shader_double_sided_base() { }

        public car_shader_double_sided_base(XElement xml)
            : base(xml)
        {
            XElement diff11 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_DiffuseColour1").FirstOrDefault();
            XElement norm11 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Normal_Map1").FirstOrDefault();
            XElement spec11 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Spec_Map1").FirstOrDefault();
            XElement diff12 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_DiffuseColour2").FirstOrDefault();
            XElement norm12 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Normal_Map2").FirstOrDefault();
            XElement spec12 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side1_Spec_Map2").FirstOrDefault();
            XElement diff21 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side2_DiffuseColour").FirstOrDefault();
            XElement norm21 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side2_Normal_Map").FirstOrDefault();
            XElement spec21 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Side2_Spec_Map").FirstOrDefault();
            XElement blen = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "BlendMap").FirstOrDefault();

            if (diff12 != null) { Side1_DiffuseColour2 = diff12.Attribute("FileName").Value; }
            if (norm12 != null) { Side1_Normal_Map2 = norm12.Attribute("FileName").Value; }
            if (spec12 != null) { Side1_Spec_Map2 = spec12.Attribute("FileName").Value; }

            if (diff11 != null) { Side1_DiffuseColour1 = diff11.Attribute("FileName").Value; }
            if (norm11 != null) { Side1_Normal_Map1 = norm11.Attribute("FileName").Value; }
            if (spec11 != null) { Side1_Spec_Map1 = spec11.Attribute("FileName").Value; }

            if (diff21 != null) { Side2_DiffuseColour = diff21.Attribute("FileName").Value; }
            if (norm21 != null) { Side2_Normal_Map = norm21.Attribute("FileName").Value; }
            if (spec21 != null) { Side2_Spec_Map = spec21.Attribute("FileName").Value; }

            if (blen != null) { blend = blen.Attribute("FileName").Value; }

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

        protected override void initDefaultValues()
        {
            Translucent = Troolean.False;
            ReceivesShadows = Troolean.True;
            FogEnabled = Troolean.True;
            NeedsWorldSpaceVertexNormal = Troolean.True;
            NeedsWorldEyePos = Troolean.True;
            NeedsWorldVertexPos = Troolean.True;
            NeedsVertexColour = Troolean.True;
            NeedsLocalCubeMap = Troolean.True;
            NeedsLightingSpaceVertexNormal = Troolean.True;
            //NEEDS_PER_PIXEL_DIFFUSE_LIGHTING = 1,
            //NEEDS_PER_PIXEL_DIFFUSE_COLOUR = 1,
            //NEEDS_PER_PIXEL_SPECULAR_LIGHTING = 1,
            //NEEDS_PER_PIXEL_EMISSIVE_LIGHTING = 1,
            //NEEDS_PER_PIXEL_AMBIENT_LIGHTING = 1,
            //NEEDS_AMBIENT_LIGHT = 1,
            //NEEDS_TANGENT_FRAME = 1,
            //NEEDS_SPECULAR_MASK = 1,
            BlendMap = "Paint_B";

            TextureCoordSources = new List<TextureCoordSource>
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
            };

            Samplers = new List<Sampler>
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
            };
        }
    }
}