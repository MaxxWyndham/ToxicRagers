using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Formats
{
    public class OpponentsTXT
    {
        public class OpponentDetails
        {
            public string DriverName;
            public string DriverShortName;
            public int CarIndex;
            public int StrengthRating;
            public string NetworkAvailability;
            public string CarFLIFilename;
            public string CarFilename;
            public string DriveFLIFilename;
            public int NumberOfTextChunks;
            public Vector2 Section1TextOffset;
            public Vector2 Section1Frames;
            public int Section1NumberOfLines;
            public string TopSpeed;
            public string KerbWeight;
            public string To60;
            public Vector2 Section2TextOffset;
            public Vector2 Section2Frames;
            public int Section2NumberOfLines;
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
                OpponentDetails opponent = new OpponentDetails
                {
                    DriverName = file.ReadLine(),
                    DriverShortName = file.ReadLine(),
                    CarIndex = file.ReadInt(),
                    StrengthRating = file.ReadInt(),
                    NetworkAvailability = file.ReadLine(),
                    CarFLIFilename = file.ReadLine(),
                    CarFilename = file.ReadLine(),
                    DriveFLIFilename = file.ReadLine(),
                    NumberOfTextChunks = file.ReadInt()
                };
                if (opponent.NumberOfTextChunks == 2)
                {
                    opponent.Section1TextOffset = file.ReadVector2();
                    opponent.Section1Frames = file.ReadVector2();
                    opponent.Section1NumberOfLines = file.ReadInt();
                    opponent.TopSpeed = file.ReadLine();
                    opponent.KerbWeight = file.ReadLine();
                    opponent.To60 = file.ReadLine();
                    for (int j = 3; j < opponent.Section1NumberOfLines; j++)
                    {
                        file.ReadLine();
                    }
                    opponent.Section2TextOffset = file.ReadVector2();
                    opponent.Section2Frames = file.ReadVector2();
                    opponent.Section2NumberOfLines = file.ReadInt();
                    for (int j = 0; j < opponent.Section2NumberOfLines; j++)
                    {
                        opponent.Bio = $"{opponent.Bio}{file.ReadLine()} ";
                    }
                }


                opponents.Opponents.Add(opponent);
            }

            return opponents;
        }
    }
}
