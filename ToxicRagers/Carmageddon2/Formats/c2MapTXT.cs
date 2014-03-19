using System;
using System.Globalization;
using System.IO;
using ToxicRagers.Helpers;
using ToxicRagers.Carmageddon2.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    class c2MapTXT
    {
        // A class for reading (and eventually saving) C2 Map.txt files (Usually located in \Carmageddon\DATA\CARS\ )

        public static void Load(string pathToFile, c2Map map)
        {
            if (pathToFile.ToLower().EndsWith("nuke.txt")) { return; }
            StreamReader sr = new StreamReader(pathToFile);
            CultureInfo culture = new CultureInfo("en-GB");
            string[] s;

            while (!sr.EndOfStream)
            {
                int version = 0;
                if (!TestLine("VERSION ", getNextLine(sr), out version)) { break; }

                if (version == 1)
                {
                    // V1 has global lighting data
                    getNextLine(sr); // RGB for main directional light-source
                    getNextLine(sr); // Ambient/Diffuse light to be used when plaything ambient says 0
                    getNextLine(sr); // Ambient/Diffuse light to be used when plaything ambient says 1
                    getNextLine(sr); // Ambient/Diffuse light to be used when plaything ambient says anything else
                }

                getNextLine(sr); // Position of centre of start of grid
                getNextLine(sr); // Direction that grid faces in

                string t = getNextLine(sr);

                if (version == 8)
                {
                    // alpha and demo v8 files had an extra block here, we'll test if it
                    if (t.Contains(","))
                    {
                        getNextLine(sr); // # laps
                        getNextLine(sr); // Race completed bonus (all laps raced) for each skill level
                        getNextLine(sr); // Race completed bonus (all peds killed) for each skill level
                        getNextLine(sr); // Race completed bonus (all oppos wasted) for each skill level

                        t = getNextLine(sr);
                    }
                }

                int i = Convert.ToInt32(t);
                for (int k = 0; k < i; k++)
                {
                    getNextLine(sr); // Timer increment for each skill level (ped mode)
                    if (!TestLine("1", getNextLine(sr))) { break; }
                    getNextLine(sr); // Point #0
                    getNextLine(sr); // Point #1
                    getNextLine(sr); // Point #2
                    getNextLine(sr); // Point #3
                }

                i = Convert.ToInt32(getNextLine(sr)); // Number of smash specs

                if (i > 0) { Console.WriteLine(pathToFile); }
                for (int k = 0; k < i; k++)
                {
                    int flags = Convert.ToInt32(getNextLine(sr));
                    string trigger = getNextLine(sr);
                    if (trigger.Length == 3 && trigger.StartsWith("&")) { if (!TestLine("7", getNextLine(sr))) { Console.WriteLine("Noncar smash"); return; } }
                    string mode = getNextLine(sr);

                    switch (mode)
                    {
                        case "remove":
                            Single removalthreshold = Convert.ToSingle(getNextLine(sr), culture);
                            if (!processConnotations(sr)) { return; }
                            break;

                        case "nochange":
                            Single threshold = Convert.ToSingle(getNextLine(sr), culture);
                            if (!processConnotations(sr)) { return; }
                            break;

                        case "replacemodel":
                            Single replacethreshold = Convert.ToSingle(getNextLine(sr), culture);
                            if (!processConnotations(sr)) { return; }
                            string newmodel = getNextLine(sr);
                            int chanceoffire = Convert.ToInt32(getNextLine(sr));
                            if (chanceoffire > 0)
                            {
                                int numfires = Convert.ToInt32(getNextLine(sr));
                                Vector2 smokeindex = Vector2.Parse(getNextLine(sr));
                            }
                            break;

                        case "texturechange":
                            string intactpixelmap = getNextLine(sr);
                            int numlevels = Convert.ToInt32(getNextLine(sr));
                            for (int j = 0; j < numlevels; j++)
                            {
                                Single triggerthreshold = Convert.ToSingle(getNextLine(sr), culture);
                                int texchangeflags = Convert.ToInt32(getNextLine(sr));
                                string collisiontype = getNextLine(sr);

                                if (!processConnotations(sr)) { return; }

                                int numpixelmaps = Convert.ToInt32(getNextLine(sr));
                                for (int l = 0; l < numpixelmaps; l++)
                                {
                                    string pixelmap = getNextLine(sr);
                                }
                            }
                            break;

                        default:
                            Console.WriteLine("Unknown mode : " + mode);
                            return;
                    }

                    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 1"); return; }
                    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 2"); return; }
                    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 3"); return; }
                    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 4"); return; }
                }

                break;
                //if (!TestLine(car.Name.ToUpper() + ".TXT", getNextLine(sr))) { break; }

                //if (!TestLine("START OF DRIVABLE STUFF", getNextLine(sr))) { break; }
                //car.DriverHeadPosition = Vector3.Parse(getNextLine(sr));
                //car.DriverHeadTurnAngles = Vector2.Parse(getNextLine(sr));

                //s = getNextLine(sr).Split(","[0]);
                //car.MirrorCamOffset = Vector3.Parse(s[0] + "," + s[1] + "," + s[2]);
                //car.MirrorCamViewingAngle = Convert.ToInt32(s[3]);

                //s = getNextLine(sr).Split(","[0]);
                //for (int i = 0; i < 4; i++) { car.PratcamBorders[i] = s[i]; }

                //if (!TestLine("END OF DRIVABLE STUFF", getNextLine(sr))) { break; }

                //getNextLine(sr); // Engine noises
                //car.StealWorthy = !getNextLine(sr).Contains("not");

                //// This next block is all about impacts, 6 blocks in the order of top, bottom, left, right, front, back
                //for (int i = 0; i < 6; i++)
                //{
                //    int j = Convert.ToInt32(getNextLine(sr));
                //    for (int k = 0; k < j; k++)
                //    {
                //        getNextLine(sr); // condition
                //        int l = Convert.ToInt32(getNextLine(sr)); // systems count
                //        for (int m = 0; m < l; m++)
                //        {
                //            getNextLine(sr); // damage
                //        }
                //    }
                //}

                //getNextLine(sr); // grid images

                //// three pix blocks
                //for (int i = 0; i < 3; i++)
                //{
                //    int j = Convert.ToInt32(getNextLine(sr));
                //    for (int k = 0; k < j; k++)
                //    {
                //        getNextLine(sr); // pix file
                //    }
                //}

                //int n = Convert.ToInt32(getNextLine(sr));
                //for (int k = 0; k < n; k++)
                //{
                //    getNextLine(sr); // shadetable
                //}

                //// three mat blocks
                //for (int i = 0; i < 3; i++)
                //{
                //    int j = Convert.ToInt32(getNextLine(sr));
                //    for (int k = 0; k < j; k++)
                //    {
                //        getNextLine(sr); // mat file
                //    }
                //}

                //car.ModelCount = Convert.ToInt32(getNextLine(sr));
                //for (int k = 0; k < car.ModelCount; k++)
                //{
                //    car.LoadModel(k, getNextLine(sr)); // model file
                //}

                //car.ActorCount = Convert.ToInt32(getNextLine(sr));
                //for (int k = 0; k < car.ActorCount; k++)
                //{
                //    s = getNextLine(sr).Split(","[0]);
                //    car.LoadActor(k, s[1]);
                //}

                //getNextLine(sr); // reflective screen material

                //n = Convert.ToInt32(getNextLine(sr));
                //for (int k = 0; k < n; k++)
                //{
                //    getNextLine(sr); // GroovyFunkRef for steerable wheel
                //}

                //getNextLine(sr); // Left-front suspension parts GroovyFunkRef
                //getNextLine(sr); // Right-front suspension parts GroovyFunkRef
                //getNextLine(sr); // Left-rear suspension parts GroovyFunkRef
                //getNextLine(sr); // Right-rear suspension parts GroovyFunkRef
                //getNextLine(sr); // Driven wheels GroovyFunkRefs (for spinning)
                //getNextLine(sr); // Non-driven wheels GroovyFunkRefs (for spinning)
                //getNextLine(sr); // Driven wheels diameter
                //getNextLine(sr); // Non-driven wheels diameter

                //if (!TestLine("START OF FUNK", getNextLine(sr))) { break; }
                //while (getNextLine(sr) != "END OF FUNK") { }
                ////if (!TestLine("END OF FUNK", getNextLine(sr))) { break; }

                //if (!TestLine("START OF GROOVE", getNextLine(sr))) { break; }
                //while (getNextLine(sr) != "END OF GROOVE") { }
                ////if (!TestLine("END OF GROOVE", getNextLine(sr))) { break; }

                //// Crush data, one block per model
                //for (int i = 0; i < car.ModelCount; i++)
                //{
                //    car.Crushes[i] = new CrushData();
                //    car.Crushes[i].UnknownA = Convert.ToSingle(getNextLine(sr), culture);
                //    car.Crushes[i].UnknownB = Vector2.Parse(getNextLine(sr));
                //    car.Crushes[i].UnknownC = Convert.ToSingle(getNextLine(sr), culture);
                //    car.Crushes[i].UnknownD = Convert.ToSingle(getNextLine(sr), culture);
                //    car.Crushes[i].UnknownE = Convert.ToSingle(getNextLine(sr), culture);
                //    car.Crushes[i].UnknownF = Convert.ToSingle(getNextLine(sr), culture);
                //    car.Crushes[i].SetChunkCount(Convert.ToInt32(getNextLine(sr)));

                //    for (int j = 0; j < car.Crushes[i].ChunkCount; j++)
                //    {
                //        car.Crushes[i].Chunks[j] = new CrushChunk();
                //        car.Crushes[i].Chunks[j].RootVertex = Convert.ToInt32(getNextLine(sr));
                //        car.Crushes[i].Chunks[j].A = Vector3.Parse(getNextLine(sr));
                //        car.Crushes[i].Chunks[j].B = Vector3.Parse(getNextLine(sr));
                //        car.Crushes[i].Chunks[j].C = Vector3.Parse(getNextLine(sr));
                //        car.Crushes[i].Chunks[j].D = Vector3.Parse(getNextLine(sr));

                //        car.Crushes[i].Chunks[j].SetChunkEntryCount(Convert.ToInt32(getNextLine(sr)));

                //        int lastVert = -1;
                //        for (int k = 0; k < car.Crushes[i].Chunks[j].ChunkEntryCount; k++)
                //        {
                //            car.Crushes[i].Chunks[j].CrushVerts[k] = new CrushChunkEntry();
                //            lastVert += Convert.ToInt32(getNextLine(sr));

                //            car.Crushes[i].Chunks[j].CrushVerts[k].VertIndex = lastVert;
                //            car.Crushes[i].Chunks[j].CrushVerts[k].Weight = Convert.ToInt32(getNextLine(sr));
                //        }
                //    }
                //}

                //int version = 0;
                //if (!TestLine("START OF MECHANICS STUFF version ", getNextLine(sr), out version)) { break; }
                //getNextLine(sr);    // left rear wheel position
                //getNextLine(sr);    // right rear
                //getNextLine(sr);    // left front wheel position
                //getNextLine(sr);    // right front
                //getNextLine(sr);    // centre of mass position
                //switch (version)
                //{
                //    case 2:
                //        getNextLine(sr);
                //        getNextLine(sr);
                //        getNextLine(sr);
                //        break;
                //    case 3:
                //    case 4:
                //        getNextLine(sr);
                //        getNextLine(sr);
                //        n = Convert.ToInt32(getNextLine(sr));
                //        for (int k = 0; k < n; k++)
                //        {
                //            getNextLine(sr); // extra point
                //        }
                //        break;
                //    default:
                //        Console.WriteLine("Unknown version " + version);
                //        break;
                //}
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //getNextLine(sr);
                //if (!TestLine("END OF MECHANICS STUFF", getNextLine(sr))) { break; }

                //n = Convert.ToInt32(getNextLine(sr));
                //for (int k = 0; k < n; k++)
                //{
                //    getNextLine(sr); // shrapnel material
                //}

                //if (!sr.EndOfStream)
                //{
                //    // probably fire points, there are 12 of them
                //    for (int i = 0; i < 12; i++)
                //    {
                //        getNextLine(sr); // fire point
                //    }
                //}
            }

            sr.Close();
        }

        private static void processInitialVelocity(StreamReader sr)
        {
            CultureInfo culture = new CultureInfo("en-GB");
            Vector2 towardsyouspeed = Vector2.Parse(getNextLine(sr));
            Single impacteevelocityfactor = Convert.ToSingle(getNextLine(sr), culture);
            Single randomvelocity = Convert.ToSingle(getNextLine(sr), culture);
            Single randomupvelocity = Convert.ToSingle(getNextLine(sr), culture);
            Single randomnormalvelocity = Convert.ToSingle(getNextLine(sr), culture);
            Single randomspinrate = Convert.ToSingle(getNextLine(sr), culture);
        }

        private static bool processConnotations(StreamReader sr)
        {
            CultureInfo culture = new CultureInfo("en-GB");

            int numsounds = Convert.ToInt32(getNextLine(sr));
            for (int l = 0; l < numsounds; l++)
            {
                string soundid = getNextLine(sr);
            }

            int numshrapnel = Convert.ToInt32(getNextLine(sr));
            for (int l = 0; l < numshrapnel; l++)
            {
                string shrapneltype = getNextLine(sr);

                processInitialVelocity(sr);

                if (shrapneltype != "shards")
                {
                    string initialposition = getNextLine(sr);
                    switch (initialposition)
                    {
                        case "actorbased":
                            // do nothing
                            break;

                        case "sphereclumped":
                            Single clumpingradius = Convert.ToSingle(getNextLine(sr), culture);
                            string clumpingcentre = getNextLine(sr);
                            break;

                        default:
                            Console.WriteLine("Unknown initial position : " + initialposition);
                            return false;
                    }
                }

                if (shrapneltype != "noncars") { Vector2 time = Vector2.Parse(getNextLine(sr)); }

                switch (shrapneltype)
                {
                    case "ghostparts":
                        string t = getNextLine(sr);
                        if (t.Contains(","))
                        {
                            Vector2 vghostpartcount = Vector2.Parse(t);
                        }
                        else
                        {
                            int ighostpartcount = Convert.ToInt32(t);
                        }
                        int numghosts = Convert.ToInt32(getNextLine(sr));
                        if (numghosts > 0)
                        {
                            for (int m = 0; m < numghosts; m++)
                            {
                                string actorname = getNextLine(sr);
                            }
                        }
                        else
                        {
                            string shrapnelactor = getNextLine(sr);
                        }
                        break;

                    case "noncars":
                        Vector2 noncarcount = Vector2.Parse(getNextLine(sr));
                        int chanceoffire = Convert.ToInt32(getNextLine(sr));
                        if (chanceoffire > 0)
                        {
                            int numfires = Convert.ToInt32(getNextLine(sr));
                            Vector2 smokeindex = Vector2.Parse(getNextLine(sr));
                        }
                        string actorhierarchy = getNextLine(sr);
                        int numactors = Convert.ToInt32(getNextLine(sr));
                        for (int m = 0; m < numactors; m++)
                        {
                            string actorname = getNextLine(sr);
                            string actorfile = getNextLine(sr);
                        }
                        break;

                    case "shards":
                        Single mincutlength = Convert.ToSingle(getNextLine(sr), culture);
                        if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Shards flags"); return false; }
                        string shardmaterial = getNextLine(sr);
                        break;

                    default:
                        Console.WriteLine("Unknown shrapnel type : " + shrapneltype);
                        return false;
                }
            }

            int numexplosions = Convert.ToInt32(getNextLine(sr));
            for (int l = 0; l < numexplosions; l++)
            {
                Vector2 explosionettecount = Vector2.Parse(getNextLine(sr));
                Vector2 starttime = Vector2.Parse(getNextLine(sr));
                Vector3 explosionoffset = Vector3.Parse(getNextLine(sr));
                Vector2 xfactor = Vector2.Parse(getNextLine(sr));
                Vector2 yfactor = Vector2.Parse(getNextLine(sr));
                Vector2 zfactor = Vector2.Parse(getNextLine(sr));
                Vector2 framerate = Vector2.Parse(getNextLine(sr));
                Vector2 scalingfactor = Vector2.Parse(getNextLine(sr));
                string rotationmode = getNextLine(sr);
                int numframes = Convert.ToInt32(getNextLine(sr));
                for (int m = 0; m < numframes; m++)
                {
                    Single opacity = Convert.ToSingle(getNextLine(sr), culture);
                    string pixname = getNextLine(sr);
                }
            }

            if (!TestLine("none", getNextLine(sr))) { Console.WriteLine("Slick"); return false; }

            int numnoncarcuboids = Convert.ToInt32(getNextLine(sr));
            for (int l = 0; l < numnoncarcuboids; l++)
            {
                Vector2 cuboidtime = Vector2.Parse(getNextLine(sr));
                string coordsystem = getNextLine(sr);
                string noncarindex = getNextLine(sr);
                Vector3 min = Vector3.Parse(getNextLine(sr));
                Vector3 max = Vector3.Parse(getNextLine(sr));
                processInitialVelocity(sr);
            }

            int numsmashcuboids = Convert.ToInt32(getNextLine(sr));
            for (int l = 0; l < numsmashcuboids; l++)
            {
                Vector2 cuboidtime = Vector2.Parse(getNextLine(sr));
                string triggertype = getNextLine(sr);
                string coordsystem = getNextLine(sr);
                Vector3 min = Vector3.Parse(getNextLine(sr));
                Vector3 max = Vector3.Parse(getNextLine(sr));
                string direction = getNextLine(sr);
                Single force = Convert.ToSingle(getNextLine(sr), culture);
            }

            if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Extension flags"); return false; }

            string roomturnoncode = getNextLine(sr);
            if (roomturnoncode != "0" && roomturnoncode != "14") { Console.WriteLine("Unknown room turn on code : " + roomturnoncode); }

            string awardcode = getNextLine(sr);
            switch (awardcode)
            {
                case "none":
                    // do nothing
                    break;

                case "doitregardless":
                case "repeated":
                case "singleshot":
                    int awardpoints = Convert.ToInt32(getNextLine(sr));
                    int awardtime = Convert.ToInt32(getNextLine(sr));
                    int hudindex = Convert.ToInt32(getNextLine(sr));
                    int fancyhudindex = Convert.ToInt32(getNextLine(sr));
                    break;

                default:
                    Console.WriteLine("Unknown award code: " + awardcode);
                    return false;
            }

            int numvariablechanges = Convert.ToInt32(getNextLine(sr));
            for (int l = 0; l < numvariablechanges; l++)
            {
                Vector2 variables = Vector2.Parse(getNextLine(sr));
            }

            return true;
        }

        private static string getNextLine(StreamReader sr)
        {
            string s = sr.ReadLine();
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

        private static bool TestLine(string expected, string received)
        {
            if (expected == received) { return true; }

            Console.WriteLine("Expected : " + expected);
            Console.WriteLine("Received : " + received);
            return false;
        }

        private static bool TestLine(string expected, string received, out int version)
        {
            version = 0;

            if (received.StartsWith(expected))
            {
                version = Convert.ToInt32(received.Replace(expected, ""));
                return true;
            }

            Console.WriteLine("Expected : " + expected);
            Console.WriteLine("Received : " + received);
            return false;
        }
    }
}
