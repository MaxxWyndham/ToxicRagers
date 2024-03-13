//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using ToxicRagers.Brender.Formats;
//using ToxicRagers.Carmageddon2.Helpers;
//using ToxicRagers.Helpers;

//namespace ToxicRagers.CarmageddonPSX
//{
//    public class Helpers
//    {
//        //Single scale = 0.00165f; // C2 scale

//        public struct CpsxTex
//        {
//            public string Name;
//            public int Width;
//            public int Height;
//        }

//        public static void ProcessOBJ(string PathIn, string PathOut, float scale)
//        {
//            string pathIn = PathIn.Substring(0, PathIn.LastIndexOf("\\") + 1);
//            string fileIn = PathIn.Replace(pathIn, "");

//            BinaryReader br = new BinaryReader(new FileStream(pathIn + fileIn, FileMode.Open));
//            bool bDebug = false;

//            ACT a1 = new ACT();
//            ACT a2 = new ACT();
//            ACT a7 = new ACT();
//            ACT a8 = new ACT();
//            ACT a9 = new ACT();
//            ACT a13 = new ACT();

//            a1.AddRootNode("Type1");
//            a2.AddRootNode("Type2");
//            a7.AddRootNode("Type7");
//            a8.AddRootNode("Type8");
//            a9.AddRootNode("Type9");
//            a13.AddRootNode("Type13");

//            br.ReadInt32(); // always 0
//            int objCount = br.ReadInt32();

//            for (int i = 0; i < objCount; i++)
//            {
//                int oType = br.ReadInt32();

//                //switch (oType)
//                //{
//                //    case 1:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 2:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 7:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 8:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 9:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 10:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 12:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 13:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    case 14:
//                //        br.ReadBytes(108);
//                //        continue;
//                //        break;

//                //    default:
//                //        Console.WriteLine("Unknown type " + oType);
//                //        return;
//                //        break;
//                //}

//                if (bDebug)
//                {
//                    Console.Write(fileIn + "\t");
//                    Console.Write(oType + "\t");
//                    Console.Write(i + "\t");
//                }

//                if (br.ReadInt32() != 0) { Console.WriteLine("Byte A not zero!"); }
//                if (br.ReadInt32() != 0) { Console.WriteLine("Byte B not zero!"); }

//                Vector3 pos = new Vector3(-br.ReadInt32(), br.ReadInt32(), br.ReadInt32()) * scale;

//                if (bDebug)
//                {
//                    Console.Write(br.ReadInt32() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                }
//                else
//                {
//                    br.ReadBytes(6);
//                }

//                int rotateY = br.ReadInt16();
//                if (bDebug)
//                {
//                    Console.Write(rotateY + "\t");
//                    Console.Write(br.ReadInt32() + "\t");
//                }
//                else
//                {
//                    br.ReadBytes(4);
//                }

//                if (br.ReadInt32() != 0) { Console.WriteLine("Byte D not zero!"); }
//                if (br.ReadInt32() != 0) { Console.WriteLine("Byte E not zero!"); }
//                if (br.ReadInt32() != 0) { Console.WriteLine("Byte F not zero!"); }
//                if (br.ReadInt32() != 0) { Console.WriteLine("Byte G not zero!"); }
//                if (br.ReadInt32() != 0) { Console.WriteLine("Byte H not zero!"); }
//                int index = br.ReadInt16();

//                if (bDebug)
//                {
//                    Console.Write(index + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt32() + "\t");
//                    Console.Write(br.ReadInt32() + "\t");
//                    Console.Write(br.ReadInt32() + "\t");
//                    Console.Write(br.ReadInt32() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt16() + "\t");
//                    Console.Write(br.ReadInt32() + "\t");
//                    Console.Write(br.ReadInt32() + "\t");
//                    Console.WriteLine(br.ReadInt32());
//                }
//                else
//                {
//                    br.ReadBytes(54);
//                }

//                switch (oType)
//                {
//                    case 1:
//                        a1.AddActor("a1" + i, "Sphere_48", new Matrix3D(pos), false);
//                        break;

//                    case 2:
//                        Matrix3D m = Matrix3D.Identity;// Matrix3D.CreateRotationY(-rotateY / 11.4f);
//                        m.Position = pos;
//                        a2.AddActor("a2" + i, "cell" + index.ToString().PadLeft(2, "0"[0]) + "_00", m, false);
//                        break;

//                    case 7:
//                        a7.AddActor("a7" + i, "Sphere_48", new Matrix3D(pos), false);
//                        break;

//                    case 8:
//                        a8.AddActor("a8" + i, "Sphere_48", new Matrix3D(pos), false);
//                        break;

//                    case 9:
//                        a9.AddActor("a9" + i, "Sphere_48", new Matrix3D(pos), false);
//                        break;

//                    case 13:
//                        a13.AddActor("a13" + i, "Sphere_48", new Matrix3D(pos), false);
//                        break;
//                }
//            }

