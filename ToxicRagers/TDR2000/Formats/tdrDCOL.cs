using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class DCOL
    {
        public float Mass { get; set; }
        public Vector3 InertialLength { get; set; }
        public float CoefficientOfRestitution { get; set; }
        public Vector3 CentreOfMass { get; set; }
        public Vector3 BoundingBoxCentre { get; set; }
        public Vector3 BoundingBoxExtents { get; set; }
        public List<DCOLSphere> Spheres { get; set; } = new List<DCOLSphere>();
        public List<Vector3> Vertices { get; set; } = new List<Vector3>();
        public List<DCOLPoly> Polies { get; set; } = new List<DCOLPoly>();
        public List<DCOLEdge> Edges { get; set; } = new List<DCOLEdge>();

        public static DCOL Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            DCOL dcol = new DCOL();

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                dcol.Mass = br.ReadSingle();
                dcol.InertialLength = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                dcol.CoefficientOfRestitution = br.ReadSingle();
                dcol.CentreOfMass = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                dcol.BoundingBoxCentre = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                dcol.BoundingBoxExtents = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                uint sphereCount = br.ReadUInt32();

                for (int i = 0; i < sphereCount; i++)
                {
                    dcol.Spheres.Add(new DCOLSphere
                    {
                        Centre = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                        Radius = br.ReadSingle()
                    });
                }

                uint vertCount = br.ReadUInt32();

                for (int i = 0; i < vertCount; i++)
                {
                    dcol.Vertices.Add(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                }

                uint polygonCount = br.ReadUInt32();

                for (int i = 0; i < polygonCount; i++)
                {
                    DCOLPoly poly = new DCOLPoly
                    {
                        Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                    };
                    
                    uint polyVertCount = br.ReadUInt32();

                    for (int j = 0; j < polyVertCount; j++)
                    {
                        poly.Vertices.Add((int)br.ReadUInt32());
                    }

                    dcol.Polies.Add(poly);
                }

                uint edgeCount = br.ReadUInt32();

                for (int i = 0; i < edgeCount; i++)
                {
                    dcol.Edges.Add(new DCOLEdge
                    {
                        V1 = (int)br.ReadUInt32(),
                        V2 = (int)br.ReadUInt32()
                    });
                }

                for (int i = 0; i < edgeCount; i++)
                {
                    // Average of face normals along shared edge
                    // ((face[x].Normal + face[y].Normal) / 2).Normalised

                    dcol.Edges[i].Normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }
            }

            return dcol;
        }

        public class DCOLEdge
        {
            public int V1 { get; set; }
            public int V2 { get; set; }
            public Vector3 Normal { get; set; }
        }

        public class DCOLPoly
        {
            public Vector3 Normal { get; set; }

            public List<int> Vertices { get; set; } = new List<int>();
        }

        public class DCOLSphere
        {
            public Vector3 Centre { get; set; }
            public float Radius { get; set; }
        }
    }
}
