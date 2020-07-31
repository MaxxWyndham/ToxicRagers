using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.Carmageddon.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class OpponentsTXT
    {
        public class OpponentDetails
        {
            public string DriverName;
            public string DriverShortName;
            public string CarName;
            public int StrengthRating;
            public int CostToBuy;
            public string NetworkAvailability;
            public string CarFilename;
            public string TopSpeed;
            public string KerbWeight;
            public string To60;
            public string Bio;
        }

        public List<OpponentDetails> Opponents = new List<OpponentDetails>();

        public static OpponentsTXT Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            OpponentsTXT opponents = new OpponentsTXT();

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
                    NetworkAvailability = file.ReadLine(),
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
}