//            a1.Save(PathOut + "\\1.act");
//            a2.Save(PathOut + "\\2.act");
//            a7.Save(PathOut + "\\7.act");
//            a8.Save(PathOut + "\\8.act");
//            a9.Save(PathOut + "\\9.act");
//            a13.Save(PathOut + "\\13.act");

//            br.Close();
//        }

//        public static void ProcessHGX(string PathIn, string PathOut)
//        {
//            string pathIn = PathIn.Substring(0, PathIn.LastIndexOf("\\") + 1);
//            string fileIn = PathIn.Replace(pathIn, "");

//            if (!Directory.Exists(PathOut)) { Directory.CreateDirectory(PathOut); }

//            BinaryReader br = new BinaryReader(new FileStream(pathIn + fileIn, FileMode.Open));

//            int width = br.ReadInt16();
//            int height = br.ReadInt16();
//            int x = 0, y = 0, j = 0;
//            Color[] colours = new Color[256];

//            for (int i = 0; i < colours.Length; i++) { colours[i] = pixelToRGB(br.ReadUInt16()); }

//            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppArgb1555);

//            for (int i = 0; i < (width * height); i++)
//            {
//                int b = br.ReadByte();

//                bmp.SetPixel(x, y, colours[b]);
//                //Console.WriteLine(x + ", " + y + " :: " + colours[b].ToString());

//                j++;

//                if (j % width == 0) { x = 0; y++; } else { x++; }
//            }

//            bmp.Save(PathOut + fileIn.Replace("HGX", "tif"), System.Drawing.Imaging.ImageFormat.Tiff);

//            br.Close();
//        }

//        public static void ProcessTEX(string PathIn, string PathOut, out CpsxTex[] psxTex)
//        {
//            string pathIn = PathIn.Substring(0, PathIn.LastIndexOf("\\") + 1);
//            string fileIn = PathIn.Replace(pathIn, "");

//            bool bNonCars = fileIn.Contains(".MOT");

//            if (!Directory.Exists(PathOut)) { Directory.CreateDirectory(PathOut); }
//            if (!Directory.Exists(PathOut + "\\TIFFRGB")) { Directory.CreateDirectory(PathOut + "\\TIFFRGB"); }

//            MAT m = new MAT();
//            using (BinaryReader br = new BinaryReader(new FileStream(pathIn + fileIn, FileMode.Open)))
//            {

//                int iTexCount = br.ReadInt32();
//                Color[] colours = new Color[16];

//                psxTex = new CpsxTex[iTexCount];

//                int k = 0;

//                while (br.BaseStream.Position < br.BaseStream.Length)
//                {
//                    if (br.ReadInt32() != 0) { Console.WriteLine("int not 0"); }

//                    int width = br.ReadInt16();
//                    int height = br.ReadInt16();

//                    // Palette
//                    for (int i = 0; i < 16; i++) { colours[i] = pixelToRGB(br.ReadUInt16()); }

//                    Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);

//                    // Pixels
//                    int x = 0;
//                    int y = height - 1;
//                    int j = 0;

//                    psxTex[k].Width = width;
//                    psxTex[k].Height = height;

//                    for (int i = 0; i < ((width * height) / 2); i++)
//                    {
//                        int b = br.ReadByte();

//                        int idxA = (b & 0xF0) >> 4;
//                        int idxB = (b & 0xF);

//                        bmp.SetPixel(x, y, colours[idxB]);
//                        bmp.SetPixel(x + 1, y, colours[idxA]);

//                        j += 2;

//                        if (j % width == 0) { x = 0; y--; } else { x += 2; }
//                    }

//                    psxTex[k].Name = "psx" + fileIn.Replace(".TEX", "").Replace(".MOT", "").Replace(".MAT", "") + (bNonCars ? "nc" : "") + k.ToString().PadLeft(2, "0"[0]);
//                    m.Materials.Add(new MATMaterial(psxTex[k].Name, psxTex[k].Name));
//                    bmp.Save(PathOut + "\\TIFFRGB\\" + psxTex[k].Name + ".tif", System.Drawing.Imaging.ImageFormat.Tiff);
//                    k++;
//                }
//            }

//            m.Save(PathOut + (bNonCars ? "nc" : "") + fileIn.Replace(".TEX", ".mat").Replace(".MOT", ".mat"));
//        }

//        public static void ProcessMOD(string PathIn, string PathOut, CpsxTex[] psxTex, float scaleFactor, bool bDebug = false, bool bSplit = true)
//        {
//            string pathIn = PathIn.Substring(0, PathIn.LastIndexOf("\\"));
//            string fileIn = PathIn.Replace(pathIn, "");

//            if (!Directory.Exists(PathOut)) { Directory.CreateDirectory(PathOut); }

//            BinaryReader br = new BinaryReader(new FileStream(pathIn + fileIn, FileMode.Open));

