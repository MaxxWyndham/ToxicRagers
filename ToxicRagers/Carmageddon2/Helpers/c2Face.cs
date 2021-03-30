using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    public class C2Face
    {
        public int[] Verts { get; set; } = new int[3];

        public int[] UVs { get; set; } = new int[3];

        public int MaterialID { get; set; } = -1;

        public Vector3 Normal { get; set; } = Vector3.Up;

        public int SmoothingGroup { get; set; } = 0;

        public C2Face(int v1, int v2, int v3, int materialID)
            : this(v1, v2, v3, -1, -1, -1, materialID)
        {
        }

        public C2Face(int v1, int v2, int v3, int uv1, int uv2, int uv3)
            : this(v1, v2, v3, uv1, uv2, uv3, -1)
        {
        }

        public C2Face(int v1, int v2, int v3, int uv1, int uv2, int uv3, int materialID)
        {
            Verts[0] = v1;
            Verts[1] = v2;
            Verts[2] = v3;

            UVs[0] = uv1;
            UVs[1] = uv2;
            UVs[2] = uv3;

            MaterialID = materialID;
        }

        public int V1 => Verts[0];

        public int V2 => Verts[1];

        public int V3 => Verts[2];

        public int UV1 => UVs[0];

        public int UV2 => UVs[1];

        public int UV3 => UVs[2];

        public void SetMaterialID(int materialID)
        {
            MaterialID = materialID;
        }

        public void ReplaceVertID(int oldID, int newID)
        {
            for (int i = 0; i < UVs.Length; i++)
            {
                if (Verts[i] == oldID) { Verts[i] = newID; }
            }
        }

        public override string ToString()
        {
            return $"V1: {V1} V2: {V2} V3: {V3}";
        }
    }
}