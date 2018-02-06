using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToxicRagers.CarmageddonReincarnation.Helpers
{
    public enum LUACodeBlockMethodType
    {
        Add,
        Set
    }

    public enum LUACodeBlockMethodParameterType
    {
        String,
        Float,
        Int,
        Boolean
    }

    public abstract class LUACodeBlock
    {
        protected string blockPrefix = "";
        protected bool underScored = true;
        protected List<LUACodeBlockMethod> methods;

        public bool IsGreaterThan(float x, float y)
        {
            return x > y;
        }

        public string BlockPrefix
        {
            get => blockPrefix;
            set => blockPrefix = value;
        }

        public bool Underscored
        {
            get => underScored;
            set => underScored = value;
        }

        public List<LUACodeBlockMethod> Methods => methods;

        public LUACodeBlock()
        {
            methods = new List<LUACodeBlockMethod>();
        }

        public void AddMethod(LUACodeBlockMethodType methodType, string[] methodNames, params LUACodeBlockMethodParameter[] methodParameters)
        {
            foreach (string methodName in methodNames)
            {
                AddMethod(methodType, methodName, methodParameters.Select(p => p.Clone()).ToArray());
            }
        }

        public void AddMethod(LUACodeBlockMethodType methodType, string methodName, params LUACodeBlockMethodParameter[] methodParameters)
        {
            AddMethod(methodType, methodName, null, null, null, 0, methodParameters);
        }

        public void AddMethod(LUACodeBlockMethodType methodType, string methodName, string dependsOnMethod, string dependsOnParameter, Func<float, float, bool> comparison, float value, params LUACodeBlockMethodParameter[] methodParameters)
        {
            LUACodeBlockMethod method = new LUACodeBlockMethod()
            {
                Type = methodType,
                Name = methodName
            };

            if (comparison != null)
            {
                method.Dependency = () => comparison(Convert.ToSingle(methods.Find(m => m.Name == dependsOnMethod).Parameters.Find(p => p.Name == dependsOnParameter).Value), value);
            }

            foreach (LUACodeBlockMethodParameter parameter in methodParameters)
            {
                if (parameter.Value == null)
                {
                    switch (parameter.Type)
                    {
                        case LUACodeBlockMethodParameterType.Boolean:
                            parameter.Value = false;
                            break;

                        case LUACodeBlockMethodParameterType.Float:
                            parameter.Value = 0.0f;
                            break;

                        case LUACodeBlockMethodParameterType.Int:
                            parameter.Value = 0;
                            break;

                        case LUACodeBlockMethodParameterType.String:
                            break;
                    }
                }

                method.Parameters.Add(parameter);
            }

            methods.Add(method);
        }

        public void SetParametersForMethod(string methodName, params object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i += 2)
            {
                SetParameterForMethod(methodName, parameters[i].ToString(), parameters[i + 1]);
            }
        }

        public void SetParameterForMethod(string methodName, int parameterIndex, object parameterValue)
        {
            SetParameterForMethod(methodName, methods.Find(m => m.Name == methodName).Parameters[parameterIndex].Name, parameterValue);
        }

        public void SetParameterForMethod(string methodName, string parameterName, object parameterValue)
        {
            var match = methods.Select((m, index) => new { Index = index, Method = m }).Where(m => m.Method.Name == methodName).Last();

            if (match != null)
            {
                LUACodeBlockMethodParameter parameter = match.Method.Parameters.Find(p => p.Name == parameterName);

                if (parameter != null)
                {
                    if (match.Method.Type == LUACodeBlockMethodType.Add && parameter.HasBeenSet)
                    {
                        methods.Insert(match.Index + 1, match.Method.Clone(true));
                        SetParameterForMethod(methodName, parameterName, parameterValue);
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

        public static T Parse<T>(string cdata) where T : LUACodeBlock, new()
        {
            T r = new T();

            List<string> lines = cdata.Split('\r', '\n')
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

                string[] c = line.Split(':', '(', ')', ',')
                    .Select(str => str.Trim())
                    .Where(str => str != string.Empty)
                    .ToArray();

                if (c[0] != r.blockPrefix) { throw new ArgumentOutOfRangeException(string.Format("{0} was unexpected, expected {1}", c[0], r.blockPrefix)); }

                for (int j = 2; j < c.Length; j++) { r.SetParameterForMethod(c[1].Substring((r.underScored ? 4 : 3)), j - 2, c[j]); }
            }

            return r;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            List<LUACodeBlockMethod> toWrite = methods.Where(m => m.ShouldWrite == true).ToList();
            int methodCount = toWrite.Count;

            for (int i = 0; i < methodCount; i++)
            {
                LUACodeBlockMethod method = toWrite[i];

                sb.AppendFormat("{0}:{1}{2}{3}( ", blockPrefix, method.Type, (underScored ? "_" : ""), method.Name);

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

    public class LUACodeBlockMethod
    {
        LUACodeBlockMethodType methodType;
        string methodName;
        List<LUACodeBlockMethodParameter> parameters;
        Func<bool> dependency = () => true;

        public LUACodeBlockMethodType Type
        {
            get => methodType;
            set => methodType = value;
        }

        public string Name
        {
            get => methodName;
            set => methodName = value;
        }

        public List<LUACodeBlockMethodParameter> Parameters
        {
            get => parameters;
            set => parameters = value;
        }

        public Func<bool> Dependency
        {
            get => dependency;
            set => dependency = value;
        }

        public bool ShouldWrite
        {
            get
            {
                if (!dependency()) { return false; }
                if (HasBeenSet) { return true; }

                foreach (LUACodeBlockMethodParameter parameter in parameters)
                {
                    if (parameter.HasBeenSet) { return true; }
                }

                return false;
            }
        }

        public bool HasBeenSet => parameters.Any(p => p.HasBeenSet);

        public LUACodeBlockMethod()
        {
            parameters = new List<LUACodeBlockMethodParameter>();
        }

        public LUACodeBlockMethod Clone(bool bResetParameterValues = false)
        {
            LUACodeBlockMethod m = new LUACodeBlockMethod()
            {
                methodType = methodType,
                methodName = methodName
            };

            foreach (LUACodeBlockMethodParameter parameter in parameters)
            {
                m.parameters.Add(parameter.Clone(bResetParameterValues));
            }

            return m;
        }
    }

    public class LUACodeBlockMethodParameter
    {
        private LUACodeBlockMethodParameterType type;
        private string name;
        private string namePretty;
        private string description;
        private object value;
        private bool forceOutput = false;
        private bool explicitlySet = false;

        public LUACodeBlockMethodParameterType Type
        {
            get => type;
            set => type = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string PrettyName
        {
            get => namePretty;
            set => namePretty = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public object Value
        {
            get => value;
            set => this.value = value;
        }

        public bool ForceOutput
        {
            set => forceOutput = value;
        }

        public object FormattedValue
        {
            get
            {
                switch (type)
                {
                    case LUACodeBlockMethodParameterType.String:
                        return "\"" + value + "\"";

                    case LUACodeBlockMethodParameterType.Float:
                        return (Convert.ToSingle(value) < 0 ? value : string.Format(ToxicRagers.Culture, "{0:0.####}", value));

                    case LUACodeBlockMethodParameterType.Boolean:
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
                if (explicitlySet) { return true; }

                switch (type)
                {
                    case LUACodeBlockMethodParameterType.Int:
                        return forceOutput || (value != null && Convert.ToInt32(value) != default(int));

                    case LUACodeBlockMethodParameterType.String:
                        return value != null;

                    case LUACodeBlockMethodParameterType.Float:
                        return forceOutput || (value != null && Convert.ToSingle(value) != default(float));

                    case LUACodeBlockMethodParameterType.Boolean:
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
            explicitlySet = true;

            switch (type)
            {
                case LUACodeBlockMethodParameterType.String:
                    this.value = value.ToString().Replace("\"", "");
                    break;

                case LUACodeBlockMethodParameterType.Boolean:
                    this.value = (value.ToString() == "true");
                    break;

                default:
                    this.value = value;
                    break;
            }
        }

        public LUACodeBlockMethodParameter Clone(bool bClearValue = false)
        {
            LUACodeBlockMethodParameter p = new LUACodeBlockMethodParameter()
            {
                type = type,
                name = name,
                namePretty = namePretty,
                description = description
            };

            if (!bClearValue) { p.value = value; }

            return p;
        }
    }
}