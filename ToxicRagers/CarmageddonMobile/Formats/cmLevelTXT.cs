using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

using DocumentParser = ToxicRagers.CarmageddonReincarnation.Helpers.DocumentParser;

namespace ToxicRagers.CarmageddonMobile.Formats
{
    public class Level
    {
        public Version Version { get; set; }                // [VERSION]
        public string Environment { get; set; }             // [ENVIRONMENT]
        public string EnvironmentMap { get; set; }          // [ENVIRONMENT_MAP]
        public Vector3 ShadowStrength { get; set; }          // [SHADOW_STRENGTH]
        public MapData Map { get; set; }                    // [MAP]
        public string BigMap { get; set; }                  // [BIGMAP]
        public GridData Grid { get; set; }                  // [GRID]
        public Matrix3D MapMatrix { get; set; }             // [MAPMATRIX]
        public Colour Fog { get; set; }                     // [FOG]
        public Colour Lighting { get; set; }                // [LIGHTING]
        public bool? Headlights { get; set; }                // [HEADLIGHTS]
        public string Splash { get; set; }                  // [SPLASH]
        public float? AutoRecoverHeight { get; set; }        // [AUTO_RECOVER_HEIGHT]
        public int? Viz { get; set; }                        // [VIZ]
        public string AIPath { get; set; }                  // [AI_PATH]
        public List<PowerupDef> Powerups { get; set; }      // [POWERUP]
        public List<PedestrianDef> Peds { get; set; }       // [PEDESTRIAN]
        public List<AccessoryDef> Accessories { get; set; } // [ACCESSORY]
        public List<Colour> SmokeColours { get; set; }      // [SMOKE_COLS]
        public List<MatMod> MatMods { get; set; }           // [MAT_MODS]
        public List<CopDef> Cops { get; set; }              // [COP]

        public Level()
        {
            Powerups = new List<PowerupDef>();
            Peds = new List<PedestrianDef>();
            Accessories = new List<AccessoryDef>();
            SmokeColours = new List<Colour>();
            MatMods = new List<MatMod>();
            Cops = new List<CopDef>();
            Version = new Version(2, 0);
        }
        public static Level Load(string path)
        {
            Level level = new Level();

            using (DocumentParser doc = new DocumentParser(path))
            {
                string line;


                do
                {
                    if (!doc.NextLineIsASection())
                    {
                        Console.WriteLine($"Expecting a section header on line {doc.LineNum} but found: {doc.ReadNextLine()}");
                    }
                    line = doc.ReadNextLine();
                    switch (line.ToUpper())
                    {
                        case "[VERSION]":
                            level.Version = Version.Parse(doc.ReadNextLine());
                            break;
                        case "[ENVIRONMENT]":
                            level.Environment = doc.ReadNextLine();
                            break;
                        case "[ENVIRONMENT_MAP]":
                            level.EnvironmentMap = doc.ReadNextLine();
                            break;
                        case "[SHADOW_STRENGTH]":
                            level.ShadowStrength = doc.ReadVector3();
                            break;
                        case "[MAP]":
                            level.Map = MapData.Load(doc);
                            break;
                        case "[BIGMAP]":
                            level.BigMap = doc.ReadNextLine();
                            break;
                        case "[GRID]":
                            level.Grid = GridData.Load(doc);
                            break;
                        case "[MAPMATRIX]":
                            Vector3 xAxis = doc.ReadVector3();
                            Vector3 yAxis = doc.ReadVector3();
                            Vector3 zAxis = doc.ReadVector3();
                            Vector3 pos = doc.ReadVector3();

                            level.MapMatrix = new Matrix3D(xAxis, yAxis, zAxis, pos);
                            break;
                        case "[FOG]":
                            level.Fog = doc.ReadColour();
                            break;
                        case "[LIGHTING]":
                            level.Lighting = doc.ReadColour();
                            break;
                        case "[HEADLIGHTS]":
                            level.Headlights = doc.ReadInt() == 1;
                            break;
                        case "[SPLASH]":
                            level.Splash = doc.ReadNextLine();
                            break;
                        case "[VIZ]":
                            level.Viz = doc.ReadInt();
                            break;
                        case "[AUTO_RECOVER_HEIGHT]":
                            level.AutoRecoverHeight = doc.ReadFloat();
                            break;
                        case "[AI_PATH]":
                            level.AIPath = doc.ReadNextLine();
                            break;
                        case "[POWERUP]":
                            level.Powerups.Add(PowerupDef.Load(doc));
                            break;
                        case "[PEDESTRIAN]":
                            level.Peds.Add(PedestrianDef.Load(doc));
                            break;
                        case "[ACCESSORY]":
                            level.Accessories.Add(AccessoryDef.Load(doc));
                            break;
                        case "[SMOKE_COLS]":
                            int numSmokeColours = doc.ReadInt();
                            for (int i = 0; i < numSmokeColours; i++)
                            {
                                level.SmokeColours.Add(doc.ReadColour());
                            }

                            break;
                        case "[MATMODS]":
                            int numMatMods = doc.ReadInt();
                            for (int i = 0; i < numMatMods; i++)
                            {
                                level.MatMods.Add(MatMod.Load(doc));
                            }

                            break;
                        case "[COP]":
                            level.Cops.Add(CopDef.Load(doc));
                            break;
                        default:
                            Console.WriteLine($"Unknown section header: {line}");
                            break;
                    }
                } while (line != null && !doc.EOF());
            }

            return level;
        }

