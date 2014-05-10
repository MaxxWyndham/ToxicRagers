using System;
using System.Collections.Generic;
using ToxicRagers.Helpers;
using ToxicRagers.CarmageddonReincarnation.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class Routes
    {
        List<string> races;
        List<AINode> nodes;
        List<AILink> links;

        public List<string> Races
        {
            get { return races; }
            set { races = value; }
        }

        public List<AINode> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        public List<AILink> Links
        {
            get { return links; }
            set { links = value; }
        }

        public Routes()
        {
            races = new List<string>();
            nodes = new List<AINode>();
            links = new List<AILink>();
        }

        public static Routes Load(string pathToFile)
        {
            Routes routes = new Routes();

            using (var doc = new DocumentParser(pathToFile))
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
                            var node = new AINode();

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
                            var link = new AILink();

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

                                    case "<TYPE>":
                                        for (int i = 0; i < routes.races.Count; i++)
                                        {
                                            link.Types.Add(doc.ReadInt());
                                        }
                                        break;

                                    default:
                                        bAILink = false;
                                        routes.links.Add(link);
                                        if (line != null && !line.StartsWith("[")) { Console.WriteLine("Unexpected [AILINK] line: " + line); }
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
            get { return index; }
            set { index = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 RaceLine
        {
            get { return raceLine; }
            set { raceLine = value; }
        }

        public float RaceLineOffset
        {
            get { return raceLineOffset; }
            set { raceLineOffset = value; }
        }
    }

    public class AILink
    {
        int nodeA;
        int nodeB;
        string name;
        float width;
        List<int> types;

        public AILink()
        {
            types = new List<int>();
        }

        public int NodeA
        {
            get { return nodeA; }
            set { nodeA = value; }
        }

        public int NodeB
        {
            get { return nodeB; }
            set { nodeB = value; }
        }

        public string Value
        {
            get { return name; }
            set { name = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public List<int> Types
        {
            get { return types; }
            set { types = value; }
        }
    }
}
