using ToxicRagers.TDR2000.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class AmbientSoundDescriptor
    {
        public string SFXList { get; set; }

        public string PoliceDriverType { get; set; }

        public List<AmbientLocation> AmbientLocations { get; set; } = new List<AmbientLocation>();

        public List<string> RandomSFX { get; set; } = new List<string>();

        public static AmbientSoundDescriptor Load(string path)
        {
            DocumentParser file = new(path);
            AmbientSoundDescriptor ambientSoundDescriptor = new()
            {
                SFXList = file.ReadString(),
                PoliceDriverType = file.ReadString()
            };

            int numAmbientSounds = file.ReadInt();

            for (int i = 0; i < numAmbientSounds; i++)
            {
                ambientSoundDescriptor.AmbientLocations.Add(file.Read<AmbientLocation>());
            }

            int numRandomSFX = file.ReadInt();

            for (int i = 0; i < numRandomSFX; i++)
            {
                ambientSoundDescriptor.RandomSFX.Add(file.ReadString());
            }

            return ambientSoundDescriptor;
        }
    }

    public class AmbientLocation
    {
        public string Sound { get; set; }

        public Vector3 Location { get; set; }
    }
}
