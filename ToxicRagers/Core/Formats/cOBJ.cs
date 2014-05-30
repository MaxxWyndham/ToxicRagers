using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    public class OBJ
    {
        string materials = null;
        List<OBJMesh> meshes = new List<OBJMesh>();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> norms = new List<Vector3>();

        public List<OBJMesh> Meshes { get { return meshes; } }
        public List<Vector3> Vertices { get { return verts; } }
        public List<Vector3> Normals { get { return norms; } }
        public List<Vector2> UVs { get { return uvs; } }

        public static OBJ Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            OBJ obj = new OBJ();

            OBJMesh currentMesh = null;
            string material = null;
            int smoothingGroup = -1;

            using (var sr = new StreamReader(fi.OpenRead()))
            {
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    switch (line[0])
                    {
                        case "#": // Comment
                            break;

                        case "mtllib": // Material library
                            obj.materials = line[1];
                            break;

                        case "o":
                        case "g":
                            if (currentMesh != null) { obj.meshes.Add(currentMesh); }
                            currentMesh = new OBJMesh(line[1]);
                            break;

                        case "v":
                            obj.verts.Add(new Vector3(Convert.ToSingle(line[1], ToxicRagers.Culture), Convert.ToSingle(line[2], ToxicRagers.Culture), Convert.ToSingle(line[3], ToxicRagers.Culture)));
                            break;

                        case "vn":
                            obj.norms.Add(new Vector3(Convert.ToSingle(line[1], ToxicRagers.Culture), Convert.ToSingle(line[2], ToxicRagers.Culture), Convert.ToSingle(line[3], ToxicRagers.Culture)));
                            break;

                        case "vt":
                            obj.uvs.Add(new Vector2(Convert.ToSingle(line[1], ToxicRagers.Culture), Convert.ToSingle(line[2], ToxicRagers.Culture)));
                            break;

                        case "usemtl":
                            material = line[1];
                            break;

                        case "s":
                            smoothingGroup = (line[1] == "off" ? 0 : Convert.ToInt32(line[1]));
                            break;

                        case "f":
                            currentMesh.Faces.Add(new OBJFace(new ArraySegment<string>(line, 1, line.Length - 1).Array, material, smoothingGroup));
                            break;

                        default:
                            Console.WriteLine("Unknown OBJ entry: {0}", line[0]);
                            break;
                    }
                }
            }

            if (currentMesh != null) { obj.meshes.Add(currentMesh); }
            return obj;
        }

        public void Save(string path)
        {
            using (var sw = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                sw.WriteLine("# Exported from Flummery");
                sw.WriteLine("# www.toxic-ragers.co.uk");
                if (materials != null) { sw.WriteLine(string.Format("mtllib {0}", this.materials)); }

                foreach (var o in this.meshes)
                {
                    sw.WriteLine("o {0}", o.Name);

                }
            }
        }
    }

    public class OBJMesh
    {
        string name;
        List<OBJFace> faces;

        public string Name { get { return name; } }
        public List<OBJFace> Faces { get { return faces; } }

        public OBJMesh(string name)
        {
            faces = new List<OBJFace>();
            this.name = name;
        }
    }

    public class OBJFace
    {
        string material;
        int smoothingGroup;
        List<OBJPoint> points;

        public OBJFace(string[] points, string materialName, int smoothingGroup)
        {
            this.points = new List<OBJPoint>();

            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i].Split('/');

                this.points.Add(
                    new OBJPoint(
                        Convert.ToInt32(p[0]) - 1,
                        Convert.ToInt32(p[1]) - 1,
                        (p.Length == 3 ? Convert.ToInt32(p[2]) - 1 : -1)
                    )
                );
            }

            this.material = materialName;
            this.smoothingGroup = smoothingGroup;
        }
    }

    public class OBJPoint
    {
        int vertex;
        int uv;
        int normal;

        public int Vertex { get { return vertex; } }
        public int UV { get { return uv; } }
        public int Normal { get { return normal; } }

        public OBJPoint(int vertex, int uv, int normal = -1)
        {
            this.vertex = vertex;
            this.uv = uv;
            this.normal = normal;
        }
    }
}
