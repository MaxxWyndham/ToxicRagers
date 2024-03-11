using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Brender.Formats
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
        public List<ACTNode> Sections { get; private set; } = new List<ACTNode>();

        public static ACT Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            ACT act = new ACT();

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadUInt32() != 0x12 ||
                    br.ReadUInt32() != 0x8 ||
                    br.ReadUInt32() != 0x1 ||
                    br.ReadUInt32() != 0x2)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid ACT file", path);
                    return null;
                }

                ACTNode a;

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    Section tag = (Section)br.ReadUInt32();
                    int _ = (int)br.ReadUInt32();

                    a = new ACTNode(tag);

                    switch (tag)
                    {
                        case Section.Name:  // 00 00 00 23
                            a.ActorType = (ActorType)br.ReadByte();
                            a.RenderStyle = (RenderStyle)br.ReadByte();
                            a.Identifier = br.ReadString();
                            if (a.Identifier.Length == 0) { a.Identifier = "NO_IDENTIFIER"; }
                            break;

                        case Section.Model:
                            a.Model = br.ReadString();
                            break;

                        case Section.Section37: // 00 00 00 25
                            break;

                        case Section.Material:
                            a.Material = br.ReadString();
                            break;

                        case Section.SubLevelBegin: // 00 00 00 29
                            break;

                        case Section.SubLevelEnd:
                            break;

                        case Section.Matrix: // 00 00 00 2B
                            a.Transform = new Matrix3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            break;

                        case Section.BoundingBox: // 00 00 00 32
                            a.Bounds = new MeshExtents(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()), new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            break;

                        case Section.EOF:
                            break;

                        default:
                            Console.WriteLine("Unknown ACT tag: {0} ({1} of {2})", tag, br.BaseStream.Position, br.BaseStream.Length);
                            return null;
                    }

                    act.Sections.Add(a);
                }
            }

            return act;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BEBinaryWriter bw = new BEBinaryWriter(fs, Encoding.Default))
            {
                int length;

                bw.WriteInt32(0x12);   // Magic Number
                bw.WriteInt32(0x8);    // 
                bw.WriteInt32(0x1);    // 
                bw.WriteInt32(0x2);    // 

                foreach (ACTNode A in Sections)
                {
                    bw.WriteInt32((int)A.Section);

                    switch (A.Section)
                    {
                        case Section.Name:
                            length = A.Identifier.Length + 3;

                            bw.WriteInt32(length);
                            bw.WriteByte((byte)A.ActorType);
                            bw.WriteByte((byte)A.RenderStyle);
                            bw.Write(A.Identifier.ToCharArray());
                            bw.WriteByte(0);
                            break;

                        case Section.Matrix:
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

                        case Section.Section37:
                        case Section.SubLevelBegin:
                        case Section.SubLevelEnd:
                            bw.WriteInt32(0);
                            break;

                        case Section.Model:
                            bw.WriteInt32(A.Model.Length + 1);
                            bw.Write(A.Model.ToCharArray());
                            bw.WriteByte(0);
                            break;
                    }
                }

                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }
        }

        public void AddRootNode(string name = "")
        {
            Sections.Add(new ACTNode(Section.Name, name));
            Sections.Add(new ACTNode(Section.Matrix));
            Sections.Add(new ACTNode(Section.Section37));
        }

        public void AddActor(string actorname, string model, Matrix3D transform, bool parent)
        {
            Sections.Add(new ACTNode(Section.Name, actorname) { ActorType = model != null ? ActorType.BR_ACTOR_MODEL : ActorType.BR_ACTOR_NONE });
            Sections.Add(new ACTNode(transform));
            Sections.Add(new ACTNode(Section.Section37));
            if (model != null) { Sections.Add(new ACTNode(Section.Model, model)); }
            if (!parent) { Sections.Add(new ACTNode(Section.SubLevelEnd)); }
        }

        public void AddPivot(string pivotname, string actorname, string actormodel, Matrix3D transform)
        {
            Sections.Add(new ACTNode(Section.Name, pivotname)); //, false, 0, 0
            Sections.Add(new ACTNode(transform));
            Sections.Add(new ACTNode(Section.Section37));

            Sections.Add(new ACTNode(Section.Name, actorname)); //, false, 1, 4
            Sections.Add(new ACTNode(Matrix3D.Identity));
            Sections.Add(new ACTNode(Section.Section37));
            Sections.Add(new ACTNode(Section.Model, actormodel));
            Sections.Add(new ACTNode(Section.SubLevelEnd));

            Sections.Add(new ACTNode(Section.SubLevelEnd));
        }

        public void AddSubLevelBegin()
        {
            Sections.Add(new ACTNode(Section.SubLevelBegin));
        }

        public void AddSubLevelEnd()
        {
            Sections.Add(new ACTNode(Section.SubLevelEnd));
        }
    }

    public class ACTNode
    {
        public Section Section { get; set; }

        public ActorType ActorType { get; set; } = ActorType.BR_ACTOR_MODEL;

        public RenderStyle RenderStyle { get; set; } = RenderStyle.BR_RSTYLE_FACES;

        public string Identifier { get; set; }

        public string Model { get; set; }

        public string Material { get; set; }

        public Matrix3D Transform { get; set; }

        public MeshExtents Bounds { get; set; }

        public ACTNode(Section section, string name = null)
        {
            Section = section;

            switch (Section)
            {
                case Section.Name:
                    Identifier = name;
                    break;

                case Section.Model:
                    Model = name;
                    break;

                case Section.Material:
                    Material = name;
                    break;
            }
        }

        public ACTNode(Matrix3D transform)
        {
            Section = Section.Matrix;
            Transform = transform;
        }
    }

    public enum ActorType
    {
        BR_ACTOR_NONE = 0x0,
        BR_ACTOR_MODEL = 0x1,
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
        BR_RSTYLE_FACES = 0x4,
        BR_RSTYLE_BOUNDING_POINTS,
        BR_RSTYLE_BOUNDING_EDGES,
        BR_RSTYLE_BOUNDING_FACES
    }
}