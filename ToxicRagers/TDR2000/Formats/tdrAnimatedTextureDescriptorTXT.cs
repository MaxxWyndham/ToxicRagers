using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class AnimatedTextureDescriptor : List<AnimatedTextureDefinition>
    {
        public static AnimatedTextureDescriptor Load(string path)
        {
            DocumentParser file = new(path);
            AnimatedTextureDescriptor animatedTextureDescriptor = new();

            int numTextures = file.ReadInt();

            for (int i = 0; i < numTextures; i++)
            {
                animatedTextureDescriptor.Add(file.Read<AnimatedTextureDefinition>());
            }

            return animatedTextureDescriptor;
        }
    }

    public class AnimatedTextureDefinition
    {
        public string AnimationScript { get; set; }

        public string TextureToAnimate { get; set; }
    }
}