//            int iMagic = br.ReadInt32();
//            if (bDebug) { Console.WriteLine("Magic number : " + iMagic); }

//            if (iMagic == 21102604)
//            {
//                // Process map
//                int iBlockCount = br.ReadInt32();
//                int gX = br.ReadInt32();
//                int gZ = br.ReadInt32();
//                int h4 = br.ReadInt32();

//                if (bDebug) { Console.WriteLine("Block Count : " + iBlockCount + " :: Width : " + gX + " :: Length : " + gZ + " :: H4 : " + h4); }

//                DAT dat = new DAT();

//                if (bDebug) { Console.WriteLine("Reading 256 4 byte chunks"); }
//                // Possibly some sort of vert colours?
//                for (int i = 0; i < 256; i++)
//                {
//                    //Console.WriteLine(i + ")\t" + br.ReadInt32());
//                    br.ReadInt32();
//                }

//                int iFirstBlockSize = br.ReadInt32();
//                if (bDebug) { Console.WriteLine("Reading header block"); }
//                int[] blockOffsets = new int[iBlockCount + 1];

//                for (int i = 0; i < blockOffsets.Length; i++)
//                {
//                    blockOffsets[i] = br.ReadInt32();
//                    if (bDebug) { Console.WriteLine("Block start at : " + blockOffsets[i]); }
//                }

//                Console.WriteLine("Reading " + h4 + " records (" + (h4 * 8) + " bytes) of the header block at " + br.BaseStream.Position);

//                // This is similar to the header block in the car files, I have no idea what it is for.
//                for (int i = 0; i < h4; i++)
//                {
//                    //Console.WriteLine(i + "\t" + br.ReadInt16() + "\t" + br.ReadInt16() + "\t" + br.ReadInt16() + "\t" + br.ReadByte() + "\t" + br.ReadByte());
//                    br.ReadBytes(8);
//                }

//                Console.WriteLine("Reading " + iFirstBlockSize + " records (" + (iFirstBlockSize * 8) + " bytes) of the first block at " + br.BaseStream.Position);

//                // The first two values are always(?) the same and never repeat.
//                // Material ID lookup
//                int[] matIDs = new int[iFirstBlockSize];
//                for (int i = 0; i < iFirstBlockSize; i++)
//                {
//                    matIDs[i] = br.ReadInt16();
//                    //Console.WriteLine(i + ")\t" + br.ReadInt16() + "\t" + br.ReadUInt16() + "\t" + br.ReadUInt16());
//                    br.ReadBytes(6);
//                }

//                Console.WriteLine("Began seeking at " + br.BaseStream.Position + " of " + br.BaseStream.Length);

//                for (int i = 1; i < blockOffsets.Length; i++)
//                {
//                    int z = (i - 1) / gX;
//                    int x = (i - 1) - (z * gX);

//                    Console.WriteLine(x + ", " + z);

//                    if (blockOffsets[i] > 0)
//                    {
//                        br.BaseStream.Seek(blockOffsets[i], SeekOrigin.Begin);

//                        int uAs, uAe, uC, uD, uE, uF;
//                        short[] sX = new short[8];

//                        if (bDebug)
//                        {
//                            Console.WriteLine();
//                            Console.WriteLine("Processing footer for block " + (i - 1));
//                        }

//                        uAs = br.ReadInt32();
//                        uAe = br.ReadInt32();
//                        uC = br.ReadInt32();
//                        uD = br.ReadInt32();
//                        if (bDebug)
//                        {
//                            Console.WriteLine("Block A Start:\t" + uAs);
//                            Console.WriteLine("Block A End  :\t" + uAe);
//                            Console.WriteLine("Offset C     :\t" + uC);
//                            Console.WriteLine("Offset D     :\t" + uD);
//                        }
//                        for (int j = 0; j < 8; j++)
//                        {
//                            sX[j] = br.ReadInt16();
//                            if (bDebug) { Console.WriteLine(j + ")\t" + sX[j]); }
//                        }

//                        uE = br.ReadInt32();
//                        uF = br.ReadInt32();
//                        if (bDebug)
//                        {
//                            Console.WriteLine("Offset E     :\t" + uE);
//                            Console.WriteLine("Offset F     :\t" + uF);
//                        }

//                        bool bFlipU = false;

//                        if (1 == 1)
//                        {
//                            C2Mesh m = new C2Mesh();

//                            if (psxTex != null)
//                            {
//                                for (int j = 0; j < psxTex.Length; j++)
//                                {
//                                    m.AddListMaterial(psxTex[j].Name);
//                                }
//                            }

//                            br.BaseStream.Seek(uAs, SeekOrigin.Begin);
//                            Vector3[] verts = new Vector3[sX[0]];

//                            if (bDebug) { Console.WriteLine("Reading " + sX[0] + " records (" + (sX[0] * 8) + " bytes) of block " + (i - 1) + " at " + br.BaseStream.Position); }

