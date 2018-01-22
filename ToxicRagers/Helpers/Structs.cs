namespace ToxicRagers.Helpers
{
    public struct MeshExtents
    {
        public Vector3 Min;
        public Vector3 Max;

        public MeshExtents(Vector3 Min, Vector3 Max)
        {
            this.Min = Min;
            this.Max = Max;
        }

        public override string ToString()
        {
            return "Min : " + Min.ToString() + " Max : " + Max.ToString();
        }
    }
}