        public bool Save(string path)
        {
            using (DocumentWriter dw = new DocumentWriter(path))
            {
                if (Version != null)
                    dw.WriteSection("VERSION", $"{Version.Major}.{Version.Minor}");

                if (!string.IsNullOrWhiteSpace(Environment))
                    dw.WriteSection("ENVIRONMENT", $"{Environment}");

                if (!string.IsNullOrWhiteSpace(EnvironmentMap))
                    dw.WriteSection("ENVIRONMENT_MAP", $"{EnvironmentMap}");

                if (ShadowStrength != null)
                    dw.WriteSection("SHADOW_STRENGTH", $"{ShadowStrength.X:F6},{ShadowStrength.Y:F6},{ShadowStrength.Z:F6}");

                if (Map != null)
                    dw.WriteSection("MAP", $"{Map}");

                if (!string.IsNullOrWhiteSpace(BigMap))
                    dw.WriteSection("BIGMAP", $"{BigMap}");

                if (MapMatrix != null)
                    dw.WriteSection("MAPMATRIX", $"{MapMatrix.M11:F6},{MapMatrix.M12:F6},{MapMatrix.M13:F6}\r\n{MapMatrix.M21:F6},{MapMatrix.M22:F6},{MapMatrix.M23:F6}\r\n{MapMatrix.M31:F6},{MapMatrix.M32:F6},{MapMatrix.M33:F6}\r\n{MapMatrix.M41:F6},{MapMatrix.M42:F6},{MapMatrix.M43:F6}");

                if (Grid != null)
                    dw.WriteSection("GRID", $"{Grid}");

                if (Fog != null)
                    dw.WriteSection("FOG", $"{(int)(Fog.A * 255)},{(int)(Fog.G * 255)},{(int)(Fog.B * 255)}");

                if (Lighting != null)
                    dw.WriteSection("LIGHTING", $"{(int)(Lighting.R * 255)},{(int)(Lighting.G * 255)},{(int)(Lighting.B * 255)}");

                dw.WriteSection("HEADLIGHTS", Headlights.HasValue && Headlights.Value ? "1" : "0");

                if (Splash != null)
                    dw.WriteSection("SPLASH", $"{Splash}");

                if (AutoRecoverHeight.HasValue)
                    dw.WriteSection("AUTO_RECOVER_HEIGHT", $"{AutoRecoverHeight:F6}");

                if (Viz.HasValue)
                    dw.WriteSection("VIZ", $"{Viz}");

                if (!string.IsNullOrWhiteSpace(AIPath))
                    dw.WriteSection("AI_PATH", $"{AIPath}");

                if (SmokeColours != null && SmokeColours.Count > 0)
                    dw.WriteSection("SMOKE_COLS", $"{SmokeColours.Count}\r\n{string.Join("\r\n", SmokeColours.Select(s => $"{(int)(s.R * 255)},{(int)(s.G * 255)},{(int)(s.B * 255)},{(int)(s.A * 255)}"))}");

                if (MatMods != null && MatMods.Count > 0)
                    dw.WriteSection("MATMODS", $"{MatMods.Count}\r\n{string.Join("\r\n\r\n", MatMods)}");

                foreach (var item in Accessories)
                {
                    dw.WriteSection("ACCESSORY", $"{item}");
                }

                foreach (var item in Peds)
                {
                    dw.WriteSection("PEDESTRIAN", $"{item}");
                }

                foreach (var item in Powerups)
                {
                    dw.WriteSection("POWERUP", $"{item}");
                }

                foreach (var item in Cops)
                {
                    dw.WriteSection("COP", $"{item}");
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            Level other = (Level)obj;

            if (other == null || MatMods.Count != other.MatMods.Count || Accessories.Count != other.Accessories.Count ||
                Peds.Count != other.Peds.Count || Cops.Count != other.Cops.Count)
            {
                Console.WriteLine("Other is null or counts don't match up");
                return false;
            }

            for (int i = 0; i < MatMods.Count; i++)
            {
                if (MatMods[i].Equals(other.MatMods[i]))
                {
                    Console.WriteLine($"MatMod[{i}] doesn't match up");
                    return false;
                }
            }

            for (int i = 0; i < Accessories.Count; i++)
            {
                if (Accessories[i].Equals(other.Accessories[i]))
                {
                    Console.WriteLine($"Accessories[{i}] doesn't match up");
                    return false;
                }
            }

            for (int i = 0; i < Peds.Count; i++)
            {
                if (Peds[i].Equals(other.Peds[i]))
                {
                    Console.WriteLine($"Peds[{i}] doesn't match up");
                    return false;
                }
            }

            for (int i = 0; i < Powerups.Count; i++)
            {
                if (Powerups[i].Equals(other.Powerups[i]))
                {
                    Console.WriteLine($"Powerups[{i}] doesn't match up");
                    return false;
                }
            }

            for (int i = 0; i < Cops.Count; i++)
            {
                if (Cops[i].Equals(other.Cops[i]))
                {
                    Console.WriteLine($"Cops[{i}] doesn't match up");
                    return false;
                }
            }

            bool same = true;

            same &= Version?.Equals(other.Version) ?? (Version == null && other.Version == null);
            same &= Environment?.Equals(other.Environment) ?? (Environment == null && other.Environment == null);
            same &= EnvironmentMap?.Equals(other.EnvironmentMap) ?? (EnvironmentMap == null && other.EnvironmentMap == null);
            same &= ShadowStrength?.Equals(other.ShadowStrength) ?? (ShadowStrength == null && other.ShadowStrength == null);
            same &= Map?.Equals(other.Map) ?? (Map == null && other.Map == null);
            same &= BigMap?.Equals(other.BigMap) ?? (BigMap == null && other.BigMap == null);
            same &= MapMatrix?.Equals(other.MapMatrix) ?? (MapMatrix == null && other.MapMatrix == null);
            same &= Grid?.Equals(other.Grid) ?? (Grid == null && other.Grid == null);
            same &= Fog?.Equals(other.Fog) ?? (Fog == null && other.Fog == null);
            same &= Lighting?.Equals(other.Lighting) ?? (Lighting == null && other.Lighting == null);
            same &= Headlights?.Equals(other.Headlights) ?? (Headlights == null && other.Headlights == null);
            same &= Splash?.Equals(other.Splash) ?? (Splash == null && other.Splash == null);
            same &= AutoRecoverHeight?.Equals(other.AutoRecoverHeight) ?? (AutoRecoverHeight == null && other.AutoRecoverHeight == null);
            same &= Viz?.Equals(other.Viz) ?? (Viz == null && other.Viz == null);
            same &= AIPath?.Equals(other.AIPath) ?? (AIPath == null && other.AIPath == null);
            same &= SmokeColours?.Equals(other.SmokeColours) ?? (SmokeColours == null && other.SmokeColours == null);

            return same;
        }
    }

    public class MapData
    {
        public string Name { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Center { get; set; }

        public static MapData Load(DocumentParser doc)
        {
            MapData mapData = new MapData();
            mapData.Name = doc.ReadNextLine();
            mapData.Size = Vector2.Parse(doc.ReadNextLine());
            mapData.Center = Vector2.Parse(doc.ReadNextLine());

            return mapData;
        }

        public override string ToString()
        {
            return $"{Name}\r\n{Size.X:F6},{Size.Y:F6}\r\n{Center.X:F6},{Center.Y:F6}";
        }

        public override bool Equals(object obj)
        {
            MapData other = (MapData)obj;

            if (other == null)
                return false;

            return Name == other.Name && Size.Equals(other.Size) && Center.Equals(other.Center);
        }
    }

    public class GridData
    {
        public Vector3 Position { get; set; }
        public float Orientation { get; set; }

        public static GridData Load(DocumentParser doc)
        {
            GridData gridData = new GridData();
            gridData.Position = doc.ReadVector3();
            gridData.Orientation = doc.ReadFloat();

            return gridData;
        }

        public override string ToString()
        {
            return $"{Position.X:F6},{Position.Y:F6},{Position.Z:F6}\n{Orientation:F6}";
        }

        public override bool Equals(object obj)
        {
            GridData other = (GridData)obj;

            if (other == null)
                return false;

            return Position.Equals(other.Position) && Orientation.Equals(other.Orientation);
        }
    }

    public class PowerupDef
    {
        public int PowerupCount { get; set; }
        public List<string> PowerupNames { get; set; }
        public Matrix3D Transform { get; set; }
        public float RespawnTime { get; set; }

        public PowerupDef()
        {
            PowerupNames = new List<string>();
        }
        public static PowerupDef Load(DocumentParser doc)
        {
            PowerupDef powerup = new PowerupDef();

            string nextLine = "";
            while (!doc.EOF() && !doc.NextLineIsASection())
            {
                nextLine = doc.ReadNextLine();
                if (nextLine.ToUpper() == "<TYPES>")
                {
                    powerup.PowerupCount = doc.ReadInt();
                    for (int i = 0; i < powerup.PowerupCount; i++)
                    {
                        powerup.PowerupNames.Add(doc.ReadNextLine());
                    }
                }
                else if (nextLine.ToUpper() == "<RESPAWN>")
                {
                    powerup.RespawnTime = doc.ReadFloat();
                }
                else
                {
                    doc.Rewind();

                    Vector3 xAxis = doc.ReadVector3();
                    Vector3 yAxis = doc.ReadVector3();
                    Vector3 zAxis = doc.ReadVector3();
                    Vector3 pos = doc.ReadVector3();

                    powerup.Transform = new Matrix3D(xAxis, yAxis, zAxis, pos);
                }
            }

            return powerup;
        }

        public override string ToString()
        {
            return $"<types>\r\n{PowerupNames.Count}\r\n{string.Join("\r\n", PowerupNames)}\r\n\r\n" +
                   $"{Transform.M11:F6},{Transform.M12:F6},{Transform.M13:F6}\r\n{Transform.M21:F6},{Transform.M22:F6},{Transform.M23:F6}\r\n{Transform.M31:F6},{Transform.M32:F6},{Transform.M33:F6}\r\n{Transform.M41:F6},{Transform.M42:F6},{Transform.M43:F6}\r\n\r\n" +
                   $"<respawn>\r\n{RespawnTime:F6}";
        }

        public override bool Equals(object obj)
        {
            PowerupDef other = (PowerupDef)obj;

            if (other == null)
                return false;

            if (PowerupNames.Count != other.PowerupNames.Count)
                return false;

            for (int i = 0; i < PowerupNames.Count; i++)
            {
                if (PowerupNames[i] != other.PowerupNames[i])
                    return false;
            }

            return PowerupCount == other.PowerupCount && Transform.Equals(other.Transform) && RespawnTime == other.RespawnTime;
        }
    }

    public class PedestrianDef
    {
        public string PedType { get; set; }
        public int PathCount { get; set; }
        public List<Vector3> Paths { get; set; }
        public int LoopCount { get; set; }

        public PedestrianDef()
        {
            Paths = new List<Vector3>();
        }
        public static PedestrianDef Load(DocumentParser doc)
        {
            PedestrianDef ped = new PedestrianDef();
            string nextLine = "";
            while (!doc.EOF() && !doc.NextLineIsASection())
            {
                nextLine = doc.ReadNextLine();
                if (nextLine.ToUpper() == "<TYPE>")
                {
                    ped.PedType = doc.ReadNextLine();
                }
                else if (nextLine.ToUpper() == "<PATH>")
                {
                    ped.PathCount = doc.ReadInt();
                    for (int i = 0; i < ped.PathCount; i++)
                    {
                        ped.Paths.Add(doc.ReadVector3());
                    }

                    if (!doc.EOF() && !doc.NextLineIsASection())
                    {
                        ped.LoopCount = doc.ReadInt();
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown pedestrian sub-section: {nextLine}");
                }
            }

            return ped;
        }
        public override string ToString()
        {
            return $"<TYPE>\r\n{PedType}\r\n\r\n" +
                   $"<PATH>\r\n{Paths.Count}\r\n{string.Join("\r\n", Paths.Select(p => $"{p.X:F6},{p.Y:F6},{p.Z:F6}"))}\r\n{LoopCount}\r\n";

        }

        public override bool Equals(object obj)
        {
            PedestrianDef other = (PedestrianDef)obj;

            if (other == null || Paths.Count != other.Paths.Count)
                return false;

            for (int i = 0; i < Paths.Count; i++)
            {
                if (Paths[i].Equals(other.Paths[i]) == false)
                    return false;
            }


            return PedType != other.PedType && LoopCount != other.LoopCount;
        }
    }

    public class AccessoryDef
    {
        public string Name { get; set; }            // <NAME>
        public Matrix3D Transform { get; set; }     // <POSITION>
        public string InstanceName { get; set; }    // <INSTANCE_NAME>
        public Colour InstanceColour { get; set; }  // <INSTANCE_COLOUR>

        public static AccessoryDef Load(DocumentParser doc)
        {
            AccessoryDef accessory = new AccessoryDef();
            string nextLine = "";
            while (!doc.EOF() && !doc.NextLineIsASection())
            {
                nextLine = doc.ReadNextLine();
                switch (nextLine.ToUpper())
                {
                    case "<NAME>":
                        accessory.Name = doc.ReadNextLine();
                        break;
                    case "<POSITION>":
                        Vector3 xAxis = doc.ReadVector3();
                        Vector3 yAxis = doc.ReadVector3();
                        Vector3 zAxis = doc.ReadVector3();
                        Vector3 pos = doc.ReadVector3();

                        accessory.Transform = new Matrix3D(xAxis, yAxis, zAxis, pos);
                        break;
                    case "<INSTANCE_NAME>":
                        accessory.InstanceName = doc.ReadNextLine();
                        break;
                    case "<INSTANCE_COLOUR>":
                        accessory.InstanceColour = doc.ReadColour();
                        break;
                    default:
                        Console.WriteLine($"Unknown accessory sub-section: {nextLine}");
                        break;
                }
            }

            return accessory;
        }

        public override string ToString()
        {
            string output = $"<NAME>\r\n{Name}\r\n\r\n" +
                            $"<POSITION>\r\n{Transform.M11:F6},{Transform.M12:F6},{Transform.M13:F6}\r\n{Transform.M21:F6},{Transform.M22:F6},{Transform.M23:F6}\r\n{Transform.M31:F6},{Transform.M32:F6},{Transform.M33:F6}\r\n{Transform.M41:F6},{Transform.M42:F6},{Transform.M43:F6}\r\n\r\n";
            if (InstanceName != null)
                output += $"<INSTANCE_NAME>\r\n{InstanceName}\r\n";

            if (InstanceColour != null)
                output += $"<INSTANCE_COLOUR>\r\n{(int)(InstanceColour.R * 255)},{(int)(InstanceColour.G * 255)},{(int)(InstanceColour.B * 255)}";

            return output;
        }

        public override bool Equals(object obj)
        {
            AccessoryDef other = (AccessoryDef)obj;

            if (other == null)
                return false;

            if (InstanceColour == null && other.InstanceColour != null ||
                InstanceColour != null && other.InstanceColour == null)
                return false;

            if (Transform == null && other.Transform != null ||
                Transform != null && other.Transform == null)
                return false;

            bool same = Name == other.Name;

            if (InstanceColour != null && other.InstanceColour != null)
                same &= InstanceColour.Equals(other.InstanceColour);

            if (Transform != null && other.Transform != null)
                same &= Transform.Equals(other.Transform);

            return same && InstanceName == other.InstanceName;
        }
    }

    public class CopDef
    {
        public enum CopType
        {
            NORM,   // Standard Cop
            APC     // Suppressor
        }
        public CopType Type { get; set; }
        public Vector3 Position { get; set; }

        public static CopDef Load(DocumentParser doc)
        {
            CopDef copDef = new CopDef();
            string nextLine = "";
            while (!doc.EOF() && !doc.NextLineIsASection())
            {
                nextLine = doc.ReadNextLine();
                switch (nextLine.ToUpper())
                {
                    case "<TYPE>":
                        copDef.Type = (CopType)Enum.Parse(typeof(CopType), doc.ReadNextLine().ToUpper());
                        break;
                    case "<POS>":
                        copDef.Position = doc.ReadVector3();
                        break;
                    default:
                        Console.WriteLine($"Unknown cop sub-section: {nextLine}");
                        break;
                }
            }

            return copDef;
        }

        public override string ToString()
        {
            return $"<TYPE>\r\n{Type}\r\n\r\n" +
                   $"<POS>\r\n{Position.X:F6},{Position.Y:F6},{Position.Z:F6}\r\n";
        }

        public override bool Equals(object obj)
        {
            CopDef other = (CopDef)obj;

            if (other == null)
                return false;

            return Type == other.Type && Position.Equals(other.Position);
        }
    }

    public class MatMod
    {
        public float WallFriction { get; set; }
        public float TyreFriction { get; set; }
        public float DownForce { get; set; }
        public float Bumpiness { get; set; }
        public int TyreSound { get; set; }
        public int CrashSound { get; set; }
        public int ScrapeSound { get; set; }
        public float Sparks { get; set; }
        public float SmokeType { get; set; }

        public static MatMod Load(DocumentParser doc)
        {
            MatMod matMod = new MatMod();
            matMod.WallFriction = doc.ReadFloat();
            matMod.TyreFriction = doc.ReadFloat();
            matMod.DownForce = doc.ReadFloat();
            matMod.Bumpiness = doc.ReadFloat();
            matMod.TyreSound = doc.ReadInt();
            matMod.CrashSound = doc.ReadInt();
            matMod.ScrapeSound = doc.ReadInt();
            matMod.Sparks = doc.ReadFloat();
            matMod.SmokeType = doc.ReadFloat();
            return matMod;
        }

        public override string ToString()
        {
            return $"{WallFriction:F6}{new string(' ', 16 - WallFriction.ToString("F6").Length)}// wall friction\r\n" +
                   $"{TyreFriction:F6}{new string(' ', 16 - TyreFriction.ToString("F6").Length)}// tyre friction\r\n" +
                   $"{DownForce:F6}{new string(' ', 16 - DownForce.ToString("F6").Length)}// down force \r\n" +
                   $"{Bumpiness:F6}{new string(' ', 16 - Bumpiness.ToString("F6").Length)}// bumpiness\r\n" +
                   $"{TyreSound}{new string(' ', 16 - TyreSound.ToString().Length)}// tyre sound\r\n" +
                   $"{CrashSound}{new string(' ', 16 - CrashSound.ToString().Length)}// crash sound\r\n" +
                   $"{ScrapeSound}{new string(' ', 16 - ScrapeSound.ToString().Length)}// scrape sound\r\n" +
                   $"{Sparks:F6}{new string(' ', 16 - Sparks.ToString("F6").Length)}// sparks\r\n" +
                   $"{SmokeType:F6}{new string(' ', 16 - SmokeType.ToString("F6").Length)}// smoke type";

        }

        public override bool Equals(object obj)
        {
            MatMod other = (MatMod)obj;

            if (other == null)
                return false;
            return WallFriction == other.WallFriction &&
                   TyreFriction == other.TyreFriction &&
                   DownForce == other.DownForce &&
                   Bumpiness == other.Bumpiness &&
                   TyreSound == other.TyreSound &&
                   CrashSound == other.CrashSound &&
                   ScrapeSound == other.ScrapeSound &&
                   Sparks == other.Sparks &&
                   SmokeType == other.SmokeType;
        }
    }
}