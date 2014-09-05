using System;
using System.Collections.Generic;
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
        Plasma_Engine,
        rl_wheel,
        rr_wheel,
        steering,
        transmission
    }

    public class SystemsDamage
    {
        List<SystemsDamageSystemUnit> systemUnits;

        public List<SystemsDamageSystemUnit> Units
        {
            get { return systemUnits; }
            set { systemUnits = value; }
        }

        public SystemsDamage()
        {
            systemUnits = new List<SystemsDamageSystemUnit>();
        }

        public static SystemsDamage Load(string path)
        {
            SystemsDamage systemsDamage = new SystemsDamage();

            using (var xml = new XMLParser(path, "STRUCTURE"))
            {
                var systems = xml.GetNode("SYSTEMS");

                foreach (XmlNode system in systems.ChildNodes)
                {
                    systemsDamage.systemUnits.Add(new SystemsDamageSystemUnit(system));
                }
            }

            return systemsDamage;
        }

        public void Save(string path)
        {
            var xml = new XDocument();

            var systems = new XElement("SYSTEMS");
            foreach (var unit in systemUnits)
            {
                systems.Add(unit.Write());
            }

            xml.Add(new XElement("STRUCTURE", systems));

            XMLWriter.Save(xml, path + "\\SystemsDamage.xml");
        }
    }

    public class SystemsDamageSystemUnit
    {
        SystemsDamageSystemUnitType unitType;
        SystemsDamageUnitCode unitSettings;

        public SystemsDamageSystemUnitType UnitType
        {
            get { return unitType; }
            set { unitType = value; }
        }

        public SystemsDamageUnitCode Settings
        {
            get { return unitSettings; }
            set { unitSettings = value; }
        }

        public SystemsDamageSystemUnit() { }

        public SystemsDamageSystemUnit(XmlNode node)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "name":
                        this.unitType = attribute.Value.ToEnum<SystemsDamageSystemUnitType>();
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
                        this.unitSettings = SystemsDamageUnitCode.Parse(data.InnerText);
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
            var xe = new XElement("UNIT");
            xe.Add(new XAttribute("name", this.unitType));

            string unitCDATA = unitSettings.ToString();
            if (unitCDATA.Trim() != "") { xe.Add(new XCData(unitCDATA)); }

            return xe;
        }
    }

    public class SystemsDamageUnitCode : LUACodeBlock
    {
        public SystemsDamageUnitCode()
        {
            this.blockPrefix = "CUnitParameters";

            // TODO: Find out what these parameters are
            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "CrushablePart",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Part" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "WastedLinearContribution",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "C" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "SolidPart",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Part" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "B" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "DamageEffect_Drive",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Add,
                "ComplicatedWheel",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.String, Name = "Wheel" },
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "A" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "WastedContribution",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "DamageEffect_Steering",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );

            this.AddMethod(
                LUACodeBlockMethodType.Set,
                "DamageEffect_Brakes",
                new LUACodeBlockMethodParameter() { Type = LUACodeBlockMethodParameterType.Float, Name = "Factor" }
            );
        }

        public static SystemsDamageUnitCode Parse(string cdata)
        {
            return Parse<SystemsDamageUnitCode>(cdata);
        }
    }
}
