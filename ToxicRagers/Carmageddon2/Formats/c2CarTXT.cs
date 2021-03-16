using System;
using System.Collections.Generic;

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

        public string Name { get; set; }

        public float SoftnessFactor { get; set; }

        public Vector3 DriversHeadOffset { get; set; }

        public Vector2 DriversHeadTurnAngles { get; set; }

        public Vector4 MirrorCamera { get; set; }

        public string[] PratcamBorders { get; set; }

        public int[] EngineNoises { get; set; }

        public bool Stealworthy { get; set; }

        public ImpactSpec ImpactTop { get; set; }

        public ImpactSpec ImpactBottom { get; set; }

        public ImpactSpec ImpactLeft { get; set; }

        public ImpactSpec ImpactRight { get; set; }

        public ImpactSpec ImpactFront { get; set; }

        public ImpactSpec ImpactBack { get; set; }

        public string[] GridImages { get; set; }

        public List<int> ExtraLevelsOfDetail { get; set; } = new List<int>();

        public string WAM { get; set; }

        public string ReflectiveScreenMaterial { get; set; }

        public float TransparencyOfWindscreen { get; set; }

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

            car.ImpactTop = ImpactSpec.Load("top", file);
            car.ImpactBottom = ImpactSpec.Load("bottom", file);
            car.ImpactLeft = ImpactSpec.Load("left", file);
            car.ImpactRight = ImpactSpec.Load("right", file);
            car.ImpactFront = ImpactSpec.Load("front", file);
            car.ImpactBack = ImpactSpec.Load("rear", file);

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
                car.Funks.Add(Funk.Load(file));
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
                car.Grooves.Add(Groove.Load(file));
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
            using (DocumentWriter dw = new DocumentWriter(path))
            {
                dw.WriteLine($"VERSION {Version}");
                dw.WriteLine("//	Version 1 :		New crush data");
                if (Version > 1) { dw.WriteLine("//		2 :		New windscreen spec"); }
                dw.WriteLine($"{(FemaleDriver ? "GIRL" : "")}");
                dw.WriteLine($"{Name}", "Name of car");

                dw.WriteLine();

                dw.WriteLine($"{SoftnessFactor}", "softness_factor");

                dw.WriteLine();

                dw.WriteLine("START OF DRIVABLE STUFF");
                dw.WriteLine();
                dw.WriteLine($"{DriversHeadOffset.X},{DriversHeadOffset.Y},{DriversHeadOffset.Z}", "Offset of driver's head in 3D space");
                dw.WriteLine($"{DriversHeadTurnAngles.X},{DriversHeadTurnAngles.Y}", "Angles to turn to make head go left and right");
                dw.WriteLine($"{MirrorCamera.X},{MirrorCamera.Y},{MirrorCamera.Z},{MirrorCamera.W}", "Offset of 'mirror camera' in 3D space, viewing angle of mirror");
                dw.WriteLine($"{string.Join(",", PratcamBorders)}", "Pratcam border names (left, top, right, bottom)");
                dw.WriteLine();
                dw.WriteLine("END OF DRIVABLE STUFF");

                dw.WriteLine();

                dw.WriteLine($"{string.Join(",",EngineNoises)}", "Engine noise (normal, enclosed space, underwater)");

                dw.WriteLine();

                dw.WriteLine($"{(Stealworthy ? "stealworthy" : "")}", "Can be stolen");

                dw.WriteLine();

                foreach (ImpactSpec impactSpec in new List<ImpactSpec> { ImpactTop, ImpactBottom, ImpactLeft, ImpactRight, ImpactFront, ImpactBack })
                {
                    dw.WriteLine($"// Damage info for {impactSpec.Description} impacts");
                    dw.WriteLine($"{impactSpec.Clauses.Count}", "Number of clauses");
                    dw.IncreaseIndent();
                    foreach (ImpactSpecClause clause in impactSpec.Clauses)
                    {
                        dw.WriteLine($"{clause.Clause}", "Condition");
                        dw.WriteLine($"{clause.Systems.Count}", "Systems count");

                        dw.IncreaseIndent();
                        foreach (ImpactSpecClauseSystem system in clause.Systems)
                        {
                            dw.WriteLine($"{system.Part},{system.Damage:F1}", "Damage");
                        }
                        dw.DecreaseIndent();
                    }
                    dw.DecreaseIndent();
                }

                dw.WriteLine();

                dw.WriteLine($"{string.Join(",", GridImages)}", "Grid image (opponent, frank, annie)");

                dw.WriteLine();

                dw.WriteLine($"{ExtraLevelsOfDetail.Count}", "Number of extra levels of detail");
                foreach (int extraLevelOfDetail in ExtraLevelsOfDetail)
                {
                    dw.WriteLine($"{extraLevelOfDetail}", "min_dist_squared");
                }

                dw.WriteLine();

                dw.WriteLine($"{WAM}", "crush data file (will be incorporated into this file)");

                dw.WriteLine();

                dw.WriteLine($"{ReflectiveScreenMaterial}", "Name of reflective screen material (or none if non-reflective)");
                dw.WriteLine($"{TransparencyOfWindscreen}", "Percentage transparency of windscreen");

                dw.WriteLine();

                dw.WriteLine($"{SteerableWheels.Count}", "Number of steerable wheels");
                foreach (int steerableWheel in SteerableWheels)
                {
                    dw.WriteLine($"{steerableWheel}", "GroovyFunkRef of nth steerable wheel");
                }

                dw.WriteLine();

                dw.WriteLine($"{string.Join(",", LeftFrontSuspension)}", "Left-front suspension parts GroovyFunkRef");
                dw.WriteLine($"{string.Join(",", RightFrontSuspension)}", "Right-front suspension parts GroovyFunkRef");
                dw.WriteLine($"{string.Join(",", LeftRearSuspension)}", "Left-rear suspension parts GroovyFunkRef");
                dw.WriteLine($"{string.Join(",", RightRearSuspension)}", "Right-rear suspension parts GroovyFunkRef");

                dw.WriteLine();

                dw.WriteLine($"{string.Join(",", DrivenWheels)}", "Driven wheels GroovyFunkRefs (for spinning) - MUST BE 4 ITEMS");
                dw.WriteLine($"{string.Join(",", NonDrivenWheels)}", "Non-driven wheels GroovyFunkRefs (for spinning) - MUST BE 4 ITEMS");

                dw.WriteLine();

                dw.WriteLine($"{DrivenWheelDiameter}", "Driven wheels diameter");
                dw.WriteLine($"{NonDrivenWheelDiameter}", "Non-driven wheels diameter");

                dw.WriteLine();
                dw.WriteLine("START OF FUNK");
                dw.WriteLine();

                for (int i = 0; i < Funks.Count; i++)
                {
                    Funk funk = Funks[i];

                    dw.WriteLine($"{funk.Material}");
                    dw.WriteLine($"{funk.Mode}");
                    dw.WriteLine($"{funk.MatrixModType}");
                    if (funk.MatrixModType != FunkMatrixMode.None) { dw.WriteLine($"{funk.MatrixModMode}"); }

                    switch (funk.MatrixModType)
                    {
                        case FunkMatrixMode.roll:
                            dw.WriteLine($"{funk.RollPeriods.X},{funk.RollPeriods.Y}");
                            break;

                        case FunkMatrixMode.slither:
                            dw.WriteLine($"{funk.SlitherSpeed.X},{funk.SlitherSpeed.Y}");
                            dw.WriteLine($"{funk.SlitherAmount.X},{funk.SlitherAmount.Y}");
                            break;

                        case FunkMatrixMode.spin:
                            dw.WriteLine($"{funk.SpinPeriod}");
                            break;
                    }

                    dw.WriteLine($"{funk.LightingMode}");
                    dw.WriteLine($"{funk.AnimationType}");

                    switch (funk.AnimationType)
                    {
                        case FunkAnimationType.frames:
                            dw.WriteLine($"{funk.Framerate}");
                            dw.WriteLine($"{funk.FrameMode}");

                            switch (funk.FrameMode)
                            {
                                case FrameType.texturebits:
                                    dw.WriteLine($"{funk.TextureBitMode}");
                                    break;

                                case FrameType.continuous:
                                    dw.WriteLine($"{funk.FrameSpeed}");
                                    break;
                            }

                            dw.WriteLine($"{funk.Frames.Count}");
                            foreach (string frame in funk.Frames)
                            {
                                dw.WriteLine($"{frame}");
                            }
                            break;
                    }

                    if (i + 1 != Funks.Count)
                    {
                        dw.WriteLine();
                        dw.WriteLine("NEXT FUNK");
                        dw.WriteLine();
                    }
                }

                dw.WriteLine();
                dw.WriteLine("END OF FUNK");
                dw.WriteLine();

                dw.WriteLine();
                dw.WriteLine("START OF GROOVE");
                dw.WriteLine();

                for (int i = 0; i < Grooves.Count; i++)
                {
                    Groove groove = Grooves[i];

                    dw.WriteLine($"{groove.Part}");
                    dw.WriteLine($"{groove.LollipopMode}");
                    dw.WriteLine($"{groove.Mode}");
                    dw.WriteLine($"{groove.PathType}");
                    if (groove.PathType != GroovePathNames.None) { dw.WriteLine($"{groove.PathMode}"); }

                    switch (groove.PathType)
                    {
                        case GroovePathNames.straight:
                            dw.WriteLine($"{groove.PathCentre.X},{groove.PathCentre.Y},{groove.PathCentre.Z}");
                            dw.WriteLine($"{groove.PathPeriod}");
                            dw.WriteLine($"{groove.PathDelta.X},{groove.PathDelta.Y},{groove.PathDelta.Z}");
                            break;
                    }

                    dw.WriteLine($"{groove.AnimationType}");
                    if (groove.AnimationType != GrooveAnimation.None) { dw.WriteLine($"{groove.AnimationMode}"); }

                    switch (groove.AnimationType)
                    {
                        case GrooveAnimation.rock:
                            dw.WriteLine($"{groove.AnimationPeriod}");
                            dw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            dw.WriteLine($"{groove.AnimationAxis}");
                            dw.WriteLine($"{groove.RockMaxAngle}");
                            break;

                        case GrooveAnimation.shear:
                            dw.WriteLine($"{groove.ShearPeriod.X},{groove.ShearPeriod.Y},{groove.ShearPeriod.Z}");
                            dw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            dw.WriteLine($"{groove.ShearMagnitude.X},{groove.ShearMagnitude.Y},{groove.ShearMagnitude.Z}");
                            break;

                        case GrooveAnimation.spin:
                            dw.WriteLine($"{groove.AnimationPeriod}");
                            dw.WriteLine($"{groove.AnimationCentre.X},{groove.AnimationCentre.Y},{groove.AnimationCentre.Z}");
                            dw.WriteLine($"{groove.AnimationAxis}");
                            break;
                    }

                    if (i + 1 != Grooves.Count)
                    {
                        dw.WriteLine();
                        dw.WriteLine("NEXT GROOVE");
                        dw.WriteLine();
                    }
                }

                dw.WriteLine();
                dw.WriteLine("END OF GROOVE");
                dw.WriteLine();

                dw.WriteLine("// END OF CRUSH DATA");

                dw.WriteLine();
                dw.WriteLine("START OF MECHANICS STUFF version 1");
                dw.WriteLine();

                dw.WriteLine($"{MinimumTurningCircle:F6}", "Minimum turning circle.");
                dw.WriteLine($"{BrakeMultiplier:F6}", "Brake multiplier.");
                dw.WriteLine($"{BrakingStrengthMultiplier:F6}", "Braking strength multiplier.");
                dw.WriteLine($"{NumberOfGears}", "Number of gears.");
                dw.WriteLine($"{TopGearRedlineSpeed:F4}", "Speed at red line in highest gear.");
                dw.WriteLine($"{TopGearAcceleration:F6}", "Acceleration in highest gear (m/s^2) i.e. engine strength.");

                dw.WriteLine();

                dw.WriteLine("// Sub member: Root part");
                dw.WriteLine($"{RootPartType}", "Type");
                dw.WriteLine($"{RootPartIdentifier}", "Identifier");
                dw.WriteLine($"{RootPartActor}", "Actor");
                dw.WriteLine("// Sub member: Joint data");
                dw.WriteLine($"{SubPartType}", "Type");
                dw.WriteLine($"{CentreOfMass.X:F6},{CentreOfMass.Y:F6},{CentreOfMass.Z:F6}", "Centre of mass");
                dw.WriteLine($"{Mass}", "Mass");
                dw.WriteLine($"{AngularMomentumProportions.X:F6},{AngularMomentumProportions.Y:F6},{AngularMomentumProportions.Z:F6}", "Angular momentum proportions");
                dw.WriteLine($"{DownforceToWeightBalanceSpeed:F6}", "Downforce-to-weight balance speed");
                dw.WriteLine($"{Wheels.Count}", "Number of 'Wheels' entries.");

                for (int i = 0; i < Wheels.Count; i++)
                {
                    Wheel wheel = Wheels[i];

                    dw.WriteLine($"// Wheels entry #{i + 1}");
                    dw.WriteLine($"{(int)wheel.Type}", "Type");
                    dw.WriteLine($"{wheel.Identifier}", "Identifier");
                    dw.WriteLine($"{wheel.Actor}", "Actor");
                    dw.WriteLine($"{wheel.Position.X},{wheel.Position.Y},{wheel.Position.Z}", "Position");
                    dw.WriteLine($"{wheel.SteerableFlags}", "Steerable flags");
                    dw.WriteLine($"{wheel.DrivenFlags}", "Driven flags");
                    dw.WriteLine($"{wheel.SuspensionGive:F6}", "Suspension give");
                    dw.WriteLine($"{wheel.DampingFactor:F6}", "Damping factor");
                    dw.WriteLine($"{wheel.SlipFrictionReductionFraction:F6}", "Fractional reduction in friction when slipping");
                    dw.WriteLine($"{wheel.FrictionAngles.X:F6},{wheel.FrictionAngles.Y:F6}", "Friction angles");
                    dw.WriteLine($"{wheel.TractionFractionalMultiplier:F6}", "Traction fractional multiplier");
                    dw.WriteLine($"{wheel.RollingResistance:F6}", "Rolling resistance");
                }

                dw.WriteLine();

                dw.WriteLine($"{BoundingShapes.Count}", "Number of 'Bounding shapes' entries.");

                for (int i = 0; i < BoundingShapes.Count; i++)
                {
                    BoundingShape shape = BoundingShapes[i];

                    dw.WriteLine($"// Bounding shapes entry #{i + 1}");
                    dw.WriteLine($"{shape.Type}", "Type");
                    dw.WriteLine($"{shape.Points.Count}");
                    foreach (Vector3 point in shape.Points)
                    {
                        dw.WriteLine($"{point.X:F2},{point.Y:F2},{point.Z:F2}");
                    }
                }

                dw.WriteLine();
                dw.WriteLine($"{SubParts.Count}", "Number of sub-parts.");
                dw.WriteLine();

                dw.WriteLine();
                dw.WriteLine("END OF MECHANICS STUFF");
                dw.WriteLine();

                dw.WriteLine("// Materials for shrapnel");
                dw.WriteLine($"{Shrapnel.Count}", "number of materials");
                foreach (string shrapnel in Shrapnel)
                {
                    dw.WriteLine($"{shrapnel}");
                }

                dw.WriteLine();

                dw.WriteLine("//damage vertices fire points");
                foreach (int point in FirePoints)
                {
                    dw.WriteLine($"{point}");
                }

                dw.WriteLine();

                dw.WriteLine("// start of keyword stuff");
                foreach (Keyword keyword in Keywords)
                {
                    keyword.Write(dw);
                }

                dw.WriteLine("// End of keyword stuff");
                dw.WriteLine("END");
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

        public abstract void Write(DocumentWriter dw);
    }

    public class CameraPosition : Keyword
    {
        public Vector3 BumperPosition { get; set; }

        public Vector3 CockpitPosition { get; set; }

        public CameraPosition()
        {
            Name = "CAMERA_POSITIONS";
        }

        public override void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{Name}");
            dw.WriteLine($"{BumperPosition.X}, {BumperPosition.Y}, {BumperPosition.Z}", "bumper position");
            dw.WriteLine($"{CockpitPosition.X}, {CockpitPosition.Y}, {CockpitPosition.Z}", "cockpit position");
        }
    }

    public class CameraTurnOffMaterials : Keyword
    {
        public List<CameraTurnOffMaterialEntry> Entries { get; set; } = new List<CameraTurnOffMaterialEntry>();

        public CameraTurnOffMaterials()
        {
            Name = "CAMERA_TURN_OFF_MATERIALS";
        }

        public override void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{Name}");
            dw.WriteLine($"{Entries.Count}", "Count");

            foreach (CameraTurnOffMaterialEntry entry in Entries)
            {
                dw.WriteLine($"{entry.MaterialName}");
                dw.WriteLine($"{entry.Materials.Count}");

                foreach (string material in entry.Materials)
                {
                    dw.WriteLine($"{material}");
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
