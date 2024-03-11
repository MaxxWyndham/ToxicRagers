using ToxicRagers.TDR2000.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class Hierarchy
    {
        public string FileName { get; set; }

        public static Hierarchy Load(string path)
        {
            DocumentParser file = new(path);
            Hierarchy hierarchy = new()
            {
                FileName = file.ReadString()
            };

            return hierarchy;
        }
    }
}
