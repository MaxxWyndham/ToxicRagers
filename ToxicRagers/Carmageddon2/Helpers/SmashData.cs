using System.Collections.Generic;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class SmashData
    {
        public enum SmashTriggerMode
        {
            nochange,
            remove,
            replacemodel,
            texturechange
        }

        public int Flags { get; set; }

        public string Trigger { get; set; }

        public int TriggerFlags { get; set; }

        public SmashTriggerMode TriggerMode { get; set; }

        public string IntactMaterial { get; set; }

        public List<SmashDataTextureLevel> Levels { get; set; } = new List<SmashDataTextureLevel>();

        public float RemovalThreshold { get; set; }

        public SmashDataConnotations Connotations { get; set; } = new SmashDataConnotations();

        public string NewModel { get; set; }

        public int ChanceOfFire { get; set; }

        public int NumFires { get; set; }

        public int[] SmokeLevel { get; set; }

        public int Reserved1 { get; set; }

        public int Reserved2 { get; set; }

        public int Reserved3 { get; set; }

        public int Reserved4 { get; set; }
    }

    public class SmashDataTextureLevel
    {
        public enum TextureLevelCollisionType
        {
            passthrough,
            solid,
            edges
        }

        public float TriggerThreshold { get; set; }

        public int Flags { get; set; }

        public TextureLevelCollisionType CollisionType { get; set; }

        public SmashDataConnotations Connotations { get; set; } = new SmashDataConnotations();

        public List<string> Pixelmaps { get; set; } = new List<string>();
    }

    public class SmashDataConnotations
    {
        public enum AwardCodeType
        {
            none,
            repeated,
            singleshot,
            doitregardless
        }

        public List<SmashDataExplosion> Explosions { get; set; } = new List<SmashDataExplosion>();

        public List<SmashDataNoncarActivationCuboid> NoncarCuboids { get; set; } = new List<SmashDataNoncarActivationCuboid>();

        public List<SmashDataSmashActivationCuboid> SmashCuboids { get; set; } = new List<SmashDataSmashActivationCuboid>();

        public List<int> Sounds { get; set; } = new List<int>();

        public List<SmashDataShrapnel> Shrapnel { get; set; } = new List<SmashDataShrapnel>();

        public string SlickMaterial { get; set; }

        public int ExtensionFlags { get; set; }

        public int RoomTurnOnCode { get; set; }

        public AwardCodeType AwardCode { get; set; }

        public int PointsAwarded { get; set; }

        public int TimeAwarded { get; set; }

        public int HudIndex { get; set; }

        public int FancyHUDIndex { get; set; }

        public List<string> RuntimeVariableChanges { get; set; } = new List<string>();

        public void Load(DocumentParser file)
        {
            int possibleSounds = file.ReadInt();
            for (int k = 0; k < possibleSounds; k++)
            {
                Sounds.Add(file.ReadInt());
            }

            int shrapnelCount = file.ReadInt();
            for (int k = 0; k < shrapnelCount; k++)
            {
                SmashDataShrapnel shrapnel = new SmashDataShrapnel
                {
                    ShrapnelType = file.ReadEnum<SmashDataShrapnel.SmashDataShrapnelType>()
                };

                shrapnel.InitialVelocity.TowardsYouSpeed = file.ReadVector2();
                shrapnel.InitialVelocity.ImpacteeVelocityFactor = file.ReadSingle();
                shrapnel.InitialVelocity.MaxRandomVelocity = file.ReadSingle();
                shrapnel.InitialVelocity.MaxUpVelocity = file.ReadSingle();
                shrapnel.InitialVelocity.MaxNormalVelocity = file.ReadSingle();
                shrapnel.InitialVelocity.MaxRandomSpinRate = file.ReadSingle();

                if (shrapnel.ShrapnelType != SmashDataShrapnel.SmashDataShrapnelType.shards)
                {
                    shrapnel.InitialPositionType = file.ReadEnum<SmashDataShrapnel.SmashDataInitialPositionType>();

                    if (shrapnel.InitialPositionType == SmashDataShrapnel.SmashDataInitialPositionType.sphereclumped)
                    {
                        shrapnel.ClumpingRadius = file.ReadSingle();
                        shrapnel.ClumpingCentre = file.ReadEnum<SmashDataShrapnel.ClumpCentre>();
                    }
                }

                if (shrapnel.ShrapnelType != SmashDataShrapnel.SmashDataShrapnelType.noncars)
                {
                    shrapnel.Time = file.ReadVector2();
                }

                if (shrapnel.ShrapnelType == SmashDataShrapnel.SmashDataShrapnelType.shards)
                {
                    shrapnel.CutLength = file.ReadSingle();
                    shrapnel.Flags = file.ReadInt();
                    shrapnel.MaterialName = file.ReadLine();
                }
                else if (shrapnel.ShrapnelType == SmashDataShrapnel.SmashDataShrapnelType.ghostparts)
                {
                    int[] count = file.ReadInts();
                    shrapnel.MinCount = count[0];
                    shrapnel.MaxCount = count.Length == 2 ? count[1] : count[0];

                    int numActors = file.ReadInt();
                    if (numActors > 0)
                    {
                        for (int l = 0; l < numActors; l++)
                        {
                            shrapnel.GhostPartActors.Add(file.ReadLine());
                        }
                    } 
                    else
                    {
                        shrapnel.GhostPartActors.Add(file.ReadLine());
                    }
                }
                else if (shrapnel.ShrapnelType == SmashDataShrapnel.SmashDataShrapnelType.noncars)
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
                        shrapnel.Actors.Add(new SmashDataShrapnelActor
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

                Shrapnel.Add(shrapnel);
            }

            int explosionCount = file.ReadInt();
            for (int k = 0; k < explosionCount; k++)
            {
                Explosions.Add(SmashDataExplosion.Load(file));
            }

            SlickMaterial = file.ReadLine();

            int noncarCuboidCount = file.ReadInt();
            for (int k = 0; k < noncarCuboidCount; k++)
            {
                NoncarCuboids.Add(SmashDataNoncarActivationCuboid.Load(file));
            }

            int smashCuboidCount = file.ReadInt();
            for (int k = 0; k < smashCuboidCount; k++)
            {
                SmashCuboids.Add(SmashDataSmashActivationCuboid.Load(file));
            }

            ExtensionFlags = file.ReadInt();
            RoomTurnOnCode = file.ReadInt();
            AwardCode = file.ReadEnum<AwardCodeType>();

            if (AwardCode != AwardCodeType.none)
            {
                PointsAwarded = file.ReadInt();
                TimeAwarded = file.ReadInt();
                HudIndex = file.ReadInt();
                FancyHUDIndex = file.ReadInt();
            }

            int runtimeVariableChanges = file.ReadInt();
            for (int k = 0; k < runtimeVariableChanges; k++)
            {
                RuntimeVariableChanges.Add(file.ReadLine());
            }
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine("// Connotations:");
            dw.WriteLine();

            dw.WriteLine($"{Sounds.Count}", "number of possible sounds");
            foreach (int sound in Sounds) { dw.WriteLine($"{sound}", "sound ID"); }

            dw.WriteLine();

            dw.WriteLine($"{Shrapnel.Count}", "shrapnel count");
            foreach (SmashDataShrapnel shrapnel in Shrapnel) { shrapnel.Write(dw); }

            dw.WriteLine();

            dw.WriteLine($"{Explosions.Count}", "number of explosion groups");
            foreach (SmashDataExplosion explosion in Explosions) { explosion.Write(dw); }

            dw.WriteLine($"{SlickMaterial}", "slick material");

            dw.WriteLine($"{NoncarCuboids.Count}", "no. of non car cuboids activated");
            foreach (SmashDataNoncarActivationCuboid cuboid in NoncarCuboids) { cuboid.Write(dw); }

            dw.WriteLine($"{SmashCuboids.Count}", "Radius of side-effect smashes");
            foreach (SmashDataSmashActivationCuboid cuboid in SmashCuboids) { cuboid.Write(dw); }

            dw.WriteLine($"{ExtensionFlags}", "extensions flags");
            dw.WriteLine($"{RoomTurnOnCode}", "room turn on code");
            dw.WriteLine($"{AwardCode}", "award code");

            if (AwardCode != AwardCodeType.none)
            {
                dw.WriteLine($"{PointsAwarded}");
                dw.WriteLine($"{TimeAwarded}");
                dw.WriteLine($"{HudIndex}");
                dw.WriteLine($"{FancyHUDIndex}");
            }

            dw.WriteLine($"{RuntimeVariableChanges.Count}", "no. run time variable changes");
            foreach (string runtimeVariable in RuntimeVariableChanges)
            {
                dw.WriteLine($"{runtimeVariable}");
            }
        }
    }

    public class SmashDataShrapnel
    {
        public enum SmashDataShrapnelType
        {
            ghostparts,
            shards,
            noncars
        }

        public enum SmashDataInitialPositionType
        {
            actorbased,
            sphereclumped
        }

        public enum ClumpCentre
        {
            impact,
            model
        }

        public SmashDataShrapnelType ShrapnelType { get; set; }

        public SmashDataInitialVelocity InitialVelocity { get; set; } = new SmashDataInitialVelocity();

        public SmashDataInitialPositionType InitialPositionType { get; set; }

        public float ClumpingRadius { get; set; }

        public ClumpCentre ClumpingCentre { get; set; }

        public Vector2 Time { get; set; }

        public int MinCount { get; set; }

        public int MaxCount { get; set; }

        public string Actor { get; set; }

        public float CutLength { get; set; }

        public int Flags { get; set; }

        public string MaterialName { get; set; }

        public int ChanceOfFire { get; set; }

        public int NumFires { get; set; }

        public int[] SmokeLevel { get; set; }

        public List<string> GhostPartActors { get; set; } = new List<string>();

        public List<SmashDataShrapnelActor> Actors { get; set; } = new List<SmashDataShrapnelActor>();

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{ShrapnelType}", "Shrapnel type");
            InitialVelocity.Write(dw);

            if (ShrapnelType != SmashDataShrapnelType.shards)
            {
                dw.WriteLine($"{InitialPositionType}", "How the shrapnel bits are initially placed in the World");

                if (InitialPositionType == SmashDataInitialPositionType.sphereclumped)
                {
                    dw.WriteLine($"{ClumpingRadius}");
                    dw.WriteLine($"{ClumpingCentre}");
                }
            }

            if (ShrapnelType != SmashDataShrapnelType.noncars)
            {
                dw.WriteLine($"{Time.X},{Time.Y}", "Min time, Max time");
            }

            if (ShrapnelType == SmashDataShrapnelType.shards)
            {
                dw.WriteLine($"{CutLength}", "Min cut length");
                dw.WriteLine($"{Flags}", "flags");
                dw.WriteLine($"{MaterialName}", "name of shrapnel material");
            }
            else if (ShrapnelType == SmashDataShrapnelType.ghostparts)
            {
                dw.WriteLine($"{MinCount},{MaxCount}");
                dw.WriteLine($"{GhostPartActors.Count}");
                foreach (string ghostPartActor in GhostPartActors) { dw.WriteLine($"{ghostPartActor}"); }
            }
            else if (ShrapnelType == SmashDataShrapnelType.noncars)
            {
                dw.WriteLine($"{MinCount},{MaxCount}", "Min number,Max number (-1,-1 means use exactly one of each type of bit)");
                dw.WriteLine($"{ChanceOfFire}", "% Chance of fire/smoke");

                if (ChanceOfFire > 0)
                {
                    dw.WriteLine($"{NumFires}", "Number of fires/smoke column");
                    dw.WriteLine($"{string.Join(",", SmokeLevel)}", "Min,Max smokiness (0 = fire, 1 = black smoke, 2 = grey smoke, 3 = white smoke)");
                }

                dw.WriteLine($"{Actor}", "Name of actor file");
                dw.WriteLine($"{Actors.Count}", "Number of separate actors in file");
                foreach (SmashDataShrapnelActor actor in Actors)
                {
                    dw.WriteLine($"{actor.Name}", "Actor name");
                    dw.WriteLine($"{actor.FileName}", "Non-car text file to use for the above actor");
                }
            }
            else
            {
                dw.WriteLine($"{MinCount}");
                dw.WriteLine($"{MaxCount}");
                dw.WriteLine($"{Actor}");
            }
        }
    }

    public class SmashDataShrapnelActor
    {
        public string Name { get; set; }

        public string FileName { get; set; }
    }

    public class SmashDataExplosion
    {
        public enum ExplosionRotationMode
        {
            randomrotate
        }

        public int[] Count { get; set; }

        public Vector2 StartDelay { get; set; }

        public Vector3 Offset { get; set; }

        public Vector2 XFactor { get; set; }

        public Vector2 YFactor { get; set; }

        public Vector2 ZFactor { get; set; }

        public Vector2 FrameRate { get; set; }

        public Vector2 ScalingFactor { get; set; }

        public ExplosionRotationMode RotationMode { get; set; }

        public List<SmashDataExplosionFrame> Frames { get; set; } = new List<SmashDataExplosionFrame>();

        public static SmashDataExplosion Load(DocumentParser file)
        {
            SmashDataExplosion explosion = new SmashDataExplosion
            {
                Count = file.ReadInts(),
                StartDelay = file.ReadVector2(),
                Offset = file.ReadVector3(),
                XFactor = file.ReadVector2(),
                YFactor = file.ReadVector2(),
                ZFactor = file.ReadVector2(),
                FrameRate = file.ReadVector2(),
                ScalingFactor = file.ReadVector2(),
                RotationMode = file.ReadEnum<ExplosionRotationMode>()
            };

            int frameCount = file.ReadInt();

            for (int i = 0; i < frameCount; i++)
            {
                explosion.Frames.Add(new SmashDataExplosionFrame
                {
                    Opacity = file.ReadInt(),
                    Pixelmap = file.ReadLine()
                });
            }

            return explosion;
        }

        public void Write(DocumentWriter dw)
        {
            bool firstLoop = true;

            dw.WriteLine($"{string.Join(",", Count)}", "min count, max count");
            dw.WriteLine($"{StartDelay.X},{StartDelay.Y}", "min start delay, max start delay");
            dw.WriteLine($"{Offset.X},{Offset.Y},{Offset.Z}", "offset");
            dw.WriteLine($"{XFactor.X},{XFactor.Y}", "min x factor, max x factor");
            dw.WriteLine($"{YFactor.X},{YFactor.Y}", "min y factor, max y factor");
            dw.WriteLine($"{ZFactor.X},{ZFactor.Y}", "min z factor, max z factor");
            dw.WriteLine($"{FrameRate.X},{FrameRate.Y}", "min frame rate, max frame rate");
            dw.WriteLine($"{ScalingFactor.X},{ScalingFactor.Y}", "min scaling factor, max scaling factor");
            dw.WriteLine($"{RotationMode}", "rotate mode");
            dw.WriteLine($"{Frames.Count}", "number of frames");
            foreach (SmashDataExplosionFrame frame in Frames)
            {
                dw.WriteLine($"{frame.Opacity}", firstLoop ? "opacity" : null);
                dw.WriteLine($"{frame.Pixelmap}", firstLoop ? "frame pix name" : null);

                firstLoop = false;
            }
            dw.WriteLine();
        }
    }

    public class SmashDataExplosionFrame
    {
        public int Opacity { get; set; }

        public string Pixelmap { get; set; }
    }

    public class SmashDataActivationCuboid
    {
        public enum CuboidCoordinateSystem
        {
            absolute,
            relative
        }

        public Vector2 Delay { get; set; }

        public CuboidCoordinateSystem CoordinateSystem { get; set; }

        public Vector3 Min { get; set; }

        public Vector3 Max { get; set; }
    }

    public class SmashDataNoncarActivationCuboid : SmashDataActivationCuboid
    {
        public int NoncarNumber { get; set; }

        public SmashDataInitialVelocity InitialVelocity { get; set; }

        public static SmashDataNoncarActivationCuboid Load(DocumentParser file)
        {
            SmashDataNoncarActivationCuboid cuboid = new SmashDataNoncarActivationCuboid
            {
                Delay = file.ReadVector2(),
                CoordinateSystem = file.ReadEnum<CuboidCoordinateSystem>(),
                NoncarNumber = file.ReadInt(),
                Min = file.ReadVector3(),
                Max = file.ReadVector3(),
                InitialVelocity = new SmashDataInitialVelocity
                {
                    TowardsYouSpeed = file.ReadVector2(),
                    ImpacteeVelocityFactor = file.ReadSingle(),
                    MaxRandomVelocity = file.ReadSingle(),
                    MaxUpVelocity = file.ReadSingle(),
                    MaxNormalVelocity = file.ReadSingle(),
                    MaxRandomSpinRate = file.ReadSingle()
                }
            };

            return cuboid;
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{Delay.X},{Delay.Y}");
            dw.WriteLine($"{CoordinateSystem}");
            dw.WriteLine($"{NoncarNumber}");
            dw.WriteLine($"{Min.X},{Min.Y},{Min.Z}");
            dw.WriteLine($"{Max.X},{Max.Y},{Max.Z}");
            InitialVelocity.Write(dw);
        }
    }

    public class SmashDataSmashActivationCuboid : SmashDataActivationCuboid
    {
        public enum SmashImpactDirection
        {
            away
        }

        public string Name { get; set; }

        public SmashImpactDirection ImpactDirection { get; set; }

        public float ImpactStrength { get; set; }

        public static SmashDataSmashActivationCuboid Load(DocumentParser file)
        {
            SmashDataSmashActivationCuboid cuboid = new SmashDataSmashActivationCuboid
            {
                Delay = file.ReadVector2(),
                Name = file.ReadLine(),
                CoordinateSystem = file.ReadEnum<CuboidCoordinateSystem>(),
                Min = file.ReadVector3(),
                Max = file.ReadVector3(),
                ImpactDirection = file.ReadEnum<SmashImpactDirection>(),
                ImpactStrength = file.ReadSingle()
            };

            return cuboid;
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{Delay.X},{Delay.Y}");
            dw.WriteLine($"{Name}");
            dw.WriteLine($"{CoordinateSystem}");
            dw.WriteLine($"{Min.X},{Min.Y},{Min.Z}");
            dw.WriteLine($"{Max.X},{Max.Y},{Max.Z}");
            dw.WriteLine($"{ImpactDirection}");
            dw.WriteLine($"{ImpactStrength}");
        }
    }

    public class SmashDataInitialVelocity
    {
        public Vector2 TowardsYouSpeed { get; set; }

        public float ImpacteeVelocityFactor { get; set; }

        public float MaxRandomVelocity { get; set; }

        public float MaxUpVelocity { get; set; }

        public float MaxNormalVelocity { get; set; }

        public float MaxRandomSpinRate { get; set; }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{TowardsYouSpeed.X},{TowardsYouSpeed.Y}", "Min, max towards you speed");
            dw.WriteLine($"{ImpacteeVelocityFactor:F1}", "Impactee velocity factor");
            dw.WriteLine($"{MaxRandomVelocity:F1}", "Random velocity (max)");
            dw.WriteLine($"{MaxUpVelocity:F1}", "Random up velocity (max)");
            dw.WriteLine($"{MaxNormalVelocity}", "Random normal velocity (max)");
            dw.WriteLine($"{MaxRandomSpinRate:F1}", "Random spin rate (max)");
        }
    }
}
