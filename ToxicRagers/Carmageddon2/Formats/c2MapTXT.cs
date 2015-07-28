using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;
using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Carmageddon2.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class Map
    {
        int version;
        Vector3 lightColour;
        Vector2 diffuseLight0;
        Vector2 diffuseLight1;
        Vector2 diffuseLightOther;

        Vector3 gridPosition;
        int gridRotation;

        List<Checkpoint> checkpoints;

        List<PedSpec> pedSpecs;

        string additionalActor;

        string skyPixelmap;
        int horizontalRepetitions;
        int verticalSize;
        int positionOfHorizon;
        string depthCueMode;
        Vector2 fogDarkness;
        Vector3 depthCueColour;

        int defaultEngineNoise;

        List<SpecialEffectVolume> specialVolumes;

        string materialDefault;
        string materialDarkness;
        string materialFog;

        string mapPixelmap;
        Matrix3D worldMapTransform;

        List<Vector3> nodes;
        List<OpponentPathSection> paths;

        public Map()
        {
            checkpoints = new List<Checkpoint>();
            pedSpecs = new List<PedSpec>();
            specialVolumes = new List<SpecialEffectVolume>();
            nodes = new List<Vector3>();
            paths = new List<OpponentPathSection>();
        }

        public static Map Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            Map map = new Map();

            string version = file.ReadLine();

            if (!version.StartsWith("VERSION"))
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            map.version = version.Replace("VERSION ", "").ToInt();

            if (map.version == 1)
            {
                // V1 has global lighting data
                map.lightColour = file.ReadVector3();       // RGB for main directional light-source
                map.diffuseLight0 = file.ReadVector2();     // Ambient/Diffuse light to be used when plaything ambient says 0
                map.diffuseLight1 = file.ReadVector2();     // Ambient/Diffuse light to be used when plaything ambient says 1
                map.diffuseLightOther = file.ReadVector2(); // Ambient/Diffuse light to be used when plaything ambient says anything else
            }

            map.gridPosition = file.ReadVector3();          // Position of centre of start of grid
            map.gridRotation = file.ReadInt();              // Direction that grid faces in

            //string t = getNextLine(sr);

            //if (version == 8)
            //{
            //    // alpha and demo v8 files had an extra block here, we'll test if it
            //    if (t.Contains(","))
            //    {
            //        getNextLine(sr); // # laps
            //        getNextLine(sr); // Race completed bonus (all laps raced) for each skill level
            //        getNextLine(sr); // Race completed bonus (all peds killed) for each skill level
            //        getNextLine(sr); // Race completed bonus (all oppos wasted) for each skill level

            //        t = getNextLine(sr);
            //    }
            //}

            int checkpointCount = file.ReadInt();
            for (int i = 0; i < checkpointCount; i++)
            {
                Checkpoint cp = new Checkpoint();

                cp.TimerIncrements = file.ReadVector3();

                int quadCount = file.ReadInt();

                for (int j = 0; j < quadCount; j++)
                {
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                }

                map.checkpoints.Add(cp);
            }

            int smashSpecs = file.ReadInt();

            if (smashSpecs > 0)
            {
                throw new NotImplementedException("Can't handle smash specs yet!");
            }

            //for (int k = 0; k < i; k++)
            //{
            //    int flags = Convert.ToInt32(getNextLine(sr));
            //    string trigger = getNextLine(sr);
            //    if (trigger.Length == 3 && trigger.StartsWith("&")) { if (!TestLine("7", getNextLine(sr))) { Console.WriteLine("Noncar smash"); return null; } }
            //    string mode = getNextLine(sr);

            //    switch (mode)
            //    {
            //        case "remove":
            //            Single removalthreshold = getNextLine(sr).ToSingle();
            //            if (!processConnotations(sr)) { return null; }
            //            break;

            //        case "nochange":
            //            Single threshold = getNextLine(sr).ToSingle();
            //            if (!processConnotations(sr)) { return null; }
            //            break;

            //        case "replacemodel":
            //            Single replacethreshold = getNextLine(sr).ToSingle();
            //            if (!processConnotations(sr)) { return null; }
            //            string newmodel = getNextLine(sr);
            //            int chanceoffire = Convert.ToInt32(getNextLine(sr));
            //            if (chanceoffire > 0)
            //            {
            //                int numfires = Convert.ToInt32(getNextLine(sr));
            //                Vector2 smokeindex = Vector2.Parse(getNextLine(sr));
            //            }
            //            break;

            //        case "texturechange":
            //            string intactpixelmap = getNextLine(sr);
            //            int numlevels = Convert.ToInt32(getNextLine(sr));
            //            for (int j = 0; j < numlevels; j++)
            //            {
            //                Single triggerthreshold = getNextLine(sr).ToSingle();
            //                int texchangeflags = Convert.ToInt32(getNextLine(sr));
            //                string collisiontype = getNextLine(sr);

            //                if (!processConnotations(sr)) { return null; }

            //                int numpixelmaps = Convert.ToInt32(getNextLine(sr));
            //                for (int l = 0; l < numpixelmaps; l++)
            //                {
            //                    string pixelmap = getNextLine(sr);
            //                }
            //            }
            //            break;

            //        default:
            //            Console.WriteLine("Unknown mode : " + mode);
            //            return null;
            //    }

            //    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 1"); return null; }
            //    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 2"); return null; }
            //    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 3"); return null; }
            //    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Reserved 4"); return null; }
            //}

            int pedSpecs = file.ReadInt();

            for (int i = 0; i < pedSpecs; i++)
            {
                PedSpec ps = new PedSpec();

                ps.MaterialName = file.ReadLine();
                ps.MovementIndex = file.ReadInt();
                ps.GroupIndex = file.ReadInt();
                ps.PedsPer100SquareMetres = file.ReadInt();

                int exclusionCount = file.ReadInt();
                for (int j = 0; j < exclusionCount; j++)
                {
                    ps.ExclusionMaterials.Add(file.ReadLine());
                }

                int exceptionCount = file.ReadInt();
                for (int j = 0; j < exceptionCount; j++)
                {
                    ps.ExceptionMaterials.Add(file.ReadLine());
                }

                map.pedSpecs.Add(ps);
            }

            map.additionalActor = file.ReadLine();

            map.skyPixelmap = file.ReadLine();
            map.horizontalRepetitions = file.ReadInt();
            map.verticalSize = file.ReadInt();
            map.positionOfHorizon = file.ReadInt();
            map.depthCueMode = file.ReadLine();
            map.fogDarkness = file.ReadVector2();
            map.depthCueColour = file.ReadVector3();

            map.defaultEngineNoise = file.ReadInt();

            int specialEffectVolumeCount = file.ReadInt();

            for (int i = 0; i < specialEffectVolumeCount; i++)
            {
                SpecialEffectVolume sev = new SpecialEffectVolume();

                sev.Name = file.ReadLine();

                if (i > 0)
                {
                    sev.Corners.Add(file.ReadVector3());
                    sev.Corners.Add(file.ReadVector3());
                    sev.Corners.Add(file.ReadVector3());
                    sev.Corners.Add(file.ReadVector3());
                }

                sev.GravityMultiplier = file.ReadSingle();
                sev.ViscosityMultiplier = file.ReadSingle();
                sev.CarDamagePerMillisecond = file.ReadSingle();
                sev.PedDamagePerMillisecond = file.ReadSingle();
                sev.CameraEffectIndex = file.ReadInt();
                sev.SkyColour = file.ReadInt();
                sev.WindscreenMaterial = file.ReadLine();
                sev.EntrySoundID = file.ReadInt();
                sev.ExitSoundID = file.ReadInt();
                sev.EngineNoiseIndex = file.ReadInt();
                sev.MaterialIndex = file.ReadInt();

                map.specialVolumes.Add(sev);
            }

            int soundGeneratorCount = file.ReadInt();

            if (soundGeneratorCount > 0)
            {
                throw new NotImplementedException("Can't handle sound generators yet!");
            }

            map.materialDefault = file.ReadLine();
            map.materialDarkness = file.ReadLine();
            map.materialFog = file.ReadLine();

            file.ReadLine();    // (ignore) # areas with different screens

            map.mapPixelmap = file.ReadLine();
            map.worldMapTransform = new Matrix3D(
                file.ReadVector3(),
                file.ReadVector3(),
                file.ReadVector3(),
                file.ReadVector3()
            );

            Console.WriteLine(map.worldMapTransform);

            while (file.ReadLine() != "END OF FUNK") ;
            while (file.ReadLine() != "END OF GROOVE") ;

            if (file.ReadLine() != "START OF OPPONENT PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            int nodeCount = file.ReadInt();
            for (int i = 0; i < nodeCount; i++)
            {
                map.nodes.Add(file.ReadVector3());
            }

            int pathCount = file.ReadInt();
            for (int i = 0; i < pathCount; i++)
            {
                map.paths.Add(new OpponentPathSection(file.ReadLine()));
            }

            return map;
        }

        //private static void processInitialVelocity(StreamReader sr)
        //{
        //    Vector2 towardsyouspeed = Vector2.Parse(getNextLine(sr));
        //    Single impacteevelocityfactor = getNextLine(sr).ToSingle();
        //    Single randomvelocity = getNextLine(sr).ToSingle();
        //    Single randomupvelocity = getNextLine(sr).ToSingle();
        //    Single randomnormalvelocity = getNextLine(sr).ToSingle();
        //    Single randomspinrate = getNextLine(sr).ToSingle();
        //}

        //private static bool processConnotations(StreamReader sr)
        //{
        //    int numsounds = Convert.ToInt32(getNextLine(sr));
        //    for (int l = 0; l < numsounds; l++)
        //    {
        //        string soundid = getNextLine(sr);
        //    }

        //    int numshrapnel = Convert.ToInt32(getNextLine(sr));
        //    for (int l = 0; l < numshrapnel; l++)
        //    {
        //        string shrapneltype = getNextLine(sr);

        //        processInitialVelocity(sr);

        //        if (shrapneltype != "shards")
        //        {
        //            string initialposition = getNextLine(sr);
        //            switch (initialposition)
        //            {
        //                case "actorbased":
        //                    // do nothing
        //                    break;

        //                case "sphereclumped":
        //                    Single clumpingradius = getNextLine(sr).ToSingle();
        //                    string clumpingcentre = getNextLine(sr);
        //                    break;

        //                default:
        //                    Console.WriteLine("Unknown initial position : " + initialposition);
        //                    return false;
        //            }
        //        }

        //        if (shrapneltype != "noncars") { Vector2 time = Vector2.Parse(getNextLine(sr)); }

        //        switch (shrapneltype)
        //        {
        //            case "ghostparts":
        //                string t = getNextLine(sr);
        //                if (t.Contains(","))
        //                {
        //                    Vector2 vghostpartcount = Vector2.Parse(t);
        //                }
        //                else
        //                {
        //                    int ighostpartcount = Convert.ToInt32(t);
        //                }
        //                int numghosts = Convert.ToInt32(getNextLine(sr));
        //                if (numghosts > 0)
        //                {
        //                    for (int m = 0; m < numghosts; m++)
        //                    {
        //                        string actorname = getNextLine(sr);
        //                    }
        //                }
        //                else
        //                {
        //                    string shrapnelactor = getNextLine(sr);
        //                }
        //                break;

        //            case "noncars":
        //                Vector2 noncarcount = Vector2.Parse(getNextLine(sr));
        //                int chanceoffire = Convert.ToInt32(getNextLine(sr));
        //                if (chanceoffire > 0)
        //                {
        //                    int numfires = Convert.ToInt32(getNextLine(sr));
        //                    Vector2 smokeindex = Vector2.Parse(getNextLine(sr));
        //                }
        //                string actorhierarchy = getNextLine(sr);
        //                int numactors = Convert.ToInt32(getNextLine(sr));
        //                for (int m = 0; m < numactors; m++)
        //                {
        //                    string actorname = getNextLine(sr);
        //                    string actorfile = getNextLine(sr);
        //                }
        //                break;

        //            case "shards":
        //                Single mincutlength = getNextLine(sr).ToSingle();
        //                if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Shards flags"); return false; }
        //                string shardmaterial = getNextLine(sr);
        //                break;

        //            default:
        //                Console.WriteLine("Unknown shrapnel type : " + shrapneltype);
        //                return false;
        //        }
        //    }

        //    int numexplosions = Convert.ToInt32(getNextLine(sr));
        //    for (int l = 0; l < numexplosions; l++)
        //    {
        //        Vector2 explosionettecount = Vector2.Parse(getNextLine(sr));
        //        Vector2 starttime = Vector2.Parse(getNextLine(sr));
        //        Vector3 explosionoffset = Vector3.Parse(getNextLine(sr));
        //        Vector2 xfactor = Vector2.Parse(getNextLine(sr));
        //        Vector2 yfactor = Vector2.Parse(getNextLine(sr));
        //        Vector2 zfactor = Vector2.Parse(getNextLine(sr));
        //        Vector2 framerate = Vector2.Parse(getNextLine(sr));
        //        Vector2 scalingfactor = Vector2.Parse(getNextLine(sr));
        //        string rotationmode = getNextLine(sr);
        //        int numframes = Convert.ToInt32(getNextLine(sr));
        //        for (int m = 0; m < numframes; m++)
        //        {
        //            Single opacity = getNextLine(sr).ToSingle();
        //            string pixname = getNextLine(sr);
        //        }
        //    }

        //    if (!TestLine("none", getNextLine(sr))) { Console.WriteLine("Slick"); return false; }

        //    int numnoncarcuboids = Convert.ToInt32(getNextLine(sr));
        //    for (int l = 0; l < numnoncarcuboids; l++)
        //    {
        //        Vector2 cuboidtime = Vector2.Parse(getNextLine(sr));
        //        string coordsystem = getNextLine(sr);
        //        string noncarindex = getNextLine(sr);
        //        Vector3 min = Vector3.Parse(getNextLine(sr));
        //        Vector3 max = Vector3.Parse(getNextLine(sr));
        //        processInitialVelocity(sr);
        //    }

        //    int numsmashcuboids = Convert.ToInt32(getNextLine(sr));
        //    for (int l = 0; l < numsmashcuboids; l++)
        //    {
        //        Vector2 cuboidtime = Vector2.Parse(getNextLine(sr));
        //        string triggertype = getNextLine(sr);
        //        string coordsystem = getNextLine(sr);
        //        Vector3 min = Vector3.Parse(getNextLine(sr));
        //        Vector3 max = Vector3.Parse(getNextLine(sr));
        //        string direction = getNextLine(sr);
        //        Single force = getNextLine(sr).ToSingle();
        //    }

        //    if (!TestLine("0", getNextLine(sr))) { Console.WriteLine("Extension flags"); return false; }

        //    string roomturnoncode = getNextLine(sr);
        //    if (roomturnoncode != "0" && roomturnoncode != "14") { Console.WriteLine("Unknown room turn on code : " + roomturnoncode); }

        //    string awardcode = getNextLine(sr);
        //    switch (awardcode)
        //    {
        //        case "none":
        //            // do nothing
        //            break;

        //        case "doitregardless":
        //        case "repeated":
        //        case "singleshot":
        //            int awardpoints = Convert.ToInt32(getNextLine(sr));
        //            int awardtime = Convert.ToInt32(getNextLine(sr));
        //            int hudindex = Convert.ToInt32(getNextLine(sr));
        //            int fancyhudindex = Convert.ToInt32(getNextLine(sr));
        //            break;

        //        default:
        //            Console.WriteLine("Unknown award code: " + awardcode);
        //            return false;
        //    }

        //    int numvariablechanges = Convert.ToInt32(getNextLine(sr));
        //    for (int l = 0; l < numvariablechanges; l++)
        //    {
        //        Vector2 variables = Vector2.Parse(getNextLine(sr));
        //    }

        //    return true;
        //}
    }

    public class Checkpoint
    {
        Vector3 timerIncrement;
        List<Vector3> points;

        public Vector3 TimerIncrements
        {
            get { return timerIncrement; }
            set { timerIncrement = value; }
        }

        public List<Vector3> Points
        {
            get { return points; }
            set { points = value; }
        }

        public Checkpoint()
        {
            points = new List<Vector3>();
        }
    }

    public class PedSpec
    {
        string materialName;
        int movementIndex;
        int groupIndex;
        int pedsPer100SquareMetres;
        List<string> exclusionMaterials;
        List<string> exceptionMaterials;

        public string MaterialName
        {
            get { return materialName; }
            set { materialName = value; }
        }

        public int MovementIndex 
        {
            get { return movementIndex; }
            set { movementIndex = value; }
        }

        public int GroupIndex
        {
            get { return groupIndex; }
            set { groupIndex = value; }
        }

        public int PedsPer100SquareMetres
        {
            get { return pedsPer100SquareMetres; }
            set { pedsPer100SquareMetres = value; }
        }

        public List<string> ExclusionMaterials
        {
            get { return exclusionMaterials; }
            set { exclusionMaterials = value; }
        }

        public List<string> ExceptionMaterials
        {
            get { return exceptionMaterials; }
            set { exceptionMaterials = value; }
        }

        public PedSpec()
        {
            exclusionMaterials = new List<string>();
            exclusionMaterials = new List<string>();
        }
    }

    public class SpecialEffectVolume
    {
        string name;
        Single gravityMultiplier;
        Single viscosityMultiplier;
        Single carDamagePerMillisecond;
        Single pedDamagePerMillisecond;
        int cameraEffectIndex;
        int skyColour;
        string windscreenMaterial;
        int entrySoundID;
        int exitSoundID;
        int engineNoiseIndex;
        int materialIndex;
        List<Vector3> corners;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Single GravityMultiplier
        {
            get { return gravityMultiplier; }
            set { gravityMultiplier = value; }
        }

        public Single ViscosityMultiplier
        {
            get { return viscosityMultiplier; }
            set { viscosityMultiplier = value; }
        }

        public Single CarDamagePerMillisecond
        {
            get { return carDamagePerMillisecond; }
            set { carDamagePerMillisecond = value; }
        }

        public Single PedDamagePerMillisecond
        {
            get { return pedDamagePerMillisecond; }
            set { pedDamagePerMillisecond = value; }
        }

        public int CameraEffectIndex
        {
            get { return cameraEffectIndex; }
            set { cameraEffectIndex = value; }
        }

        public int SkyColour
        {
            get { return skyColour; }
            set { skyColour = value; }
        }

        public string WindscreenMaterial
        {
            get { return windscreenMaterial; }
            set { windscreenMaterial = value; }
        }

        public int EntrySoundID
        {
            get { return entrySoundID; }
            set { entrySoundID = value; }
        }

        public int ExitSoundID
        {
            get { return exitSoundID; }
            set { exitSoundID = value; }
        }

        public int EngineNoiseIndex
        {
            get { return engineNoiseIndex; }
            set { engineNoiseIndex = value; }
        }

        public int MaterialIndex
        {
            get { return materialIndex; }
            set { materialIndex = value; }
        }

        public List<Vector3> Corners
        {
            get { return corners; }
            set { corners = value; }
        }

        public SpecialEffectVolume()
        {
            corners = new List<Vector3>();
        }
    }
}
