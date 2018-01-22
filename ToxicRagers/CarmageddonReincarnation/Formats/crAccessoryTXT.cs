using System;
using System.Collections.Generic;

using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public enum CustomAccessoryType
    {
        None,
        AngularDampedAccessory,
        Checkpoint,
        ConveyorAccessory,
        CopSpawn,
        ExplodingAccessory,
        GibletAccessoryType,
        ManagedAccessory,
        MultiplayerSpawn,
        Powerup,
        RigidBodyAnimation,
        RockingAccessory,
        RotatingAccessory,
        StandardAccessory,
        StartingGrid,
        TrailerSpawn,
        Bicycle
    }

    public class Accessory
    {
        // APP_DATA
        CustomAccessoryType customAccessoryType;
        Dictionary<string, string> customAccessoryProperties = new Dictionary<string, string>();
        string customAccessoryAudio;

        public CustomAccessoryType CustomAccessoryType
        {
            get => customAccessoryType;
            set => customAccessoryType = value;
        }

        public Dictionary<string, string> CustomAccessoryProperties => customAccessoryProperties;

        public string CustomAccessoryAudio
        {
            get => customAccessoryAudio;
            set => customAccessoryAudio = value;
        }

        // ACOLYTE / ACOLYTE
        bool bPowerup;
        bool bHidden;

        public bool Powerup
        {
            get => bPowerup;
            set => bPowerup = value;
        }

        public bool Hidden
        {
            get => bHidden;
            set => bHidden = value;
        }

        // ANIMATION
        string animationType;
        string animationFile;
        float animationSpeed;
        float animationPhase;

        public string AnimationType
        {
            get => animationType;
            set => animationType = value;
        }

        public string AnimationFile
        {
            get => animationFile;
            set => animationFile = value;
        }

        public float AnimationSpeed
        {
            get => animationSpeed;
            set => animationSpeed = value;
        }

        public float AnimationPhase
        {
            get => animationPhase;
            set => animationPhase = value;
        }

        // DYNAMICS
        List<AccessoryDynamics> dynamics;

        public Accessory()
        {
            dynamics = new List<AccessoryDynamics>();
        }

        public static Accessory Load(string pathToFile)
        {
            Accessory accessory = new Accessory();

            using (DocumentParser doc = new DocumentParser(pathToFile))
            {
                string line = doc.ReadNextLine();

                while (line != null)
                {
                    switch (line)
                    {
                        case "[LUMP]":
                            doc.ReadNextLine();  // level
                            line = doc.ReadNextLine();
                            break;

                        case "[VERSION]":
                            doc.ReadNextLine();  // 2.500000
                            line = doc.ReadNextLine();
                            break;

                        case "[RACE_LAYERS]":
                            line = doc.SkipToNextSection();
                            break;

                        case "[LUA_SCRIPTS]":
                            line = doc.SkipToNextSection();
                            break;

                        case "[APP_DATA]":
                            if (TestLine("<CustomAccessoryType>", doc.ReadNextLine(), out line))
                            {
                                line = doc.ReadNextLine();

                                if (!Enum.TryParse<CustomAccessoryType>(line, out CustomAccessoryType accessoryType)) { throw new NotImplementedException("Unknown CustomAccessoryType: " + line); }
                                accessory.CustomAccessoryType = accessoryType;

                                switch (accessoryType)
                                {
                                    case CustomAccessoryType.ConveyorAccessory:
                                    case CustomAccessoryType.CopSpawn:
                                    case CustomAccessoryType.ExplodingAccessory:
                                    case CustomAccessoryType.GibletAccessoryType:
                                    case CustomAccessoryType.MultiplayerSpawn:
                                    case CustomAccessoryType.StandardAccessory:
                                    case CustomAccessoryType.StartingGrid:
                                    case CustomAccessoryType.AngularDampedAccessory:
                                    case CustomAccessoryType.Checkpoint:
                                    case CustomAccessoryType.Powerup:
                                    case CustomAccessoryType.ManagedAccessory:
                                    case CustomAccessoryType.RigidBodyAnimation:
                                    case CustomAccessoryType.RockingAccessory:
                                    case CustomAccessoryType.RotatingAccessory:
                                    case CustomAccessoryType.TrailerSpawn:
                                    case CustomAccessoryType.Bicycle:
                                        while (!doc.NextLineIsASection() && !doc.EOF())
                                        {
                                            accessory.customAccessoryProperties.Add(doc.ReadNextLine().Split(' '));
                                        }
                                        break;

                                    default:
                                        Console.WriteLine("Unhandled accessory");
                                        break;
                                }

                                line = doc.ReadNextLine();

                                if (line != null && line.ToLower().StartsWith("accessoryaudio"))
                                {
                                    accessory.CustomAccessoryAudio = line.Replace("accessoryaudio ", "", StringComparison.OrdinalIgnoreCase);
                                    line = doc.ReadNextLine();
                                }
                            }
                            break;

                        case "[ACOLYTE]":
                        case "[ACOYLTE]":
                            bool bAcolyte = true;

                            while (bAcolyte)
                            {
                                line = doc.ReadNextLine();

                                switch (line)
                                {
                                    case "<powerup>":
                                        accessory.Powerup = true;
                                        break;

                                    case "<hidden>":
                                        accessory.Hidden = true;
                                        break;

                                    default:
                                        bAcolyte = false;
                                        if (line != null && !line.StartsWith("[")) { Console.WriteLine("Unexpected [ACOLYTE] line: " + line); }
                                        break;
                                }
                            }
                            break;

                        case "[ANIMATION]":
                            accessory.AnimationType = doc.ReadNextLine();
                            accessory.AnimationFile = doc.ReadNextLine();
                            accessory.AnimationSpeed = doc.ReadFloat();
                            accessory.AnimationPhase = doc.ReadFloat();
                            line = doc.ReadNextLine();
                            break;

                        case "[DYNAMICS]":
                            accessory.dynamics.Add(new AccessoryDynamics(doc));
                            line = doc.ReadNextLine();
                            break;

                        default:
                            Console.WriteLine(pathToFile);
                            throw new NotImplementedException("Unexpected [SECTION]: " + line);
                    }
                }
            }

            return accessory;
        }

        public static bool TestLine(string expected, string received, out string line)
        {
            line = received;
            if (expected == received) { return true; }
            return false;
        }
    }

    public class AccessoryDynamics
    {
        string lump;
        float mass;
        bool bDrivableOn;
        bool bSolid;
        bool bStopSinkingIntoGround;
        bool bPartOfWorld;
        bool bBuouyant;
        float buoyancyCount;
        Vector3 buoyancyVector;
        Vector3 moments;
        bool bBuoyancyRelativeToCOM;
        Vector3 centreOfMass;
        AccessoryShape shape;
        float sphereRollingResistance;
        AccessoryBreak breakable;
        AccessoryJoint worldJoint;
        AccessoryJoint childJoint;
        bool bIgnoreWorld;
        int substance;
        bool bInfMass;
        int group;
        int ignoreGroup;
        bool bInfMi;

        public string Lump
        {
            get => lump;
            set => lump = value;
        }

        public float Mass
        {
            get => mass;
            set => mass = value;
        }

        public bool DrivableOn
        {
            get => bDrivableOn;
            set => bDrivableOn = value;
        }

        public bool Solid
        {
            get => bSolid;
            set => bSolid = value;
        }

        public bool StopSinkingIntoGround
        {
            get => bStopSinkingIntoGround;
            set => bStopSinkingIntoGround = value;
        }

        public bool PartOfWorld
        {
            get => bPartOfWorld;
            set => bPartOfWorld = value;
        }

        public bool Buoyant
        {
            get => bBuouyant;
            set => bBuouyant = value;
        }

        public bool BuoyancyRelativeToCOM
        {
            get => bBuoyancyRelativeToCOM;
            set => bBuoyancyRelativeToCOM = value;
        }

        public float BuoyancyCount
        {
            get => buoyancyCount;
            set => buoyancyCount = value;
        }

        public Vector3 BuoyancyVector
        {
            get => buoyancyVector;
            set => buoyancyVector = value;
        }

        public Vector3 Moments
        {
            get => moments;
            set => moments = value;
        }

        public Vector3 CentreOfMass
        {
            get => centreOfMass;
            set => centreOfMass = value;
        }

        public AccessoryShape Shape
        {
            get => shape;
            set => shape = value;
        }

        public AccessoryBreak Break
        {
            get => breakable;
            set => breakable = value;
        }

        public AccessoryJoint WorldJoint
        {
            get => worldJoint;
            set => worldJoint = value;
        }

        public AccessoryJoint ChildJoint
        {
            get => childJoint;
            set => childJoint = value;
        }

        public bool IgnoreWorld
        {
            get => bIgnoreWorld;
            set => bIgnoreWorld = value;
        }

        public int Substance
        {
            get => substance;
            set => substance = value;
        }

        public int Group
        {
            get => group;
            set => group = value;
        }

        public int IgnoreGroup
        {
            get => ignoreGroup;
            set => ignoreGroup = value;
        }

        public bool InfMass
        {
            get => bInfMass;
            set => bInfMass = value;
        }

        public bool InfMi
        {
            get => bInfMi;
            set => bInfMi = value;
        }

        public AccessoryDynamics(DocumentParser doc)
        {
            while (!doc.NextLineIsASection() && !doc.EOF())
            {
                string line = doc.ReadNextLine();

                switch (line)
                {
                    case "<lump_name>":
                        lump = doc.ReadNextLine();
                        break;

                    case "<mass>":
                        mass = doc.ReadFloat();
                        break;

                    case "<drivable_on>":
                        bDrivableOn = true;
                        break;

                    case "<solid>":
                        bSolid = true;
                        break;

                    case "<stop_sinking_into_ground>":
                        bStopSinkingIntoGround = true;
                        break;

                    case "<part_of_world>":
                        bPartOfWorld = true;
                        break;

                    case "<ignore_world>":
                        bIgnoreWorld = true;
                        break;

                    case "<inf_mass>":
                        bInfMass = true;
                        break;

                    case "<inf_mi>":
                        bInfMi = true;
                        break;

                    case "<buoyant>":
                        bBuouyant = true;
                        buoyancyCount = doc.ReadFloat();
                        buoyancyVector = doc.ReadVector3();
                        break;

                    case "<substance>":
                        substance = doc.ReadInt();
                        break;

                    case "<group>":
                        group = doc.ReadInt();
                        break;

                    case "<ignore_group>":
                        ignoreGroup = doc.ReadInt();
                        break;

                    case "<moments>":
                        moments = doc.ReadVector3();
                        break;

                    case "<buoyancy_relative_to_com>":
                        bBuoyancyRelativeToCOM = true;
                        break;

                    case "<centre_of_mass>":
                        centreOfMass = doc.ReadVector3();
                        break;

                    case "<sphere_rolling_resistance>":
                        sphereRollingResistance = doc.ReadFloat();
                        break;

                    case "<shape>":
                        shape = new AccessoryShape(doc);
                        break;

                    case "<breakable>":
                        breakable = new AccessoryBreak(doc);
                        break;

                    case "<world_joint>":
                        worldJoint = new AccessoryJoint(doc);
                        break;

                    case "<child_joint>":
                        childJoint = new AccessoryJoint(doc);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Unknown [DYNAMICS] setting: {0}", line));
                }
            }
        }
    }

    public class AccessoryShape
    {
        string name;
        int boxCount;
        List<AccessoryShapeComponent> boxes = new List<AccessoryShapeComponent>();

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Count
        {
            get => boxCount;
            set => boxCount = value;
        }

        public AccessoryShape(DocumentParser sr)
        {
            name = sr.ReadNextLine();

            if (name.Replace("_", " ") != "(no shape)")
            {
                boxCount = sr.ReadInt();

                for (int i = 0; i < boxCount; i++)
                {
                    boxes.Add(new AccessoryShapeComponent(sr));
                }
            }
        }
    }

    public class AccessoryShapeComponent
    {
        public enum ComponentType
        {
            AlignedCuboid,
            Polyhedron,
            RoundedPolyhedron,
            Sphere,
            TicTac
        }

        ComponentType componentType;
        List<Vector3> points;
        float radius;
        int group;

        public ComponentType Type
        {
            get => componentType;
            set => componentType = value;
        }

        public int Group
        {
            get => group;
            set => group = value;
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public AccessoryShapeComponent(DocumentParser sr)
        {
            points = new List<Vector3>();

            string s = sr.ReadNextLine();
            int pointCount;

            switch (s)
            {
                case "AlignedCuboid":
                    componentType = ComponentType.AlignedCuboid;
                    points.Add(sr.ReadVector3());
                    points.Add(sr.ReadVector3());
                    break;

                case "Polyhedron":
                    componentType = ComponentType.Polyhedron;
                    pointCount = sr.ReadInt();
                    for (int i = 0; i < pointCount; i++) { points.Add(sr.ReadVector3()); }
                    break;

                case "RoundedPolyhedron":
                    componentType = ComponentType.RoundedPolyhedron;
                    radius = sr.ReadFloat();
                    pointCount = sr.ReadInt();
                    for (int i = 0; i < pointCount; i++) { points.Add(sr.ReadVector3()); }
                    break;

                case "Sphere":
                    points.Add(sr.ReadVector3());
                    radius = sr.ReadFloat();
                    break;

                case "TicTac":
                    points.Add(sr.ReadVector3());
                    points.Add(sr.ReadVector3());
                    radius = sr.ReadFloat();
                    break;

                default:
                    throw new NotImplementedException("Unknown ComponentType: " + s);
            }

            while (sr.ReadNextLine() == "form_collision_groups")
            {
                group = sr.ReadInt();
            }

            sr.Rewind();
        }
    }

    public class AccessoryBreak
    {
        List<AccessoryBreakReplacement> replacements = new List<AccessoryBreakReplacement>();
        int breakImpulse;
        bool bDetatchChildren;
        bool bDestroyChildren;
        int explodeForce;
        Vector2 randomRotation;
        bool bTriggerParticles;
        string sound;

        public int BreakImpulse
        {
            get => breakImpulse;
            set => breakImpulse = value;
        }

        public List<AccessoryBreakReplacement> Replacements
        {
            get => replacements;
            set => replacements = value;
        }

        public AccessoryBreak(DocumentParser sr)
        {
            bool bBreak = true;
            string[] settings;

            if (!Accessory.TestLine("Breakable", sr.ReadNextLine(), out string line)) { Console.WriteLine("Unexpected value: {0}", line); }

            while (bBreak)
            {
                line = sr.ReadNextLine();
                if (line == null) { break; }

                settings = line.Split(' ');

                switch (settings[0].ToLower())
                {
                    case "break":
                    case "break_impulse":
                        breakImpulse = settings[settings.Length - 1].ToInt();
                        break;

                    case "detach_children":
                    case "detatch_children":
                        bDetatchChildren = true;
                        break;

                    case "destroy_children":
                        bDestroyChildren = true;
                        break;

                    case "explode":
                    case "explode_force":
                        explodeForce = settings[settings.Length - 1].ToInt();
                        break;

                    case "random_rotation":
                        if (settings.Length == 2)
                        {
                            randomRotation = Vector2.Parse(settings[1]);
                        }
                        else
                        {
                            randomRotation = new Vector2(settings[1].ToSingle(), settings[2].ToSingle());
                        }
                        break;

                    case "sound":
                        sound = settings[1];
                        break;

                    case "trigger_particles":
                        bTriggerParticles = true;
                        break;

                    case "collision_with_world_will_break_me":
                        Console.WriteLine("collision_with_world_will_break_me: {0}", settings[1]);
                        break;

                    case "replace":
                        if (settings.Length == 3)
                        {
                            replacements.Add(new AccessoryBreakReplacement(settings[1], settings[2]));
                        }
                        else
                        {
                            replacements.Add(new AccessoryBreakReplacement(settings[1]));
                        }
                        break;

                    default:
                        bBreak = false;

                        if (!settings[0].StartsWith("[") && !settings[0].StartsWith("<"))
                        {
                            Console.WriteLine("Unexpected setting: {0}", settings[0]);
                        }
                        else
                        {
                            sr.Rewind();
                        }
                        break;
                }
            }
        }

        public void Save()
        {
            // Pointless code makes visual studio warnings go away
            bool b = bDetatchChildren &
                bDestroyChildren &
                bTriggerParticles;
        }
    }

    public class AccessoryBreakReplacement
    {
        string direction;
        string model;

        public AccessoryBreakReplacement(string Model) : this("", Model) { }

        public AccessoryBreakReplacement(string Direction, string Model)
        {
            direction = Direction;
            model = Model;
        }
    }

    public class AccessoryJoint
    {
        int vertexNum;

        public AccessoryJoint(DocumentParser sr)
        {
            if (!Accessory.TestLine("joint", sr.ReadNextLine(), out string line)) { Console.WriteLine("Unexpected value: {0}", line); }

            vertexNum = sr.ReadInt();
            float f1 = sr.ReadFloat();
            Vector3 v1 = sr.ReadVector3();
            Vector3 v2 = sr.ReadVector3();
            Vector3 v3 = sr.ReadVector3();
            Vector3 v4 = sr.ReadVector3();
            Vector3 v5 = sr.ReadVector3();
            Vector3 v6 = sr.ReadVector3();
            Vector3 v7 = sr.ReadVector3();
            Vector3 v8 = sr.ReadVector3();
            int i1 = sr.ReadInt();
            for (int i = 0; i < i1; i++)
            {
                int i2 = sr.ReadInt();
                float f2 = sr.ReadFloat();
                float f3 = sr.ReadFloat();
                Vector3 v9 = sr.ReadVector3();
                Vector3 v10 = sr.ReadVector3();
                float f4 = sr.ReadFloat();
                string s1 = sr.ReadNextLine();
                Vector3 v11 = sr.ReadVector3();
                Vector3 v12 = sr.ReadVector3();
            }
            if (!Accessory.TestLine("(no_weakness)", sr.ReadNextLine(), out line))
            {
                string s3 = sr.ReadNextLine();
                Vector3 v13 = sr.ReadVector3();
                float f5 = sr.ReadFloat();
                string s4 = sr.ReadNextLine();
                Vector3 v15 = sr.ReadVector3();

                string t = sr.ReadNextLine();
                if (t.Contains(","))
                {
                    Vector2 v_1 = Vector2.Parse(t);
                }
                else
                {
                    float f6 = t.ToSingle();
                }
            }
        }
    }
}