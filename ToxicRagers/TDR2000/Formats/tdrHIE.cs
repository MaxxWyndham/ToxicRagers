using System.Globalization;

using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class HIE
    {
        public static CultureInfo Culture = new CultureInfo("en-GB");

        public int Version { get; set; }

        public int CollisionDataMeshCount { get; set; }

        public List<string> CollisionDataMeshes { get; set; } = new List<string>();

        public int CullNodeCount { get; set; }

        public int LineCount { get; set; }

        public List<string> LineNames { get; set; } = new List<string>();

        public int TextureCount { get; set; }

        public List<string> Textures { get; set; } = new List<string>();

        public int MaterialCount { get; set; }

        public List<TDRMaterial> Materials { get; set; } = new List<TDRMaterial>();

        public int MatrixCount { get; set; }

        public List<TDRMatrix> Matrixes { get; set; } = new List<TDRMatrix>();

        public int MeshCount { get; set; }

        public List<string> Meshes { get; set; } = new List<string>();

        public int ExpressionCount { get; set; }

        public List<Vector2> Expressions { get; set; } = new List<Vector2>();

        public int NodeCount { get; set; }

        public List<TDRNode> Nodes { get; set; } = new List<TDRNode>();

        public TDRNode Root => Nodes[0];

        public static HIE Load(string path)
        {
            return Load(path, null);
        }

        public static HIE Load(string path, string hFilePath = null)
        {
            void walkHierarchy(TDRNode parent, int index, HIE hierarchy)
            {
                TDRNode node = hierarchy.Nodes[index];

                switch (node.Type)
                {
                    case TDRNode.NodeType.Matrix:
                        node.Transform = hierarchy.Matrixes[node.Index].Matrix;
                        break;
                }

                if (node.Parent != null) { parent.Children.Remove(node); }

                node.Parent = parent;
                parent.Children.Add(node);

                if (node.Child > -1) { walkHierarchy(node, node.Child, hierarchy); }
                if (node.Sibling > -1) { walkHierarchy(parent, node.Sibling, hierarchy); }
            }

            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            HIE hie = new HIE();
            H h = new H();
            if (hFilePath == null)
            {
                FileInfo[] f = fi.Directory.GetFiles($"*.h");
                if (f.Length == 0)
                {
                    f = fi.Directory.Parent.GetFiles($"{Path.GetFileNameWithoutExtension(path)}.h");
                }

                if (f.Length > 0)
                {
                    h = H.Load(f[0].FullName);
                }
            }
            else
            {
                h = H.Load(hFilePath);
            }

            string[] lines;

            using (StreamReader sr = new StreamReader(fi.OpenRead())) { lines = sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
            lines = lines.Select(l => l.Trim()).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                switch (lines[i].ToLower())
                {
                    case "//version number":
                        hie.Version = int.Parse(lines[i + 2]);
                        i += 2;
                        break;

                    case "// number of cull nodes":
                        hie.CullNodeCount = int.Parse(lines[++i]);
                        break;

                    case "// cull node list":
                        for (int j = 0; j < hie.CullNodeCount; j++)
                        {
                            string cullNode = lines[++i];
                        }
                        break;

                    case "// number of collision data meshes":
                        hie.CollisionDataMeshCount = int.Parse(lines[++i]);
                        break;

                    case "// collision data list":
                        for (int j = 0; j < hie.CollisionDataMeshCount; j++)
                        {
                            hie.CollisionDataMeshes.Add(lines[++i]);
                        }
                        break;

                    case "// number of lines":
                        hie.LineCount = int.Parse(lines[++i]);
                        break;

                    case "// line name list":
                        while (!lines[++i].StartsWith("//"))
                        {
                            hie.LineNames.Add(lines[i].Trim().Replace("\"", ""));
                        }

                        i--;
                        break;

                    case "// number of textures":
                        hie.TextureCount = int.Parse(lines[++i]);
                        break;

                    case "// texture name list":
                        for (int j = 0; j < hie.TextureCount; j++)
                        {
                            hie.Textures.Add(lines[++i].Replace("\"", "").Trim());
                        }
                        break;

                    case "// number of materials":
                        hie.MaterialCount = int.Parse(lines[++i]);
                        break;

                    case "// material name list":
                        for (int j = 0; j < hie.MaterialCount; j++)
                        {
                            hie.Materials.Add(new TDRMaterial(lines[++i]));
                        }
                        break;

                    case "// number of matrices":
                        hie.MatrixCount = int.Parse(lines[++i]);
                        break;

                    case "// matrix name list":
                        for (int j = 0; j < hie.MatrixCount; j++)
                        {
                            string[] parts;
                            TDRMatrix matrix = new TDRMatrix();

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M11 = Convert.ToSingle(parts[0], Culture);
                            matrix.Matrix.M12 = Convert.ToSingle(parts[1], Culture);
                            matrix.Matrix.M13 = Convert.ToSingle(parts[2], Culture);

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M21 = Convert.ToSingle(parts[0], Culture);
                            matrix.Matrix.M22 = Convert.ToSingle(parts[1], Culture);
                            matrix.Matrix.M23 = Convert.ToSingle(parts[2], Culture);

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M31 = Convert.ToSingle(parts[0], Culture);
                            matrix.Matrix.M32 = Convert.ToSingle(parts[1], Culture);
                            matrix.Matrix.M33 = Convert.ToSingle(parts[2], Culture);

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M41 = Convert.ToSingle(parts[0], Culture);
                            matrix.Matrix.M42 = Convert.ToSingle(parts[1], Culture);
                            matrix.Matrix.M43 = Convert.ToSingle(parts[2], Culture);

                            matrix.Name = lines[++i].Trim().Replace("\"", "");

                            hie.Matrixes.Add(matrix);
                        }
                        break;

                    case "// number of meshes":
                        hie.MeshCount = int.Parse(lines[++i]);
                        break;

                    case "// mesh name list":
                        while (!lines[++i].StartsWith("//"))
                        {
                            hie.Meshes.Add(lines[i].Trim().Replace("\"", ""));
                        }

                        i--;
                        break;

                    case "// number of expressions":
                        hie.ExpressionCount = int.Parse(lines[++i]);
                        break;

                    case "// expression list :":
                        if (lines[i + 1].ToLower() == "// high_val  low_val") { i++; }
                        for (int j = 0; j < hie.ExpressionCount; j++)
                        {
                            string[] p = lines[++i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            hie.Expressions.Add(new Vector2(p[0].ToSingle(), p[1].ToSingle()));
                        }
                        break;

                    case "// number of nodes":
                        hie.NodeCount = int.Parse(lines[++i]);
                        break;

                    case "// node list :":
                        if (lines[i + 1].ToLower() == "// type  index  child  sibling") { i++; }
                        for (int j = 0; j < hie.NodeCount; j++)
                        {
                            hie.Nodes.Add(new TDRNode((h.Definitions.ContainsKey(j) ? h.Definitions[j] : "DEFAULT"), j, lines[++i]));
                        }
                        break;

                    default:
                        Console.WriteLine(i + "] " + lines[i]);
                        break;
                }
            }

            hie.Root.Name = h.Definitions.ContainsKey(0) ? h.Definitions[0] : "DEFAULT";
            hie.Root.Transform = hie.Matrixes[hie.Root.Index].Matrix;

            walkHierarchy(hie.Root, 1, hie);

            return hie;
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine();

                if (Version == 2)
                {
                    sw.WriteLine("//Version Number");
                    sw.WriteLine();
                    sw.WriteLine("\"Version\"");
                    sw.WriteLine();
                    sw.WriteLine(Version);
                    sw.WriteLine();
                }

                sw.WriteLine();

                if (Version == 2)
                {
                    sw.WriteLine("// Number of Collision Data meshes");
                    sw.WriteLine();
                    sw.WriteLine($"    {CollisionDataMeshCount}");
                    sw.WriteLine();

                    sw.WriteLine("// Collision Data list");
                    sw.WriteLine();
                    foreach (string collisionDataMesh in CollisionDataMeshes)
                    {
                        sw.WriteLine($"    {collisionDataMesh}");
                    }
                    sw.WriteLine();
                }

                sw.WriteLine("// Number of lines");
                sw.WriteLine();
                sw.WriteLine($"    {LineCount}");
                sw.WriteLine();

                sw.WriteLine("// Line name list");
                sw.WriteLine();
                foreach (string lineName in LineNames)
                {
                    sw.WriteLine($"    {lineName}");
                }
                sw.WriteLine();

                sw.WriteLine("// Number of textures");
                sw.WriteLine();
                sw.WriteLine($"    {TextureCount}");
                sw.WriteLine();

                sw.WriteLine("// Texture name list");
                sw.WriteLine();
                foreach (string texture in Textures)
                {
                    sw.WriteLine($"    \"{texture}\"");
                }
                sw.WriteLine();

                sw.WriteLine("// Number of materials");
                sw.WriteLine();
                sw.WriteLine($"    {MaterialCount}");
                sw.WriteLine();

                sw.WriteLine("// Material name list");
                sw.WriteLine();
                foreach (TDRMaterial material in Materials)
                {
                    sw.WriteLine($"    {material.V.X:0.000000}     {material.V.Y:0.000000}    {material.V.Z:0.000000}    {material.V.W:0.000000}    {material.I}");
                }
                sw.WriteLine();

                sw.WriteLine("// Number of matrices");
                sw.WriteLine();
                sw.WriteLine($"    {MatrixCount}");
                sw.WriteLine();

                sw.WriteLine("// Matrix name list");
                sw.WriteLine();
                foreach (TDRMatrix matrix in Matrixes)
                {
                    sw.WriteLine($"    {matrix.Matrix.M11:0.000000}    {matrix.Matrix.M12:0.000000}    {matrix.Matrix.M13:0.000000}    {matrix.Matrix.M14:0.000000};");
                    sw.WriteLine($"    {matrix.Matrix.M21:0.000000}    {matrix.Matrix.M22:0.000000}    {matrix.Matrix.M23:0.000000}    {matrix.Matrix.M24:0.000000};");
                    sw.WriteLine($"    {matrix.Matrix.M31:0.000000}    {matrix.Matrix.M32:0.000000}    {matrix.Matrix.M33:0.000000}    {matrix.Matrix.M34:0.000000};");
                    sw.WriteLine($"    {matrix.Matrix.M41:0.000000}    {matrix.Matrix.M42:0.000000}    {matrix.Matrix.M43:0.000000}    {matrix.Matrix.M44:0.000000};");
                    sw.WriteLine($"\"{matrix.Name}\"");
                    sw.WriteLine();
                }
                sw.WriteLine();

                sw.WriteLine("// Number of meshes");
                sw.WriteLine();
                sw.WriteLine($"    {MeshCount}");
                sw.WriteLine();

                sw.WriteLine("// Mesh name list");
                sw.WriteLine();
                foreach (string mesh in Meshes)
                {
                    sw.WriteLine($"    \"{mesh}\"");
                }
                sw.WriteLine();

                sw.WriteLine("// Number of expressions");
                sw.WriteLine();
                sw.WriteLine($"    {ExpressionCount}");
                sw.WriteLine();

                sw.WriteLine("// Expression list :");
                sw.WriteLine("// HIGH_VAL  LOW_VAL");
                sw.WriteLine();
                foreach (Vector2 expression in Expressions)
                {
                    sw.WriteLine($"    {expression.X:0.000000}    {expression.Y:0.000000}");
                }
                sw.WriteLine();

                sw.WriteLine("// Number of nodes");
                sw.WriteLine();
                sw.WriteLine($"    {NodeCount}");
                sw.WriteLine();

                sw.WriteLine("// Node list :");
                sw.WriteLine("// TYPE  INDEX  CHILD  SIBLING");
                sw.WriteLine();
                foreach (TDRNode node in Nodes)
                {
                    sw.WriteLine($"    {(int)node.Type}    {node.Index}    {(node.Child > -1 ? $"{node.Child}" : "NULL")}    {(node.Sibling > -1 ? $"{node.Sibling}" : "NULL")}");
                }
                sw.WriteLine();
            }
        }
    }

    public class TDRNode
    {
        public enum NodeType
        {
            Matrix = 1,
            Texture,
            Mesh,
            Expression,
            Material,
            Spline,
            DynamicCollision,
            CullNode
        }

        public int ID { get; set; }

        public NodeType Type { get; set; }

        public int Index { get; set; }

        public int Child { get; set; }

        public int Sibling { get; set; }

        public string Name { get; set; }

        public TDRNode Parent { get; set; }

        public List<TDRNode> Children { get; set; } = new List<TDRNode>();

        public Matrix4D Transform { get; set; }

        public TDRNode(string name, int i, string line)
        {
            Name = name;
            ID = i;

            string[] parts = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Type = (NodeType)int.Parse(parts[0]);
            Index = int.Parse(parts[1]);
            Child = parts[2] == "NULL" ? -1 : int.Parse(parts[2]);
            Sibling = parts[3] == "NULL" ? -1 : int.Parse(parts[3]);
        }
    }

    public class TDRMatrix
    {
        public string Name { get; set; }

        public Matrix4D Matrix { get; set; } = Matrix4D.Identity;
    }

    public class TDRMaterial
    {
        public Vector4 V { get; set; }

        public int I { get; set; }

        public TDRMaterial(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            V = new Vector4(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            I = int.Parse(parts[4]);
        }
    }
}