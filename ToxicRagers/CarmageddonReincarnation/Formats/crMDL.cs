using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class MDL
    {
        int faceCount;
        int vertexCount;
        int version;
        string name;

        List<MDLMaterial> materials = new List<MDLMaterial>();
        List<MDLFace> faces = new List<MDLFace>();
        List<MDLVertex> verts = new List<MDLVertex>();

        public string Name
        {
            get { return (name != null ? name : "Unknown Mesh"); }
        }

        public List<MDLMaterial> Materials
        {
            get { return materials; }
        }

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
                    mdl.faces.Add(
                        new MDLFace(
                            (int)br.ReadUInt32(),   // ???, possibly material index
                            (int)br.ReadUInt32(),   // Vert index A
                            (int)br.ReadUInt32(),   // Vert index B
                            (int)br.ReadUInt32()    // Vert index C
                        )
                    );
                }

                mdl.vertexCount = (int)br.ReadUInt32();

                Logger.LogToFile("Actual verts: {0}", mdl.vertexCount);

                for (int i = 0; i < mdl.vertexCount; i++)
                {
                    mdl.verts.Add(
                        new MDLVertex(
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
                        )
                    );
                }

                Logger.LogToFile("This is usually {0}: {1}", nameCount, br.ReadUInt16());

                // TODO: Work out how to process subsequent material lists
                //for (int i = 0; i < 1; i++)
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

                    Logger.LogToFile("Always 0: {0}", br.ReadUInt32());
                    int a = (int)br.ReadUInt32();
                    int b = (int)br.ReadUInt32();
                    Logger.LogToFile("{0} is less than {1}: {2}", a, b, a < b);

                    for (int j = 0; j < b; j++)
                    {
                        material.VertexList.Add(mdl.verts[br.ReadUInt16()]);
                        br.ReadUInt16(); // is degenerate?
                    }

                    int uA = (int)br.ReadUInt32();
                    int uB = (int)br.ReadUInt32();
                    int uC = (int)br.ReadUInt32();

                    Logger.LogToFile("Unknown A: {0}", uA);
                    Logger.LogToFile("Unknown B: {0}", uB);
                    Logger.LogToFile("Unknown C: {0}", uC);

                    for (int j = 0; j < uC; j++)
                    {
                        br.ReadUInt32();
                    }

                    //b = (int)br.ReadUInt32();
                    //Logger.LogToFile("new b: {0}", b);

                    //material.VertexList.Clear();

                    //for (int j = 0; j < b; j++)
                    //{
                    //    material.VertexList.Add(mdl.verts[br.ReadUInt16()]);
                    //    br.ReadUInt16(); // is degenerate?
                    //}
                }

                Logger.LogToFile("Position: {0}", br.BaseStream.Position.ToString("X"));
            }

            return mdl;
        }

        public List<MDLVertex> GetTriangleStrip(int index)
        {
            return materials[index].VertexList;
        }

        /*
            Names are padded to the nearest 4 bytes with a 4 byte spacer.  For example:
            Rims would get 4 bytes of padding
            ToxicRagers would get 5 bytes of padding

            Data_Core\Content\Vehicles\Countslash\driver.MDL
            0   45 23 02 06     Magic Number
            4	92 0F B9 05
            8	81 00 00 00
            C	00 00 00 00
            10	02 00 00 00     Sometimes Face Count
            14	04 00 00 00     Sometimes Vertex Count
            18	D2 02 00 00     Bytes remaining
                C2 9C D2 3C     0.025709514
                98 B2 9E BC     -0.01937227
                6F 12 83 BA     -0.001
                BF 1E 93 BC     -0.017958997
                71 81 96 3C     0.01837227
                6F 12 83 3A     0.001
                98 ED 8A 3C     0.016958997
                70 12 03 BA     ?? -5.000001E-4 ??
                00 00 00 00
                70 12 03 BA     ?? -5.000001E-4 ??
            44	01 00		    Number of names [*C*]
            46	04 00 00 00     Length of first name
            4A	52 69 6D 73     "Rims"
                00 00 00 00     Padding to 8 bytes
            52	02 00 00 00     Face Count [*A*]
            56	[*A*] x 16 byte blocks - face data
            76	04 00 00 00     Vertex Count [*B*]
            7A	[*B*] x 44 byte blocks - vertex data
            12A	01 00           Same as [*C*]
            12C	00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                00 D3 CC 3C     0.025002956
                71 81 96 BC     -0.01837227
                00 00 00 00     0
                98 ED 8A BC     -0.016958997
                71 81 96 3C     0.01837227
                00 00 00 00     0
                98 ED 8A 3C     0.016958997
                00 00 00 00     0
                04 00 00 00     4
                04 00 00 00     4
                00 00 00 00     0
                01 00 00 00     1
                02 00 00 00     2
                03 00 00 00     3
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                71 81 96 BC     -0.01837227
                00 00 00 00     0
                98 ED 8A BC     -0.016958997
                01 00 00 00     1
                71 81 96 3C     0.01837227
                00 00 00 00     0
                98 ED 8A BC     -0.016958997
                01 00 00 00     1
                71 81 96 BC     -0.01837227
                00 00 00 00     0
                98 ED 8A 3C     0.016958997
                01 00 00 00     1
                71 81 96 3C     0.01837227
                00 00 00 00     0
                98 ED 8A 3C     0.016958997
                01 00 00 00     1
                00 00 00 00     0
                00 00 00 00     0
                00 00 80 3F     1
                00 00 00 80     -0
                00 00 00 00     0
                00 00 80 3F     1
                00 00 00 80     -0
                00 00 00 00     0
                00 00 80 3F     1
                00 00 00 00     0
                00 00 00 00     0
                00 00 80 3F     1
                00 00 00 00     0
                00 00 00 00     0
                01 00 00 00     1
                02 00 00 00     2
                03 00 00 00     3
                00 00 00 00     0
                80 80 80 FF     Vertex colour
                80 80 80 FF     Vertex colour
                80 80 80 FF     Vertex colour
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                00 00 80 3F     1
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                00 00 80 3F     1
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
                00 00 00 00     0
	            00              Terminator?
	            00 00 00 80     -0
	            00 00 80 3F     1
	            00 00 00 00     0
	            00 00 00 80     -0
	            00 00 80 3F     1
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 80 3F     1
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 80 3F     1
	            00 00 00 00     0
	            00 00 00 00     0
	            01 00 00 00     1
	            01 00 00 00     1
	            00 00 00 00     0
	            03 00 00 00     3
	            80 80 80 FF     Vertex colour
	            80 80 80 FF     Vertex colour
	            80 80 80 FF     Vertex colour
	            00 00 80 3F     1
	            00 00 80 3F     1
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 80 3F     1
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 80 3F     1
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 00 00     0
	            00 00 00 00     0
	            00              Terminator?
	            01 00 00 00     1
	            04 00 00 00     4
	            02 00 00 00     2
	            03 00 00 00     3
	            00 00 00 00     0
	            01 00 00 00     1
         */
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
    }

    public class MDLMaterial
    {
        string name;
        List<MDLVertex> vertexList;

        public string Name { get { return name; } }
        public List<MDLVertex> VertexList { get { return vertexList; } }

        public MDLMaterial(string Name)
        {
            name = Name;
            vertexList = new List<MDLVertex>();
        }
    }
}
