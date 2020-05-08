using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ToxicRagers.Stainless.Formats;

namespace ToxicRagers.Helpers
{
    public class Octree
    {
        public const int MaxDepth = 9;
        public const int MaxFacesPerLeaf = 200;
        public const float NodeSplitParameter = 0.5f;
        private int version = 5;
        private int numModels;
        private byte pathLength;
        private bool hasMatrix;
        private BoundingBox bounds = new BoundingBox();
        private ushort[] faceData = new ushort[0];
        private OctreeNode root;

        private int faceOffset = 0;

        public static Octree ReadFromMemory(BinaryReader br)
        {
            Octree octree = new Octree
            {
                version = (int)br.ReadUInt32(),
                numModels = (int)br.ReadUInt32(),
                pathLength = br.ReadByte(), // what on earth is path length?
                hasMatrix = br.ReadBoolean()
            };

            if (octree.hasMatrix)
            {
                // load M34
                return null;
            }

            octree.bounds.Min = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            octree.bounds.Max = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            int faceDataLength = (int)br.ReadUInt32();

            octree.faceData = new ushort[faceDataLength];

            for (int i = 0; i < faceDataLength; i++)
            {
                octree.faceData[i] = br.ReadUInt16();
            }

            octree.root = OctreeNode.Read(br, octree);

            Console.WriteLine($"{octree.GetNodeFaceDataLength(octree.root)} :: {octree.faceData.Length}");

            br.ReadUInt32(); // checksum

            return octree;
        }

        public ushort[] ReadFaceData(int faceCount)
        {
            ushort[] data = new ushort[faceCount];
            Array.Copy(faceData, faceOffset, data, 0, faceCount);
            faceOffset += faceCount;
            return data;
        }

