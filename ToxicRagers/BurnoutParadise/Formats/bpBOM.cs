using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.BurnoutParadise.Formats
{
    public class BOM
    {
        List<int> offsets;
        List<BOMMesh> meshes;
        List<BOMVertex> verts;
        string name;

        public List<BOMMesh> Meshes => meshes;
        public List<BOMVertex> Verts => verts;
        public string Name => name;

        public BOM()
        {
            offsets = new List<int>();
            meshes = new List<BOMMesh>();
            verts = new List<BOMVertex>();
        }

        public static BOM Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            BOM bom = new BOM() { name = Path.GetFileNameWithoutExtension(path) };

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                br.ReadBytes(18);   // Unknown
                int meshCount = br.ReadInt16();
                br.ReadBytes(28);   // Unknown

                for (int i = 0; i < meshCount; i++) { bom.offsets.Add(br.ReadInt32()); }

                for (int i = 0; i < meshCount; i++)
                {
                    br.BaseStream.Seek(bom.offsets[i], SeekOrigin.Begin);
                    BOMMesh mesh = new BOMMesh();

                    br.ReadBytes(76);
                    mesh.VertCount = br.ReadInt32();
                    br.ReadBytes(4);
                    mesh.FaceCount = br.ReadInt32();
                    bom.meshes.Add(mesh);

                    br.ReadBytes(40);
                }

                br.ReadBytes(32);

                for (int i = 0; i < meshCount; i++)
                {
                    br.ReadBytes(80);
                }

                for (int i = 0; i < meshCount; i++)
                {
                    for (int j = 0; j < bom.meshes[i].FaceCount * 3; j++)
                    {
                        bom.meshes[i].IndexBuffer.Add(br.ReadUInt16());
                    }
                }

                br.BaseStream.Seek(16 - (br.BaseStream.Position % 16), SeekOrigin.Current);

                for (int i = 0; i < meshCount; i++)
                {
                    for (int j = 0; j < bom.meshes[i].VertCount; j++)
                    {
                        bom.verts.Add(
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
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
            }
        }
    }

    public class BOMMesh
    {
        List<int> ibo;
        int faceCount;
        int vertCount;

        public int FaceCount
        {
            get => faceCount;
            set => faceCount = value;
        }

        public int VertCount
        {
            get => vertCount;
            set => vertCount = value;
        }

        public List<int> IndexBuffer => ibo;

        public BOMMesh()
        {
            ibo = new List<int>();
        }
    }

    public class BOMVertex
    {
        Vector3 position;
        Vector3 normal;

        public Vector3 Position => position;
        public Vector3 Normal => normal;

        public BOMVertex(float pX, float pY, float pZ, float nX, float nY, float nZ)
        {
            position = new Vector3(pX, pY, pZ);
            normal = new Vector3(nX, nY, nZ);
        }
    }
}