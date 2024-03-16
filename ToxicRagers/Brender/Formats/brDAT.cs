using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Brender.Formats
{
    public class DAT
    {
        public List<DatMesh> DatMeshes { get; set; } = new List<DatMesh>();

        public static DAT Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            DAT dat = new();

            DatMesh mesh = new();
            int count;

            using (BEBinaryReader br = new(fi.OpenRead(), Encoding.Default))
            {
                if (br.ReadUInt32() != 0x12 ||
                    br.ReadUInt32() != 0x8 ||
                    br.ReadUInt32() != 0xFACE ||
                    br.ReadUInt32() != 0x2)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid DAT file", path);
                    return null;
                }

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    ChunkId tag = (ChunkId)br.ReadUInt32();
                    int _ = (int)br.ReadUInt32();

                    switch (tag)
                    {
                        // 00 00 00 36
                        case ChunkId.ModelOld2:
                            mesh = new DatMesh()
                            {
                                Flags = br.ReadUInt16(),
                                Name = br.ReadString()
                            };
                            break;

                        // 00 00 00 17
                        case ChunkId.Vertices:
                            count = (int)br.ReadUInt32();

                            for (int i = 0; i < count; i++)
                            {
                                mesh.Vertices.Add(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            }
                            break;

                        // 00 00 00 18
                        case ChunkId.UVs:
                            count = (int)br.ReadUInt32();

                            for (int i = 0; i < count; i++)
                            {
                                mesh.UVs.Add(new Vector2(br.ReadSingle(), br.ReadSingle()));
                            }
                            break;

                        // 00 00 00 35
                        case ChunkId.Faces:
                            count = (int)br.ReadUInt32();

                            for (int i = 0; i < count; i++)
                            {
                                mesh.Faces.Add(new DatFace
                                {
                                    V1 = br.ReadUInt16(),
                                    V2 = br.ReadUInt16(),
                                    V3 = br.ReadUInt16(),
                                    SmoothingGroup = br.ReadUInt16(),
                                    Flags = br.ReadByte()
                                });
                            }
                            break;

                        // 00 00 00 16
                        case ChunkId.Materials:
                            mesh.Materials.AddRange(br.ReadStrings((int)br.ReadUInt32()));
                            break;

                        // 00 00 00 1A
                        case ChunkId.FaceMaterial:
                            count = (int)br.ReadUInt32();
                            br.ReadBytes(4);

                            for (int i = 0; i < count; i++)
                            {
                                mesh.Faces[i].MaterialId = br.ReadUInt16();
                            }
                            break;

                        case ChunkId.EOF:
                            dat.DatMeshes.Add(mesh);
                            break;

                        default:
                            Logger.LogToFile(Logger.LogLevel.Error, "Unknown ChunkId: {0} ({1:x2})", tag, br.BaseStream.Position);
                            return null;
                    }
                }
            }

            return dat;
        }

        public void Save(string path)
        {
            using FileStream fs = new(path, FileMode.Create);
            using BEBinaryWriter bw = new(fs);
            int matListLength;
            string name;

            //output header
            bw.WriteInt32(0x12);
            bw.WriteInt32(0x8);
            bw.WriteInt32(0xface);
            bw.WriteInt32(0x2);

            for (int i = 0; i < DatMeshes.Count; i++)
            {
                DatMesh dm = DatMeshes[i];
                matListLength = 0;

                for (int j = 0; j < dm.Materials.Count; j++)
                {
                    matListLength += dm.Materials[j].Length + 1;
                }

                name = dm.Name;

                // begin name section
                // 00 00 00 36
                bw.WriteInt32((int)ChunkId.ModelOld2);
                bw.WriteInt32(name.Length + 3);
                bw.WriteInt16(dm.Flags);
                bw.Write(name.ToCharArray());
                bw.WriteByte(0);
                // end name section

                // begin vertex data
                // 00 00 00 17
                bw.WriteInt32((int)ChunkId.Vertices);
                bw.WriteInt32(dm.Vertices.Count * 12 + 4);
                bw.WriteInt32(dm.Vertices.Count);

                for (int j = 0; j < dm.Vertices.Count; j++)
                {
                    bw.WriteSingle(dm.Vertices[j].X);
                    bw.WriteSingle(dm.Vertices[j].Y);
                    bw.WriteSingle(dm.Vertices[j].Z);
                }
                // end vertex data

                // begin uv data
                // 00 00 00 18
                bw.WriteInt32((int)ChunkId.UVs);
                bw.WriteInt32(dm.UVs.Count * 8 + 4);
                bw.WriteInt32(dm.UVs.Count);

                for (int j = 0; j < dm.UVs.Count; j++)
                {
                    bw.WriteSingle(dm.UVs[j].X);
                    bw.WriteSingle(dm.UVs[j].Y);
                }
                // end uv data

                // begin face data
                // 00 00 00 35
                bw.WriteInt32((int)ChunkId.Faces);
                bw.WriteInt32(dm.Faces.Count * 9 + 4);
                bw.WriteInt32(dm.Faces.Count);

                for (int j = 0; j < dm.Faces.Count; j++)
                {
                    bw.WriteInt16(dm.Faces[j].V1);
                    bw.WriteInt16(dm.Faces[j].V2);
                    bw.WriteInt16(dm.Faces[j].V3);
                    bw.WriteInt16(dm.Faces[j].SmoothingGroup);
                    bw.WriteByte(dm.Faces[j].Flags);
                }
                // end face data

                // begin material list
                // 00 00 00 16
                bw.WriteInt32((int)ChunkId.Materials);
                bw.WriteInt32(matListLength + 4);
                bw.WriteInt32(dm.Materials.Count);

                for (int j = 0; j < dm.Materials.Count; j++)
                {
                    bw.Write(dm.Materials[j].ToCharArray());
                    bw.WriteByte(0);
                }
                // end material list

                // begin face textures
                // 00 00 00 1A
                bw.WriteInt32((int)ChunkId.FaceMaterial);
                bw.WriteInt32(dm.Faces.Count * 2 + 4);
                bw.WriteInt32(dm.Faces.Count);
                bw.WriteInt32(2);

                for (int j = 0; j < dm.Faces.Count; j++)
                {
                    bw.WriteInt16(dm.Faces[j].MaterialId);
                }
                // end face textures

                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }
        }
    }

    public class DatMesh
    {
        public string Name { get; set; } = "";

        public ushort Flags { get; set; } = 0;

        public List<string> Materials { get; set; } = new List<string>();

        public List<Vector3> Vertices { get; set; } = new List<Vector3>();

        public List<Vector2> UVs { get; set; } = new List<Vector2>();

        public List<DatFace> Faces { get; set; } = new List<DatFace>();

        public MeshExtents Extents { get; set; }
    }

    public class DatFace
    {
        public int V1 { get; set; }

        public int V2 { get; set; }

        public int V3 { get; set; }

        public int SmoothingGroup { get; set; }

        // MaterialId 0 has no material assigned
        public int MaterialId { get; set; }

        public byte Flags { get; set; }
    }
}