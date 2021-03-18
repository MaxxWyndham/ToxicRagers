using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public enum NetworkAvailability
    {
        eagle,
        all
    }

    public class OpponentTXT
    {
        public List<OpponentDetails> Opponents = new List<OpponentDetails>();

        public static OpponentTXT Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            OpponentTXT opponents = new OpponentTXT();

            int numOpponents = file.ReadInt();

            for (int i = 0; i < numOpponents; ++i)
            {
                opponents.Opponents.Add(new OpponentDetails
                {
                    DriverName = file.ReadLine(),
                    DriverShortName = file.ReadLine(),
                    CarName = file.ReadLine(),
                    StrengthRating = file.ReadInt(),
                    CostToBuy = file.ReadInt(),
                    NetworkAvailability = file.ReadEnum<NetworkAvailability>(),
                    CarFilename = file.ReadLine(),
                    TopSpeed = file.ReadLine(),
                    KerbWeight = file.ReadLine(),
                    To60 = file.ReadLine(),
                    Bio = file.ReadLine()
                });
            }

            return opponents;
        }
    }

    public class OpponentDetails
    {
        public string DriverName { get; set; }

        public string DriverShortName { get; set; }

        public string CarName { get; set; }

        public int StrengthRating { get; set; }

        public int CostToBuy { get; set; }

        public NetworkAvailability NetworkAvailability { get; set; }

        public string CarFilename { get; set; }

        public string TopSpeed { get; set; }

        public string KerbWeight { get; set; }

        public string To60 { get; set; }

        public string Bio { get; set; }
    }
}