//                            for (int j = 0; j < sX[0]; j++)
//                            {
//                                //Single oX = x * -8192;
//                                //Single oZ = z * 8192;

//                                //verts[j] = new Vector3(-br.ReadInt16() + oX, -br.ReadInt16(), br.ReadInt16() + oZ);

//                                //Single oX = x * 8192;
//                                //Single oZ = z * 8192;

//                                //verts[j] = new Vector3(br.ReadInt16() + oX, -br.ReadInt16(), br.ReadInt16() + oZ);

//                                float oX = x * 8192;
//                                float oZ = z * -8192;

//                                verts[j] = new Vector3(br.ReadInt16() + oX, -br.ReadInt16(), -br.ReadInt16() + oZ);

//                                // Note: Spare bytes are either 0 0 or 255 255
//                                br.ReadBytes(2);
//                            }

//                            if (bDebug) { Console.WriteLine("Reading " + sX[2] + " records (" + (sX[2] * 24) + " bytes) of block " + (i - 1) + " at " + br.BaseStream.Position); }

//                            for (int j = 0; j < sX[2]; j++)
//                            {
//                                int v1 = br.ReadInt16();
//                                int v2 = br.ReadInt16();
//                                int v3 = br.ReadInt16();

//                                int v4 = br.ReadInt16(); // This is very occasionally not 0!

//                                float v1u = br.ReadByte();
//                                float v1v = br.ReadByte();
//                                float v2u = br.ReadByte();
//                                float v2v = br.ReadByte();
//                                float v3u = br.ReadByte();
//                                float v3v = br.ReadByte();
//                                float v4u = br.ReadByte(); // This is very occasionally not 0!
//                                float v4v = br.ReadByte(); // This is very occasionally not 0!

//                                br.ReadBytes(5);

//                                int matID = matIDs[br.ReadByte()];

//                                br.ReadBytes(2);

//                                if (psxTex != null)
//                                {
//                                    if (!bFlipU)
//                                    {
//                                        v1u /= psxTex[matID].Width;
//                                        v2u /= psxTex[matID].Width;
//                                        v3u /= psxTex[matID].Width;
//                                    }
//                                    else
//                                    {
//                                        v1u = 1.0f - (v1u / psxTex[matID].Width);
//                                        v2u = 1.0f - (v2u / psxTex[matID].Width);
//                                        v3u = 1.0f - (v3u / psxTex[matID].Width);
//                                    }
//                                    v1v = 1.0f - (v1v / psxTex[matID].Height);
//                                    v2v = 1.0f - (v2v / psxTex[matID].Height);
//                                    v3v = 1.0f - (v3v / psxTex[matID].Height);
//                                }

//                                //Console.WriteLine("Vert #" + v1 + " has UV #" + m.AddUV(new Vector2(v1u, v1v)));
//                                //Console.WriteLine("Vert #" + v2 + " has UV #" + m.AddUV(new Vector2(v2u, v2v)));
//                                //Console.WriteLine("Vert #" + v3 + " has UV #" + m.AddUV(new Vector2(v3u, v3v)));

//                                //m.AddFace(verts[v1], verts[v2], verts[v3], new Vector2(v1u, v1v), new Vector2(v2u, v2v), new Vector2(v3u, v3v), matID);
//                                m.AddFace(verts[v3], verts[v2], verts[v1], new Vector2(v3u, v3v), new Vector2(v2u, v2v), new Vector2(v1u, v1v), matID);
//                                //m.AddFace(v1, v2, v3, m.AddUV(new Vector2(v1u, v1v)), m.AddUV(new Vector2(v2u, v2v)), m.AddUV(new Vector2(v3u, v3v)), matID);

//                                //Console.WriteLine(v1 + " : " + verts[v1].ToString() + " || " + v2 + " : " + verts[v2].ToString() + " || " + v3 + " : " + verts[v3].ToString());
//                                //Console.WriteLine(j + ")\t" + br.ReadByte() + "\t" + br.ReadByte() + "\t" + br.ReadByte() + "\t" + br.ReadByte() + "\t" + br.ReadByte() + "\t" + br.ReadByte() + "\t" + br.ReadInt16());
//                                //br.ReadBytes(10);
//                            }

//                            if (bDebug) { Console.WriteLine("Reading " + sX[3] + " records (" + (sX[3] * 24) + " bytes) of block " + (i - 1) + " at " + br.BaseStream.Position); }

//                            for (int j = 0; j < sX[3]; j++)
//                            {
//                                // Quads :|
//                                int v1 = br.ReadInt16();
//                                int v2 = br.ReadInt16();
//                                int v3 = br.ReadInt16();
//                                int v4 = br.ReadInt16();

//                                float v1u = br.ReadByte();
//                                float v1v = br.ReadByte();
//                                float v2u = br.ReadByte();
//                                float v2v = br.ReadByte();
//                                float v3u = br.ReadByte();
//                                float v3v = br.ReadByte();
//                                float v4u = br.ReadByte();
//                                float v4v = br.ReadByte();

