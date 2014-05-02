using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToxicRagers.Helpers;
//using Microsoft.Xna.Framework;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class c2Mesh
    {
        public List<string> Materials;
        public List<Vector3> Verts;
        public List<Vector3> Normals;
        public List<Vector2> UVs;
        public List<c2Face> Faces;
        public MeshExtents Extents;

        public c2Mesh()
        {
            Verts = new List<Vector3>();
            UVs = new List<Vector2>();
            Faces = new List<c2Face>();
            Materials = new List<string>();
        }

        // ============= Build by List =============
        public void AddListVertex(Vector3 V)
        {
            Verts.Add(V);
        }

        public void AddListVertex(Single X, Single Y, Single Z)
        {
            Verts.Add(new Vector3(X, Y, Z));
        }

        public void AddListUV(Vector2 UV)
        {
            UVs.Add(UV);
        }

        public void AddListUV(Single U, Single V)
        {
            UVs.Add(new Vector2(U, V));
        }

        public void AddListMaterial(string Name)
        {
            Materials.Add(Name);
        }

        public void AddFace(int v1, int v2, int v3)
        {
            Faces.Add(new c2Face(v1, v2, v3, -1));
        }

        public void AddFace(int v1, int v2, int v3, int MatID)
        {
            Faces.Add(new c2Face(v1, v2, v3, MatID));
        }

        public void AddFace(int v1, int v2, int v3, int uv1, int uv2, int uv3, int MaterialID)
        {
            Faces.Add(new c2Face(v1, v2, v3, uv1, uv2, uv3, MaterialID));
        }
        // =========================================

        // ============ Build by Values ============
        public void AddFace(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            if (!Verts.Contains(v1)) { Verts.Add(v1); }
            if (!Verts.Contains(v2)) { Verts.Add(v2); }
            if (!Verts.Contains(v3)) { Verts.Add(v3); }

            AddFace(Verts.IndexOf(v1), Verts.IndexOf(v2), Verts.IndexOf(v3), 0);
        }

        public void AddFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3, int matID)
        {
            int iv1, iv2, iv3, iuv1, iuv2, iuv3;

            Verts.Add(v1);
            iv1 = Verts.Count - 1;
            Verts.Add(v2);
            iv2 = Verts.Count - 1;
            Verts.Add(v3);
            iv3 = Verts.Count - 1;

            UVs.Add(uv1);
            iuv1 = UVs.Count - 1;
            UVs.Add(uv2);
            iuv2 = UVs.Count - 1;
            UVs.Add(uv3);
            iuv3 = UVs.Count - 1;

            AddFace(iv1, iv2, iv3, iuv1, iuv2, iuv3, matID);
        }
        // =========================================

        // ============= Build by Shape ============
        public void BuildFromExtents(MeshExtents box)
        {
            Verts.Add(new Vector3(box.Min.X, box.Min.Y, box.Max.Z));
            Verts.Add(new Vector3(box.Max.X, box.Min.Y, box.Max.Z));
            Verts.Add(new Vector3(box.Max.X, box.Max.Y, box.Max.Z));
            Verts.Add(new Vector3(box.Min.X, box.Max.Y, box.Max.Z));
            Verts.Add(new Vector3(box.Min.X, box.Min.Y, box.Min.Z));
            Verts.Add(new Vector3(box.Max.X, box.Min.Y, box.Min.Z));
            Verts.Add(new Vector3(box.Max.X, box.Max.Y, box.Min.Z));
            Verts.Add(new Vector3(box.Min.X, box.Max.Y, box.Min.Z));

            UVs.Add(new Vector2(0.0001f, 0.9999f));
            UVs.Add(new Vector2(0.9999f, 0.9999f));
            UVs.Add(new Vector2(0.9999f, 0.9999f));
            UVs.Add(new Vector2(0.0001f, 0.9999f));
            UVs.Add(new Vector2(0.0001f, 0.0001f));
            UVs.Add(new Vector2(0.9999f, 0.0001f));
            UVs.Add(new Vector2(0.9999f, 0.0001f));
            UVs.Add(new Vector2(0.0001f, 0.0001f));

            AddFace(3, 4, 0);
            AddFace(0, 2, 3);
            AddFace(0, 1, 2);
            AddFace(1, 5, 6);
            AddFace(1, 6, 2);
            AddFace(3, 7, 4);
            AddFace(2, 7, 3);
            AddFace(2, 6, 7);
            AddFace(0, 5, 1);
            AddFace(4, 7, 6);
            AddFace(0, 4, 5);
            AddFace(4, 6, 5);
        }
        // =========================================

        public int AddUV(Vector2 uv)
        {
            if (!UVs.Contains(uv)) { UVs.Add(uv); }
            return UVs.IndexOf(uv);
        }

        public void SetMaterialForFace(int faceID, int matID)
        {
            Faces[faceID].MaterialID = matID;
        }

        public void ProcessMesh()
        {
            Vector3 min, max;
            min = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
            max = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);

            for (int i = 0; i < Verts.Count; i++)
            {
                if (min.X > Verts[i].X) { min.X = Verts[i].X; }
                if (min.Y > Verts[i].Y) { min.Y = Verts[i].Y; }
                if (min.Z > Verts[i].Z) { min.Z = Verts[i].Z; }

                if (max.X < Verts[i].X) { max.X = Verts[i].X; }
                if (max.Y < Verts[i].Y) { max.Y = Verts[i].Y; }
                if (max.Z < Verts[i].Z) { max.Z = Verts[i].Z; }
            }

            Extents = new MeshExtents(min, max);
        }

        public Vector3[] GetVertexList()
        {
            Vector3[] v = new Vector3[Faces.Count * 3];

            for (int i = 0; i < Faces.Count; i++)
            {
                v[i * 3] = Verts[Faces[i].V1];
                v[(i * 3) + 1] = Verts[Faces[i].V2];
                v[(i * 3) + 2] = Verts[Faces[i].V3];
            }

            return v;
        }

        public Vector2[] GetUVList()
        {
            Vector2[] uv = new Vector2[Faces.Count * 3];

            if (UVs.Count > 0)
            {
                for (int i = 0; i < Faces.Count; i++)
                {
                    uv[i * 3] = UVs[Faces[i].V1];
                    uv[(i * 3) + 1] = UVs[Faces[i].V2];
                    uv[(i * 3) + 2] = UVs[Faces[i].V3];
                }
            }
            else
            {
                for (int i = 0; i < Faces.Count; i++)
                {
                    uv[i * 3] = Vector2.Zero;
                    uv[(i * 3) + 1] = Vector2.Zero;
                    uv[(i * 3) + 2] = Vector2.Zero;
                }
            }

            return uv;
        }

        public void GenerateNormals()
        {
            Normals = new List<Vector3>();

            for (int i = 0; i < Verts.Count; i++) { Normals.Add(Vector3.Zero); }

            for (int i = 0; i < Faces.Count; i++)
            {
                Vector3 v0 = Verts[Faces[i].V1];
                Vector3 v1 = Verts[Faces[i].V2];
                Vector3 v2 = Verts[Faces[i].V3];

                Vector3 normal = Vector3.Cross(v2 - v0, v1 - v0);

                float sin_alpha = normal.Length / ((v2 - v0).Length * (v1 - v0).Length);
                normal = normal.Normalise * (float)Math.Asin(sin_alpha);

                Normals[Faces[i].V1] += normal;
                Normals[Faces[i].V2] += normal;
                Normals[Faces[i].V3] += normal;
            }

            for (int i = 0; i < Normals.Count; i++)
            {
                Normals[i] = Normals[i].Normalise;
            }
        }

        public void AssignUVs()
        {
            foreach (var face in Faces)
            {
                face.UVs[0] = face.V1;
                face.UVs[1] = face.V2;
                face.UVs[2] = face.V3;
            }
        }

        public Vector3 Centre
        {
            get
            {
                return (Extents.Min + Extents.Max) / 2;
            }
        }

        // Helper functions
        public void Translate(Vector3 by)
        {
            for (int i = 0; i < Verts.Count; i++)
            {
                Verts[i] += by;
            }
        }

        public void Scale(Single by)
        {
            for (int i = 0; i < Verts.Count; i++)
            {
                Verts[i] *= by;
            }
        }

        public void Optimise()
        {
            List<c2Vertex> points = new List<c2Vertex>();
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            for (int i = 0; i < Verts.Count; i++)
            {
                c2Vertex p = new c2Vertex(Verts[i], UVs[i]);

                //Console.WriteLine("Vert " + i);
                int newID = points.IndexOf(p);
                //int newID = verts.IndexOf(Verts[i]);

                if (newID == -1)
                {
                    //Console.WriteLine("Adding " + p.ToString());
                    points.Add(p);
                    verts.Add(Verts[i]);
                    uvs.Add(UVs[i]);
                    newID = verts.Count - 1;
                }

                for (int j = 0; j < Faces.Count; j++)
                {
                    Faces[j].ReplaceVertID(i, newID);
                }
            }

            //Console.WriteLine("Reduced Vert count from " + Verts.Count + " to " + verts.Count);
            //Console.WriteLine("Reduced UV count from " + UVs.Count + " to " + uvs.Count);

            Verts.Clear();
            for (int i = 0; i < verts.Count; i++) { Verts.Add(verts[i]); }
            UVs.Clear();
            for (int i = 0; i < uvs.Count; i++) { UVs.Add(uvs[i]); }
        }

        public void GenerateKDOP(int K = 6)
        {
            Vector3[] AABB = new Vector3[3];
            AABB[0] = new Vector3(1, 0, 0);
            AABB[1] = new Vector3(0, 1, 0);
            AABB[2] = new Vector3(0, 0, 1);

            Vector3[] Corners = new Vector3[4];
            Corners[0] = new Vector3(1, 1, 1);
            Corners[1] = new Vector3(1, -1, 1);
            Corners[2] = new Vector3(1, 1, -1);
            Corners[3] = new Vector3(1, -1, -1);

            Vector3[] Edges = new Vector3[6];
            Edges[0] = new Vector3(1, 1, 0);
            Edges[1] = new Vector3(1, 0, 1);
            Edges[2] = new Vector3(0, 1, 1);
            Edges[3] = new Vector3(1, -1, 0);
            Edges[4] = new Vector3(1, 0, -1);
            Edges[5] = new Vector3(0, 1, -1);

            Vector3[] bounds = new Vector3[K / 2];
            int offset = 0;

            if (K == 6 || K == 14 || K == 18 || K == 26)
            {
                Array.Copy(AABB, 0, bounds, offset, AABB.Length);
                offset += AABB.Length;
            }

            if (K == 8 || K == 14 || K == 26)
            {
                Array.Copy(Corners, 0, bounds, offset, Corners.Length);
                offset += Corners.Length;
            }

            if (K == 12 || K == 18 || K == 26)
            {
                Array.Copy(Edges, 0, bounds, offset, Edges.Length);
                offset += Edges.Length;
            }

            // Initialise output extents
            Plane[] min = new Plane[K / 2];
            Plane[] max = new Plane[K / 2];

            for (int i = 0; i < min.Length; i++)
            {
                min[i] = new Plane(Vector3.Zero, bounds[i], Single.MaxValue);
                max[i] = new Plane(Vector3.Zero, bounds[i], Single.MinValue);
            }

            Single value = 0;
            for (int i = 0; i < Verts.Count; i++)
            {
                for (int j = 0; j < bounds.Length; j++)
                {
                    value = (Verts[i] * bounds[j]).Sum();

                    if (value < min[j].Distance)
                    {
                        min[j].Distance = value;
                        min[j].Point = Verts[i];
                    }
                    else if (value > max[j].Distance)
                    {
                        max[j].Distance = value;
                        max[j].Point = Verts[i];
                    }
                }
            }

            Plane[] planes = new Plane[K];
            for (int i = 0; i < min.Length; i++)
            {
                planes[2 * i] = new Plane(min[i].Point, -min[i].Normal, Vector3.Dot(-min[i].Normal, min[i].Point));
                planes[2 * i + 1] = max[i];
            }

            for (int i = 0; i < planes.Length; i++)
            {
                //Console.WriteLine(Vector3.Dot(planes[i].Normal, planes[i].Point) + " :: " + planes[i].Distance);
                Console.WriteLine(planes[i]);
            }

            Vector3 v;

            //for (int a = 0; a < planes.Length; a++)
            //{
            //    for (int b = a; b < planes.Length; b++)
            //    {
            //        for (int c = b; c < planes.Length; c++)
            //        {
            //            if (PlanePlanePlaneIntersect(planes[a], planes[b], planes[c], out v))
            //            {
            //                Console.WriteLine(a + " :: " + b + " :: " + c);
            //                intersectionPoints.Add(v);
            //            }
            //        }
            //    }
            //}

            PlanePlanePlaneIntersect(planes[3], planes[7], planes[11], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[3], planes[11], planes[12], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[3], planes[12], planes[17], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[3], planes[17], planes[7], out v);
            intersectionPoints.Add(v);

            PlanePlanePlaneIntersect(planes[0], planes[11], planes[5], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[0], planes[16], planes[5], out v);
            intersectionPoints.Add(v);

            PlanePlanePlaneIntersect(planes[0], planes[12], planes[4], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[0], planes[6], planes[4], out v);
            intersectionPoints.Add(v);

            PlanePlanePlaneIntersect(planes[1], planes[7], planes[5], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[1], planes[13], planes[5], out v);
            intersectionPoints.Add(v);

            PlanePlanePlaneIntersect(planes[1], planes[17], planes[4], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[1], planes[10], planes[4], out v);
            intersectionPoints.Add(v);

            PlanePlanePlaneIntersect(planes[2], planes[13], planes[16], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[2], planes[16], planes[6], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[2], planes[6], planes[10], out v);
            intersectionPoints.Add(v);
            PlanePlanePlaneIntersect(planes[2], planes[10], planes[13], out v);
            intersectionPoints.Add(v);
        }

        public List<Vector3> intersectionPoints = new List<Vector3>();

        public bool PlanePlanePlaneIntersect(Plane p1, Plane p2, Plane p3, out Vector3 r)
        {
            r = Vector3.Zero;

            Vector3 m1 = new Vector3(p1.Normal.X, p2.Normal.X, p3.Normal.X);
            Vector3 m2 = new Vector3(p1.Normal.Y, p2.Normal.Y, p3.Normal.Y);
            Vector3 m3 = new Vector3(p1.Normal.Z, p2.Normal.Z, p3.Normal.Z);

            Vector3 u = Vector3.Cross(m2, m3);
            Single denom = Vector3.Dot(m1, u);

            if (Math.Abs(denom) < SMALL_NUM) { return false; }

            Vector3 d = new Vector3(p1.Distance, p2.Distance, p3.Distance);
            Vector3 v = Vector3.Cross(m1, d);
            Single ood = 1.0f / denom;

            r.X = Vector3.Dot(d, u) * ood;
            r.Y = Vector3.Dot(m3, v) * ood;
            r.Z = -Vector3.Dot(m2, v) * ood;

            return true;
        }

        Single SMALL_NUM = 0.00000001f;

        //public int PlanePlaneIntersect(Plane p1, Plane p2, out Line L)
        //{
        //    L = new Line();

        //    Console.WriteLine(p1);
        //    Console.WriteLine(p2);
        //    //Vector3 direction = Vector3.Cross(p1.Normal, p2.Normal);
        //    Vector3 direction = Vector3.Cross(p1.Normal * p1.Distance, p2.Normal * p2.Distance);
        //    Single dnorm = direction.Length;

        //    if (dnorm < SMALL_NUM)
        //    {
        //        // parallel
        //        Console.WriteLine();
        //        return 0;
        //    }

        //    Single abs;
        //    Single maxabs = (direction.X < 0 ? -direction.X : direction.X);
        //    int index = 0;

        //    if ((abs = (direction.Y < 0 ? -direction.Y : direction.Y)) > maxabs) { maxabs = abs; index = 1; }
        //    if ((abs = (direction.Z < 0 ? -direction.Z : direction.Z)) > maxabs) { maxabs = abs; index = 2; }

        //    switch (index)
        //    {
        //        case 0:
        //            Console.WriteLine(new Vector3(0, (p1.Normal.Y * p2.Normal.Z - p2.Normal.Y * p1.Normal.Z) / direction.X, (p2.Normal.X * p1.Normal.Z - p1.Normal.X * p2.Normal.Z) / direction.X));
        //            break;
        //    }

        //    Console.WriteLine(direction * (1 / dnorm));
        //    Console.WriteLine();
        //    return 0;
        //}

        ////public int PlanePlaneIntersect(Plane Pn1, Plane Pn2, out Line L)
        ////{
        ////    Vector3 u = Vector3.Cross(Pn1.Normal, Pn2.Normal);
        ////    Single ax = (u.X >= 0 ? u.X : -u.X);
        ////    Single ay = (u.Y >= 0 ? u.Y : -u.Y);
        ////    Single az = (u.Z >= 0 ? u.Z : -u.Z);

        ////    L = new Line();

        ////    // test if the two planes are parallel
        ////    if ((ax+ay+az) < SMALL_NUM) {       // Pn1 and Pn2 are near parallel
        ////        // test if disjoint or coincide
        ////        Vector3 v = Pn2.Point - Pn1.Point;
        ////        if (Vector3.Dot(Pn1.Normal, v) == 0)
        ////        {
        ////            //Console.WriteLine("Planes coincide:");
        ////            //Console.WriteLine("P1 " + Pn1);
        ////            //Console.WriteLine("P2 " + Pn2);
        ////            //Console.WriteLine();
        ////            return 1;
        ////        }
        ////        else
        ////        {
        ////            //Console.WriteLine("Planes disjointed:");
        ////            //Console.WriteLine("P1 " + Pn1);
        ////            //Console.WriteLine("P2 " + Pn2);
        ////            //Console.WriteLine();
        ////            return 0;
        ////        }
        ////    }

        ////    // Pn1 and Pn2 intersect in a line
        ////    // first determine max abs coordinate of cross product
        ////    int      maxc;                      // max coordinate
        ////    if (ax > ay) {
        ////        if (ax > az)
        ////             maxc = 1;
        ////        else maxc = 3;
        ////    }
        ////    else {
        ////        if (ay > az)
        ////             maxc = 2;
        ////        else maxc = 3;
        ////    }

        ////    // next, to get a point on the intersect line
        ////    // zero the max coord, and solve for the other two
        ////    Vector3  iP = Vector3.Identity;               // intersect point
        ////    Single   d1, d2;           // the constants in the 2 plane equations
        ////    d1 = -Vector3.Dot(Pn1.Normal, Pn1.Point);  // note: could be pre-stored with plane
        ////    d2 = -Vector3.Dot(Pn2.Normal, Pn2.Point);  // ditto

        ////    switch (maxc) {            // select max coordinate
        ////    case 1:                    // intersect with x=0
        ////        iP.X = 0;
        ////        iP.Y = (d2*Pn1.Normal.Z - d1*Pn2.Normal.Z) / u.X;
        ////        iP.Z = (d1*Pn2.Normal.Y - d2*Pn1.Normal.Y) / u.X;
        ////        break;
        ////    case 2:                    // intersect with y=0
        ////        iP.X = (d1*Pn2.Normal.Z - d2*Pn1.Normal.Z) / u.Y;
        ////        iP.Y = 0;
        ////        iP.Z = (d2*Pn1.Normal.X - d1*Pn2.Normal.X) / u.Y;
        ////        break;
        ////    case 3:                    // intersect with z=0
        ////        iP.X = (d2*Pn1.Normal.Y - d1*Pn2.Normal.Y) / u.Z;
        ////        iP.Y = (d1*Pn2.Normal.X - d2*Pn1.Normal.X) / u.Z;
        ////        iP.Z = 0;
        ////        break;
        ////    }

        ////    L.Point0 = iP;
        ////    L.Point0 = iP + u;

        ////    //Console.WriteLine("Planes intersect:");
        ////    //Console.WriteLine("P1 " + Pn1);
        ////    //Console.WriteLine("P2 " + Pn2);
        ////    //Console.WriteLine("L  " + L);
        ////    //Console.WriteLine();

        ////    return 2;
        ////}

        ////public void LineLineIntersect(Line l1, Line l2)
        ////{
        ////    Vector3 P1 = l1.Point0;
        ////    Vector3 P2 = l2.Point0;
        ////    Vector3 V1 = l1.Direction;
        ////    Vector3 V2 = l2.Direction;

        ////    Vector3 lhs = Vector3.Cross(V1, V2);
        ////    Vector3 rhs = Vector3.Cross(P2 - P1, V2);

        ////    if (lhs.Length == 0) { return; }
        ////    //if (Vector3.Cross(lhs,rhs) != Vector3.Identity) { return; }

        ////    Single a = lhs.Length / rhs.Length;
        ////    Console.WriteLine(P1 + (a * V1));
        ////}

        //public void LinePlaneIntersect(Line l, Plane p)
        //{
        //    Vector3 u = l.Point0 - l.Point1;

        //    if (Vector3.Dot(p.Normal, u) == 0)
        //    {
        //        // Perpendicular
        //        Console.WriteLine(l);
        //        Console.WriteLine(p);
        //        Console.WriteLine(Vector3.Dot(p.Normal, l.Point0 - p.Point));
        //    }


        //}
    }
}
