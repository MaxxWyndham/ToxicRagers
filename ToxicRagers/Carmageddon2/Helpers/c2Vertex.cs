using System;
using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    class c2Vertex : IEquatable<c2Vertex>
    {
        public Vector3 Position;
        public Vector2 UV;

        public c2Vertex(Vector3 Position, Vector2 UV)
        {
            this.Position = Position;
            this.UV = UV;
        }

        public override string ToString()
        {
            return Position.ToString() + " :: " + UV.ToString();
        }

        public bool Equals(c2Vertex other)
        {
            //Console.WriteLine("Comparing...");
            //Console.WriteLine(this.Position.ToString() + " == " + other.Position.ToString() + " :: " + (this.Position == other.Position));
            //Console.WriteLine(this.UV.ToString() + " == " + other.UV.ToString() + " :: " + (this.UV == other.UV));
            //if ((this.Position == other.Position || this.UV == other.UV)) { Console.WriteLine("MATCH!!"); }
            return (this.Position == other.Position && this.UV == other.UV);
        }
    }
}
