using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public enum NetworkAvailability
    {
        eagle,
        all,
        never
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
                opponents.Opponents.Add(OpponentDetails.Load(file));
            }

            if (file.ReadLine() != "END") { return null; }

            return opponents;
        }

        public void Save(string path)
        {
            using (DocumentWriter dw = new DocumentWriter(path))
            {
                dw.WriteLine($"{Opponents.Count}");
                dw.WriteLine();

                for (int i = 0; i < Opponents.Count; i++)
                {
                    dw.WriteLine($"// Opponent {i}");
                    Opponents[i].Write(dw);
                    dw.WriteLine();
                }

                dw.WriteLine("END");
                dw.WriteLine();
            }
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

        public static OpponentDetails Load(DocumentParser file)
        {
            return new OpponentDetails
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
            };
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine(DriverName);
            dw.WriteLine(DriverShortName);
            dw.WriteLine(CarName);
            dw.WriteLine($"{StrengthRating}", "Strength rating (1-5)");
            dw.WriteLine($"{CostToBuy}", "Cost to buy it");
            dw.WriteLine($"{NetworkAvailability}", "Network availability ('eagle', or 'all')");
            dw.WriteLine(CarFilename, "Vehicle filename");
            dw.WriteLine("//vehicle description");
            dw.WriteLine(TopSpeed);
            dw.WriteLine(KerbWeight);
            dw.WriteLine(To60);
            dw.WriteLine(Bio);
        }
    }
}