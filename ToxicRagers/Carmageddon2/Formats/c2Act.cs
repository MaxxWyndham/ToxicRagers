using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class c2Act
    {
        public List<Actor> Actors;

        public c2Act()
        {
            Actors = new List<Actor>();
        }

        public bool Load(string Path)
        {
            bool bSuccess = true;
            byte depth = 0;
            actor A = new actor(ActorType.BR_ACTOR_NONE);

            BEBinaryReader br = new BEBinaryReader(new FileStream(Path, FileMode.Open));


            if (br.ReadUInt32() != 18 ||
                br.ReadUInt32() != 8 ||
                br.ReadUInt32() != 1 ||
                br.ReadUInt32() != 2)
            {
                Console.WriteLine("{0} isn't a valid ACT file", Path);
                return false;
            }

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                int Tag = (int)br.ReadUInt32();
                int Length = (int)br.ReadUInt32();

                switch (Tag)
                {
                    case 35: // 00 00 00 23 : Name
                        A = new actor((ActorType)br.ReadByte());
                        A.renderstyle = (RenderStyle)br.ReadByte();
                        A.identifier = br.ReadString();
                        if (A.identifier.Length == 0) { A.identifier = "NO_IDENTIFIER"; }

                        Console.WriteLine("{0}\t{1} {2} {3}", depth, A.identifier, A.type, A.renderstyle);
                        break;

                    case 36: // 00 00 00 24 : MeshName
                        A.model = br.ReadString();
                        Console.WriteLine("\t{0}", A.model);
                        break;

                    case 37: // 00 00 00 25
                        depth++;
                        break;

                    case 38: // 00 00 00 26 : DefaultMaterial
                        A.material = br.ReadString();
                        break;

                    case 39: // 00 00 00 27
                        break;

                    case 41: // 00 00 00 29 : SubLevelBegin
                        break;

                    case 42: // 00 00 00 2A : SubLevelEnd
                        depth--;
                        break;

                    case 43: // 00 00 00 2B : Transformation Matrix
                        A.transform = new Matrix3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        break;

                    case 50: // 00 00 00 32 : Bounding Box
                        A.bounds = new MeshExtents(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()), new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                        Console.WriteLine("\t{0}", A.bounds.ToString());
                        break;

                    case 51:
                        // Light block (only seen inside .lit files)
                        br.ReadBytes(21);
                        break;

                    case 0: // 00 00 00 00  : EndOfFile
                        break;

                    default:
                        Console.WriteLine("Unknown ACT tag: " + Tag + " (" + br.BaseStream.Position + " :: " + br.BaseStream.Length + ")");
                        br.BaseStream.Position = br.BaseStream.Length;
                        bSuccess = false;
                        break;
                }
            }

            br.Close();
            return bSuccess;
        }

        public void Save(string Path)
        {
            string sPath, sFile;
            sPath = Path.Substring(0, Path.LastIndexOf("\\") + 1);
            sFile = Path.Substring(Path.LastIndexOf("\\") + 1);

            BEBinaryWriter bw = new BEBinaryWriter(new FileStream(Path, FileMode.Create));
            int iLength;

            //output header
            bw.WriteInt32(18);
            bw.WriteInt32(8);
            bw.WriteInt32(1);
            bw.WriteInt32(2);

            foreach (Actor A in Actors)
            {
                bw.WriteInt32((int)A.Section);

                switch ((int)A.Section)
                {
                    case 35:
                        //Name
                        iLength = A.Name.Length + 3;
                        if (A.PoundPrefix) { iLength += 2; }
                        bw.WriteInt32(iLength);
                        bw.WriteByte(A.AttributeA);
                        bw.WriteByte(A.AttributeB);
                        if (A.PoundPrefix)
                        {
                            bw.WriteByte(38);  //&
                            bw.WriteByte(163); //£
                        }
                        bw.Write(A.Name.ToCharArray());
                        bw.WriteByte(0);
                        break;

                    case 43:
                        //Matrix
                        bw.WriteInt32(48);
                        bw.WriteSingle(A.Matrix.M11);
                        bw.WriteSingle(A.Matrix.M12);
                        bw.WriteSingle(A.Matrix.M13);
                        bw.WriteSingle(A.Matrix.M21);
                        bw.WriteSingle(A.Matrix.M22);
                        bw.WriteSingle(A.Matrix.M23);
                        bw.WriteSingle(A.Matrix.M31);
                        bw.WriteSingle(A.Matrix.M32);
                        bw.WriteSingle(A.Matrix.M33);
                        bw.WriteSingle(A.Matrix.M41);
                        bw.WriteSingle(A.Matrix.M42);
                        bw.WriteSingle(A.Matrix.M43);
                        break;

                    case 37:
                        //Section 37
                        bw.WriteInt32(0);
                        break;

                    case 36:
                        //Model
                        bw.WriteInt32(A.Model.Length + 1);
                        bw.Write(A.Model.ToCharArray());
                        bw.WriteByte(0);
                        break;

                    case 42:
                        //Sub-Level End
                        bw.WriteInt32(0);
                        break;
                }
            }

            bw.WriteInt32(0);
            bw.WriteInt32(0);

            bw.Close();
        }

        public void AddRootNode(string Name = "")
        {
            Actors.Add(new Actor(Actor.Sections.Name, Name));
            Actors.Add(new Actor(Actor.Sections.Matrix));
            Actors.Add(new Actor(Actor.Sections.Section37));
        }

        public void AddActor(string ActorName, string Model, Matrix3D Transform, bool Parent)
        {
            Actors.Add(new Actor(Actor.Sections.Name, ActorName, false, 1, 4));
            Actors.Add(new Actor(Transform));
            Actors.Add(new Actor(Actor.Sections.Section37));
            Actors.Add(new Actor(Actor.Sections.Model, Model));
            if (!Parent) { Actors.Add(new Actor(Actor.Sections.SubLevelEnd)); }
        }

        public void AddPivot(string PivotName, string ActorName, string ActorModel, Matrix3D Transform)
        {
            Actors.Add(new Actor(Actor.Sections.Name, PivotName, false, 0, 0));
            Actors.Add(new Actor(Transform));
            Actors.Add(new Actor(Actor.Sections.Section37));

            Actors.Add(new Actor(Actor.Sections.Name, ActorName, false, 1, 4));
            Actors.Add(new Actor(Matrix3D.Identity));
            Actors.Add(new Actor(Actor.Sections.Section37));
            Actors.Add(new Actor(Actor.Sections.Model, ActorModel));
            Actors.Add(new Actor(Actor.Sections.SubLevelEnd));

            Actors.Add(new Actor(Actor.Sections.SubLevelEnd));
        }
    }

    public class Actor
    {
        #region Enums
        public enum Sections
        {
            Name = 35,          // 00 00 00 23
            Matrix = 43,        // 00 00 00 2B
            Section37 = 37,     // 00 00 00 25
            BoundingBox = 50,
            Material = 38,
            Model = 36,         // 00 00 00 24
            SubLevelBegin = 41,
            SubLevelEnd = 42,   // 00 00 00 2A
        }
        #endregion

        #region Variables
        private Sections _section = Sections.Section37;
        private string _name = "";
        private string _model = "";
        private string _material = "";
        private bool _poundprefix;
        private byte _attribA = 0;
        private byte _attribB = 0;
        private Matrix3D _matrix = Matrix3D.Identity;
        #endregion

        #region Properties
        public Sections Section
        {
            get { return _section; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public byte AttributeA
        {
            get { return _attribA; }
            set { _attribA = value; }
        }

        public byte AttributeB
        {
            get { return _attribB; }
            set { _attribB = value; }
        }
        #endregion

        public bool PoundPrefix { get { return _poundprefix; } }

        public Matrix3D Matrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        #region Constructors
        public Actor(Sections Section)
            : this(Section, string.Empty, false, 0, 0)
        {
        }

        public Actor(Sections Section, string Name)
            : this(Section, Name, false, 0, 0)
        {
        }

        public Actor(Sections Section, string Name, bool PoundPrefix)
            : this(Section, Name, PoundPrefix, 0, 0)
        {
        }

        public Actor(Sections Section, string Name, bool PoundPrefix, byte AttributeA, byte AttributeB)
        {
            _section = Section;

            if (Name != string.Empty)
            {
                _poundprefix = PoundPrefix;
                _attribA = AttributeA;
                _attribB = AttributeB;

                switch (Section)
                {
                    case Sections.Name:
                        _name = Name;
                        break;
                    case Sections.Model:
                        _model = Name;
                        break;
                    case Sections.Material:
                        _material = Name;
                        break;
                }
            }
        }

        public Actor(Matrix3D Matrix)
        {
            _section = Sections.Matrix;
            _matrix = Matrix;
        }
        #endregion
    }

    public class actor
    {
        public ActorType type;
        public RenderStyle renderstyle;
        public string identifier;
        public string model;
        public string material;
        public Matrix3D transform;
        public MeshExtents bounds;

        public actor(ActorType type)
        {
            this.type = type;
        }
    }

    public enum ActorType
    {
        BR_ACTOR_NONE,
        BR_ACTOR_MODEL,
        BR_ACTOR_LIGHT,
        BR_ACTOR_CAMERA,
        BR_ACTOR_BOUNDS,
        BR_ACTOR_BOUNDS_CORRECT,
        BR_ACTOR_CLIP_PLANE
    }

    public enum RenderStyle
    {
        BR_RSTYLE_DEFAULT,
        BR_RSTYLE_NONE,
        BR_RSTYLE_POINTS,
        BR_RSTYLE_EDGES,
        BR_RSTYLE_FACES,
        BR_RSTYLE_BOUNDING_POINTS,
        BR_RSTYLE_BOUNDING_EDGES,
        BR_RSTYLE_BOUNDING_FACES
    }
}