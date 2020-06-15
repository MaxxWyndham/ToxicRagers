using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class SmashData
    {
        public enum SmashTriggerMode
        {
            TextureChange,
            ReplaceModel
        }

        public int Flags { get; set; }

        public string Trigger { get; set; }

        public SmashTriggerMode TriggerMode { get; set; }

        public string IntactMaterial { get; set; }

        public List<SmashDataTextureLevel> Levels { get; set; } = new List<SmashDataTextureLevel>();

        public float RemovalThreshold { get; set; }

        public SmashDataConnotations Connotations { get; set; } = new SmashDataConnotations();

        public string NewModel { get; set; }

        public int ChanceOfFire { get; set; }

        public int NumFires { get; set; }

        public int[] SmokeLevel { get; set; }
    }

    public class SmashDataTextureLevel
    {
        public enum TextureLevelCollisionType
        {
            Passthrough,
            Solid,
            Edges
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
            None,
            Repeated,
            SingleShot
        }

        public List<SmashDataExplosion> Explosions { get; set; }  = new List<SmashDataExplosion>();
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

                if (shrapnel.ShrapnelType != SmashDataShrapnel.SmashDataShrapnelType.Shards)
                {
                    shrapnel.InitialPositionType = file.ReadEnum<SmashDataShrapnel.SmashDataInitialPositionType>();

                    if (shrapnel.InitialPositionType == SmashDataShrapnel.SmashDataInitialPositionType.SphereClumped)
                    {
                        shrapnel.ClumpingRadius = file.ReadSingle();
                        shrapnel.ClumpingCentre = file.ReadEnum<SmashDataShrapnel.ClumpCentre>();
                    }
                }

                if (shrapnel.ShrapnelType != SmashDataShrapnel.SmashDataShrapnelType.NonCars)
                {
                    shrapnel.Time = file.ReadVector2();
                }

                if (shrapnel.ShrapnelType == SmashDataShrapnel.SmashDataShrapnelType.Shards)
                {
                    shrapnel.CutLength = file.ReadSingle();
                    shrapnel.Flags = file.ReadInt();
                    shrapnel.MaterialName = file.ReadLine();
                }
                else if (shrapnel.ShrapnelType == SmashDataShrapnel.SmashDataShrapnelType.NonCars)
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

            }

            int smashCuboidCount = file.ReadInt();
            for (int k = 0; k < smashCuboidCount; k++)
            {
                SmashCuboids.Add(SmashDataSmashActivationCuboid.Load(file));
            }

            ExtensionFlags = file.ReadInt();
            RoomTurnOnCode = file.ReadInt();
            AwardCode = file.ReadEnum<AwardCodeType>();

            if (AwardCode != AwardCodeType.None)
            {
                PointsAwarded = file.ReadInt();
                TimeAwarded = file.ReadInt();
                HudIndex = file.ReadInt();
                FancyHUDIndex = file.ReadInt();
            }

            int runtimeVariableChanges = file.ReadInt();
            for (int k = 0; k < runtimeVariableChanges; k++)
            {

            }
        }
    }

    public class SmashDataShrapnel
    {
        public enum SmashDataShrapnelType
        {
            GhostParts,
            Shards,
            NonCars
        }

        public enum SmashDataInitialPositionType
        {
            ActorBased,
            SphereClumped
        }

        public enum ClumpCentre
        {
            impact
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

        public List<SmashDataShrapnelActor> Actors { get; set; } = new List<SmashDataShrapnelActor>();
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
            RandomRotate
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
            Relative
        }

        public Vector2 Delay { get; set; }

        public CuboidCoordinateSystem CoordinateSystem { get; set; }

        public Vector3 Min { get; set; }

        public Vector3 Max { get; set; }
    }

    public class SmashDataNoncarActivationCuboid : SmashDataActivationCuboid
    {
    }

    public class SmashDataSmashActivationCuboid : SmashDataActivationCuboid
    {
        public enum SmashImpactDirection
        {
            Away
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
    }

    public class SmashDataInitialVelocity
    {
        public Vector2 TowardsYouSpeed { get; set; }

        public float ImpacteeVelocityFactor { get; set; }

        public float MaxRandomVelocity { get; set; }

        public float MaxUpVelocity { get; set; }

        public float MaxNormalVelocity { get; set; }

        public float MaxRandomSpinRate { get; set; }
    }
}
