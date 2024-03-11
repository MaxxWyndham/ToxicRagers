using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Formats
{
    public class Level
    {
        public int Version { get; set; }
        public Vector3 GridCenter { get; set; }
        public float GridRotation { get; set; }
        public int[] InitialTimerPerSkill { get; set; }
        public int NumberOfLaps { get; set; }
        public int[] AllLapsRaceCompletedBonusPerSkill { get; set; }
        public int[] AllPedsRaceCompletedBonusPerSkill { get; set; }
        public int[] AllWastedRaceCompletedBonusPerSkill { get; set; }
        public int[] MapCheckpointWidth { get; set; }
        public int[] MapCheckpointHeight { get; set; }
        public int NumberOfCheckpoints { get; set; }
        public List<Checkpoint> Checkpoints { get; set; }
        public int NumberOfPixelMaps1 { get; set; }
        public List<string> PixelMaps1 { get; set; }
        public int NumberOfPixelMaps2 { get; set; }
        public List<string> PixelMaps2 { get; set; }
        public int NumberOfShadeTabs { get; set; }
        public List<string> ShadeTabs { get; set; }
        public int NumberOfMaterials1 { get; set; }
        public List<string> Materials1 { get; set; }
        public int NumberOfMaterials2 { get; set; }
        public List<string> Materials2 { get; set; }
        public int NumberOfModelFiles1 { get; set; }
        public List<string> Models1 { get; set; }
        public int NumberOfModelFiles2 { get; set; }
        public List<string> Models2 { get; set; }
        public string Actor1 { get; set; }
        public string Actor2 { get; set; }
        public int DefaultTransparencyOfBangMats { get; set; }
        public string Actor3 { get; set; }
        public string SkyTexture { get; set; }
        public int SkyHorizontalRepetitions { get; set; }
        public int SkyVeticalSize { get; set; }
        public int SkyHorizonVerticalPosition { get; set; }
        public HorizonDepthCueMode SkyDepthCueMode { get; set; }
        public int SkyFog { get; set; }
        public int SkyDarkness { get; set; }
        public int DefaultEngineNoise { get; set; }
        public int NumberOfSpecialEffectsVolumes { get; set; }
        public List<SFXVolume> SpecialEffectsVolumes { get; set; }
        public string WindscreenDefault { get; set; }
        public string WindscreenDark { get; set; }
        public string WindscreenFog { get; set; }
        public int NumberOfAreasWithDifferentWindscreens { get; set; }
        public List<WindscreenArea> WindscreenAreas { get; set; }
        public string MapPixelMap { get; set; }
        public Matrix3D WorldMapMatrix { get; set; }
        public List<Funk> Funks { get; set; }
        public List<Groove> Grooves { get; set; }
        public int PedSubsTable { get; set; }
        public int NumberOfPeds { get; set; }
        public List<PedSpec> PedSpecs { get; set; }
        public int NumberOfPathNodes { get; set; }
        public List<Vector3> PathNodes { get; set; }
        public int NumberOfPathSections { get; set; }
        public List<PathSection> PathSections { get; set; }
        public int NumberOfCopStartPoints { get; set; }
        public List<CopSpawn> CopStartPoints { get; set; }
        public int NumberOfMaterialModifiers { get; set; }
        public List<MaterialModifier> MaterialModifiers { get; set; }
        public int NumberOfNonCars { get; set; }
        public List<string> Noncars { get; set; }
        public int NumberOfDustShadeTables { get; set; }
        public List<DustShadeTable> DustShadeTables { get; set; }
        public int NumberOfNetworkStarts { get; set; }
        public List<StartPoint> NetworkStartPoints { get; set; }
        public int NumberOfSplashFiles { get; set; }
        public List<string> SplashPixelMap { get; set; }
        public float YonModifier { get; set; }
        public string TextFileName { get; set; }
        
        public static Level Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            Level level = new Level();
            string versionLine = file.ReadLine();
            if(versionLine.Substring(0, 7).ToUpper() != "VERSION")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon level .txt file, expected VERSION string but found {0}", versionLine);
                return null;
            }
            level.Version = versionLine.Split(' ')[1].ToInt();
            
            level.GridCenter = file.ReadVector3();
            level.GridRotation = file.ReadSingle();
            
            level.InitialTimerPerSkill = file.ReadInts();
            level.NumberOfLaps = file.ReadInt();
            level.AllLapsRaceCompletedBonusPerSkill = file.ReadInts();
            level.AllPedsRaceCompletedBonusPerSkill = file.ReadInts();
            level.AllWastedRaceCompletedBonusPerSkill = file.ReadInts();
            
            level.MapCheckpointWidth = file.ReadInts();
            level.MapCheckpointHeight= file.ReadInts();
            level.NumberOfCheckpoints = file.ReadInt();
            level.Checkpoints = new List<Checkpoint>();
            for(int i = 0; i < level.NumberOfCheckpoints; i++)
            {
                level.Checkpoints.Add(Checkpoint.Load(file));
            }

            level.NumberOfPixelMaps1 = file.ReadInt();
            level.PixelMaps1 = new List<string>();
            for (int i = 0; i < level.NumberOfPixelMaps1; i++)
            {
                level.PixelMaps1.Add(file.ReadLine());
            }

            level.NumberOfPixelMaps2 = file.ReadInt();
            level.PixelMaps2 = new List<string>();
            for (int i = 0; i < level.NumberOfPixelMaps2; i++)
            {
                level.PixelMaps2.Add(file.ReadLine());
            }


            level.NumberOfShadeTabs = file.ReadInt();
            level.ShadeTabs = new List<string>();
            for (int i = 0; i < level.NumberOfShadeTabs; i++)
            {
                level.ShadeTabs.Add(file.ReadLine());
            }

            level.NumberOfMaterials1 = file.ReadInt();
            level.Materials1 = new List<string>();
            for (int i = 0; i < level.NumberOfMaterials1; i++)
            {
                level.Materials1.Add(file.ReadLine());
            }

            level.NumberOfMaterials2 = file.ReadInt();
            level.Materials2 = new List<string>();
            for (int i = 0; i < level.NumberOfMaterials2; i++)
            {
                level.Materials2.Add(file.ReadLine());
            }

            level.NumberOfModelFiles1 = file.ReadInt();
            level.Models1 = new List<string>();
            for (int i = 0; i < level.NumberOfModelFiles1; i++)
            {
                level.Models1.Add(file.ReadLine());
            }

            if (level.Version >= 6)
            {
                level.NumberOfModelFiles2 = file.ReadInt();
                level.Models2 = new List<string>();
                for (int i = 0; i < level.NumberOfModelFiles2; i++)
                {
                    level.Models2.Add(file.ReadLine());
                }
            }

            level.Actor1 = file.ReadLine();
            level.Actor2 = file.ReadLine();

            if (level.Version >= 7)
            {
                level.DefaultTransparencyOfBangMats = file.ReadInt();
            }
            if (level.Version >= 6)
            {
                level.Actor3 = file.ReadLine();
            }

            level.SkyTexture = file.ReadLine();
            level.SkyHorizontalRepetitions = file.ReadInt();
            level.SkyVeticalSize = file.ReadInt();
            level.SkyHorizonVerticalPosition = file.ReadInt();
            level.SkyDepthCueMode = file.ReadLine().ToEnumWithDefault<HorizonDepthCueMode>(HorizonDepthCueMode.none);

            int[] fogDarkness = file.ReadInts();
            level.SkyFog = fogDarkness[0];
            level.SkyDarkness = fogDarkness[1];

            level.DefaultEngineNoise = file.ReadInt();
            
            level.NumberOfSpecialEffectsVolumes = file.ReadInt();
            level.SpecialEffectsVolumes = new List<SFXVolume>();
            for (int i = 0; i < level.NumberOfSpecialEffectsVolumes; i++)
            {
                level.SpecialEffectsVolumes.Add(SFXVolume.Load(file));
            }

            level.WindscreenDefault = file.ReadLine();
            level.WindscreenDark = file.ReadLine();
            level.WindscreenFog = file.ReadLine();
            level.NumberOfAreasWithDifferentWindscreens = file.ReadInt();
            level.WindscreenAreas = new List<WindscreenArea>();
            for (int i = 0; i < level.NumberOfAreasWithDifferentWindscreens; i++)
            {
                level.WindscreenAreas.Add(WindscreenArea.Load(file));
            }

            level.MapPixelMap = file.ReadLine();
            level.WorldMapMatrix = new Matrix3D(file.ReadVector3(), file.ReadVector3(), file.ReadVector3(), file.ReadVector3());

            string startOfFunkLine = file.ReadLine();
            if (startOfFunkLine != "START OF FUNK")
            {

                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon level .txt file, expected START OF FUNK but found {0}", startOfFunkLine);
                return null;
            }

            level.Funks = new List<Funk>();
            if (file.PeekLine() != "END OF FUNK")
            {
                while (!file.EOF)
                {
                    level.Funks.Add(Funk.Load(file));
                    if (file.ReadLine() == "END OF FUNK")
                    {
                        break;
                    }
                }
            }
            else
            {
                file.ReadLine();
            }

            string startOfGrooveLine = file.ReadLine();
            if (startOfGrooveLine != "START OF GROOVE")
            {

                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon level .txt file, expected START OF GROOVE but found {0}", startOfGrooveLine);
                return null;
            }

            level.Grooves = new List<Groove>();
            if (file.PeekLine() != "END OF GROOVE")
            {
                while (!file.EOF)
                {
                    level.Grooves.Add(Groove.Load(file));
                    if (file.ReadLine() == "END OF GROOVE")
                    {
                        break;
                    }
                }
            }
            else
            {
                file.ReadLine();
            }

            level.PedSubsTable = file.ReadInt();
            level.NumberOfPeds = file.ReadInt();
            level.PedSpecs = new List<PedSpec>();
            for (int i = 0; i < level.NumberOfPeds; i++)
            {
                level.PedSpecs.Add(PedSpec.Load(file));
            }

            string opponentPathsString = file.ReadLine();
            if (opponentPathsString != "START OF OPPONENT PATHS")
            {

                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon level .txt file, expected START OF OPPONENT PATHS string but found {0}", opponentPathsString);
                return null;
            }

            level.NumberOfPathNodes = file.ReadInt();
            level.PathNodes = new List<Vector3>();
            for (int i = 0; i < level.NumberOfPathNodes; i++)
            {
                level.PathNodes.Add(file.ReadVector3());
            }

            level.NumberOfPathSections = file.ReadInt();
            level.PathSections = new List<PathSection>();
            for (int i = 0; i < level.NumberOfPathSections; i++)
            {
                level.PathSections.Add(PathSection.Load(file));
            }

            level.NumberOfCopStartPoints = file.ReadInt();
            level.CopStartPoints = new List<CopSpawn>();
            for(int i = 0; i < level.NumberOfCopStartPoints; i++)
            {
                level.CopStartPoints.Add(CopSpawn.Load(file));
            }

            string endOfPathsString = file.ReadLine();
            if(endOfPathsString != "END OF OPPONENT PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon level .txt file, expected END OF OPPONENT PATHS but found {0}", endOfPathsString);
                return null;
            }

            level.NumberOfMaterialModifiers = file.ReadInt();
            level.MaterialModifiers = new List<MaterialModifier>();
            
            for(int i = 0; i < level.NumberOfMaterialModifiers; i++)
            {
                level.MaterialModifiers.Add(MaterialModifier.Load(file));
            }

            level.NumberOfNonCars = file.ReadInt();
            level.Noncars = new List<string>();
            for (int i = 0; i < level.NumberOfNonCars; i++)
            {
                level.Noncars.Add(file.ReadLine());
            }

            level.NumberOfDustShadeTables = file.ReadInt();
            level.DustShadeTables = new List<DustShadeTable>();
            for(int i = 0; i < level.NumberOfDustShadeTables; i++)
            {
                level.DustShadeTables.Add(DustShadeTable.Load(file));
            }

            level.NumberOfNetworkStarts = file.ReadInt();
            level.NetworkStartPoints = new List<StartPoint>();
            for(int i = 0; i < level.NumberOfNetworkStarts; i++)
            {
                level.NetworkStartPoints.Add(StartPoint.Load(file));
            }

            level.NumberOfSplashFiles = file.ReadInt();
            level.SplashPixelMap = new List<string>();
            for(int i = 0; i < level.NumberOfSplashFiles; i++)
            {
                level.SplashPixelMap.Add(file.ReadLine());
            }

            if (level.Version >= 5)
            {
                level.YonModifier = file.ReadSingle();
            }
            level.TextFileName = file.ReadLine();

            return level;
        }
    }
    public enum HorizonDepthCueMode
    {
        none,
        dark,
        fod
    }
    public class Checkpoint
    {
        public int[] PedModeTimerIncrementPerSkill { get; set; }
        public int[] PedlessModeTimerIncrementPerSkill { get; set; }
        public int NumberOfQuads { get; set; }
        public CheckpointQuad[] Quads { get; set; }
        public int[] MapCheckpointPosX { get; set; }
        public int[] MapCheckpointPosY { get; set; }

        public static Checkpoint Load(DocumentParser file)
        {
            Checkpoint checkpoint = new Checkpoint();

            checkpoint.PedModeTimerIncrementPerSkill = file.ReadInts();
            checkpoint.PedlessModeTimerIncrementPerSkill = file.ReadInts();
            checkpoint.NumberOfQuads = file.ReadInt();
            checkpoint.Quads = new CheckpointQuad[checkpoint.NumberOfQuads];
            for( int i = 0; i < checkpoint.NumberOfQuads; i++)
            {
                checkpoint.Quads[i] = new CheckpointQuad
                {
                    Point1 = file.ReadVector3(),
                    Point2 = file.ReadVector3(),
                    Point3 = file.ReadVector3(),
                    Point4 = file.ReadVector3()
                };
            }
            checkpoint.MapCheckpointPosX = file.ReadInts();
            checkpoint.MapCheckpointPosX = file.ReadInts();
            return checkpoint;
        }
    }
    public class CheckpointQuad
    {
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Point3;
        public Vector3 Point4;
    }
    public class SFXVolume
    {
        public string VolumeName { get; set; }
        public float GravityMultiplier { get; set; }
        public float ViscosityMultiplier { get; set; }
        public float CarDamagePerMillisecond { get; set; }
        public float PedDamagePerMillisecond { get; set; }
        public int CameraEffectIndex { get; set; }
        public int SkyColour { get; set; }
        public string WindscreenMatToUse { get; set; }
        public int SoundIDOfEntry { get; set; }
        public int SoundIDOfExit { get; set; }
        public int EngineNoiseIndex { get; set; }
        public int MaterialIndex { get; set; }
        public bool NewImproved { get; set; }
        public Vector3 Point1 { get; set; }
        public Vector3 Point2 { get; set; }
        public Vector3 Point3 { get; set; }
        public Vector3 Point4 { get; set; }

        public static SFXVolume Load(DocumentParser file)
        {
            SFXVolume volume = new SFXVolume();

            volume.VolumeName = file.ReadLine();
            if (volume.VolumeName == "NEW IMPROVED!")
            {
                volume.NewImproved = true;
                volume.Point1 = file.ReadVector3();
                volume.Point2 = file.ReadVector3();
                volume.Point3 = file.ReadVector3();
                volume.Point4 = file.ReadVector3();
            }
            else
            {
                volume.NewImproved = false;
            }
            volume.GravityMultiplier = file.ReadSingle();
            volume.ViscosityMultiplier = file.ReadSingle();
            volume.CarDamagePerMillisecond = file.ReadSingle();
            volume.PedDamagePerMillisecond = file.ReadSingle();
            volume.CameraEffectIndex = file.ReadInt();
            volume.SkyColour = file.ReadInt();
            volume.WindscreenMatToUse = file.ReadLine();
            volume.SoundIDOfEntry = file.ReadInt();
            volume.SoundIDOfExit = file.ReadInt();
            volume.EngineNoiseIndex = file.ReadInt();
            volume.MaterialIndex = file.ReadInt();
            return volume;
        }
    }

    public class WindscreenArea
    {
        public Vector4 Area { get; set; }
        public string ReplacementMat { get; set; }

        public static WindscreenArea Load(DocumentParser file)
        {
            WindscreenArea area = new WindscreenArea();
            area.Area = file.ReadVector4();
            area.ReplacementMat = file.ReadLine();

            return area;
        }
    }
    public class PedSpec
    {
        public int PedReference { get; set; }
        public int NumberOfInstructions { get; set; }
        public int InitialInstruction { get; set; }
        public List<PedSpecInstruction> Instructions { get; set; }
        public bool IsPowerup { get => PedReference >= 100; }

        public static PedSpec Load(DocumentParser file)
        {
            PedSpec spec = new PedSpec();

            spec.PedReference = file.ReadInt();
            spec.NumberOfInstructions = file.ReadInt();
            spec.InitialInstruction = file.ReadInt();
            spec.Instructions = new List<PedSpecInstruction>();
            for(int i=0; i < spec.NumberOfInstructions; i++)
            {
                spec.Instructions.Add(PedSpecInstruction.Load(file));
            }

            return spec;
        }
    }
    public enum PedSpecInstructionType
    {
        point,
        reverse
    }
    public class PedSpecInstruction {
        public PedSpecInstructionType InstructionType { get; set; }
        public Vector3 InstructionPosition { get; set; }

        public static PedSpecInstruction Load(DocumentParser file)
        {
            PedSpecInstruction instruction = new PedSpecInstruction();
            instruction.InstructionType = file.ReadLine().ToEnumWithDefault<PedSpecInstructionType>(PedSpecInstructionType.point);
            if(instruction.InstructionType == PedSpecInstructionType.point)
            {
                instruction.InstructionPosition = file.ReadVector3();
            }
            return instruction;
        }
    }

    public enum PathType
    {
        Normal = 0,
        Race = 1,
        Cheat = 2
    }

    public class PathSection
    {
        public int StartNode { get; set; }
        public int EndNode { get; set; }
        public int StartMinSpeed { get; set; }
        public int StartMaxSpeed { get; set; }
        public int EndMinSpeed { get; set; }
        public int EndMaxSpeed { get; set; }
        public float Width { get; set; }
        public PathType Type { get; set; }
        public bool Oneway   { get; set; }

        public static PathSection Load(DocumentParser file)
        {
            PathSection section = new PathSection();
            string[] sectionValues = file.ReadStrings();
            section.StartNode = sectionValues[0].ToInt();
            section.EndNode = sectionValues[1].ToInt();
            section.StartMinSpeed = sectionValues[2].ToInt();
            section.StartMaxSpeed = sectionValues[3].ToInt();
            section.EndMinSpeed = sectionValues[4].ToInt();
            section.EndMaxSpeed = sectionValues[5].ToInt();
            section.Width = sectionValues[6].ToSingle();

            int sectionType = sectionValues[7].ToInt();
            if(sectionType >= 1000)
            {
                section.Oneway = true;
                sectionType -= 1000;
            }
            section.Type = (PathType)sectionType;
            return section;
        }
    }

    public class MaterialModifier
    {
        public float CarWallFriction { get; set; }
        public float TyreRoadFriction { get; set; }
        public float Downforce { get; set; }
        public float Bumpiness { get; set; }
        public int TyreSound { get; set; }
        public int CrashSound { get; set; }
        public int ScrapeSound { get; set; }
        public float Sparkiness { get; set; }
        public int RoomForExpansion { get; set; }
        public string SkidmarkMaterial { get; set; }

        public static MaterialModifier Load(DocumentParser file)
        {
            MaterialModifier modifier = new MaterialModifier();

            modifier.CarWallFriction = file.ReadSingle();
            modifier.TyreRoadFriction = file.ReadSingle();
            modifier.Downforce = file.ReadSingle();
            modifier.Bumpiness = file.ReadSingle();
            modifier.TyreSound = file.ReadInt();
            modifier.CrashSound = file.ReadInt();
            modifier.ScrapeSound = file.ReadInt();
            modifier.Sparkiness = file.ReadSingle();
            modifier.ScrapeSound = file.ReadInt();
            modifier.SkidmarkMaterial = file.ReadLine();

            return modifier;
        }
    }
    public class DustShadeTable
    {
        public Colour DustColour { get; set; }
        public Vector3 Strengths { get; set; }

        internal static DustShadeTable Load(DocumentParser file)
        {
            DustShadeTable dust = new DustShadeTable();
            int[] colourRGB = file.ReadInts();
            dust.DustColour = Colour.FromRgb((byte)colourRGB[0], (byte)colourRGB[1], (byte)colourRGB[2]);
            dust.Strengths = file.ReadVector3();

            return dust;
        }
    }
    public class StartPoint
    {
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }

        internal static StartPoint Load(DocumentParser file)
        {
            StartPoint startPoint = new StartPoint();
            startPoint.Position = file.ReadVector3();
            startPoint.Rotation = file.ReadSingle();
            return startPoint;
        }
    }
    public class CopSpawn
    {
        public Vector3 Position { get; set; }
        public bool BigAPC { get; set; }

        public static CopSpawn Load(DocumentParser file)
        {
            CopSpawn spawn = new CopSpawn();
            string[] points = file.ReadStrings();
            spawn.Position = new Vector3(points[0].ToSingle(), points[1].ToSingle(), points[2].ToSingle());
            spawn.BigAPC = points[3] == "9" && points[4] == "9" && points[5] == "9";

            return spawn;
        }
    }
}
