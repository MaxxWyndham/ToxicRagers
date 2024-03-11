using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class MAT
    {
        public List<MATMaterial> Materials { get; } = new List<MATMaterial>();

        public static MAT Load(string path)
        {
	        FileInfo fi = new FileInfo(path);
	        Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
	        return Load(fi.OpenRead(), path);
        }

		public static MAT Load(Stream stream, string path)
        {
            MAT mat = new MAT();

            MATMaterial M = new MATMaterial();
            bool bDebug = false;

            using (BEBinaryReader br = new BEBinaryReader(stream, Encoding.Default))
            {
                br.ReadBytes(16); // Header

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int tag = (int)br.ReadUInt32();
                    int length = (int)br.ReadUInt32();

                    switch (tag)
                    {
                        case 0x4:
                            // C1 mat file
                            M = new MATMaterial();

                            M.DiffuseColour[0] = br.ReadByte(); // R
                            M.DiffuseColour[1] = br.ReadByte(); // G
                            M.DiffuseColour[2] = br.ReadByte(); // B
                            M.DiffuseColour[3] = br.ReadByte(); // A
                            M.AmbientLighting = br.ReadSingle();
                            M.DirectionalLighting = br.ReadSingle();
                            M.SpecularLighting = br.ReadSingle();
                            M.SpecularPower = br.ReadSingle();
                            M.SetFlags((int)br.ReadUInt16()); // Flags
                            if (M.GetFlag(MATMaterial.Settings.UnknownSetting) || M.GetFlag(MATMaterial.Settings.IFromV) || M.GetFlag(MATMaterial.Settings.UFromI) || M.GetFlag(MATMaterial.Settings.VFromI)) { bDebug = true; }
                            M.UVMatrix = new Matrix2D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            byte x1 = br.ReadByte(); // ??
                            byte x2 = br.ReadByte(); // ??
                            M.Name = br.ReadString();

                            if (bDebug) { Console.WriteLine(path + " :: " + M.Name); bDebug = false; }
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
                            M.SetFlags((int)br.ReadUInt32()); // Flags
                            M.UVMatrix = new Matrix2D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            if (br.ReadUInt32() != 169803776) { Console.WriteLine("Weird Beard! (" + path + ")"); }
                            br.ReadBytes(13); // 13 bytes of nothing
                            M.Name = br.ReadString();
                            break;

                        case 0x1c:
                            M.Texture = br.ReadString();
                            break;

                        case 0x1f:
                            string _ = br.ReadString(); // shadetable
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

                    bw.WriteInt32(M.Flags);

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

                    if (M.HasTexture)
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
        public enum Settings
        {
            Lit = 1,
            PreLit = 2,
            Smooth = 4,
            EnvMapped_inf = 8,
            EnvMapped_loc = 16,
            CorrectPerspective = 32,
            Decal = 64,
            IFromU = 128,
            IFromV = 256,
            UFromI = 512,
            VFromI = 1024,
            AlwaysVisible = 2048,
            TwoSided = 4096,
            ForceFront = 8192,
            Dither = 16384,
            UnknownSetting = 32768,
            MapAntialiasing = 65536,
            MapInterpolation = 131072,
            MipInterpolation = 262144,
            FogLocal = 524288,
            Subdivide = 1048576,
            ZTransparency = 2097152,
        }

        public string Name { get; set; } = "NEW_MATERIAL";

        public byte[] DiffuseColour { get; set; } = new byte[] { 255, 255, 255, 255 };

        public float AmbientLighting { get; set; } = 0.10000000149011612f;

        public float DirectionalLighting { get; set; } = 0.699999988079071f;

        public float SpecularLighting { get; set; } = 0;

        public float SpecularPower { get; set; } = 20;

        public string Texture { get; set; }

        public Matrix2D UVMatrix { get; set; } = new Matrix2D(1, 0, 0, 1, 0, 0);

        public void SetFlags(int Flags)
        {
            this.Flags = Flags;
        }

        public void SetFlags(Settings Flags)
        {
            this.Flags = (int)Flags;
        }

        public bool GetFlag(Settings Flag)
        {
            return GetFlag((int)Flag);
        }

        public bool GetFlag(int Flag)
        {
            return ((Flags & Flag) > 0);
        }

        public int Flags { get; private set; } = ((int)Settings.Lit | (int)Settings.CorrectPerspective);

        public int Width { get; set; }

        public int Height { get; set; }

        public bool HasTexture => (Texture != null && Texture.Length > 0);

        public MATMaterial()
            : this("", "", Settings.Lit | Settings.CorrectPerspective)
        {
        }

        public MATMaterial(string name, string texture)
            : this(name, texture, Settings.Lit | Settings.CorrectPerspective)
        {
        }

        public MATMaterial(string name, string texture, Settings flags)
        {
            Name = name;
            Texture = texture;
            Flags = (int)flags;
        }
    }
}