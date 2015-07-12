using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToxicRagers.Helpers;
using ToxicRagers.Carmageddon2.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class MAT
    {
        List<MATMaterial> materials;

        public List<MATMaterial> Materials { get { return materials; } }

        public MAT()
        {
            materials = new List<MATMaterial>();
        }

        public static MAT Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            MAT mat = new MAT();

            MATMaterial M = new MATMaterial();
            bool bDebug = false;

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead(), Encoding.Default))
            {
                br.ReadBytes(16); // Header

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int tag = (int)br.ReadUInt32();
                    int length = (int)br.ReadUInt32();

                    switch (tag)
                    {
                        case 4:
                            // C1 mat file
                            M = new MATMaterial();

                            br.ReadByte(); // R
                            br.ReadByte(); // G
                            br.ReadByte(); // B
                            br.ReadByte(); // A
                            br.ReadSingle(); // Emissive Colour
                            br.ReadSingle(); // Directional
                            br.ReadSingle(); // Specular Colour
                            br.ReadSingle(); // Specular Power
                            M.SetFlags((int)br.ReadUInt16()); // Flags
                            if (M.GetFlag(MATMaterial.Settings.UnknownSetting) || M.GetFlag(MATMaterial.Settings.IFromV) || M.GetFlag(MATMaterial.Settings.UFromI) || M.GetFlag(MATMaterial.Settings.VFromI)) { bDebug = true; }
                            M.UVMatrix = new Matrix2D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            byte x1 = br.ReadByte(); // ??
                            byte x2 = br.ReadByte(); // ??
                            M.Name = br.ReadString();
                            //Console.WriteLine(Path + "\t" + M.Name + "\t" + x1 + "\t" + x2);

                            if (bDebug) { Console.WriteLine(path + " :: " + M.Name); bDebug = false; }
                            break;

                        case 60:
                            M = new MATMaterial();

                            br.ReadByte(); // R
                            br.ReadByte(); // G
                            br.ReadByte(); // B
                            br.ReadByte(); // A
                            br.ReadSingle(); // Emissive Colour
                            br.ReadSingle(); // Directional
                            br.ReadSingle(); // Specular Colour
                            br.ReadSingle(); // Specular Power
                            M.SetFlags((int)br.ReadUInt32()); // Flags
                            M.UVMatrix = new Matrix2D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            if (br.ReadUInt32() != 169803776) { Console.WriteLine("Weird Beard! (" + path + ")"); }
                             br.ReadBytes(13); // 13 bytes of nothing
                            M.Name = br.ReadString();
                            break;

                        case 28:
                            M.Texture = br.ReadString();
                            //Tif t = new Tif(Path.Substring(0, Path.LastIndexOf("\\")) + "\\tiffrgb\\" + M.Texture + ".tif");
                            //M.TextureData = t.GetPixelData();
                            //M.Width = t.ImageWidth;
                            //M.Height = t.ImageHeight;
                            break;

                        case 31:
                            string s = br.ReadString(); // shadetable
                            break;

                        case 0:
                            mat.materials.Add(M);
                            break;

                        default:
                            Logger.LogToFile(Logger.LogLevel.Error, "Unknown MAT tag: {0} ({1})", tag, br.BaseStream.Position.ToString("X"));
                            return null;
                    }
                    //if (ReadUInt32(br) != 60) { break; }
                    //int Length = (int)ReadUInt32(br) - 67; // Length of material name

                    //materialContent = new BasicMaterialContent();

                    //materialContent.DiffuseColor = new Vector3(br.ReadByte(), br.ReadByte(), br.ReadByte()); materialContent.DiffuseColor = null;
                    //materialContent.Alpha = br.ReadByte();
                    //materialContent.EmissiveColor = Color.White.ToVector3() * ReadSingle(br);
                    //ReadSingle(br); // Directional
                    //materialContent.SpecularColor = Color.White.ToVector3() * ReadSingle(br);
                    //materialContent.SpecularPower = ReadSingle(br);
                    //ReadUInt32(br); // Flags, don't know what to do with these yet
                    //ReadSingle(br); // Xx
                    //ReadSingle(br); // Yx
                    //ReadSingle(br); // Xy - 2d transformation matrix, again, no idea how this is useful
                    //ReadSingle(br); // Yy
                    //ReadSingle(br); // Px
                    //ReadSingle(br); // Py
                    //ReadUInt32(br);
                    //br.ReadBytes(13); // 13 pointless bytes of nothing
                    //materialContent.Name = ToString(br.ReadChars(Length));
                    //if (ReadUInt32(br) != 28) { break; }
                    //Length = (int)ReadUInt32(br);
                    //materialContent.Texture = new ExternalReference<TextureContent>("tiffrgb\\" + ToString(br.ReadChars(Length)) + ".tif", rootNode.Identity);

                    //if (ReadUInt32(br) == 0 && ReadUInt32(br) == 0)
                    //{
                    //    if (!materials.ContainsKey(materialContent.Name)) { materials.Add(materialContent.Name, materialContent); }
                    //}
                    //else
                    //{
                    //    Log("Something has gone horribly wrong");
                    //    br.BaseStream.Position = br.BaseStream.Length;
                    //}
                }
            }

            return mat;
        }

        public void Save(string Path)
        {
            if (this.materials.Count == 0) { return; }

            using (BEBinaryWriter bw = new BEBinaryWriter(new FileStream(Path, FileMode.Create)))
            {
                bw.WriteInt32(18);
                bw.WriteInt32(8);
                bw.WriteInt32(5);
                bw.WriteInt32(2);

                foreach (var M in this.materials)
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
        #region Enums
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
        #endregion

        private int _width;
        private int _height;
        private string _name = "NEW_MATERIAL";
        private string _texture;
        private int _flags = ((int)Settings.Lit | (int)Settings.CorrectPerspective);

        private byte[] _diffuse = new byte[] { 255, 255, 255, 255 };
        private Single _ambient = 0.10000000149011612f;
        private Single _directional = 0.699999988079071f;
        private Single _specular = 0;
        private Single _specularpower = 20;

        private Matrix2D _matrix = new Matrix2D(1, 0, 0, 1, 0, 0);
        //private Color[] _texturedata;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public byte[] DiffuseColour
        {
            get { return _diffuse; }
            set { _diffuse = value; }
        }

        public Single AmbientLighting
        {
            get { return _ambient; }
            set { _ambient = value; }
        }

        public Single DirectionalLighting
        {
            get { return _directional; }
            set { _directional = value; }
        }

        public Single SpecularLighting
        {
            get { return _specular; }
            set { _specular = value; }
        }

        public Single SpecularPower
        {
            get { return _specularpower; }
            set { _specularpower = value; }
        }

        public string Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        //public Color[] TextureData
        //{
        //    get { return _texturedata; }
        //    set { _texturedata = value; }
        //}

        public Matrix2D UVMatrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public void SetFlags(int Flags)
        {
            _flags = Flags;
        }

        public void SetFlags(Settings Flags)
        {
            _flags = (int)Flags;
        }

        public bool GetFlag(Settings Flag)
        {
            return GetFlag((int)Flag);
        }

        public bool GetFlag(int Flag)
        {
            return ((_flags & Flag) > 0);
        }

        public int Flags { get { return _flags; } }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public bool HasTexture
        {
            get { return (_texture != null && _texture.Length > 0); }
        }

        #region Constructors
        public MATMaterial()
            : this("", "", (Settings.Lit | Settings.CorrectPerspective))
        {
        }

        public MATMaterial(string Name, string Texture)
            : this(Name, Texture, (Settings.Lit | Settings.CorrectPerspective))
        {
        }

        public MATMaterial(string Name, string Texture, Settings Flags)
        {
            _name = Name;
            _texture = Texture;
            _flags = (int)Flags;
        }
        #endregion
    }
}