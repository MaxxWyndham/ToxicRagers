using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class MSHS
    {
        List<TDRMesh> meshes = new List<TDRMesh>();

        public List<TDRMesh> Meshes { get { return meshes; } }

        public static MSHS Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            MSHS mshs = new MSHS();

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int faces = br.ReadInt16();
                    int mode = br.ReadInt16();
                    int vertCount;

                    TDRMesh mshMesh = new TDRMesh();

                    switch (mode)
                    {
                        case 0:   // PathFollower
                            vertCount = br.ReadInt32();
                            br.ReadBytes(16);

                            for (int i = 0; i < faces; i++)
                            {
                            //                        iVertCount = br.ReadInt32()

                            //                        mshMesh.BeginFace(tdrMesh.FaceMode.VertexList)
                            //                        mshMesh.SetFaceNormal(New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()))

                            //                        For j As Integer = 0 To iVertCount - 1
                            //                            mshMesh.AddFaceVertex(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                            //                            'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                            //                            'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                            //                            br.ReadBytes(12) 'Vector3 - 3x float - No idea
                            //                            br.ReadBytes(12) 'Vector3 - 3x float - No idea
                            //                            If br.ReadSingle() <> 1 Then MsgBox("Strange number isn't 1!")
                            //                            mshMesh.AddFaceUV(br.ReadSingle(), br.ReadSingle())
                            //                        Next

                            //                        mshMesh.EndFace()
                            }
                            break;

                        case 256:   // Car
                            var verts = new List<Vector3>();

                            vertCount = br.ReadInt32();
                            br.ReadBytes(16);

                            for (int i = 0; i < vertCount; i++)
                            {
                                verts.Add(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            }

                            for (int i = 0; i < faces; i++)
                            {
                                br.ReadBytes(12);  //Vector3 - 3x float - Probably face normal
                                var vp0 = verts[br.ReadInt32()];
                                var vp1 = verts[br.ReadInt32()];
                                var vp2 = verts[br.ReadInt32()];
                                br.ReadBytes(16); //4x float - Probably V1 colour
                                br.ReadBytes(16); //4x float - Probably V1 colour
                                br.ReadBytes(16); //4x float - Probably V1 colour
                                var vt0 = new Vector2(br.ReadSingle(), br.ReadSingle());
                                var vt1 = new Vector2(br.ReadSingle(), br.ReadSingle());
                                var vt2 = new Vector2(br.ReadSingle(), br.ReadSingle());
                                var vn0 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                var vn1 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                var vn2 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                                mshMesh.Vertexes.Add(new TDRVertex(vp0.X, vp0.Y, vp0.Z, vn0.X, vn0.Y, vn0.Z, 0, vt0.X, vt0.Y));
                                mshMesh.Vertexes.Add(new TDRVertex(vp1.X, vp1.Y, vp1.Z, vn1.X, vn1.Y, vn1.Z, 0, vt1.X, vt1.Y));
                                mshMesh.Vertexes.Add(new TDRVertex(vp2.X, vp2.Y, vp2.Z, vn2.X, vn2.Y, vn2.Z, 0, vt2.X, vt2.Y));
                                mshMesh.Faces.Add(new TDRFace(mshMesh.Vertexes.Count - 3, mshMesh.Vertexes.Count - 2, mshMesh.Vertexes.Count - 1));
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

                    mshs.meshes.Add(mshMesh);
                }
            }

            return mshs;
        }
    }

    public class TDRMesh
    {
        string name;
        List<TDRVertex> verts;
        List<TDRFace> faces;

        public string Name { get { return name; } }
        public List<TDRVertex> Vertexes { get { return verts; } set { verts = value; } }
        public List<TDRFace> Faces { get { return faces; } set { faces = value; } }

        public TDRMesh()
        {
            verts = new List<TDRVertex>();
            faces = new List<TDRFace>();
        }
    }

    public class TDRVertex
    {
        Vector3 position;
        Vector3 normal;
        Vector2 uv;
        Single unknown;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public Vector2 UV
        {
            get { return uv; }
            set { uv = value; }
        }

        public Single Unknown
        {
            get { return unknown; }
            set { unknown = value; }
        }

        public TDRVertex(Single X, Single Y, Single Z, Single NX, Single NY, Single NZ, Single Unknown, Single U, Single V)
        {
            position = new Vector3(X, Y, Z);
            normal = new Vector3(NX, NY, NZ);
            uv = new Vector2(U, V);
            unknown = Unknown;
        }

        public TDRVertex(Single X, Single Y, Single Z) : this(X, Y, Z, 0, 0, 0, 0, 0, 0) { }

        public override string ToString()
        {
            return "{ Position: {X:" + Position.X + " Y:" + Position.Y + " Z:" + Position.Z + "} Normal: {X:" + Normal.X + " Y:" + Normal.Y + " Z:" + Normal.Z + "} UV: {U:" + UV.X + " V:" + UV.Y + "} Unknown: { " + Unknown + " } }";
        }
    }

    public class TDRFace
    {
        int vertexA;
        int vertexB;
        int vertexC;

        public int V1 { get { return vertexA; } }
        public int V2 { get { return vertexB; } }
        public int V3 { get { return vertexC; } }

        public TDRFace(int A, int B, int C)
        {
            this.vertexA = A;
            this.vertexB = B;
            this.vertexC = C;
        }

        public override string ToString()
        {
            return "{ Face: {A:" + vertexA + " B:" + vertexB + " C:" + vertexC + "} }";
        }
    }
}
