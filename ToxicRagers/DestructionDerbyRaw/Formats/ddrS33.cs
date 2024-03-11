using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.Helpers;
using ToxicRagers.TDR2000.Formats;

namespace ToxicRagers.DestructionDerbyRaw.Formats
{
    public class S33
    {
        public List<Mesh> Meshes { get; set; } = new List<Mesh>();
        public List<string> Textures { get; set; } = new List<string>();

        public static S33 Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            S33 s33 = new();

            using (BinaryReader br = new(fi.OpenRead()))
            {
                uint lod1Offset, lod2Offset, textureOffset;
                uint somethingCount;

                lod1Offset = br.ReadUInt16();
                lod2Offset = br.ReadUInt16();
                textureOffset = br.ReadUInt16();
                somethingCount = br.ReadUInt16();

                br.ReadBytes(60);

                for (int i = 0; i <= somethingCount; i++) { br.ReadBytes(24); }

                Point point;
                uint pointOffset, faceCount, nextBlockOffset;
                uint totalFaceCount = 0;
                long startOfBlock;

                do
                {
                    startOfBlock = br.BaseStream.Position;

                    // header length : 20 (0x14)
                    br.ReadBytes(4);
                    pointOffset = br.ReadUInt16();
                    faceCount = br.ReadUInt16();
                    totalFaceCount += faceCount;
                    textureOffset = br.ReadUInt16();
                    br.ReadBytes(8);
                    nextBlockOffset = br.ReadUInt16();

                    Mesh mesh = new();
                    short stop;

                    do
                    {
                        short x, y, z;

                        x = br.ReadInt16();
                        y = br.ReadInt16();
                        z = br.ReadInt16();
                        point = new Point { X = x, Y = y, Z = z };
                        mesh.Points.Add(point);

                        z = br.ReadInt16();
                        x = br.ReadInt16();
                        y = br.ReadInt16();
                        point = new Point { X = x, Y = y, Z = z };
                        mesh.Points.Add(point);

                        x = br.ReadInt16();
                        y = br.ReadInt16();
                        z = br.ReadInt16();
                        point = new Point { X = x, Y = y, Z = z };
                        mesh.Points.Add(point);

                        stop = br.ReadInt16();
                    } while (stop == 0);

                    br.BaseStream.Seek(startOfBlock + pointOffset, SeekOrigin.Begin);

                    //if (br.BaseStream.Position != pointOffset)
                    //{
                    //    // Something has gone very wrong!
                    //    return null;
                    //}

                    for (int i = 0; i < faceCount; i++)
                    {
                        mesh.Faces.Add(new Face
                        {
                            V1 = br.ReadByte(),
                            V2 = br.ReadByte(),
                            V3 = br.ReadByte(),
                            V4 = br.ReadByte()
                        });
                    }

                    br.ReadUInt32();    // 0

                    s33.Meshes.Add(mesh);
                } while (nextBlockOffset != 0);

                //if (br.BaseStream.Position != textureOffset)
                //{
                //    // Something has gone very wrong!
                //    return null;
                //}

                br.ReadBytes(8);    // TEXTURES

                foreach (var mesh in s33.Meshes)
                foreach (var face in mesh.Faces)
                {
                    face.MaterialIndex = (ushort)(br.ReadUInt16() - 1);

                    face.UV1 = new UV { U = br.ReadByte(), V = br.ReadByte() };
                    face.UV2 = new UV { U = br.ReadByte(), V = br.ReadByte() };
                    face.UV3 = new UV { U = br.ReadByte(), V = br.ReadByte() };
                    face.UV4 = new UV { U = br.ReadByte(), V = br.ReadByte() };
                }

                br.ReadUInt16();    // 0

                while (br.PeekChar() != 0)
                {
                    s33.Textures.Add(br.ReadString(8));
                }

                br.ReadUInt32();    // 0x00002A2A

                br.ReadBytes(8);    // SHADEMAP

                for (int i = 0; i < totalFaceCount; i++)
                {
                    br.ReadBytes(4);
                }

                br.ReadBytes(8);    // BASEMAPP

                for (int i = 0; i < totalFaceCount; i++)
                {
                    br.ReadBytes(4);
                }

                br.ReadBytes(8);    // ENVMAPPP

                for (int i = 0; i < totalFaceCount; i++)
                {
                    br.ReadBytes(8);
                }
            }

            return s33;
        }
    }
}
