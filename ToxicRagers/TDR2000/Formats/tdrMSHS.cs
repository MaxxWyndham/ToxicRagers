using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    TDRMesh msh = new TDRMesh
                    {
                        FaceCount = br.ReadUInt16(),
                        Mode = (TDRMesh.MSHMode)br.ReadUInt16()
                    };

                    if (msh.Mode == TDRMesh.MSHMode.Tri) { br.ReadBytes(16); }

                    msh.VertexCount = br.ReadInt32();

                    if (msh.Mode != TDRMesh.MSHMode.Tri)
                    {
                        msh.Centre = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        msh.Radius = br.ReadSingle();
                    }

                    switch (msh.Mode)
                    {
                        case TDRMesh.MSHMode.NGon:
                            for (int i = 0; i < msh.FaceCount; i++)
                            {
                                TDRFace face = new TDRFace
                                {
                                    VertexCount = br.ReadInt32(),
                                    Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                                };

                                for (int j = 0; j < face.VertexCount; j++)
                                {
                                    face.Vertices.Add(new TDRVertex
                                    {
                                        Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                        Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                        Colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                        UV = new Vector2(br.ReadSingle(), br.ReadSingle())
                                    });
                                }

                                msh.Faces.Add(face);
                            }
                            break;

                        case TDRMesh.MSHMode.TriIndexedPosition:
                            for (int i = 0; i < msh.VertexCount; i++)
                            {
                                msh.Positions.Add(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            }

                            for (int i = 0; i < msh.FaceCount; i++)
                            {
                                TDRFace face = new TDRFace
                                {
                                    Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                                };

                                TDRVertex v1 = new TDRVertex { PositionIndex = br.ReadInt32() };
                                TDRVertex v2 = new TDRVertex { PositionIndex = br.ReadInt32() };
                                TDRVertex v3 = new TDRVertex { PositionIndex = br.ReadInt32() };

                                v1.Colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                v2.Colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                v3.Colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                                v1.UV = new Vector2(br.ReadSingle(), br.ReadSingle());
                                v2.UV = new Vector2(br.ReadSingle(), br.ReadSingle());
                                v3.UV = new Vector2(br.ReadSingle(), br.ReadSingle());

                                v1.Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                v2.Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                v3.Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                                face.Vertices.Add(v1);
                                face.Vertices.Add(v2);
                                face.Vertices.Add(v3);

                                msh.Faces.Add(face);
                            }

                            //        if (singleMesh)
                            //        {
                            //            for (int i = 0; i < vertCount; i++)
                            //            {
                            //                int pointCount = br.ReadUInt16();

                            //                for (int j = 0; j < pointCount; j++)
                            //                {
                            //                    br.ReadUInt16();
                            //                }
                            //            }
                            //        }
                            break;

                        case TDRMesh.MSHMode.Tri:
                            for (int i = 0; i < msh.VertexCount; i++)
                            {
                                msh.Vertices.Add(new TDRVertex
                                {
                                    Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                    Colour = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                    UV = new Vector2(br.ReadSingle(), br.ReadSingle())
                                });
                            }

                            for (int i = 0; i < msh.FaceCount; i++)
                            {
                                msh.Faces.Add(new TDRFace
                                {
                                    V1 = br.ReadInt32(),
                                    V2 = br.ReadInt32(),
                                    V3 = br.ReadInt32()
                                });
                            }

                            foreach (TDRFace face in msh.Faces)
                            {
                                Vector3 v0 = msh.Vertices[face.V1].Position;
                                Vector3 v1 = msh.Vertices[face.V2].Position;
                                Vector3 v2 = msh.Vertices[face.V3].Position;

                                Vector3 u = v0 - v1;
                                Vector3 v = v0 - v2;

                                face.Normal = Vector3.Cross(u, v).Normalised;

                                msh.Vertices[face.V1].Normal += face.Normal;
                                msh.Vertices[face.V2].Normal += face.Normal;
                                msh.Vertices[face.V3].Normal += face.Normal;
                            }
                            break;
                    }

                    mshs.Meshes.Add(msh);
                }
            }

            return mshs;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream($"{path}{(Meshes.Count > 1 && !Path.GetExtension(path).EndsWith("s") ? "s" : "")}", FileMode.Create)))
            {
                foreach (TDRMesh mesh in Meshes)
                {
                    bw.Write((short)mesh.Faces.Count);
                    bw.Write((short)mesh.Mode);

                    if (mesh.Mode == TDRMesh.MSHMode.Tri) { bw.Write(Vector4.Zero); }

                    switch (mesh.Mode)
                    {
                        case TDRMesh.MSHMode.NGon:
                            bw.Write(mesh.Faces.Sum(f => f.Vertices.Count));
                            bw.Write(mesh.Centre);
                            bw.Write(mesh.Radius);

                            foreach (TDRFace face in mesh.Faces)
                            {
                                bw.Write(face.Vertices.Count);
                                bw.Write(face.Normal);

                                foreach (TDRVertex vert in face.Vertices)
                                {
                                    bw.Write(vert.Position);
                                    bw.Write(vert.Normal);
                                    bw.Write(vert.Colour);
                                    bw.Write(vert.UV);
                                }
                            }
                            break;

                        case TDRMesh.MSHMode.TriIndexedPosition:
                            bw.Write(mesh.Positions.Count);
                            bw.Write(mesh.Centre);
                            bw.Write(mesh.Radius);

                            foreach (Vector3 p in mesh.Positions) { bw.Write(p); }

                            foreach (TDRFace face in mesh.Faces)
                            {
                                bw.Write(face.Normal);

                                bw.Write(face.Vertices[0].PositionIndex);
                                bw.Write(face.Vertices[1].PositionIndex);
                                bw.Write(face.Vertices[2].PositionIndex);

                                bw.Write(face.Vertices[0].Colour);
                                bw.Write(face.Vertices[1].Colour);
                                bw.Write(face.Vertices[2].Colour);

                                bw.Write(face.Vertices[0].UV);
                                bw.Write(face.Vertices[1].UV);
                                bw.Write(face.Vertices[2].UV);

                                bw.Write(face.Vertices[0].Normal);
                                bw.Write(face.Vertices[1].Normal);
                                bw.Write(face.Vertices[2].Normal);
                            }
                            break;

                        case TDRMesh.MSHMode.Tri:
                            bw.Write(mesh.Vertices.Count);

                            foreach (TDRVertex vert in mesh.Vertices)
                            {
                                bw.Write(vert.Position);
                                bw.Write(vert.Colour);
                                bw.Write(vert.UV);
                            }

                            foreach (TDRFace face in mesh.Faces)
                            {
                                bw.Write(face.V1);
                                bw.Write(face.V2);
                                bw.Write(face.V3);
                            }
                            break;
                    }
                }
            }
        }
    }

    public class TDRMesh
    {
        public enum MSHMode
        {
            NGon = 0,
            TriIndexedPosition = 256,
            Tri = 512
        }

        public MSHMode Mode { get; set; }

        public int FaceCount { get; set; }

        public int VertexCount { get; set; }

        public Vector3 Centre { get; set; }

        public float Radius { get; set; }

        public List<Vector3> Positions { get; set; } = new List<Vector3>();

        public List<TDRVertex> Vertices { get; set; } = new List<TDRVertex>();

        public List<TDRFace> Faces { get; set; } = new List<TDRFace>();
    }

    public class TDRVertex
    {
        public int PositionIndex { get; set; } = -1;

        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; } = Vector3.Zero;

        public Vector4 Colour { get; set; }

        public Vector2 UV { get; set; }

        public override string ToString()
        {
            return $"{{ Position: {(PositionIndex > -1 ? $"{PositionIndex}" : $"{{X:{Position.X} Y:{Position.Y} Z:{Position.Z}}}")} Normal: {{X:{Normal.X} Y:{Normal.Y} Z:{Normal.Z}}} UV: {{U:{UV.X} V:{UV.Y}}} Colour: {{R:{Colour.X} G:{Colour.Y} B:{Colour.Z} A:{Colour.W}}} }}";
        }
    }

    public class TDRFace
    {
        public int VertexCount { get; set; }

        public Vector3 Normal { get; set; } = Vector3.Zero;

        public List<TDRVertex> Vertices { get; set; } = new List<TDRVertex>();

        public int V1 { get; set; }

        public int V2 { get; set; }

        public int V3 { get; set; }
    }
}