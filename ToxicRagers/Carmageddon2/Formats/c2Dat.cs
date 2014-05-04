using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToxicRagers.Helpers;
using ToxicRagers.Carmageddon2.Helpers;

namespace ToxicRagers.Carmageddon2.Formats
{
    public class DAT
    {
        public List<DatMesh> DatMeshes;

        public DAT()
        {
            DatMeshes = new List<DatMesh>();
        }

        public DAT(DatMesh dm)
        {
            DatMeshes = new List<DatMesh>();
            DatMeshes.Add(dm);
        }

        public static DAT Load(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            DAT dat = new DAT();

            DatMesh D = new DatMesh();
            int count;

            using (BEBinaryReader br = new BEBinaryReader(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadUInt32() != 18 ||
                    br.ReadUInt32() != 8 ||
                    br.ReadUInt32() != 64206 ||
                    br.ReadUInt32() != 2)
                {
                    Logger.LogToFile("{0} isn't a valid DAT file", Path);
                    return null;
                }

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    int tag = (int)br.ReadUInt32();
                    int length = (int)br.ReadUInt32();

                    switch (tag)
                    {
                        case 54: // 00 00 00 36
                            D = new DatMesh();
                            D.UnknownAttribute = br.ReadUInt16();   // I think this is actually two byte values
                            D.Name = br.ReadString();
                            break;

                        case 23: // 00 00 00 17 : vertex data
                            count = (int)br.ReadUInt32();

                            for (int i = 0; i < count; i++)
                            {
                                D.Mesh.AddListVertex(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            }
                            break;

                        case 24: // 00 00 00 18 : UV co-ordinates
                            count = (int)br.ReadUInt32();

                            for (int i = 0; i < count; i++)
                            {
                                D.Mesh.AddListUV(br.ReadSingle(), br.ReadSingle());
                            }

                            D.Mesh.AssignUVs();
                            break;

                        case 53:    // 00 00 00 35 : faces
                            count = (int)br.ReadUInt32();

                            for (int i = 0; i < count; i++)
                            {
                                D.Mesh.AddFace(br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16());
                                br.ReadByte(); // smoothing groups 9 - 16
                                br.ReadByte(); // smoothing groups 1 - 8
                                br.ReadByte(); // number of edges, 0 and 3 = tri.  4 = quad.
                            }
                            break;

                        case 22: // 00 00 00 16 : material list
                            D.Mesh.Materials.AddRange(br.ReadStrings((int)br.ReadUInt32()));
                            break;

                        case 26: // 00 00 00 1A : face textures
                            count = (int)br.ReadUInt32();
                            br.ReadBytes(4); // fuck knows what this is
                            for (int i = 0; i < count; i++)
                            {
                                D.Mesh.SetMaterialForFace(i, br.ReadUInt16() - 1);
                            }
                            break;

                        case 0:
                            // EndOfFile
                            D.Mesh.ProcessMesh();
                            dat.DatMeshes.Add(D);
                            break;

                        default:
                            Logger.LogToFile("Unknown DAT tag: {0} ({1})", tag, br.BaseStream.Position.ToString("X"));
                            return null;
                    }
                }
            }

            return dat;
        }

