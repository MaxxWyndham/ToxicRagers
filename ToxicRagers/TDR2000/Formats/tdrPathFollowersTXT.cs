using ToxicRagers.Helpers;
using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class PathFollowers : List<PathFollowerDefinition>
    {
        public static PathFollowers Load(string path)
        {
            DocumentParser file = new(path);
            PathFollowers pathFollowers = new();

            int numFollowers = file.ReadInt();

            for (int i = 0; i < numFollowers; i++)
            {
                pathFollowers.Add(new PathFollowerDefinition
                {
                    Follower = file.ReadString(),
                    Path = file.ReadString(),
                    Unknown = file.ReadString(),
                    A = file.Read<Vector4>(),
                    B = file.ReadSingle(),
                    C = file.ReadInt(),
                    D = file.ReadInt()
                });
            }

            return pathFollowers;
        }
    }

    public class PathFollowerDefinition
    {
        public string Follower { get; set; }

        public string Path { get; set; }

        public string Unknown { get; set; }

        public Vector4 A { get; set; }

        public float B { get; set; }

        public int C { get; set; }

        public int D { get; set; }
    }
}
