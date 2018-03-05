using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_additive_base : MT2
    {
        // Uses:
        // CastsShadows, DoubleSided
        // NeedsSeperateObjectColour, NeedsVertexColour

        [Required]
        public string DiffuseColour
        {
            get => diffuse;
            set => diffuse = value;
        }

        public new float Multiplier
        {
            get => multiplier.X;
            set => multiplier.X = value;
        }

        public simple_additive_base() { }

        public simple_additive_base(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
        }

        protected override void initDefaultValues()
        {
            Translucent = Troolean.True;
            CastsShadows = Troolean.False;

            NeedsWorldSpaceVertexNormal = Troolean.True;
            NeedsWorldEyePos = Troolean.True;
            NeedsWorldVertexPos = Troolean.True;
            //NEEDS_PER_PIXEL_DIFFUSE_COLOUR = 1,
            FogEnabled = Troolean.True;
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
                }
            };
        }
    }
}