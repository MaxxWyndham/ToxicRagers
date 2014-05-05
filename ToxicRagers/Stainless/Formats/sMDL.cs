using System;
using System.Collections.Generic;
using System.IO;
using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class MDL
    {
        [Flags]
        public enum Flags
        {
            Model = 1,
            UnknownA = 8,
            UnknownB = 32,
            DoubleSided = 128
        }

        public static Dictionary<string, Version> SupportedVersions = new Dictionary<string, Version>
        {
            //{ "5.6", new Version(5,6)}, // iOS, Novadrome
            { "5.9", new Version(5,9)}, // iOS, Novadrome
            { "6.0", new Version(6,0)}, // iOS, Novadrome
            { "6.1", new Version(6,1)}, // Novadrome
            { "6.2", new Version(6,2)}  // C:R
        };

        static bool bDebug = false;
        int faceCount;
        int vertexCount;
        Version version;
        string name;
        int flags = 1;

        List<MDLMesh> meshes = new List<MDLMesh>();
        List<MDLFace> faces = new List<MDLFace>();
        List<MDLVertex> verts = new List<MDLVertex>();

        MDLExtents extents = new MDLExtents();

        public string Name { get { return (name != null ? name : "Unknown Mesh"); } }
        public List<MDLMesh> Meshes { get { return meshes; } }
        public int FaceCount { get { return faceCount; } }
        public int VertexCount { get { return vertexCount; } }

        public List<MDLFace> Faces { get { return faces; } set { faces = value; } }
        public List<MDLVertex> Vertices { get { return verts; } set { verts = value; } }

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
                    br.ReadByte() != 35)
                {
                    Logger.LogToFile("{0} isn't a valid MDL file", Path);
                    return null;
                }

                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                mdl.version = new Version(major, minor);

                if (!MDL.SupportedVersions.ContainsKey(mdl.version.ToString()))
                {
                    Logger.LogToFile("Unsupported MDL version: v{0}", mdl.version.ToString());
                    return null;
                }

                Logger.LogToFile("MDL v{0}", mdl.version.ToString());

                // TODO: v5.6
                // Ref : F:\Novadrome_Demo\Novadrome_Demo\WADs\data\DATA\SFX\CAR_EXPLOSION\DEBPOOL\DEBPOOL.MDL
                // Ref : C:\Users\Maxx\Downloads\Carma_iOS\Carmageddon Mobile\Data_IOS\DATA\CONTENT\SFX\SHRAPNEL.MDL
                // 01 00 00 00 EE 02 00 00 02 00 00 00 04 00 00 00 01 00 00 00 49 33 35 3F 89 41 00 BF 18 B7 51 BA 89 41 00 BF 00 00 00 3F 52 49 9D 3A 00 00 00 3F 00 12 03 BA 18 B7 51 39 00 12 03 BA 01 00 66 69 72 65 70 6F 6F 6C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 17 B7 51 39 17 B7 D1 38 00 00 80 3F 17 B7 D1 B8 00 00 00 00 03 00 00 00 02 00 00 00 01 00 00 00 17 B7 51 39 17 B7 D1 B8 00 00 80 3F 17 B7 D1 38 04 00 00 00 00 00 00 3F 17 B7 D1 38 00 00 00 BF 17 B7 D1 38 00 00 80 3F 17 B7 D1 B8 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 BF 17 B7 51 39 00 00 00 BF 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 3F 17 B7 51 39 00 00 00 3F 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 BF 17 B7 D1 38 00 00 00 3F 17 B7 D1 B8 00 00 80 3F 17 B7 D1 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 80 80 FF 01 00 00 00 00 00 17 B7 D1 38 00 00 00 00 00 00 00 00 04 00 00 00 04 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF

                if (bDebug)
                {
                    var bytes = br.ReadBytes(4);
                    Logger.LogToFile("{0}\t{1}", br.BaseStream.Position.ToString("X"), BitConverter.ToString(bytes).Replace("-", " "));
                }
                else
                {
                    br.ReadBytes(4);    // Some sort of hash/derived value, files with identical verts/faces share the same value here
                }
                
                mdl.flags = (int)br.ReadUInt32();
                Logger.LogToFile("Flags {0}", (Flags)mdl.flags);

                Logger.LogToFile("Remaining A: {0} ({1})", br.ReadUInt32(), br.BaseStream.Length - br.BaseStream.Position); // Bytes remaining

                int headerFaceCount = (int)br.ReadUInt32();
                int headerVertCount = (int)br.ReadUInt32();

                Logger.LogToFile("Faces: {0}", headerFaceCount);
                Logger.LogToFile("Verts: {0}", headerVertCount);

                Logger.LogToFile("Remaining B: {0} ({1})", br.ReadUInt32(), br.BaseStream.Length - br.BaseStream.Position); // Bytes remaining

                if (bDebug) { Logger.LogToFile("{0}", br.ReadSingle()); } else { br.ReadSingle(); }
                mdl.extents.Min = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                mdl.extents.Max = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                br.ReadBytes(12); // Centre v3*3

                int nameCount = br.ReadInt16();

                Logger.LogToFile("Name count: {0}", nameCount);
                for (int i = 0; i < nameCount; i++)
                {
                    string meshName;        // Is also the name of the material
                    int nameLength = -1;
                    int padding = -1;

                    if (mdl.version.Major < 6)
                    {
                        meshName = br.ReadBytes(32).ToName();
                    }
                    else
                    {
                        nameLength = (int)br.ReadInt32();
                        padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + (mdl.version.Major == 6 && mdl.version.Minor > 0 ? 4 : 0);
                        meshName = br.ReadString(nameLength);
                        br.ReadBytes(padding);
                    }

                    mdl.meshes.Add(new MDLMesh(meshName));
                    Logger.LogToFile("Added name \"{0}\" of length {1}, padding of {2}", meshName, nameLength, padding);
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

                int meshCount = br.ReadUInt16();
                Logger.LogToFile("This is usually {0}: {1}", nameCount, meshCount);

                for (int i = 0; i < meshCount; i++)
                {
                    Logger.LogToFile("Block {0} of {1}", i, nameCount);
                    Logger.LogToFile("Position: {0}", br.BaseStream.Position.ToString("X"));

                    var mesh = mdl.meshes[i];

                    for (int j = 0; j < 10; j++)
                    {
                        var bytes = br.ReadBytes(4);
                        Logger.LogToFile("{0}\t{1}\t{2}", br.BaseStream.Position.ToString("X"), BitConverter.ToString(bytes).Replace("-", " "), BitConverter.ToSingle(bytes, 0));
                    }

                    mesh.StripOffset = (int)br.ReadUInt32();
                    int a = (int)br.ReadUInt32();   // a == Actual Verts where Name count == 1.
                    int b = (int)br.ReadUInt32();
                    Logger.LogToFile("Offset: {0}", mesh.StripOffset);
                    Logger.LogToFile("{0} is less than {1}: {2}", a, b, a < b);

                    //if (a != 0 && b != 0) { return null; }

                    int degenerateTriangles = 0;

                    for (int j = 0; j < b; j++)
                    {
                        int index = br.ReadUInt16();
                        bool bDegenerate = (br.ReadUInt16() != 0);

                        mesh.StripList.Add(new MDLPoint(index + mesh.StripOffset, bDegenerate));

                        if (bDebug) { Logger.LogToFile("{0}] {1} {2} ({3})", j, index + mesh.StripOffset, mdl.verts[index + mesh.StripOffset].ToString(), bDegenerate); }
                        if (bDegenerate) { degenerateTriangles++; }
                    }

                    Logger.LogToFile("Total degenerates {0}", degenerateTriangles);

                    // After the mesh has been described with TriangleStrips we have to patch any remaining holes

                    mesh.PatchOffset = (int)br.ReadUInt32();
                    int uB = (int)br.ReadUInt32();
                    int patchVertCount = (int)br.ReadUInt32();

                    if (uB != patchVertCount) { Logger.LogToFile("Patch Verts mismatch: {0} != {1}", uB, patchVertCount); }

                    for (int j = 0; j < patchVertCount; j++) { mesh.PatchList.Add(new MDLPoint(mesh.PatchOffset + (int)br.ReadUInt32())); }
                }

                Logger.LogToFile("Position: {0}", br.BaseStream.Position.ToString("X"));

                Logger.LogToFile("Always 0: {0}", br.ReadUInt32());

                // v5.6 successfully parses from this point down

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
                    br.ReadBytes((mdl.version.Major == 5 && mdl.version.Minor == 6 ? 133 : 137));
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

                if (br.BaseStream.Position != br.BaseStream.Length)
                {
                    Logger.LogToFile("Data still to process.  Reached {0}", br.BaseStream.Position.ToString("X"));
                }
            }

            return mdl;
        }

        public MDLMesh GetMesh(int index)
        {
            return meshes[index];
        }

        public void Save(string Path)
        {
            int nameLength, padding;
            this.CalculateExtents();

            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path, FileMode.Create)))
            {
                bw.Write(new byte[] { 69, 35, 2, 6 });

                bw.Write(new byte[] { 0, 0, 0, 0 });    // No idea
                bw.Write(this.flags);
                bw.Write(new byte[] { 0, 0, 0, 0 });    // No idea

                bw.Write(this.faces.Count);
                bw.Write(this.verts.Count);

                bw.Write(0);    // Back filled post save

                bw.Write(this.extents.HalfLongestAxis);
                bw.Write(this.extents.Min.X);
                bw.Write(this.extents.Min.Y);
                bw.Write(this.extents.Min.Z);
                bw.Write(this.extents.Max.X);
                bw.Write(this.extents.Max.Y);
                bw.Write(this.extents.Max.Z);
                bw.Write(this.extents.Centre.X);
                bw.Write(this.extents.Centre.Y);
                bw.Write(this.extents.Centre.Z);

                bw.Write((short)this.Meshes.Count);

                for (int i = 0; i < this.Meshes.Count; i++)
                {
                    nameLength = this.Meshes[i].Name.Length;
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + 4;

                    bw.Write(nameLength);
                    bw.WriteString(this.Meshes[i].Name);
                    bw.Write(new byte[padding]);
                }

                bw.Write(this.faces.Count);

                for (int i = 0; i < this.faces.Count; i++)
                {
                    bw.Write(this.faces[i].MaterialID);
                    bw.Write(this.faces[i].V1);
                    bw.Write(this.faces[i].V2);
                    bw.Write(this.faces[i].V3);
                }

                bw.Write(this.verts.Count);

                for (int i = 0; i < this.verts.Count; i++)
                {
                    bw.Write(this.verts[i].Position.X);
                    bw.Write(this.verts[i].Position.Y);
                    bw.Write(this.verts[i].Position.Z);

                    bw.Write(this.verts[i].Normal.X);
                    bw.Write(this.verts[i].Normal.Y);
                    bw.Write(this.verts[i].Normal.Z);

                    bw.Write(this.verts[i].UV.X);
                    bw.Write(this.verts[i].UV.Y);

                    bw.Write(this.verts[i].UV.X);
                    bw.Write(this.verts[i].UV.Y);

                    bw.Write((byte)255); // R
                    bw.Write((byte)255); // G
                    bw.Write((byte)255); // B
                    bw.Write((byte)255); // A
                }

                bw.Write((short)this.meshes.Count);

                for (int i = 0; i < this.meshes.Count; i++)
                {
                    var mesh = this.meshes[i];

                    for (int j = 0; j < 10; j++)
                    {
                        bw.Write(new byte[] { 0, 0, 0, 0 });    // No idea
                    }

                    // TriangleStrips - TO DO
                    bw.Write(0);    // offset
                    bw.Write(0);
                    bw.Write(0);

                    bw.Write(mesh.PatchOffset);
                    bw.Write(mesh.PatchList.Count);
                    bw.Write(mesh.PatchList.Count);

                    for (int j = 0; j < mesh.PatchList.Count; j++)
                    {
                        bw.Write(mesh.PatchList[j].Index);
                    }
                }

                bw.Write(0);

                for (int i = 0; i < this.verts.Count; i++)
                {
                    bw.Write(this.verts[i].Position.X);
                    bw.Write(this.verts[i].Position.Y);
                    bw.Write(this.verts[i].Position.Z);
                    bw.Write(1);
                }

                for (int i = 0; i < this.faces.Count; i++)
                {
                    bw.Write(0);
                    bw.Write(this.verts[this.faces[i].V1].Normal.X);
                    bw.Write(this.verts[this.faces[i].V1].Normal.Y);
                    bw.Write(this.verts[this.faces[i].V1].Normal.Z);
                    bw.Write(this.verts[this.faces[i].V2].Normal.X);
                    bw.Write(this.verts[this.faces[i].V2].Normal.Y);
                    bw.Write(this.verts[this.faces[i].V2].Normal.Z);
                    bw.Write(this.verts[this.faces[i].V3].Normal.X);
                    bw.Write(this.verts[this.faces[i].V3].Normal.Y);
                    bw.Write(this.verts[this.faces[i].V3].Normal.Z);
                    bw.Write(this.verts[this.faces[i].V3].Normal.X);
                    bw.Write(this.verts[this.faces[i].V3].Normal.Y);
                    bw.Write(this.verts[this.faces[i].V3].Normal.Z);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(3);
                    bw.Write(1);
                    bw.Write(0);
                    bw.Write((byte)128); bw.Write((byte)128); bw.Write((byte)128); bw.Write((byte)255);
                    bw.Write((byte)128); bw.Write((byte)128); bw.Write((byte)128); bw.Write((byte)255);
                    bw.Write((byte)128); bw.Write((byte)128); bw.Write((byte)128); bw.Write((byte)255);
                    bw.Write(this.verts[this.faces[i].V1].UV.X);
                    bw.Write(this.verts[this.faces[i].V1].UV.Y);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(this.verts[this.faces[i].V3].UV.X);
                    bw.Write(this.verts[this.faces[i].V3].UV.Y);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(this.verts[this.faces[i].V2].UV.X);
                    bw.Write(this.verts[this.faces[i].V2].UV.Y);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write((byte)0);
                }

                for (int i = 0; i < this.faces.Count; i++)
                {
                    bw.Write(i);
                }

                bw.Write(this.verts.Count);

                for (int i = 0; i < this.verts.Count; i++)
                {
                    bw.Write(i);
                }
            }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(Path, FileMode.Open)))
            {
                bw.Seek(24, SeekOrigin.Begin);
                bw.Write((int)(bw.BaseStream.Length - 28));
            }
        }

        public void CalculateExtents()
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < this.verts.Count; i++)
            {
                if (this.verts[i].Position.X < min.X) { min.X = this.verts[i].Position.X; }
                if (this.verts[i].Position.Y < min.Y) { min.Y = this.verts[i].Position.Y; }
                if (this.verts[i].Position.Z < min.Z) { min.Z = this.verts[i].Position.Z; }

                if (this.verts[i].Position.X > max.X) { max.X = this.verts[i].Position.X; }
                if (this.verts[i].Position.Y > max.Y) { max.Y = this.verts[i].Position.Y; }
                if (this.verts[i].Position.Z > max.Z) { max.Z = this.verts[i].Position.Z; }
            }

            this.extents.Min = min;
            this.extents.Max = max;
        }
    }

    public class MDLFace
    {
        int materialID;
        int vertexA;
        int vertexB;
        int vertexC;

        public int MaterialID { get { return materialID; } }
        public int V1 { get { return vertexA; } }
        public int V2 { get { return vertexB; } }
        public int V3 { get { return vertexC; } }

        public MDLFace(int MaterialID, int A, int B, int C)
        {
            this.materialID = MaterialID;
            this.vertexA = A;
            this.vertexB = B;
            this.vertexC = C;
        }

        public override string ToString()
        {
            return "{ Face: {A:" + vertexA + " B:" + vertexB + " C:" + vertexC + "} Material: " + materialID + " }";
        }
    }

    public class MDLExtents
    {
        Vector3 min;
        Vector3 max;

        public Vector3 Min
        {
            get { return min; }
            set { min = value; }
        }

        public Vector3 Max
        {
            get { return max; }
            set { max = value; }
        }

        public Vector3 Centre { get { return max - min; } }
        public Single HalfLongestAxis { get { return (min + max).Length; } }
    }

    public class MDLVertex
    {
        Vector3 position;
        Vector3 normal;
        Vector2 uv;
        float ua;
        float ub;

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

            ua = Unk6;
            ub = Unk7;
            //Logger.LogToFile("Unknown data: {0} {1}", Unk6, Unk7);
        }

        public override string ToString()
        {
            return "{ Position: {X:" + Position.X + " Y:" + Position.Y + " Z:" + Position.Z + "} Normal: {X:" + Normal.X + " Y:" + Normal.Y + " Z:" + Normal.Z + "} UV: {U:" + UV.X + " V:" + UV.Y + "} Unknown: {A:" + ua + " B:" + ub + "} }";
        }
    }

    public class MDLPoint
    {
        int index;
        bool bDegenerate;

        public int Index { get { return index; } }

        public MDLPoint(int Index, bool IsDegenerate = false)
        {
            index = Index;
            bDegenerate = IsDegenerate;
        }
    }

    public class MDLMesh
    {
        string name;
        int stripOffset;
        int patchOffset;
        List<MDLPoint> stripList;
        List<MDLPoint> patchList;

        public string Name { get { return name; } }
        public int StripOffset { get { return stripOffset; } set { stripOffset = value; } }
        public int PatchOffset { get { return patchOffset; } set { patchOffset = value; } }
        public List<MDLPoint> StripList { get { return stripList; } set { stripList = value; } }
        public List<MDLPoint> PatchList { get { return patchList; } set { patchList = value; } }

        public MDLMesh(string Name)
        {
            name = Name;
            stripList = new List<MDLPoint>();
            patchList = new List<MDLPoint>();
        }
    }
}
