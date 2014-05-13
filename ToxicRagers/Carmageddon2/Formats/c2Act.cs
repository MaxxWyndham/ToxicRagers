using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public enum Section
    {
        Name = 35,
        Matrix = 43,
        Section37 = 37,
        BoundingBox = 50,
        Material = 38,
        Model = 36,
        SubLevelBegin = 41,
        SubLevelEnd = 42,
        EOF = 0
    }

    public class ACT
    {
        List<ACTNode> sections;

        public List<ACTNode> Sections { get { return sections; } }

        public ACT()
        {
            sections = new List<ACTNode>();
        }

        public static ACT Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            ACT act = new ACT();

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadUInt32() != 18 ||
                    br.ReadUInt32() != 8 ||
                    br.ReadUInt32() != 1 ||
                    br.ReadUInt32() != 2)
                {
                    Logger.LogToFile("{0} isn't a valid ACT file", Path);
                    return null;
                }

                ACTNode a;

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    Section tag = (Section)br.ReadUInt32();
                    int length = (int)br.ReadUInt32();

                    a = new ACTNode(tag);

                    switch (tag)
                    {
                        case Section.Name:
                            a.ActorType = (ActorType)br.ReadByte();
                            a.RenderStyle = (RenderStyle)br.ReadByte();
                            a.Identifier = br.ReadString();
                            if (a.Identifier.Length == 0) { a.Identifier = "NO_IDENTIFIER"; }
                            break;

                        case Section.Model:
                            a.Model = br.ReadString();
                            break;

                        case Section.Section37:
                            break;

                        case Section.Material:
                            a.Material = br.ReadString();
                            break;

                        case Section.SubLevelBegin:
                            break;

                        case Section.SubLevelEnd:
                            break;

                        case Section.Matrix:
                            a.Transform = new Matrix3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            break;

                        case Section.BoundingBox:
                            a.Bounds = new MeshExtents(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()), new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            break;

                        case Section.EOF:
                            break;

                        default:
                            Console.WriteLine("Unknown ACT tag: {0} ({1} of {2})", tag, br.BaseStream.Position, br.BaseStream.Length);
                            return null;
                    }

                    act.sections.Add(a);
                }
            }

            return act;
        }

        public void Save(string Path)
        {
            string sPath, sFile;
            sPath = Path.Substring(0, Path.LastIndexOf("\\") + 1);
            sFile = Path.Substring(Path.LastIndexOf("\\") + 1);

            using (BEBinaryWriter bw = new BEBinaryWriter(new FileStream(Path, FileMode.Create), Encoding.Default))
            {
                int iLength;

                //output header
                bw.WriteInt32(18);
                bw.WriteInt32(8);
                bw.WriteInt32(1);
                bw.WriteInt32(2);

                foreach (ACTNode A in sections)
                {
                    bw.WriteInt32((int)A.Section);

                    switch ((int)A.Section)
                    {
                        case 35: //Name
                            iLength = A.Identifier.Length + 3;
                            bw.WriteInt32(iLength);
                            bw.WriteByte((byte)A.ActorType);
                            bw.WriteByte((byte)A.RenderStyle);
                            bw.Write(A.Identifier.ToCharArray());
                            bw.WriteByte(0);
                            break;

                        case 43: //Matrix
                            bw.WriteInt32(48);
                            bw.WriteSingle(A.Transform.M11);
                            bw.WriteSingle(A.Transform.M12);
                            bw.WriteSingle(A.Transform.M13);
                            bw.WriteSingle(A.Transform.M21);
                            bw.WriteSingle(A.Transform.M22);
                            bw.WriteSingle(A.Transform.M23);
                            bw.WriteSingle(A.Transform.M31);
                            bw.WriteSingle(A.Transform.M32);
                            bw.WriteSingle(A.Transform.M33);
                            bw.WriteSingle(A.Transform.M41);
                            bw.WriteSingle(A.Transform.M42);
                            bw.WriteSingle(A.Transform.M43);
                            break;

                        case 37: //Section 37
                            bw.WriteInt32(0);
                            break;

                        case 36: //Model
                            bw.WriteInt32(A.Model.Length + 1);
                            bw.Write(A.Model.ToCharArray());
                            bw.WriteByte(0);
                            break;

                        case 42: //Sub-Level End
                            bw.WriteInt32(0);
                            break;
                    }
                }

                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }
        }

        public void AddRootNode(string Name = "")
        {
            sections.Add(new ACTNode(Section.Name, Name));
            sections.Add(new ACTNode(Section.Matrix));
            sections.Add(new ACTNode(Section.Section37));
        }

        public void AddActor(string ActorName, string Model, Matrix3D Transform, bool Parent)
        {
            sections.Add(new ACTNode(Section.Name, ActorName)); //, false, 1, 4
            sections.Add(new ACTNode(Transform));
            sections.Add(new ACTNode(Section.Section37));
            sections.Add(new ACTNode(Section.Model, Model));
            if (!Parent) { sections.Add(new ACTNode(Section.SubLevelEnd)); }
        }

        public void AddPivot(string PivotName, string ActorName, string ActorModel, Matrix3D Transform)
        {
            sections.Add(new ACTNode(Section.Name, PivotName)); //, false, 0, 0
            sections.Add(new ACTNode(Transform));
            sections.Add(new ACTNode(Section.Section37));

            sections.Add(new ACTNode(Section.Name, ActorName)); //, false, 1, 4
            sections.Add(new ACTNode(Matrix3D.Identity));
            sections.Add(new ACTNode(Section.Section37));
            sections.Add(new ACTNode(Section.Model, ActorModel));
            sections.Add(new ACTNode(Section.SubLevelEnd));

            sections.Add(new ACTNode(Section.SubLevelEnd));
        }
    }

    public class ACTNode
    {
        Section section;
        ActorType type;
        RenderStyle renderStyle;
        string identifier;
        string model;
        string material;
        Matrix3D transform;
        MeshExtents bounds;

        public Section Section
        {
            get { return section; }
            set { section = value; }
        }

        public ActorType ActorType
        {
            get { return type; }
            set { type = value; }
        }

        public RenderStyle RenderStyle
        {
            get { return renderStyle; }
            set { renderStyle = value; }
        }

        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public string Material
        {
            get { return material; }
            set { material = value; }
        }

        public Matrix3D Transform
        {
            get { return transform; }
            set { transform = value; }
        }

        public MeshExtents Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public ACTNode(Section section, string name = null)
        {
            this.section = section;

            switch (Section)
            {
                case Section.Name:
                    this.identifier = name;
                    break;

                case Section.Model:
                    this.model = name;
                    break;

                case Section.Material:
                    this.material = name;
                    break;
            }
        }

        public ACTNode(Matrix3D transform)
        {
            this.section = Section.Matrix;
            this.transform = transform;
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