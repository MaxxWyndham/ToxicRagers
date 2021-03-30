using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.BurnoutParadise.Formats
{
    public class BOM
    {
        public List<BOMMesh> Meshes { get; set; } = new List<BOMMesh>();

        public List<BOMVertex> Verts { get; set; } = new List<BOMVertex>();

        public List<int> Offsets { get; set; } = new List<int>();

        public string Name { get; set; }

        public static BOM Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            BOM bom = new BOM { Name = Path.GetFileNameWithoutExtension(path) };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                br.ReadBytes(18);   // Unknown
                int meshCount = br.ReadInt16();
                br.ReadBytes(28);   // Unknown

                for (int i = 0; i < meshCount; i++) { bom.Offsets.Add(br.ReadInt32()); }

                for (int i = 0; i < meshCount; i++)
                {
                    br.BaseStream.Seek(bom.Offsets[i], SeekOrigin.Begin);
                    BOMMesh mesh = new BOMMesh();

                    br.ReadBytes(76);
                    mesh.VertCount = br.ReadInt32();
                    br.ReadBytes(4);
                    mesh.FaceCount = br.ReadInt32();
                    bom.Meshes.Add(mesh);

                    br.ReadBytes(40);
                }

                br.ReadBytes(32);

                for (int i = 0; i < meshCount; i++)
                {
                    br.ReadBytes(80);
                }

                for (int i = 0; i < meshCount; i++)
                {
                    for (int j = 0; j < bom.Meshes[i].FaceCount * 3; j++)
                    {
                        bom.Meshes[i].IndexBuffer.Add(br.ReadUInt16());
                    }
                }

                br.BaseStream.Seek(16 - (br.BaseStream.Position % 16), SeekOrigin.Current);

                for (int i = 0; i < meshCount; i++)
                {
                    for (int j = 0; j < bom.Meshes[i].VertCount; j++)
                    {
                        bom.Verts.Add(
                            new BOMVertex(
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                            )
                        );
                        //Console.WriteLine("X {0} Y {1} Z {2}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        //Console.WriteLine("U {0} V {1}", br.ReadSingle(), br.ReadSingle());
                        //Console.WriteLine("A {0} B {1}", br.ReadInt32(), br.ReadInt32());
                        br.ReadBytes((i == 0 ? 36 : 28));
                    }

                    if (i == 0) { br.ReadBytes(12); }
                }
            }

            return bom;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
            }
        }
    }

    public class BOMMesh
    {
        public int FaceCount { get; set; }

        public int VertCount { get; set; }

        public List<int> IndexBuffer { get; set; } = new List<int>();
    }

    public class BOMVertex
    {
        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; }

        public BOMVertex(float pX, float pY, float pZ, float nX, float nY, float nZ)
        {
            Position = new Vector3(pX, pY, pZ);
            Normal = new Vector3(nX, nY, nZ);
        }
    }
}