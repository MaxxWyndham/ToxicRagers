using System.Text;

using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.Brender.Formats
{
    public class MAT
    {
        public List<MATMaterial> Materials { get; } = new List<MATMaterial>();

        public static MAT Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            MAT mat = new();

            MATMaterial M = new();

            using (BEBinaryReader br = new(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadUInt32() != 0x12 ||
                    br.ReadUInt32() != 0x8 ||
                    br.ReadUInt32() != 0x5 ||
                    br.ReadUInt32() != 0x2)
                {
                    mat = LoadAscii(path);

                    if (mat is null)
                    {
                        Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid MAT file", path);
                        return null;
                    }
                    else
                    {
                        return mat;
                    }
                }

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int tag = (int)br.ReadUInt32();
                    int length = (int)br.ReadUInt32();

                    switch (tag)
                    {
                        case 0x4:   // C1 mat file
                            M = new MATMaterial();

                            M.DiffuseColour[0] = br.ReadByte();     // R
                            M.DiffuseColour[1] = br.ReadByte();     // G
                            M.DiffuseColour[2] = br.ReadByte();     // B
                            M.DiffuseColour[3] = br.ReadByte();     // A
                            M.AmbientLighting = br.ReadSingle();
                            M.DirectionalLighting = br.ReadSingle();
                            M.SpecularLighting = br.ReadSingle();
                            M.SpecularPower = br.ReadSingle();
                            M.Flags = (MATMaterial.Settings)br.ReadUInt16();
                            M.UVMatrix = new Matrix2D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            M.IndexBase = br.ReadByte();
                            M.IndexRange = br.ReadByte();
                            M.Name = br.ReadString();
                            break;

                        case 0x3c:
                            M = new MATMaterial();

                            M.DiffuseColour[0] = br.ReadByte(); // R
                            M.DiffuseColour[1] = br.ReadByte(); // G
                            M.DiffuseColour[2] = br.ReadByte(); // B
                            M.DiffuseColour[3] = br.ReadByte(); // A
                            M.AmbientLighting = br.ReadSingle();
                            M.DirectionalLighting = br.ReadSingle();
                            M.SpecularLighting = br.ReadSingle();
                            M.SpecularPower = br.ReadSingle();
                            M.Flags = (MATMaterial.Settings)br.ReadUInt32();
                            M.UVMatrix = new Matrix2D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            if (br.ReadUInt32() != 169803776) { Console.WriteLine("Weird Beard! (" + path + ")"); }
                            br.ReadBytes(13); // 13 bytes of nothing
                            M.Name = br.ReadString();
                            break;

                        case 0x1c:  // colour_map
                            M.Texture = br.ReadString();
                            break;

                        case 0x1f:  // shadetable
                            M.ShadeTable = br.ReadString(); 
                            break;

                        case 0x0:
                            mat.Materials.Add(M);
                            break;

                        default:
                            Logger.LogToFile(Logger.LogLevel.Error, "Unknown MAT tag: {0} ({1})", tag, br.BaseStream.Position.ToString("X"));
                            return null;
                    }
                }
            }

            return mat;
        }

        public static MAT LoadAscii(string path)
        {
            MATMaterial material = null;
            MAT mat = new();

            DocumentParser file = new DocumentParser(path);

            do
            {
                string line = file.ReadLine();

                if (line.StartsWith("#")) { continue; }
                if (line.Contains('#')) { line = line[..line.IndexOf("#")]; }

                string[] parts = line.Split(new char[] { '=', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parts.Length == 0) { continue; }

                switch (parts[0].ToLower())
                {
                    case "material":
                        material = new()
                        {
                            Flags = 0
                        };
                        break;

                    case "identifier":
                        material.Name = parts[1].Replace("\"", "");
                        break;

                    case "flags":
                        string[] flags = parts[1][1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        foreach (string flag in flags) { material.Flags |= Enum.Parse<MATMaterial.Settings>(flag, true); }
                        break;

                    case "colour":
                        string[] colour = parts[1][1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        material.DiffuseColour[0] = byte.Parse(colour[0]);
                        material.DiffuseColour[1] = byte.Parse(colour[1]);
                        material.DiffuseColour[2] = byte.Parse(colour[2]);
                        break;

                    case "ambient":
                        material.AmbientLighting = float.Parse(parts[1]);
                        break;

                    case "diffuse":
                        material.DirectionalLighting = float.Parse(parts[1]);
                        break;

                    case "specular":
                        material.SpecularLighting = float.Parse(parts[1]);
                        break;

                    case "power":
                        material.SpecularPower = float.Parse(parts[1]);
                        break;

                    case "opacity":
                        material.DiffuseColour[3] = byte.Parse(parts[1]);
                        break;

                    case "index_base":
                        material.IndexBase = byte.Parse(parts[1]);
                        break;

                    case "index_range":
                        material.IndexRange = byte.Parse(parts[1]);
                        break;

                    case "colour_map":
                        material.Texture = parts[1].Replace("\"", "");
                        break;

                    case "index_shade":
                        material.ShadeTable = parts[1].Replace("\"", "");
                        break;

                    case "index_blend":
                        material.BlendTable = parts[1].Replace("\"", "");
                        break;

                    case "index_fog":
                        material.FogTable = parts[1].Replace("\"", "");
                        break;

                    case "]":
                        mat.Materials.Add(material);
                        break;

                    default:
                        Logger.LogToFile(Logger.LogLevel.Error, $"Unknown material setting: {parts[0]}");
                        return null;
                }
            } while (!file.EOF);

            return mat;
        }

        public void Save(string Path)
        {
            if (Materials.Count == 0) { return; }

            using (BEBinaryWriter bw = new BEBinaryWriter(new FileStream(Path, FileMode.Create)))
            {
                bw.WriteInt32(18);
                bw.WriteInt32(8);
                bw.WriteInt32(5);
                bw.WriteInt32(2);

                foreach (MATMaterial M in Materials)
                {
                    bw.Write(new byte[] { 0, 0, 0, 60 });
                    bw.WriteInt32(68 + M.Name.Length);

                    bw.Write(M.DiffuseColour);
                    bw.WriteSingle(M.AmbientLighting);
                    bw.WriteSingle(M.DirectionalLighting);
                    bw.WriteSingle(M.SpecularLighting);
                    bw.WriteSingle(M.SpecularPower);

                    bw.WriteInt32((int)M.Flags);

                    bw.WriteSingle(M.UVMatrix.M11);
                    bw.WriteSingle(M.UVMatrix.M12);
                    bw.WriteSingle(M.UVMatrix.M21);
                    bw.WriteSingle(M.UVMatrix.M22);
                    bw.WriteSingle(M.UVMatrix.M31);
                    bw.WriteSingle(M.UVMatrix.M32);

                    bw.Write(new byte[] { 10, 31, 0, 0 });                          //Unknown, seems to be a constant
                    bw.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }); //13 bytes of nothing please!

                    bw.Write(M.Name.ToCharArray());
                    bw.WriteByte(0);

                    if (!string.IsNullOrEmpty(M.Texture))
                    {
                        bw.Write(new byte[] { 0, 0, 0, 28 });
                        bw.WriteInt32(M.Texture.Length + 1);
                        bw.Write(M.Texture.ToCharArray());
                        bw.WriteByte(0);
                    }

                    bw.Write(0);
                    bw.Write(0);
                }
            }
        }
    }

    public class MATMaterial
    {
        [Flags]
        public enum Settings
        {
            Light = 1,
            PreLit = 2,
            Smooth = 4,
            Environment = 8,
            Environment_Local = 16,
            Perspective = 32,
            Decal = 64,
            IFromU = 128,
            IFromV = 256,
            UFromI = 512,
            VFromI = 1024,
            AlwaysVisible = 2048,
            Two_Sided = 4096,
            ForceFront = 8192,
            Dither = 16384,
            Custom = 32768,
            MapAntialiasing = 65536,
            MapInterpolation = 131072,
            MipInterpolation = 262144,
            Fog_Local = 524288,
            Subdivide = 1048576,
            ZTransparency = 2097152,
        }

        public string Name { get; set; } = "NEW_MATERIAL";

        public byte[] DiffuseColour { get; set; } = new byte[] { 255, 255, 255, 255 };

        public float AmbientLighting { get; set; } = 0.10000000149011612f;

        public float DirectionalLighting { get; set; } = 0.699999988079071f;

        public float SpecularLighting { get; set; } = 0;

        public float SpecularPower { get; set; } = 20;

        public byte IndexBase { get; set; }

        public byte IndexRange { get; set; }

        public string Texture { get; set; }

        public string ShadeTable { get; set; }

        public string BlendTable { get; set; }

        public string FogTable { get; set; }

        public Matrix2D UVMatrix { get; set; } = new Matrix2D(1, 0, 0, 1, 0, 0);

        public Settings Flags { get; set; } = Settings.Light | Settings.Perspective;

        public MATMaterial()
            : this("", "", Settings.Light | Settings.Perspective)
        {
        }

        public MATMaterial(string name, string texture)
            : this(name, texture, Settings.Light | Settings.Perspective)
        {
        }

        public MATMaterial(string name, string texture, Settings flags)
        {
            Name = name;
            Texture = texture;
            Flags = flags;
        }
    }
}