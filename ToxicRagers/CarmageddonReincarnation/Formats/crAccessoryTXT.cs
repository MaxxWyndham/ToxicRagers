using System;
using System.Collections.Generic;

using ToxicRagers.Helpers;
using ToxicRagers.CarmageddonReincarnation.Helpers;

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
        StartingGrid
    }

    public class Accessory
    {
        // APP_DATA
        CustomAccessoryType customAccessoryType;
        Dictionary<string, string> customAccessoryProperties = new Dictionary<string, string>();
        string customAccessoryAudio;

        public CustomAccessoryType CustomAccessoryType
        {
            get { return customAccessoryType; }
            set { customAccessoryType = value; }
        }

        public Dictionary<string, string> CustomAccessoryProperties { get { return customAccessoryProperties; } }

        public string CustomAccessoryAudio
        {
            get { return customAccessoryAudio; }
            set { customAccessoryAudio = value; }
        }

        // ACOLYTE / ACOLYTE
        bool bPowerup;
        bool bHidden;

        public bool Powerup { get { return bPowerup; } set { bPowerup = value; } }
        public bool Hidden { get { return bHidden; } set { bHidden = value; } }

        // ANIMATION
        string animationType;
        string animationFile;
        Single animationSpeed;
        Single animationPhase;

        public string AnimationType
        {
            get { return animationType; }
            set { animationType = value; }
        }

        public string AnimationFile
        {
            get { return animationFile; }
            set { animationFile = value; }
        }

        public Single AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }

        public Single AnimationPhase
        {
            get { return animationPhase; }
            set { animationPhase = value; }
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

            using (var doc = new DocumentParser(pathToFile))
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
                                CustomAccessoryType accessoryType;
                                line = doc.ReadNextLine();

                                if (!Enum.TryParse<CustomAccessoryType>(line, out accessoryType)) { throw new NotImplementedException("Unknown CustomAccessoryType: " + line); }
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
            get { return lump; }
            set { lump = value; }
        }

        public Single Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public bool DrivableOn
        {
            get { return bDrivableOn; }
            set { bDrivableOn = value; }
        }

        public bool Solid
        {
            get { return bSolid; }
            set { bSolid = value; }
        }
        public bool StopSinkingIntoGround
        {
            get { return bStopSinkingIntoGround; }
            set { bStopSinkingIntoGround = value; }
        }
        public bool PartOfWorld
        {
            get { return bPartOfWorld; }
            set { bPartOfWorld = value; }
        }
        public bool Buoyant
        {
            get { return bBuouyant; }
            set { bBuouyant = value; }
        }
        public bool BuoyancyRelativeToCOM
        {
            get { return bBuoyancyRelativeToCOM; }
            set { bBuoyancyRelativeToCOM = value; }
        }

        public Single BuoyancyCount
        {
            get { return buoyancyCount; }
            set { buoyancyCount = value; }
        }

        public Vector3 BuoyancyVector
        {
            get { return buoyancyVector; }
            set { buoyancyVector = value; }
        }

        public Vector3 Moments
        {
            get { return moments; }
            set { moments = value; }
        }

        public Vector3 CentreOfMass
        {
            get { return centreOfMass; }
            set { centreOfMass = value; }
        }

        public AccessoryShape Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        public AccessoryBreak Break
        {
            get { return breakable; }
            set { breakable = value; }
        }

        public AccessoryJoint WorldJoint
        {
            get { return worldJoint; }
            set { worldJoint = value; }
        }

        public AccessoryJoint ChildJoint
        {
            get { return childJoint; }
            set { childJoint = value; }
        }

        public bool IgnoreWorld
        {
            get { return bIgnoreWorld; }
            set { bIgnoreWorld = value; }
        }

        public int Substance
        {
            get { return substance; }
            set { substance = value; }
        }

        public int Group
        {
            get { return group; }
            set { group = value; }
        }

        public int IgnoreGroup
        {
            get { return ignoreGroup; }
            set { ignoreGroup = value; }
        }

        public bool InfMass
        {
            get { return bInfMass; }
            set { bInfMass = value; }
        }
        public bool InfMi
        {
            get { return bInfMi; }
            set { bInfMi = value; }
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
            get { return name; }
            set { name = value; }
        }

        public int Count
        {
            get { return boxCount; }
            set { boxCount = value; }
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
        Single radius;
        int group;

        public ComponentType Type
        {
            get { return componentType; }
            set { componentType = value; }
        }

        public int Group
        {
            get { return group; }
            set { group = value; }
        }

        public Single Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public AccessoryShapeComponent(DocumentParser sr)
        {
            points = new List<Vector3>();

            string s = sr.ReadNextLine();
            int pointCount;

            switch (s)
            {
                case "AlignedCuboid":
                    this.componentType = ComponentType.AlignedCuboid;
                    points.Add(sr.ReadVector3());
                    points.Add(sr.ReadVector3());
                    break;

                case "Polyhedron":
                    this.componentType = ComponentType.Polyhedron;
                    pointCount = sr.ReadInt();
                    for (int i = 0; i < pointCount; i++) { points.Add(sr.ReadVector3()); }
                    break;

                case "RoundedPolyhedron":
                    this.componentType = ComponentType.RoundedPolyhedron;
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
            get { return breakImpulse; }
            set { breakImpulse = value; }
        }

        public List<AccessoryBreakReplacement> Replacements
        {
            get { return replacements; }
            set { replacements = value; }
        }

        public AccessoryBreak(DocumentParser sr)
        {
            bool bBreak = true;
            string[] settings;
            string line;

            if (!Accessory.TestLine("Breakable", sr.ReadNextLine(), out line)) { Console.WriteLine("Unexpected value: {0}", line); }

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
            string line;
            if (!Accessory.TestLine("joint", sr.ReadNextLine(), out line)) { Console.WriteLine("Unexpected value: {0}", line); }

            vertexNum = sr.ReadInt();
            var f1 = sr.ReadFloat();
            var v1 = sr.ReadVector3();
            var v2 = sr.ReadVector3();
            var v3 = sr.ReadVector3();
            var v4 = sr.ReadVector3();
            var v5 = sr.ReadVector3();
            var v6 = sr.ReadVector3();
            var v7 = sr.ReadVector3();
            var v8 = sr.ReadVector3();
            var i1 = sr.ReadInt();
            for (int i = 0; i < i1; i++)
            {
                var i2 = sr.ReadInt();
                var f2 = sr.ReadFloat();
                var f3 = sr.ReadFloat();
                var v9 = sr.ReadVector3();
                var v10 = sr.ReadVector3();
                var f4 = sr.ReadFloat();
                var s1 = sr.ReadNextLine();
                var v11 = sr.ReadVector3();
                var v12 = sr.ReadVector3();
            }
            if (!Accessory.TestLine("(no_weakness)", sr.ReadNextLine(), out line))
            {
                var s3 = sr.ReadNextLine();
                var v13 = sr.ReadVector3();
                var f5 = sr.ReadFloat();
                var s4 = sr.ReadNextLine();
                var v15 = sr.ReadVector3();

                var t = sr.ReadNextLine();
                if (t.Contains(","))
                {
                    var v_1 = Vector2.Parse(t);
                }
                else
                {
                    var f6 = t.ToSingle();
                }
            }
        }
    }
}