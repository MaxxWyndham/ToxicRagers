using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Brender.Formats
{
    public class ACT
    {
        public List<ACTNode> Chunks { get; private set; } = new List<ACTNode>();

        public static ACT Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            ACT act = new();

            using (BEBinaryReader br = new(fi.OpenRead(), Encoding.Default))
            {
                if (br.BaseStream.Length < 16 ||
                    br.ReadUInt32() != 0x12 ||
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
                    ChunkId tag = (ChunkId)br.ReadUInt32();
                    int _ = (int)br.ReadUInt32();

                    a = new ACTNode(tag);

                    switch (tag)
                    {
                        // 00 00 00 23
                        case ChunkId.Actor:
                            a.ActorType = (ActorType)br.ReadByte();
                            a.RenderStyle = (RenderStyle)br.ReadByte();
                            a.Identifier = br.ReadString();
                            if (a.Identifier.Length == 0) { a.Identifier = "NO_IDENTIFIER"; }
                            break;

                        // 00 00 00 24
                        case ChunkId.Model:
                            a.Model = br.ReadString();
                            break;

                        // 00 00 00 25
                        case ChunkId.Transform:
                            break;

                        // 00 00 00 26
                        case ChunkId.Material:
                            a.Material = br.ReadString();
                            break;

                        // 00 00 00 27
                        case ChunkId.Light:
                            break;

                        // 00 00 00 27
                        case ChunkId.CameraOld:
                            break;

                        // 00 00 00 29
                        case ChunkId.Bounds:
                            break;

                        // 00 00 00 2A
                        case ChunkId.AddChild:
                            break;

                        // 00 00 00 2B
                        case ChunkId.Matrix:
                            a.Transform = new Matrix3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            break;

                        // 00 00 00 32
                        case ChunkId.BoundingBox:
                            a.Bounds = new MeshExtents(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()), new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            break;

                        // 00 00 00 33
                        case ChunkId.LightOld:
                            a.Light = new Light()
                            {
                                Type = (LightType)br.ReadByte(),
                                Colour = Colour.FromRgb(br.ReadByte(), br.ReadByte(), br.ReadByte()),
                                Attenuation = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                ConeInner = br.ReadUInt16(),
                                ConeOuter = br.ReadUInt16()
                            };
                            a.Identifier = br.ReadString();
                            break;

                        // 00 00 00 34
                        case ChunkId.Camera:
                            a.Camera = new Camera()
                            {
                                Type = (CameraType)br.ReadByte(),
                                FOV = br.ReadUInt16(),
                                HitherZ = br.ReadSingle(),
                                YonZ = br.ReadSingle(),
                                Aspect = br.ReadSingle()
                            };
                            a.Identifier = br.ReadString();
                            break;

                        case ChunkId.EOF:
                            break;

                        default:
                            Console.WriteLine("Unknown ChunkId: {0} ({1} of {2})", tag, br.BaseStream.Position, br.BaseStream.Length);
                            return null;
                    }

                    act.Chunks.Add(a);
                }
            }

            return act;
        }

        public void Save(string path)
        {
            using (FileStream fs = new(path, FileMode.Create))
            using (BEBinaryWriter bw = new(fs, Encoding.Default))
            {
                int length;

                bw.WriteInt32(0x12);   // Magic Number
                bw.WriteInt32(0x8);    // 
                bw.WriteInt32(0x1);    // 
                bw.WriteInt32(0x2);    // 

                foreach (ACTNode A in Chunks)
                {
                    bw.WriteInt32((int)A.Section);

                    switch (A.Section)
                    {
                        case ChunkId.Actor:
                            length = A.Identifier.Length + 3;

                            bw.WriteInt32(length);
                            bw.WriteByte((byte)A.ActorType);
                            bw.WriteByte((byte)A.RenderStyle);
                            bw.Write(A.Identifier.ToCharArray());
                            bw.WriteByte(0);
                            break;

                        case ChunkId.Matrix:
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

                        case ChunkId.Transform:
                        case ChunkId.Bounds:
                        case ChunkId.AddChild:
                            bw.WriteInt32(0);
                            break;

                        case ChunkId.Model:
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
            Chunks.Add(new ACTNode(ChunkId.Actor, name));
            Chunks.Add(new ACTNode(ChunkId.Matrix));
            Chunks.Add(new ACTNode(ChunkId.Transform));
        }

        public void AddActor(string actorname, string model, Matrix3D transform, bool parent)
        {
            Chunks.Add(new ACTNode(ChunkId.Actor, actorname) { ActorType = model != null ? ActorType.BR_ACTOR_MODEL : ActorType.BR_ACTOR_NONE });
            Chunks.Add(new ACTNode(transform));
            Chunks.Add(new ACTNode(ChunkId.Transform));
            if (model != null) { Chunks.Add(new ACTNode(ChunkId.Model, model)); }
            if (!parent) { Chunks.Add(new ACTNode(ChunkId.AddChild)); }
        }

        public void AddPivot(string pivotname, string actorname, string actormodel, Matrix3D transform)
        {
            Chunks.Add(new ACTNode(ChunkId.Actor, pivotname)); //, false, 0, 0
            Chunks.Add(new ACTNode(transform));
            Chunks.Add(new ACTNode(ChunkId.Transform));

            Chunks.Add(new ACTNode(ChunkId.Actor, actorname)); //, false, 1, 4
            Chunks.Add(new ACTNode(Matrix3D.Identity));
            Chunks.Add(new ACTNode(ChunkId.Transform));
            Chunks.Add(new ACTNode(ChunkId.Model, actormodel));
            Chunks.Add(new ACTNode(ChunkId.AddChild));

            Chunks.Add(new ACTNode(ChunkId.AddChild));
        }
    }

    public class ACTNode
    {
        public ChunkId Section { get; set; }

        public ActorType ActorType { get; set; } = ActorType.BR_ACTOR_MODEL;

        public RenderStyle RenderStyle { get; set; } = RenderStyle.BR_RSTYLE_FACES;

        public string Identifier { get; set; }

        public string Model { get; set; }

        public string Material { get; set; }

        public Matrix3D Transform { get; set; }

        public MeshExtents Bounds { get; set; }

        public Light Light { get; set; }

        public Camera Camera { get; set; }

        public ACTNode(ChunkId section, string name = null)
        {
            Section = section;

            switch (Section)
            {
                case ChunkId.Actor:
                    Identifier = name;
                    break;

                case ChunkId.Model:
                    Model = name;
                    break;

                case ChunkId.Material:
                    Material = name;
                    break;
            }
        }

        public ACTNode(Matrix3D transform)
        {
            Section = ChunkId.Matrix;
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

    public enum LightType
    {

    }

    public class Light
    {
        public LightType Type { get; set; }

        public Colour Colour { get; set; }

        public Vector3 Attenuation { get; set; }

        public ushort ConeInner { get; set; }

        public ushort ConeOuter { get; set; }
    }

    public enum CameraType
    {

    }

    public class Camera
    {
        public CameraType Type { get; set; }

        public ushort FOV { get; set; }

        public float HitherZ { get; set; }

        public float YonZ { get; set; }

        public float Aspect { get; set; }
    }
}