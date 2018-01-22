using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class simple_norm_detail_base : MT2
    {
        string normal;
        string normal2;

        public string Normal_Map
        {
            get => normal;
            set => normal = value;
        }

        public string Normal_Map2
        {
            get => normal2;
            set => normal2 = value;
        }

        public simple_norm_detail_base() { }

        public simple_norm_detail_base(XElement xml)
            : base(xml)
        {
            coreDefaults = new simple_norm_detail_base
            {

            };

            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement nor1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement nor2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (nor1 != null) { normal = nor1.Attribute("FileName").Value; }
            if (nor2 != null) { normal2 = nor2.Attribute("FileName").Value; }
        }
    }
}