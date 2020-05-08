using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class MSHS
    {
        public List<TDRMesh> Meshes { get; } = new List<TDRMesh>();

        public static MSHS Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            MSHS mshs = new MSHS();
            bool singleMesh = !fi.Extension.EndsWith("s");

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int faces = br.ReadInt16();
                    int mode = br.ReadInt16();
                    int vertCount;
                    Vector3 centre;
                    float radius;

                    TDRMesh mshMesh = new TDRMesh();

                    switch (mode)
                    {
                        case 0:   // PathFollower
                            vertCount = br.ReadInt32();

                            centre = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            radius = br.ReadSingle();

                            for (int i = 0; i < faces; i++)
                            {
                                int iVertCount = br.ReadInt32();

                                //mshMesh.BeginFace(tdrMesh.FaceMode.VertexList);
                                //mshMesh.SetFaceNormal(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                                br.ReadBytes(12);

                                for (int j = 0; j < iVertCount; j++)
                                {
                                    TDRVertex v = new TDRVertex(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                                    {
                                        Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                                    };
                                    //Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                                    br.ReadBytes(12); //Vector3 - 3x float - No idea
                                    if (br.ReadSingle() != 1) { Logger.LogToFile(Logger.LogLevel.Error, "Strange value isn't 1"); }

                                    v.UV = new Vector2(br.ReadSingle(), br.ReadSingle());

                                    mshMesh.Vertexes.Add(v);

                                    if (j >= 2)
                                    {
                                        int flip = j % 2;

                                        mshMesh.Faces.Add(
                                            new TDRFace(
                                                mshMesh.Vertexes.Count - (3 + flip),
                                                mshMesh.Vertexes.Count - 2,
                                                mshMesh.Vertexes.Count - 1));
                                    }
                                }
                            }
                            break;

                        case 256:   // Car
                            vertCount = br.ReadInt32();

                            centre = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            radius = br.ReadSingle();

                            List<Vector3> verts = new List<Vector3>();

                            for (int i = 0; i < vertCount; i++)
                            {
                                verts.Add(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            }

                            for (int i = 0; i < faces; i++)
                            {
                                Vector3 faceNormal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Vector3 vp0 = verts[br.ReadInt32()];
                                Vector3 vp1 = verts[br.ReadInt32()];
                                Vector3 vp2 = verts[br.ReadInt32()];
                                Vector4 v1colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Vector4 v2colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Vector4 v3colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Vector2 vt0 = new Vector2(br.ReadSingle(), br.ReadSingle());
                                Vector2 vt1 = new Vector2(br.ReadSingle(), br.ReadSingle());
                                Vector2 vt2 = new Vector2(br.ReadSingle(), br.ReadSingle());
                                Vector3 vn0 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Vector3 vn1 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Vector3 vn2 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                                mshMesh.Vertexes.Add(new TDRVertex(vp0.X, vp0.Y, vp0.Z, vn0.X, vn0.Y, vn0.Z, 0, vt0.X, vt0.Y));
                                mshMesh.Vertexes.Add(new TDRVertex(vp1.X, vp1.Y, vp1.Z, vn1.X, vn1.Y, vn1.Z, 0, vt1.X, vt1.Y));
                                mshMesh.Vertexes.Add(new TDRVertex(vp2.X, vp2.Y, vp2.Z, vn2.X, vn2.Y, vn2.Z, 0, vt2.X, vt2.Y));

                                mshMesh.Faces.Add(new TDRFace(mshMesh.Vertexes.Count - 3, mshMesh.Vertexes.Count - 2, mshMesh.Vertexes.Count - 1));
                            }

                            if (singleMesh)
                            {
                                for (int i = 0; i < vertCount; i++)
                                {
                                    int pointCount = br.ReadUInt16();

                                    for (int j = 0; j < pointCount; j++)
                                    {
                                        br.ReadUInt16();
                                    }
                                }
                            }
                            break;

                        case 512:   // Map
                            br.ReadBytes(16);
                            vertCount = br.ReadInt32();

                            for (int i = 0; i < vertCount; i++)
                            {
                                mshMesh.Vertexes.Add(
                                    new TDRVertex(
                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                        br.ReadSingle(),
                                        br.ReadSingle(), br.ReadSingle()
                                    )
                                );
                            }

                            for (int i = 0; i < faces; i++)
                            {
                                mshMesh.Faces.Add(
                                    new TDRFace(
                                        br.ReadInt32(), br.ReadInt32(), br.ReadInt32()
                                    )
                                );
                            }
                            break;

                        default:
                            Logger.LogToFile(Logger.LogLevel.Error, "Unknown mode: {0}", mode);
                            return null;
                    }

                    mshs.Meshes.Add(mshMesh);
                }
            }

            return mshs;
        }
    }

    public class TDRMesh
    {
        List<TDRVertex> verts;
        List<TDRFace> faces;

        public List<TDRVertex> Vertexes
        {
            get => verts;
            set => verts = value;
        }

        public List<TDRFace> Faces
        {
            get => faces;
            set => faces = value;
        }

        public TDRMesh()
        {
            verts = new List<TDRVertex>();
            faces = new List<TDRFace>();
        }
    }

    public class TDRVertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 UV { get; set; }
        public float Unknown { get; set; }

        public TDRVertex(float X, float Y, float Z, float NX, float NY, float NZ, float Unknown, float U, float V)
        {
            Position = new Vector3(X, Y, Z);
            Normal = new Vector3(NX, NY, NZ);
            UV = new Vector2(U, V);
            this.Unknown = Unknown;
        }

        public TDRVertex(float X, float Y, float Z) : this(X, Y, Z, 0, 0, 0, 0, 0, 0) { }

        public override string ToString()
        {
            return $"{{ Position: {{X:{Position.X} Y:{Position.Y} Z:{Position.Z}}} Normal: {{X:{Normal.X} Y:{Normal.Y} Z:{Normal.Z}}} UV: {{U:{UV.X} V:{UV.Y}}} Unknown: {{ {Unknown} }} }}";
        }
    }

    public class TDRFace
    {
        public int V1 { get; set; }
        public int V2 { get; set; }
        public int V3 { get; set; }

        public TDRFace(int A, int B, int C)
        {
            V1 = A;
            V2 = B;
            V3 = C;
        }

        public override string ToString()
        {
            return $"{{ Face: {{A:{V1} B:{V2} C:{V3}}} }}";
        }
    }
}