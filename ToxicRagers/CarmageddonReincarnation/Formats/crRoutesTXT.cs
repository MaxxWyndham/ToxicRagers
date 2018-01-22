using System;
using System.Collections.Generic;

using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class Routes
    {
        List<string> raceNames;
        List<string> raceWriteups;
        List<string> raceImages;
        List<string> raceBackgrounds;
        List<string> races;
        List<AINode> nodes;
        List<AILink> links;

        public List<string> RaceNames
        {
            get => raceNames;
            set => raceNames = value;
        }

        public List<string> RaceWriteups
        {
            get => raceWriteups;
            set => raceWriteups = value;
        }

        public List<string> RaceImages
        {
            get => raceImages;
            set => raceImages = value;
        }

        public List<string> RaceBackgrounds
        {
            get => raceBackgrounds;
            set => raceBackgrounds = value;
        }

        public List<string> Races
        {
            get => races;
            set => races = value;
        }

        public List<AINode> Nodes
        {
            get => nodes;
            set => nodes = value;
        }

        public List<AILink> Links
        {
            get => links;
            set => links = value;
        }

        public Routes()
        {
            raceNames = new List<string>();
            raceWriteups = new List<string>();
            raceImages = new List<string>();
            raceBackgrounds = new List<string>();
            races = new List<string>();
            nodes = new List<AINode>();
            links = new List<AILink>();
        }

        public static Routes Load(string pathToFile)
        {
            Routes routes = new Routes();

            using (DocumentParser doc = new DocumentParser(pathToFile))
            {
                string line = doc.ReadNextLine();

                while (line != null)
                {
                    switch (line)
                    {
                        case "[LUMP]":
                            doc.ReadNextLine();  // level
                            line = doc.ReadNextLine();
                            break;

                        case "[ENVIRONMENT]":
                            doc.ReadNextLine();
                            line = doc.ReadNextLine();
                            break;

                        case "[RACE_NAMES]":
                            while (!doc.NextLineIsASection())
                            {
                                routes.raceNames.Add(doc.ReadNextLine());
                            }

                            line = doc.ReadNextLine();
                            break;

                        case "[RACE_WRITEUP]":
                            while (!doc.NextLineIsASection())
                            {
                                routes.raceWriteups.Add(doc.ReadNextLine());
                            }

                            line = doc.ReadNextLine();
                            break;

                        case "[RACE_IMAGES]":
                            while (!doc.NextLineIsASection())
                            {
                                routes.raceImages.Add(doc.ReadNextLine());
                            }

                            line = doc.ReadNextLine();
                            break;

                        case "[RACE_BACKGROUNDS]":
                            while (!doc.NextLineIsASection())
                            {
                                routes.raceBackgrounds.Add(doc.ReadNextLine());
                            }

                            line = doc.ReadNextLine();
                            break;

                        case "[VERSION]":
                            doc.ReadNextLine();  // 2.500000
                            line = doc.ReadNextLine();
                            break;

                        case "[RACE_LAYERS]":
                            line = doc.SkipToNextSection();
                            break;

                        case "[LUA_SCRIPTS]":
                            line = doc.SkipToNextSection();
                            break;

                        case "[RACES]":
                            while (!doc.NextLineIsASection())
                            {
                                routes.races.Add(doc.ReadNextLine());
                            }

                            line = doc.ReadNextLine();
                            break;

                        case "[AINODE]":
                            bool bAINode = true;
                            AINode node = new AINode();

                            while (bAINode)
                            {
                                line = doc.ReadNextLine();

                                switch (line)
                                {
                                    case "<INDEX>":
                                        node.Index = doc.ReadInt();
                                        break;

                                    case "<TYPE>":
                                        node.Type = doc.ReadInt();
                                        break;

                                    case "<RADIUS>":
                                        node.Radius = doc.ReadFloat();
                                        break;

                                    case "<POS>":
                                        node.Position = doc.ReadVector3();
                                        break;

                                    case "<RACE_LINE>":
                                        node.RaceLine = doc.ReadVector3();
                                        break;

                                    case "<RACE_LINE_OFFSET>":
                                        node.RaceLineOffset = doc.ReadFloat();
                                        break;

                                    default:
                                        bAINode = false;
                                        routes.nodes.Add(node);
                                        if (line != null && !line.StartsWith("[")) { Console.WriteLine("Unexpected [AINODE] line: " + line); }
                                        break;
                                }
                            }
                            break;

                        case "[AILINK]":
                            bool bAILink = true;
                            AILink link = new AILink();

                            while (bAILink)
                            {
                                line = doc.ReadNextLine();

                                switch (line)
                                {
                                    case "<NODES>":
                                        link.NodeA = doc.ReadInt();
                                        link.NodeB = doc.ReadInt();
                                        break;

                                    case "<WIDTH>":
                                        link.Width = doc.ReadFloat();
                                        break;

                                    case "<VALUE>":
                                        link.Value = doc.ReadNextLine();
                                        break;

                                    case "<ONEWAY>":
                                        link.OneWay = true;
                                        break;

                                    case "<TYPE>":
                                        for (int i = 0; i < routes.races.Count; i++)
                                        {
                                            link.Types.Add(doc.ReadInt());
                                        }
                                        break;

                                    case "<RACE_VALUE>":
                                        link.RaceValueAmount = doc.ReadInt();
                                        link.RaceValue = doc.ReadNextLine();
                                        break;

                                    default:
                                        bAILink = false;
                                        routes.links.Add(link);
                                        if (line != null && !line.StartsWith("[")) { throw new NotImplementedException("Unexpected [AILINK] line: " + line); }
                                        break;
                                }
                            }
                            break;

                        default:
                            Console.WriteLine(pathToFile);
                            throw new NotImplementedException("Unexpected [SECTION]: " + line);
                    }
                }
            }

            return routes;
        }
    }

    public class AINode
    {
        int index;
        int type;
        float radius;
        Vector3 position;
        Vector3 raceLine;
        float raceLineOffset;

        public int Index
        {
            get => index;
            set => index = value;
        }

        public int Type
        {
            get => type;
            set => type = value;
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        public Vector3 RaceLine
        {
            get => raceLine;
            set => raceLine = value;
        }

        public float RaceLineOffset
        {
            get => raceLineOffset;
            set => raceLineOffset = value;
        }
    }

    public class AILink
    {
        int nodeA;
        int nodeB;
        string name;
        float width;
        bool oneWay;
        int raceValueAmount;
        string raceValue;
        List<int> types;

        public AILink()
        {
            types = new List<int>();
            oneWay = false;
        }

        public int NodeA
        {
            get => nodeA;
            set => nodeA = value;
        }

        public int NodeB
        {
            get => nodeB;
            set => nodeB = value;
        }

        public string Value
        {
            get => name;
            set => name = value;
        }

        public float Width
        {
            get => width;
            set => width = value;
        }

        public int RaceValueAmount
        {
            get => raceValueAmount;
            set => raceValueAmount = value;
        }

        public string RaceValue
        {
            get => raceValue;
            set => raceValue = value;
        }

        public List<int> Types
        {
            get => types;
            set => types = value;
        }

        public bool OneWay
        {
            get => oneWay;
            set => oneWay = value;
        }
    }
}