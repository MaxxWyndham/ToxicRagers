using System;
using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Carmageddon2.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class Map
    {
        public int Version { get; set; }

        public Vector3 LightColour { get; set; }

        public Vector2 DiffuseLight0 { get; set; }

        public Vector2 DiffuseLight1 { get; set; }

        public Vector2 DiffuseLightOther { get; set; }

        public Vector3 GridPosition { get; set; }

        public int GridRotation { get; set; }

        public List<Checkpoint> Checkpoints { get; set; } = new List<Checkpoint>();

        public List<SmashData> Smashables { get; set; } = new List<SmashData>();

        public List<PedSpec> PedSpecs { get; set; } = new List<PedSpec>();

        public string AdditionalActor { get; set; }

        public string SkyPixelmap { get; set; }

        public int HorizontalRepetitions { get; set; }

        public int VerticalSize { get; set; }

        public int PositionOfHorizon { get; set; }

        public string DepthCueMode { get; set; }

        public Vector2 FogDarkness { get; set; }

        public Vector3 DepthCueColour { get; set; }

        public int DefaultEngineNoise { get; set; }

        public List<SpecialEffectVolume> SpecialVolumes { get; set; } = new List<SpecialEffectVolume>();

        public string MaterialDefault { get; set; }

        public string MaterialDarkness { get; set; }

        public string MaterialFog { get; set; }

        public string MapPixelmap { get; set; }

        public Matrix3D WorldMapTransform { get; set; }

        public List<Funk> Funks { get; set; } = new List<Funk>();

        public List<Groove> Grooves { get; set; } = new List<Groove>();

        public List<Vector3> Nodes { get; set; } = new List<Vector3>();

        public List<OpponentPathSection> Paths { get; set; } = new List<OpponentPathSection>();

        public int DronePathVersion { get; set; }

        public List<DronePathNode> DronePaths { get; set; } = new List<DronePathNode>();

        public List<MaterialModifier> MaterialModifiers { get; set; } = new List<MaterialModifier>();

        public List<string> Noncars { get; set; } = new List<string>();

        public List<ShadeTable> ShadeTables { get; set; } = new List<ShadeTable>();

        public List<NetworkStart> NetworkStarts { get; set; } = new List<NetworkStart>();

        public List<string> SplashFiles { get; set; } = new List<string>();

        public List<string> TxtFiles { get; set; } = new List<string>();

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

            map.Version = version.Replace("VERSION ", "").ToInt();

            if (map.Version == 1)
            {
                // V1 has global lighting data
                map.LightColour = file.ReadVector3();       // RGB for main directional light-source
                map.DiffuseLight0 = file.ReadVector2();     // Ambient/Diffuse light to be used when plaything ambient says 0
                map.DiffuseLight1 = file.ReadVector2();     // Ambient/Diffuse light to be used when plaything ambient says 1
                map.DiffuseLightOther = file.ReadVector2(); // Ambient/Diffuse light to be used when plaything ambient says anything else
            }

            map.GridPosition = file.ReadVector3();          // Position of centre of start of grid
            map.GridRotation = file.ReadInt();              // Direction that grid faces in

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
                Checkpoint cp = new Checkpoint() { TimerIncrements = file.ReadVector3() };
                int quadCount = file.ReadInt();

                for (int j = 0; j < quadCount; j++)
                {
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                    cp.Points.Add(file.ReadVector3());
                }

                map.Checkpoints.Add(cp);
            }

            int smashSpecs = file.ReadInt();
            for (int i = 0; i < smashSpecs; i++)
            {
                SmashData smashable = new SmashData
                {
                    Flags = file.ReadInt(),
                    Trigger = file.ReadLine()
                };

                if (smashable.Trigger.Length == 3 && smashable.Trigger.StartsWith("&"))
                {
                    smashable.TriggerFlags = file.ReadInt();
                }

                smashable.TriggerMode = file.ReadEnum<SmashData.SmashTriggerMode>();

                switch (smashable.TriggerMode)
                {
                    case SmashData.SmashTriggerMode.NoChange:
                    case SmashData.SmashTriggerMode.Remove:
                        smashable.RemovalThreshold = file.ReadSingle();
                        smashable.Connotations.Load(file);
                        break;

                    case SmashData.SmashTriggerMode.ReplaceModel:
                        smashable.RemovalThreshold = file.ReadSingle();
                        smashable.Connotations.Load(file);
                        smashable.NewModel = file.ReadLine();
                        smashable.ChanceOfFire = file.ReadInt();

                        if (smashable.ChanceOfFire > 0)
                        {
                            smashable.NumFires = file.ReadInt();
                            smashable.SmokeLevel = file.ReadInts();
                        }
                        break;

                    case SmashData.SmashTriggerMode.TextureChange:
                        smashable.IntactMaterial = file.ReadLine();
                        int textureLevels = file.ReadInt();

                        for (int j = 0; j < textureLevels; j++)
                        {
                            SmashDataTextureLevel textureLevel = new SmashDataTextureLevel()
                            {
                                TriggerThreshold = file.ReadSingle(),
                                Flags = file.ReadInt(),
                                CollisionType = file.ReadEnum<SmashDataTextureLevel.TextureLevelCollisionType>()
                            };

                            textureLevel.Connotations.Load(file);

                            int pixelmaps = file.ReadInt();
                            for (int k = 0; k < pixelmaps; k++)
                            {
                                textureLevel.Pixelmaps.Add(file.ReadLine());
                            }

                            smashable.Levels.Add(textureLevel);
                        }
                        break;

                    default:
                        throw new NotImplementedException($"Unknown TriggerMode '{smashable.TriggerMode}'");
                }

                file.ReadInt(); // Reserved 1
                file.ReadInt(); // Reserved 2
                file.ReadInt(); // Reserved 3
                file.ReadInt(); // Reserved 4

                map.Smashables.Add(smashable);
            }

            int pedSpecs = file.ReadInt();

            for (int i = 0; i < pedSpecs; i++)
            {
                PedSpec ps = new PedSpec()
                {
                    MaterialName = file.ReadLine(),
                    MovementIndex = file.ReadInt(),
                    GroupIndex = file.ReadInt(),
                    PedsPer100SquareMetres = file.ReadSingle()
                };

                int exclusionCount = file.ReadInt();
                for (int j = 0; j < exclusionCount; j++)
                {
                    ps.ExclusionMaterials.Add(new PedExclusionMaterial
                    {
                        Flags = file.ReadInt(),
                        Name = file.ReadLine()
                    });
                }

                int exceptionCount = file.ReadInt();
                for (int j = 0; j < exceptionCount; j++)
                {
                    ps.ExceptionMaterials.Add(file.ReadLine());
                }

                map.PedSpecs.Add(ps);
            }

            map.AdditionalActor = file.ReadLine();

            map.SkyPixelmap = file.ReadLine();
            map.HorizontalRepetitions = file.ReadInt();
            map.VerticalSize = file.ReadInt();
            map.PositionOfHorizon = file.ReadInt();
            map.DepthCueMode = file.ReadLine();
            map.FogDarkness = file.ReadVector2();
            map.DepthCueColour = file.ReadVector3();

            map.DefaultEngineNoise = file.ReadInt();

            int specialEffectVolumeCount = file.ReadInt();

            for (int i = 0; i < specialEffectVolumeCount; i++)
            {
                SpecialEffectVolume sev = new SpecialEffectVolume { Type = file.ReadLine().ToUpper() };

                if (sev.Type == "BOX")
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

                if (file.PeekLine().Contains(","))
                {
                    sev.SkyColourRGB = file.ReadVector3();
                }
                else
                {
                    sev.SkyColour = file.ReadInt();
                }
                
                sev.WindscreenMaterial = file.ReadLine();
                sev.EntrySoundID = file.ReadInt();
                sev.ExitSoundID = file.ReadInt();
                sev.EngineNoiseIndex = file.ReadInt();
                sev.MaterialIndex = file.ReadInt();

                if (sev.Type == "BOX")
                {
                    sev.SoundType = file.ReadEnum<SpecialEffectVolume.SpecVolSoundType>();

                    if (sev.SoundType != SpecialEffectVolume.SpecVolSoundType.None)
                    {
                        sev.SoundSpec = SoundSpec.Load(file);
                    }
                }

                map.SpecialVolumes.Add(sev);
            }

            int soundGeneratorCount = file.ReadInt();

            if (soundGeneratorCount > 0)
            {
                throw new NotImplementedException("Can't handle sound generators yet!");
            }

            map.MaterialDefault = file.ReadLine();
            map.MaterialDarkness = file.ReadLine();
            map.MaterialFog = file.ReadLine();

            file.ReadLine();    // (ignore) # areas with different screens

            map.MapPixelmap = file.ReadLine();
            map.WorldMapTransform = new Matrix3D(
                file.ReadVector3(),
                file.ReadVector3(),
                file.ReadVector3(),
                file.ReadVector3()
            );

            if (file.ReadLine() != "START OF FUNK")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Map.TXT file?", "START OF FUNK");
                return null;
            }

            while (file.PeekLine() != "END OF FUNK")
            {
                map.Funks.Add(new Funk(file));
                if (file.PeekLine() == "NEXT FUNK") { file.ReadLine(); }
            }
            file.ReadLine();

            if (file.ReadLine() != "START OF GROOVE")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Map.TXT file?", "START OF GROOVE");
                return null;
            }

            while (file.PeekLine() != "END OF GROOVE")
            {
                map.Grooves.Add(new Groove(file));
                if (file.PeekLine() == "NEXT GROOVE") { file.ReadLine(); }
            }
            file.ReadLine();

            if (file.ReadLine() != "START OF OPPONENT PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            int nodeCount = file.ReadInt();
            for (int i = 0; i < nodeCount; i++)
            {
                map.Nodes.Add(file.ReadVector3());
            }

            int pathCount = file.ReadInt();
            for (int i = 0; i < pathCount; i++)
            {
                map.Paths.Add(new OpponentPathSection(file.ReadLine()));
            }

            int copStartPoints = file.ReadInt();
            if (copStartPoints > 0) { throw new NotImplementedException("Cop start points?  Really?"); }

            if (file.ReadLine() != "END OF OPPONENT PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            if (file.ReadLine() != "START OF DRONE PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            map.DronePathVersion = file.ReadInt();

            int dronepathNodeCount = file.ReadInt();
            for (int i = 0; i < dronepathNodeCount; i++)
            {
                DronePathNode node = new DronePathNode
                {
                    Position = file.ReadVector3(),
                    DroneName = file.ReadLine()
                };

                if (map.DronePathVersion == 0)
                {
                    node.UnknownVector = file.ReadVector3();
                }
                else
                {
                    node.UnknownInt = file.ReadInt();
                }

                int nextNodeCount = file.ReadInt();
                for (int j = 0; j < nextNodeCount; j++)
                {
                    node.Destinations.Add(file.ReadLine());
                }

                map.DronePaths.Add(node);
            }

            if (file.ReadLine() != "END OF DRONE PATHS")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 race .txt file");
                return null;
            }

            int materialModifierCount = file.ReadInt();
            for (int i = 0; i < materialModifierCount; i++)
            {
                map.MaterialModifiers.Add(new MaterialModifier
                {
                    CarWallFriction = file.ReadSingle(),
                    TyreRoadFriction = file.ReadSingle(),
                    DownForce = file.ReadSingle(),
                    Bumpiness = file.ReadSingle(),
                    TyreSoundIndex = file.ReadInt(),
                    CrashSoundIndex = file.ReadInt(),
                    ScrapeNoiseIndex = file.ReadInt(),
                    Sparkiness = file.ReadSingle(),
                    RoomForExpansion = file.ReadInt(),
                    SkidmarkMaterial = file.ReadLine()
                });
            }

            int noncarCount = file.ReadInt();
            for (int i = 0; i < noncarCount; i++)
            {
                map.Noncars.Add(file.ReadLine());
            }

            int shadetableCount = file.ReadInt();
            for (int i = 0; i < shadetableCount; i++)
            {
                map.ShadeTables.Add(new ShadeTable
                {
                    RGB = file.ReadVector3(),
                    Strengths = file.ReadVector3()
                });
            }

            int networkStartCount = file.ReadInt();
            for (int i = 0; i < networkStartCount; i++)
            {
                map.NetworkStarts.Add(new NetworkStart
                {
                    Position = file.ReadVector3(),
                    Rotation = file.ReadInt()
                });
            }

            int splashfileCount = file.ReadInt();
            for (int i = 0; i < splashfileCount; i++)
            {
                map.SplashFiles.Add(file.ReadLine());
            }

            int txtfileCount = file.ReadInt();
            for (int i = 0; i < txtfileCount; i++)
            {
                map.TxtFiles.Add(file.ReadLine());
            }

            return map;
        }
    }

    public class Checkpoint
    {
        public Vector3 TimerIncrements { get; set; }

        public List<Vector3> Points { get; set; } = new List<Vector3>();
    }

    public class PedSpec
    {
        public string MaterialName { get; set; }

        public int MovementIndex { get; set; }

        public int GroupIndex { get; set; }

        public float PedsPer100SquareMetres { get; set; }

        public List<PedExclusionMaterial> ExclusionMaterials { get; set; } = new List<PedExclusionMaterial>();

        public List<string> ExceptionMaterials { get; set; } = new List<string>();
    }

    public class PedExclusionMaterial
    {
        public int Flags { get; set; }

        public string Name { get; set; }
    }

    public class SpecialEffectVolume
    {
        public enum SpecVolSoundType
        {
            None,
            Scattered,
            Saturated
        }

        public string Type { get; set; }

        public float GravityMultiplier { get; set; }

        public float ViscosityMultiplier { get; set; }

        public float CarDamagePerMillisecond { get; set; }

        public float PedDamagePerMillisecond { get; set; }

        public int CameraEffectIndex { get; set; }

        public int SkyColour { get; set; }

        public Vector3 SkyColourRGB { get; set; }

        public string WindscreenMaterial { get; set; }

        public int EntrySoundID { get; set; }

        public int ExitSoundID { get; set; }

        public int EngineNoiseIndex { get; set; }

        public int MaterialIndex { get; set; }

        public List<Vector3> Corners { get; set; } = new List<Vector3>();

        public SpecVolSoundType SoundType { get; set; }

        public SoundSpec SoundSpec { get; set; }
    }

    public class SoundSpec
    {
        public enum SoundSpecType
        {
            Random,
            Continuous
        }

        public SoundSpecType SoundType { get; set; }

        public Vector2 TimeBetween { get; set; }

        public int MaxPercentPitchDeviation { get; set; }

        public List<int> SoundIDs { get; set; } = new List<int>();

        public static SoundSpec Load(DocumentParser file)
        {
            SoundSpec spec = new SoundSpec
            {
                SoundType = file.ReadEnum<SoundSpecType>()
            };

            switch (spec.SoundType)
            {
                case SoundSpecType.Random:
                    spec.TimeBetween = file.ReadVector2();
                    break;
            }

            spec.MaxPercentPitchDeviation = file.ReadInt();
            int numSounds = file.ReadInt();

            for (int i = 0; i < numSounds; i++)
            {
                spec.SoundIDs.Add(file.ReadInt());
            }

            return spec;
        }
    }

    public class DronePathNode
    {
        public Vector3 Position { get; set; }

        public string DroneName { get; set; }

        public int UnknownInt { get; set; }

        public Vector3 UnknownVector { get; set; }

        public List<string> Destinations { get; set; } = new List<string>();
    }

    public class MaterialModifier
    {
        public float CarWallFriction { get; set; }

        public float TyreRoadFriction { get; set; }

        public float DownForce { get; set; }

        public float Bumpiness { get; set; }

        public int TyreSoundIndex { get; set; }

        public int CrashSoundIndex { get; set; }

        public int ScrapeNoiseIndex { get; set; }

        public float Sparkiness { get; set; }

        public int RoomForExpansion { get; set; }

        public string SkidmarkMaterial { get; set; }
    }

    public class ShadeTable
    {
        public Vector3 RGB { get; set; }

        public Vector3 Strengths { get; set; }
    }

    public class NetworkStart
    {
        public Vector3 Position { get; set; }

        public int Rotation { get; set; }
    }
}