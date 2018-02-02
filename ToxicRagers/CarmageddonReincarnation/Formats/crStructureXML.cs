using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

using ToxicRagers.CarmageddonReincarnation.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public enum StructurePhysicsProperty
    {
        FRONT_LEFT_POINT_OF_SUSPENSION,
        FRONT_LEFT_POINT_OF_STEERING,
        FRONT_LEFT_POINT_OF_ROTATION,
        FRONT_LEFT_WHEEL,
        FRONT_RIGHT_POINT_OF_SUSPENSION,
        FRONT_RIGHT_POINT_OF_STEERING,
        FRONT_RIGHT_POINT_OF_ROTATION,
        FRONT_RIGHT_WHEEL,
        LEFT_STEERING,
        REAR_LEFT_POINT_OF_SUSPENSION,
        REAR_LEFT_POINT_OF_ROTATION,
        REAR_LEFT_WHEEL,
        REAR_RIGHT_POINT_OF_SUSPENSION,
        REAR_RIGHT_POINT_OF_ROTATION,
        REAR_RIGHT_WHEEL,
        RIGHT_STEERING,
        STEERING_WHEEL
    }

    public enum StructurePartVariable
    {
        AIR_BRAKE,
        CONSTANT_OVER_TIME,
        ENGINE_CRANK_ANGLE,
        ENGINE_NORMALISED_RPM,
        GEARBOX_OUTPUT_ANGLE,
        SPEED_DEPENDENT_AEROFOIL,
        SPEED_DEPENDENT_AEROFOIL_2,
        SPEED_OVER_TIME,
        STEERING,
        WHEEL_ROTATION_FL,
        WHEEL_ROTATION_FR,
        WHEEL_ROTATION_RL,
        WHEEL_ROTATION_RR
    }

    public class Structure
    {
        StructureCharacteristicsCode characteristics;
        StructurePart root;

        public StructureCharacteristicsCode Characteristics
        {
            get => characteristics;
            set => characteristics = value;
        }

        public StructurePart Root
        {
            get => root;
            set => root = value;
        }

        public Structure()
        {
            characteristics = new StructureCharacteristicsCode();
        }

        public static Structure Load(string path)
        {
            Structure structure = new Structure();

            using (XMLParser xml = new XMLParser(path, "STRUCTURE"))
            {
                structure.characteristics = StructureCharacteristicsCode.Parse(xml.GetNode("CHARACTERISTICS").FirstChild.InnerText);
                structure.root = new StructurePart(xml.GetNode("ROOT"));
            }

            return structure;
        }

        public void Save(string path)
        {
            XDocument xml = new XDocument();

            XElement structure = new XElement("STRUCTURE");
            structure.Add(new XElement("CHARACTERISTICS", new XCData(characteristics.ToString())));
            structure.Add(root.Write());
            xml.Add(structure);

            XMLWriter.Save(xml, path);
        }
    }

    public class StructurePart
    {
        bool bIsRoot = false;
        string name;
        List<StructurePart> parts;
        List<StructureWeld> welds;
        List<StructureJoint> joints;

        StructureDamageCode damage;

        public bool IsRoot
        {
            get => bIsRoot;
            set => bIsRoot = true;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public List<StructurePart> Parts
        {
            get => parts;
            set => parts = value;
        }

        public List<StructureWeld> Welds
        {
            get => welds;
            set => welds = value;
        }

        public List<StructureJoint> Joints
        {
            get => joints;
            set => joints = value;
        }

        public StructureDamageCode DamageSettings => damage;

        public StructurePart()
        {
            damage = new StructureDamageCode();

            parts = new List<StructurePart>();
            welds = new List<StructureWeld>();
            joints = new List<StructureJoint>();
        }

        public StructurePart(XmlNode node)
            : this()
        {
            bIsRoot = (node.Name == "ROOT");

            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "name":
                        name = attribute.Value;
                        break;

                    default:
                        throw new NotImplementedException("Unknown Part attribute: " + attribute.Name);
                }
            }

            foreach (XmlNode data in node.ChildNodes)
            {
                switch (data.NodeType)
                {
                    case XmlNodeType.CDATA:
                        damage = StructureDamageCode.Parse(data.InnerText);
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            case "PART":
                                parts.Add(new StructurePart(data));
                                break;

                            case "WELD":
                                welds.Add(new StructureWeld(data));
                                break;

                            case "JOINT":
                                joints.Add(new StructureJoint(data));
                                break;

                            default:
                                throw new NotImplementedException("Unknown Element of PART: " + data.Name);
                        }
                        break;
                }
            }
        }

        public XElement Write()
        {
            XElement xe = new XElement((IsRoot ? "ROOT" : "PART"));
            xe.Add(new XAttribute("name", name));

            string damageCDATA = damage.ToString();
            if (damageCDATA.Trim() != "") { xe.Add(new XCData(damageCDATA)); }

            foreach (StructureWeld weld in welds)
            {
                xe.Add(weld.Write());
            }

            foreach (StructurePart part in parts)
            {
                xe.Add(part.Write());
            }

            return xe;
        }
    }

    public class StructureWeld
    {
        string name;
        string partner;
        List<StructureJoint> joints;
        List<StructurePart> parts;

        StructureWeldCode weldSettings;

        public string Partner
        {
            get => partner;
            set => partner = value;
        }

        public List<StructureJoint> Joints
        {
            get => joints;
            set => joints = value;
        }

        public List<StructurePart> Parts
        {
            get => parts;
            set => parts = value;
        }

        public StructureWeldCode WeldSettings => weldSettings;

        public StructureWeld()
        {
            joints = new List<StructureJoint>();
            parts = new List<StructurePart>();

            weldSettings = new StructureWeldCode();
        }

        public StructureWeld(XmlNode node)
            : this()
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "name":
                        name = attribute.Value;
                        break;

                    case "partner":
                        partner = attribute.Value;
                        break;

                    default:
                        throw new NotImplementedException("Unknown Weld attribute: " + attribute.Name);
                }
            }

            foreach (XmlNode data in node.ChildNodes)
            {
                switch (data.NodeType)
                {
                    case XmlNodeType.CDATA:
                        weldSettings = StructureWeldCode.Parse(data.InnerText);
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            case "JOINT":
                                joints.Add(new StructureJoint(data));
                                break;

                            case "PART":
                                parts.Add(new StructurePart(data));
                                break;

                            default:
                                throw new NotImplementedException("Unknown Element of WELD: " + data.Name);
                        }
                        break;
                }
            }
        }

        public XElement Write()
        {
            XElement xe = new XElement("WELD");
            if (name != null) { xe.Add(new XAttribute("name", name)); }
            if (partner != null) { xe.Add(new XAttribute("partner", partner)); }

            string weld = weldSettings.ToString();
            if (weld.Trim() != "") { xe.Add(new XCData(weld)); }

            foreach (StructureJoint joint in joints)
            {
                xe.Add(joint.Write());
            }

            foreach (StructurePart part in parts)
            {
                xe.Add(part.Write());
            }

            return xe;
        }
    }

    public class StructureJoint
    {
        StructureJointCode jointSettings;

        public StructureJointCode JointSettings
        {
            get => jointSettings;
            set => jointSettings = value;
        }

        public StructureJoint()
        {
            jointSettings = new StructureJointCode();
        }

        public StructureJoint(XmlNode node)
            : this()
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    default:
                        throw new NotImplementedException("Unknown WeldJoint attribute: " + attribute.Name);
                }
            }

            foreach (XmlNode data in node.ChildNodes)
            {
                switch (data.NodeType)
                {
                    case XmlNodeType.CDATA:
                        jointSettings = StructureJointCode.Parse(data.InnerText);
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            default:
                                throw new NotImplementedException("Unknown Element of JOINT: " + data.Name);
                        }
                }
            }
        }

        public XElement Write()
        {
            XElement xe = new XElement("JOINT");

            string jointCDATA = jointSettings.ToString();
            if (jointCDATA.Trim() != "") { xe.Add(new XCData(jointCDATA)); }

            return xe;
        }
    }

    public class StructureWeldCode : LUACodeBlock
    {
        public StructureWeldCode()
        {
            blockPrefix = "CWeldParameters";

            AddMethod(
                LUACodeBlockMethodType.Set,
                "VertexColour",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "R", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "G", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "B", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "A", Value = 0 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Weakness",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = -3 }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AbsoluteLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "PartSpaceVertex",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Break",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "ChanceOfFailure",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "GangedBreak",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );
        }

        public static StructureWeldCode Parse(string cdata)
        {
            return Parse<StructureWeldCode>(cdata);
        }
    }

    public class StructureJointCode : LUACodeBlock
    {
        public StructureJointCode()
        {
            blockPrefix = "CWeldJointParameters";

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Hinge",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "JointAxis",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            // TODO: Find out what the parameters are for Add_FlapSpring
            AddMethod(
                LUACodeBlockMethodType.Add,
                "FlapSpring",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Weakness",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "JointLocation",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "BallJoint",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "JointNormal",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "MinLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "MinLimit2",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxLimit2",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "MinTwistLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxTwistLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "TorqueWeakness",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );
        }

        public static StructureJointCode Parse(string cdata)
        {
            return Parse<StructureJointCode>(cdata);
        }
    }

    public class StructureDamageCode : LUACodeBlock
    {
        public StructureDamageCode()
        {
            blockPrefix = "CDamageParameters";

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Crushability",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Stiffness",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.3f }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "DriverBoxVertexColour",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "R", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "G", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "B", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "A", Value = 0 }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "VehicleSimpleWeapon",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Resiliance",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                new string[] {
                    "PreIK_StrutWishboneMountFL",
                    "PreIK_StrutWishboneMountFR",
                    "PreIK_StrutWishboneMountRL",
                    "PreIK_StrutWishboneMountRR",
                    "PreIK_StrutUpperMountFL",
                    "PreIK_StrutUpperMountFR",
                    "PreIK_StrutUpperMountRL",
                    "PreIK_StrutUpperMountRR",
                    "PreIK_WishboneMountLowerFL",
                    "PreIK_WishboneMountLowerFR",
                    "PreIK_WishboneMountLowerRL",
                    "PreIK_WishboneMountLowerRR",
                    "PreIK_WishboneMountUpperFL",
                    "PreIK_WishboneMountUpperFR",
                    "PreIK_WishboneMountUpperRL",
                    "PreIK_WishboneMountUpperRR"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X", PrettyName = "Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y", PrettyName = "Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z", PrettyName = "Pivot Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                new string[] {
                    "PreIK_StrutWishbone",
                    "PreIK_WishboneLower",
                    "PreIK_WishboneUpper"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "InboardX", PrettyName = "Inboard Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "InboardY", PrettyName = "Inboard Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "InboardZ", PrettyName = "Inboard Pivot Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "OutboardX", PrettyName = "Outboard Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "OutboardY", PrettyName = "Outboard Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "OutboardZ", PrettyName = "Outboard Pivot Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "PhysicsProperty",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                new string[] {
                    "PreIK_StrutHub",
                    "PreIK_WishboneHub"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "UpperX", PrettyName = "Upper Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "UpperY", PrettyName = "Upper Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "UpperZ", PrettyName = "Upper Pivot Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LowerX", PrettyName = "Lower Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LowerY", PrettyName = "Lower Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LowerZ", PrettyName = "Lower Pivot Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PositionX", PrettyName = "Wheel Position X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PositionY", PrettyName = "Wheel Position Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PositionZ", PrettyName = "Wheel Position Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "ShapeType",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Shape" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Restitution",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PreIK_StrutHub",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "UpperX", PrettyName = "Upper Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "UpperY", PrettyName = "Upper Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "UpperZ", PrettyName = "Upper Pivot Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LowerX", PrettyName = "Lower Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LowerY", PrettyName = "Lower Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LowerZ", PrettyName = "Lower Pivot Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PositionX", PrettyName = "Wheel Position X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PositionY", PrettyName = "Wheel Position Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PositionZ", PrettyName = "Wheel Position Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PostIK_SnapPointToPointOnOtherPart",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThisX", PrettyName = "This PartSpace X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThisY", PrettyName = "This PartSpace Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThisZ", PrettyName = "This PartSpace Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Partner" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatX", PrettyName = "That PartSpace X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatY", PrettyName = "That PartSpace Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatZ", PrettyName = "That PartSpace Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                new string[] {
                    "PostIK_RotatePointToPointOnOtherPart",
                    "PostIK_RotatePointToPointOnOtherPartWithScaling"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RotateX", PrettyName = "Rotation Axis X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RotateY", PrettyName = "Rotation Axis Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RotateZ", PrettyName = "Rotation Axis Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Partner" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatX", PrettyName = "That PartSpace X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatY", PrettyName = "That PartSpace Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatZ", PrettyName = "That PartSpace Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                new string[] {
                    "PreIK_LiveAxle",
                    "PreIK_LiveAxle_Hub",
                    "PreIK_LiveAxle_TrailingArmMount"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MountX", PrettyName = "Right Trailing Mount Point X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MountY", PrettyName = "Right Trailing Mount Point Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MountZ", PrettyName = "Right Trailing Mount Point Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                new string[] {
                    "PostIK_RotateInX",
                    "PostIK_RotateInY",
                    "PostIK_RotateInZ",
                    "PostIK_SlideInY"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Variable" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PreIK_LiveAxle_TrailingArm",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MountX", PrettyName = "Mount Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MountY", PrettyName = "Mount Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MountZ", PrettyName = "Mount Pivot Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "AxleX", PrettyName = "Axle Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "AxleY", PrettyName = "Axle Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "AxleZ", PrettyName = "Axle Pivot Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PostIK_RotateVibrateZ",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Variable" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MinFreq", PrettyName = "Min frequency in Hz" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MaxFreq", PrettyName = "Max frequency in Hz" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RandFreq", PrettyName = "Random frequency perturbation (as a fraction of total frequency)" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MinAmp", PrettyName = "Min amplitude in degrees" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MaxAmp", PrettyName = "Max amplitude in degrees" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RandAmp", PrettyName = "Random amplitude perturbation (as a fraction of total amplitude)" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PointX", PrettyName = "Vibrate Pivot X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PointY", PrettyName = "Vibrate Pivot Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PointZ", PrettyName = "Vibrate Pivot Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                new string[] {
                    "PostIK_NamedRotateInX",
                    "PostIK_NamedRotateInY",
                    "PostIK_NamedRotateInZ"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Variable" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Part" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Mass",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "CrushDamageSoundSubCat",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Category" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "FunctionalLight",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Light" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Part" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CrushDamageMaterial",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Material" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Texture" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CrushDamageEmitter",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "EmitterName", PrettyName = "Emitter Name" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PostIK_RotatePointToLineOnOtherPart",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThisX", PrettyName = "This PartSpace X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThisY", PrettyName = "This PartSpace Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThisZ", PrettyName = "This PartSpace Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Partner" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatX", PrettyName = "That PartSpace X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatY", PrettyName = "That PartSpace Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ThatZ", PrettyName = "That PartSpace Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LineX", PrettyName = "Line X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LineY", PrettyName = "Line Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "LineZ", PrettyName = "Line Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PostIK_RockInX",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Variable" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Speed", PrettyName = "Speed Factor" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Amplitude", PrettyName = "Amplitude in degrees" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RockX", PrettyName = "Rock Centre X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RockY", PrettyName = "Rock Centre Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RockZ", PrettyName = "Rock Centre Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                new string[] {
                    "PedWeapon",
                    "VehicleWeapon",
                    "AccessoryWeapon"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Weapon" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Constant" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "DriverEjectionSmash",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "SoundConfigFile",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "File" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                new string[] {
                    "DetachPartEmitter",
                    "DetachParentEmitter"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "EmitterName", PrettyName = "Emitter Name" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", PrettyName = "Snap-force factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AlwaysJointed",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "PostIK_OscillateInZ",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Variable" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "BackFactor", PrettyName = "Back Factor" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ForthFactor", PrettyName = "Forward Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "LumpRenderLevel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Name" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "RenderLevel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "CollisionBoundsMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "ShapeTracking",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "HiddenByCheat",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CaterpillarTrack_DriveWheel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Unknown" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "CaterpillarTrack_RoadWheel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Unknown" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "CaterpillarTrack_DefinesSpeed",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Unknown" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "IndexedPhysicsProperty",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Property" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "GangedWheelSteering",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Wheel" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "GangedWheelRotation",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Wheel" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "GangedWheelSuspension",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Wheel" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "WheelTracksGround",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "CentreOfMass",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "ShapeExcludesOpponents",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "TrailerHitch",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Unknown" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "SteeringProportion",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "TrailerLegs_Down",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "LumpName" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "TrailerLegs_Up",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "LumpName" }
            );
        }

        public static StructureDamageCode Parse(string cdata)
        {
            return Parse<StructureDamageCode>(cdata);
        }
    }

    public class StructureCharacteristicsCode : LUACodeBlock
    {
        public StructureCharacteristicsCode()
        {
            blockPrefix = "CVehicleCharacteristics";

            AddMethod(
                LUACodeBlockMethodType.Set,
                "DefenceGeneral",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "DefenceAgainstCars",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Offence",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "WholeBodyDeformationFactor",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "ValueFactor",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "PermanentPowerup",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilOpenSound",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Sound" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilCloseSound",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Sound" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilSoundLump",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMinSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMaxSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMinParametric",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMovementUpTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMovementDownTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeDropTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilUpSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilDownSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilMovementUpTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilMovementDownTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2UpSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2DownSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2MovementUpTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2MovementDownTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "ExtraFallingDamageThreshold",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "ExtraFallingDamageFactor",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Damage_MaxDeflectionForDamageTexture",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CaterpillarTrack_SegmentDefinition",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Name" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Pitch" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "XOffset" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CaterpillarTrack_SagHeights",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "AmbientLow" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "AbsoluteLowest" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CaterpillarTrack_DamageMode",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "NumberOfParts" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Restitution" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MassPerSegment" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CaterpillarTrack_DamageWobble",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "SpeedForMaxWobble" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "FreqOfMaxWobble" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "AmplitudeOfMaxWobble" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RandomVariance" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Damage_CaterpillarTrack_SoundWhineVolume",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "VolumeAtZero" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "SpeedAtSlopeChange" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "VolumeAtSlopeChange" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "SpeedAtMaxVolume" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MaxVolume" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Damage_CaterpillarTrack_SoundWhinePitch",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PitchAtZero" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MaxSpeed" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "PitchAtMaxSpeed" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "Damage_CaterpillarTrack_SoundClunkVolume",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "VolumeAtZero" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "SpeedAtSlopeChange" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "VolumeAtSlopeChange" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "SpeedAtMaxVolume" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MaxVolume" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "Damage_CaterpillarTrack_SoundHighSpeedClunkVolume",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "VolumeAtZero" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "SpeedAtSlopeChange" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "VolumeAtSlopeChange" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "SpeedAtMaxVolume" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MaxVolume" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "CannotBeSplit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "DownwardCrushingExtraOffence",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MultiplierAtZero" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MaxDownwardSpeedForMultiplier" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "MultiplierAtThatSpeed" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "TowingPowerMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Multiplier" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "TowingTractionMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Multiplier" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "TowingSteerSpeedMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Multiplier" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AllowCOMZOutsideOfWheelbase",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );
        }

        public static StructureCharacteristicsCode Parse(string cdata)
        {
            return Parse<StructureCharacteristicsCode>(cdata);
        }
    }
}