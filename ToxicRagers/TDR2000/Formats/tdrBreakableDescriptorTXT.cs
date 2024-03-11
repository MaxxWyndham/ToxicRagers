using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class BreakableDescriptor
    {
        public string Hierarchy { get; set; }

        public int RenderNode { get; set; }

        public List<BreakableTexture> BreakableTextures { get; set; } = new List<BreakableTexture>();

        public static BreakableDescriptor Load(string path)
        {
            DocumentParser file = new(path);
            BreakableDescriptor breakableDescriptor = new()
            {
                Hierarchy = file.ReadString(),
                RenderNode = file.ReadInt()
            };

            int numBreakableTextures = file.ReadInt();

            for (int i = 0; i < numBreakableTextures; i++)
            {
                BreakableTexture texture = new()
                {
                    TextureName = file.ReadString(),
                    BreakabilityType = file.ReadInt(),
                    BreakSound = file.ReadString(),
                    ImpactVelocityScale = file.ReadSingle(),
                    RandonVelocityMag = file.ReadSingle(),
                    RandomUpwardVelocityMag = file.ReadSingle(),
                    RandomNormalVelocityMag = file.ReadSingle(),
                    RandomSpinRate = file.ReadSingle()
                };

                int numDamageLevels = file.ReadInt();

                for (int j = 0; j < numDamageLevels; j++)
                {
                    texture.ForceLevels.Add(file.ReadSingle());
                }

                breakableDescriptor.BreakableTextures.Add(texture);
            }

            return breakableDescriptor;
        }
    }

    public class BreakableTexture
    {
        public string TextureName { get; set; }

        public int BreakabilityType { get; set; }

        public string BreakSound { get; set; }

        public float ImpactVelocityScale { get; set; }

        public float RandonVelocityMag { get; set; }

        public float RandomUpwardVelocityMag { get; set; }

        public float RandomNormalVelocityMag { get; set; }

        public float RandomSpinRate { get; set; }

        public List<float> ForceLevels { get; set; } = new List<float>();
    }
}
