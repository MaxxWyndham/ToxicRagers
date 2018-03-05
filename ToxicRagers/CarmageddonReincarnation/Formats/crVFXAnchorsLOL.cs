using System.IO;
using System.Linq;

using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class VFXAnchors
    {
        VFXAnchorsScript anchors;

        public VFXAnchors() { }

        public static VFXAnchors Load(string path)
        {
            VFXAnchors vfxAnchors = new VFXAnchors();
            LOL lol = LOL.Load(path);

            vfxAnchors.anchors = VFXAnchorsScript.Parse(lol.Document);

            return vfxAnchors;
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(path, "vfx_anchors.lol")))
            {
                sw.WriteLine(anchors.ToString().Indent());
            }
        }
    }

    public class VFXAnchorsScript : LUAScript
    {
        public VFXAnchorsScript()
        {
            AddProperty(
                "tags",
                LUAScriptPropertyType.Table,
                tableType: typeof(VFXAnchorsGroup),
                isArray: true
            );
        }

        public static VFXAnchorsScript Parse(string script)
        {
            return Parse<VFXAnchorsScript>(script);
        }
    }

    public class VFXAnchorsGroup : LUAScript
    {
        public VFXAnchorsGroup()
        {
            IsTable = true;

            AddProperty(
                "id",
                LUAScriptPropertyType.String
            );

            AddProperty(
                null,
                LUAScriptPropertyType.Table,
                isArray: true,
                tableType: typeof(VFXAnchor)
            );
        }

        public static VFXAnchorsGroup Parse(string script)
        {
            return Parse<VFXAnchorsGroup>(script);
        }
    }

    public class VFXAnchor : LUAScript
    {
        public VFXAnchor()
        {
            IsTable = true;

            AddProperty(
                "id",
                LUAScriptPropertyType.String
            );

            AddProperty(
                "lump_name",
                LUAScriptPropertyType.String
            );

            AddProperty(
                "offset",
                LUAScriptPropertyType.Float,
                true
            );

            AddProperty(
                "angle",
                LUAScriptPropertyType.Float,
                true
            );

            AddProperty(
                "scale",
                LUAScriptPropertyType.Float,
                true
            );
        }

        public static VFXAnchor Parse(string script)
        {
            return Parse<VFXAnchor>(script);
        }
    }
}