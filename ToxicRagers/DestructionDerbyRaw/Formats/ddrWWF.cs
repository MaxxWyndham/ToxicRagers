using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicRagers.Helpers;
using ToxicRagers.TDR2000.Formats;
using static unluac.decompile.expression.TableLiteral;

namespace ToxicRagers.DestructionDerbyRaw.Formats
{

    public class TrackSection
    {
        public List<Thing> Things { get; set; } = new List<Thing>();

        public ushort OffsetX { get; set; }

        public short OffsetXMultiplier { get; set; }

        public ushort OffsetY { get; set; }

        public short OffsetYMultiplier { get; set; }

        public byte C { get; set; }
        public byte D { get; set; }

        public byte Rows { get; set; }

        public byte Columns { get; set; }

        public byte G { get; set; }
        public byte H { get; set; }
        public byte I { get; set; }
        public byte J { get; set; }
        public byte K { get; set; }
        public byte L { get; set; }
        public byte M { get; set; }
        public byte N { get; set; }
        public byte O { get; set; }
        public byte P { get; set; }
        public byte Q { get; set; }
        public byte R { get; set; }

        public ushort StartOfData { get; set; }
    }

    public class Thing
    {
        public Point P { get; set; }

        public ushort UVIndex { get; set; }

        public ushort Flags { get; set; }
    }

    public class WWF
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public List<Grid> MapBlocks { get; set; } = new List<Grid>();

        public List<string> Textures { get; set; } = new List<string>();

        public List<TrackSection> Sections { get; set; } = new List<TrackSection>();

        public List<UVEntry> UVTable { get; set; } = new List<UVEntry>();

