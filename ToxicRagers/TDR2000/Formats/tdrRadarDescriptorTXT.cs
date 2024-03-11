using ToxicRagers.Helpers;
using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class RadarDescriptor
    {
        public string RadarHierarchy { get; set; }

        public int RadarHierarchyRenderNode { get; set; }

        public float TileXSize { get; set; }

        public float TileYSize { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Vector2 MapCentre { get; set; }

        public static RadarDescriptor Load(string path)
        {
            DocumentParser file = new(path);
            RadarDescriptor radarDescriptor = new()
            {
                RadarHierarchy = file.ReadString(),
                RadarHierarchyRenderNode = file.ReadInt(),
                TileXSize = file.ReadSingle(),
                TileYSize = file.ReadSingle(),
                Width = file.ReadInt(),
                Height = file.ReadInt(),
                MapCentre = file.Read<Vector2>()
            };

            return radarDescriptor;
        }
    }
}
