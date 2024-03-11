using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class AnimatedProps : List<AnimatedPropDefinition>
    {
        public static AnimatedProps Load(string path)
        {
            DocumentParser file = new(path);
            AnimatedProps animatedProps = new();

            int numProps = file.ReadInt();

            for (int i = 0; i < numProps; i++)
            {
                animatedProps.Add(file.Read<AnimatedPropDefinition>());
            }

            return animatedProps;
        }
    }

    public class AnimatedPropDefinition
    {
        public int Type { get; set; }

        public string PropName { get; set; }
    }
}
