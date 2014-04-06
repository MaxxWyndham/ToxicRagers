using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class MDL
    {
        static bool bDebug = false;
        int faceCount;
        int vertexCount;
        int version;
        string name;

        List<MDLMaterial> materials = new List<MDLMaterial>();
        List<MDLFace> faces = new List<MDLFace>();
        List<MDLVertex> verts = new List<MDLVertex>();

        public string Name { get { return (name != null ? name : "Unknown Mesh"); } }
        public List<MDLMaterial> Materials { get { return materials; } }
        public int FaceCount { get { return faceCount; } }
        public int VertexCount { get { return vertexCount; } }

        public static MDL Load(string Path)
        {
            // All these (int) casts are messy
            FileInfo fi = new FileInfo(Path);
            Logger.LogToFile("{0}", Path);
            MDL mdl = new MDL();

            mdl.name = fi.Name.Replace(fi.Extension, "");

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                if (br.ReadByte() != 69 ||
                    br.ReadByte() != 35 ||
                    br.ReadByte() != 2 ||
                    br.ReadByte() != 6)
                {
                    Logger.LogToFile("{0} isn't a valid MDL file", Path);
                    return null;
                }

                br.ReadBytes(4);    // No idea
                mdl.version = (int)br.ReadUInt32();
                br.ReadBytes(4);    // No idea

                Logger.LogToFile("Version {0}", mdl.version);

                int headerFaceCount = (int)br.ReadUInt32();
                int headerVertCount = (int)br.ReadUInt32();

                Logger.LogToFile("Faces: {0}", headerFaceCount);
                Logger.LogToFile("Verts: {0}", headerVertCount);

                br.ReadUInt32();    // Bytes remaining

                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea
                br.ReadBytes(4);    // No idea

                int nameCount = br.ReadInt16();

                Logger.LogToFile("Name count: {0}", nameCount);
                for (int i = 0; i < nameCount; i++)
                {
                    int nameLength = (int)br.ReadInt32();
                    int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + 4;

                    mdl.materials.Add(new MDLMaterial(br.ReadString(nameLength)));
                    br.ReadBytes(padding);

                    Logger.LogToFile("Added name \"{0}\" of length {1}, padding of {2}", mdl.materials[mdl.materials.Count - 1].Name, nameLength, padding);
                }

                mdl.faceCount = (int)br.ReadUInt32();

                Logger.LogToFile("Actual faces: {0}", mdl.faceCount);

                for (int i = 0; i < mdl.faceCount; i++)
                {
                    var face = new MDLFace(
                        (int)br.ReadUInt32(),   // ???, possibly material index
                        (int)br.ReadUInt32(),   // Vert index A
                        (int)br.ReadUInt32(),   // Vert index B
                        (int)br.ReadUInt32()    // Vert index C
                    );

                    mdl.faces.Add(face);

                    if (bDebug) { Logger.LogToFile("{0}) {1}", i, face.ToString()); }
                }

                mdl.vertexCount = (int)br.ReadUInt32();

                Logger.LogToFile("Actual verts: {0}", mdl.vertexCount);

                for (int i = 0; i < mdl.vertexCount; i++)
                {
                    var vert = new MDLVertex(
                        br.ReadSingle(),        // X
                        br.ReadSingle(),        // Y
                        br.ReadSingle(),        // Z
                        br.ReadSingle(),        // N.X
                        br.ReadSingle(),        // N.Y
                        br.ReadSingle(),        // N.Z
                        br.ReadSingle(),        // U
                        br.ReadSingle(),        // V
                        br.ReadSingle(),        // ?Unk6
                        br.ReadSingle(),        // ?Unk7
                        br.ReadByte(),          // R
                        br.ReadByte(),          // G
                        br.ReadByte(),          // B
                        br.ReadByte()           // A
                    );

                    mdl.verts.Add(vert);

                    if (bDebug) { Logger.LogToFile("{0}) {1}", i, vert.ToString()); }
                }

                Logger.LogToFile("This is usually {0}: {1}", nameCount, br.ReadUInt16());

                for (int i = 0; i < nameCount; i++)
                {
                    Logger.LogToFile("Block {0} of {1}", i, nameCount);
                    Logger.LogToFile("Position: {0}", br.BaseStream.Position.ToString("X"));

                    var material = mdl.materials[i];

                    for (int j = 0; j < 10; j++)
                    {
                        var bytes = br.ReadBytes(4);
                        Logger.LogToFile("{0}\t{1}\t{2}", br.BaseStream.Position.ToString("X"), BitConverter.ToString(bytes).Replace("-", " "), BitConverter.ToSingle(bytes, 0));
                    }

                    int offset = (int)br.ReadUInt32();
                    int a = (int)br.ReadUInt32();   // a == Actual Verts where Name count == 1.
                    int b = (int)br.ReadUInt32();
                    Logger.LogToFile("Offset: {0}", offset);
                    Logger.LogToFile("{0} is less than {1}: {2}", a, b, a < b);

                    int degenerateTriangles = 0;

                    for (int j = 0; j < b; j++)
                    {
                        int index = br.ReadUInt16();
                        bool bDegenerate = (br.ReadUInt16() != 0);

                        material.VertexList.Add(mdl.verts[index + offset]);
                        material.DegenerateList.Add(bDegenerate);

                        if (bDebug) { Logger.LogToFile("{0}] {1} {2} ({3})", j, index + offset, mdl.verts[index + offset].ToString(), bDegenerate); }
                        if (bDegenerate) { degenerateTriangles++; }
                    }

                    Logger.LogToFile("Total degenerates {0}", degenerateTriangles);

                    int uA = (int)br.ReadUInt32();
                    int uB = (int)br.ReadUInt32();
                    int uC = (int)br.ReadUInt32();

                    Logger.LogToFile("Unknown A: {0}", uA);
                    Logger.LogToFile("Unknown B: {0}", uB);
                    Logger.LogToFile("Unknown C: {0}", uC);

                    //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", Path, nameCount, headerFaceCount, headerVertCount, mdl.faceCount, mdl.vertexCount, uA, uB, uC);

                    if (uA > 0) 
                    {
                        if (material.VertexList.Count > 0)
                        {
                            mdl.materials.Add(new MDLMaterial(material.Name));
                            material = mdl.materials[mdl.materials.Count - 1];
                        }

                        material.Mode = "triangles";
                    }

                    for (int j = 0; j < uC; j++)
                    {
                        
                        int index = (int)br.ReadUInt32();
                        //Logger.LogToFile("{0}] {1}", j, index);
                        material.VertexList.Add(mdl.verts[uA + index]);
                    }
                }

                Logger.LogToFile("Position: {0}", br.BaseStream.Position.ToString("X"));

                Logger.LogToFile("Always 0: {0}", br.ReadUInt32());

                for (int i = 0; i < headerVertCount; i++)
                {
                    
                    if (bDebug)
                    {
                        Logger.LogToFile("{0}, {1}, {2} : {3}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadUInt32());
                    }
                    else
                    {
                        br.ReadBytes(16);
                    }
                }

                for (int i = 0; i < Math.Max(headerFaceCount, mdl.faceCount); i++)
                {
                    br.ReadBytes(137);
                }

                for (int i = 0; i < mdl.faceCount; i++)
                {
                    
                    if (bDebug)
                    {
                        Logger.LogToFile("{0}", br.ReadUInt32());
                    }
                    else
                    {
                        br.ReadUInt32();
                    }
                }

                Logger.LogToFile("{0} == {1}", mdl.vertexCount, br.ReadUInt32());

                for (int i = 0; i < mdl.vertexCount; i++)
                {
                    if (bDebug)
                    {
                        Logger.LogToFile("{0}", br.ReadUInt32());
                    }
                    else
                    {
                        br.ReadUInt32();
                    }
                }

                Logger.LogToFile("{0} :: {1}", br.BaseStream.Position.ToString("X"), br.BaseStream.Length.ToString("X"));
            }

            return mdl;
        }

        public List<MDLVertex> GetTriangleStrip(int index)
        {
            return materials[index].VertexList;
        }

        public string GetMaterialMode(int index)
        {
            return materials[index].Mode;
        }
    }

    public class MDLFace
    {
        int unknown;    // Probably Material ID/Index?
        int vertexA;
        int vertexB;
        int vertexC;

        public int V1 { get { return vertexA; } }
        public int V2 { get { return vertexB; } }
        public int V3 { get { return vertexC; } }

        public MDLFace(int Unknown, int A, int B, int C)
        {
            this.unknown = Unknown;
            this.vertexA = A;
            this.vertexB = B;
            this.vertexC = C;
        }

        public override string ToString()
        {
            return "{ Face: {A:" + vertexA + " B:" + vertexB + " C:" + vertexC + "} Material: " + unknown + " }";
        }
    }

    public class MDLVertex
    {
        Vector3 position;
        Vector3 normal;
        Vector2 uv;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public Vector2 UV
        {
            get { return uv; }
            set { uv = value; }
        }

        public MDLVertex(Single X, Single Y, Single Z, Single NX, Single NY, Single NZ, Single U, Single V, Single Unk6, Single Unk7, byte R, byte G, byte B, byte Alpha)
        {
            position = new Vector3(X, Y, Z);
            normal = new Vector3(NX, NY, NZ);
            uv = new Vector2(U, V);

            //Logger.LogToFile("Unknown data: {0} {1}", Unk6, Unk7);
        }

        public override string ToString()
        {
            return "{ Position: {X:" + Position.X + " Y:" + Position.Y + " Z:" + Position.Z + "} Normal: {X:" + Normal.X + " Y:" + Normal.Y + " Z:" + Normal.Z + "} UV: {U:" + UV.X + " V:" + UV.Y + "} }";
        }
    }

    public class MDLMaterial
    {
        string name;
        List<MDLVertex> vertexList;
        List<bool> degenerateList;
        string mode = "trianglestrip";

        public string Name { get { return name; } }
        public List<MDLVertex> VertexList { get { return vertexList; } set { vertexList = value; } }
        public List<bool> DegenerateList { get { return degenerateList; } }
        public string Mode { get { return mode; } set { mode = value; } }

        public MDLMaterial(string Name)
        {
            name = Name;
            vertexList = new List<MDLVertex>();
            degenerateList = new List<bool>();
        }
    }
}
