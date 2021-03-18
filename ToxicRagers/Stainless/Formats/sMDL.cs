using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class MDL
    {
        [Flags]
        public enum Flags
        {
            USERData = 1,
            SubdivideMaterialGroups = 2,
            ClipTestMaterialGroups = 4,
            USERSkinData = 8,
            SphereNormals = 16,
            PREPSkinData = 32,
            CompressPrepData = 64,
            PreferBoundingBox = 128,
            Use32bitPrep = 256,
            LODData = 512
        }

        public static List<Version> SupportedVersions = new List<Version>
        {
            //new Version(5,6), // iOS, Novadrome
            new Version(5,9), // iOS, Novadrome
            new Version(6,0), // iOS, Novadrome
            new Version(6,1), // Novadrome
            new Version(6,2), // C:R
            new Version(6,3)  // C:R
        };

        readonly List<MDLUserVertexEntry> userVertexList = new List<MDLUserVertexEntry>();
        readonly List<MDLUserFaceEntry> userFaceList = new List<MDLUserFaceEntry>();
        readonly List<MDLBone> prepBoneList = new List<MDLBone>();
        readonly List<int> ptouVertexLookup = new List<int>();
        readonly List<int> ptouFaceLookup = new List<int>();
        readonly List<MDLPrepSkinWeightLookup> prepVertSkinWeightLookup = new List<MDLPrepSkinWeightLookup>();

        public MDLExtents Extents { get; set; } = new MDLExtents();

        public Version Version { get; set; }

        public string Name { get; set; } = "Unknown Mesh";

        public int FileSize { get; set; }

        public List<MDLMaterialGroup> Meshes { get; } = new List<MDLMaterialGroup>();

        public int PREPFaceCount { get; private set; }

        public int USERFaceCount { get; private set; }

        public int PREPVertexCount { get; private set; }

        public int USERVertexCount { get; private set; }

        public Flags UserFlags { get; private set; }

        public List<MDL> LODs { get; set; } = new List<MDL>();

        public List<MDLFace> Faces { get; set; } = new List<MDLFace>();

        public List<MDLVertex> Vertices { get; set; } = new List<MDLVertex>();

        public int Checksum { get; set; }

        public Flags ModelFlags { get; set; }

        public int PrepDataSize { get; set; }

        public static MDL Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            MDL mdl = new MDL { Name = Path.GetFileNameWithoutExtension(path) };

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            {
                mdl = Load(ms, Path.GetFileNameWithoutExtension(path));
            }

            return mdl;
        }

        public static MDL Load(Stream stream, string name = null)
        {
            MDL mdl = new MDL { Name = name };

            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
            {
                if (br.ReadByte() != 0x45 ||
                    br.ReadByte() != 0x23)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "This isn't a valid MDL file");
                    return null;
                }

                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                mdl.Version = new Version(major, minor);

                if (!SupportedVersions.Contains(mdl.Version))
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Unsupported MDL version: v{0}", mdl.Version.ToString());
                    return null;
                }

                Logger.LogToFile(Logger.LogLevel.Info, "MDL v{0}", mdl.Version.ToString());

                // TODO: v5.6
                // Ref : Novadrome_Demo\WADs\data\DATA\SFX\CAR_EXPLOSION\DEBPOOL\DEBPOOL.MDL
                // Ref : Carmageddon Mobile\Data_IOS\DATA\CONTENT\SFX\SHRAPNEL.MDL
                // 01 00 00 00 EE 02 00 00 02 00 00 00 04 00 00 00 01 00 00 00 49 33 35 3F 89 41 00 BF 18 B7 51 BA 89 41 00 BF 00 00 00 3F 52 49 9D 3A 00 00 00 3F 00 12 03 BA 18 B7 51 39 00 12 03 BA 01 00 66 69 72 65 70 6F 6F 6C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 17 B7 51 39 17 B7 D1 38 00 00 80 3F 17 B7 D1 B8 00 00 00 00 03 00 00 00 02 00 00 00 01 00 00 00 17 B7 51 39 17 B7 D1 B8 00 00 80 3F 17 B7 D1 38 04 00 00 00 00 00 00 3F 17 B7 D1 38 00 00 00 BF 17 B7 D1 38 00 00 80 3F 17 B7 D1 B8 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 BF 17 B7 51 39 00 00 00 BF 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 3F 17 B7 51 39 00 00 00 3F 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 BF 17 B7 D1 38 00 00 00 3F 17 B7 D1 B8 00 00 80 3F 17 B7 D1 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 80 80 FF 01 00 00 00 00 00 17 B7 D1 38 00 00 00 00 00 00 00 00 04 00 00 00 04 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF

                mdl.Checksum = (int)br.ReadUInt32();

                mdl.ModelFlags = (Flags)br.ReadUInt32();
                Logger.LogToFile(Logger.LogLevel.Debug, "Flags {0}", (Flags)mdl.ModelFlags);

                mdl.PrepDataSize = (int)br.ReadUInt32();    // PREP data size

                mdl.USERFaceCount = (int)br.ReadUInt32();
                mdl.USERVertexCount = (int)br.ReadUInt32();

                Logger.LogToFile(Logger.LogLevel.Debug, "USER Faces: {0}", mdl.USERFaceCount);
                Logger.LogToFile(Logger.LogLevel.Debug, "USER Verts: {0}", mdl.USERVertexCount);

                mdl.FileSize = (int)br.ReadUInt32();

                mdl.Extents.Radius = br.ReadSingle();
                mdl.Extents.Min = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                mdl.Extents.Max = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                br.ReadBytes(12);   //  BoundingBox centre, Flummery auto calculates from min and max

                int materialCount = br.ReadInt16();

                Logger.LogToFile(Logger.LogLevel.Debug, "Material count: {0}", materialCount);

                for (int i = 0; i < materialCount; i++)
                {
                    string materialName;
                    int nameLength;
                    int padding;

                    if (mdl.Version.Major < 6)
                    {
                        materialName = br.ReadBytes(32).ToName();
                    }
                    else
                    {
                        nameLength = br.ReadInt32();
                        padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + (mdl.Version.Major == 6 && mdl.Version.Minor > 0 ? 4 : 0);
                        materialName = br.ReadString(nameLength);
                        br.ReadBytes(padding);
                    }

                    mdl.Meshes.Add(new MDLMaterialGroup(i, materialName));
                }

                // START PREP DATA
                mdl.PREPFaceCount = (int)br.ReadUInt32();

                Logger.LogToFile(Logger.LogLevel.Debug, "PREP Faces: {0}", mdl.PREPFaceCount);

                for (int i = 0; i < mdl.PREPFaceCount; i++)
                {
                    MDLFace face = new MDLFace(
                        br.ReadUInt16(),        // Material index
                        br.ReadUInt16(),        // Material Flags
                        (int)br.ReadUInt32(),   // Vert index A
                        (int)br.ReadUInt32(),   // Vert index B
                        (int)br.ReadUInt32()    // Vert index C
                    );

                    mdl.Faces.Add(face);

                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} : {1}", i, face);
                }

                mdl.PREPVertexCount = (int)br.ReadUInt32();

                Logger.LogToFile(Logger.LogLevel.Debug, "PREP Verts: {0}", mdl.PREPVertexCount);

                for (int i = 0; i < mdl.PREPVertexCount; i++)
                {
                    MDLVertex vert = new MDLVertex(
                        br.ReadSingle(),        // X
                        br.ReadSingle(),        // Y
                        br.ReadSingle(),        // Z
                        br.ReadSingle(),        // N.X
                        br.ReadSingle(),        // N.Y
                        br.ReadSingle(),        // N.Z
                        br.ReadSingle(),        // U
                        br.ReadSingle(),        // V
                        br.ReadSingle(),        // U2
                        br.ReadSingle(),        // V2
                        br.ReadByte(),          // R
                        br.ReadByte(),          // G
                        br.ReadByte(),          // B
                        br.ReadByte()           // A
                    );

                    mdl.Vertices.Add(vert);

                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} : {1}", i, vert);
                }

                int materialGroups = br.ReadUInt16();

                for (int i = 0; i < materialGroups; i++)
                {
                    MDLMaterialGroup mesh = mdl.Meshes[i];

                    br.ReadBytes(12);   // BoundingBox Centre, we recalculate it from Min and Max
                    mesh.Extents.Radius = br.ReadSingle();
                    mesh.Extents.Min = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    mesh.Extents.Max = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                    mesh.StripOffset = (int)br.ReadUInt32();
                    mesh.StripVertCount = (int)br.ReadUInt32();
                    int stripPointCount = (int)br.ReadUInt32();

                    Logger.LogToFile(Logger.LogLevel.Info, "{0} : {1} : {2}", mesh.StripOffset, mesh.StripVertCount, stripPointCount);

                    for (int j = 0; j < stripPointCount; j++)
                    {
                        uint index = br.ReadUInt32();
                        bool bDegenerate = ((index & 0x80000000) != 0);
                        index &= ~0x80000000;

                        mesh.StripList.Add(new MDLPoint((int)index + mesh.StripOffset, bDegenerate));

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0} ] {1} : {2}", j, index, bDegenerate);
                    }

                    mesh.TriListOffset = (int)br.ReadUInt32();
                    mesh.TriListVertCount = (int)br.ReadUInt32();
                    int listPointCount = (int)br.ReadUInt32();

                    Logger.LogToFile(Logger.LogLevel.Info, "{0} : {1} : {2}", mesh.TriListOffset, mesh.TriListVertCount, listPointCount);

                    for (int j = 0; j < listPointCount; j++)
                    {
                        uint index = br.ReadUInt32();

                        mesh.TriList.Add(new MDLPoint((int)index + mesh.TriListOffset));

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0} ] {1}", j, index);
                    }
                }

                if (mdl.ModelFlags.HasFlag(Flags.PREPSkinData))
                {
                    Logger.LogToFile(Logger.LogLevel.Debug, "Processing PREP skin data");

                    int bodyPartCount = br.ReadUInt16();
                    int maxBonesPerVertex = br.ReadUInt16();
                    int rootBoneIndex = br.ReadUInt16();
                    string[] boneNames = br.ReadStrings(bodyPartCount);

                    Logger.LogToFile(Logger.LogLevel.Debug, "Body Part Count: {0}. Max Bones per Vertex : {1}. Root Bone Index : {2}", bodyPartCount, maxBonesPerVertex, rootBoneIndex);

                    for (int i = 0; i < bodyPartCount; i++)
                    {
                        MDLBone bone = new MDLBone()
                        {
                            Name = boneNames[i],
                            MinExtents = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),   // Min and Max bone local space
                            MaxExtents = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                            Offset = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),       // Offset is in parents local space

                            Parent = br.ReadByte(),
                            Child = br.ReadByte(),
                            Sibling = br.ReadByte()
                        };

                        mdl.prepBoneList.Add(bone);
                    }

                    for (int i = 0; i < bodyPartCount; i++)
                    {
                        mdl.prepBoneList[i].Rotation = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        mdl.prepBoneList[i].Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        br.ReadBytes(4);

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}) {1}", i, mdl.prepBoneList[i].Name);
                        Logger.LogToFile(Logger.LogLevel.Debug, "P{0} C{1} S{2}", mdl.prepBoneList[i].Parent, mdl.prepBoneList[i].Child, mdl.prepBoneList[i].Sibling);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].MinExtents);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].MaxExtents);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].Offset);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].Rotation);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].Position);
                        Logger.LogToFile(Logger.LogLevel.Debug, "");
                    }

                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP skin vert weight table");

                    for (int i = 0; i < mdl.PREPVertexCount; i++)
                    {
                        int weightCount = br.ReadUInt16();
                        br.ReadBytes(2);
                        int weightOffset = (int)br.ReadUInt32();

                        mdl.prepVertSkinWeightLookup.Add(new MDLPrepSkinWeightLookup { Count = weightCount, Index = weightOffset });

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0,5}]  {1} @ {2}", i, weightCount, weightOffset);
                    }

                    int prepSkinWeightCount = (int)br.ReadUInt32();

                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP Skin Weight Count: {0}", prepSkinWeightCount);

                    for (int i = 0; i < prepSkinWeightCount; i++)
                    {
                        int boneIndex = br.ReadUInt16();
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0,5}] {1}", i, boneIndex);
                    }

                    for (int i = 0; i < prepSkinWeightCount; i++)
                    {
                        float weight = br.ReadSingle();
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0,5}] {1}", i, weight);
                    }
                }
                // END PREP DATA

                if (mdl.ModelFlags.HasFlag(Flags.LODData))
                {
                    ushort lodLevel;

                    do
                    {
                        lodLevel = br.ReadUInt16();
                        int nameLength = br.ReadInt32();

                        if (nameLength > 0)
                        {
                            int padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + (mdl.Version.Major == 6 && mdl.Version.Minor > 0 ? 4 : 0);
                            string lodLevelName = br.ReadString(nameLength);
                            br.ReadBytes(padding);

                            mdl.LODs.Add(Load(br.BaseStream, lodLevelName));
                        }
                    } while (lodLevel < 4);
                }

                // START USER DATA
                if (mdl.ModelFlags.HasFlag(Flags.USERData))
                {
                    mdl.UserFlags = (Flags)br.ReadUInt32();

                    // v5.6 successfully parses from this point down

                    Logger.LogToFile(Logger.LogLevel.Debug, "USER vertex list with index count");
                    for (int i = 0; i < mdl.USERVertexCount; i++)
                    {
                        mdl.userVertexList.Add(new MDLUserVertexEntry(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), (int)br.ReadUInt32()));
                    }

                    Logger.LogToFile(Logger.LogLevel.Debug, "USER face data");
                    for (int i = 0; i < mdl.USERFaceCount; i++)
                    {
                        if (mdl.Version.Major == 5 && mdl.Version.Minor == 6)
                        {
                            br.ReadBytes(133);
                        }
                        else
                        {
                            mdl.userFaceList.Add(
                                new MDLUserFaceEntry(
                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), // plane equation
                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),     // vertex[0] normal
                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),     // vertex[1] normal
                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),     // vertex[2] normal
                                        (int)br.ReadUInt32(),                                               // material index
                                        (int)br.ReadUInt32(),                                               // smoothing group
                                        (int)br.ReadUInt32(),                                               // vertex[0]
                                        (int)br.ReadUInt32(),                                               // vertex[1]
                                        (int)br.ReadUInt32(),                                               // vertex[2]
                                        br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),         // colour[0] RGBA
                                        br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),         // colour[1] RGBA
                                        br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),         // colour[2] RGBA
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv[0]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv2[0]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv[1]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv2[1]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv[2]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv2[2]
                                        br.ReadByte(),                                                      // flags
                                        (int)br.ReadUInt32()                                                // application specific flags
                                ) // 137 bytes || 0x89
                            );
                        }
                    }

                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP to USER face lookup");
                    for (int i = 0; i < mdl.PREPFaceCount; i++)
                    {
                        mdl.ptouFaceLookup.Add((int)br.ReadUInt32());
                    }

                    int prepVertexMapCount = (int)br.ReadUInt32();
                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP to USER vertex lookup");

                    for (int i = 0; i < prepVertexMapCount; i++)
                    {
                        mdl.ptouVertexLookup.Add((int)br.ReadUInt32());
                    }

                    if (mdl.UserFlags.HasFlag(Flags.USERSkinData))
                    {
                        Logger.LogToFile(Logger.LogLevel.Debug, "Processing USER skin data");

                        int boneCount = br.ReadUInt16();

                        Logger.LogToFile(Logger.LogLevel.Debug, "Bone count: {0}", boneCount);
                        for (int i = 0; i < boneCount; i++)
                        {
                            string boneName = br.ReadString(32);
                            short parentBoneIndex = br.ReadInt16();
                            Matrix3D boneTransform = new Matrix3D(
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                                        );

                            Logger.LogToFile(Logger.LogLevel.Debug, "{0}) {1}", i, boneName);
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0}", parentBoneIndex);
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0}", boneTransform);
                            Logger.LogToFile(Logger.LogLevel.Debug, "");
                        }

                        int userDataCount = (int)br.ReadUInt32();
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0} == {1}", userDataCount, mdl.USERVertexCount);

                        for (int i = 0; i < mdl.USERVertexCount; i++)
                        {
                            int entryCount = br.ReadUInt16();

                            for (int j = 0; j < entryCount; j++)
                            {
                                int boneIndex = br.ReadUInt16();
                                float weight = br.ReadSingle();
                                Vector3 vertexPosition = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Logger.LogToFile(Logger.LogLevel.Debug, "{0}.{1}]  {2,2} : {3,6:0.00}% : {4}", i, j, boneIndex, (weight * 100.0f), vertexPosition);
                            }
                        }
                    }
                }

                if (br.BaseStream.Position != br.BaseStream.Length) { Logger.LogToFile(Logger.LogLevel.Warning, "Still has data remaining (processed {0:x2} of {1:x2}", br.BaseStream.Position, br.BaseStream.Length); }
            }

            return mdl;
        }

        public MDLMaterialGroup GetMesh(int index)
        {
            return Meshes[index];
        }

        public void Save(string path)
        {
            int nameLength, padding;
            CalculateExtents();

            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                bw.Write(new byte[] { 0x45, 0x23 });    // Magic Number
                bw.Write(new byte[] { 2, 6 });          // Version (6.2)

                bw.Write(new byte[] { 0, 0, 0, 0 });    // Checksum, to calculate
                bw.Write((int)ModelFlags);

                int prepDataSize = 4 + (Faces.Count * 16) + 4 + (Vertices.Count * 44) + 2;

                for (int i = 0; i < Meshes.Count; i++)
                {
                    MDLMaterialGroup mesh = Meshes[i];

                    prepDataSize += 52;
                    prepDataSize += (4 * mesh.StripList.Count);

                    prepDataSize += 12;
                    prepDataSize += (4 * mesh.TriList.Count);
                }

                bw.Write(prepDataSize);                 // PREP data size

                bw.Write(Faces.Count);             // USER face count
                bw.Write(Vertices.Count);             // USER vert count

                bw.Write(0);                            // Back filled post save

                bw.Write(Extents.Radius);
                bw.Write(Extents.Min.X);
                bw.Write(Extents.Min.Y);
                bw.Write(Extents.Min.Z);
                bw.Write(Extents.Max.X);
                bw.Write(Extents.Max.Y);
                bw.Write(Extents.Max.Z);
                bw.Write(Extents.Centre.X);
                bw.Write(Extents.Centre.Y);
                bw.Write(Extents.Centre.Z);

                bw.Write((short)Meshes.Count);

                for (int i = 0; i < Meshes.Count; i++)
                {
                    nameLength = Meshes[i].Name.Length;
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + 4;

                    bw.Write(nameLength);
                    bw.WriteString(Meshes[i].Name);
                    bw.Write(new byte[padding]);
                }

                bw.Write(Faces.Count);

                for (int i = 0; i < Faces.Count; i++)
                {
                    bw.Write((short)Faces[i].MaterialID);
                    bw.Write((short)0);
                    bw.Write(Faces[i].Verts[0]);
                    bw.Write(Faces[i].Verts[1]);
                    bw.Write(Faces[i].Verts[2]);
                }

                bw.Write(Vertices.Count);

                for (int i = 0; i < Vertices.Count; i++)
                {
                    bw.Write(Vertices[i].Position.X);
                    bw.Write(Vertices[i].Position.Y);
                    bw.Write(Vertices[i].Position.Z);

                    bw.Write(Vertices[i].Normal.X);
                    bw.Write(Vertices[i].Normal.Y);
                    bw.Write(Vertices[i].Normal.Z);

                    bw.Write(Vertices[i].UV.X);
                    bw.Write(Vertices[i].UV.Y);

                    bw.Write(Vertices[i].UV2.X);
                    bw.Write(Vertices[i].UV2.Y);

                    bw.Write(Vertices[i].Colour.R);
                    bw.Write(Vertices[i].Colour.G);
                    bw.Write(Vertices[i].Colour.B);
                    bw.Write(Vertices[i].Colour.A);
                }

                bw.Write((short)Meshes.Count);

                for (int i = 0; i < Meshes.Count; i++)
                {
                    MDLMaterialGroup mesh = Meshes[i];

                    bw.Write(mesh.Extents.Centre.X);
                    bw.Write(mesh.Extents.Centre.Y);
                    bw.Write(mesh.Extents.Centre.Z);
                    bw.Write(mesh.Extents.Radius);
                    bw.Write(mesh.Extents.Min.X);
                    bw.Write(mesh.Extents.Min.Y);
                    bw.Write(mesh.Extents.Min.Z);
                    bw.Write(mesh.Extents.Max.X);
                    bw.Write(mesh.Extents.Max.Y);
                    bw.Write(mesh.Extents.Max.Z);

                    // TriangleStrips
                    bw.Write(mesh.StripOffset);
                    bw.Write(mesh.StripVertCount);
                    bw.Write(mesh.StripList.Count);

                    for (int j = 0; j < mesh.StripList.Count; j++)
                    {
                        bw.Write((uint)mesh.StripList[j].Index | (mesh.StripList[j].Degenerate ? 0x80000000 : 0x0));
                    }

                    bw.Write(mesh.TriListOffset);
                    bw.Write(mesh.TriListVertCount);
                    bw.Write(mesh.TriList.Count);

                    for (int j = 0; j < mesh.TriList.Count; j++)
                    {
                        bw.Write(mesh.TriList[j].Index);
                    }
                }

                if (ModelFlags.HasFlag(Flags.USERData))
                {
                    bw.Write(0);

                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        bw.Write(Vertices[i].Position.X);
                        bw.Write(Vertices[i].Position.Y);
                        bw.Write(Vertices[i].Position.Z);
                        bw.Write(1);
                    }

                    for (int i = 0; i < Faces.Count; i++)
                    {
                        Vector3 v12 = Vertices[Faces[i].Verts[1]].Normal - Vertices[Faces[i].Verts[0]].Normal;
                        Vector3 v13 = Vertices[Faces[i].Verts[2]].Normal - Vertices[Faces[i].Verts[0]].Normal;
                        Vector3 n = Vector3.Cross(v12, v13).Normalised;
                        float d = Vector3.Dot(n, Vertices[Faces[i].Verts[0]].Normal);

                        bw.Write(d);
                        bw.Write(n.X);
                        bw.Write(n.Y);
                        bw.Write(n.Z);
                        bw.Write(Vertices[Faces[i].Verts[0]].Normal.X);
                        bw.Write(Vertices[Faces[i].Verts[0]].Normal.Y);
                        bw.Write(Vertices[Faces[i].Verts[0]].Normal.Z);
                        bw.Write(Vertices[Faces[i].Verts[1]].Normal.X);
                        bw.Write(Vertices[Faces[i].Verts[1]].Normal.Y);
                        bw.Write(Vertices[Faces[i].Verts[1]].Normal.Z);
                        bw.Write(Vertices[Faces[i].Verts[2]].Normal.X);
                        bw.Write(Vertices[Faces[i].Verts[2]].Normal.Y);
                        bw.Write(Vertices[Faces[i].Verts[2]].Normal.Z);
                        bw.Write(Faces[i].MaterialID);
                        bw.Write(0);
                        bw.Write(Faces[i].Verts[0]);
                        bw.Write(Faces[i].Verts[1]);
                        bw.Write(Faces[i].Verts[2]);
                        bw.Write(Vertices[Faces[i].Verts[0]].Colour.R); bw.Write(Vertices[Faces[i].Verts[0]].Colour.G); bw.Write(Vertices[Faces[i].Verts[0]].Colour.B); bw.Write(Vertices[Faces[i].Verts[0]].Colour.A);
                        bw.Write(Vertices[Faces[i].Verts[1]].Colour.R); bw.Write(Vertices[Faces[i].Verts[1]].Colour.G); bw.Write(Vertices[Faces[i].Verts[1]].Colour.B); bw.Write(Vertices[Faces[i].Verts[1]].Colour.A);
                        bw.Write(Vertices[Faces[i].Verts[2]].Colour.R); bw.Write(Vertices[Faces[i].Verts[2]].Colour.G); bw.Write(Vertices[Faces[i].Verts[2]].Colour.B); bw.Write(Vertices[Faces[i].Verts[2]].Colour.A);
                        bw.Write(Vertices[Faces[i].Verts[0]].UV.X);
                        bw.Write(Vertices[Faces[i].Verts[0]].UV.Y);
                        bw.Write(Vertices[Faces[i].Verts[0]].UV2.X);
                        bw.Write(Vertices[Faces[i].Verts[0]].UV2.Y);
                        bw.Write(Vertices[Faces[i].Verts[1]].UV.X);
                        bw.Write(Vertices[Faces[i].Verts[1]].UV.Y);
                        bw.Write(Vertices[Faces[i].Verts[1]].UV2.X);
                        bw.Write(Vertices[Faces[i].Verts[1]].UV2.Y);
                        bw.Write(Vertices[Faces[i].Verts[2]].UV.X);
                        bw.Write(Vertices[Faces[i].Verts[2]].UV.Y);
                        bw.Write(Vertices[Faces[i].Verts[2]].UV2.X);
                        bw.Write(Vertices[Faces[i].Verts[2]].UV2.Y);
                        bw.Write((byte)0);
                        bw.Write(0);
                    }

                    for (int i = 0; i < Faces.Count; i++) { bw.Write(i); }

                    bw.Write(Vertices.Count);

                    for (int i = 0; i < Vertices.Count; i++) { bw.Write(i); }

                    if (ModelFlags.HasFlag(Flags.USERSkinData))
                    {

                    }
                }
            }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Open)))
            {
                bw.Seek(24, SeekOrigin.Begin);
                bw.Write((int)(bw.BaseStream.Length - 28));
            }
        }

        public void CalculateExtents()
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i].Position.X < min.X) { min.X = Vertices[i].Position.X; }
                if (Vertices[i].Position.Y < min.Y) { min.Y = Vertices[i].Position.Y; }
                if (Vertices[i].Position.Z < min.Z) { min.Z = Vertices[i].Position.Z; }

                if (Vertices[i].Position.X > max.X) { max.X = Vertices[i].Position.X; }
                if (Vertices[i].Position.Y > max.Y) { max.Y = Vertices[i].Position.Y; }
                if (Vertices[i].Position.Z > max.Z) { max.Z = Vertices[i].Position.Z; }
            }

            Extents.Min = min;
            Extents.Max = max;

            Extents.Radius = (float)Math.Max(
                        Math.Abs(Math.Sqrt(Math.Pow(min.X, 2) + Math.Pow(min.Y, 2) + Math.Pow(min.Z, 2))),
                        Math.Abs(Math.Sqrt(Math.Pow(max.X, 2) + Math.Pow(max.Y, 2) + Math.Pow(max.Z, 2)))
                      );
        }
    }

    public class MDLFace
    {
        public int MaterialID { get; }
        public int Flags { get; }
        public int[] Verts { get; } = new int[3];

        public MDLFace(int MaterialID, int Flags, int A, int B, int C)
        {
            this.MaterialID = MaterialID;
            this.Flags = Flags;
            Verts[0] = A;
            Verts[1] = B;
            Verts[2] = C;
        }

        public override string ToString()
        {
            return "{ Face: {A:" + Verts[0] + " B:" + Verts[1] + " C:" + Verts[2] + "} Material: " + MaterialID + " Flags: " + Flags + " }";
        }
    }

    public class MDLExtents
    {
        float radius;
        Vector3 min;
        Vector3 max;

        public Vector3 Min
        {
            get => min;
            set => min = value;
        }

        public Vector3 Max
        {
            get => max;
            set => max = value;
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public Vector3 Centre => (min + max) / 2.0f;

        public override string ToString()
        {
            return "{ Min: " + min.ToString() + ", Max: " + max.ToString() + ", Centre: " + Centre.ToString() + ", Radius: " + radius + " }";
        }
    }

    public class MDLVertex
    {
        public Vector3 Position { get; set; }

        // Alias for 3dsmax interop.
        public Vector3 Pos => Position;

        public Vector3 Normal { get; set; }

        public Vector2 UV { get; set; }

        public Vector2 UV2 { get; set; }

        public Colour Colour { get; set; }

        public MDLVertex(float X, float Y, float Z, float NX, float NY, float NZ, float U, float V, float U2, float V2, byte R, byte G, byte B, byte Alpha)
        {
            Position = new Vector3(X, Y, Z);
            Normal = new Vector3(NX, NY, NZ);
            UV = new Vector2(U, V);
            UV2 = new Vector2(U2, V2);
            Colour = Colour.FromArgb(Alpha, R, G, B);
        }

        public override string ToString()
        {
            return "{ Position: {X:" + Position.X + " Y:" + Position.Y + " Z:" + Position.Z + "} Normal: {X:" + Normal.X + " Y:" + Normal.Y + " Z:" + Normal.Z + "} UV: {U:" + UV.X + " V:" + UV.Y + "} UV2: {A:" + UV2.X + " B:" + UV2.Y + "} { UV Length:" + UV.Length + " } }";
        }
    }

    [DebuggerDisplay("Index {Index} Degenerate {Degenerate}")]
    public class MDLPoint
    {
        public int Index { get; }

        public bool Degenerate { get; }

        public MDLPoint(int Index, bool IsDegenerate = false)
        {
            this.Index = Index;
            Degenerate = IsDegenerate;
        }
    }

    public class MDLBone
    {
        public string Name { get; set; }

        public Vector3 MinExtents { get; set; }

        public Vector3 MaxExtents { get; set; }

        public Vector3 Offset { get; set; }

        public byte Parent { get; set; }

        public byte Child { get; set; }

        public byte Sibling { get; set; }

        public Vector4 Rotation { get; set; } // Actually a quat

        public Vector3 Position { get; set; }
    }

    public class MDLPrepSkinWeightLookup
    {
        public int Count { get; set; }

        public int Index { get; set; }
    }

    public class MDLPrepSkinWeight
    {
        public int BoneIndex { get; set; }

        public float Weight { get; set; }
    }

    public class MDLUserVertexEntry
    {
        public int Count { get; }

        public Vector3 Position { get; }

        public MDLUserVertexEntry(float x, float y, float z, int n)
        {
            Position = new Vector3(x, y, z);
            Count = n;
        }
    }

    public class MDLUserFaceEntry
    {
        public Vector4 Plane { get; set; }

        public Vector3 Norm1 { get; set; }

        public Vector3 Norm2 { get; set; }

        public Vector3 Norm3 { get; set; }

        public int MaterialID { get; set; }

        public int SmoothingGroup { get; set; }

        public int VertexID1 { get; set; }

        public int VertexID2 { get; set; }

        public int VertexID3 { get; set; }

        public Color Colour1 { get; set; }

        public Color Colour2 { get; set; }

        public Color Colour3 { get; set; }

        public Vector2 UV1 { get; set; }

        public Vector2 UV21 { get; set; }

        public Vector2 UV2 { get; set; }

        public Vector2 UV22 { get; set; }

        public Vector2 UV3 { get; set; }

        public Vector2 UV23 { get; set; }

        public byte Flags { get; set; }

        public int ApplicationFlags { get; set; }

        public MDLUserFaceEntry(float d, float a, float b, float c, Vector3 n1, Vector3 n2, Vector3 n3, int materialID, int smoothingGroup, int v1, int v2, int v3, byte r1, byte g1, byte b1, byte a1, byte r2, byte g2, byte b2, byte a2, byte r3, byte g3, byte b3, byte a3, Vector2 uv1, Vector2 uv21, Vector2 uv2, Vector2 uv22, Vector2 uv3, Vector2 uv23, byte flags, int applicationFlags)
        {
            Plane = new Vector4(d, a, b, c);
            Norm1 = n1;
            Norm2 = n2;
            Norm3 = n3;
            MaterialID = materialID;
            SmoothingGroup = smoothingGroup;
            VertexID1 = v1;
            VertexID2 = v2;
            VertexID3 = v3;
            Colour1 = Color.FromArgb(a1, r1, g1, b1);
            Colour2 = Color.FromArgb(a2, r2, g2, b2);
            Colour3 = Color.FromArgb(a3, r3, g3, b3);
            UV1 = uv1;
            UV21 = uv21;
            UV2 = uv2;
            UV22 = uv22;
            UV3 = uv3;
            UV23 = uv23;
            Flags = flags;
            ApplicationFlags = applicationFlags;
        }
    }

    public class MDLMaterialGroup
    {
        public int Index { get; }

        public string Name { get; }

        public int StripOffset { get; set; }

        public int StripVertCount { get; set; }

        public int TriListOffset { get; set; }

        public int TriListVertCount { get; set; }

        public List<MDLPoint> StripList { get; set; }

        public List<MDLPoint> TriList { get; set; }

        public MDLExtents Extents { get; set; }

        public MDLMaterialGroup(int Index, string Name)
        {
            this.Index = Index;
            this.Name = Name;
            StripList = new List<MDLPoint>();
            TriList = new List<MDLPoint>();
            Extents = new MDLExtents();
        }

        public void CalculateExtents(List<MDLVertex> Vertices)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (MDLPoint point in StripList)
            {
                min.X = Math.Min(Vertices[point.Index + StripOffset].Position.X, min.X);
                min.Y = Math.Min(Vertices[point.Index + StripOffset].Position.Y, min.Y);
                min.Z = Math.Min(Vertices[point.Index + StripOffset].Position.Z, min.Z);

                max.X = Math.Max(Vertices[point.Index + StripOffset].Position.X, max.X);
                max.Y = Math.Max(Vertices[point.Index + StripOffset].Position.Y, max.Y);
                max.Z = Math.Max(Vertices[point.Index + StripOffset].Position.Z, max.Z);
            }

            foreach (MDLPoint point in TriList)
            {
                min.X = Math.Min(Vertices[point.Index + TriListOffset].Position.X, min.X);
                min.Y = Math.Min(Vertices[point.Index + TriListOffset].Position.Y, min.Y);
                min.Z = Math.Min(Vertices[point.Index + TriListOffset].Position.Z, min.Z);

                max.X = Math.Max(Vertices[point.Index + TriListOffset].Position.X, max.X);
                max.Y = Math.Max(Vertices[point.Index + TriListOffset].Position.Y, max.Y);
                max.Z = Math.Max(Vertices[point.Index + TriListOffset].Position.Z, max.Z);
            }

            Extents.Min = min;
            Extents.Max = max;

            Extents.Radius = (float)Math.Max(
                                    Math.Abs(Math.Sqrt(Math.Pow(min.X, 2) + Math.Pow(min.Y, 2) + Math.Pow(min.Z, 2))),
                                    Math.Abs(Math.Sqrt(Math.Pow(max.X, 2) + Math.Pow(max.Y, 2) + Math.Pow(max.Z, 2)))
                                  );
        }
    }
}