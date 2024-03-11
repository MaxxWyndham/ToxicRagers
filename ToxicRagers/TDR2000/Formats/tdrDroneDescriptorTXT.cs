using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class DroneDescriptor
    {
        public List<DroneSpec> Drones { get; set; } = new List<DroneSpec>();

        public static DroneDescriptor Load(string path)
        {
            DocumentParser file = new(path);
            DroneDescriptor droneDescriptor = new();

            int numDrones = file.ReadInt();

            for (int i = 0; i < numDrones; i++)
            {
                droneDescriptor.Drones.Add(file.Read<DroneSpec>());
            }

            return droneDescriptor;
        }
    }

    public class DroneSpec
    {
        public string DroneName { get; set; }

        public int Instances { get; set; }
    }
}
