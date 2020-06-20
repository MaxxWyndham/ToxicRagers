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
        public CustomAccessoryType CustomAccessoryType { get; set; }

        public Dictionary<string, string> CustomAccessoryProperties { get; } = new Dictionary<string, string>();

        public string CustomAccessoryAudio { get; set; }

        // ACOLYTE / ACOYLTE
        public bool Powerup { get; set; }

        public bool Hidden { get; set; }

        // ANIMATION
        public string AnimationType { get; set; }

        public string AnimationFile { get; set; }

        public float AnimationSpeed { get; set; }

        public float AnimationPhase { get; set; }

        // DYNAMICS
        public List<AccessoryDynamics> Dynamics { get; } = new List<AccessoryDynamics>();

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
                                            accessory.CustomAccessoryProperties.Add(doc.ReadNextLine().Split(' '));
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
                            accessory.Dynamics.Add(new AccessoryDynamics(doc));
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
        public string Lump { get; set; }

        public float Mass { get; set; }

        public bool DrivableOn { get; set; }

        public bool Solid { get; set; }

        public bool StopSinkingIntoGround { get; set; }

        public bool PartOfWorld { get; set; }

        public bool Buoyant { get; set; }

        public bool BuoyancyRelativeToCOM { get; set; }

        public float BuoyancyCount { get; set; }

        public Vector3 BuoyancyVector { get; set; }

        public Vector3 Moments { get; set; }

        public Vector3 CentreOfMass { get; set; }

        public float SphereRollingResistance { get; set; }

        public AccessoryShape Shape { get; set; }

        public AccessoryBreak Break { get; set; }

        public AccessoryJoint WorldJoint { get; set; }

        public AccessoryJoint ChildJoint { get; set; }

        public bool IgnoreWorld { get; set; }

        public int Substance { get; set; }

        public int Group { get; set; }

        public int IgnoreGroup { get; set; }

        public bool InfMass { get; set; }

        public bool InfMi { get; set; }

        public AccessoryDynamics(DocumentParser doc)
        {
            while (!doc.NextLineIsASection() && !doc.EOF())
            {
                string line = doc.ReadNextLine();

                switch (line)
                {
                    case "<lump_name>":
                        Lump = doc.ReadNextLine();
                        break;

                    case "<mass>":
                        Mass = doc.ReadFloat();
                        break;

                    case "<drivable_on>":
                        DrivableOn = true;
                        break;

                    case "<solid>":
                        Solid = true;
                        break;

                    case "<stop_sinking_into_ground>":
                        StopSinkingIntoGround = true;
                        break;

                    case "<part_of_world>":
                        PartOfWorld = true;
                        break;

                    case "<ignore_world>":
                        IgnoreWorld = true;
                        break;

                    case "<inf_mass>":
                        InfMass = true;
                        break;

                    case "<inf_mi>":
                        InfMi = true;
                        break;

                    case "<buoyant>":
                        Buoyant = true;
                        BuoyancyCount = doc.ReadFloat();
                        BuoyancyVector = doc.ReadVector3();
                        break;

                    case "<substance>":
                        Substance = doc.ReadInt();
                        break;

                    case "<group>":
                        Group = doc.ReadInt();
                        break;

                    case "<ignore_group>":
                        IgnoreGroup = doc.ReadInt();
                        break;

                    case "<moments>":
                        Moments = doc.ReadVector3();
                        break;

                    case "<buoyancy_relative_to_com>":
                        BuoyancyRelativeToCOM = true;
                        break;

                    case "<centre_of_mass>":
                        CentreOfMass = doc.ReadVector3();
                        break;

                    case "<sphere_rolling_resistance>":
                        SphereRollingResistance = doc.ReadFloat();
                        break;

                    case "<shape>":
                        Shape = new AccessoryShape(doc);
                        break;

                    case "<breakable>":
                        Break = new AccessoryBreak(doc);
                        break;

                    case "<world_joint>":
                        WorldJoint = new AccessoryJoint(doc);
                        break;

                    case "<child_joint>":
                        ChildJoint = new AccessoryJoint(doc);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Unknown [DYNAMICS] setting: {0}", line));
                }
            }
        }
    }

    public class AccessoryShape
    {
        public string Name { get; set; }

        public List<AccessoryShapeComponent> Boxes { get; set; } = new List<AccessoryShapeComponent>();

        public AccessoryShape(DocumentParser sr)
        {
            Name = sr.ReadNextLine();

            if (Name.Replace("_", " ") != "(no shape)")
            {
                int boxCount = sr.ReadInt();

                for (int i = 0; i < boxCount; i++)
                {
                    Boxes.Add(new AccessoryShapeComponent(sr));
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

        public ComponentType Type { get; set; }

        public List<Vector3> Points { get; set; } = new List<Vector3>();

        public int Group { get; set; }

        public float Radius { get; set; }

        public AccessoryShapeComponent(DocumentParser sr)
        {
            string s = sr.ReadNextLine();
            int pointCount;

            switch (s)
            {
                case "AlignedCuboid":
                    Type = ComponentType.AlignedCuboid;
                    Points.Add(sr.ReadVector3());
                    Points.Add(sr.ReadVector3());
                    break;

                case "Polyhedron":
                    Type = ComponentType.Polyhedron;
                    pointCount = sr.ReadInt();
                    for (int i = 0; i < pointCount; i++) { Points.Add(sr.ReadVector3()); }
                    break;

                case "RoundedPolyhedron":
                    Type = ComponentType.RoundedPolyhedron;
                    Radius = sr.ReadFloat();
                    pointCount = sr.ReadInt();
                    for (int i = 0; i < pointCount; i++) { Points.Add(sr.ReadVector3()); }
                    break;

                case "Sphere":
                    Points.Add(sr.ReadVector3());
                    Radius = sr.ReadFloat();
                    break;

                case "TicTac":
                    Points.Add(sr.ReadVector3());
                    Points.Add(sr.ReadVector3());
                    Radius = sr.ReadFloat();
                    break;

                default:
                    throw new NotImplementedException($"Unknown ComponentType: {s}");
            }

            while (sr.ReadNextLine() == "form_collision_groups")
            {
                Group = sr.ReadInt();
            }

            sr.Rewind();
        }
    }

    public class AccessoryBreak
    {
        public int BreakImpulse { get; set; }

        public List<AccessoryBreakReplacement> Replacements { get; set; } = new List<AccessoryBreakReplacement>();

        public bool DetatchChildren { get; set; }

        public bool DestroyChildren { get; set; }

        public int ExplodeForce { get; set; }

        public Vector2 RandomRotation { get; set; }

        public bool TriggerParticles { get; set; }

        public string Sound { get; set; }

        public AccessoryBreak(DocumentParser sr)
        {
            bool inBreak = true;
            string[] settings;

            if (!Accessory.TestLine("Breakable", sr.ReadNextLine(), out string line)) { Console.WriteLine("Unexpected value: {0}", line); }

            while (inBreak)
            {
                line = sr.ReadNextLine();
                if (line == null) { break; }

                settings = line.Split(' ');

                switch (settings[0].ToLower())
                {
                    case "break":
                    case "break_impulse":
                        BreakImpulse = settings[settings.Length - 1].ToInt();
                        break;

                    case "detach_children":
                    case "detatch_children":
                        DetatchChildren = true;
                        break;

                    case "destroy_children":
                        DestroyChildren = true;
                        break;

                    case "explode":
                    case "explode_force":
                        ExplodeForce = settings[settings.Length - 1].ToInt();
                        break;

                    case "random_rotation":
                        if (settings.Length == 2)
                        {
                            RandomRotation = Vector2.Parse(settings[1]);
                        }
                        else
                        {
                            RandomRotation = new Vector2(settings[1].ToSingle(), settings[2].ToSingle());
                        }
                        break;

                    case "sound":
                        Sound = settings[1];
                        break;

                    case "trigger_particles":
                        TriggerParticles = true;
                        break;

                    case "collision_with_world_will_break_me":
                        Console.WriteLine("collision_with_world_will_break_me: {0}", settings[1]);
                        break;

                    case "replace":
                        if (settings.Length == 3)
                        {
                            Replacements.Add(new AccessoryBreakReplacement(settings[1], settings[2]));
                        }
                        else
                        {
                            Replacements.Add(new AccessoryBreakReplacement(settings[1]));
                        }
                        break;

                    default:
                        inBreak = false;

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
        public string Direction { get; set; }

        public string Model { get; set; }

        public AccessoryBreakReplacement(string model) : this("", model) { }

        public AccessoryBreakReplacement(string direction, string model)
        {
            Direction = direction;
            Model = model;
        }
    }

    public class AccessoryJoint
    {
        public int VertexNum { get; set; }

        public AccessoryJoint(DocumentParser sr)
        {
            if (!Accessory.TestLine("joint", sr.ReadNextLine(), out string line)) { Console.WriteLine("Unexpected value: {0}", line); }

            throw new NotImplementedException("AccessoryJoint not supported!");

            VertexNum = sr.ReadInt();
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