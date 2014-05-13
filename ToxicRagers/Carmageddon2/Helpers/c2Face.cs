using System;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class c2Face
    {
        public int[] Verts = new int[3];
        public int[] UVs = new int[3];
        public int MaterialID = -1;
        public Vector3 Normal = Vector3.Up;

        public c2Face(int V1, int V2, int V3, int MaterialID)
            : this(V1, V2, V3, -1, -1, -1, MaterialID)
        {
        }

        public c2Face(int V1, int V2, int V3, int UV1, int UV2, int UV3)
            : this(V1, V2, V3, UV1, UV2, UV3, -1)
        {
        }

        public c2Face(int V1, int V2, int V3, int UV1, int UV2, int UV3, int MaterialID)
        {
            Verts[0] = V1;
            Verts[1] = V2;
            Verts[2] = V3;

            UVs[0] = UV1;
            UVs[1] = UV2;
            UVs[2] = UV3;

            this.MaterialID = MaterialID;
        }

        public int V1 { get { return Verts[0]; } }
        public int V2 { get { return Verts[1]; } }
        public int V3 { get { return Verts[2]; } }

        public int UV1 { get { return UVs[0]; } }
        public int UV2 { get { return UVs[1]; } }
        public int UV3 { get { return UVs[2]; } }

        public void SetMaterialID(int MaterialID)
        {
            this.MaterialID = MaterialID;
        }

        public void ReplaceVertID(int OldID, int NewID)
        {
            for (int i = 0; i < UVs.Length; i++)
            {
                if (Verts[i] == OldID) { Verts[i] = NewID; }
            }
        }

        public override string ToString()
        {
            return "V1: " + V1 + " V2: " + V2 + " V3: " + V3;
        }
    }
}
