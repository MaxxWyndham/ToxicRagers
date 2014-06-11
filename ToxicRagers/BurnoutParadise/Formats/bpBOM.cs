using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.BurnoutParadise.Formats
{
    public class BOM
    {
        List<BOMFace> faces;
        List<BOMVertex> verts;
        string name;
        int faceCount;

        public string Name { get { return name; } }
        public List<BOMFace> Faces { get { return faces; } }
        public List<BOMVertex> Verts { get { return verts; } }

        public BOM()
        {
            faces = new List<BOMFace>();
            verts = new List<BOMVertex>();
        }

        public static BOM Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            BOM bom = new BOM();

            bom.name = Path.GetFileNameWithoutExtension(path);

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                br.BaseStream.Seek(0xB4, SeekOrigin.Begin);
                bom.faceCount = (int)br.ReadInt32();

                br.BaseStream.Seek(0x200, SeekOrigin.Begin);
                for (int i = 0; i < bom.faceCount; i++)
                {
                    bom.faces.Add(new BOMFace(br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16()));
                }

                br.BaseStream.Seek(0x4500, SeekOrigin.Begin);
                while (br.BaseStream.Position + 52 < br.BaseStream.Length)
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
                    br.ReadBytes(28);
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

    public class BOMFace
    {
        int vertexA;
        int vertexB;
        int vertexC;

        public int V1 { get { return vertexA; } }
        public int V2 { get { return vertexB; } }
        public int V3 { get { return vertexC; } }

        public BOMFace(int A, int B, int C)
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

    public class BOMVertex
    {
        Vector3 position;
        Vector3 normal;

        public Vector3 Position { get { return position; } }
        public Vector3 Normal { get { return normal; } }

        public BOMVertex(float pX, float pY, float pZ, float nX, float nY, float nZ)
        {
            this.position = new Vector3(pX, pY, pZ);
            this.normal = new Vector3(nX, nY, nZ);
        }
    }
}
