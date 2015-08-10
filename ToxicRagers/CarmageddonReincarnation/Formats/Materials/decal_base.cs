using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class decal_base : MT2
    {
        // Uses:
        // EmissiveFactor

        Vector3 tileSize;

        public Vector2 TileSize
        {
            get { return new Vector2(tileSize.X, tileSize.Y); }
            set { tileSize.X = value.X; tileSize.Y = value.Y; }
        }

        public decal_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var tile = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "TileSize").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (tile != null) { tileSize = ReadConstant(tile); }
        }
    }
}
