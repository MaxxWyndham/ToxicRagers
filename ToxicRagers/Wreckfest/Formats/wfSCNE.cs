using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Wreckfest.Formats
{
    // Assumes the file has been decompressed
    public class SCNE
    {
        List<SCNEBone> bones = new List<SCNEBone>();

        public List<SCNEBone> Bones
        {
            get { return bones; }
        }

        public static SCNE Load(string path)
        {
            SCNE scne = new SCNE();
            int count;

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            using (BinaryReader br = new BinaryReader(ms, System.Text.Encoding.UTF8))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    string section = br.ReadString(4);
                    int unknown;

                    switch (section)
                    {
                        case "ldom":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            int modelCount = (int)br.ReadUInt32();
                            for (int k = 0; k < modelCount; k++)
                            {
                                Logger.LogToFile(Logger.LogLevel.Debug, "{0} of {1}", k, modelCount);

                                SCNEBone bone = new SCNEBone();
                                bone.Name = br.ReadString((int)br.ReadUInt32());

                                string dntpName = br.ReadString((int)br.ReadUInt32());
                                Logger.LogToFile(Logger.LogLevel.Debug, dntpName);

                                br.ReadString(4);   // ptnd
                                string dntpFile = br.ReadString((int)br.ReadUInt32());
                                Logger.LogToFile(Logger.LogLevel.Debug, dntpFile);

                                bone.Transform = new Matrix4D(
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                );

                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                                bool bLoop = true;

                                do
                                {
                                    section = br.ReadString(4);

                                    switch (section)
                                    {
                                        case "hsmp":
                                            unknown = (int)br.ReadUInt32();
                                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                            }
                                            break;

                                        case "hsem":
                                            unknown = (int)br.ReadUInt32();
                                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                            int meshCount = (int)br.ReadUInt32();
                                            for (int mi = 0; mi < meshCount; mi++)
                                            {
                                                Logger.LogToFile(Logger.LogLevel.Debug, "{0} of {1}", mi, meshCount);

                                                SCNEMesh mesh = new SCNEMesh();
                                                mesh.Name = br.ReadString((int)br.ReadUInt32());

                                                Logger.LogToFile(Logger.LogLevel.Debug, mesh.Name);

                                                if ((section = br.ReadString(4)) != "hctb") { throw new InvalidDataException("Expected hctb"); }
                                                unknown = (int)br.ReadUInt32();
                                                Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                int batchCount = (int)br.ReadUInt32();
                                                for (int bi = 0; bi < batchCount; bi++)
                                                {
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} of {1}", bi, batchCount);

                                                    SCNEMeshPart part = new SCNEMeshPart();

                                                    int ia = (int)br.ReadUInt32();
                                                    int ib = (int)br.ReadUInt32();
                                                    int ic = (int)br.ReadUInt32();
                                                    Single sa = br.ReadSingle();
                                                    Single sb = br.ReadSingle();
                                                    Single sc = br.ReadSingle();
                                                    Single sd = br.ReadSingle();
                                                    Single se = br.ReadSingle();
                                                    Single sf = br.ReadSingle();
                                                    Single sg = br.ReadSingle();

                                                    Logger.LogToFile(Logger.LogLevel.Info, mesh.Name);
                                                    Logger.LogToFile(Logger.LogLevel.Info, "{0} : {1} : {2}", ia, ib, ic);
                                                    Logger.LogToFile(Logger.LogLevel.Info, "{0} : {1} : {2}", sa, sb, sc);
                                                    Logger.LogToFile(Logger.LogLevel.Info, "{0} : {1} : {2}", sd, se, sf);
                                                    Logger.LogToFile(Logger.LogLevel.Info, "{0}", sg);

                                                    if ((section = br.ReadString(4)) != "lrtm") { throw new InvalidDataException("Expected lrtm"); }
                                                    unknown = (int)br.ReadUInt32();
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        Logger.LogToFile(Logger.LogLevel.Debug, "{0} of {1}", i, count);

                                                        part.Materials.Add(br.ReadString((int)br.ReadUInt32()));

                                                        br.ReadUInt32();    // 0x37
                                                        br.ReadSingle();
                                                        br.ReadSingle();
                                                        br.ReadSingle();
                                                        for (int j = 0; j < 10; j++) { br.ReadUInt32(); } // 0
                                                    }

                                                    if ((section = br.ReadString(4)) != "rtxt") { throw new InvalidDataException("Expected rtxt"); }
                                                    unknown = (int)br.ReadUInt32();
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        br.ReadUInt32();
                                                        part.Textures.Add(new SCNETexture
                                                        {
                                                            Format = br.ReadString(4).ToEnum<SCNETexture.TextureType>(),
                                                            Name = br.ReadString((int)br.ReadUInt32())
                                                        });
                                                    }

                                                    if ((section = br.ReadString(4)) != "rtlc") { throw new InvalidDataException("Expected rtlc"); }
                                                    unknown = (int)br.ReadUInt32();
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                                        br.ReadUInt32();
                                                        br.ReadUInt32();
                                                        br.ReadString(4);
                                                        br.ReadString((int)br.ReadUInt32());
                                                        br.ReadString((int)br.ReadUInt32());
                                                    }

                                                    if ((section = br.ReadString(4)) != "trev") { throw new InvalidDataException("Expected trev"); }
                                                    unknown = (int)br.ReadUInt32();
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        SCNEVertex v = new SCNEVertex();
                                                        v.Position = unpack(br.ReadUInt64()).ToVector3();
                                                        v.Normal = unpackNormal(br.ReadUInt32()).ToVector3();
                                                        v.UV = unpack(br.ReadUInt32());
                                                        part.Verts.Add(v);

                                                        br.ReadBytes(16);
                                                    }

                                                    if ((section = br.ReadString(4)) != "airt") { throw new InvalidDataException("Expected airt"); }
                                                    unknown = (int)br.ReadUInt32();
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        part.IndexBuffer.Add((int)br.ReadUInt16());
                                                        part.IndexBuffer.Add((int)br.ReadUInt16());
                                                        part.IndexBuffer.Add((int)br.ReadUInt16());
                                                    }

                                                    if ((section = br.ReadString(4)) != "mgde") { throw new InvalidDataException("Expected mgde"); }
                                                    unknown = (int)br.ReadUInt32();
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                    count = (int)br.ReadUInt32();
                                                    if (count > 0) { throw new NotImplementedException("Can't handle mgde!"); }
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                    }

                                                    mesh.Parts.Add(part);
                                                }

                                                bone.Meshes.Add(mesh);
                                            }
                                            break;

                                        case "ephs":
                                            unknown = (int)br.ReadUInt32();
                                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                br.ReadUInt32();    // 1
                                                br.ReadBytes((int)br.ReadUInt32());
                                            }
                                            break;

                                        case "xbhs":
                                            unknown = (int)br.ReadUInt32();
                                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                for (int j = 0; j < 22; j++)
                                                {
                                                    br.ReadSingle();
                                                }
                                            }
                                            break;

                                        case "mina": // mina = anim
                                            unknown = (int)br.ReadUInt32();
                                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                // arfk = key frame
                                                if ((section = br.ReadString(4)) != "arfk") { throw new InvalidDataException("Expected arfk"); }
                                                unknown = (int)br.ReadUInt32();
                                                Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                int animFrameCount = (int)br.ReadUInt32();
                                                for (int ki = 0; ki < animFrameCount; ki++)
                                                {
                                                    br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                                    br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                                }
                                            }
                                            break;

                                        case "niks": // niks = skin
                                            unknown = (int)br.ReadUInt32();
                                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                // enob = bone
                                                if ((section = br.ReadString(4)) != "enob") { throw new InvalidDataException("Expected enob"); }
                                                unknown = (int)br.ReadUInt32();
                                                Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                int boneCount = (int)br.ReadUInt32();
                                                for (int bi = 0; bi < boneCount; bi++)
                                                {
                                                    br.ReadString((int)br.ReadUInt32()); // bone name

                                                    new Matrix4D(
                                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                                    );

                                                    // arfk = key frame
                                                    if ((section = br.ReadString(4)) != "arfk") { throw new InvalidDataException("Expected arfk"); }
                                                    unknown = (int)br.ReadUInt32();
                                                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                                    int boneFrameCount = (int)br.ReadUInt32();
                                                    for (int ki = 0; ki < boneFrameCount; ki++)
                                                    {
                                                        br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                                        br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                                    }
                                                }
                                            }
                                            break;

                                        case "ymmd": // ymmd = dummy, signifies the end of batch when inside a mesh and a null node when not
                                            unknown = (int)br.ReadUInt32();
                                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                new Matrix4D(
                                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                                );

                                                br.ReadString((int)br.ReadUInt32()); // null node name
                                            }
                                            break;

                                        default:
                                            // bloody unicode
                                            if (section[0] == 0xfffd &&
                                                section[1] == 0xfffd &&
                                                section[2] == 0xfffd &&
                                                section[3] == 0xfffd)
                                            {
                                                br.ReadUInt32();    // 0xe2c9 (example value, not fixed)
                                                br.ReadUInt32();    // 0
                                                bLoop = false;
                                                break;
                                            }
                                            else
                                            {
                                                throw new NotImplementedException(string.Format("Unknown section \"{0}\" at {1:x2}", section, br.BaseStream.Position));
                                            }

                                    }
                                } while (bLoop);

                                scne.bones.Add(bone);
                            }
                            break;

                        case "ymmd":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                new Matrix4D(
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                );

                                br.ReadString((int)br.ReadUInt32());
                            }
                            break;

                        case "dptl":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                new Matrix4D(
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                );

                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle();
                            }
                            break;

                        case "ecss":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                new Matrix4D(
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                );

                                br.ReadUInt32();    // 0
                                br.ReadUInt32();    // 0

                                br.ReadString((int)br.ReadUInt32());
                                br.ReadString(4);
                                br.ReadString((int)br.ReadUInt32());
                            }
                            break;

                        case "lrpa":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadUInt32();    // 0
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                            }
                            break;

                        case "tria":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            if (count > 0) { throw new NotImplementedException("Can't handle tria!"); }
                            for (int i = 0; i < count; i++)
                            {
                            }
                            break;

                        case "csia":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle();
                            }
                            br.ReadByte(); br.ReadByte(); br.ReadByte(); br.ReadByte(); // 0xff
                            br.ReadByte(); br.ReadByte(); br.ReadByte(); br.ReadByte(); // 0xff
                            br.ReadUInt32();    // 0
                            break;

                        case "psrt":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadString((int)br.ReadUInt32());

                                new Matrix4D(
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                );
                            }
                            break;

                        case "pcrt":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                            }
                            break;

                        case "mlvt":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                new Matrix4D(
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                );

                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                                br.ReadString((int)br.ReadUInt32());
                            }
                            break;

                        case "lpvt":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                            }
                            break;

                        case "fpcs":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadString(4);
                                br.ReadString((int)br.ReadUInt32());
                            }
                            break;

                        case "hpsv":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadString((int)br.ReadUInt32());

                                br.ReadSingle();
                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            }
                            break;

                        case "tcev":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            Vector4 lastValue = new Vector4(0);
                            for (int i = 0; i < count; i++)
                            {
                                lastValue = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            }

                            if (lastValue.W == 0) { br.ReadSingle(); }
                            break;

                        case "enil":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadUInt16();
                                br.ReadUInt16();
                            }
                            break;

                        case "rtet":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadUInt16();
                                br.ReadUInt16();
                                br.ReadUInt16();
                                br.ReadUInt16();
                            }
                            break;

                        case "xobv":
                            unknown = (int)br.ReadUInt32();
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0} => {1}", section, unknown);

                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadString((int)br.ReadUInt32());

                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            }
                            break;

                        default:
                            throw new NotImplementedException(string.Format("Unknown section \"{0}\" at {1:x2}", section, br.BaseStream.Position));
                    }
                }
            }

            return scne;
        }

        public static Vector4 unpack(ulong packedValue)
        {
            return new Vector4(
                ((short)(packedValue & 0xFFFF) / 4095.0f),
                ((short)((packedValue >> 0x10) & 0xFFFF) / 4095.0f),
                ((short)((packedValue >> 0x20) & 0xFFFF) / 4095.0f),
                ((short)((packedValue >> 0x30) & 0xFFFF) / 4095.0f));
        }

        public static Vector2 unpack(uint packedValue)
        {
            float x = 1 + (short)(packedValue & 0xFFFF) / 2047.0f;
            float y = 1 + (short)(packedValue >> 0x10) / 2047.0f;

            //Logger.LogToFile("{0} : {1} : {2}", packedValue, x, y);

            return new Vector2(x, y);
        }

        public static Vector4 unpackNormal(uint i)
        {
            return new Vector4(
                ((sbyte)(i & 0xFF)) / 127.0f,
                ((sbyte)((i >> 8) & 0xFF)) / 127.0f,
                ((sbyte)((i >> 16) & 0xFF)) / 127.0f,
                ((sbyte)((i >> 24) & 0xFF)) / 127.0f);
        }
    }

    public class SCNEBone
    {
        List<SCNEMesh> meshes;
        string name;
        Matrix4D matrix;

        public List<SCNEMesh> Meshes
        {
            get { return meshes; }
            set { meshes = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Matrix4D Transform
        {
            get { return matrix; }
            set { matrix = value; }
        }

        public SCNEBone()
        {
            meshes = new List<SCNEMesh>();
        }
    }

    public class SCNEMesh
    {
        List<SCNEMeshPart> parts;
        string name;

        public List<SCNEMeshPart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public SCNEMesh()
        {
            parts = new List<SCNEMeshPart>();
        }
    }

    public class SCNEMeshPart
    {
        List<string> materials;
        List<SCNETexture> textures;
        List<SCNEVertex> verts;
        List<int> indexBuffer;

        public List<string> Materials
        {
            get { return materials; }
            set { materials = value; }
        }

        public List<SCNETexture> Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        public List<SCNEVertex> Verts
        {
            get { return verts; }
            set { verts = value; }
        }

        public List<int> IndexBuffer
        {
            get { return indexBuffer; }
            set { indexBuffer = value; }
        }

        public SCNEMeshPart()
        {
            materials = new List<string>();
            textures = new List<SCNETexture>();
            verts = new List<SCNEVertex>();
            indexBuffer = new List<int>();
        }
    }

    public class SCNETexture
    {
        public enum TextureType
        {
            pamb
        }

        TextureType type;
        string name;

        public TextureType Format
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    public class SCNEVertex
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
    }
}