        public static WWF Load(string path)
        {
            FileInfo fi = new(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            WWF wwf = new();

            List<uint> blockOffsets = new();

            using (BinaryReader br = new(fi.OpenRead()))
            {
                int gridX = 0;
                int gridY = 0;

                br.ReadUInt16();    // 0
                wwf.Width = br.ReadByte();
                wwf.Height = br.ReadByte();

                for (int x = 0; x < wwf.Width; x++)
                    for (int y = 0; y < wwf.Height; y++)
                    {
                        blockOffsets.Add(br.ReadUInt32());
                    }

                Point point;
                uint pointOffset, nextBlockOffset;
                long startOfBlock;

                foreach (uint offset in blockOffsets)
                {
                    Grid grid = new() { X = gridX, Y = gridY };

                    if (offset > 0)
                    {
                        br.ReadBytes(8);    // MAPBLOCK

                        if (br.BaseStream.Position != offset)
                        {
                            // do something sensible
                        }

                        do
                        {
                            startOfBlock = br.BaseStream.Position;

                            Mesh mesh = new();

                            pointOffset = br.ReadUInt16();
                            short other = br.ReadInt16();
                            ushort endOfPoints = br.ReadUInt16();
                            short unknown = br.ReadInt16();
                            mesh.Offset.X = br.ReadInt16();
                            mesh.Offset.Y = br.ReadInt16();
                            mesh.Offset.Z = br.ReadInt16();
                            nextBlockOffset = br.ReadUInt16();

                            System.Diagnostics.Debug.WriteLine($"{pointOffset}\t{other}\t{endOfPoints}\t{unknown}\t{mesh.Offset.X}\t{mesh.Offset.Y}\t{mesh.Offset.Z}\t{nextBlockOffset}");

                            short stop = 0;

                            do
                            {
                                short x, y, z;

                                x = br.ReadInt16();
                                y = br.ReadInt16();
                                z = br.ReadInt16();
                                point = new Point { X = x, Y = y, Z = z };

                                System.Diagnostics.Debug.WriteLine($"{point.X}\t{point.Y}\t{point.Z}");

                                mesh.Points.Add(point);

                                z = br.ReadInt16();
                                x = br.ReadInt16();
                                y = br.ReadInt16();
                                point = new Point { X = x, Y = y, Z = z };

                                System.Diagnostics.Debug.WriteLine($"{point.X}\t{point.Y}\t{point.Z}");

                                mesh.Points.Add(point);

                                x = br.ReadInt16();
                                y = br.ReadInt16();
                                z = br.ReadInt16();
                                point = new Point { X = x, Y = y, Z = z };

                                System.Diagnostics.Debug.WriteLine($"{point.X}\t{point.Y}\t{point.Z}");

                                mesh.Points.Add(point);

                                stop = br.ReadInt16();
                            } while (stop == 0);

                            do
                            {
                                Face face = new Face
                                {
                                    V1 = br.ReadByte(),
                                    V2 = br.ReadByte(),
                                    V3 = br.ReadByte(),
                                    V4 = br.ReadByte()
                                };

                                mesh.Faces.Add(face);
                            } while (br.BaseStream.Position + 4 < endOfPoints + startOfBlock);

                            br.ReadUInt32();    // 0

                            for (int i = 0; i < mesh.Faces.Count; i++)
                            {
                                ushort s = br.ReadUInt16();

                                mesh.Faces[i].UVIndex = (ushort)(s & 0x7fff);
                                mesh.Faces[i].XYFlip = (ushort)((s & 0x8000) >> 15);

                                System.Diagnostics.Debug.WriteLine($"{mesh.Faces[i].V1}\t{mesh.Faces[i].V2}\t{mesh.Faces[i].V3}\t{mesh.Faces[i].V4}\t::\t{mesh.Faces[i].UVIndex}\t{mesh.Faces[i].XYFlip}");
                            }

                            ushort peeked;
                            do
                            {
                                peeked = br.ReadUInt16();
                            } while (peeked == 0);
                            br.BaseStream.Seek(-2, SeekOrigin.Current);

                            grid.Meshes.Add(mesh);
                        } while (nextBlockOffset != 0);
                    }

                    wwf.MapBlocks.Add(grid);

                    gridX++;

                    if (gridX == wwf.Width)
                    {
                        gridX = 0;
                        gridY++;
                    }
                }

                do
                {
                    wwf.Textures.Add(br.ReadString(8));
                } while (wwf.Textures.Last() != "n");

                wwf.Textures.RemoveAt(wwf.Textures.Count - 1);

                wwf.Textures.Reverse();

                br.ReadBytes(8);    // TRACKDAT

                startOfBlock = br.BaseStream.Position;

                do
                {
                    var track = new TrackSection
                    {
                        OffsetX = br.ReadUInt16(),
                        OffsetXMultiplier = br.ReadInt16(),
                        OffsetY = br.ReadUInt16(),
                        OffsetYMultiplier = br.ReadInt16(),
                        C = br.ReadByte(),
                        D = br.ReadByte(),
                        Rows = br.ReadByte(),
                        Columns = br.ReadByte(),
                        G = br.ReadByte(),
                        H = br.ReadByte(),
                        I = br.ReadByte(),
                        J = br.ReadByte(),
                        K = br.ReadByte(),
                        L = br.ReadByte(),
                        M = br.ReadByte(),
                        N = br.ReadByte(),
                        O = br.ReadByte(),
                        P = br.ReadByte(),
                        Q = br.ReadByte(),
                        R = br.ReadByte(),
                        StartOfData = br.ReadUInt16()
                    };

                    br.ReadUInt16();    // 0

                    wwf.Sections.Add(track);
                } while (br.BaseStream.Position + 28 <= startOfBlock + wwf.Sections[0].StartOfData);

                foreach (TrackSection section in wwf.Sections)
                {
                    if (br.BaseStream.Position - startOfBlock != section.StartOfData)
                    {
                        // we have an issue
                    }

                    System.Diagnostics.Debug.WriteLine($"{section.OffsetX}\t{section.OffsetXMultiplier}\t{section.OffsetY}\t{section.OffsetYMultiplier}\t|\t{section.C}\t{section.D}\t|\t{section.Rows}\t{section.Columns}\t|\t{section.G}\t{section.H}\t{section.I}\t|\t{section.J}\t{section.K}\t{section.L}\t|\t{section.M}\t{section.N}\t{section.O}\t|\t{section.P}\t{section.Q}\t{section.R}");

                    for (int r = 0; r < section.Rows; r++)
                    {
                        for (int c = 0; c < section.Columns; c++)
                        {
                            var thing = new Thing()
                            {
                                P = new Point
                                {
                                    X = br.ReadInt16(),
                                    Y = br.ReadInt16(),
                                    Z = br.ReadInt16()
                                }
                            };

                            ushort f = br.ReadUInt16();

                            thing.UVIndex = (ushort)(f & 0xFFF);
                            thing.Flags = (ushort)(f >> 12);

                            System.Diagnostics.Debug.WriteLine($"{thing.P.X}\t{thing.P.Y}\t{thing.P.Z}\t::\t{thing.UVIndex}\t{thing.Flags}");

                            section.Things.Add(thing);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("----");
                }

                br.ReadBytes(8);    // UV_TABLE

                do
                {
                    UVEntry entry = new()
                    {
                        UV1 = new UV { U = br.ReadByte(), V = br.ReadByte() },
                        MaterialIndex = br.ReadUInt16(),
                        UV2 = new UV { U = br.ReadByte(), V = br.ReadByte() },
                        UV3 = new UV { U = br.ReadByte(), V = br.ReadByte() },
                        UV4 = new UV { U = br.ReadByte(), V = br.ReadByte() },
                        Flags = br.ReadUInt16()
                    };

                    var calc = entry.Flags == 0 
                        ? missing(entry.UV1, entry.UV2, entry.UV4)
                        : missing(entry.UV2, entry.UV1, entry.UV4);

                    if (entry.MaterialIndex > 0) { System.Diagnostics.Debug.WriteLine($"{wwf.Textures[entry.MaterialIndex - 1]}\t{entry.UV1.U}\t{entry.UV1.V}\t{entry.UV2.U}\t{entry.UV2.V}\t{entry.UV3.U}\t{entry.UV3.V}\t{entry.UV4.U}\t{entry.UV4.V}\t:\t{entry.Flags}"); }

                    //if ((entry.UV1.U == 0 && entry.UV1.V == 0) || (entry.UV2.U == 0 && entry.UV2.V == 0) || (entry.UV4.U == 0 && entry.UV4.V == 0)) { entry.UV3 = calc; }
                    //entry.UV3.U = entry.UV4.U;
                    //entry.UV3.V = entry.UV2.V;
                    entry.UV3 = calc;

                    wwf.UVTable.Add(entry);

                    if (entry.MaterialIndex == 0 &&
                        entry.UV1.U == 0 && entry.UV1.V == 0 &&
                        entry.UV2.U == 0 && entry.UV2.V == 0 &&
                        entry.UV3.U == 0 && entry.UV3.V == 0 &&
                        entry.UV4.U == 0 && entry.UV4.V == 0) { break; }
                } while (br.BaseStream.Position + 12 <= br.BaseStream.Length);

                br.ReadUInt32();    // 0

                if (br.BaseStream.Position != br.BaseStream.Length)
                {
                    br.ReadBytes(8);    // GRCINDEX
                }

                int totalPointCount = wwf.MapBlocks.SelectMany(g => g.Meshes).Select(m => m.Points.Count).Sum();
                int totalFaceCount = wwf.MapBlocks.SelectMany(g => g.Meshes).Select(m => m.Faces.Count).Sum();

                System.Diagnostics.Debug.WriteLine($"{totalPointCount} :: {totalFaceCount}");

                return wwf;

                UV missing(UV UV1, UV UV2, UV UV3)
                {
                    if (Math.Abs(UV1.U - UV2.U) > Math.Abs(UV1.V - UV2.V))
                    {
                        return new UV { U = UV2.U, V = UV3.V };
                    }
                    else
                    {
                        return new UV { U = UV3.U, V = UV2.V };
                    }
                }
            }
        }

        public class Grid
        {
            public int X { get; set; }

            public int Y { get; set; }

            public List<Mesh> Meshes { get; set; } = new List<Mesh>();
        }

        public class UVEntry
        {
            public UV UV1 { get; set; }

            public UV UV2 { get; set; }

            public UV UV3 { get; set; }

            public UV UV4 { get; set; }

            public ushort MaterialIndex { get; set; }

            public byte A { get; set; }
            public byte B { get; set; }
            public byte C { get; set; }

            public ushort Flags { get; set; }
        }
    }
}