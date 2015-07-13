using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.Wreckfest.Formats
{
    // Assumes the file has been decompressed
    public class SCNE
    {

        List<SCNEMeshPart> meshes = new List<SCNEMeshPart>();

        public List<SCNEMeshPart> Meshes
        {
            get { return meshes; }
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

                    switch (section)
                    {
                        case "ldom":
                            br.ReadUInt32();    // 5
                            int modelCount = (int)br.ReadUInt32();
                            for (int k = 0; k < modelCount; k++)
                            {
                                br.ReadString((int)br.ReadUInt32());
                                br.ReadString((int)br.ReadUInt32());

                                br.ReadString(4);   // ptnd
                                br.ReadString((int)br.ReadUInt32());

                                var m = new Matrix4D(
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                    br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                );

                                Logger.LogToFile(Logger.LogLevel.Debug, "{0}", m);

                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                                bool bLoop = true;

                                do
                                {
                                    section = br.ReadString(4);

                                    switch (section)
                                    {
                                        case "hsmp":
                                            br.ReadUInt32();    // 0
                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                            }
                                            break;

                                        case "hsem":
                                            br.ReadUInt32();    // 0
                                            int meshCount = (int)br.ReadUInt32();
                                            for (int mi = 0; mi < meshCount; mi++)
                                            {
                                                br.ReadString((int)br.ReadUInt32());

                                                if (br.ReadString(4) != "hctb") { throw new InvalidDataException("Expected hctb"); }

                                                br.ReadUInt32();    // 2
                                                int batchCount = (int)br.ReadUInt32();
                                                for (int bi = 0; bi < batchCount; bi++)
                                                {
                                                    SCNEMeshPart mesh = new SCNEMeshPart();

                                                    br.ReadUInt32();    // 0
                                                    br.ReadUInt32();    // 0
                                                    br.ReadUInt32();    // 0
                                                    br.ReadSingle();
                                                    br.ReadSingle();
                                                    br.ReadSingle();
                                                    br.ReadSingle();
                                                    br.ReadSingle();
                                                    br.ReadSingle();
                                                    br.ReadSingle();

                                                    if (br.ReadString(4) != "lrtm") { throw new InvalidDataException("Expected lrtm"); }

                                                    br.ReadUInt32();    // 6
                                                    br.ReadUInt32();    // 1
                                                    br.ReadString((int)br.ReadUInt32());
                                                    br.ReadUInt32();    // 0x37
                                                    br.ReadSingle();
                                                    br.ReadSingle();
                                                    br.ReadSingle();
                                                    for (int i = 0; i < 10; i++) { br.ReadUInt32(); } // 0

                                                    if (br.ReadString(4) != "rtxt") { throw new InvalidDataException("Expected rtxt"); }

                                                    br.ReadUInt32();    // 0
                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        br.ReadUInt32();
                                                        br.ReadString(4);   // pamb
                                                        br.ReadString((int)br.ReadUInt32());
                                                    }

                                                    if (br.ReadString(4) != "rtlc") { throw new InvalidDataException("Expected rtlc"); }

                                                    br.ReadUInt32();    // 1
                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                                        br.ReadUInt32();
                                                        br.ReadUInt32();
                                                        br.ReadString(4);   // encs
                                                        br.ReadString((int)br.ReadUInt32());
                                                        br.ReadString((int)br.ReadUInt32());
                                                    }

                                                    if (br.ReadString(4) != "trev") { throw new InvalidDataException("Expected trev"); }

                                                    br.ReadUInt32();    // 4
                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        SCNEVertex v = new SCNEVertex();
                                                        v.Position = unpack(br.ReadUInt64()).ToVector3(); //new Vector3(calculateFraction(br.ReadUInt16()), calculateFraction(br.ReadUInt16()), calculateFraction(br.ReadUInt16())); br.ReadUInt16(); // W component of position
                                                        v.Normal = unpackNormal(br.ReadUInt32()).ToVector3();
                                                        v.UV = unpack(br.ReadUInt32());// new Vector2(calculateFraction(br.ReadUInt16()), calculateFraction(br.ReadUInt16()));
                                                        br.ReadBytes(16);
                                                        //Logger.LogToFile("X: {0:x2} Y: {1:x2} Z: {2:x2} W: {3:x2}", br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16());
                                                        //Logger.LogToFile("N: {0:x2}", br.ReadUInt32());
                                                        //Logger.LogToFile("U: {0:x2} V: {1:x2}", br.ReadUInt16(), br.ReadUInt16());
                                                        //Logger.LogToFile("N: {0:x2}", br.ReadUInt32());
                                                        //Logger.LogToFile("U: {0:x2} V: {1:x2}", br.ReadUInt16(), br.ReadUInt16());
                                                        //Logger.LogToFile("B: {0:x2}", br.ReadUInt32());
                                                        //Logger.LogToFile("B: {0:x2}", br.ReadUInt32());
                                                        mesh.Verts.Add(v);
                                                    }

                                                    if (br.ReadString(4) != "airt") { throw new InvalidDataException("Expected airt"); }

                                                    br.ReadUInt32();    // 0
                                                    count = (int)br.ReadUInt32();
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        mesh.IndexBuffer.Add((int)br.ReadUInt16());
                                                        mesh.IndexBuffer.Add((int)br.ReadUInt16());
                                                        mesh.IndexBuffer.Add((int)br.ReadUInt16());
                                                    }

                                                    if (br.ReadString(4) != "mgde") { throw new InvalidDataException("Expected mgde"); }

                                                    br.ReadUInt32();    // 0
                                                    br.ReadUInt32();    // 0

                                                    scne.meshes.Add(mesh);
                                                }
                                            }
                                            break;

                                        case "ephs":
                                            br.ReadUInt32();    // 1
                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                br.ReadUInt32();    // 1
                                                br.ReadBytes((int)br.ReadUInt32());
                                            }
                                            break;

                                        case "xbhs":
                                            br.ReadUInt32();    // 1
                                            count = (int)br.ReadUInt32();
                                            for (int i = 0; i < count; i++)
                                            {
                                                for (int j = 0; j < 22; j++)
                                                {
                                                    br.ReadSingle();
                                                }
                                            }
                                            break;

                                        case "mina":
                                            br.ReadUInt32();    // 0
                                            br.ReadUInt32();    // 1
                                            break;

                                        case "arfk":
                                            br.ReadUInt32();    // 0
                                            int animFrameCount = (int)br.ReadUInt32();
                                            for (int ki = 0; ki < animFrameCount; ki++)
                                            {
                                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                            }
                                            break;

                                        case "niks":
                                            br.ReadUInt32();    // 0
                                            br.ReadUInt32();    // 1
                                            break;

                                        case "enob":
                                            br.ReadUInt32();    // 0
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

                                                if (br.ReadString(4) != "arfk") { throw new InvalidDataException("Expected arfk"); }

                                                br.ReadUInt32();    // 0
                                                int boneFrameCount = (int)br.ReadUInt32();
                                                for (int ki = 0; ki < boneFrameCount; ki++)
                                                {
                                                    br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                                    br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                                }
                                            }
                                            break;

                                        case "ymmd":
                                            br.ReadUInt32();    // 0
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

                                        default:
                                            // bloody unicode
                                            if (section[0] == 0xfffd &&
                                                section[1] == 0xfffd &&
                                                section[2] == 0xfffd &&
                                                section[3] == 0xfffd)
                                            {
                                                br.ReadUInt32();    // 0xe2c9
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
                            }
                            break;

                        case "ymmd":
                            br.ReadUInt32();    // 0
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
                            br.ReadUInt32();    // 1
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
                            br.ReadUInt32();    // 2
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

                                br.ReadString(4);   // encs
                                br.ReadString((int)br.ReadUInt32());
                            }
                            break;

                        case "lrpa":
                            br.ReadUInt32();    // 0
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
                            br.ReadUInt32();    // 0
                            br.ReadUInt32();    // 0
                            break;

                        case "csia":
                            br.ReadUInt32();    // 1
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
                            br.ReadUInt32();    // 1
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
                            br.ReadUInt32();    // 0
                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
                            }
                            break;

                        case "mlvt":
                            br.ReadUInt32();    // 1
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
                            br.ReadUInt32();    // 0
                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); 
                            }
                            break;

                        case "fpcs":
                            br.ReadUInt32();    // 0
                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadString(4);   // bfrp
                                br.ReadString((int)br.ReadUInt32());
                            }
                            break;

                        case "hpsv":
                            br.ReadUInt32();    // 0
                            count = (int)br.ReadUInt32();
                            for (int i = 0; i < count; i++)
                            {
                                br.ReadString((int)br.ReadUInt32());
                                br.ReadSingle();
                                new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                            }
                            break;

                        case "xobv":
                            br.ReadUInt32();    // 0
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

        public static Single calculateFraction(uint i)
        {
            Single s = 0;
            int bits = 12;


            for (int x = 0; x < bits; x++)
            {
                s += (i & 0x1) * (Single)Math.Pow(2, -(bits - x));
                i >>= 1;
            }

            if ((i & 0x8) >> 3 == 1)
            {
                s = s - 1;
            }

            return s;
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

    public class SCNEMeshPart
    {
        List<SCNEVertex> verts;
        List<int> indexBuffer;

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
            verts = new List<SCNEVertex>();
            indexBuffer = new List<int>();
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
