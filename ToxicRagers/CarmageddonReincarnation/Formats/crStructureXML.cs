using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using ToxicRagers.Helpers;
using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Stainless.Formats;

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
            get { return characteristics; }
            set { characteristics = value; }
        }

        public StructurePart Root
        {
            get { return root; }
            set { root = value; }
        }

        public Structure()
        {
            this.characteristics = new StructureCharacteristicsCode();
        }

        public static Structure Load(string path)
        {
            Structure structure = new Structure();

            using (var xml = new XMLParser(path, "STRUCTURE"))
            {
                structure.characteristics = StructureCharacteristicsCode.Parse(xml.GetNode("CHARACTERISTICS").FirstChild.InnerText);
                structure.root = new StructurePart(xml.GetNode("ROOT"));
            }

            return structure;
        }

        public void Save(string path)
        {
            var xml = new XDocument();

            var structure = new XElement("STRUCTURE");
            structure.Add(new XElement("CHARACTERISTICS", new XCData(characteristics.ToString())));
            structure.Add(this.root.Write());
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

        StructureDamageCode damage;

        public bool IsRoot
        {
            get { return bIsRoot; }
            set { bIsRoot = true; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<StructurePart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }

        public List<StructureWeld> Welds
        {
            get { return welds; }
            set { welds = value; }
        }

        public StructureDamageCode DamageSettings
        {
            get { return damage; }
        }

        public StructurePart()
        {
            damage = new StructureDamageCode();

            parts = new List<StructurePart>();
            welds = new List<StructureWeld>();
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
                        this.name = attribute.Value;
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
                        this.damage = StructureDamageCode.Parse(data.InnerText);
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

                            default:
                                throw new NotImplementedException("Unknown Element of PART: " + data.Name);
                        }
                        break;
                }
            }
        }

        public XElement Write()
        {
            var xe = new XElement((this.IsRoot ? "ROOT" : "PART"));
            xe.Add(new XAttribute("name", this.name));

            string damageCDATA = damage.ToString();
            if (damageCDATA.Trim() != "") { xe.Add(new XCData(damageCDATA)); }

            foreach (var weld in welds)
            {
                xe.Add(weld.Write());
            }

            foreach (var part in parts)
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
        List<StructureWeldJoint> joints;
        List<StructurePart> parts;

        StructureWeldCode weldSettings;

        public string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        public List<StructureWeldJoint> Joints
        {
            get { return joints; }
            set { joints = value; }
        }

        public List<StructurePart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }

        public StructureWeldCode WeldSettings
        {
            get { return weldSettings; }
        }

        public StructureWeld()
        {
            joints = new List<StructureWeldJoint>();
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
                        this.name = attribute.Value;
                        break;

                    case "partner":
                        this.partner = attribute.Value;
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
                        this.weldSettings = StructureWeldCode.Parse(data.InnerText);
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            case "JOINT":
                                joints.Add(new StructureWeldJoint(data));
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
            var xe = new XElement("WELD");
            if (this.name != null) { xe.Add(new XAttribute("name", this.name)); }
            if (this.partner != null) { xe.Add(new XAttribute("partner", this.partner)); }

            var weld = weldSettings.ToString();
            if (weld.Trim() != "") { xe.Add(new XCData(weld)); }

            foreach (var joint in joints)
            {
                xe.Add(joint.Write());
            }

            foreach (var part in parts)
            {
                xe.Add(part.Write());
            }

            return xe;
        }
    }

    public class StructureWeldJoint
    {
        StructureJointCode jointSettings;

        public StructureJointCode JointSettings
        {
            get { return jointSettings; }
            set { jointSettings = value; }
        }

        public StructureWeldJoint()
        {
            jointSettings = new StructureJointCode();
        }

        public StructureWeldJoint(XmlNode node)
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
                        this.jointSettings = StructureJointCode.Parse(data.InnerText);
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
            var xe = new XElement("JOINT");

            string jointCDATA = jointSettings.ToString();
            if (jointCDATA.Trim() != "") { xe.Add(new XCData(jointCDATA)); }

            return xe;
        }
    }

    public class StructureWeldCode : LUACodeBlock
    {
        public StructureWeldCode()
        {
            this.blockPrefix = "CWeldParameters";

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "VertexColour",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "R", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "G", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "B", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "A", Value = 0 }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Weakness",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = -3 }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AbsoluteLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "PartSpaceVertex",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Break",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "ChanceOfFailure",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
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
            this.blockPrefix = "CWeldJointParameters";

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Hinge",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "JointAxis",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            // TODO: Find out what the parameters are for Add_FlapSpring
            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "FlapSpring",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Weakness",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "JointLocation",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "BallJoint",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "JointNormal",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "MinLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "MinLimit2",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxLimit2",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "MinTwistLimit",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "MaxTwistLimit",
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
            this.blockPrefix = "CDamageParameters";

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Crushability",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Stiffness",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value", Value = 0.3f }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "DriverBoxVertexColour",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "R", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "G", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "B", Value = 0 },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "A", Value = 0 }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "VehicleSimpleWeapon",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Resiliance",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "PhysicsProperty",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            this.AddMethod(
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

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "ShapeType",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Shape" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Restitution",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
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

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Mass",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "CrushDamageSoundSubCat",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Category" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "FunctionalLight",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Light" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Part" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "CrushDamageMaterial",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Material" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Texture" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "CrushDamageEmitter",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "EmitterName", PrettyName = "Emitter Name" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
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

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "PostIK_RockInX",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Variable" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Speed", PrettyName = "Speed Factor" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Amplitude", PrettyName = "Amplitude in degrees" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RockX", PrettyName = "Rock Centre X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RockY", PrettyName = "Rock Centre Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "RockZ", PrettyName = "Rock Centre Z" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                new string[] {
                    "PedWeapon",
                    "VehicleWeapon",
                    "AccessoryWeapon"
                },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Weapon" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Constant" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "DriverEjectionSmash",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "SoundConfigFile",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "File" }
            );

            this.AddMethod(
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

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AlwaysJointed",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "PostIK_OscillateInZ",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Variable" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "BackFactor", PrettyName = "Back Factor" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "ForthFactor", PrettyName = "Forward Factor" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "LumpRenderLevel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.String, Name = "Name" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "RenderLevel",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Int, Name = "Level" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "CollisionBoundsMultiplier",
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "X" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Y" },
                new LUACodeBlockMethodParameter { Type = LUACodeBlockMethodParameterType.Float, Name = "Z" }
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
            this.blockPrefix = "CVehicleCharacteristics";

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "DefenceGeneral",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "DefenceAgainstCars",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Offence",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "WholeBodyDeformationFactor",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "ValueFactor",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "PermanentPowerup",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilOpenSound",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Sound" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilCloseSound",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Sound" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilSoundLump",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Name" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMinSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMaxSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMinParametric",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMovementUpTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeMovementDownTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AirBrakeDropTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilUpSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilDownSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilMovementUpTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "AerofoilMovementDownTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2UpSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2DownSpeed",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2MovementUpTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "Aerofoil2MovementDownTime",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "ExtraFallingDamageThreshold",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "ExtraFallingDamageFactor",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Value" }
            );
        }

        public static StructureCharacteristicsCode Parse(string cdata)
        {
            return Parse<StructureCharacteristicsCode>(cdata);
        }
    }
}