//                                br.ReadBytes(5);

//                                int matID = matIDs[br.ReadByte()];

//                                br.ReadBytes(2);

//                                if (psxTex != null)
//                                {
//                                    if (!bFlipU)
//                                    {
//                                        v1u /= psxTex[matID].Width;
//                                        v2u /= psxTex[matID].Width;
//                                        v3u /= psxTex[matID].Width;
//                                        v4u /= psxTex[matID].Width;
//                                    }
//                                    else
//                                    {
//                                        v1u = 1.0f - (v1u / psxTex[matID].Width);
//                                        v2u = 1.0f - (v2u / psxTex[matID].Width);
//                                        v3u = 1.0f - (v3u / psxTex[matID].Width);
//                                        v4u = 1.0f - (v4u / psxTex[matID].Width);
//                                    }
//                                    v1v = 1.0f - (v1v / psxTex[matID].Height);
//                                    v2v = 1.0f - (v2v / psxTex[matID].Height);
//                                    v3v = 1.0f - (v3v / psxTex[matID].Height);
//                                    v4v = 1.0f - (v4v / psxTex[matID].Height);
//                                }

//                                // correct apart from UVs
//                                //m.AddFace(verts[v1], verts[v2], verts[v4], new Vector2(v1u, v1v), new Vector2(v2u, v2v), new Vector2(v4u, v4v), matID);
//                                //m.AddFace(verts[v1], verts[v4], verts[v3], new Vector2(v1u, v1v), new Vector2(v4u, v4v), new Vector2(v3u, v3v), matID);

//                                // correct apart from UVs
//                                //m.AddFace(verts[v1], verts[v2], verts[v4], new Vector2(v1u, v1v), new Vector2(v2u, v2v), new Vector2(v4u, v4v), matID);
//                                //m.AddFace(verts[v1], verts[v4], verts[v3], new Vector2(v1u, v1v), new Vector2(v4u, v4v), new Vector2(v3u, v3v), matID);

//                                // this is inside out
//                                m.AddFace(verts[v4], verts[v2], verts[v1], new Vector2(v4u, v4v), new Vector2(v2u, v2v), new Vector2(v1u, v1v), matID);
//                                m.AddFace(verts[v3], verts[v4], verts[v1], new Vector2(v3u, v3v), new Vector2(v4u, v4v), new Vector2(v1u, v1v), matID);

//                                //m.AddFace(v1, v2, v4, m.AddUV(new Vector2(v1u, v1v)), m.AddUV(new Vector2(v2u, v2v)), m.AddUV(new Vector2(v4u, v4v)), matID);
//                                //m.AddFace(v1, v4, v3, m.AddUV(new Vector2(v1u, v1v)), m.AddUV(new Vector2(v4u, v4v)), m.AddUV(new Vector2(v3u, v3v)), matID);
//                            }

//                            if (bDebug) { Console.WriteLine("Finished processing section at " + br.BaseStream.Position + " of " + br.BaseStream.Length); }

//                            m.ProcessMesh();
//                            if (bDebug)
//                            {
//                                Console.WriteLine("Adding mesh test" + i);
//                                Console.WriteLine(m.Extents.ToString());
//                            }

//                            dat.AddMesh("test" + i, 0, m);
//                        }
//                    }
//                }

//                //dat.CentreOn(0f, 0f, 0f);
//                dat.Scale(scaleFactor);
//                dat.Save(PathOut + fileIn.Replace("MAP", "dat"));
//            }
//            else
//            {
//                // Process everything else
//                int headLoop = br.ReadInt32();
//                Console.WriteLine("# of offsets " + headLoop);
//                Console.WriteLine("Unknown A " + br.ReadInt32());
//                Console.WriteLine("Unknown B " + br.ReadInt32());

//                int iCountA = br.ReadInt32();
//                Console.WriteLine("There are " + iCountA + " global entries");

//                for (int i = 0; i < headLoop - 1; i++)
//                {
//                    Console.WriteLine("Offset #" + i + " :: " + br.ReadInt32());
//                }

//                if (br.ReadInt32() != 0) { Console.WriteLine("int not 0"); }

//                Console.WriteLine("Header read at " + br.BaseStream.Position + " of " + br.BaseStream.Length);

//                Console.WriteLine("Reading " + iCountA + " (" + (8 * iCountA) + " bytes) global entries");
//                for (int i = 0; i < iCountA; i++)
//                {
//                    //Console.WriteLine(i + ") " + faceNorms[i].X + " :: " + faceNorms[i].Y + " :: " + faceNorms[i].Z + " :: " + uvScale[i]);
//                    br.ReadBytes(8);
//                }

//                DAT dat = new DAT();

//                while (br.BaseStream.Position < br.BaseStream.Length)
//                {
//                    if (bSplit) { dat = new DAT(); }
//                    C2Mesh m;

