using ToxicRagers.TDR2000.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class MovableObjects : List<MovableObjectDefinition>
    {
        public static MovableObjects Load(string path)
        {
            DocumentParser file = new(path);
            MovableObjects movableObjects = new();

            do
            {
                movableObjects.Add(file.Read<MovableObjectDefinition>());
            } while (!file.EOF);

            return movableObjects;
        }
    }

    public class MovableObjectDefinition
    {
        public string MovableObject { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}
