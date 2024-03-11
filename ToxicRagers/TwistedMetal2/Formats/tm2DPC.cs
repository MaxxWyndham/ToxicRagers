using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;
using ToxicRagers.PSX.Formats;

namespace ToxicRagers.TwistedMetal2.Formats
{
    public class DPC
    {
        public List<uint> Offsets { get; set; } = new List<uint>();
        public List<DPCNode> RootNodes { get; set; } = new List<DPCNode>();

        public static DPC Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            DPC dpc = new DPC();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                if (br.ReadByte() != 0x44 || // D
                    br.ReadByte() != 0x43 || // C
                    br.ReadByte() != 0x50 || // P
                    br.ReadByte() != 0x4d || // M
                    br.ReadByte() != 0x43 || // C
                    br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0x00 ||
                    br.ReadByte() != 0xf1 ||
                    br.ReadByte() != 0xf1 ||
                    br.ReadByte() != 0x58 ||
                    br.ReadByte() != 0x34)
                {
                    // this can't all be magic?
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid DPC file", path);
                    return null;
                }

                uint realOffset = br.ReadUInt32();
                br.ReadUInt32(); // ?? always 0x3
                br.ReadUInt32(); // offset to start of data
                br.ReadBytes(8); // padding

                uint objectCount = br.ReadUInt32();

                for (int i = 0; i < objectCount; i++)
                {
                    dpc.Offsets.Add(br.ReadUInt32() - realOffset);
                }

                foreach (uint offset in dpc.Offsets)
                {
                    dpc.RootNodes.Add(CreateNode(realOffset, offset, br));
                }
            }

