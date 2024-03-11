using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class MeshDescriptor : List<MeshEntry>
    {
        public static MeshDescriptor Load(string path)
        {
            DocumentParser file = new(path);
            MeshDescriptor meshDescriptor = new();

            do
            {
                meshDescriptor.Add(file.Read<MeshEntry>());
            } while (!file.EOF);

            return meshDescriptor;
        }
    }

    public class MeshEntry
    {
        public string Hierarchy { get; set; }

        public int RenderNode { get; set; }

        public int Optimise { get; set; }

        public string CollisionDescriptor { get; set; }
    }
}
