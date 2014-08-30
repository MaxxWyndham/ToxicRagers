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

    public enum StructureCDataCodeMethodType
    {
        Add,
        Set
    }

    public enum StructureCDataCodeMethodParameterType
    {
        String,
        Float,
        Int,
        Boolean
    }

    public class StructureCDataCodeMethodParameter
    {
        StructureCDataCodeMethodParameterType type;
        string name;
        string namePretty;
        string description;
        object value;
        bool bForceOutput = false;

        public StructureCDataCodeMethodParameterType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string PrettyName
        {
            get { return namePretty; }
            set { namePretty = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public bool ForceOutput
        {
            set { bForceOutput = value; }
        }

        public object FormattedValue
        {
            get
            {
                switch (type)
                {
                    case StructureCDataCodeMethodParameterType.String:
                        return "\"" + value + "\"";

                    case StructureCDataCodeMethodParameterType.Float:
                        return (Convert.ToSingle(value) < 0 ? value : string.Format("{0:0.0}", value));

                    case StructureCDataCodeMethodParameterType.Boolean:
                        return ((bool)value == true ? "true" : "false");

                    default:
                        return value;
                }
            }
        }

        public bool HasBeenSet
        {
            get
            {
                switch (type)
                {
                    case StructureCDataCodeMethodParameterType.String:
                        return value != null;

                    case StructureCDataCodeMethodParameterType.Float:
                        return bForceOutput || (value != null && Convert.ToSingle(value) != default(Single));

                    case StructureCDataCodeMethodParameterType.Boolean:
                        return (bool)value;

                    default:
                        return false;
                }
            }
        }

        public T GetValue<T>() where T : class
        {
            return value as T;
        }

        public void SetValue(object value)
        {
            switch (type)
            {
                case StructureCDataCodeMethodParameterType.String:
                    this.value = value.ToString().Replace("\"", "");
                    break;

                case StructureCDataCodeMethodParameterType.Boolean:
                    this.value = (value.ToString() == "true");
                    break;

                default:
                    this.value = value;
                    break;
            }
        }

        public StructureCDataCodeMethodParameter Clone(bool bClearValue = false)
        {
            var p = new StructureCDataCodeMethodParameter();

            p.type = this.type;
            p.name = this.name;
            p.namePretty = this.namePretty;
            p.description = this.description;
            if (!bClearValue) { p.value = this.value; }

            return p;
        }
    }

    public class StructureCDataCodeMethod
    {
        StructureCDataCodeMethodType methodType;
        string methodName;
        List<StructureCDataCodeMethodParameter> parameters;

        public StructureCDataCodeMethodType Type
        {
            get { return methodType; }
            set { methodType = value; }
        }

        public string Name
        {
            get { return methodName; }
            set { methodName = value; }
        }

        public List<StructureCDataCodeMethodParameter> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public bool ShouldWrite
        {
            get
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.HasBeenSet) { return true; }
                }

                return false;
            }
        }

        public StructureCDataCodeMethod()
        {
            parameters = new List<StructureCDataCodeMethodParameter>();
        }

        public StructureCDataCodeMethod Clone(bool bResetParameterValues = false)
        {
            var m = new StructureCDataCodeMethod();

            m.methodType = this.methodType;
            m.methodName = this.methodName;

            foreach (var parameter in this.parameters)
            {
                m.parameters.Add(parameter.Clone(bResetParameterValues));
            }

            return m;
        }
    }

    public abstract class StructureCDataCode
    {
        protected string blockPrefix = "";
        protected List<StructureCDataCodeMethod> methods;

        public string BlockPrefix
        {
            get { return blockPrefix; }
            set { blockPrefix = value; }
        }

        public List<StructureCDataCodeMethod> Methods
        {
            get { return methods; }
        }

        public StructureCDataCode()
        {
            methods = new List<StructureCDataCodeMethod>();
        }

        public void AddMethod(StructureCDataCodeMethodType methodType, string[] methodNames, params StructureCDataCodeMethodParameter[] methodParameters)
        {
            foreach (string methodName in methodNames)
            {
                AddMethod(methodType, methodName, methodParameters);
            }
        }

        public void AddMethod(StructureCDataCodeMethodType methodType, string methodName, params StructureCDataCodeMethodParameter[] methodParameters)
        {
            var method = new StructureCDataCodeMethod();
            method.Type = methodType;
            method.Name = methodName;

            foreach (var parameter in methodParameters)
            {
                method.Parameters.Add(parameter);
            }

            methods.Add(method);
        }

        public void SetParameterForMethod(string methodName, string parameterName, object parameterValue)
        {
            var match = methods.Select((m, index) => new { Index = index, Method = m }).Where(m => m.Method.Name == methodName).Last();

            if (match != null)
            {
                var parameter = match.Method.Parameters.Find(p => p.Name == parameterName);

                if (parameter != null)
                {
                    if (match.Method.Type == StructureCDataCodeMethodType.Add && parameter.HasBeenSet)
                    {
                        methods.Insert(match.Index + 1, match.Method.Clone(true));
                        this.SetParameterForMethod(methodName, parameterName, parameterValue);
                    }
                    else
                    {
                        parameter.SetValue(parameterValue);
                    }
                }
                else
                {
                    throw new ArgumentException(string.Format("{0} is not a parameter of {1}:{2}", parameterName, blockPrefix, methodName));
                }
            }
            else
            {
                throw new ArgumentException(string.Format("{0} is not a method of {1}", methodName, blockPrefix));
            }
        }

        public static T Parse<T>(string cdata) where T : StructureCDataCode, new()
        {
            var r = new T();

            var lines = cdata.Split('\r', '\n').Select(str => str.Trim())
                                               .Where(str => str != string.Empty)
                                               .ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Substring(0, 2) == "--") { continue; }

                var c = line.Split(':', '(', ')').Select(str => str.Trim())
                                                 .Where(str => str != string.Empty)
                                                 .ToArray();

                if (c[0] != r.blockPrefix) { throw new ArgumentOutOfRangeException(string.Format("{0} was unexpected, expected {1}", c[0], r.blockPrefix)); }

                var method = r.methods.Find(m => m.Name == c[1].Substring(4) && m.Type == c[1].Substring(0, 3).ToEnum<StructureCDataCodeMethodType>());

                if (method != null)
                {
                    for (int j = 2; j < c.Length; j++)
                    {
                        method.Parameters[j - 2].SetValue(c[j]);
                    }
                }
                else
                {
                    throw new NotImplementedException(string.Format("Unknown {0} method: {1}", r.blockPrefix, c[1]));
                }
            }

            return r;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var toWrite = methods.Select(m => m).Where(m => m.ShouldWrite == true).ToList();
            int methodCount = toWrite.Count;

            for (int i = 0; i < methodCount; i++)
            {
                var method = toWrite[i];

                sb.AppendFormat("{0}:{1}_{2}( ", blockPrefix, method.Type, method.Name);

                int parameterCount = method.Parameters.Count;

                for (int j = 0; j < parameterCount; j++)
                {
                    sb.Append(method.Parameters[j].FormattedValue);
                    if (j + 1 < parameterCount) { sb.Append(", "); }
                }

                sb.Append(" )");
                if (i + 1 < methodCount) { sb.AppendLine(); }
            }

            return sb.ToString();
        }
    }

    public class StructureWeldCode : StructureCDataCode
    {
        public StructureWeldCode()
        {
            this.blockPrefix = "CWeldParameters";

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "VertexColour",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "R", Value = 0 },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "G", Value = 0 },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "B", Value = 0 },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "A", Value = 0 }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Weakness",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value", Value = -3 }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "AbsoluteLimit",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "PartSpaceVertex",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Break",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "ChanceOfFailure",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Int, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "GangedBreak",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.String, Name = "Name" }
            );
        }

        public static StructureWeldCode Parse(string cdata)
        {
            return Parse<StructureWeldCode>(cdata);
        }
    }

    public class StructureJointCode : StructureCDataCode
    {
        public StructureJointCode()
        {
            this.blockPrefix = "CWeldJointParameters";

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Hinge",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "JointAxis",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "MaxLimit",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            // TODO: Find out what the parameters are for Add_FlapSpring
            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "FlapSpring",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "A" },
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "B" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Weakness",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "JointLocation",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "BallJoint",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "JointNormal",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "MinLimit",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "MaxLimit",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "MinLimit2",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "MaxLimit2",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "MinTwistLimit",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "MaxTwistLimit",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );
        }

        public static StructureJointCode Parse(string cdata)
        {
            return Parse<StructureJointCode>(cdata);
        }
    }

    public class StructureDamageCode : StructureCDataCode
    {
        public StructureDamageCode()
        {
            this.blockPrefix = "CDamageParameters";

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Crushability",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value", ForceOutput = true }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Stiffness",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value", Value = 0.3f }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "DriverBoxVertexColour",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "R", Value = 0 },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "G", Value = 0 },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "B", Value = 0 },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "A", Value = 0 }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "VehicleSimpleWeapon",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Name" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Resiliance",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
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
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X", PrettyName = "Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y", PrettyName = "Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z", PrettyName = "Pivot Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                new string[] {
                    "PreIK_StrutWishbone",
                    "PreIK_WishboneLower",
                    "PreIK_WishboneUpper"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "InboardX", PrettyName = "Inboard Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "InboardY", PrettyName = "Inboard Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "InboardZ", PrettyName = "Inboard Pivot Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "OutboardX", PrettyName = "Outboard Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "OutboardY", PrettyName = "Outboard Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "OutboardZ", PrettyName = "Outboard Pivot Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "PhysicsProperty",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Name" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                new string[] {
                    "PreIK_StrutHub",
                    "PreIK_WishboneHub"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "UpperX", PrettyName = "Upper Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "UpperY", PrettyName = "Upper Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "UpperZ", PrettyName = "Upper Pivot Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LowerX", PrettyName = "Lower Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LowerY", PrettyName = "Lower Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LowerZ", PrettyName = "Lower Pivot Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PositionX", PrettyName = "Wheel Position X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PositionY", PrettyName = "Wheel Position Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PositionZ", PrettyName = "Wheel Position Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "ShapeType",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Shape" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Restitution",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "PreIK_StrutHub",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "PivotAxis", PrettyName = "Pivot Axis" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "UpperX", PrettyName = "Upper Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "UpperY", PrettyName = "Upper Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "UpperZ", PrettyName = "Upper Pivot Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LowerX", PrettyName = "Lower Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LowerY", PrettyName = "Lower Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LowerZ", PrettyName = "Lower Pivot Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PositionX", PrettyName = "Wheel Position X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PositionY", PrettyName = "Wheel Position Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PositionZ", PrettyName = "Wheel Position Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "PostIK_SnapPointToPointOnOtherPart",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThisX", PrettyName = "This PartSpace X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThisY", PrettyName = "This PartSpace Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThisZ", PrettyName = "This PartSpace Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Partner" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatX", PrettyName = "That PartSpace X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatY", PrettyName = "That PartSpace Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatZ", PrettyName = "That PartSpace Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                new string[] {
                    "PostIK_RotatePointToPointOnOtherPart",
                    "PostIK_RotatePointToPointOnOtherPartWithScaling"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RotateX", PrettyName = "Rotation Axis X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RotateY", PrettyName = "Rotation Axis Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RotateZ", PrettyName = "Rotation Axis Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Partner" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatX", PrettyName = "That PartSpace X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatY", PrettyName = "That PartSpace Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatZ", PrettyName = "That PartSpace Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                new string[] {
                    "PreIK_LiveAxle",
                    "PreIK_LiveAxle_Hub",
                    "PreIK_LiveAxle_TrailingArmMount"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MountX", PrettyName = "Right Trailing Mount Point X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MountY", PrettyName = "Right Trailing Mount Point Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MountZ", PrettyName = "Right Trailing Mount Point Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                new string[] {
                    "PostIK_RotateInX",
                    "PostIK_RotateInY",
                    "PostIK_RotateInZ",
                    "PostIK_SlideInY"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Variable" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "PreIK_LiveAxle_TrailingArm",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "WheelIndex", PrettyName = "Wheel Index" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MountX", PrettyName = "Mount Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MountY", PrettyName = "Mount Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MountZ", PrettyName = "Mount Pivot Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "AxleX", PrettyName = "Axle Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "AxleY", PrettyName = "Axle Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "AxleZ", PrettyName = "Axle Pivot Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "PostIK_RotateVibrateZ",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Variable" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MinFreq", PrettyName = "Min frequency in Hz" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MaxFreq", PrettyName = "Max frequency in Hz" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RandFreq", PrettyName = "Random frequency perturbation (as a fraction of total frequency)" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MinAmp", PrettyName = "Min amplitude in degrees" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "MaxAmp", PrettyName = "Max amplitude in degrees" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RandAmp", PrettyName = "Random amplitude perturbation (as a fraction of total amplitude)" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PointX", PrettyName = "Vibrate Pivot X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PointY", PrettyName = "Vibrate Pivot Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "PointZ", PrettyName = "Vibrate Pivot Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                new string[] {
                    "PostIK_NamedRotateInX",
                    "PostIK_NamedRotateInY",
                    "PostIK_NamedRotateInZ"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Variable" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Part" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Mass",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "CrushDamageSoundSubCat",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Category" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "FunctionalLight",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Light" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Part" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "CrushDamageMaterial",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "Level" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Material" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Texture" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "CrushDamageEmitter",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Int, Name = "Level" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "EmitterName", PrettyName = "Emitter Name" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "PostIK_RotatePointToLineOnOtherPart",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThisX", PrettyName = "This PartSpace X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThisY", PrettyName = "This PartSpace Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThisZ", PrettyName = "This PartSpace Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Partner" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatX", PrettyName = "That PartSpace X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatY", PrettyName = "That PartSpace Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ThatZ", PrettyName = "That PartSpace Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LineX", PrettyName = "Line X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LineY", PrettyName = "Line Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "LineZ", PrettyName = "Line Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "PostIK_RockInX",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Variable" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Speed", PrettyName = "Speed Factor" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Amplitude", PrettyName = "Amplitude in degrees" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RockX", PrettyName = "Rock Centre X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RockY", PrettyName = "Rock Centre Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "RockZ", PrettyName = "Rock Centre Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                new string[] {
                    "PedWeapon",
                    "VehicleWeapon",
                    "AccessoryWeapon"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Weapon" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Constant" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "DriverEjectionSmash",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "SoundConfigFile",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "File" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                new string[] {
                    "DetachPartEmitter",
                    "DetachParentEmitter"
                },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "EmitterName", PrettyName = "Emitter Name" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "X" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Y" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Z" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor", PrettyName = "Snap-force factor" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "AlwaysJointed",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Boolean, Name = "Value" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "PostIK_OscillateInZ",
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.String, Name = "Variable" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "BackFactor", PrettyName = "Back Factor" },
                new StructureCDataCodeMethodParameter { Type = StructureCDataCodeMethodParameterType.Float, Name = "ForthFactor", PrettyName = "Forward Factor" }
            );
        }

        public static StructureDamageCode Parse(string cdata)
        {
            return Parse<StructureDamageCode>(cdata);
        }
    }

    public class StructureCharacteristicsCode : StructureCDataCode
    {
        public StructureCharacteristicsCode()
        {
            this.blockPrefix = "CVehicleCharacteristics";

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "DefenceGeneral",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "DefenceAgainstCars",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "Offence",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor", Value = 1.0f }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "WholeBodyDeformationFactor",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Set,
                "ValueFactor",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                StructureCDataCodeMethodType.Add,
                "PermanentPowerup",
                new StructureCDataCodeMethodParameter() { Type = StructureCDataCodeMethodParameterType.String, Name = "Name" }
            );
        }

        public static StructureCharacteristicsCode Parse(string cdata)
        {
            return Parse<StructureCharacteristicsCode>(cdata);
        }
    }
}