//                    Console.WriteLine("Object discovered at " + br.BaseStream.Position + " of " + br.BaseStream.Length);

//                    m = new C2Mesh();

//                    if (psxTex != null)
//                    {
//                        for (int i = 0; i < psxTex.Length; i++)
//                        {
//                            m.AddListMaterial(psxTex[i].Name);
//                        }
//                    }

//                    int iOffsetC = br.ReadInt32();
//                    int iOffsetD = br.ReadInt32();
//                    int iOffsetE = br.ReadInt32();
//                    Console.WriteLine(iOffsetC + " :: " + iOffsetD + " :: " + iOffsetE);

//                    int iCountB = br.ReadInt16();
//                    int iCountC = br.ReadInt16();

//                    Console.WriteLine("There are " + iCountB + " local x entries");
//                    Console.WriteLine("There are " + iCountC + " local y entries");

//                    for (int i = 0; i < 14; i++)
//                    {
//                        if (br.ReadInt32() != 0) { Console.WriteLine(i + " loop int not 0"); }
//                    }

//                    string name = ReadString(ref br, 32);
//                    Console.WriteLine("Name block :: " + name);
//                    Vector3[] verts = new Vector3[iCountB];

//                    for (int i = 0; i < iCountB; i++)
//                    {
//                        verts[i] = new Vector3(-br.ReadInt16(), -br.ReadInt16(), br.ReadInt16());
//                        //m.AddListVertex(verts[i] * scaleFactor);
//                        br.ReadInt16();
//                    }

//                    int uvCount = 0;

//                    for (int i = 0; i < iCountC; i++)
//                    {
//                        int v3 = br.ReadInt16();
//                        int v2 = br.ReadInt16();
//                        int v1 = br.ReadInt16();
//                        int faceID = br.ReadInt16();
//                        float v3u = br.ReadByte();
//                        float v3v = br.ReadByte();
//                        float v2u = br.ReadByte();
//                        float v2v = br.ReadByte();
//                        float v1u = br.ReadByte();
//                        float v1v = br.ReadByte();
//                        int unkA = br.ReadInt16();
//                        int unkB = br.ReadInt16();
//                        int matID = br.ReadInt16();

//                        v1u /= psxTex[matID].Width;
//                        v2u /= psxTex[matID].Width;
//                        v3u /= psxTex[matID].Width;
//                        v1v = 1.0f - (v1v / psxTex[matID].Height);
//                        v2v = 1.0f - (v2v / psxTex[matID].Height);
//                        v3v = 1.0f - (v3v / psxTex[matID].Height);

//                        int uv1 = uvCount;
//                        m.AddListVertex(verts[v1] * scaleFactor);
//                        m.AddListUV(new Vector2(v1u, v1v));
//                        uvCount++;

//                        int uv2 = uvCount;
//                        m.AddListVertex(verts[v2] * scaleFactor);
//                        m.AddListUV(new Vector2(v2u, v2v));
//                        uvCount++;

//                        int uv3 = uvCount;
//                        m.AddListVertex(verts[v3] * scaleFactor);
//                        m.AddListUV(new Vector2(v3u, v3v));
//                        uvCount++;

//                        //if (psxTex[matID].Width > psxTex[matID].Height)
//                        //{
//                        //    Console.WriteLine(i + ") " + v1 + " :: " + v2 + " :: " + v3 + " :: " + faceID + " :: " + v1u + ", " + v1v + " :: " + v2u + ", " + v2v + " :: " + v3u + ", " + v3v + " :: " + unkA + " :: " + unkB + " :: " + matID);
//                        //    Console.WriteLine(psxTex[matID].Name + " :: " + psxTex[matID].Width + " :: " + psxTex[matID].Height);
//                        //    Console.WriteLine(m.UVs[uv1].ToString() + " :: " + m.UVs[uv2].ToString() + " :: " + m.UVs[uv3].ToString());
//                        //}

//                        m.AddFace(uv1, uv2, uv3, uv1, uv2, uv3, matID);
//                    }

//                    m.ProcessMesh();
//                    dat.AddMesh(name, 0, m);
//                    if (bSplit) { dat.Save(PathOut + fileIn.Replace(".MOD", "_" + name + ".dat").Replace(".COL", ".dat")); }
//                    //    br.BaseStream.Position = br.BaseStream.Length;
//                }

//                if (!bSplit) { dat.Save(PathOut + fileIn.Replace(".MOD", "_all.dat").Replace(".COL", ".dat")); }
//            }

//            Console.WriteLine(br.BaseStream.Position + " of " + br.BaseStream.Length);

//            br.Close();
//        }

//        static public void LoopDirectoriesIn(string sPath, int mode)
//        {
//            foreach (DirectoryInfo d in new DirectoryInfo(sPath).GetDirectories())
//            {
//                LoopDirectoriesIn(d.FullName, mode);

