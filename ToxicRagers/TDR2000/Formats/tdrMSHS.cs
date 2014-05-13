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
            Logger.LogToFile("{0}", path);
            MSHS mshs = new MSHS();

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int faces = br.ReadInt16();
                    int mode = br.ReadInt16();

                    switch (mode)
                    {
                        case 0:
                            //                    'Standard Mesh
                            //                    Dim iVertCount As Integer = br.ReadInt32()
                            //                    'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                            //                    'Console.WriteLine("=========")
                            //                    br.ReadBytes(16) 'No clue

                            //                    Dim mshMesh As New tdrMesh(sOutName)

                            //                    For i As Integer = 0 To iFaceCount - 1
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
                            //                    Next

                            //                    mshObjects.Add(mshMesh)
                            break;

                        case 256:
                            //                    'PathFollower
                            //                    Dim iVertCount As Integer = br.ReadInt32()
                            //                    br.ReadBytes(16) 'The other two meshes have this 16 byte cluster, ignoring it here for consistency

                            //                    Dim mshMesh As New tdrMesh(sOutName)

                            //                    For i As Integer = 0 To iVertCount - 1
                            //                        mshMesh.AddListVertex(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                            //                    Next

                            //                    Dim uvOffset As Integer = 0

                            //                    For i As Integer = 0 To iFaceCount - 1
                            //                        br.ReadBytes(12) 'Vector3 - 3x float - Probably face normal
                            //                        Dim v1 As Integer = br.ReadInt32()
                            //                        Dim v2 As Integer = br.ReadInt32()
                            //                        Dim v3 As Integer = br.ReadInt32()
                            //                        br.ReadBytes(16) '4x float - Probably V1 colour
                            //                        br.ReadBytes(16) '4x float - Probably V1 colour
                            //                        br.ReadBytes(16) '4x float - Probably V1 colour
                            //                        mshMesh.AddListUV(br.ReadSingle(), br.ReadSingle())
                            //                        mshMesh.AddListUV(br.ReadSingle(), br.ReadSingle())
                            //                        mshMesh.AddListUV(br.ReadSingle(), br.ReadSingle())
                            //                        br.ReadBytes(12) 'Vector3 - 3x float - Probably V1 normal
                            //                        br.ReadBytes(12) 'Vector3 - 3x float - Probably V2 normal
                            //                        br.ReadBytes(12) 'Vector3 - 3x float - Probably V3 normal

                            //                        mshMesh.AddFace(v1, v2, v3, uvOffset, uvOffset + 1, uvOffset + 2)
                            //                        uvOffset += 3
                            //                    Next

                            //                    mshObjects.Add(mshMesh)
                            break;

                        case 512:   // Map
                            br.ReadBytes(16);
                            int vertCount = br.ReadInt32();

                            TDRMesh mshMesh = new TDRMesh();

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

                            mshs.meshes.Add(mshMesh);
                            break;

                        default:
                            Logger.LogToFile("Unknown mode: {0}", mode);
                            return null;
                    }
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