            return dpc;
        }

        public static DPCNode CreateNode(uint baseOffset, uint offset, BinaryReader br)
        {
            long originalPosition = br.BaseStream.Position;
            br.BaseStream.Seek(offset, SeekOrigin.Begin);

            ushort chunkType = br.ReadUInt16();
            DPCNode node;

            switch (chunkType)
            {
                case 0x0509:
                    node = new DPCSubEntity(br, baseOffset);
                    break;

                case 0x0701:
                    node = new DPCBoundingSphere(br, baseOffset);
                    break;

                case 0x0602:
                case 0x0802:
                    node = new DPCMeshContainer(br, baseOffset) { ChunkType = chunkType };
                    break;

                case 0x0903:
                    node = new DPCAnimationContainer(br, baseOffset);
                    break;

                case 0x0b04:
                    node = new DPCTranslation(br, baseOffset);
                    break;

                case 0x0b05:
                    node = new DPCMatrix(br, baseOffset);
                    break;

                case 0xff00:
                    node = new DPCMesh(br, baseOffset);
                    break;

                default:
                    Console.WriteLine($"Unknown chunk type: {chunkType:x2}");
                    return null;
            }

            br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);

            return node;
        }
    }

    public class DPCNode
    {
        public ushort ChunkType { get; set; }
        public List<uint> Offsets { get; set; } = new List<uint>();
        public List<DPCNode> Children { get; set; } = new List<DPCNode>();

        public void Process(BinaryReader br, uint baseOffset)
        {
            foreach (uint offset in Offsets)
            {
                Children.Add(DPC.CreateNode(baseOffset, offset, br));
            }
        }
    }

    public class DPCBoundingSphere : DPCNode
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Radius { get; set; }

        public DPCBoundingSphere(BinaryReader br, uint baseOffset)
        {
            ChunkType = 0x0701;

            br.ReadUInt16(); // ??
            X = br.ReadInt32();
            Y = br.ReadInt32();
            Z = br.ReadInt32();
            Radius = br.ReadInt32();

            byte childCount = br.ReadByte();

            br.ReadBytes(3); // ??

            for (int i = 0; i < childCount; i++)
            {
                Offsets.Add(br.ReadUInt32() - baseOffset);
            }

            Process(br, baseOffset);
        }
    }

    public class DPCSubEntity : DPCNode
    {
        public enum EntityType
        {
            Default = 0x0
        }

        public EntityType Entity { get; set; }

        public DPCSubEntity(BinaryReader br, uint baseOffset)
        {
            ChunkType = 0x0509;

            Entity = (EntityType)br.ReadUInt16();
            br.ReadUInt16(); // ??

            uint childCount = br.ReadUInt32();

            br.ReadUInt32(); // ??
            br.ReadUInt16(); // ??

            for (int i = 0; i < childCount; i++)
            {
                Offsets.Add(br.ReadUInt32() - baseOffset);
            }

            Process(br, baseOffset);
        }
    }

    public class DPCMeshContainer : DPCNode
    {
        public DPCMeshContainer(BinaryReader br, uint baseOffset)
        {
            br.ReadUInt16(); // ??
            br.ReadInt32(); // ??
            br.ReadInt32(); // ??
            br.ReadInt32(); // ??

            uint childCount = br.ReadUInt32();

            for (int i = 0; i < childCount; i++)
            {
                Offsets.Add(br.ReadUInt32() - baseOffset);
            }

            foreach (uint offset in Offsets)
            {
                br.BaseStream.Seek(offset, SeekOrigin.Begin);

                Children.Add(new DPCMeshLink(br, baseOffset));
            }
        }
    }

    public class DPCTranslation : DPCNode
    {
        public enum EntityType
        {
            Default = 0x0
        }

        public EntityType Entity { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public DPCTranslation(BinaryReader br, uint baseOffset)
        {
            ChunkType = 0x0b04;

            Entity = (EntityType)br.ReadUInt16();
            X = br.ReadInt32();
            Y = br.ReadInt32();
            Z = br.ReadInt32();
            br.ReadBytes(2); // ??
            byte childCount = br.ReadByte();
            br.ReadByte(); // ??

            for (int i = 0; i < childCount; i++)
            {
                Offsets.Add(br.ReadUInt32() - baseOffset);
            }

            Process(br, baseOffset);
        }
    }

    public class DPCMatrix : DPCNode
    {
        public Matrix3D Matrix { get; set; } = Matrix3D.Identity;

        public DPCMatrix(BinaryReader br, uint baseOffset)
        {
            ChunkType = 0x0b05;

            br.ReadUInt16(); // ??
            Matrix.M11 = br.ReadInt32() / 4096.0f;
            Matrix.M12 = br.ReadInt32() / 4096.0f;
            Matrix.M13 = br.ReadInt32() / 4096.0f;
            Matrix.M21 = br.ReadInt32() / 4096.0f;
            Matrix.M22 = br.ReadInt32() / 4096.0f;
            Matrix.M23 = br.ReadInt32() / 4096.0f;
            Matrix.M31 = br.ReadInt32() / 4096.0f;
            Matrix.M32 = br.ReadInt32() / 4096.0f;
            Matrix.M33 = br.ReadInt32() / 4096.0f;
            Matrix.M41 = br.ReadInt32();
            Matrix.M42 = br.ReadInt32();
            Matrix.M43 = br.ReadInt32();
            br.ReadBytes(2); // ??
            byte childCount = br.ReadByte();
            br.ReadByte(); // ??

            for (int i = 0; i < childCount; i++)
            {
                Offsets.Add(br.ReadUInt32() - baseOffset);
            }

            Process(br, baseOffset);
        }
    }

    public class DPCAnimationContainer : DPCNode
    {
        public DPCAnimationContainer(BinaryReader br, uint baseOffset)
        {
            ChunkType = 0x0903;

            br.ReadBytes(9); // ??
            byte childCount = br.ReadByte();
            br.ReadBytes(20); // ??

            for (int i = 0; i < childCount; i++)
            {
                Offsets.Add(br.ReadUInt32() - baseOffset);
            }

            Process(br, baseOffset);
        }
    }

    public class DPCMesh : DPCNode
    {
        public ushort FaceCount { get; set; }
        public uint VertexOffset { get; set; }
        public uint FaceOffset { get; set; }
        public List<DPCMeshFace> Faces { get; set; } = new List<DPCMeshFace>();

        public DPCMesh(BinaryReader br, uint baseOffset)
        {
            ChunkType = 0xff00;

            br.ReadUInt16(); // ??
            VertexOffset = br.ReadUInt32() - baseOffset;
            br.ReadUInt32(); // unknown offset
            FaceOffset = br.ReadUInt32() - baseOffset;
            FaceCount = br.ReadUInt16();
            br.ReadBytes(26);

            br.BaseStream.Seek(FaceOffset, SeekOrigin.Begin);

            for (int i = 0; i < FaceCount; i++)
            {
                // 04 01 0D 09 => has diffuse colours, no per vertex colours, not colour only, unknown shorts = 3
                // 04 01 0E 0C => has diffuse colours, no per vertex colours, not colour only, unknown shorts = 5
                byte vertCount = br.ReadByte();
                byte u1 = br.ReadByte();
                byte u2 = br.ReadByte();
                byte u3 = br.ReadByte();

                DPCMeshFace face = new DPCMeshFace();

                for (int j = 0; j < 4; j++)
                {
                    ushort index = br.ReadUInt16();

                    if (j < vertCount) { face.Verts.Add(index); }
                }

                face.Normal.X = br.ReadInt16() / 4096.0f;
                face.Normal.Y = br.ReadInt16() / 4096.0f;
                face.Normal.Z = br.ReadInt16() / 4096.0f;

                // 0x4 == non-billboard?
                ushort renderFlags = br.ReadUInt16();
                short polySort0 = br.ReadInt16();
                short polySort1 = br.ReadInt16();

                byte b = br.ReadByte();
                byte g = br.ReadByte();
                byte r = br.ReadByte();
                byte a = br.ReadByte();

                for (int j = 0; j < face.Verts.Count; j++)
                {
                    face.UVs.Add(new Vector2(br.ReadInt16(), br.ReadInt16()));
                }

                face.MaterialID = br.ReadByte();
                byte u4 = br.ReadByte();

                for (int j = 0; j < 3; j++) { br.ReadUInt16(); }

                Faces.Add(face);
            }
        }
    }

    public class DPCMeshFace
    {
        public List<ushort> Verts { get; set; } = new List<ushort>();
        public Vector3 Normal { get; set; } = Vector3.Zero;
        public List<Vector2> UVs { get; set; } = new List<Vector2>();
        public byte MaterialID { get; set; }
    }

    public class DPCMeshLink : DPCNode
    {
        public uint FarDrawDistance { get; set; }
        public uint NearDrawDistance { get; set; }

        public DPCMeshLink(BinaryReader br, uint baseOffset)
        {
            FarDrawDistance = br.ReadUInt32();
            NearDrawDistance = br.ReadUInt32();

            Offsets.Add(br.ReadUInt32() - baseOffset);

            Process(br, baseOffset);
        }
    }
}
