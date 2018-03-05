using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public enum SystemsDamageSystemUnitType
    {
        bodywork,
        engine,
        fl_wheel,
        fr_wheel,
        generator,
        rl_wheel,
        rr_wheel,
        steering,
        transmission,

        rl_wheel_001,
        rr_wheel_001,
        fl_wheel_001,
        fr_wheel_001,
        rl_wheel_002,
        rr_wheel_002,
        fl_wheel_002,
        fr_wheel_002,
        rl_wheel_003,
        rr_wheel_003,
        rl_wheel_004,
        rr_wheel_004,
        left_front_track,
        right_front_track,
        left_rear_track,
        right_rear_track,
        left_track,
        right_track,

        Joint
    }

    public class SystemsDamage
    {
        List<SystemsDamageSystemUnit> systemUnits;

        public List<SystemsDamageSystemUnit> Units
        {
            get => systemUnits;
            set => systemUnits = value;
        }

        public SystemsDamage()
        {
            systemUnits = new List<SystemsDamageSystemUnit>();
        }

        public static SystemsDamage Load(string path)
        {
            SystemsDamage systemsDamage = new SystemsDamage();

            using (XMLParser xml = new XMLParser(path, "STRUCTURE"))
            {
                XmlNode systems = xml.GetNode("SYSTEMS");

                foreach (XmlNode system in systems.ChildNodes)
                {
                    systemsDamage.systemUnits.Add(new SystemsDamageSystemUnit(system));
                }
            }

            return systemsDamage;
        }

        public void Save(string path)
        {
            XDocument xml = new XDocument();

            XElement systems = new XElement("SYSTEMS");
            foreach (SystemsDamageSystemUnit unit in systemUnits)
            {
                systems.Add(unit.Write());
            }

            xml.Add(new XElement("STRUCTURE", systems));

            XMLWriter.Save(xml, Path.Combine(path, "SystemsDamage.xml"));
        }
    }

    public class SystemsDamageSystemUnit
    {
        SystemsDamageSystemUnitType unitType;
        SystemsDamageUnitCode unitSettings;

        public SystemsDamageSystemUnitType UnitType
        {
            get => unitType;
            set => unitType = value;
        }

        public SystemsDamageUnitCode Settings
        {
            get => unitSettings;
            set => unitSettings = value;
        }

        public SystemsDamageSystemUnit() { }

        public SystemsDamageSystemUnit(XmlNode node)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "name":
                        unitType = attribute.Value.ToEnum<SystemsDamageSystemUnitType>();
                        break;

                    default:
                        throw new NotImplementedException("Unknown UNIT attribute: " + attribute.Name);
                }
            }

            foreach (XmlNode data in node.ChildNodes)
            {
                switch (data.NodeType)
                {
                    case XmlNodeType.CDATA:
                        unitSettings = SystemsDamageUnitCode.Parse(data.InnerText);
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            default:
                                throw new NotImplementedException("Unknown element of UNIT: " + data.Name);
                        }
                }
            }
        }

        public XElement Write()
        {
            XElement xe = new XElement("UNIT");
            xe.Add(new XAttribute("name", unitType));

            string unitCDATA = unitSettings.ToString();
            if (unitCDATA.Trim() != "") { xe.Add(new XCData(unitCDATA)); }

            return xe;
        }
    }

    public class SystemsDamageUnitCode : LUACodeBlock
    {
        public SystemsDamageUnitCode()
        {
            blockPrefix = "CUnitParameters";

            // TODO: Find out what these parameters are
            AddMethod(
                LUACodeBlockMethodType.Add,
                "CrushablePart",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Part" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "ComplicatedWheel",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Wheel" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "IndexedComplicatedWheel",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "SolidPart",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Part" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "CaterpillarTrack",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Int, Name = "Index" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "WastedContribution",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Add,
                "WastedLinearContribution",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "C" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "DamageEffect_Drive",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "DamageEffect_Steering",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "DamageEffect_Brakes",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "Trailer",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Boolean, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "AIWastedContribution",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );

            AddMethod(
                LUACodeBlockMethodType.Set,
                "HumanWastedContribution",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Value" }
            );
        }

        public static SystemsDamageUnitCode Parse(string cdata)
        {
            return Parse<SystemsDamageUnitCode>(cdata);
        }
    }
}