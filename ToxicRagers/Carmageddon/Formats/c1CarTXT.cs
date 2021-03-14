using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Formats
{
    public class Car
    {
        public string Name { get; set; }

        public Vector3 DriversHeadOffset { get; set; } = new Vector3(-0.05f, 0.16f, 0f);

        public Vector2 DriversHeadTurnAngles { get; set; } = new Vector2(80, -70);

        public Vector4 MirrorCamera { get; set; } = new Vector4(0, 0.175f, 0, 30);

        public string[] PratcamBorders { get; set; } = new string[] { "none", "none", "PRATBDRT.PIX", "PRATBDHZ.PIX" };

        public int[] EngineNoises { get; set; } = new int[] { 5300, 5301, 5302 };

        public bool Stealworthy { get; set; }

        public ImpactSpec ImpactTop { get; set; } = new ImpactSpec { Description = "top" };

        public ImpactSpec ImpactBottom { get; set; } = new ImpactSpec { Description = "bottom" };

        public ImpactSpec ImpactLeft { get; set; } = new ImpactSpec { Description = "left" };

        public ImpactSpec ImpactRight { get; set; } = new ImpactSpec { Description = "right" };

        public ImpactSpec ImpactFront { get; set; } = new ImpactSpec { Description = "front" };

        public ImpactSpec ImpactBack { get; set; } = new ImpactSpec { Description = "rear" };

        public string[] GridImages { get; set; } = new string[] { "GCAR.PIX", "GCARF.PIX", "GCAR.PIX" };

        public List<string> PixelmapsLoMem { get; set; } = new List<string>();

        public List<string> PixelmapsLoRes { get; set; } = new List<string>();

        public List<string> PixelmapsHiRes { get; set; } = new List<string>();

        public List<string> ShadeTables { get; set; } = new List<string>();

        public List<string> MaterialsLoMem { get; set; } = new List<string>();

        public List<string> MaterialsLoRes { get; set; } = new List<string>();

        public List<string> MaterialsHiRes { get; set; } = new List<string>();

        public List<string> Models { get; set; } = new List<string>();

        public List<string> Actors { get; set; } = new List<string>();

        public List<int> ActorLODs { get; set; } = new List<int>();

        public string ReflectiveScreenMaterial { get; set; }

        public List<int> SteerableWheels { get; set; } = new List<int>();

        public int[] LeftFrontSuspension { get; set; } = new int[] { -1, -1, -1, -1 };

        public int[] RightFrontSuspension { get; set; } = new int[] { -1, -1, -1, -1 };

        public int[] LeftRearSuspension { get; set; } = new int[] { -1, -1 };

        public int[] RightRearSuspension { get; set; } = new int[] { -1, -1 };

        public int[] DrivenWheels { get; set; } = new int[] { -1, -1, -1, -1 };

        public int[] NonDrivenWheels { get; set; } = new int[] { -1, -1, -1, -1 };

        public float DrivenWheelDiameter { get; set; }

        public float NonDrivenWheelDiameter { get; set; }

        public List<Funk> Funks { get; set; } = new List<Funk>();

        public List<Groove> Grooves { get; set; } = new List<Groove>();

        public List<Crush> Crushes { get; set; } = new List<Crush>();

        public Vector3 LRWheelPos { get; set; } = Vector3.Zero;

        public Vector3 RRWheelPos { get; set; } = Vector3.Zero;

        public Vector3 LFWheelPos { get; set; } = Vector3.Zero;

        public Vector3 RFWheelPos { get; set; } = Vector3.Zero;

        public Vector3 CentreOfMass { get; set; } = Vector3.Zero;

        public int MechanicsVersion { get; set; } = 4;

        public List<BoundingBox> BoundingBoxes { get; set; } = new List<BoundingBox>();

        public List<Vector3> AdditionalPoints { get; set; } = new List<Vector3>();

        public float MinimumTurningCircle { get; set; }

        public Vector2 SuspensionGive { get; set; } = new Vector2(0.015f, 0.015f);

        public float RideHeight { get; set; }

        public float DampingFactor { get; set; }

        public float Mass { get; set; }

        public float SlipFrictionReductionFraction { get; set; }

        public Vector3 FrictionAngle { get; set; } = new Vector3(80, 79.5f, 80.5f);

        public Vector3 AngularMomentumProportions { get; set; } = new Vector3(0.3f, 0.15f, 0.9f);

        public float TractionFractionalMultiplier { get; set; }

        public float DownforceToWeightBalanceSpeed { get; set; }

        public float BrakeMultiplier { get; set; }

        public float BrakingStrengthMultiplier { get; set; }

        public Vector2 RollingResistance { get; set; } = new Vector2(0.05f, 0.05f);

        public int NumberOfGears { get; set; }

        public int TopGearRedlineSpeed { get; set; }

        public float TopGearAcceleration { get; set; }

        public List<string> Shrapnel { get; set; } = new List<string>();

        public List<int> FirePoints { get; set; } = new List<int>();

        public static Car Load(string path)
        {
            return Load(path, false);
        }

        public static Car Load(string path, bool suppressFolderCheck)
        {
            DocumentParser file = new DocumentParser(path);
            Car car = new Car { Name = file.ReadLine() };

            if (!suppressFolderCheck)
            {
                if (string.Compare(Path.GetFileName(path).ToUpper(), car.Name) != 0)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon car .txt file, expected {0} but found {1}", Path.GetFileName(path).ToUpper(), car.Name);
                    return null;
                }
            }

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

            int pixCount = file.ReadInt();
            for (int i = 0; i < pixCount; i++) { car.PixelmapsLoMem.Add(file.ReadLine()); }

            pixCount = file.ReadInt();
            for (int i = 0; i < pixCount; i++) { car.PixelmapsLoRes.Add(file.ReadLine()); }

            pixCount = file.ReadInt();
            for (int i = 0; i < pixCount; i++) { car.PixelmapsHiRes.Add(file.ReadLine()); }

            int shadeCount = file.ReadInt();
            for (int j = 0; j < shadeCount; j++)
            {
                car.ShadeTables.Add(file.ReadLine());
            }

            int matCount = file.ReadInt();
            for (int i = 0; i < matCount; i++) { car.MaterialsLoMem.Add(file.ReadLine()); }

            matCount = file.ReadInt();
            for (int i = 0; i < matCount; i++) { car.MaterialsLoRes.Add(file.ReadLine()); }

            matCount = file.ReadInt();
            for (int i = 0; i < matCount; i++) { car.MaterialsHiRes.Add(file.ReadLine()); }

            int modelCount = file.ReadInt();
            for (int j = 0; j < modelCount; j++)
            {
                car.Models.Add(file.ReadLine());
            }

            int actorCount = file.ReadInt();
            for (int j = 0; j < actorCount; j++)
            {
                string[] s = file.ReadStrings();
                car.ActorLODs.Add(s[0].ToInt());
                car.Actors.Add(s[1]);
            }

            car.ReflectiveScreenMaterial = file.ReadLine();

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

            for (int i = 0; i < car.Actors.Count; i++)
            {
                car.Crushes.Add(Crush.Load(file));
            }

            car.MechanicsVersion = file.ReadLine().Replace("START OF MECHANICS STUFF version ", "", StringComparison.InvariantCultureIgnoreCase).ToInt();

            car.LRWheelPos = file.ReadVector3();
            car.RRWheelPos = file.ReadVector3();
            car.LFWheelPos = file.ReadVector3();
            car.RFWheelPos = file.ReadVector3();
            car.CentreOfMass = file.ReadVector3();

            switch (car.MechanicsVersion)
            {
                case 2:
                    int boundingBoxCount = file.ReadInt();
                    for (int i = 0; i < boundingBoxCount; i++)
                    {
                        car.BoundingBoxes.Add(new BoundingBox { Min = file.ReadVector3(), Max = file.ReadVector3() });
                    }
                    break;

                case 3:
                case 4:
                    car.BoundingBoxes.Add(new BoundingBox { Min = file.ReadVector3(), Max = file.ReadVector3() });

                    int additionalPointsCount = file.ReadInt();
                    for (int i = 0; i < additionalPointsCount; i++)
                    {
                        car.AdditionalPoints.Add(file.ReadVector3());
                    }
                    break;
            }

            car.MinimumTurningCircle = file.ReadSingle();
            car.SuspensionGive = file.ReadVector2();
            car.RideHeight = file.ReadSingle();
            car.DampingFactor = file.ReadSingle();
            car.Mass = file.ReadSingle();
            car.SlipFrictionReductionFraction = file.ReadSingle();

            if (car.MechanicsVersion == 4)
            {
                car.FrictionAngle = file.ReadVector3();
            }
            else
            {
                car.FrictionAngle = (Vector3)file.ReadVector2();
            }

            car.AngularMomentumProportions = file.ReadVector3();
            car.TractionFractionalMultiplier = file.ReadSingle();
            car.DownforceToWeightBalanceSpeed = file.ReadSingle();
            car.BrakeMultiplier = file.ReadSingle();
            car.BrakingStrengthMultiplier = file.ReadSingle();
            car.RollingResistance = file.ReadVector2();
            car.NumberOfGears = file.ReadInt();
            car.TopGearRedlineSpeed = file.ReadInt();
            car.TopGearAcceleration = file.ReadSingle();

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

            if (!file.EOF)
            {
                for (int i = 0; i < 12; i++)
                {
                    car.FirePoints.Add(file.ReadInt());
                }
            }

            if (!file.EOF)
            {
                Console.WriteLine("Still data to parse in {0}", path);
            }

            return car;
        }

        public void Save(string path)
        {
            using (DocumentWriter dw = new DocumentWriter(path))
            {
                dw.WriteLine($"{Name}", "Name of car");

                dw.WriteLine();

                dw.WriteLine("START OF DRIVABLE STUFF");
                dw.WriteLine($"{DriversHeadOffset.X},{DriversHeadOffset.Y},{DriversHeadOffset.Z}", "Offset of driver's head in 3D space");
                dw.WriteLine($"{DriversHeadTurnAngles.X},{DriversHeadTurnAngles.Y}", "Angles to turn to make head go left and right");
                dw.WriteLine($"{MirrorCamera.X},{MirrorCamera.Y},{MirrorCamera.Z},{MirrorCamera.W}", "Offset of 'mirror camera' in 3D space, viewing angle of mirror");
                dw.WriteLine($"{string.Join(",", PratcamBorders)}", "Pratcam border names (left, top, right, bottom)");
                dw.WriteLine("END OF DRIVABLE STUFF");

                dw.WriteLine();

                dw.WriteLine($"{string.Join(",", EngineNoises)}", "Engine noise (normal, enclosed space, underwater)");
                dw.WriteLine($"{(Stealworthy ? "stealworthy" : "")}", "Can be stolen");

                dw.WriteLine();

                foreach (ImpactSpec impactSpec in new List<ImpactSpec> { ImpactTop, ImpactBottom, ImpactLeft, ImpactRight, ImpactFront, ImpactBack })
                {
                    dw.WriteLine($"// Damage info for {impactSpec.Description} impacts");
                    dw.WriteLine($"{impactSpec.Clauses.Count}", "Number of clauses");

                    foreach (ImpactSpecClause clause in impactSpec.Clauses)
                    {
                        dw.WriteLine($"{clause.Clause}", "Condition");
                        dw.WriteLine($"{clause.Systems.Count}", "Systems count");

                        foreach (ImpactSpecClauseSystem system in clause.Systems)
                        {
                            dw.WriteLine($"{system.Part},{system.Damage:F1}", "Damage");
                        }
                    }
                }

                dw.WriteLine();

                dw.WriteLine($"{string.Join(",", GridImages)}", "Grid image (opponent, frank, annie)");

                dw.WriteLine();

                dw.WriteLine($"{PixelmapsLoMem.Count}", "Number of pixelmap files for this car");
                foreach (string pixelmap in PixelmapsLoMem) { dw.WriteLine($"{pixelmap}"); }
                dw.WriteLine($"{PixelmapsLoRes.Count}", "Number of pixelmap files for this car");
                foreach (string pixelmap in PixelmapsLoRes) { dw.WriteLine($"{pixelmap}"); }
                dw.WriteLine($"{PixelmapsHiRes.Count}", "Number of pixelmap files for this car");
                foreach (string pixelmap in PixelmapsHiRes) { dw.WriteLine($"{pixelmap}"); }

                dw.WriteLine($"{ShadeTables.Count}", "Number of shadetable files for this car");
                foreach (string shadetable in ShadeTables) { dw.WriteLine($"{shadetable}"); }

                dw.WriteLine($"{MaterialsLoMem.Count}", "Number of material files for this car");
                foreach (string material in MaterialsLoMem) { dw.WriteLine($"{material}"); }
                dw.WriteLine($"{MaterialsLoRes.Count}", "Number of material files for this car");
                foreach (string material in MaterialsLoRes) { dw.WriteLine($"{material}"); }
                dw.WriteLine($"{MaterialsHiRes.Count}", "Number of material files for this car");
                foreach (string material in MaterialsHiRes) { dw.WriteLine($"{material}"); }

                dw.WriteLine($"{Models.Count}", "Number of model files for this car");
                foreach (string model in Models) { dw.WriteLine($"{model}"); }

                dw.WriteLine($"{Actors.Count}", "Number of alternative actors");
                for (int i = 0; i < Actors.Count; i++)
                {
                    dw.WriteLine($"{ActorLODs[i]},{Actors[i]}", "Minimum distance away, actor name");
                }

                dw.WriteLine();

                dw.WriteLine($"{ReflectiveScreenMaterial}", "Name of reflective screen material (or none if non-reflective)");

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
                        dw.WriteLine("NEXT FUNK");
                    }
                }

                dw.WriteLine("END OF FUNK");

                dw.WriteLine();

                dw.WriteLine("START OF GROOVE");

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
                        dw.WriteLine("NEXT GROOVE");
                    }
                }

                dw.WriteLine("END OF GROOVE");

                dw.WriteLine();

                for (int i = 0; i < Actors.Count; i++) { Crushes[i].Write(dw); }

                dw.WriteLine();

                dw.WriteLine($"START OF MECHANICS STUFF version {MechanicsVersion}");

                dw.WriteLine($"{LRWheelPos.X:F3}, {LRWheelPos.Y:F3}, {LRWheelPos.Z:F3}", "left rear wheel position");
                dw.WriteLine($"{RRWheelPos.X:F3}, {RRWheelPos.Y:F3}, {RRWheelPos.Z:F3}", "right rear");
                dw.WriteLine($"{LFWheelPos.X:F3}, {LFWheelPos.Y:F3}, {LFWheelPos.Z:F3}", "left front wheel position");
                dw.WriteLine($"{RFWheelPos.X:F3}, {RFWheelPos.Y:F3}, {RFWheelPos.Z:F3}", "right front");

                dw.WriteLine($"{CentreOfMass.X}, {CentreOfMass.Y}, {CentreOfMass.Z}", "centre of mass position");

                dw.WriteLine();

                switch (MechanicsVersion)
                {
                    case 2:
                        dw.WriteLine($"{BoundingBoxes.Count}", "number of bounding boxes");
                        foreach (BoundingBox box in BoundingBoxes) { box.Write(dw); }
                        break;

                    case 3:
                    case 4:
                        foreach (BoundingBox box in BoundingBoxes) { box.Write(dw); }

                        dw.WriteLine($"{AdditionalPoints.Count}", "number of extra points  	v. 3");
                        foreach (Vector3 point in AdditionalPoints) { dw.WriteLine($"{point.X},{point.Y},{point.Z}"); }
                        break;
                }

                dw.WriteLine();

                dw.WriteLine($"{MinimumTurningCircle}", "min turning circle radius");
                dw.WriteLine($"{SuspensionGive.X:F3}, {SuspensionGive.Y:F3}", "suspension give (forward, back)");
                dw.WriteLine($"{RideHeight}", "ride height (must be more than miny in bounding box )");
                dw.WriteLine($"{DampingFactor}", "damping factor");
                dw.WriteLine($"{Mass}", "mass in tonnes");
                dw.WriteLine($"{SlipFrictionReductionFraction}", "fractional reduction in friction when slipping");
                dw.WriteLine($"{FrictionAngle.X}, {FrictionAngle.Y}{(MechanicsVersion == 4 ? $", {FrictionAngle.Z}" : null)}", "friction angle ( front and rear )");
                dw.WriteLine($"{AngularMomentumProportions.X}, {AngularMomentumProportions.Y}, {AngularMomentumProportions.Z}", "width, height, length for angular momentum calculation");
                dw.WriteLine($"{TractionFractionalMultiplier:F1}", "traction fractional multiplier v. 2");
                dw.WriteLine($"{DownforceToWeightBalanceSpeed}", "speed at which down force = weight v. 2");
                dw.WriteLine($"{BrakeMultiplier:F1}", "brake multiplier, 1 = nomral brakes v. 2");
                dw.WriteLine($"{BrakingStrengthMultiplier:F1}", "increase in brakes per second 1 = normal v. 2");
                dw.WriteLine($"{RollingResistance.X}, {RollingResistance.Y}", "rolling resistance front and back");
                dw.WriteLine($"{NumberOfGears}", "number of gears");
                dw.WriteLine($"{TopGearRedlineSpeed}", "speed at red line in highest gear");
                dw.WriteLine($"{TopGearAcceleration}", "acceleration in highest gear m/s^2 (i.e. engine strength)");

                dw.WriteLine("END OF MECHANICS STUFF");

                dw.WriteLine();

                dw.WriteLine("// Materials for shrapnel");
                dw.WriteLine($"{Shrapnel.Count}", "number of materials");
                foreach (string shrapnel in Shrapnel) { dw.WriteLine($"{shrapnel}"); }

                dw.WriteLine();

                if (FirePoints.Count > 0)
                {
                    dw.WriteLine("// damage vertices fire point");
                    foreach (int firepoint in FirePoints) { dw.WriteLine($"{firepoint}"); }
                }
            }
        }
    }

    public class Crush
    {
        public float SoftnessFactor { get; set; } = 0.7f;

        public Vector2 FoldFactor { get; set; } = new Vector2(0.15f, 0.4f);

        public float WibbleFactor { get; set; } = 0.05f;

        public float LimitDeviant { get; set; } = 0.05f;

        public float SplitChance { get; set; }

        public float MinYFoldDown { get; set; }

        public List<CrushPoint> Points { get; set; } = new List<CrushPoint>();

        public static Crush Load(DocumentParser file)
        {
            Crush crush = new Crush
            {
                SoftnessFactor = file.ReadSingle(),
                FoldFactor = file.ReadVector2(),
                WibbleFactor = file.ReadSingle(),
                LimitDeviant = file.ReadSingle(),
                SplitChance = file.ReadSingle(),
                MinYFoldDown = file.ReadSingle()
            };

            int pointCount = file.ReadInt();
            for (int i = 0; i < pointCount; i++)
            {
                crush.Points.Add(CrushPoint.Load(file));
            }

            return crush;
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine("// CRUSH DATA");
            dw.WriteLine($"{SoftnessFactor:F6}");
            dw.WriteLine($"{FoldFactor.X:F6},{FoldFactor.Y:F6}");
            dw.WriteLine($"{WibbleFactor:F6}");
            dw.WriteLine($"{LimitDeviant:F6}");
            dw.WriteLine($"{SplitChance:F6}");
            dw.WriteLine($"{MinYFoldDown:F6}");
            dw.WriteLine($"{Points.Count}");
            foreach (CrushPoint point in Points)
            {
                point.Write(dw);
            }
        }
    }

    public class CrushPoint
    {
        public int VertexIndex { get; set; }

        public Vector3 LimitMin { get; set; }

        public Vector3 LimitMax { get; set; }

        public Vector3 SoftnessNeg { get; set; }

        public Vector3 SoftnessPos { get; set; }

        public List<CrushPointNeighbour> Neighbours { get; set; } = new List<CrushPointNeighbour>();

        public static CrushPoint Load(DocumentParser file)
        {
            CrushPoint cp = new CrushPoint
            {
                VertexIndex = file.ReadInt(),
                LimitMin = file.ReadVector3(),
                LimitMax = file.ReadVector3(),
                SoftnessNeg = file.ReadVector3(),
                SoftnessPos = file.ReadVector3()
            };

            int neighbourCount = file.ReadInt();
            for (int i = 0; i < neighbourCount; i++)
            {
                cp.Neighbours.Add(CrushPointNeighbour.Load(file));
            }

            return cp;
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{VertexIndex}");
            dw.WriteLine($"{LimitMin.X:F6}, {LimitMin.Y:F6}, {LimitMin.Z:F6}");
            dw.WriteLine($"{LimitMax.X:F6}, {LimitMax.Y:F6}, {LimitMax.Z:F6}");
            dw.WriteLine($"{SoftnessNeg.X:F6}, {SoftnessNeg.Y:F6}, {SoftnessNeg.Z:F6}");
            dw.WriteLine($"{SoftnessPos.X:F6}, {SoftnessPos.Y:F6}, {SoftnessPos.Z:F6}");
            dw.WriteLine($"{Neighbours.Count}");

            foreach (CrushPointNeighbour neighbour in Neighbours)
            {
                neighbour.Write(dw);
            }
        }
    }

    public class CrushPointNeighbour
    {
        public int VertexIndex { get; set; }

        public int Factor { get; set; }

        public static CrushPointNeighbour Load(DocumentParser file)
        {
            return new CrushPointNeighbour
            {
                VertexIndex = file.ReadInt(),
                Factor = file.ReadInt()
            };
        }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{VertexIndex}");
            dw.WriteLine($"{Factor}");
        }
    }

    public class BoundingBox
    {
        public Vector3 Min { get; set; }

        public Vector3 Max { get; set; }

        public void Write(DocumentWriter dw)
        {
            dw.WriteLine($"{Min.X}, {Min.Y}, {Min.Z}", "min x, min y, min z");
            dw.WriteLine($"{Max.X}, {Max.Y}, {Max.Z}", "max x, max y, max z");
        }
    }
}