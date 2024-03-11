using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.DestructionDerbyRaw.Formats;
using ToxicRagers.Helpers;

namespace ToxicRagers.GasGuzzlersExtreme.Formats
{
    public class PEMB
    {
        public List<PEMBMesh> Meshes { get; set; } = new();

        public static PEMB Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            PEMB pemb = new();

            using (BinaryReader br = new(fi.OpenRead()))
            {
                int a = br.ReadInt32();
                int meshCount = br.ReadInt32();
                int c = br.ReadInt32();
                int d = br.ReadInt32();

                for (int x = 0; x < meshCount; x++)
                {
                    PEMBMesh mesh = new();

                    uint vertCount = br.ReadUInt32();
                    uint faceCount = br.ReadUInt32();
                    uint uvSets = br.ReadUInt32();
                    int f = br.ReadInt32();
                    int g = br.ReadInt32();
                    int h = br.ReadInt32();
                    int i = br.ReadInt32();
                    float _a = br.ReadSingle();
                    int j = br.ReadInt32();
                    float _b = br.ReadSingle();
                    float _c = br.ReadSingle();
                    float _d = br.ReadSingle();
                    float _e = br.ReadSingle();
                    float _f = br.ReadSingle();
                    byte a_ = br.ReadByte();
                    byte b_ = br.ReadByte();
                    byte c_ = br.ReadByte();

                    for (int y = 0; y < vertCount; y++)
                    {
                        mesh.Vertices.Add(new Vector3
                        {
                            X = br.ReadSingle(),
                            Y = br.ReadSingle(),
                            Z = br.ReadSingle()
                        });
                    }

                    for (int y = 0; y < vertCount; y++) 
                    {
                        br.ReadBytes(36);   // ?
                    }

                    for (int y = 0; y < faceCount; y++)
                    {
                        mesh.Faces.Add(new PEMBFace
                        {
                            V1 = br.ReadInt32(),
                            V2 = br.ReadInt32(),
                            V3 = br.ReadInt32()
                        });
                    }

                    for (int z = 0; z < uvSets; z++)
                    {
                        PEMBUVSet uvSet = new();

                        for (int y = 0; y < vertCount; y++)
                        {
                            uvSet.UVs.Add(new Vector2
                            {
                                X = br.ReadSingle(),
                                Y = br.ReadSingle()
                            });
                        }

                        mesh.UVSets.Add(uvSet);
                    }

                    pemb.Meshes.Add(mesh);
                }
            }

            return pemb;
        }
    }

    public class PEMBMesh
    {
        public List<Vector3> Vertices { get; set; } = new();

        public List<PEMBFace> Faces { get; set; } = new();

        public List<PEMBUVSet> UVSets { get; set; } = new();
    }

    public class PEMBFace
    {
        public int V1 { get; set; }

        public int V2 { get; set; }

        public int V3 { get; set; }
    }

    public class PEMBUVSet
    {
        public List<Vector2> UVs { get; set; } = new();
    }
}
