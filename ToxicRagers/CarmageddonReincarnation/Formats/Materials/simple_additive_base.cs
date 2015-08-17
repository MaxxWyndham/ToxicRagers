using System;
using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_additive_base : MT2
    {
        // Uses:
        // CastsShadows, DoubleSided
        // NeedsSeperateObjectColour, NeedsVertexColour
        // Multiplier

        [Required]
        public string DiffuseColour
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        public simple_additive_base() { }

        public simple_additive_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_additive_base
            {
                Translucent = TriStateBool.True,
                CastsShadows = TriStateBool.False,

                NeedsWorldSpaceVertexNormal = TriStateBool.True,
                NeedsWorldEyePos = TriStateBool.True,
                NeedsWorldVertexPos = TriStateBool.True,
                //NEEDS_PER_PIXEL_DIFFUSE_COLOUR = 1,
                FogEnabled = TriStateBool.True,
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
                    }
                }
            };

            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
        }
    }
}
