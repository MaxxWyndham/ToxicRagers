using System;
using System.Globalization;
using System.IO;
using ToxicRagers.Helpers;
using ToxicRagers.Carmageddon.Helpers;

namespace ToxicRagers.Carmageddon.Formats
{
    class c1CarTXT
    {
        // A class for reading (and eventually saving) C1 Car.txt files (Usually located in \Carmageddon\DATA\CARS\ )

        public static void Load(string pathToFile, c1Car car)
        {
            StreamReader sr = new StreamReader(pathToFile);
            CultureInfo culture = new CultureInfo("en-GB");
            string[] s;

            while (!sr.EndOfStream)
            {
                if (!TestLine(car.Name.ToUpper() + ".TXT", getNextLine(sr))) { break; }

                if (!TestLine("START OF DRIVABLE STUFF", getNextLine(sr))) { break; }
                car.DriverHeadPosition = Vector3.Parse(getNextLine(sr));
                car.DriverHeadTurnAngles = Vector2.Parse(getNextLine(sr));

                s = getNextLine(sr).Split(","[0]);
                car.MirrorCamOffset = Vector3.Parse(s[0] + "," + s[1] + "," + s[2]);
                car.MirrorCamViewingAngle = Convert.ToInt32(s[3]);

                s = getNextLine(sr).Split(","[0]);
                for (int i = 0; i < 4; i++) { car.PratcamBorders[i] = s[i]; }

                if (!TestLine("END OF DRIVABLE STUFF", getNextLine(sr))) { break; }

                getNextLine(sr); // Engine noises
                car.StealWorthy = !getNextLine(sr).Contains("not");

                // This next block is all about impacts, 6 blocks in the order of top, bottom, left, right, front, back
                for (int i = 0; i < 6; i++)
                {
                    int j = Convert.ToInt32(getNextLine(sr));
                    for (int k = 0; k < j; k++)
                    {
                        getNextLine(sr); // condition
                        int l = Convert.ToInt32(getNextLine(sr)); // systems count
                        for (int m = 0; m < l; m++)
                        {
                            getNextLine(sr); // damage
                        }
                    }
                }

                getNextLine(sr); // grid images

                // three pix blocks
                for (int i = 0; i < 3; i++)
                {
                    int j = Convert.ToInt32(getNextLine(sr));
                    for (int k = 0; k < j; k++)
                    {
                        getNextLine(sr); // pix file
                    }
                }

                int n = Convert.ToInt32(getNextLine(sr));
                for (int k = 0; k < n; k++)
                {
                    getNextLine(sr); // shadetable
                }

                // three mat blocks
                for (int i = 0; i < 3; i++)
                {
                    int j = Convert.ToInt32(getNextLine(sr));
                    for (int k = 0; k < j; k++)
                    {
                        getNextLine(sr); // mat file
                    }
                }

                car.ModelCount = Convert.ToInt32(getNextLine(sr));
                for (int k = 0; k < car.ModelCount; k++)
                {
                    car.LoadModel(k, getNextLine(sr)); // model file
                }

                car.ActorCount = Convert.ToInt32(getNextLine(sr));
                for (int k = 0; k < car.ActorCount; k++)
                {
                    s = getNextLine(sr).Split(","[0]);
                    car.LoadActor(k, s[1]);
                }

                getNextLine(sr); // reflective screen material

                n = Convert.ToInt32(getNextLine(sr));
                for (int k = 0; k < n; k++)
                {
                    getNextLine(sr); // GroovyFunkRef for steerable wheel
                }

                getNextLine(sr); // Left-front suspension parts GroovyFunkRef
                getNextLine(sr); // Right-front suspension parts GroovyFunkRef
                getNextLine(sr); // Left-rear suspension parts GroovyFunkRef
                getNextLine(sr); // Right-rear suspension parts GroovyFunkRef
                getNextLine(sr); // Driven wheels GroovyFunkRefs (for spinning)
                getNextLine(sr); // Non-driven wheels GroovyFunkRefs (for spinning)
                getNextLine(sr); // Driven wheels diameter
                getNextLine(sr); // Non-driven wheels diameter

                if (!TestLine("START OF FUNK", getNextLine(sr))) { break; }
                while (getNextLine(sr) != "END OF FUNK") { }
                //if (!TestLine("END OF FUNK", getNextLine(sr))) { break; }

                if (!TestLine("START OF GROOVE", getNextLine(sr))) { break; }
                while (getNextLine(sr) != "END OF GROOVE") { }
                //if (!TestLine("END OF GROOVE", getNextLine(sr))) { break; }

                // Crush data, one block per model
                for (int i = 0; i < car.ModelCount; i++)
                {
                    car.Crushes[i] = new CrushData();
                    car.Crushes[i].UnknownA = Convert.ToSingle(getNextLine(sr), culture);
                    car.Crushes[i].UnknownB = Vector2.Parse(getNextLine(sr));
                    car.Crushes[i].UnknownC = Convert.ToSingle(getNextLine(sr), culture);
                    car.Crushes[i].UnknownD = Convert.ToSingle(getNextLine(sr), culture);
                    car.Crushes[i].UnknownE = Convert.ToSingle(getNextLine(sr), culture);
                    car.Crushes[i].UnknownF = Convert.ToSingle(getNextLine(sr), culture);
                    car.Crushes[i].SetChunkCount(Convert.ToInt32(getNextLine(sr)));

                    for (int j = 0; j < car.Crushes[i].ChunkCount; j++)
                    {
                        car.Crushes[i].Chunks[j] = new CrushChunk();
                        car.Crushes[i].Chunks[j].RootVertex = Convert.ToInt32(getNextLine(sr));
                        car.Crushes[i].Chunks[j].A = Vector3.Parse(getNextLine(sr));
                        car.Crushes[i].Chunks[j].B = Vector3.Parse(getNextLine(sr));
                        car.Crushes[i].Chunks[j].C = Vector3.Parse(getNextLine(sr));
                        car.Crushes[i].Chunks[j].D = Vector3.Parse(getNextLine(sr));

                        car.Crushes[i].Chunks[j].SetChunkEntryCount(Convert.ToInt32(getNextLine(sr)));

                        int lastVert = -1;
                        for (int k = 0; k < car.Crushes[i].Chunks[j].ChunkEntryCount; k++)
                        {
                            car.Crushes[i].Chunks[j].CrushVerts[k] = new CrushChunkEntry();
                            lastVert += Convert.ToInt32(getNextLine(sr));

                            car.Crushes[i].Chunks[j].CrushVerts[k].VertIndex = lastVert;
                            car.Crushes[i].Chunks[j].CrushVerts[k].Weight = Convert.ToInt32(getNextLine(sr));
                        }
                    }
                }

                int version = 0;
                if (!TestLine("START OF MECHANICS STUFF version ", getNextLine(sr), out version)) { break; }
                getNextLine(sr);    // left rear wheel position
                getNextLine(sr);    // right rear
                getNextLine(sr);    // left front wheel position
                getNextLine(sr);    // right front
                getNextLine(sr);    // centre of mass position
                switch (version)
                {
                    case 2:
                        getNextLine(sr);
                        getNextLine(sr);
                        getNextLine(sr);
                        break;
                    case 3:
                    case 4:
                        getNextLine(sr);
                        getNextLine(sr);
                        n = Convert.ToInt32(getNextLine(sr));
                        for (int k = 0; k < n; k++)
                        {
                            getNextLine(sr); // extra point
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown version " + version);
                        break;
                }
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                getNextLine(sr);
                if (!TestLine("END OF MECHANICS STUFF", getNextLine(sr))) { break; }

                n = Convert.ToInt32(getNextLine(sr));
                for (int k = 0; k < n; k++)
                {
                    getNextLine(sr); // shrapnel material
                }

                if (!sr.EndOfStream)
                {
                    // probably fire points, there are 12 of them
                    for (int i = 0; i < 12; i++)
                    {
                        getNextLine(sr); // fire point
                    }
                }
            }

            sr.Close();
        }

        private static string getNextLine(StreamReader sr)
        {
            string s = sr.ReadLine();
            if (s.IndexOf("/") > -1) { s = s.Substring(0, s.IndexOf("/")).Trim(); }

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
