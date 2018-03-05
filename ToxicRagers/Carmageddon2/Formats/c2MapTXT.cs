using System;
using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Carmageddon2.Helpers;
using ToxicRagers.Helpers;

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
        List<SmashSpec> smashables;
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

        public Vector3 GridPosition
        {
            get => gridPosition;
            set => gridPosition = value;
        }

        public Map()
        {
            checkpoints = new List<Checkpoint>();
            smashables = new List<SmashSpec>();
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
                Checkpoint cp = new Checkpoint() { TimerIncrements = file.ReadVector3() };
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
            for (int i = 0; i < smashSpecs; i++)
            {
                SmashSpec smashable = new SmashSpec
                {
                    Flags = file.ReadInt(),
                    Trigger = file.ReadLine(),
                    TriggerMode = file.ReadEnum<SmashSpec.SmashTriggerMode>()
                };

                switch (smashable.TriggerMode)
                {
                    case SmashSpec.SmashTriggerMode.TextureChange:
                        smashable.IntactMaterial = file.ReadLine();
                        int textureLevels = file.ReadInt();

                        for (int j = 0; j < textureLevels; j++)
                        {
                            SmashSpecTextureLevel textureLevel = new SmashSpecTextureLevel()
                            {
                                TriggerThreshold = file.ReadSingle(),
                                Flags = file.ReadInt()
                            };

                            if (smashable.TriggerMode == SmashSpec.SmashTriggerMode.TextureChange)
                            {
                                textureLevel.CollisionType = file.ReadEnum<SmashSpecTextureLevel.TextureLevelCollisionType>();

                                textureLevel.Connotations.Load(file);

                                int pixelmaps = file.ReadInt();
                                for (int k = 0; k < pixelmaps; k++)
                                {
                                    textureLevel.Pixelmaps.Add(file.ReadLine());
                                }
                            }

                            smashable.Levels.Add(textureLevel);
                        }
                        break;

                    case SmashSpec.SmashTriggerMode.ReplaceModel:
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

                    default:
                        throw new NotImplementedException($"Unknown TriggerMode '{smashable.TriggerMode}'");
                }

                file.ReadInt(); // Reserved 1
                file.ReadInt(); // Reserved 2
                file.ReadInt(); // Reserved 3
                file.ReadInt(); // Reserved 4

                map.smashables.Add(smashable);
            }

            int pedSpecs = file.ReadInt();

            for (int i = 0; i < pedSpecs; i++)
            {
                PedSpec ps = new PedSpec()
                {
                    MaterialName = file.ReadLine(),
                    MovementIndex = file.ReadInt(),
                    GroupIndex = file.ReadInt(),
                    PedsPer100SquareMetres = file.ReadInt()
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
                sev.SkyColour = file.ReadInt();
                sev.WindscreenMaterial = file.ReadLine();
                sev.EntrySoundID = file.ReadInt();
                sev.ExitSoundID = file.ReadInt();
                sev.EngineNoiseIndex = file.ReadInt();
                sev.MaterialIndex = file.ReadInt();

                if (sev.Type != "DEFAULT WATER")
                {
                    sev.SoundType = file.ReadEnum<SpecialEffectVolume.SpecVolSoundType>();

                    if (sev.SoundType != SpecialEffectVolume.SpecVolSoundType.None)
                    {
                        sev.SoundSpec = SoundSpec.Load(file);
                    }
                }

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

            while (file.ReadLine() != "END OF FUNK") { }
            while (file.ReadLine() != "END OF GROOVE") { }

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
    }

    public class Checkpoint
    {
        Vector3 timerIncrement;
        List<Vector3> points;

        public Vector3 TimerIncrements
        {
            get => timerIncrement;
            set => timerIncrement = value;
        }

        public List<Vector3> Points
        {
            get => points;
            set => points = value;
        }

        public Checkpoint()
        {
            points = new List<Vector3>();
        }
    }

    public class SmashSpec
    {
        public enum SmashTriggerMode
        {
            TextureChange,
            ReplaceModel
        }

        int flags;
        string trigger;
        SmashTriggerMode triggerMode;

        string intactMaterial;
        List<SmashSpecTextureLevel> levels = new List<SmashSpecTextureLevel>();

        float removalThreshold;
        SmashSpecConnotations connotations = new SmashSpecConnotations();
        string newModel;
        int chanceOfFire;
        int numFires;
        int[] smokeLevel;

        public int Flags
        {
            get => flags;
            set => flags = value;
        }

        public string Trigger
        {
            get => trigger;
            set => trigger = value;
        }

        public SmashTriggerMode TriggerMode
        {
            get => triggerMode;
            set => triggerMode = value;
        }

        public string IntactMaterial
        {
            get => intactMaterial;
            set => intactMaterial = value;
        }

        public List<SmashSpecTextureLevel> Levels
        {
            get => levels;
            set => levels = value;
        }

        public float RemovalThreshold
        {
            get => removalThreshold;
            set => removalThreshold = value;
        }

        public SmashSpecConnotations Connotations
        {
            get => connotations;
            set => connotations = value;
        }

        public string NewModel
        {
            get => newModel;
            set => newModel = value;
        }

        public int ChanceOfFire
        {
            get => chanceOfFire;
            set => chanceOfFire = value;
        }

        public int NumFires
        {
            get => numFires;
            set => numFires = value;
        }

        public int[] SmokeLevel
        {
            get => smokeLevel;
            set => smokeLevel = value;
        }
    }

    public class SmashSpecTextureLevel
    {
        public enum TextureLevelCollisionType
        {
            Passthrough,
            Solid,
            Edges
        }

        float triggerThreshold;
        int flags;
        TextureLevelCollisionType collisionType;
        SmashSpecConnotations connotations = new SmashSpecConnotations();
        List<string> pixelmaps = new List<string>();

        public float TriggerThreshold
        {
            get => triggerThreshold;
            set => triggerThreshold = value;
        }

        public int Flags
        {
            get => flags;
            set => flags = value;
        }

        public TextureLevelCollisionType CollisionType
        {
            get => collisionType;
            set => collisionType = value;
        }

        public SmashSpecConnotations Connotations
        {
            get => connotations;
            set => connotations = value;
        }

        public List<string> Pixelmaps
        {
            get => pixelmaps;
            set => pixelmaps = value;
        }
    }

    public class SmashSpecConnotations
    {
        public enum AwardCodeType
        {
            None,
            SingleShot
        }

        List<int> sounds = new List<int>();
        List<SmashSpecShrapnel> shrapnels = new List<SmashSpecShrapnel>();
        List<SmashSpecExplosion> explosions = new List<SmashSpecExplosion>();
        string slickMaterial;
        List<SmashSpecNoncarActivationCuboid> noncarCuboids = new List<SmashSpecNoncarActivationCuboid>();
        List<SmashSpecSmashActivationCuboid> smashCuboids = new List<SmashSpecSmashActivationCuboid>();
        int extensionFlags;
        int roomTurnOnCode;

        AwardCodeType awardCode;
        int pointsAwarded;
        int timeAwarded;
        int hudIndex;
        int fancyHUDIndex;

        public List<int> Sounds
        {
            get => sounds;
            set => sounds = value;
        }

        public List<SmashSpecShrapnel> Shrapnel
        {
            get => shrapnels;
            set => shrapnels = value;
        }

        public string SlickMaterial
        {
            get => slickMaterial;
            set => slickMaterial = value;
        }

        public int ExtensionFlags
        {
            get => extensionFlags;
            set => extensionFlags = value;
        }

        public int RoomTurnOnCode
        {
            get => roomTurnOnCode;
            set => roomTurnOnCode = value;
        }

        public AwardCodeType AwardCode
        {
            get => awardCode;
            set => awardCode = value;
        }

        public void Load(DocumentParser file)
        {
            int possibleSounds = file.ReadInt();
            for (int k = 0; k < possibleSounds; k++)
            {
                sounds.Add(file.ReadInt());
            }

            int shrapnelCount = file.ReadInt();
            for (int k = 0; k < shrapnelCount; k++)
            {
                SmashSpecShrapnel shrapnel = new SmashSpecShrapnel
                {
                    ShrapnelType = file.ReadEnum<SmashSpecShrapnel.SmashSpecShrapnelType>()
                };

                shrapnel.InitialVelocity.TowardsYouSpeed = file.ReadVector2();
                shrapnel.InitialVelocity.ImpacteeVelocityFactor = file.ReadSingle();
                shrapnel.InitialVelocity.MaxRandomVelocity = file.ReadSingle();
                shrapnel.InitialVelocity.MaxUpVelocity = file.ReadSingle();
                shrapnel.InitialVelocity.MaxNormalVelocity = file.ReadSingle();
                shrapnel.InitialVelocity.MaxRandomSpinRate = file.ReadSingle();

                if (shrapnel.ShrapnelType != SmashSpecShrapnel.SmashSpecShrapnelType.Shards)
                {
                    shrapnel.InitialPositionType = file.ReadEnum<SmashSpecShrapnel.SmashSpecInitialPositionType>();
                }

                if (shrapnel.ShrapnelType != SmashSpecShrapnel.SmashSpecShrapnelType.NonCars)
                {
                    shrapnel.Time = file.ReadVector2();
                }

                if (shrapnel.ShrapnelType == SmashSpecShrapnel.SmashSpecShrapnelType.Shards)
                {
                    shrapnel.CutLength = file.ReadSingle();
                    shrapnel.Flags = file.ReadInt();
                    shrapnel.MaterialName = file.ReadLine();
                }
                else if (shrapnel.ShrapnelType == SmashSpecShrapnel.SmashSpecShrapnelType.NonCars)
                {
                    int[] count = file.ReadInts();
                    shrapnel.MinCount = count[0];
                    shrapnel.MaxCount = count[1];
                    shrapnel.ChanceOfFire = file.ReadInt();

                    if (shrapnel.ChanceOfFire > 0)
                    {
                        shrapnel.NumFires = file.ReadInt();
                        shrapnel.SmokeLevel = file.ReadInts();
                    }

                    shrapnel.Actor = file.ReadLine();
                    int numActors = file.ReadInt();

                    for (int l = 0; l < numActors; l++)
                    {
                        shrapnel.Actors.Add(new SmashSpecShrapnelActor
                        {
                            Name = file.ReadLine(),
                            FileName = file.ReadLine()
                        });
                    }
                }
                else
                {
                    shrapnel.MinCount = file.ReadInt();
                    shrapnel.MaxCount = file.ReadInt();
                    shrapnel.Actor = file.ReadLine();
                }

                shrapnels.Add(shrapnel);
            }

            int explosionCount = file.ReadInt();
            for (int k = 0; k < explosionCount; k++)
            {
                explosions.Add(SmashSpecExplosion.Load(file));
            }

            slickMaterial = file.ReadLine();

            int noncarCuboidCount = file.ReadInt();
            for (int k = 0; k < noncarCuboidCount; k++)
            {

            }

            int smashCuboidCount = file.ReadInt();
            for (int k = 0; k < smashCuboidCount; k++)
            {
                smashCuboids.Add(SmashSpecSmashActivationCuboid.Load(file));
            }

            extensionFlags = file.ReadInt();
            roomTurnOnCode = file.ReadInt();
            awardCode = file.ReadEnum<AwardCodeType>();

            if (awardCode != AwardCodeType.None)
            {
                pointsAwarded = file.ReadInt();
                timeAwarded = file.ReadInt();
                hudIndex = file.ReadInt();
                fancyHUDIndex = file.ReadInt();
            }

            int runtimeVariableChanges = file.ReadInt();
            for (int k = 0; k < runtimeVariableChanges; k++)
            {

            }
        }
    }

    public class SmashSpecShrapnel
    {
        public enum SmashSpecShrapnelType
        {
            GhostParts,
            Shards,
            NonCars
        }

        public enum SmashSpecInitialPositionType
        {
            ActorBased
        }

        SmashSpecShrapnelType shrapnelType;
        SmashSpecInitialVelocity initialVelocity = new SmashSpecInitialVelocity();
        SmashSpecInitialPositionType initialPositionType;
        Vector2 time;
        int minCount;
        int maxCount;
        string actor;
        float minCutLength;
        int flags;
        string materialName;
        int chanceOfFire;
        int numFires;
        int[] smokeLevel;
        List<SmashSpecShrapnelActor> actors = new List<SmashSpecShrapnelActor>();

        public SmashSpecShrapnelType ShrapnelType
        {
            get => shrapnelType;
            set => shrapnelType = value;
        }

        public SmashSpecInitialVelocity InitialVelocity
        {
            get => initialVelocity;
            set => initialVelocity = value;
        }

        public SmashSpecInitialPositionType InitialPositionType
        {
            get => initialPositionType;
            set => initialPositionType = value;
        }

        public Vector2 Time
        {
            get => time;
            set => time = value;
        }

        public int MinCount
        {
            get => minCount;
            set => minCount = value;
        }

        public int MaxCount
        {
            get => maxCount;
            set => maxCount = value;
        }

        public string Actor
        {
            get => actor;
            set => actor = value;
        }

        public float CutLength
        {
            get => minCutLength;
            set => minCutLength = value;
        }

        public int Flags
        {
            get => flags;
            set => flags = value;
        }

        public string MaterialName
        {
            get => materialName;
            set => materialName = value;
        }

        public int ChanceOfFire
        {
            get => chanceOfFire;
            set => chanceOfFire = value;
        }

        public int NumFires
        {
            get => numFires;
            set => numFires = value;
        }

        public int[] SmokeLevel
        {
            get => smokeLevel;
            set => smokeLevel = value;
        }

        public List<SmashSpecShrapnelActor> Actors
        {
            get => actors;
            set => actors = value;
        }
    }

    public class SmashSpecShrapnelActor
    {
        string name;
        string fileName;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }
    }

    public class SmashSpecExplosion
    {
        public enum ExplosionRotationMode
        {
            RandomRotate
        }

        int[] count;
        Vector2 startDelay;
        Vector3 offset;
        Vector2 xFactor;
        Vector2 yFactor;
        Vector2 zFactor;
        Vector2 frameRate;
        Vector2 scalingFactor;
        ExplosionRotationMode rotationMode;
        List<SmashSpecExplosionFrame> frames = new List<SmashSpecExplosionFrame>();

        public static SmashSpecExplosion Load(DocumentParser file)
        {
            SmashSpecExplosion explosion = new SmashSpecExplosion
            {
                count = file.ReadInts(),
                startDelay = file.ReadVector2(),
                offset = file.ReadVector3(),
                xFactor = file.ReadVector2(),
                yFactor = file.ReadVector2(),
                zFactor = file.ReadVector2(),
                frameRate = file.ReadVector2(),
                scalingFactor = file.ReadVector2(),
                rotationMode = file.ReadEnum<ExplosionRotationMode>()
            };

            int frameCount = file.ReadInt();

            for (int i = 0; i < frameCount; i++)
            {
                explosion.frames.Add(new SmashSpecExplosionFrame
                {
                    Opacity = file.ReadInt(),
                    Pixelmap = file.ReadLine()
                });
            }

            return explosion;
        }
    }

    public class SmashSpecExplosionFrame
    {
        int opacity;
        string pixelMap;

        public int Opacity
        {
            get => opacity;
            set => opacity = value;
        }

        public string Pixelmap
        {
            get => pixelMap;
            set => pixelMap = value;
        }
    }

    public class SmashSpecActivationCuboid
    {
        public enum CuboidCoordinateSystem
        {
            Relative
        }

        Vector2 delay;
        CuboidCoordinateSystem coordinateSystem;
        Vector3 min;
        Vector3 max;

        public Vector2 Delay
        {
            get => delay;
            set => delay = value;
        }

        public CuboidCoordinateSystem CoordinateSystem
        {
            get => coordinateSystem;
            set => coordinateSystem = value;
        }

        public Vector3 Min
        {
            get => min;
            set => min = value;
        }

        public Vector3 Max
        {
            get => max;
            set => max = value;
        }
    }

    public class SmashSpecNoncarActivationCuboid : SmashSpecActivationCuboid
    {
    }

    public class SmashSpecSmashActivationCuboid : SmashSpecActivationCuboid
    {
        public enum SmashImpactDirection
        {
            Away
        }

        string name;
        SmashImpactDirection impactDirection;
        float impactStrength;

        public static SmashSpecSmashActivationCuboid Load(DocumentParser file)
        {
            SmashSpecSmashActivationCuboid cuboid = new SmashSpecSmashActivationCuboid
            {
                Delay = file.ReadVector2(),
                name = file.ReadLine(),
                CoordinateSystem = file.ReadEnum<CuboidCoordinateSystem>(),
                Min = file.ReadVector3(),
                Max = file.ReadVector3(),
                impactDirection = file.ReadEnum<SmashImpactDirection>(),
                impactStrength = file.ReadSingle()
            };

            return cuboid;
        }
    }

    public class SmashSpecInitialVelocity
    {
        Vector2 towardsYouSpeed;
        float impacteeVelocityFactor;
        float maxRandomVelocity;
        float maxUpVelocity;
        float maxNormalVelocity;
        float maxRandomSpinRate;

        public Vector2 TowardsYouSpeed
        {
            get => towardsYouSpeed;
            set => towardsYouSpeed = value;
        }

        public float ImpacteeVelocityFactor
        {
            get => impacteeVelocityFactor;
            set => impacteeVelocityFactor = value;
        }

        public float MaxRandomVelocity
        {
            get => maxRandomVelocity;
            set => maxRandomVelocity = value;
        }

        public float MaxUpVelocity
        {
            get => maxUpVelocity;
            set => maxUpVelocity = value;
        }

        public float MaxNormalVelocity
        {
            get => maxNormalVelocity;
            set => maxNormalVelocity = value;
        }

        public float MaxRandomSpinRate
        {
            get => maxRandomSpinRate;
            set => maxRandomSpinRate = value;
        }
    }

    public class PedSpec
    {
        string materialName;
        int movementIndex;
        int groupIndex;
        int pedsPer100SquareMetres;
        List<PedExclusionMaterial> exclusionMaterials;
        List<string> exceptionMaterials;

        public string MaterialName
        {
            get => materialName;
            set => materialName = value;
        }

        public int MovementIndex
        {
            get => movementIndex;
            set => movementIndex = value;
        }

        public int GroupIndex
        {
            get => groupIndex;
            set => groupIndex = value;
        }

        public int PedsPer100SquareMetres
        {
            get => pedsPer100SquareMetres;
            set => pedsPer100SquareMetres = value;
        }

        public List<PedExclusionMaterial> ExclusionMaterials
        {
            get => exclusionMaterials;
            set => exclusionMaterials = value;
        }

        public List<string> ExceptionMaterials
        {
            get => exceptionMaterials;
            set => exceptionMaterials = value;
        }

        public PedSpec()
        {
            exclusionMaterials = new List<PedExclusionMaterial>();
            exceptionMaterials = new List<string>();
        }
    }

    public class PedExclusionMaterial
    {
        int flags;
        string name;

        public int Flags
        {
            get => flags;
            set => flags = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }
    }

    public class SpecialEffectVolume
    {
        public enum SpecVolSoundType
        {
            None,
            Scattered,
            Saturated
        }

        string volumeType;
        float gravityMultiplier;
        float viscosityMultiplier;
        float carDamagePerMillisecond;
        float pedDamagePerMillisecond;
        int cameraEffectIndex;
        int skyColour;
        string windscreenMaterial;
        int entrySoundID;
        int exitSoundID;
        int engineNoiseIndex;
        int materialIndex;
        List<Vector3> corners;
        SpecVolSoundType soundType;
        SoundSpec soundSpec;

        public string Type
        {
            get => volumeType;
            set => volumeType = value;
        }

        public float GravityMultiplier
        {
            get => gravityMultiplier;
            set => gravityMultiplier = value;
        }

        public float ViscosityMultiplier
        {
            get => viscosityMultiplier;
            set => viscosityMultiplier = value;
        }

        public float CarDamagePerMillisecond
        {
            get => carDamagePerMillisecond;
            set => carDamagePerMillisecond = value;
        }

        public float PedDamagePerMillisecond
        {
            get => pedDamagePerMillisecond;
            set => pedDamagePerMillisecond = value;
        }

        public int CameraEffectIndex
        {
            get => cameraEffectIndex;
            set => cameraEffectIndex = value;
        }

        public int SkyColour
        {
            get => skyColour;
            set => skyColour = value;
        }

        public string WindscreenMaterial
        {
            get => windscreenMaterial;
            set => windscreenMaterial = value;
        }

        public int EntrySoundID
        {
            get => entrySoundID;
            set => entrySoundID = value;
        }

        public int ExitSoundID
        {
            get => exitSoundID;
            set => exitSoundID = value;
        }

        public int EngineNoiseIndex
        {
            get => engineNoiseIndex;
            set => engineNoiseIndex = value;
        }

        public int MaterialIndex
        {
            get => materialIndex;
            set => materialIndex = value;
        }

        public List<Vector3> Corners
        {
            get => corners;
            set => corners = value;
        }

        public SpecVolSoundType SoundType
        {
            get => soundType;
            set => soundType = value;
        }

        public SoundSpec SoundSpec
        {
            get => soundSpec;
            set => soundSpec = value;
        }

        public SpecialEffectVolume()
        {
            corners = new List<Vector3>();
        }
    }

    public class SoundSpec
    {
        public enum SoundSpecType
        {
            Random,
            Continuous
        }

        SoundSpecType soundSpecType;
        Vector2 timeBetween;
        int maxPercentPitchDeviation;
        List<int> soundIDs;

        public SoundSpecType SoundType
        {
            get => soundSpecType;
            set => soundSpecType = value;
        }

        public Vector2 TimeBetween
        {
            get => timeBetween;
            set => timeBetween = value;
        }

        public SoundSpec()
        {
            soundIDs = new List<int>();
        }

        public static SoundSpec Load(DocumentParser file)
        {
            SoundSpec spec = new SoundSpec
            {
                soundSpecType = file.ReadEnum<SoundSpecType>()
            };

            switch (spec.soundSpecType)
            {
                case SoundSpecType.Random:
                    spec.timeBetween = file.ReadVector2();
                    break;
            }

            spec.maxPercentPitchDeviation = file.ReadInt();
            int numSounds = file.ReadInt();

            for (int i = 0; i < numSounds; i++)
            {
                spec.soundIDs.Add(file.ReadInt());
            }

            return spec;
        }
    }
}