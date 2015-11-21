using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class GrooveParser
    {
        public List<Groove> Grooves = new List<Groove>();

        public bool Load(string pathToFile)
        {
            StreamReader sr = new StreamReader(pathToFile);
            CultureInfo culture = new CultureInfo("en-GB");

            while (getNextLine(sr) != "START OF GROOVE") { if (sr.EndOfStream) { return true; } }

            string s = getNextLine(sr);
            while (s != "END OF GROOVE" && s != null)
            {
                Groove groove = new Groove();
                groove.File = pathToFile;
                groove.Actor = s;
                groove.Lollipop = s = getNextLine(sr);
                groove.Distance = s = getNextLine(sr);
                groove.Path = s = getNextLine(sr);

                if (groove.Path != "")
                {
                    groove.PathMovement = s = getNextLine(sr);
                    s = getNextLine(sr); groove.PathMovementCentre = Vector3.Parse(s);
                    s = getNextLine(sr); groove.PathMovementCyclesPerSecond = Convert.ToSingle(s, culture);
                    s = getNextLine(sr); groove.PathMovementAmount = Vector3.Parse(s);
                }

                groove.Movement = s = getNextLine(sr).ToLower();

                if (groove.Movement != "")
                {
                    groove.MovementMovement = s = getNextLine(sr).ToLower();

                    switch (groove.Movement)
                    {
                        case "spin":
                            s = getNextLine(sr);
                            if (groove.MovementMovement == "controlled")
                            {
                                groove.MovementGroovyFunkRef = Convert.ToInt32(s);
                            }
                            else
                            {
                                groove.MovementCyclesPerSecond = Convert.ToSingle(s, culture);
                            }

                            s = getNextLine(sr); groove.MovementCentre = Vector3.Parse(s);
                            groove.MovementAxis = s = getNextLine(sr);
                            break;

                        case "rock":
                            s = getNextLine(sr);
                            if (groove.MovementMovement == "absolute")
                            {
                                groove.MovementGroovyFunkRef = Convert.ToInt32(s);
                            }
                            else
                            {
                                groove.MovementCyclesPerSecond = Convert.ToSingle(s, culture);
                            }

                            s = getNextLine(sr); groove.MovementCentre = Vector3.Parse(s);
                            groove.MovementAxis = s = getNextLine(sr);
                            s = getNextLine(sr); groove.MovementDegrees = Convert.ToSingle(s, culture);
                            break;

                        case "shear":
                            s = getNextLine(sr); groove.ShearStart = Vector3.Parse(s); // If MovementMovement is absolute or controlled this will include a reference to a GroovyFunkRef
                            s = getNextLine(sr); groove.ShearCentre = Vector3.Parse(s);
                            s = getNextLine(sr); groove.ShearEnd = Vector3.Parse(s);
                            break;

                        default:
                            s = getNextLine(sr);
                            Console.WriteLine(groove.MovementMovement + "\t" + s + "\t" + pathToFile);
                            break;
                    }
                }

                while (getNextLine(sr) != "NEXT GROOVE") { if (sr.EndOfStream) { return true; } }
                s = getNextLine(sr);

                Grooves.Add(groove);

                s = getNextLine(sr);
                if (s == "NEXT GROOVE") { s = getNextLine(sr); }
            }

            sr.Close();
            return true;
        }

        private static string getNextLine(StreamReader sr)
        {
            string s = sr.ReadLine();
            if (s == null) { return s; }

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
    }

    public class Groove
    {
        string lollipop;
        string distance;
        string path;
        string pathmovement;
        string movement;
        string movementmovement;

        public string File;
        public string Actor;
        public Vector3 PathMovementCentre;
        public Single PathMovementCyclesPerSecond;
        public Vector3 PathMovementAmount;
        public int MovementGroovyFunkRef;
        public Single MovementCyclesPerSecond;
        public Vector3 MovementCentre;
        public string MovementAxis;
        public Single MovementDegrees;

        public Vector3 ShearStart;
        public Vector3 ShearCentre;
        public Vector3 ShearEnd;

        public string Lollipop
        {
            get { return lollipop; }
            set
            {
                switch (value.ToLower())
                {
                    case "not a lollipop":
                    case "not a fuckin lolliiipop":
                        //stainless version of "none"
                        break;
                    default:
                        Console.WriteLine("Unknown lollipop: " + value);
                        break;
                }

                lollipop = value;
            }
        }

        public string Distance
        {
            get { return distance; }
            set
            {
                switch (value.ToLower())
                {
                    case "constant":
                    case "distance":
                        distance = value;
                        break;

                    default:
                        Console.WriteLine("Unknown distance: " + value);
                        break;
                }
            }
        }

        public string Path
        {
            get { return path; }
            set
            {
                switch (value.ToLower())
                {
                    case "no fucking path you cuuuuuuunt":
                    case "no path":
                    case "no wankiriferous pathitudinality":
                    case "no wankiriferous pathitudinality 2":
                        //stainless version of "none"
                        path = "";
                        break;

                    case "straight":
                        path = value;
                        break;

                    default:
                        Console.WriteLine("Unknown path: " + value);
                        path = "";
                        break;
                }
            }
        }

        public string PathMovement
        {
            get { return pathmovement; }
            set
            {
                switch (value)
                {
                    case "absolute":
                    case "harmonic":
                    case "linear":
                        pathmovement = value;
                        break;

                    default:
                        Console.WriteLine("Unknown path movement: " + value);
                        break;
                }
            }
        }

        public string Movement
        {
            get { return movement; }
            set
            {
                switch (value)
                {
                    case "no object":
                    case "no object moving cunt":
                        //stainless version of "none"
                        movement = "";
                        break;

                    case "rock":
                    case "shear":
                    case "spin":
                        movement = value;
                        break;

                    default:
                        Console.WriteLine("Unknown movement: " + value);
                        movement = "";
                        break;
                }
            }
        }

        public string MovementMovement
        {
            get { return movementmovement; }
            set
            {
                switch (value.ToLower())
                {
                    case "absolute":
                    case "continuous":
                    case "controlled":
                    case "flash":
                    case "harmonic":
                    case "linear":
                        movementmovement = value;
                        break;

                    default:
                        Console.WriteLine("Unknown movement movement: " + value);
                        break;
                }
            }
        }
    }
}