        public static Octree CreateFromModel(MDL model)
        {
            Vector3 epsilon = new Vector3(0.001f, 0.001f, 0.001f);

            Octree octree = new Octree
            {
                numModels = 1,
                bounds = new BoundingBox(model.Extents.Min - epsilon, model.Extents.Max + epsilon)
            };

            List<OctreeFace> faces = new List<OctreeFace>();

            for (ushort i = 0; i < model.Faces.Count; i++)
            {
                faces.Add(new OctreeFace
                {
                    FaceNum = i,
                    HitNumber = -1,
                    Vertices = new List<Vector3>
                    {
                        model.Vertices[model.Faces[i].Verts[0]].Position,
                        model.Vertices[model.Faces[i].Verts[1]].Position,
                        model.Vertices[model.Faces[i].Verts[2]].Position
                    }
                });
            }

            octree.root = (OctreeNode)OctreeNode.Create(octree.bounds, faces);

            return octree;
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create), Encoding.Default))
            {
                bw.Write(version);
                bw.Write(numModels);
                bw.Write(pathLength);
                bw.Write(hasMatrix);

                bw.Write(bounds.Min.X);
                bw.Write(bounds.Min.Y);
                bw.Write(bounds.Min.Z);
                bw.Write(bounds.Max.X);
                bw.Write(bounds.Max.Y);
                bw.Write(bounds.Max.Z);

                bw.Write(GetNodeFaceDataLength(root));

                for (int i = 0; i < faceData.Length; i++)
                {
                    bw.Write(faceData[i]);
                }
            }
        }

        public int GetNodeFaceDataLength(OctreeChild n)
        {
            int l = 0;

            if (n is OctreeNode)
            {
                for (int i = 0; i < 8; i++)
                {
                    l += GetNodeFaceDataLength((n as OctreeNode).Children[i]);
                }
            }
            else
            {
                if (n is OctreeLeaf)
                {
                    l += GetLeafFaceDataLength(n as OctreeLeaf);
                }
                else
                {
                    l += 0;
                }
            }

            return l;
        }

        public int GetLeafFaceDataLength(OctreeLeaf leaf)
        {
            int l = 0;

            if (leaf.FaceData.Length > 0)
            {
                for (l = 0; leaf.FaceData[l] != 0xffff; l++) { }

                return l + 1;
            }
            else
            {
                return 0;
            }
        }

        public static List<OctreeFace> FindFacesFromList(List<OctreeFace> faces, BoundingBox bounds)
        {
            List<OctreeFace> matches = new List<OctreeFace>();

            for (int i = 0; i < faces.Count; i++)
            {
                if (bounds.IntersectsFace(faces[i].Vertices))
                {
                    matches.Add(faces[i]);
                }
            }

            return matches;
        }
    }

    public class OctreeChild
    {
        [Flags]
        public enum SplitFlags : byte
        {
            SplitInX = 0x1,
            SplitInY = 0x2,
            SplitInZ = 0x4
        }

        public int ChildType { get; set; }
        public SplitFlags Flags { get; set; }

        public static void DetermineSplittage(int depth, List<OctreeFace> faces, Vector3 centre, ref SplitFlags splitFlags)
        {
            int nFacesXMin = 0, nFacesXMax = 0;
            int nFacesYMin = 0, nFacesYMax = 0;
            int nFacesZMin = 0, nFacesZMax = 0;

            for (int i = 0; i < faces.Count; i++)
            {
                OctreeFace face = faces[i];

                if (face.HitNumber == depth || face.HitNumber < 0)
                {
                    if (face.Vertices[0].X < centre.X &&
                        face.Vertices[1].X < centre.X &&
                        face.Vertices[2].X < centre.X)
                    {
                        nFacesXMin++;
                    }

                    if (face.Vertices[0].X > centre.X &&
                        face.Vertices[1].X > centre.X &&
                        face.Vertices[2].X > centre.X)
                    {
                        nFacesXMax++;
                    }

                    if (face.Vertices[0].Y < centre.Y &&
                        face.Vertices[1].Y < centre.Y &&
                        face.Vertices[2].Y < centre.Y)
                    {
                        nFacesYMin++;
                    }

                    if (face.Vertices[0].Y > centre.Y &&
                        face.Vertices[1].Y > centre.Y &&
                        face.Vertices[2].Y > centre.Y)
                    {
                        nFacesYMax++;
                    }

                    if (face.Vertices[0].Z < centre.Z &&
                        face.Vertices[1].Z < centre.Z &&
                        face.Vertices[2].Z < centre.Z)
                    {
                        nFacesZMin++;
                    }

                    if (face.Vertices[0].Z > centre.Z &&
                        face.Vertices[1].Z > centre.Z &&
                        face.Vertices[2].Z > centre.Z)
                    {
                        nFacesZMax++;
                    }
                }
            }

            splitFlags = 0;

            if (nFacesXMin + nFacesXMax > Octree.NodeSplitParameter * faces.Count) { splitFlags |= SplitFlags.SplitInX; }
            if (nFacesYMin + nFacesYMax > Octree.NodeSplitParameter * faces.Count) { splitFlags |= SplitFlags.SplitInY; }
            if (nFacesZMin + nFacesZMax > Octree.NodeSplitParameter * faces.Count) { splitFlags |= SplitFlags.SplitInZ; }
        }
    }

    public class OctreeNode : OctreeChild
    {
        public OctreeChild[] Children { get; set; }

        public OctreeNode()
        {
            Children = new OctreeChild[8];

            for (int i = 0; i < Children.Length; i++)
            {
                Children[i] = new OctreeChild();
            }
        }

        public static OctreeChild Create(BoundingBox bounds, List<OctreeFace> faces, int depth = -1)
        {
            depth++;

            OctreeChild node = new OctreeChild();
            Vector3 centre = (bounds.Min + bounds.Max) * 0.5f;
            BoundingBox childBounds = new BoundingBox();
            SplitFlags splitFlags = 0;

            if (depth > Octree.MaxDepth || (depth > 0 && faces.Count < Octree.MaxFacesPerLeaf))
            {
                splitFlags = 0;

                if (faces.Count > Octree.MaxFacesPerLeaf / 8)
                {
                    if (bounds.Max.X - bounds.Min.X > 100.0f) { splitFlags |= SplitFlags.SplitInX; }
                    if (bounds.Max.Z - bounds.Min.Z > 100.0f) { splitFlags |= SplitFlags.SplitInZ; }
                }

                if (splitFlags == 0)
                {
                    node = OctreeLeaf.Create(depth, faces);
                    depth--;
                    return node;
                }
            }
            else
            {
                DetermineSplittage(depth, faces, centre, ref splitFlags);
            }

            if (depth > 0 && splitFlags == 0)
            {
                node = OctreeLeaf.Create(depth, faces);
                depth--;
                return node;
            }

            //numNodes++;

            node = new OctreeNode
            {
                ChildType = 0,
                Flags = splitFlags
            };

            for (int i = 0; i < (node as OctreeNode).Children.Length; i++)
            {
                if (node.Flags.HasFlag(SplitFlags.SplitInX))
                {
                    if ((i & 1) == 1)
                    {
                        childBounds.Min.X = centre.X;
                        childBounds.Max.X = bounds.Max.X;
                    }
                    else
                    {
                        childBounds.Min.X = bounds.Min.X;
                        childBounds.Max.X = centre.X;
                    }
                }
                else
                {
                    if ((i & 1) == 1)
                    {
                        continue;
                    }
                    else
                    {
                        childBounds.Min.X = bounds.Min.X;
                        childBounds.Max.X = bounds.Max.X;
                    }
                }

                if (node.Flags.HasFlag(SplitFlags.SplitInY))
                {
                    if ((i & 2) == 2)
                    {
                        childBounds.Min.Y = centre.Y;
                        childBounds.Max.Y = bounds.Max.Y;
                    }
                    else
                    {
                        childBounds.Min.Y = bounds.Min.Y;
                        childBounds.Max.Y = centre.Y;
                    }
                }
                else
                {
                    if ((i & 2) == 2)
                    {
                        continue;
                    }
                    else
                    {
                        childBounds.Min.Y = bounds.Min.Y;
                        childBounds.Max.Y = bounds.Max.Y;
                    }
                }

                if (node.Flags.HasFlag(SplitFlags.SplitInZ))
                {
                    if ((i & 4) == 4)
                    {
                        childBounds.Min.Z = centre.Z;
                        childBounds.Max.Z = bounds.Max.Z;
                    }
                    else
                    {
                        childBounds.Min.Z = bounds.Min.Z;
                        childBounds.Max.Z = centre.Z;
                    }
                }
                else
                {
                    if ((i & 4) == 4)
                    {
                        continue;
                    }
                    else
                    {
                        childBounds.Min.Z = bounds.Min.Z;
                        childBounds.Max.Z = bounds.Max.Z;
                    }
                }

                //Logger.LogToFile(Logger.LogLevel.All, $"{depth}\t{i}\t{childBounds.Min}\t{childBounds.Max}");

                List<OctreeFace> childFaces = Octree.FindFacesFromList(faces, childBounds);
                (node as OctreeNode).Children[i] = Create(childBounds, childFaces, depth);
            }

            depth--;

            return node;
        }

        public static OctreeNode Read(BinaryReader br, Octree octree)
        {
            OctreeNode node = new OctreeNode();

            int childMask;
            int nodeMask;
            int iNode;

            childMask = br.ReadByte();
            node.ChildType = br.ReadByte();
            node.Flags = (SplitFlags)br.ReadByte();

            for (iNode = 0, nodeMask = 1; iNode < 8; iNode++, nodeMask <<= 1)
            {
                if ((childMask & nodeMask) > 0)
                {
                    if ((node.ChildType & nodeMask) > 0)
                    {
                        node.Children[iNode] = OctreeLeaf.Read(br, octree);
                    }
                    else
                    {
                        node.Children[iNode] = Read(br, octree);
                    }
                }
                else
                {
                    if ((((iNode & 1) == 0) || node.Flags.HasFlag(SplitFlags.SplitInX)) &&
                        (((iNode & 2) == 0) || node.Flags.HasFlag(SplitFlags.SplitInY)) &&
                        (((iNode & 4) == 0) || node.Flags.HasFlag(SplitFlags.SplitInZ)))
                    {
                        node.Children[iNode] = new OctreeLeaf(0, 0);
                        node.ChildType |= nodeMask;
                    }
                }
            }

            return node;
        }
    }

    public class OctreeLeaf : OctreeChild
    {
        public int Depth { get; set; }
        public int NumFaces { get; set; }
        public ushort[] FaceData { get; set; }

        public OctreeLeaf() { }

        public OctreeLeaf(int depth, int numFaces)
        {
            Depth = depth;
            NumFaces = numFaces;
        }

        public static OctreeLeaf Read(BinaryReader br, Octree octree)
        {
            OctreeLeaf leaf = new OctreeLeaf
            {
                FaceData = octree.ReadFaceData((int)br.ReadUInt32())
            };

            //if ((leaf.FaceData.Length == 12 && leaf.FaceData[1] == 0x0536) || (leaf.FaceData.Length == 8 && leaf.FaceData[1] == 0x054C))
            //{
            //    Logger.LogToFile(Logger.LogLevel.All, string.Join(" ", Array.ConvertAll(leaf.FaceData, x => $"0x{x.ToString("X4")}")));
            //}


            return leaf;
        }

        public static OctreeLeaf Create(int depth, List<OctreeFace> faces)
        {
            OctreeLeaf leaf = new OctreeLeaf();

            if (faces.Count > 0)
            {
                List<ushort> faceData = new List<ushort>();
                int stackSize = faces.Count * 6 + 2;
                bool notYetFoundAFace = true;
                ushort modelIndex = 0, lastModelIndex = 0xffff, firstFaceIndex = 0xffff, lastFaceIndex = 0xffff;

                leaf.NumFaces = 0;

                for (int i = 0; i < faces.Count; i++)
                {
                    OctreeFace face = faces[i];

                    if (face.HitNumber == depth || face.HitNumber < depth)
                    {
                        if (notYetFoundAFace)
                        {
                            faceData.Add(modelIndex);
                            faceData.Add(face.FaceNum);

                            lastModelIndex = 0;
                            firstFaceIndex = face.FaceNum;
                            notYetFoundAFace = false;
                        }
                        else
                        {
                            if (face.FaceNum != lastFaceIndex + 1 || modelIndex != lastModelIndex)
                            {
                                faceData = EncodeRun(faceData, (ushort)(lastFaceIndex - firstFaceIndex));

                                if (modelIndex != lastModelIndex)
                                {
                                    faceData.Add(0xfffe);
                                    faceData.Add(modelIndex);
                                    lastModelIndex = modelIndex;
                                }

                                faceData.Add(face.FaceNum);
                                firstFaceIndex = face.FaceNum;
                            }
                        }

                        lastFaceIndex = face.FaceNum;
                        leaf.NumFaces++;
                    }
                }

                faceData = EncodeRun(faceData, (ushort)(lastFaceIndex - firstFaceIndex));
                faceData.Add(0xffff);
                leaf.FaceData = faceData.ToArray();
            }
            else
            {
                leaf.FaceData = new ushort[0];
            }

            //OctreeFace poop = faces.FirstOrDefault(f => f.FaceNum == 8090);

            //if (poop != null)
            //{
            //    Logger.LogToFile(Logger.LogLevel.All, $"{poop.Vertices[0]}\t{poop.Vertices[1]}\t{poop.Vertices[2]}");
            //    Logger.LogToFile(Logger.LogLevel.All, string.Join(" ", Array.ConvertAll(leaf.FaceData, x => $"0x{x.ToString("X4")}")));
            //}

            return leaf;
        }

        public static List<ushort> EncodeRun(List<ushort> faces, ushort runLength)
        {
            if (runLength > 0)
            {
                while (runLength > 0xfd)
                {
                    faces.Add(0xfffd);
                    runLength -= 0xfd;
                }

                faces.Add((ushort)(0xff00 | runLength));
            }

            return faces;
        }
    }

    public class OctreeFace
    {
        public ushort FaceNum { get; set; }
        public List<Vector3> Vertices { get; set; } = new List<Vector3>();
        public int HitNumber { get; set; } = 1;
    }
}
