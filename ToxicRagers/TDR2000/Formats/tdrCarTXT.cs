using ToxicRagers.Carmageddon.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class Car
    {
        private string name;
        private string description;
        private string descriptor;
        private string display_name;
        private string driver_name;
        private string driver_short_name;
        private long strength;
        private long cost;
        private long armour;
        private long power;
        private long offensive;
        private CarTypeEnum type = CarTypeEnum.Undefined;
        private DriverTypeEnum driver_type = DriverTypeEnum.UNDEFINED;
        private string armour_map;

        public enum CarTypeEnum : int
        {
            Undefined = -1,
            Eagle = 0,
            Normal = 1,
            Novelty = 2
        }

        public enum DriverTypeEnum : int
        {
            UNDEFINED = -1,
            MALE_NORMAL = 0,
            FEMALE_NORMAL = 1, 
            MALE_COP = 2, 
            FEMALE_COP = 3, 
            MALE_GANG = 4, 
            FEMALE_GANG = 5
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string Descriptor
        {
            get => descriptor;
            set => descriptor = value;
        }

        public string DisplayName
        {
            get => display_name;
            set => display_name = value;
        }

        public string DriverName
        {
            get => driver_name;
            set => driver_name = value;
        }

        public string DriverShortName
        {
            get => driver_short_name;
            set => driver_short_name = value;
        }

        public long Strength
        {
            get => strength;
            set => strength = value;
        }

        public long Cost
        {
            get => cost;
            set => cost = value;
        }

        public CarTypeEnum Type
        {
            get => type;
            set => type = value;
        }
        public DriverTypeEnum DriverType
        {
            get => driver_type;
            set => driver_type = value;
        }

        public long Armour
        {
            get => armour;
            set => armour = value;
        }

        public long Power
        {
            get => power;
            set => power = value;
        }

        public long Offensive
        {
            get => offensive;
            set => offensive = value;
        }

        public string ArmourMap
        {
            get => armour_map;
            set => armour_map = value;
        }

        public static Car Load(string path)
        {
            DocumentParser parser = new DocumentParser(path);
            Car car = new();

            bool readingDescription = false;
            while (!parser.EOF)
            {
                string currentLine = parser.ReadLine().Trim();

                if (readingDescription)
                {
                    if (currentLine.EndsWith("\""))
                    {
                        readingDescription = false;
                    }
                    car.Description += $"\n{currentLine.Replace("\"", "")}";
                    continue;
                    
                }

                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    continue;
                }

                int firstSpace = currentLine.IndexOf(" ");
                var splitString = currentLine.Split(new[] { ' ', '"' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string type = splitString[0]; // currentLine.Substring(0, firstSpace);
                string value = "";

                bool valueEndsWithQuote = false;
                bool valueIsJustQuote = false;

                if (splitString.Length > 1)
                {
                    valueIsJustQuote = splitString[1].Trim() == "\"";
                    valueEndsWithQuote = splitString[1].Trim().EndsWith("\"");
                    value = splitString[1].Replace("\"", "");// currentLine.Substring(firstSpace + 1).Replace("\"", "");
                }
                else
                {
                    while (!parser.EOF && string.IsNullOrWhiteSpace(value))
                    {
                        value = parser.ReadLine().Trim().Replace("\"", "");
                    }
                }
                switch (type)
                {
                    case "NAME":
                        car.Name = value;
                        break;
                    case "DESCRIPTION":
                        if (valueIsJustQuote || !valueEndsWithQuote)
                        {
                            readingDescription = true;
                        }
                        car.Description = value;
                        break;
                    case "DESCRIPTOR":
                        car.Descriptor = value;
                        break;
                    case "DISPLAY_NAME":
                        car.DisplayName = value;
                        break;
                    case "DRIVER_NAME":
                        car.DriverName = value;
                        break;
                    case "DRIVER_SHORT_NAME":
                        car.DriverShortName = value;
                        break;
                    case "STRENGTH":
                        car.Strength = long.Parse(value);
                        break;
                    case "COST":
                        car.Cost = long.Parse(value);
                        break;
                    case "TYPE":
                        car.Type = (CarTypeEnum)(int.Parse(value));
                        break;
                    case "DRIVER_TYPE":
                        car.DriverType = (DriverTypeEnum)(int.Parse(value));
                        break;
                    case "ARMOUR":
                        car.Armour = long.Parse(value);
                        break;
                    case "POWER":
                        car.Power = long.Parse(value);
                        break;
                    case "OFFENSIVE":
                        car.Offensive = long.Parse(value);
                        break;
                    default:
                        Console.WriteLine($"Unknown type {type} in {path}");
                        break;
                }
            }

            return car;
        }

        public void Save(string path)
        {

            using (DocumentWriter writer = new DocumentWriter(path))
            {

                if (!string.IsNullOrEmpty(Name))
                    writer.WriteLine($"NAME \"{Name}\"");

                if (!string.IsNullOrEmpty(Description))
                    writer.WriteLine($"DESCRIPTION \"{Description}\"");

                if (!string.IsNullOrEmpty(Descriptor))
                    writer.WriteLine($"DESCRIPTOR \"{Descriptor}\"");

                if (!string.IsNullOrEmpty(DisplayName))
                    writer.WriteLine($"DISPLAY_NAME \"{DisplayName}\"");

                if (!string.IsNullOrEmpty(DriverName))
                    writer.WriteLine($"DRIVER_NAME \"{DriverName}\"");
                
                if (!string.IsNullOrEmpty(DriverShortName))
                    writer.WriteLine($"DRIVER_SHORT_NAME \"{DriverShortName}\"");
                
                if(Strength > 0)
                    writer.WriteLine($"STRENGTH {Strength}");

                if(Armour > 0)
                    writer.WriteLine($"ARMOUR {Armour}");
                
                if(Power > 0)
                    writer.WriteLine($"POWER {Power}"); 
                
                if(Offensive > 0)
                    writer.WriteLine($"OFFENSIVE {Offensive}");

                if(Cost > 0)
                    writer.WriteLine($"COST {Cost}", "in thousands!");

                if(Type != CarTypeEnum.Undefined)
                    writer.WriteLine($"TYPE {(int)Type}", "0 = eagle, 1 = normal, 2 = novelty");

                if(DriverType != DriverTypeEnum.UNDEFINED)
                    writer.WriteLine($"DRIVER_TYPE {(int)DriverType}", "MALE_NORMAL = 0, FEMALE_NORMAL = 1, MALE_COP = 2, FEMALE_COP = 3, MALE_GANG = 4, FEMALE_GANG = 5");
            }
        }
    }
}
