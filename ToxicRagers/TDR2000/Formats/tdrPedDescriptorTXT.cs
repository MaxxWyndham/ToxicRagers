using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class PedDescriptor
    {
        public int NumPeds { get; set; }

        public string AIPaths { get; set; }

        public string PlacementFile { get; set; }

        public List<string> Skeletons { get; set; } = new List<string>();

        public List<string> Skins { get; set; } = new List<string>();

        public List<PedTextureDefinition> Textures { get; set; } = new List<PedTextureDefinition>();

        public static PedDescriptor Load(string path)
        {
            DocumentParser file = new(path);
            PedDescriptor pedDescriptor = new()
            {
                NumPeds = file.ReadInt(),
                AIPaths = file.ReadString(),
                PlacementFile = file.ReadString()
            };

            int numSkeletons = file.ReadInt();

            for (int i = 0; i < numSkeletons; i++)
            {
                pedDescriptor.Skeletons.Add(file.ReadString());
            }

            int numSkins = file.ReadInt();

            for (int i = 0; i < numSkins; i++)
            {
                pedDescriptor.Skins.Add(file.ReadString());
            }

            int numTextures = file.ReadInt();

            for (int i = 0; i < numTextures; i++)
            {
                pedDescriptor.Textures.Add(file.Read<PedTextureDefinition>());
            }

            return pedDescriptor;
        }
    }

    public class PedTextureDefinition
    {
        public string Face { get; set; }

        public string Body { get; set; }

        public string FaceDamage { get; set; }

        public string BodyDamage { get; set; }

        public int SkinNumber { get; set; }

        public string SkinType { get; set; }
    }
}
