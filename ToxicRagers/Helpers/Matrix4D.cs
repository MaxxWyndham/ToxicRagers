namespace ToxicRagers.Helpers
{
    public class Matrix4D
    {
        public float M11; public float M12; public float M13; public float M14;
        public float M21; public float M22; public float M23; public float M24;
        public float M31; public float M32; public float M33; public float M34;
        public float M41; public float M42; public float M43; public float M44;

        public Matrix4D(
            float M11, float M12, float M13, float M14,
            float M21, float M22, float M23, float M24,
            float M31, float M32, float M33, float M34,
            float M41, float M42, float M43, float M44)
        {
            this.M11 = M11; this.M12 = M12; this.M13 = M13; this.M14 = M14;
            this.M21 = M21; this.M22 = M22; this.M23 = M23; this.M24 = M24;
            this.M31 = M31; this.M32 = M32; this.M33 = M33; this.M34 = M34;
            this.M41 = M41; this.M42 = M42; this.M43 = M43; this.M44 = M44;
        }

        public override string ToString()
        {
            return string.Format("{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} }}", M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
        }
    }
}