using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class FunkParser
    {
        public List<Funk> Funks = new List<Funk>();

        public bool Load(string pathToFile)
        {
            StreamReader sr = new StreamReader(pathToFile);

            while (getNextLine(sr) != "START OF FUNK") { if (sr.EndOfStream) { return true; } }

            string s = getNextLine(sr);
            while (s != "END OF FUNK" && s != null)
            {
                Funk funk = new Funk()
                {
                    File = pathToFile,
                    Material = s
                };

                funk.SetDistance(getNextLine(sr));
                funk.SetDistortion(getNextLine(sr));

                if (funk.Distortion != Funk.DistortionType.None)
                {
                    //Console.WriteLine(pathToFile);
                    Console.WriteLine(getNextLine(sr));
                }

                while (getNextLine(sr) != "NEXT FUNK") { if (sr.EndOfStream) { return true; } }
                s = getNextLine(sr);

                if (funk.HasMovementType) { funk.MovementType = s = getNextLine(sr); }
                if (funk.HasMovementSettings)
                {

                    //if (funk.Distortion == "spin")
                    //{
                    //    s = getNextLine(sr); funk.SpinSpeed = Convert.ToInt32(s); ;
                    //}
                    //else
                    //{
                    //    s = getNextLine(sr); funk.MovementSettings = Vector2.Parse(s);
                    //}

                }
                if (funk.HasMovementSettings2) { s = getNextLine(sr); funk.MovementSettings2 = Vector2.Parse(s); }
                if (funk.HasMovementSettings3) { s = getNextLine(sr); funk.MovementSettings3 = Vector2.Parse(s); }
                funk.Lighting = s = getNextLine(sr);
                funk.Animation = s = getNextLine(sr);
                if (funk.HasAnimationSettings) { funk.AnimationQuality = s = getNextLine(sr); }
                if (funk.Animation == "flic") { funk.FlicFile = s = getNextLine(sr); }
                if (funk.Animation == "frames")
                {
                    funk.AnimationSpeed = s = getNextLine(sr);

                    if (funk.AnimationSpeed == "texturebits")
                    {
                        funk.TexturebitsMode = s = getNextLine(sr);
                    }
                    else
                    {
                        s = getNextLine(sr); funk.FramesPerSecond = Convert.ToInt32(Convert.ToSingle(s));
                    }

                    s = getNextLine(sr); funk.Frames = Convert.ToInt32(s);

                    for (int i = 0; i < funk.Frames; i++)
                    {
                        funk.FrameFile[i] = s = getNextLine(sr);
                    }
                }

                Funks.Add(funk);

                s = getNextLine(sr);
                if (s == "NEXT FUNK") { s = getNextLine(sr); }
            }

            foreach (Funk funk in Funks)
            {
                //Console.WriteLine(funk.Distance);
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

    public class Funk
    {
        public enum DistanceType
        {
            Constant,
            Distance,
            LastLap,
            OtherLaps
        }

        public enum DistortionType
        {
            Rock,
            Roll,
            Slither,
            Spin,
            Throb,
            None
        }

        public string File;
        public string Material;
        public string FlicFile;
        public int FramesPerSecond;
        public int SpinSpeed;
        int frames;
        public string[] FrameFile;
        public DistanceType Distance;
        public DistortionType Distortion = DistortionType.None;
        string movementType;
        string lighting;
        string animation;
        string animationQuality;
        string animationSpeed;
        string texturebitsMode;
        public Vector2 MovementSettings;
        public Vector2 MovementSettings2;
        public Vector2 MovementSettings3;
        public bool HasMovementType = false;
        public bool HasMovementSettings = false;
        public bool HasMovementSettings2 = false;
        public bool HasMovementSettings3 = false;
        public bool HasAnimationSettings = false;

        public int Frames
        {
            get => frames;
            set
            {
                FrameFile = new string[value];
                frames = value;
            }
        }

        public void SetDistance(string distance)
        {
            foreach (string s in Enum.GetNames(typeof(DistanceType)))
            {
                if (s.ToLower() == distance.ToLower())
                {
                    Distance = (DistanceType)Enum.Parse(typeof(DistanceType), distance, true);
                    return;
                }
            }

            Console.WriteLine("Unknown distance: " + distance);
        }

        public void SetDistortion(string distortion)
        {
            foreach (string s in Enum.GetNames(typeof(DistortionType)))
            {
                if (s.ToLower() == distortion.ToLower())
                {

                    Distortion = (DistortionType)Enum.Parse(typeof(DistortionType), distortion, true);
                    return;
                }
            }

            if (distortion.ToLower() == "piss off") { return; }
            // "piss off" is stainless' version of "none"

            Console.WriteLine("Unknown distortion: " + distortion);
        }

        public string MovementType
        {
            get => movementType;
            set
            {
                switch (value)
                {
                    case "absolute":
                    case "continuous":
                    case "controlled":
                    case "harmonic":
                        break;
                    default:
                        Console.WriteLine(File);
                        Console.WriteLine("Unknown movement type: " + value);
                        break;
                }

                movementType = value;
            }
        }

        public string Lighting
        {
            get => lighting;
            set
            {
                switch (value)
                {
                    case "no fucking lighting":
                    case "no fucking lighting bastards":
                        // stainless version of "none"
                        break;

                    default:
                        Console.WriteLine(File);
                        Console.WriteLine("Unknown lighting: " + value);
                        break;
                }

                lighting = value;
            }
        }

        public string Animation
        {
            get => animation;
            set
            {
                switch (value)
                {
                    case "no animation you cunt":
                        // stainless version of "none"
                        break;

                    case "flic":
                    case "frames":
                        HasAnimationSettings = true;
                        break;

                    default:
                        Console.WriteLine(File);
                        Console.WriteLine("Unknown animation: " + value);
                        break;
                }

                animation = value;
            }
        }

        public string AnimationQuality
        {
            get => animationQuality;
            set
            {
                switch (value)
                {
                    case "accurate":
                    case "approximate":
                        break;
                    default:
                        Console.WriteLine(File);
                        Console.WriteLine("Unknown animation quality: " + value);
                        break;
                }

                animationQuality = value;
            }
        }

        public string AnimationSpeed
        {
            get => animationSpeed;
            set
            {
                switch (value)
                {
                    case "texturebits":
                    case "continuous":
                        break;
                    default:
                        Console.WriteLine(File);
                        Console.WriteLine("Unknown animation speed: " + value);
                        break;
                }

                animationSpeed = value;
            }
        }

        public string TexturebitsMode
        {
            get => texturebitsMode;
            set
            {
                switch (value)
                {
                    case "B":
                    case "BV":
                    case "V":
                    case "VB":
                        break;
                    default:
                        Console.WriteLine(File);
                        Console.WriteLine("Unknown texturebits mode: " + value);
                        break;
                }

                texturebitsMode = value;
            }
        }
    }
}