        public void Save(string Path)
        {
            BEBinaryWriter bw = new BEBinaryWriter(new FileStream(Path, FileMode.Create));
            int iMatListLength;
            string name;

            //output header
            bw.WriteInt32(18);
            bw.WriteInt32(8);
            bw.WriteInt32(64206);
            bw.WriteInt32(2);

            for (int i = 0; i < DatMeshes.Count; i++)
            {
                DatMesh dm = DatMeshes[i];
                iMatListLength = 0;

                for (int j = 0; j < dm.Mesh.Materials.Count; j++)
                {
                    iMatListLength += dm.Mesh.Materials[j].Length + 1;
                }

                name = dm.Name;
                //Console.WriteLine(name + " : " + dm.Mesh.Verts.Count);

                //begin name section
                bw.WriteInt32(54);
                bw.WriteInt32(name.Length + 3);
                bw.WriteByte(0);
                bw.WriteByte(0);
                bw.Write(name.ToCharArray());
                bw.WriteByte(0);
                //end name section

                //begin vertex data
                bw.WriteInt32(23);
                bw.WriteInt32((dm.Mesh.Verts.Count * 12) + 4);
                bw.WriteInt32(dm.Mesh.Verts.Count);

                for (int j = 0; j < dm.Mesh.Verts.Count; j++)
                {
                    bw.WriteSingle(dm.Mesh.Verts[j].X);
                    bw.WriteSingle(dm.Mesh.Verts[j].Y);
                    bw.WriteSingle(dm.Mesh.Verts[j].Z);
                }
                //end vertex data

                //begin uv data (00 00 00 18)
                bw.WriteInt32(24);
                bw.WriteInt32((dm.Mesh.UVs.Count * 8) + 4);
                bw.WriteInt32(dm.Mesh.UVs.Count);

                for (int j = 0; j < dm.Mesh.UVs.Count; j++)
                {
                    bw.WriteSingle(dm.Mesh.UVs[j].X);
                    bw.WriteSingle(dm.Mesh.UVs[j].Y);
                }
                //end uv data

                //begin face data (00 00 00 35)
                bw.WriteInt32(53);
                bw.WriteInt32((dm.Mesh.Faces.Count * 9) + 4);
                bw.WriteInt32(dm.Mesh.Faces.Count);

                for (int j = 0; j < dm.Mesh.Faces.Count; j++)
                {
                    bw.WriteInt16(dm.Mesh.Faces[j].V1);
                    bw.WriteInt16(dm.Mesh.Faces[j].V2);
                    bw.WriteInt16(dm.Mesh.Faces[j].V3);
                    bw.WriteByte(0); // smoothing groups 9 - 16
                    bw.WriteByte(1);   // smoothing groups 1 - 8
                    bw.WriteByte(0);   // number of edges, 0 and 3 = tri.  4 = quad.
                }
                //end face data

                //begin material list
                bw.WriteInt32(22);
                bw.WriteInt32(iMatListLength + 4);
                bw.WriteInt32(dm.Mesh.Materials.Count);

                for (int j = 0; j < dm.Mesh.Materials.Count; j++)
                {
                    bw.Write(dm.Mesh.Materials[j].ToCharArray());
                    bw.WriteByte(0);
                }
                //end material list

                //begin face textures
                bw.WriteInt32(26);
                bw.WriteInt32((dm.Mesh.Faces.Count * 2) + 4);
                bw.WriteInt32(dm.Mesh.Faces.Count);
                bw.WriteInt32(2);

                for (int j = 0; j < dm.Mesh.Faces.Count; j++)
                {
                    bw.WriteInt16(dm.Mesh.Faces[j].MaterialID + 1);
                }
                //end face textures

                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }

            bw.Close();
        }

        public void AddMesh(string Name, byte Flag, c2Mesh Mesh)
        {
            DatMesh d = new DatMesh();
            d.Name = Name;
            d.UnknownAttribute = Flag;
            d.Mesh = Mesh;
            DatMeshes.Add(d);
        }

        // aggregate functions, apply to all submeshes
        #region Aggregate functions
        public MeshExtents Extents
        {
            get
            {
                Vector3 min, max;
                min = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
                max = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);

                foreach (DatMesh d in DatMeshes)
                {
                    d.Mesh.ProcessMesh();

                    if (min.X > d.Mesh.Extents.Min.X) { min.X = d.Mesh.Extents.Min.X; }
                    if (min.Y > d.Mesh.Extents.Min.Y) { min.Y = d.Mesh.Extents.Min.Y; }
                    if (min.Z > d.Mesh.Extents.Min.Z) { min.Z = d.Mesh.Extents.Min.Z; }

                    if (max.X < d.Mesh.Extents.Max.X) { max.X = d.Mesh.Extents.Max.X; }
                    if (max.Y < d.Mesh.Extents.Max.Y) { max.Y = d.Mesh.Extents.Max.Y; }
                    if (max.Z < d.Mesh.Extents.Max.Z) { max.Z = d.Mesh.Extents.Max.Z; }
                }

                return new MeshExtents(min, max);
            }
        }

        public void Optimise()
        {
            foreach (DatMesh d in DatMeshes)
            {
                d.Mesh.Optimise();
            }
        }

        public void CentreOn(Single x, Single y, Single z)
        {
            MeshExtents extents = Extents;
            Vector3 offset = (extents.Min + extents.Max) / 2;

            Console.WriteLine(offset);

            foreach (DatMesh d in DatMeshes)
            {
                d.Mesh.Translate(-offset);
                d.Mesh.ProcessMesh();
            }
        }

        public void Scale(Single scale)
        {
            foreach (DatMesh d in DatMeshes)
            {
                d.Mesh.Scale(scale);
                d.Mesh.ProcessMesh();
            }
        }
        #endregion
    }

    public class DatMesh
    {
        #region Variables
        private string _name = "";
        private int _attribUnknown = 0;
        private c2Mesh _mesh = new c2Mesh();
        #endregion

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int UnknownAttribute
        {
            get { return _attribUnknown; }
            set { _attribUnknown = value; }
        }

        public c2Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public DatMesh() { }

        public DatMesh(string Name, c2Mesh m)
        {
            _name = Name;
            _mesh = m;
        }
    }
}
