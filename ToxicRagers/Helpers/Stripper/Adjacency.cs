using System;
using System.Diagnostics;
using System.Linq;

namespace ToxicRagers.Helpers.Stripper
{
    public class Adjacency
    {
        int currentFace = 0;
        readonly int facecount;
        int edgecount = 0;
        readonly AdjEdge[] edges;

        public AdjTriangle[] Faces { get; }

        public Adjacency(int faceCount, int[] indexBuffer)
        {
            facecount = faceCount;
            Faces = new AdjTriangle[facecount];
            edges = new AdjEdge[facecount * 3];

            for (int i = 0; i < facecount; i++)
            {
                AddTriangle(
                    indexBuffer[i * 3 + 0], 
                    indexBuffer[i * 3 + 2], // + 1
                    indexBuffer[i * 3 + 1]  // + 2 to reverse winding order of strips
                );
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            Faces[currentFace] = new AdjTriangle();
            Faces[currentFace].Ref[0] = a;
            Faces[currentFace].Ref[1] = b;
            Faces[currentFace].Ref[2] = c;

            Faces[currentFace].Tri[0] = -1;
            Faces[currentFace].Tri[1] = -1;
            Faces[currentFace].Tri[2] = -1;

            if (a < b) { AddEdge(a, b, currentFace); }
            else {       AddEdge(b, a, currentFace); }

            if (a < c) { AddEdge(a, c, currentFace); }
            else {       AddEdge(c, a, currentFace); }

            if (b < c) { AddEdge(b, c, currentFace); }
            else {       AddEdge(c, b, currentFace); }

            currentFace++;
        }

        public void AddEdge(int a, int b, int face)
        {
            edges[edgecount] = new AdjEdge
            {
                Ref0 = a,
                Ref1 = b,
                Face = face
            };

            edgecount++;
        }

        public void CreateDatabase()
        {
            AdjEdge[] sorted = edges.OrderBy(e => e.Ref1).ThenBy(e => e.Ref0).ThenBy(e => e.Face).ToArray();

            int lastRef0 = sorted[0].Ref0;
            int lastRef1 = sorted[0].Ref1;
            int count = 0;
            int[] tmp = new int[3];

            for (int i = 0; i < edgecount; i++)
            {
                int face = sorted[i].Face;
                int ref0 = sorted[i].Ref0;
                int ref1 = sorted[i].Ref1;

                if (ref0 == lastRef0 && ref1 == lastRef1)
                {
                    tmp[count++] = face;
                    if (count == 3)
                    {
                        return;
                        //throw new NotImplementedException("Gah");
                    }
                }
                else
                {
                    if (count == 2)
                    {
                        if (!UpdateLink(tmp[0], tmp[1], lastRef0, lastRef1))
                        {
                            throw new NotImplementedException("Urk");
                        }
                    }

                    count = 0;
                    tmp[count++] = face;
                    lastRef0 = ref0;
                    lastRef1 = ref1;
                }
            }

            if (count == 2) { UpdateLink(tmp[0], tmp[1], lastRef0, lastRef1); }
        }

        public bool UpdateLink(int tri1, int tri2, int ref0, int ref1)
        {
            AdjTriangle t1 = Faces[tri1];
            AdjTriangle t2 = Faces[tri2];

            byte edge0 = t1.FindEdge(ref0, ref1); if (edge0 == 255) { return false; }
            byte edge1 = t2.FindEdge(ref0, ref1); if (edge1 == 255) { return false; }

            t1.Tri[edge0] = tri2;// | ((int)edge1 << 30);
            t2.Tri[edge1] = tri1;// | ((int)edge0 << 30);

            return true;
        }
    }

    [DebuggerDisplay("VRef={Ref[0]} {Ref[1]} {Ref[2]} ATri={Tri[0]} {Tri[1]} {Tri[2]}")]
    public class AdjTriangle
    {
        public int[] Ref { get; } = new int[3];

        public int[] Tri { get; } = new int[3];

        public byte FindEdge(int ref0, int ref1)
        {
            byte edge = 255;

                 if (Ref[0] == ref0 && Ref[1] == ref1) { edge = 0; }
            else if (Ref[0] == ref1 && Ref[1] == ref0) { edge = 0; }
            else if (Ref[0] == ref0 && Ref[2] == ref1) { edge = 1; }
            else if (Ref[0] == ref1 && Ref[2] == ref0) { edge = 1; }
            else if (Ref[1] == ref0 && Ref[2] == ref1) { edge = 2; }
            else if (Ref[1] == ref1 && Ref[2] == ref0) { edge = 2; }

            return edge;
        }

        public int OppositeVertex(int ref0, int ref1)
        {
            int vref = -1;

                 if (Ref[0] == ref0 && Ref[1] == ref1) { vref = Ref[2]; }
            else if (Ref[0] == ref1 && Ref[1] == ref0) { vref = Ref[2]; }
            else if (Ref[0] == ref0 && Ref[2] == ref1) { vref = Ref[1]; }
            else if (Ref[0] == ref1 && Ref[2] == ref0) { vref = Ref[1]; }
            else if (Ref[1] == ref0 && Ref[2] == ref1) { vref = Ref[0]; }
            else if (Ref[1] == ref1 && Ref[2] == ref0) { vref = Ref[0]; }

            return vref;
        }
    }

    [DebuggerDisplay("Ref0={Ref0} Ref1={Ref1} FaceNb={Face}")]
    public class AdjEdge
    {
        public int Ref0 { get; set; }

        public int Ref1 { get; set; }

        public int Face { get; set; }
    }
}
