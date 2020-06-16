using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon.Formats
{
    public class Car
    {
        string name;
        Vector3 driversHeadOffset;
        Vector2 driversHeadTurnAngles;
        Vector4 mirrorCamera;
        string[] pratcamBorders;
        int[] engineNoises;
        bool stealWorthy;
        ImpactSpec impactTop;
        ImpactSpec impactBottom;
        ImpactSpec impactLeft;
        ImpactSpec impactRight;
        ImpactSpec impactFront;
        ImpactSpec impactBack;
        string[] gridImages;
        List<string>[] pixelmaps;
        List<string>[] materials;
        List<string> models;
        List<string> actors;
        List<int> actorLOD;
        string reflectiveScreenMaterial;
        List<int> steerableWheels;
        int[] leftFrontSuspension;
        int[] rightFrontSuspension;
        int[] leftRearSuspension;
        int[] rightRearSuspension;
        int[] drivenWheels;
        int[] nonDrivenWheels;
        float drivenWheelDiameter;
        float nonDrivenWheelDiameter;
        List<Funk> funks;
        List<Groove> grooves;
        List<Crush> crushes;
        Vector3 lrWheelPos;
        Vector3 rrWheelPos;
        Vector3 lfWheelPos;
        Vector3 rfWheelPos;
        Vector3 centreOfMass;
        List<BoundingBox> boundingBoxes;
        List<Vector3> additionalPoints;
        float turningCircleRadius;
        Vector2 suspensionGive; // Forward, Back
        float rideHeight;
        float dampingFactor;
        float massInTonnes;
        float fReduction;
        Vector2 frictionAngle;
        Vector3 widthHeightLength;
        float tractionFractionalMultiplier;
        float downforceSpeed;
        float brakeMultiplier;
        float increaseInBrakesPerSecond;
        Vector2 rollingResistance; // Front, Back
        int numberOfGears;
        int redLineSpeed;
        float accelerationInHighestGear;
        List<string> shrapnel;
        List<int> firePoints;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public List<string>[] Pixelmaps
        {
            get => pixelmaps;
            set => pixelmaps = value;
        }

        public List<string>[] Materials
        {
            get => materials;
            set => materials = value;
        }

        public List<string> Models
        {
            get => models;
            set => models = value;
        }

        public List<string> Actors
        {
            get => actors;
            set => actors = value;
        }

        public List<int> ActorLODs
        {
            get => actorLOD;
            set => actorLOD = value;
        }

        public List<Crush> Crushes
        {
            get => crushes;
            set => crushes = value;
        }

        public Car()
        {
            pixelmaps = new List<string>[3];
            materials = new List<string>[3];
            models = new List<string>();
            actors = new List<string>();
            actorLOD = new List<int>();
            steerableWheels = new List<int>();
            funks = new List<Funk>();
            grooves = new List<Groove>();
            crushes = new List<Crush>();
            boundingBoxes = new List<BoundingBox>();
            shrapnel = new List<string>();
            additionalPoints = new List<Vector3>();
            firePoints = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                pixelmaps[i] = new List<string>();
                materials[i] = new List<string>();
            }
        }

        public static Car Load(string path)
        {
            DocumentParser file = new DocumentParser(path);
            Car car = new Car() { name = file.ReadLine() };

            if (string.Compare(Path.GetFileName(path).ToUpper(), car.name) != 0)
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Not a valid Carmageddon car .txt file, expected {0} but found {1}", Path.GetFileName(path).ToUpper(), car.name);
                return null;
            }

            if (file.ReadLine() != "START OF DRIVABLE STUFF")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "START OF DRIVABLE STUFF");
                return null;
            }

            car.driversHeadOffset = file.ReadVector3();
            car.driversHeadTurnAngles = file.ReadVector2();
            car.mirrorCamera = file.ReadVector4();
            car.pratcamBorders = file.ReadStrings();

            if (file.ReadLine() != "END OF DRIVABLE STUFF")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "END OF DRIVABLE STUFF");
                return null;
            }

            car.engineNoises = file.ReadInts();
            car.stealWorthy = (file.ReadLine().ToLower() == "stealworthy");

            car.impactTop = new ImpactSpec("top", file);
            car.impactBottom = new ImpactSpec("bottom", file);
            car.impactLeft = new ImpactSpec("left", file);
            car.impactRight = new ImpactSpec("right", file);
            car.impactFront = new ImpactSpec("front", file);
            car.impactBack = new ImpactSpec("back", file);

            car.gridImages = file.ReadStrings();

            for (int i = 0; i < 3; i++)
            {
                int pixCount = file.ReadInt();

                for (int j = 0; j < pixCount; j++)
                {
                    car.pixelmaps[i].Add(file.ReadLine());
                }
            }

            int shadeCount = file.ReadInt();
            for (int j = 0; j < shadeCount; j++)
            {
                Console.WriteLine();
            }

            for (int i = 0; i < 3; i++)
            {
                int matCount = file.ReadInt();

                for (int j = 0; j < matCount; j++)
                {
                    car.materials[i].Add(file.ReadLine());
                }
            }

            int modelCount = file.ReadInt();
            for (int j = 0; j < modelCount; j++)
            {
                car.models.Add(file.ReadLine());
            }

            int actorCount = file.ReadInt();
            for (int j = 0; j < actorCount; j++)
            {
                string[] s = file.ReadStrings();
                car.actorLOD.Add(s[0].ToInt());
                car.actors.Add(s[1]);
            }

            car.reflectiveScreenMaterial = file.ReadLine();

            int steerableWheelsCount = file.ReadInt();
            for (int j = 0; j < steerableWheelsCount; j++)
            {
                car.steerableWheels.Add(file.ReadInt());
            }

            car.leftFrontSuspension = file.ReadInts();
            car.rightFrontSuspension = file.ReadInts();
            car.leftRearSuspension = file.ReadInts();
            car.rightRearSuspension = file.ReadInts();
            car.drivenWheels = file.ReadInts();
            car.nonDrivenWheels = file.ReadInts();

            car.drivenWheelDiameter = file.ReadSingle();
            car.nonDrivenWheelDiameter = file.ReadSingle();

            if (file.ReadLine() != "START OF FUNK")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "START OF FUNK");
                return null;
            }

            while (file.PeekLine() != "END OF FUNK")
            {
                car.funks.Add(new Funk(file));
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
                car.grooves.Add(new Groove(file));
                if (file.PeekLine() == "NEXT GROOVE") { file.ReadLine(); }
            }
            file.ReadLine();

            for (int i = 0; i < car.actors.Count; i++)
            {
                car.crushes.Add(new Crush(file));
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", car.actors[i], car.crushes[i].SoftnessFactor, car.crushes[i].FoldFactor, car.crushes[i].WibbleFactor, car.crushes[i].LimitDeviant, car.crushes[i].SplitChance, car.crushes[i].MinYFoldDown);
            }

            int mechanicsVersion = file.ReadLine().Replace("START OF MECHANICS STUFF version ", "", StringComparison.InvariantCultureIgnoreCase).ToInt();

            car.lrWheelPos = file.ReadVector3();
            car.rrWheelPos = file.ReadVector3();
            car.lfWheelPos = file.ReadVector3();
            car.rfWheelPos = file.ReadVector3();
            car.centreOfMass = file.ReadVector3();

            switch (mechanicsVersion)
            {
                case 2:
                    int boundingBoxCount = file.ReadInt();
                    for (int i = 0; i < boundingBoxCount; i++)
                    {
                        car.boundingBoxes.Add(new BoundingBox { Min = file.ReadVector3(), Max = file.ReadVector3() });
                    }
                    break;

                case 3:
                case 4:
                    car.boundingBoxes.Add(new BoundingBox { Min = file.ReadVector3(), Max = file.ReadVector3() });

                    int additionalPointsCount = file.ReadInt();
                    for (int i = 0; i < additionalPointsCount; i++)
                    {
                        car.additionalPoints.Add(file.ReadVector3());
                    }
                    break;
            }

            car.turningCircleRadius = file.ReadSingle();
            car.suspensionGive = file.ReadVector2();
            car.rideHeight = file.ReadSingle();
            car.dampingFactor = file.ReadSingle();
            car.massInTonnes = file.ReadSingle();
            car.fReduction = file.ReadSingle();
            car.frictionAngle = file.ReadVector2();
            car.widthHeightLength = file.ReadVector3();
            car.tractionFractionalMultiplier = file.ReadSingle();
            car.downforceSpeed = file.ReadSingle();
            car.brakeMultiplier = file.ReadSingle();
            car.increaseInBrakesPerSecond = file.ReadSingle();
            car.rollingResistance = file.ReadVector2();
            car.numberOfGears = file.ReadInt();
            car.redLineSpeed = file.ReadInt();
            car.accelerationInHighestGear = file.ReadSingle();

            if (file.ReadLine() != "END OF MECHANICS STUFF")
            {
                Logger.LogToFile(Logger.LogLevel.Error, "Expected \"{0}\", didn't get it.  Are you sure this is a Car.TXT file?", "END OF MECHANICS STUFF");
                return null;
            }

            int shrapnelCount = file.ReadInt();
            for (int i = 0; i < shrapnelCount; i++)
            {
                car.shrapnel.Add(file.ReadLine());
            }

            if (!file.EOF)
            {
                for (int i = 0; i < 12; i++)
                {
                    car.firePoints.Add(file.ReadInt());
                }
            }

            if (!file.EOF)
            {
                Console.WriteLine("Still data to parse in {0}", path);
            }

            return car;
        }
    }

    public class Crush
    {
        float softnessFactor;
        Vector2 foldFactor; // min, max
        float wibbleFactor;
        float limitDeviant;
        float splitChance;
        float minYFoldDown;
        List<CrushPoint> points;

        public float SoftnessFactor
        {
            get => softnessFactor;
            set => softnessFactor = value;
        }

        public Vector2 FoldFactor
        {
            get => foldFactor;
            set => foldFactor = value;
        }

        public float WibbleFactor
        {
            get => wibbleFactor;
            set => wibbleFactor = value;
        }

        public float LimitDeviant
        {
            get => limitDeviant;
            set => limitDeviant = value;
        }

        public float SplitChance
        {
            get => splitChance;
            set => splitChance = value;
        }

        public float MinYFoldDown
        {
            get => minYFoldDown;
            set => minYFoldDown = value;
        }

        public List<CrushPoint> Points
        {
            get => points;
            set => points = value;
        }

        public Crush()
        {
            points = new List<CrushPoint>();
        }

        public Crush(DocumentParser file)
            : this()
        {
            softnessFactor = file.ReadSingle();
            foldFactor = file.ReadVector2();
            wibbleFactor = file.ReadSingle();
            limitDeviant = file.ReadSingle();
            splitChance = file.ReadSingle();
            minYFoldDown = file.ReadSingle();

            int pointCount = file.ReadInt();
            for (int i = 0; i < pointCount; i++)
            {
                points.Add(new CrushPoint(file));
            }
        }
    }

    public class CrushPoint
    {
        int vertexIndex;
        Vector3 limitNeg;
        Vector3 limitPos;
        Vector3 softnessNeg;
        Vector3 softnessPos;
        List<CrushPointNeighbour> neighbours;

        public int VertexIndex
        {
            get => vertexIndex;
            set => vertexIndex = value;
        }

        public Vector3 LimitMin
        {
            get => limitNeg;
            set => limitNeg = value;
        }

        public Vector3 LimitMax
        {
            get => limitPos;
            set => limitPos = value;
        }

        public Vector3 SoftnessNeg
        {
            get => softnessNeg;
            set => softnessNeg = value;
        }

        public Vector3 SoftnessPos
        {
            get => softnessPos;
            set => softnessPos = value;
        }

        public List<CrushPointNeighbour> Neighbours
        {
            get => neighbours;
            set => neighbours = value;
        }

        public CrushPoint()
        {
            neighbours = new List<CrushPointNeighbour>();
        }

        public CrushPoint(DocumentParser file)
            : this()
        {
            vertexIndex = file.ReadInt();
            limitNeg = file.ReadVector3();
            limitPos = file.ReadVector3();
            softnessNeg = file.ReadVector3();
            softnessPos = file.ReadVector3();

            int neighbourCount = file.ReadInt();
            for (int i = 0; i < neighbourCount; i++)
            {
                neighbours.Add(new CrushPointNeighbour(file));
            }
        }
    }

    public class CrushPointNeighbour
    {
        int vertexIndex;
        float factor;

        public int VertexIndex
        {
            get => vertexIndex;
            set => vertexIndex = value;
        }

        public float Factor
        {
            get => factor;
            set => factor = value;
        }

        public CrushPointNeighbour() { }
        public CrushPointNeighbour(DocumentParser file)
            : this()
        {
            vertexIndex = file.ReadInt();
            factor = file.ReadSingle();
        }
    }

    public class BoundingBox
    {
        Vector3 min;
        Vector3 max;

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
}