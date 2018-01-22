using System;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class OpponentPathParser
    {
        public OpponentPath OpponentPath;

        public bool Load(string pathToFile)
        {
            StreamReader sr = new StreamReader(pathToFile);

            while (getNextLine(sr) != "START OF OPPONENT PATHS") { if (sr.EndOfStream) { return true; } }

            Console.WriteLine(pathToFile);

            OpponentPath = new OpponentPath();
            string s = getNextLine(sr);

            int nodeCount = Convert.ToInt32(s);
            OpponentPath.PathNodeCount = nodeCount;

            for (int i = 0; i < nodeCount; i++)
            {
                s = getNextLine(sr);
                OpponentPath.PathNodes[i] = new OpponentPathNode(Vector3.Parse(s));
            }

            s = getNextLine(sr);
            int sectionCount = Convert.ToInt32(s);
            OpponentPath.PathSectionCount = sectionCount;

            for (int i = 0; i < sectionCount; i++)
            {
                s = getNextLine(sr);
                OpponentPath.PathSections[i] = new OpponentPathSection(s);
            }

            s = getNextLine(sr);
            int copCount = Convert.ToInt32(s);
            OpponentPath.CopCount = copCount;

            for (int i = 0; i < copCount; i++)
            {
                s = getNextLine(sr);
                OpponentPath.Cops[i] = new OpponentPathCop(s);
            }

            s = getNextLine(sr);
            if (s != "END OF OPPONENT PATHS") { Console.WriteLine("Unexpected content : " + s); }

            sr.Close();
            return true;
        }

        private static string getNextLine(StreamReader sr)
        {
            string s = sr.ReadLine();
            if (s == null) { return s; }

            if (s.IndexOf("/") > -1) { s = s.Substring(0, s.IndexOf("/")).Trim(); } else { s = s.Trim(); }

            if (s.Length == 0)
            {
                return getNextLine(sr);
            }
            else
            {
                return s;
            }
        }
    }

    public class OpponentPath
    {
        int pathnodes;
        int pathsections;
        int cops;
        public OpponentPathNode[] PathNodes;
        public OpponentPathSection[] PathSections;
        public OpponentPathCop[] Cops;

        public int PathNodeCount
        {
            get => pathnodes;
            set
            {
                PathNodes = new OpponentPathNode[value];
                pathnodes = value;
            }
        }

        public int PathSectionCount
        {
            get => pathsections;
            set
            {
                PathSections = new OpponentPathSection[value];
                pathsections = value;
            }
        }

        public int CopCount
        {
            get => cops;
            set
            {
                Cops = new OpponentPathCop[value];
                cops = value;
            }
        }

        public int MinSectionNode
        {
            get
            {
                int x = int.MaxValue;

                for (int i = 0; i < pathsections; i++)
                {
                    if (PathSections[i].StartNode < x) { x = PathSections[i].StartNode; }
                    if (PathSections[i].EndNode < x) { x = PathSections[i].EndNode; }
                }

                return x;
            }
        }

        public int MaxSectionNode
        {
            get
            {
                int x = int.MinValue;

                for (int i = 0; i < pathsections; i++)
                {
                    if (PathSections[i].StartNode > x) { x = PathSections[i].StartNode; }
                    if (PathSections[i].EndNode > x) { x = PathSections[i].EndNode; }
                }

                return x;
            }
        }
    }

    public class OpponentPathNode
    {
        public Vector3 Position;

        public OpponentPathNode(Vector3 position)
        {
            Position = position;
        }
    }

    public class OpponentPathSection
    {
        public int StartNode = -1;
        public int EndNode = -1;
        public int SectionType = -1;

        public OpponentPathSection(string line)
        {
            string[] s = line.Split(',');

            StartNode = s[0].ToInt();
            EndNode = s[1].ToInt();
            // 2, 3 = start path min, max speed
            // 4, 5 = end path min, max speed
            // 6    = width
            SectionType = s[7].ToInt();

            switch (SectionType)
            {
                case 0:
                case 1:
                case 2:
                case 1000:
                case 1001:
                    // 0    = roam path
                    // 1    = race path
                    // 1000 = ???
                    // 1001 = ???
                    break;

                default:
                    Console.WriteLine("Unknown section type: " + SectionType);
                    break;
            }
        }
    }

    public class OpponentPathCop
    {
        public Vector3 Position;

        public OpponentPathCop()
        {
        }

        public OpponentPathCop(string line)
        {
            string[] s = line.Split(","[0]);
            Position = Vector3.Parse(s[0] + "," + s[1] + "," + s[2]);
            // 3, 4, 5 are used for setting squad car or suppressor
        }
    }
}