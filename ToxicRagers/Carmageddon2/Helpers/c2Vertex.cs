using System;

using ToxicRagers.Helpers;

namespace ToxicRagers.Carmageddon2.Helpers
{
    class C2Vertex : IEquatable<C2Vertex>
    {
        public Vector3 Position { get; set; }

        public Vector2 UV { get; set; }

        public C2Vertex(Vector3 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }

        public override string ToString()
        {
            return $"{Position} :: {UV}";
        }

        public bool Equals(C2Vertex other)
        {
            return Position == other.Position && UV == other.UV;
        }
    }
}