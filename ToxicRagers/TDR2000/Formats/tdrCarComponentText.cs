using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class CarComponentText
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

        private CarComponentType componentType;
        private List<ComponentTextData> data = new List<ComponentTextData>();

        public CarComponentType ComponentType
        {
            get => componentType;
            set => componentType = value;
        }

        public List<ComponentTextData> Data
        {
            get => data;
            set => data = value;
        }

        public static CarComponentText Load(string path)
        {

            CarComponentText componentText = new CarComponentText();

            DocumentParser parser = new DocumentParser(path);

            bool readingDescription = false;

            componentText.ComponentType = (CarComponentType)parser.ReadInt();
            while (!parser.EOF)
            {
                string[] splitLine = parser.ReadLine().Split(new char[]{' ','\t'}, 2, StringSplitOptions.RemoveEmptyEntries);
                
                if(splitLine.Length < 2)
                    continue;
                
                switch (splitLine[0].ToUpper())
                {
                    case "I":
                        componentText.Data.Add(new ComponentTextData()
                        {
                            Type = ComponentTextData.DataType.Int,
                            Value = splitLine[1].ToInt()
                        });
                        break;
                    case "F":
                        componentText.Data.Add(new ComponentTextData()
                        {
                            Type = ComponentTextData.DataType.Float,
                            Value = float.Parse(splitLine[1])
                        });

                        break;
                    case "S":
                        componentText.Data.Add(new ComponentTextData()
                        {
                            Type = ComponentTextData.DataType.String,
                            Value = splitLine[1].Replace("\"","")

                        });
                        break;
                }

            }

            switch (componentText.ComponentType)
            {
                case CarComponentType.Detachable:
                    componentText.Data[0].Name = "AttachedFlag";
                    componentText.Data[1].Name = "RigidbodyIndex";
                    if(componentText.Data.Count > 2)
                        componentText.Data[2].Name = "DCOLForDetached";
                    break;
                case CarComponentType.DeformAndDetach:
                    componentText.Data[0].Name = "Softness";
                    componentText.Data[1].Name = "CrushType";
                    componentText.Data[2].Name = "EaseOfDetachment";
                    componentText.Data[3].Name = "CurrentDamage";
                    componentText.Data[4].Name = "DamageLevel1TexturePointer";
                    componentText.Data[5].Name = "DamageLevel1TexturePointer";
                    componentText.Data[6].Name = "DamageLevel2Threshhold";
                    componentText.Data[7].Name = "DamageLevel1Texture";
                    componentText.Data[8].Name = "DamageLevel2Texture";
                    break;
                case CarComponentType.Rotating:
                    componentText.Data[0].Name = "Axis";
                    componentText.Data[1].Name = "Current";
                    componentText.Data[2].Name = "CurrentDelta";
                    componentText.Data[3].Name = "MinAlwaysZero";
                    componentText.Data[4].Name = "Max";
                    break;
                case CarComponentType.DeformAndDetach2:
                    componentText.Data[0].Name = "Softness";
                    componentText.Data[1].Name = "CrushType";
                    componentText.Data[2].Name = "EaseOfDetachment";
                    componentText.Data[3].Name = "CurrentDamage";
                    componentText.Data[4].Name = "DamageLevel1TexturePointer";
                    componentText.Data[5].Name = "DamageLevel1TexturePointer";
                    componentText.Data[6].Name = "DamageLevel2Threshhold";
                    componentText.Data[7].Name = "DamageLevel1Texture";
                    componentText.Data[8].Name = "DamageLevel2Texture";
                    break;
                case CarComponentType.Spinner:
                    componentText.Data[0].Name = "Axis";
                    componentText.Data[1].Name = "Current";
                    componentText.Data[2].Name = "CurrentDeltaSpeed";
                    break;
                case CarComponentType.Pumper:
                    componentText.Data[0].Name = "Axis";
                    componentText.Data[1].Name = "Current";
                    componentText.Data[2].Name = "CurrentDelta";
                    componentText.Data[3].Name = "MinAlwaysZero";
                    componentText.Data[4].Name = "Max";
                    break;
                case CarComponentType.Corona:
                    // None
                    break;
                case CarComponentType.PivotPoints:
                    //None
                    break;
                case CarComponentType.UVScrollWithWheels:
                    componentText.Data[0].Name = "ScrollMultiplier";
                    break;
                case CarComponentType.SpinWithWheels:
                    componentText.Data[0].Name = "SpinMultiplier";
                    break;
                case CarComponentType.HideWhenWasted:
                    componentText.Data[0].Name = "DrawNode";
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown component type {(int)componentText.ComponentType}");
            }

            return componentText;
        }

        public void Save(string path)
        {

            using (DocumentWriter writer = new DocumentWriter(path))
            {
                writer.WriteLine($"{(int)ComponentType}");

                foreach (var entry in data)
                {
                    writer.WriteLine($"// {entry.Name}");
                    writer.WriteLine($"{entry}");
                }
            }
        }

        public class ComponentTextData
        {
            public enum DataType
            {
                Int,
                Float,
                String
            }
            private string name;
            private object value;
            private DataType type;
            
            public string Name
            {
                get => name;
                set => name = value;
            }
            public object Value
            {
                get => value;
                set => this.value = value;
            }

            public DataType Type
            {
                get => type;
                set => type = value;
            }

            public override string ToString()
            {
                switch (type)
                {
                    case DataType.Int:
                        return $"I\t{value}";
                        break;
                    case DataType.Float:
                        return $"F\t{value}";
                        break;
                    case DataType.String:
                        return $"S\t{value}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
