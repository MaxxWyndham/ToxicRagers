using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToxicRagers.CarmageddonReincarnation.Helpers
{
    public enum LUAScriptPropertyType
    {
        String,
        Float,
        Int,
        Boolean,
        Table
    }

    public abstract class LUAScript
    {
        protected string activeProperty;

        List<string> modules = new List<string>();
        List<LUAScriptProperty> properties = new List<LUAScriptProperty>();
        bool isTable = false;

        public List<LUAScriptProperty> Properties => properties;

        public bool IsTable
        {
            get => isTable;
            set => isTable = value;
        }

        public void AddProperty(string propertyName, LUAScriptPropertyType propertyType, bool isArray = false, Type tableType = null)
        {
            properties.Add(new LUAScriptProperty { Name = propertyName, Type = propertyType, Array = isArray, TableType = tableType });
        }

        public void SetProperty(string propertyName, string value)
        {
            LUAScriptProperty property = properties.Where(p => p.Name == propertyName).FirstOrDefault();

            if (property != null)
            {
                property.Value = value;
            }
            else
            {
                throw new ArgumentException($"{propertyName} is an unknown property (value is '{value}')");
            }
        }

        public void SetActiveProperty(string propertyName)
        {
            activeProperty = propertyName;
        }

        protected static T Parse<T>(string script) where T : LUAScript, new()
        {
            Stack<LUAScript> stack = new Stack<LUAScript>();
            LUAScript r = new T();

            stack.Push(r);

            List<string> lines = script.Split('\r', '\n')
                .Select(str => str.Trim())
                .Where(str => str != string.Empty && !str.StartsWith("--"))
                .ToList();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i] == ")" && !lines[i - 1].EndsWith(")"))
                {
                    lines[i - 1] += " )";
                    lines.RemoveAt(i);
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                string[] c = line.Split('=', '(', ')', ',')
                    .Select(str => str.Trim())
                    .Where(str => str != string.Empty)
                    .ToArray();

                if (c[0] == "module")
                {
                    r.modules.AddRange(c.ToList().GetRange(1, c.Length - 1));
                }
                else if (c[0] == "{")
                {
                    r = (LUAScript)Activator.CreateInstance(r.Properties.Where(p => p.Name == r.activeProperty).First().TableType);
                    stack.Push(r);
                }
                else if (c[0] == "}")
                {
                    if (r.activeProperty != null)
                    {
                        r.SetActiveProperty(null);
                    }
                    else
                    {
                        LUAScript child = stack.Pop();
                        r = stack.Peek();
                        r.Properties.Where(p => p.Name == r.activeProperty).First().Value = child;
                    }
                }
                else
                {
                    if (c.Length == 1)
                    {
                        r.SetProperty(r.activeProperty, c[0]);
                    }
                    else
                    {
                        for (int j = 1; j < c.Length; j++)
                        {
                            if (c[j] == "{")
                            {
                                r.SetActiveProperty(c[0]);
                            }
                            else
                            {
                                r.SetProperty(c[0], c[j]);
                            }
                        }
                    }
                }
            }

            return (T)stack.Pop();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<string> props = new List<string>();

            foreach (LUAScriptProperty property in properties)
            {
                if (!property.ShouldWrite) { continue; }

                if (!property.Array)
                {
                    props.Add($"{property.Name} = {property.FormattedValue}");
                }
                else
                {
                    props.Add($"{(property.Name != null ? $"{property.Name} = {{\r\n" : "")}{property.FormattedValue}{(property.Name != null ? $"\r\n}}" : "")}");
                }
            }

            if (modules.Count > 0) { sb.AppendLine($"module({string.Join(",", modules)})"); }
            if (isTable) { sb.AppendLine("{"); }
            sb.AppendLine(string.Join($"{(isTable ? "," : string.Empty)}\r\n", props.Select(e => $"{e}")));
            if (isTable) { sb.Append("}"); }

            return sb.ToString();
        }
    }

    public class LUAScriptProperty
    {
        private string name;
        private LUAScriptPropertyType type;
        private Type tableType;
        private object value = null;
        private bool isArray;
        private bool explicitlySet = false;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public LUAScriptPropertyType Type
        {
            get => type;
            set => type = value;
        }

        public Type TableType
        {
            get => tableType;
            set => tableType = value;
        }

        public bool Array
        {
            get => isArray;
            set => isArray = value;
        }

        public bool ShouldWrite => explicitlySet;

        public object Value
        {
            get => value;
            set
            {
                explicitlySet = true;

                switch (type)
                {
                    case LUAScriptPropertyType.String:
                        value = value.ToString();
                        break;

                    case LUAScriptPropertyType.Boolean:
                        value = Convert.ToBoolean(value);
                        break;

                    case LUAScriptPropertyType.Int:
                        value = Convert.ToInt32(value);
                        break;

                    case LUAScriptPropertyType.Float:
                        value = Convert.ToSingle(value);
                        break;
                }

                if (isArray)
                {
                    if (this.value == null) { this.value = new List<object>(); }
                    (this.value as List<object>).Add(value);
                }
                else
                {
                    this.value = value;
                }
            }
        }

        public string FormattedValue
        {
            get
            {
                if (isArray) { return FormattedValues; }

                switch (type)
                {
                    case LUAScriptPropertyType.Boolean:
                        return value.ToString().ToLower();

                    case LUAScriptPropertyType.Float:
                        return string.Format(ToxicRagers.Culture, "{0:0.####}", value);

                    default:
                        return value.ToString();
                }
            }
        }

        private string FormattedValues => string.Join(",\r\n", (value as List<object>).Select(e => $"{e}"));
    }
}