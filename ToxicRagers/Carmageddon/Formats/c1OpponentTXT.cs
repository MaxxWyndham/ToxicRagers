using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Formats
{
    public enum NetworkAvailability
    {
        never,
        eagle,
        hawk,
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
                OpponentDetails opponent = new OpponentDetails
                {
                    DriverName = file.ReadLine(),
                    DriverShortName = file.ReadLine(),
                    CarNumber = file.ReadInt(),
                    StrengthRating = file.ReadInt(),
                    NetworkAvailability = file.ReadEnum<NetworkAvailability>(),
                    MugshotName = file.ReadLine(),
                    CarFilename = file.ReadLine(),
                    StolenCarFlicName = file.ReadLine()
                };

                int textChunks = file.ReadInt();

                for (int j = 0; j < textChunks; j++)
                {
                    TextChunk chunk = new TextChunk
                    {
                        Position = file.ReadVector2(),
                        Frame = file.ReadVector2()
                    };

                    int chunkLines = file.ReadInt();

                    while (chunkLines > 8) { file.ReadLine(); chunkLines--; }

                    for (int k = 0; k < chunkLines; k++)
                    {
                        chunk.Lines.Add(file.ReadLine());
                    }

                    opponent.TextChunks.Add(chunk);
                }

                opponents.Opponents.Add(opponent);
            }

            return opponents;
        }
    }

    public class OpponentDetails
    {
        public string DriverName { get; set; }

        public string DriverShortName { get; set; }

        public int CarNumber { get; set; }

        public int StrengthRating { get; set; }

        public NetworkAvailability NetworkAvailability { get; set; }

        public string MugshotName { get; set; }

        public string CarFilename { get; set; }

        public string StolenCarFlicName { get; set; }

        public List<TextChunk> TextChunks { get; set; } = new List<TextChunk>();

        // Helpers

        public string TopSpeed => TextChunks.Count > 0 && TextChunks[0].Lines.Count > 0 ? TextChunks[0].Lines[0] : null;

        public string KerbWeight => TextChunks.Count > 0 && TextChunks[0].Lines.Count > 1 ? TextChunks[0].Lines[1] : null;

        public string To60 => TextChunks.Count > 0 && TextChunks[0].Lines.Count > 2 ? TextChunks[0].Lines[2] : null;

        public string Bio => TextChunks.Count > 1 && TextChunks[1].Lines.Count > 0 ? string.Join(" ", TextChunks[1].Lines) : null;
    }

    public class TextChunk
    {
        public Vector2 Position { get; set; }

        public Vector2 Frame { get; set; }

        public List<string> Lines { get; set; } = new List<string>();
    }
}