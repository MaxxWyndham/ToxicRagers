using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class CarComponent
    {
        public enum CarComponentType : int
        {
            Detachable = 1,
            DeformAndDetach = 3,
            Rotating = 4,
            DeformAndDetach2 = 6,
            Spinner = 7,
            Pumper = 8,
            Corona = 10,
            PivotPoints = 11,
            UVScrollWithWheels = 13,
            SpinWithWheels = 14,
            HideWhenWasted = 15
        }

        public CarComponentType ComponentType { get; set; }

        public List<ComponentData> Data { get; set; } = new();

        public static CarComponent Load(string path)
        {
            Helpers.DocumentParser parser = new(path);
            CarComponent component = new();

            bool readingDescription = false;

            component.ComponentType = (CarComponentType)parser.ReadInt();
            while (!parser.EOF)
            {
                string[] splitLine = parser.ReadLine().Split(new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (splitLine.Length < 2)
                    continue;

                switch (splitLine[0].ToUpper())
                {
                    case "I":
                        component.Data.Add(new ComponentData()
                        {
                            Type = ComponentData.DataType.Int,
                            Value = splitLine[1].ToInt()
                        });
                        break;

                    case "F":
                        component.Data.Add(new ComponentData()
                        {
                            Type = ComponentData.DataType.Float,
                            Value = float.Parse(splitLine[1])
                        });
                        break;

                    case "S":
                        component.Data.Add(new ComponentData()
                        {
                            Type = ComponentData.DataType.String,
                            Value = splitLine[1].Replace("\"", "")

                        });
                        break;
                }
            }

            switch (component.ComponentType)
            {
                case CarComponentType.Detachable:
                    component.Data[0].Name = "AttachedFlag";
                    component.Data[1].Name = "RigidbodyIndex";
                    if (component.Data.Count > 2)
                        component.Data[2].Name = "DCOLForDetached";
                    break;

                case CarComponentType.DeformAndDetach:
                    component.Data[0].Name = "Softness";
                    component.Data[1].Name = "CrushType";
                    component.Data[2].Name = "EaseOfDetachment";
                    component.Data[3].Name = "CurrentDamage";
                    component.Data[4].Name = "DamageLevel1TexturePointer";
                    component.Data[5].Name = "DamageLevel1TexturePointer";
                    component.Data[6].Name = "DamageLevel2Threshhold";
                    component.Data[7].Name = "DamageLevel1Texture";
                    component.Data[8].Name = "DamageLevel2Texture";
                    break;

                case CarComponentType.Rotating:
                    component.Data[0].Name = "Axis";
                    component.Data[1].Name = "Current";
                    component.Data[2].Name = "CurrentDelta";
                    component.Data[3].Name = "MinAlwaysZero";
                    component.Data[4].Name = "Max";
                    break;

                case CarComponentType.DeformAndDetach2:
                    component.Data[0].Name = "Softness";
                    component.Data[1].Name = "CrushType";
                    component.Data[2].Name = "EaseOfDetachment";
                    component.Data[3].Name = "CurrentDamage";
                    component.Data[4].Name = "DamageLevel1TexturePointer";
                    component.Data[5].Name = "DamageLevel1TexturePointer";
                    component.Data[6].Name = "DamageLevel2Threshhold";
                    component.Data[7].Name = "DamageLevel1Texture";
                    component.Data[8].Name = "DamageLevel2Texture";
                    break;

                case CarComponentType.Spinner:
                    component.Data[0].Name = "Axis";
                    component.Data[1].Name = "Current";
                    component.Data[2].Name = "CurrentDeltaSpeed";
                    break;

                case CarComponentType.Pumper:
                    component.Data[0].Name = "Axis";
                    component.Data[1].Name = "Current";
                    component.Data[2].Name = "CurrentDelta";
                    component.Data[3].Name = "MinAlwaysZero";
                    component.Data[4].Name = "Max";
                    break;

                case CarComponentType.Corona:
                    // None
                    break;

                case CarComponentType.PivotPoints:
                    //None
                    break;

                case CarComponentType.UVScrollWithWheels:
                    component.Data[0].Name = "ScrollMultiplier";
                    break;

                case CarComponentType.SpinWithWheels:
                    component.Data[0].Name = "SpinMultiplier";
                    break;

                case CarComponentType.HideWhenWasted:
                    component.Data[0].Name = "DrawNode";
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown component type {(int)component.ComponentType}");
            }

            return component;
        }

        public void Save(string path)
        {
            using (DocumentWriter writer = new(path))
            {
                writer.WriteLine($"{(int)ComponentType}");

                foreach (var entry in Data)
                {
                    writer.WriteLine($"// {entry.Name}");
                    writer.WriteLine($"{entry}");
                }
            }
        }

        public class ComponentData
        {
            public enum DataType
            {
                Int,
                Float,
                String
            }

            public string Name { get; set; }

            public object Value { get; set; }

            public DataType Type { get; set; }

            public override string ToString()
            {
                switch (Type)
                {
                    case DataType.Int:
                        return $"I\t{Value}";

                    case DataType.Float:
                        return $"F\t{Value}";

                    case DataType.String:
                        return $"S\t{Value}";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
