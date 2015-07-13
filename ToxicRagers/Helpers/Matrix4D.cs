using System;

namespace ToxicRagers.Helpers
{
    public class Matrix4D
    {
        public Single M11; public Single M12; public Single M13; public Single M14;
        public Single M21; public Single M22; public Single M23; public Single M24;
        public Single M31; public Single M32; public Single M33; public Single M34;
        public Single M41; public Single M42; public Single M43; public Single M44;

        public Matrix4D(
            Single M11, Single M12, Single M13, Single M14,
            Single M21, Single M22, Single M23, Single M24,
            Single M31, Single M32, Single M33, Single M34,
            Single M41, Single M42, Single M43, Single M44)
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
