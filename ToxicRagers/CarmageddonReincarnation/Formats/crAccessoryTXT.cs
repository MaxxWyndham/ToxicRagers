using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static CultureInfo Culture = new CultureInfo("en-GB");

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
        string lump;
        Single mass;
        bool bDrivableOn;
        bool bSolid;
        bool bStopSinkingIntoGround;
        bool bPartOfWorld;
        bool bBuouyant;
        Single buoyancyCount;
        Vector3 buoyancyVector;
        Vector3 moments;
        bool bBuoyancyRelativeToCOM;
        Vector3 centreOfMass;
        AccessoryShape shape;
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

        public bool DrivableOn { get { return bDrivableOn; } set { bDrivableOn = value; } }
        public bool Solid { get { return bSolid; } set { bSolid = value; } }
        public bool StopSinkingIntoGround { get { return bStopSinkingIntoGround; } set { bStopSinkingIntoGround = value; } }
        public bool PartOfWorld { get { return bPartOfWorld; } set { bPartOfWorld = value; } }
        public bool Buoyant { get { return bBuouyant; } set { bBuouyant = value; } }
        public bool BuoyancyRelativeToCOM { get { return bBuoyancyRelativeToCOM; } set { bBuoyancyRelativeToCOM = value; } }

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

        public bool IgnoreWorld { get { return bIgnoreWorld; } set { bIgnoreWorld = value; } }

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

        public bool InfMass { get { return bInfMass; } set { bInfMass = value; } }
        public bool InfMi { get { return bInfMi; } set { bInfMi = value; } }

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
                                        // Do nothing
                                        break;

                                    case CustomAccessoryType.AngularDampedAccessory:
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // damper
                                        break;

                                    case CustomAccessoryType.Checkpoint:
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // bottom
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // adjust_left
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // adjust_right
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // current
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // next
                                        break;

                                    case CustomAccessoryType.Powerup:
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // powerup name
                                        break;

                                    case CustomAccessoryType.ManagedAccessory:
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // break_fuse
                                        break;

                                    case CustomAccessoryType.RigidBodyAnimation:
                                    case CustomAccessoryType.RockingAccessory:
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // anim
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  // looping or message_triggered

                                        if (accessory.CustomAccessoryProperties.ContainsKey("looping"))
                                        {
                                            while (true)
                                            {
                                                if (!TestLine("[DYNAMICS]", doc.ReadNextLine(), out line))
                                                {
                                                    if (line.ToLower().StartsWith("accessoryaudio"))
                                                    {
                                                        doc.Rewind();
                                                        break;
                                                    }

                                                    accessory.CustomAccessoryProperties.Add(line.Split(' '));
                                                }
                                                else
                                                {
                                                    doc.Rewind();
                                                    break;
                                                }
                                            }
                                        }
                                        break;

                                    case CustomAccessoryType.RotatingAccessory:
                                        accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));  //  speed
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
                            accessory.AnimationSpeed = Convert.ToSingle(doc.ReadNextLine(), Culture);
                            accessory.AnimationPhase = Convert.ToSingle(doc.ReadNextLine(), Culture);
                            line = doc.ReadNextLine();
                            break;

                        case "[DYNAMICS]":
                            bool bDynamics = true;

                            while (bDynamics)
                            {
                                line = doc.ReadNextLine();

                                switch (line)
                                {
                                    case "<lump_name>":
                                        accessory.Lump = doc.ReadNextLine();
                                        break;

                                    case "<mass>":
                                        accessory.Mass = Convert.ToSingle(doc.ReadNextLine(), Culture);
                                        break;

                                    case "<drivable_on>":
                                        accessory.DrivableOn = true;
                                        break;

                                    case "<solid>":
                                        accessory.Solid = true;
                                        break;

                                    case "<stop_sinking_into_ground>":
                                        accessory.StopSinkingIntoGround = true;
                                        break;

                                    case "<part_of_world>":
                                        accessory.PartOfWorld = true;
                                        break;

                                    case "<ignore_world>":
                                        accessory.IgnoreWorld = true;
                                        break;

                                    case "<inf_mass>":
                                        accessory.InfMass = true;
                                        break;

                                    case "<inf_mi>":
                                        accessory.InfMi = true;
                                        break;

                                    case "<buoyant>":
                                        accessory.Buoyant = true;
                                        accessory.BuoyancyCount = Convert.ToSingle(doc.ReadNextLine(), Culture);
                                        accessory.BuoyancyVector = Vector3.Parse(doc.ReadNextLine());
                                        break;

                                    case "<substance>":
                                        accessory.Substance = Convert.ToInt32(doc.ReadNextLine(), Culture);
                                        break;

                                    case "<group>":
                                        accessory.Group = Convert.ToInt32(doc.ReadNextLine(), Culture);
                                        break;

                                    case "<ignore_group>":
                                        accessory.IgnoreGroup = Convert.ToInt32(doc.ReadNextLine(), Culture);
                                        break;

                                    case "<moments>":
                                        accessory.Moments = Vector3.Parse(doc.ReadNextLine());
                                        break;

                                    case "<buoyancy_relative_to_com>":
                                        accessory.BuoyancyRelativeToCOM = true;
                                        break;

                                    case "<centre_of_mass>":
                                        accessory.CentreOfMass = Vector3.Parse(doc.ReadNextLine());
                                        break;

                                    case "<shape>":
                                        accessory.Shape = new AccessoryShape(doc);
                                        break;

                                    case "<breakable>":
                                        accessory.Break = new AccessoryBreak(doc);
                                        break;

                                    case "<world_joint>":
                                        accessory.WorldJoint = new AccessoryJoint(doc);
                                        break;

                                    case "<child_joint>":
                                        accessory.ChildJoint = new AccessoryJoint(doc);
                                        break;

                                    default:
                                        bDynamics = false;
                                        if (line != null && !line.StartsWith("[")) { Console.WriteLine("Unexpected [DYNAMICS] line: " + line); }
                                        break;
                                }
                            }
                            break;

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
                boxCount = Convert.ToInt32(sr.ReadNextLine(), Accessory.Culture);

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
                    points.Add(Vector3.Parse(sr.ReadNextLine()));
                    points.Add(Vector3.Parse(sr.ReadNextLine()));
                    break;

                case "Polyhedron":
                    this.componentType = ComponentType.Polyhedron;
                    pointCount = Convert.ToInt32(sr.ReadNextLine(), Accessory.Culture);
                    for (int i = 0; i < pointCount; i++) { points.Add(Vector3.Parse(sr.ReadNextLine())); }
                    break;

                case "RoundedPolyhedron":
                    this.componentType = ComponentType.RoundedPolyhedron;
                    radius = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
                    pointCount = Convert.ToInt32(sr.ReadNextLine(), Accessory.Culture);
                    for (int i = 0; i < pointCount; i++) { points.Add(Vector3.Parse(sr.ReadNextLine())); }
                    break;

                case "Sphere":
                    points.Add(Vector3.Parse(sr.ReadNextLine()));
                    radius = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
                    break;

                case "TicTac":
                    points.Add(Vector3.Parse(sr.ReadNextLine()));
                    points.Add(Vector3.Parse(sr.ReadNextLine()));
                    radius = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
                    break;

                default:
                    throw new NotImplementedException("Unknown ComponentType: " + s);
            }

            if (sr.ReadNextLine() == "form_collision_groups")
            {
                group = Convert.ToInt32(sr.ReadNextLine(), Accessory.Culture);
            }
            else
            {
                sr.Rewind();
            }
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
                        breakImpulse = Convert.ToInt32(settings[settings.Length - 1], Accessory.Culture);
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
                        explodeForce = Convert.ToInt32(settings[settings.Length - 1], Accessory.Culture);
                        break;

                    case "random_rotation":
                        if (settings.Length == 2)
                        {
                            randomRotation = Vector2.Parse(settings[1]);
                        }
                        else
                        {
                            randomRotation = Vector2.Parse(settings[1] + "," + settings[2]);
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

            vertexNum = Convert.ToInt32(sr.ReadNextLine(), Accessory.Culture);
            var f1 = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
            var v1 = Vector3.Parse(sr.ReadNextLine());
            var v2 = Vector3.Parse(sr.ReadNextLine());
            var v3 = Vector3.Parse(sr.ReadNextLine());
            var v4 = Vector3.Parse(sr.ReadNextLine());
            var v5 = Vector3.Parse(sr.ReadNextLine());
            var v6 = Vector3.Parse(sr.ReadNextLine());
            var v7 = Vector3.Parse(sr.ReadNextLine());
            var v8 = Vector3.Parse(sr.ReadNextLine());
            var i1 = Convert.ToInt32(sr.ReadNextLine(), Accessory.Culture);
            for (int i = 0; i < i1; i++)
            {
                var i2 = Convert.ToInt32(sr.ReadNextLine(), Accessory.Culture);
                var f2 = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
                var f3 = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
                var v9 = Vector3.Parse(sr.ReadNextLine());
                var v10 = Vector3.Parse(sr.ReadNextLine());
                var f4 = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
                var s1 = sr.ReadNextLine();
                var v11 = Vector3.Parse(sr.ReadNextLine());
                var v12 = Vector3.Parse(sr.ReadNextLine());
            }
            if (!Accessory.TestLine("(no_weakness)", sr.ReadNextLine(), out line))
            {
                var s3 = sr.ReadNextLine();
                var v13 = Vector3.Parse(sr.ReadNextLine());
                var f5 = Convert.ToSingle(sr.ReadNextLine(), Accessory.Culture);
                var s4 = sr.ReadNextLine();
                var v15 = Vector3.Parse(sr.ReadNextLine());

                var t = sr.ReadNextLine();
                if (t.Contains(","))
                {
                    var v_1 = Vector2.Parse(t);
                }
                else
                {
                    var f6 = Convert.ToSingle(t, Accessory.Culture);
                }
            }
        }
    }
}
