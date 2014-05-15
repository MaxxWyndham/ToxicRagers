using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class HIE
    {
        public static CultureInfo Culture = new CultureInfo("en-GB");

        int version;
        int cullNodeCount;
        int collisionDataMeshCount;
        int lineCount;
        int textureCount;
        int materialCount;
        int matrixCount;
        int meshCount;
        int expressionCount;
        int nodeCount;
        List<string> textures;
        List<TDRMatrix> matrixes;
        List<string> meshes;
        List<TDRNode> nodes;
        TDRNode root;

        public TDRNode Root { get { return root; } }
        public List<string> Textures { get { return textures; } }
        public List<TDRMatrix> Matrixes { get { return matrixes; } }
        public List<string> Meshes { get { return meshes; } }

        public HIE()
        {
            textures = new List<string>();
            matrixes = new List<TDRMatrix>();
            meshes = new List<string>();
            nodes = new List<TDRNode>();
        }

        public static HIE Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            HIE hie = new HIE();
            H h = new H(); ;

            var f = fi.Directory.GetFiles(Path.GetFileNameWithoutExtension(path) + ".h");
            if (f.Length == 0) { f = fi.Directory.Parent.GetFiles(Path.GetFileNameWithoutExtension(path) + ".h"); }
            if (f.Length > 0) { h = H.Load(f[0].FullName); }

            string[] lines;

            using (var sr = new StreamReader(fi.OpenRead())) { lines = sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
            lines = lines.Select(l => l.Trim()).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                switch (lines[i].ToLower())
                {
                    case "//version number":
                        hie.version = int.Parse(lines[i + 2]);
                        i += 2;
                        break;

                    case "// number of cull nodes":
                        hie.cullNodeCount = int.Parse(lines[++i]);
                        break;

                    case "// cull node list":
                        for (int j = 0; j < hie.cullNodeCount; j++)
                        {
                            var cullNode = lines[++i];
                        }
                        break;

                    case "// number of collision data meshes":
                        hie.collisionDataMeshCount = int.Parse(lines[++i]);
                        break;

                    case "// collision data list":
                        for (int j = 0; j < hie.collisionDataMeshCount; j++)
                        {
                            var collisionDataMesh = lines[++i];
                        }
                        break;

                    case "// number of lines":
                        hie.lineCount = int.Parse(lines[++i]);
                        break;

                    case "// line name list":
                        for (int j = 0; j < hie.lineCount; j++)
                        {
                            var line = lines[++i];
                        }
                        break;

                    case "// number of textures":
                        hie.textureCount = int.Parse(lines[++i]);
                        break;

                    case "// texture name list":
                        for (int j = 0; j < hie.textureCount; j++)
                        {
                            hie.textures.Add(lines[++i].Replace("\"", "").Trim());
                        }
                        break;

                    case "// number of materials":
                        hie.materialCount = int.Parse(lines[++i]);
                        break;

                    case "// material name list":
                        for (int j = 0; j < hie.materialCount; j++)
                        {
                            var material = lines[++i];
                        }
                        break;

                    case "// number of matrices":
                        hie.matrixCount = int.Parse(lines[++i]);
                        break;

                    case "// matrix name list":
                        for (int j = 0; j < hie.matrixCount; j++)
                        {
                            string[] parts;
                            var matrix = new TDRMatrix();

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M11 = Convert.ToSingle(parts[0], HIE.Culture);
                            matrix.Matrix.M12 = Convert.ToSingle(parts[1], HIE.Culture);
                            matrix.Matrix.M13 = Convert.ToSingle(parts[2], HIE.Culture);

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M21 = Convert.ToSingle(parts[0], HIE.Culture);
                            matrix.Matrix.M22 = Convert.ToSingle(parts[1], HIE.Culture);
                            matrix.Matrix.M23 = Convert.ToSingle(parts[2], HIE.Culture);

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M31 = Convert.ToSingle(parts[0], HIE.Culture);
                            matrix.Matrix.M32 = Convert.ToSingle(parts[1], HIE.Culture);
                            matrix.Matrix.M33 = Convert.ToSingle(parts[2], HIE.Culture);

                            parts = lines[++i].Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            matrix.Matrix.M41 = Convert.ToSingle(parts[0], HIE.Culture);
                            matrix.Matrix.M42 = Convert.ToSingle(parts[1], HIE.Culture);
                            matrix.Matrix.M43 = Convert.ToSingle(parts[2], HIE.Culture);

                            matrix.Name = lines[++i].Trim().Replace("\"", "");

                            hie.matrixes.Add(matrix);
                        }
                        break;

                    case "// number of meshes":
                        hie.meshCount = int.Parse(lines[++i]);
                        break;

                    case "// mesh name list":
                        while (!lines[++i].StartsWith("//"))
                        {
                            hie.meshes.Add(lines[i].Trim().Replace("\"", ""));
                        }

                        i--;
                        break;

                    case "// number of expressions":
                        hie.expressionCount = int.Parse(lines[++i]);
                        break;

                    case "// expression list :":
                        if (lines[i + 1].ToLower() == "// high_val  low_val") { i++; }
                        for (int j = 0; j < hie.expressionCount; j++)
                        {
                            var expression = lines[++i];
                        }
                        break;

                    case "// number of nodes":
                        hie.nodeCount = int.Parse(lines[++i]);
                        break;

                    case "// node list :":
                        if (lines[i + 1].ToLower() == "// type  index  child  sibling") { i++; }
                        for (int j = 0; j < hie.nodeCount; j++)
                        {
                            hie.nodes.Add(new TDRNode((h.Definitions.ContainsKey(j) ? h.Definitions[j] : "DEFAULT"), j, lines[++i]));
                        }
                        break;

                    default:
                        Console.WriteLine(i + "] " + lines[i]);
                        break;
                }
            }

            //        If iLineCount > 0 Then lSplines = ProcessLins(sFile.Replace(".hie", ".lins"))

            //        For i As Integer = 0 To iTexCount - 1
            //            TextureFlags.Add(0)
            //        Next

            //        For i As Integer = 0 To iMeshCount - 1
            //            MatrixList.Add(New List(Of Matrix3D))
            //            MeshTexture.Add(0)
            //        Next

            //        Dim tree As New Node(0, lNodes, 0)
            //        Dim stack As New Stack(Of Node)
            //        Dim tID, mID, matFlags As Integer

            hie.root = hie.nodes[0];
            hie.root.Name = (h.Definitions.ContainsKey(0) ? h.Definitions[0] : "DEFAULT");
            hie.root.Transform = hie.matrixes[hie.root.Index].Matrix;

            walkHierarchy(hie.root, 1, hie);


            //        stack.Push(tree)
            //        While stack.Count > 0
            //            Dim current As Node = stack.Pop()

            //            Console.WriteLine(Repeat(Chr(32), current.Depth * 2) & current.iNode & " - " & current.Type.ToString() & "[" & current.Index & "]")

            //            Select Case current.Type
            //                Case Node.NodeType.Matrix
            //                    mID = current.Index

            //                Case Node.NodeType.Material
            //                    matFlags = lMaterials(current.Index)

            //                Case Node.NodeType.Texture
            //                    tID = current.Index
            //                    If tID < 0 Then tID = 0
            //                    TextureFlags(tID) = matFlags

            //                Case Node.NodeType.Mesh
            //                    MatrixList(current.Index) = Node.BuildMatrix(current, lMatrix)
            //                    MeshTexture(current.Index) = tID

            //                Case Node.NodeType.Spline
            //                    Lines.Add(New tdrSpline(lMatrixFile(mID), Node.BuildSingleMatrix(current, lMatrix), lSplines(current.Index)))

            //            End Select

            //            For Each child As Node In current.Children
            //                stack.Push(child)
            //            Next
            //        End While
            //    End Sub

            return hie;
        }

        private static void walkHierarchy(TDRNode parent, int index, HIE hie) 
        {
            TDRNode node = hie.nodes[index];

            Console.WriteLine("{0} > {1} : {2}", parent.ID, node.ID, node.Type);

            switch (node.Type)
            {
                case TDRNode.NodeType.Matrix:
                    node.Transform = hie.matrixes[node.Index].Matrix;
                    break;
            }

            node.Parent = parent;
            parent.Children.Insert(0, node);

            if (node.Sibling > -1) { walkHierarchy(parent, node.Sibling, hie); }
            if (node.Child > -1) { walkHierarchy(node, node.Child, hie); }
        }
    }

    public class TDRNode
    {
        public enum NodeType
        {
            Unknown_0,
            Matrix,
            Texture,
            Mesh,
            Unknown_4,
            Material,
            Spline,
            DynamicCollision,
            CullNode
        }

        NodeType type;
        int id;
        int index;
        int child;
        int sibling;
        Matrix3D transform;

        string name;

        TDRNode parent;
        List<TDRNode> children;

        public NodeType Type { get { return type; } }
        public int ID { get { return id; } }
        public TDRNode Parent { get { return parent; } set { parent = value; } }
        public List<TDRNode> Children { get { return children; } }

        public string Name
        {
            get { return name; } 
            set { name = value; } 
        }
        
        public Matrix3D Transform 
        { 
            get { return transform; } 
            set { transform = value; } 
        }

        public int Index { get { return index; } }
        public int Child { get { return child; } set { child = value; } }
        public int Sibling { get { return sibling; } set { sibling = value; } }

        public TDRNode(string name, int i, string line)
        {
            this.name = name;
            this.id = i;

            children = new List<TDRNode>();
            var parts = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            type = (NodeType)int.Parse(parts[0]);
            index = int.Parse(parts[1]);
            child = (parts[2] == "NULL" ? -1 : int.Parse(parts[2]));
            sibling = (parts[3] == "NULL" ? -1 : int.Parse(parts[3]));
        }
    }

    public class TDRMatrix
    {
        string name;
        Matrix3D matrix;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Matrix3D Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }

        public TDRMatrix()
        {
            matrix = Matrix3D.Identity;
        }
    }
}
