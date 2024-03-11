using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.Core.Formats
{
    public class FLI : ITexture
    {
        public enum ChunkType
        {
            Colour256 = 4,
            SS2 = 7,
            Colour = 11,
            LC = 12,
            Black = 13,
            Brun = 15,
            Copy = 16,
            PStamp = 18
        }

        public List<FLIFrame> Frames { get; set; } = new List<FLIFrame>();

        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public ushort BitsPerPixel { get; set; } = 8;

        public int Speed { get; set; }

        public string Name { get; set; }

        public string Extension { get; } = "fli";

        public List<MipMap> MipMaps { get; set; } = new List<MipMap>();

        public D3DFormat Format { get; }

        public static FLI Load(string path)
        {
            FLI fli = new FLI();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                br.ReadInt32();     // file length
                if (br.ReadUInt16() != 0xaf12) { return null; }
                br.ReadUInt16();    // frame count
                fli.Width = br.ReadUInt16();
                fli.Height = br.ReadUInt16();
                fli.BitsPerPixel = br.ReadUInt16();
                br.ReadUInt16();    // flags
                fli.Speed = (int)br.ReadUInt32();
                br.ReadUInt16();    // reserved
                br.ReadUInt32();    // created
                br.ReadString(4);   // creator
                br.ReadUInt32();    // updated
                br.ReadString(4);   // updater
                br.ReadUInt16();    // aspect-x
                br.ReadUInt16();    // aspect-y
                br.ReadBytes(38);   // reserved
                br.ReadUInt32();    // offset to first chunk
                br.ReadUInt32();    // offset to second chunk
                br.ReadBytes(40);   // reserved

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    FLIFrame frame = new FLIFrame();

                    br.ReadUInt32();    // chunk size
                    br.ReadUInt16();    // chunk type
                    int chunkCount = br.ReadUInt16();    // number of child chunks
                    br.ReadBytes(8);    // reserved

                    for (int i = 0; i < chunkCount; i++)
                    {
                        IFLIFrameChunk chunk = null;
                        int chunkLength = (int)br.ReadUInt32();
                        ChunkType type = (ChunkType)br.ReadUInt16();
                        byte[] data = br.ReadBytes(chunkLength - 6);

                        switch (type)
                        {
                            case ChunkType.Colour256:
                                chunk = new FLIFrameChunkColour256(data);
                                break;

                            case ChunkType.Brun:
                                chunk = new FLIFrameChunkBRun(data, fli.Width, fli.Height);
                                break;

                            default:
                                throw new NotImplementedException();
                        }

                        frame.Chunks.Add(chunk);
                    }

                    fli.Frames.Add(frame);
                }
            }

            return fli;
        }

        public bool Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(0);
                bw.Write((ushort)0xaf12);
                bw.Write((short)Frames.Count);
                bw.Write(Width);
                bw.Write(Height);
                bw.Write(BitsPerPixel);
                bw.Write((short)0);     // flags
                bw.Write(Speed);
                bw.Write((short)0);     // reserved
                bw.Write(0);            // created
                bw.WriteString("ToRa");
                bw.Write(0);            // updated
                bw.Write(0);            // updater
                bw.Write((short)0);     // aspect-x
                bw.Write((short)0);     // aspect-y
                bw.WriteBytes(38);      // reserved
                bw.Write(0);            // offset #1
                bw.Write(0);            // offset #2
                bw.WriteBytes(40);      // reserved

                foreach (FLIFrame frame in Frames)
                {
                    bw.Write(frame.Size);
                    bw.Write((ushort)0xf1fa);
                    bw.Write((short)frame.Chunks.Count);
                    bw.WriteBytes(8);

                    foreach (IFLIFrameChunk chunk in frame.Chunks)
                    {
                        bw.Write(chunk.Size);
                        bw.Write((short)chunk.Type);
                        bw.Write(chunk.Data);
                    }
                }
            }

            return true;
        }
    }

    public class FLIFrame
    {
        public List<IFLIFrameChunk> Chunks { get; set; } = new List<IFLIFrameChunk>();

        public int Size { get; set; }
    }

    public interface IFLIFrameChunk
    {
        int Size { get; }

        FLI.ChunkType Type { get; }

        byte[] Data { get; }
    }

    public class FLIFrameChunkColour256 : IFLIFrameChunk
    {
        public int Size => throw new NotImplementedException();

        public FLI.ChunkType Type => FLI.ChunkType.Colour256;

        public byte[] Data => throw new NotImplementedException();

        public List<Colour> Palette = new List<Colour>();

        public FLIFrameChunkColour256(byte[] data)
        {
            for (int i = 0; i < 256; i++) { Palette.Add(Colour.Black); }

            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                int packetCount = br.ReadUInt16();
                int offset = 0;

                for (int i = 0; i < packetCount; i++)
                {
                    byte count = br.ReadByte();
                    byte skip = br.ReadByte();

                    if (count == 0) { count = 255; }

                    offset += skip;

                    for (int j = 0; j < count; j++)
                    {
                        Palette[offset++] = Colour.FromRgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
                    }
                }
            }
        }
    }

    public class FLIFrameChunkBRun : IFLIFrameChunk
    {
        public int Size => throw new NotImplementedException();

        public FLI.ChunkType Type => FLI.ChunkType.Brun;

        public byte[] Data => throw new NotImplementedException();

        public FLIFrameChunkBRun(byte[] data, ushort width, ushort height)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                for (int i = 0; i < height; i++)
                {
                    br.ReadByte(); // obsolete packet count
                    int toRead = width;

                    while (toRead > 0)
                    {
                        sbyte typesize = br.ReadSByte();

                        if (typesize < 0)
                        {
                            typesize = Math.Abs(typesize);

                            br.ReadBytes(typesize);
                        }
                        else
                        {
                            br.ReadByte(); // pixel to repeat typesize times
                        }

                        toRead -= typesize;
                    }
                }
            }
        }
    }
}