//                switch (mode)
//                {
//                    case 1:
//                        processMODsIn(d.FullName);
//                        break;

//                    case 2:
//                        processHGXIn(d.FullName);
//                        break;

//                    case 3:
//                        processOBJsIn(d.FullName);
//                        break;
//                }
//            }
//        }

//        static private void processMODsIn(string sPath)
//        {
//            foreach (FileInfo f in new DirectoryInfo(sPath).GetFiles("*.map"))
//            {
//                Console.WriteLine("Processing file " + f.FullName);
//                ProcessMOD(f.FullName, "D:\\PSX\\", null, 0.01f, false);
//            }
//        }

//        static private void processOBJsIn(string sPath)
//        {
//            foreach (FileInfo f in new DirectoryInfo(sPath).GetFiles("*.obj"))
//            {
//                //Console.WriteLine("Processing file " + f.FullName);
//                //ProcessOBJ(f.FullName);
//            }
//        }

//        static private void processHGXIn(string sPath)
//        {
//            DirectoryInfo di = new DirectoryInfo(sPath);

//            foreach (FileInfo f in di.GetFiles("*.hgx"))
//            {
//                Console.WriteLine("Processing file " + f.FullName);
//                ProcessHGX(f.FullName, "D:\\HGX\\" + di.Name + "\\");
//            }
//        }

//        private static Color pixelToRGB(int i)
//        {
//            int r = ((i & 0x7C00) >> 10) << 3;
//            int g = ((i & 0x3E0) >> 5) << 3;
//            int b = (i & 0x1F) << 3;
//            int a = ((i & 0x8000) == 0 ? 0 : 255);
//            return Color.FromArgb(a, b, g, r);
//        }

//        public static string ReadString(ref BinaryReader br, int iLen)
//        {
//            string s = "";

//            for (int i = 0; i < iLen; i++)
//            {
//                byte b = br.ReadByte();
//                if (b != 0) { s += (char)b; }
//            }

//            return s;
//        }

//        // Meta/Aggregate functions
//        public static void ProcessCar(string CarName, string DisplayName, string InRoot, string OutParent, float ChassisOffset = 0.5f, Vector3 frontOffset = null, Vector3 backOffset = null)
//        {
//            //frontOffset = frontOffset ?? Vector3.Zero;
//            //backOffset = backOffset ?? Vector3.Zero;

//            //string outPath = OutParent + "\\" + CarName + "\\";
//            //if (!Directory.Exists(outPath)) { Directory.CreateDirectory(outPath); }

//            //ProcessTEX(InRoot + "CARS\\TEX\\" + CarName + ".TEX", outPath, out CpsxTex[] psxTex);
//            ////ProcessTEX(InRoot + "RUNTIME\\GFX\\MODELS\\TEX\\" + CarName + ".TEX", outPath, out psxTex);
//            //ProcessMOD(InRoot + "CARS\\MOD\\" + CarName + ".MOD", outPath, psxTex, 0.00165f);

//            //int datCount = new DirectoryInfo(outPath).GetFiles("*_00.dat").Length;

//            //DAT dChassis = DAT.Load(outPath + CarName + "_cell00_00.dat");
//            //DAT dFRWheel = DAT.Load(outPath + CarName + "_cell01_00.dat");
//            //DAT dFLWheel = DAT.Load(outPath + CarName + "_cell01_00.dat");
//            //DAT dRRWheel = DAT.Load(outPath + CarName + "_cell0" + (datCount > 2 ? "2" : "1") + "_00.dat");
//            //DAT dRLWheel = DAT.Load(outPath + CarName + "_cell0" + (datCount > 2 ? "2" : "1") + "_00.dat");
//            //DAT car = new DAT();

//            //dChassis.DatMeshes[0].Name = DisplayName + "PSX.dat";
//            //dFRWheel.DatMeshes[0].Name = "FRWHEEL.DAT";
//            //dFLWheel.DatMeshes[0].Name = "FLWHEEL.DAT";
//            //dRRWheel.DatMeshes[0].Name = "RRWHEEL.DAT";
//            //dRLWheel.DatMeshes[0].Name = "RLWHEEL.DAT";

//            //dChassis.DatMeshes[0].Mesh.Translate(new Vector3(0, -dChassis.DatMeshes[0].Mesh.Extents.Min.Y + (Math.Max(dFRWheel.DatMeshes[0].Mesh.Extents.Max.Y, dRRWheel.DatMeshes[0].Mesh.Extents.Max.Y) * ChassisOffset), 0));
//            ////dFRWheel.DatMeshes[0].Mesh.Translate(new Vector3(frontOffset.X, dFRWheel.DatMeshes[0].Mesh.Extents.Max.Y, frontOffset.Z));
//            ////dFLWheel.DatMeshes[0].Mesh.Translate(new Vector3(-frontOffset.X, dFLWheel.DatMeshes[0].Mesh.Extents.Max.Y, frontOffset.Z));
//            ////dRRWheel.DatMeshes[0].Mesh.Translate(new Vector3(backOffset.X, dRRWheel.DatMeshes[0].Mesh.Extents.Max.Y, backOffset.Z));
//            ////dRLWheel.DatMeshes[0].Mesh.Translate(new Vector3(-backOffset.X, dRLWheel.DatMeshes[0].Mesh.Extents.Max.Y, backOffset.Z));

