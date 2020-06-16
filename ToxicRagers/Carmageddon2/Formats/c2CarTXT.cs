using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class Car
    {
        public enum PartType
        {
            none,
            normal
        }

        public int Version { get; set; }

        public bool FemaleDriver { get; set; }

        [Description("Name of car")]
        public string Name { get; set; }

        [Description("softness_factor")]
        public float SoftnessFactor { get; set; }

        [Description("Offset of driver's head in 3D space")]
        public Vector3 DriversHeadOffset { get; set; }

        [Description("Angles to turn to make head go left and right")]
        public Vector2 DriversHeadTurnAngles { get; set; }

        [Description("Offset of 'mirror camera' in 3D space, viewing angle of mirror")]
        public Vector4 MirrorCamera { get; set; }

        [Description("Pratcam border names (left, top, right, bottom)")]
        public string[] PratcamBorders { get; set; }

        [Description("Engine noise (normal, enclosed space, underwater)")]
        public int[] EngineNoises { get; set; }

        [Description("Can be stolen")]
        public bool Stealworthy { get; set; }

        public ImpactSpec ImpactTop { get; set; }
        public ImpactSpec ImpactBottom { get; set; }
        public ImpactSpec ImpactLeft { get; set; }
        public ImpactSpec ImpactRight { get; set; }
        public ImpactSpec ImpactFront { get; set; }
        public ImpactSpec ImpactBack { get; set; }

        [Description("Grid image (opponent, frank, annie)")]
        public string[] GridImages { get; set; }

        [Description("Number of extra levels of detail")]
        public List<int> ExtraLevelsOfDetail { get; set; } = new List<int>();

        [Description("crush data file (will be incorporated into this file)")]
        public string WAM { get; set; }

        [Description("Name of reflective screen material (or none if non-reflective)")]
        public string ReflectiveScreenMaterial { get; set; }

        [Description("Percentage transparency of windscreen")]
        public float TransparencyOfWindscreen { get; set; }

        [Description("Number of steerable wheels")]
        public List<int> SteerableWheels { get; set; } = new List<int>();
        public int[] LeftFrontSuspension { get; set; }
        public int[] RightFrontSuspension { get; set; }
        public int[] LeftRearSuspension { get; set; }
        public int[] RightRearSuspension { get; set; }
        public int[] DrivenWheels { get; set; }
        public int[] NonDrivenWheels { get; set; }
        public float DrivenWheelDiameter { get; set; }
        public float NonDrivenWheelDiameter { get; set; }
        public List<Funk> Funks { get; set; } = new List<Funk>();
        public List<Groove> Grooves { get; set; } = new List<Groove>();
        public float MinimumTurningCircle { get; set; }
        public float BrakeMultiplier { get; set; }
        public float BrakingStrengthMultiplier { get; set; }
        public float NumberOfGears { get; set; }
        public float TopGearRedlineSpeed { get; set; }
        public float TopGearAcceleration { get; set; }
        public PartType RootPartType { get; set; }
        public string RootPartIdentifier { get; set; }
        public string RootPartActor { get; set; }
        public PartType SubPartType { get; set; }
        public Vector3 CentreOfMass { get; set; }
        public float Mass { get; set; }
        public Vector3 AngularMomentumProportions { get; set; }
        public float DownforceToWeightBalanceSpeed { get; set; }
        public List<Wheel> Wheels { get; set; } = new List<Wheel>();
        public List<BoundingShape> BoundingShapes { get; set; } = new List<BoundingShape>();
        public List<object> SubParts { get; set; } = new List<object>();
        public List<string> Shrapnel { get; set; } = new List<string>();
        public List<int> FirePoints { get; set; } = new List<int>();
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();

        public static Car Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            Car car = new Car();

            string version = file.ReadLine();

            if (!version.StartsWith("VERSION"))
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon 2 car .txt file");
                return null;
            }

            car.Version = version.Replace("VERSION ", "").ToInt();

            if (file.PeekLine() == "GIRL")
            {
                car.FemaleDriver = true;
                file.ReadLine();
            }

            car.Name = file.ReadLine();
            car.SoftnessFactor = file.ReadSingle();

            if (file.ReadLine() != "START OF DRIVABLE STUFF")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "START OF DRIVABLE STUFF");
                return null;
            }

            car.DriversHeadOffset = file.ReadVector3();
            car.DriversHeadTurnAngles = file.ReadVector2();
            car.MirrorCamera = file.ReadVector4();
            car.PratcamBorders = file.ReadStrings();

            if (file.ReadLine() != "END OF DRIVABLE STUFF")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "END OF DRIVABLE STUFF");
                return null;
            }

            car.EngineNoises = file.ReadInts();
            car.Stealworthy = file.ReadLine().ToLower() == "stealworthy";

            car.ImpactTop = new ImpactSpec("top", file);
            car.ImpactBottom = new ImpactSpec("bottom", file);
            car.ImpactLeft = new ImpactSpec("left", file);
            car.ImpactRight = new ImpactSpec("right", file);
            car.ImpactFront = new ImpactSpec("front", file);
            car.ImpactBack = new ImpactSpec("back", file);

            car.GridImages = file.ReadStrings();

            int extraLevelsOfDetail = file.ReadInt();

            for (int j = 0; j < extraLevelsOfDetail; j++)
            {
                car.ExtraLevelsOfDetail.Add(file.ReadInt());
            }

            car.WAM = file.ReadLine();

            car.ReflectiveScreenMaterial = file.ReadLine();
            car.TransparencyOfWindscreen = file.ReadSingle();

            int steerableWheelsCount = file.ReadInt();
            for (int j = 0; j < steerableWheelsCount; j++)
            {
                car.SteerableWheels.Add(file.ReadInt());
            }

            car.LeftFrontSuspension = file.ReadInts();
            car.RightFrontSuspension = file.ReadInts();
            car.LeftRearSuspension = file.ReadInts();
            car.RightRearSuspension = file.ReadInts();
            car.DrivenWheels = file.ReadInts();
            car.NonDrivenWheels = file.ReadInts();

            car.DrivenWheelDiameter = file.ReadSingle();
            car.NonDrivenWheelDiameter = file.ReadSingle();

            if (file.ReadLine() != "START OF FUNK")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "START OF FUNK");
                return null;
            }

            while (file.PeekLine() != "END OF FUNK")
            {
                car.Funks.Add(new Funk(file));
                if (file.PeekLine() == "NEXT FUNK") { file.ReadLine(); }
            }
            file.ReadLine();

            if (file.ReadLine() != "START OF GROOVE")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "START OF GROOVE");
                return null;
            }

            while (file.PeekLine() != "END OF GROOVE")
            {
                car.Grooves.Add(new Groove(file));
                if (file.PeekLine() == "NEXT GROOVE") { file.ReadLine(); }
            }
            file.ReadLine();

            int _ = file.ReadLine().Replace("START OF MECHANICS STUFF version ", "", StringComparison.InvariantCultureIgnoreCase).ToInt();

            car.MinimumTurningCircle = file.ReadSingle();
            car.BrakeMultiplier = file.ReadSingle();
            car.BrakingStrengthMultiplier = file.ReadSingle();
            car.NumberOfGears = file.ReadSingle();
            car.TopGearRedlineSpeed = file.ReadSingle();
            car.TopGearAcceleration = file.ReadSingle();

            car.RootPartType = file.ReadEnum<PartType>();
            car.RootPartIdentifier = file.ReadLine();
            car.RootPartActor = file.ReadLine();

            car.SubPartType = file.ReadEnum<PartType>();

            car.CentreOfMass = file.ReadVector3();
            car.Mass = file.ReadSingle();
            car.AngularMomentumProportions = file.ReadVector3();
            car.DownforceToWeightBalanceSpeed = file.ReadSingle();

            int numberOfWheels = file.ReadInt();

            for (int j = 0; j < numberOfWheels; j++)
            {
                Wheel wheel = new Wheel
                {
                    Type = file.ReadEnum<Wheel.WheelType>(),
                    Identifier = file.ReadLine(),
                    Actor = file.ReadLine(),
                    Position = file.ReadVector3(),
                    SteerableFlags = file.ReadInt(),
                    DrivenFlags = file.ReadInt(),
                    SuspensionGive = file.ReadSingle(),
                    DampingFactor = file.ReadSingle(),
                    SlipFrictionReductionFraction = file.ReadSingle(),
                    FrictionAngles = file.ReadVector2(),
                    TractionFractionalMultiplier = file.ReadSingle(),
                    RollingResistance = file.ReadSingle()
                };

                car.Wheels.Add(wheel);
            }

            int boundingShapes = file.ReadInt();

            for (int j = 0; j < boundingShapes; j++)
            {
                BoundingShape shape = new BoundingShape
                {
                    Type = file.ReadEnum<BoundingShape.BoundingShapeType>()
                };

                int shapePoints = file.ReadInt();

                for (int k = 0; k < shapePoints; k++)
                {
                    shape.Points.Add(file.ReadVector3());
                }

                car.BoundingShapes.Add(shape);
            }

            int subParts = file.ReadInt();

            for (int j = 0; j < subParts; j++)
            {
                throw new NotImplementedException("subparts?!");
            }

            if (file.ReadLine() != "END OF MECHANICS STUFF")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "END OF MECHANICS STUFF");
                return null;
            }

            int shrapnelCount = file.ReadInt();
            for (int i = 0; i < shrapnelCount; i++)
            {
                car.Shrapnel.Add(file.ReadLine());
            }

            for (int i = 0; i < 12; i++)
            {
                car.FirePoints.Add(file.ReadInt());
            }

            while (file.PeekLine() != "END")
            {
                string keyWord = file.ReadLine();

                switch (keyWord)
                {
                    case "CAMERA_POSITIONS":
                        car.Keywords.Add(new CameraPosition
                        {
                            BumperPosition = file.ReadVector3(),
                            CockpitPosition = file.ReadVector3()
                        });
                        break;

                    case "CAMERA_TURN_OFF_MATERIALS":
                        {
                            CameraTurnOffMaterials ctom = new CameraTurnOffMaterials();

                            int materialCount = file.ReadInt();

                            for (int i = 0; i < materialCount; i++)
                            {
                                CameraTurnOffMaterialEntry ctomEntry = new CameraTurnOffMaterialEntry
                                {
                                    MaterialName = file.ReadLine()
                                };

                                int entryCount = file.ReadInt();

                                for (int j =0; j < entryCount; j++)
                                {
                                    ctomEntry.Materials.Add(file.ReadLine());
                                }

                                ctom.Entries.Add(ctomEntry);
                            }

                            car.Keywords.Add(ctom);
                        }
                        break;

                    default:
                        throw new NotImplementedException($"{keyWord} not supported!");
                }
            }

            return car;
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine($"VERSION {Version}");
                sw.WriteLine("//	Version 1 :		New crush data");
                if (Version > 1) { sw.WriteLine("//		2 :		New windscreen spec"); }
                sw.WriteLine($"{(FemaleDriver ? "GIRL" : "")}");
                sw.WriteLine($"{Name}");

                sw.WriteLine();

                sw.WriteLine($"{SoftnessFactor}");

                sw.WriteLine();

                sw.WriteLine("START OF DRIVABLE STUFF");
                sw.WriteLine();
                sw.WriteLine($"{DriversHeadOffset.X},{DriversHeadOffset.Y},{DriversHeadOffset.Z}");
                sw.WriteLine($"{DriversHeadTurnAngles.X},{DriversHeadTurnAngles.Y}");
                sw.WriteLine($"{MirrorCamera.X},{MirrorCamera.Y},{MirrorCamera.Z},{MirrorCamera.W}");
                sw.WriteLine($"{string.Join(",", PratcamBorders)}");
                sw.WriteLine();
                sw.WriteLine("END OF DRIVABLE STUFF");

                sw.WriteLine();

                sw.WriteLine($"{string.Join(",",EngineNoises)}");

                sw.WriteLine();

                sw.WriteLine($"{(Stealworthy ? "stealworthy" : "")}");

                sw.WriteLine();

                foreach (ImpactSpec impactSpec in new List<ImpactSpec> { ImpactTop, ImpactBottom, ImpactLeft, ImpactRight, ImpactFront, ImpactBack })
                {
                    sw.WriteLine($"// Damage info for {impactSpec.Description} impacts");
                    sw.WriteLine($"{impactSpec.Clauses.Count}");
                    foreach (ImpactSpecClause clause in impactSpec.Clauses)
                    {
                        sw.WriteLine($"\t{clause.Clause}");
                        sw.WriteLine($"\t{clause.Systems.Count}");

                        foreach (ImpactSpecClauseSystem system in clause.Systems)
                        {
                            sw.WriteLine($"\t\t{system.Part},{system.Damage}");
                        }
                    }
                }

                sw.WriteLine();

                sw.WriteLine($"{string.Join(",", GridImages)}");

                sw.WriteLine();

                sw.WriteLine($"{ExtraLevelsOfDetail.Count}");
                foreach (int extraLevelOfDetail in ExtraLevelsOfDetail)
                {
                    sw.WriteLine($"{extraLevelOfDetail}");
                }

                sw.WriteLine();

                sw.WriteLine($"{WAM}");

                sw.WriteLine();

                sw.WriteLine($"{ReflectiveScreenMaterial}");
                sw.WriteLine($"{TransparencyOfWindscreen}");

                sw.WriteLine();

                sw.WriteLine($"{SteerableWheels.Count}");
                foreach (int steerableWheel in SteerableWheels)
                {
                    sw.WriteLine($"{steerableWheel}");
                }

                sw.WriteLine();

                sw.WriteLine($"{string.Join(",", LeftFrontSuspension)}");
                sw.WriteLine($"{string.Join(",", RightFrontSuspension)}");
                sw.WriteLine($"{string.Join(",", LeftRearSuspension)}");
                sw.WriteLine($"{string.Join(",", RightRearSuspension)}");

                sw.WriteLine();

                sw.WriteLine($"{string.Join(",", DrivenWheels)}");
                sw.WriteLine($"{string.Join(",", NonDrivenWheels)}");

                sw.WriteLine();

                sw.WriteLine($"{DrivenWheelDiameter}");
                sw.WriteLine($"{NonDrivenWheelDiameter}");

                sw.WriteLine();
                sw.WriteLine("START OF FUNK");
                sw.WriteLine();

                for (int i = 0; i < Funks.Count; i++)
                {
                    Funk funk = Funks[i];

                    sw.WriteLine($"{funk.Material}");
                    sw.WriteLine($"{funk.Mode}");
                    sw.WriteLine($"{funk.MatrixModType}");
                    if (funk.MatrixModType != FunkMatrixMode.None) { sw.WriteLine($"{funk.MatrixModMode}"); }

                    switch (funk.MatrixModType)
                    {
                        case FunkMatrixMode.roll:
                            sw.WriteLine($"{funk.RollPeriods.X},{funk.RollPeriods.Y}");
                            break;

                        case FunkMatrixMode.slither:
                            sw.WriteLine($"{funk.SlitherSpeed.X},{funk.SlitherSpeed.Y}");
                            sw.WriteLine($"{funk.SlitherAmount.X},{funk.SlitherAmount.Y}");
                            break;

                        case FunkMatrixMode.spin:
                            sw.WriteLine($"{funk.SpinPeriod}");
                            break;
                    }

                    sw.WriteLine($"{funk.LightingMode}");
                    sw.WriteLine($"{funk.AnimationType}");

                    switch (funk.AnimationType)
                    {
                        case FunkAnimationType.frames:
                            sw.WriteLine($"{funk.Framerate}");
                            sw.WriteLine($"{funk.FrameMode}");

                            switch (funk.FrameMode)
                            {
                                case FrameType.texturebits:
                                    sw.WriteLine($"{funk.TextureBitMode}");
                                    break;

                                case FrameType.continuous:
                                    sw.WriteLine($"{funk.FrameSpeed}");
                                    break;
                            }

                            sw.WriteLine($"{funk.Frames.Count}");
                            foreach (string frame in funk.Frames)
                            {
                                sw.WriteLine($"{frame}");
                            }
                            break;
                    }

                    if (i + 1 != Funks.Count)
                    {
                        sw.WriteLine();
                        sw.WriteLine("NEXT FUNK");
                        sw.WriteLine();
                    }
                }

                sw.WriteLine();
                sw.WriteLine("END OF FUNK");
                sw.WriteLine();

                sw.WriteLine();
                sw.WriteLine("START OF GROOVE");
                sw.WriteLine();

                for (int i = 0; i < Grooves.Count; i++)
                {
                    Groove groove = Grooves[i];

                    sw.WriteLine($"{groove.Part}");
                    sw.WriteLine($"{groove.LollipopMode}");
                    sw.WriteLine($"{groove.Mode}");
                    sw.WriteLine($"{groove.PathType}");
                    if (groove.PathType != GroovePathNames.None) { sw.WriteLine($"{groove.PathMode}"); }

                    switch (groove.PathType)
                    {
                        case GroovePathNames.straight:
                            sw.WriteLine($"{groove.PathCentre.X},{groove.PathCentre.Y},{groove.PathCentre.Z}");
                            sw.WriteLine($"{groove.PathPeriod}");
                            sw.WriteLine($"{groove.PathDelta.X},{groove.PathDelta.Y},{groove.PathDelta.Z}");
                            break;
                    }

                    sw.WriteLine($"{groove.AnimationType}");
                    if (groove.AnimationType != GrooveAnimation.None) { sw.WriteLine($"{groove.AnimationMode}"); }

                    switch (groove.AnimationType)
                    {
                        case GrooveAnimation.rock:
                            sw.WriteLine($"{groove.AnimationPeriod}");
                            sw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            sw.WriteLine($"{groove.AnimationAxis}");
                            sw.WriteLine($"{groove.RockMaxAngle}");
                            break;

                        case GrooveAnimation.shear:
                            sw.WriteLine($"{groove.ShearPeriod.X},{groove.ShearPeriod.Y},{groove.ShearPeriod.Z}");
                            sw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            sw.WriteLine($"{groove.ShearMagnitude.X},{groove.ShearMagnitude.Y},{groove.ShearMagnitude.Z}");
                            break;

                        case GrooveAnimation.spin:
                            sw.WriteLine($"{groove.AnimationPeriod}");
                            sw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            sw.WriteLine($"{groove.AnimationAxis}");
                            break;
                    }

                    if (i + 1 != Grooves.Count)
                    {
                        sw.WriteLine();
                        sw.WriteLine("NEXT GROOVE");
                        sw.WriteLine();
                    }
                }

                sw.WriteLine();
                sw.WriteLine("END OF GROOVE");
                sw.WriteLine();

                sw.WriteLine("// END OF CRUSH DATA");

                sw.WriteLine();
                sw.WriteLine("START OF MECHANICS STUFF version 1");
                sw.WriteLine();

                sw.WriteLine($"{MinimumTurningCircle}");
                sw.WriteLine($"{BrakeMultiplier}");
                sw.WriteLine($"{BrakingStrengthMultiplier}");
                sw.WriteLine($"{NumberOfGears}");
                sw.WriteLine($"{TopGearRedlineSpeed}");
                sw.WriteLine($"{TopGearAcceleration}");

                sw.WriteLine();

                sw.WriteLine("// Sub member: Root part");
                sw.WriteLine($"{RootPartType}");
                sw.WriteLine($"{RootPartIdentifier}");
                sw.WriteLine($"{RootPartActor}");
                sw.WriteLine("// Sub member: Joint data");
                sw.WriteLine($"{SubPartType}");
                sw.WriteLine($"{CentreOfMass.X},{CentreOfMass.Y},{CentreOfMass.Z}");
                sw.WriteLine($"{Mass}");
                sw.WriteLine($"{AngularMomentumProportions.X},{AngularMomentumProportions.Y},{AngularMomentumProportions.Z}");
                sw.WriteLine($"{DownforceToWeightBalanceSpeed}");
                sw.WriteLine($"{Wheels.Count}");

                for (int i = 0; i < Wheels.Count; i++)
                {
                    Wheel wheel = Wheels[i];

                    sw.WriteLine($"// Wheels entry #{(i + 1)}");
                    sw.WriteLine($"{(int)wheel.Type}");
                    sw.WriteLine($"{wheel.Identifier}");
                    sw.WriteLine($"{wheel.Actor}");
                    sw.WriteLine($"{wheel.Position.X},{wheel.Position.Y},{wheel.Position.Z}");
                    sw.WriteLine($"{wheel.SteerableFlags}");
                    sw.WriteLine($"{wheel.DrivenFlags}");
                    sw.WriteLine($"{wheel.SuspensionGive}");
                    sw.WriteLine($"{wheel.DampingFactor}");
                    sw.WriteLine($"{wheel.SlipFrictionReductionFraction}");
                    sw.WriteLine($"{wheel.FrictionAngles.X},{wheel.FrictionAngles.Y}");
                    sw.WriteLine($"{wheel.TractionFractionalMultiplier}");
                    sw.WriteLine($"{wheel.RollingResistance}");
                }

                sw.WriteLine();

                sw.WriteLine($"{BoundingShapes.Count}");

                for (int i = 0; i < BoundingShapes.Count; i++)
                {
                    BoundingShape shape = BoundingShapes[i];

                    sw.WriteLine($"// Bounding shapes entry #{i + 1}");
                    sw.WriteLine($"{shape.Type}");
                    sw.WriteLine($"{shape.Points.Count}");
                    foreach (Vector3 point in shape.Points)
                    {
                        sw.WriteLine($"{point.X},{point.Y},{point.Z}");
                    }
                }

                sw.WriteLine();
                sw.WriteLine($"{SubParts.Count}");
                sw.WriteLine();

                sw.WriteLine();
                sw.WriteLine("END OF MECHANICS STUFF");
                sw.WriteLine();

                sw.WriteLine("// Materials for shrapnel");
                sw.WriteLine($"{Shrapnel.Count}");
                foreach (string shrapnel in Shrapnel)
                {
                    sw.WriteLine($"{shrapnel}");
                }

                sw.WriteLine();

                sw.WriteLine("//damage vertices fire points");
                foreach (int point in FirePoints)
                {
                    sw.WriteLine($"{point}");
                }

                sw.WriteLine();

                sw.WriteLine("// start of keyword stuff");
                foreach (Keyword keyword in Keywords)
                {
                    keyword.Write(sw);
                }

                sw.WriteLine("// End of keyword stuff");
                sw.WriteLine("END");
            }
        }
    }

    public class Wheel
    {
        public enum WheelType
        {
            normal = 0
        }

        public WheelType Type { get; set; }

        public string Identifier { get; set; }

        public string Actor { get; set; }

        public Vector3 Position { get; set; }

        public int SteerableFlags { get; set; }

        public int DrivenFlags { get; set; }

        public float SuspensionGive { get; set; }

        public float DampingFactor { get; set; }

        public float SlipFrictionReductionFraction { get; set; }

        public Vector2 FrictionAngles { get; set; }

        public float TractionFractionalMultiplier { get; set; }

        public float RollingResistance { get; set; }
    }

    public class BoundingShape
    {
        public enum BoundingShapeType
        {
            polyhedron
        }

        public BoundingShapeType Type { get; set; }

        public List<Vector3> Points { get; set; } = new List<Vector3>();
    }

    public abstract class Keyword 
    { 
        public string Name { get; set; }

        public abstract void Write(StreamWriter sw);
    }

    public class CameraPosition : Keyword
    {
        public Vector3 BumperPosition { get; set; }

        public Vector3 CockpitPosition { get; set; }

        public CameraPosition()
        {
            Name = "CAMERA_POSITIONS";
        }

        public override void Write(StreamWriter sw)
        {
            sw.WriteLine($"{Name}");
            sw.WriteLine($"{BumperPosition.X}, {BumperPosition.Y}, {BumperPosition.Z}");
            sw.WriteLine($"{CockpitPosition.X}, {CockpitPosition.Y}, {CockpitPosition.Z}");
        }
    }

    public class CameraTurnOffMaterials : Keyword
    {
        public List<CameraTurnOffMaterialEntry> Entries { get; set; } = new List<CameraTurnOffMaterialEntry>();

        public CameraTurnOffMaterials()
        {
            Name = "CAMERA_TURN_OFF_MATERIALS";
        }

        public override void Write(StreamWriter sw)
        {
            sw.WriteLine($"{Name}");
            sw.WriteLine($"{Entries.Count}");

            foreach (CameraTurnOffMaterialEntry entry in Entries)
            {
                sw.WriteLine($"{entry.MaterialName}");
                sw.WriteLine($"{entry.Materials.Count}");

                foreach (string material in entry.Materials)
                {
                    sw.WriteLine($"{material}");
                }
            }
        }
    }

    public class CameraTurnOffMaterialEntry
    {
        public string MaterialName { get; set; }

        public List<string> Materials { get; set; } = new List<string>();
    }
}