//            //dChassis.DatMeshes[0].Mesh.GenerateKDOP(18);
//            //SaveBoundingBox(dChassis.DatMeshes[0].Mesh.intersectionPoints, outPath + DisplayName + "PSX_BB.txt");
//            ////ACT kdop = new ACT();
//            ////kdop.AddRootNode("pointcloud");
//            ////for (int i = 0; i < dChassis.DatMeshes[0].Mesh.intersectionPoints.Count; i++)
//            ////{
//            ////    Matrix3D m = Matrix3D.Identity;
//            ////    m.Scale = 0.05f;
//            ////    m.Position = dChassis.DatMeshes[0].Mesh.intersectionPoints[i];

//            ////    kdop.AddActor("p" + i, "Sphere_48", m, false);
//            ////}
//            ////kdop.Save("D:\\kdop.act");

//            //dChassis.DatMeshes[0].Mesh.ProcessMesh();
//            //Console.WriteLine(dChassis.DatMeshes[0].Mesh.Extents);

//            //car.DatMeshes.Add(dChassis.DatMeshes[0]);
//            //car.DatMeshes.Add(dFRWheel.DatMeshes[0]);
//            //car.DatMeshes.Add(dFLWheel.DatMeshes[0]);
//            //car.DatMeshes.Add(dRRWheel.DatMeshes[0]);
//            //car.DatMeshes.Add(dRLWheel.DatMeshes[0]);
//            //car.Optimise();
//            //car.Save(outPath + DisplayName + "PSX.dat");

//            //C2Mesh mSimple = new C2Mesh();
//            //mSimple.BuildFromExtents(car.DatMeshes[0].Mesh.Extents);
//            //DAT dSimple = new DAT(new DatMesh(DisplayName + "PSX.1", mSimple));
//            //dSimple.Save(outPath + "simple_" + DisplayName + "PSX.dat");

//            //ACT aSimple = new ACT();
//            //aSimple.AddActor(DisplayName + "PSX.1", DisplayName + "PSX.1", Matrix3D.Identity, true);
//            //aSimple.Save(outPath + "simple_" + DisplayName + "PSX.act");

//            //dChassis.DatMeshes[0].Name = "SHELL";
//            //DAT shell = new DAT();
//            //shell.DatMeshes.Add(dChassis.DatMeshes[0]);
//            //shell.Optimise();
//            //shell.Save(outPath + DisplayName + "PSXshell.dat");

//            //ACT a = new ACT();
//            //a.AddActor(DisplayName + "PSX.dat", DisplayName + "PSX.dat", Matrix3D.Identity, true);
//            //a.AddActor("RRWHEEL.ACT", "RRWHEEL.DAT", new Matrix3D(car.DatMeshes[3].Mesh.Centre + new Vector3(backOffset.X, dRRWheel.DatMeshes[0].Mesh.Extents.Max.Y, backOffset.Z)), false);
//            //a.AddActor("RLWHEEL.ACT", "RLWHEEL.DAT", new Matrix3D(car.DatMeshes[4].Mesh.Centre + new Vector3(-backOffset.X, dRLWheel.DatMeshes[0].Mesh.Extents.Max.Y, backOffset.Z)), false);
//            //a.AddPivot("FRPIVOT.ACT", "FRWHEEL.ACT", "FRWHEEL.DAT", new Matrix3D(car.DatMeshes[1].Mesh.Centre + new Vector3(frontOffset.X, dFRWheel.DatMeshes[0].Mesh.Extents.Max.Y, frontOffset.Z)));
//            //a.AddPivot("FLPIVOT.ACT", "FLWHEEL.ACT", "FLWHEEL.DAT", new Matrix3D(car.DatMeshes[2].Mesh.Centre + new Vector3(-frontOffset.X, dFLWheel.DatMeshes[0].Mesh.Extents.Max.Y, frontOffset.Z)));
//            a.Save(outPath + DisplayName + "PSX.act");
//        }

//        public static void SaveBoundingBox(List<Vector3> points, string file)
//        {
//            using (StreamWriter txt = new StreamWriter(file))
//            {
//                txt.WriteLine("polyhedron\t\t\t// Type");
//                txt.WriteLine(points.Count);

//                for (int i = 0; i < points.Count; i++)
//                {
//                    txt.WriteLine(string.Format("{0},{1},{2}", points[i].X, points[i].X, points[i].X));
//                }
//            }
//        }
//    }
//}