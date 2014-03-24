using System;
using System.Linq;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class MT2
    {
        // We'll cheat for now and just expose the XML;
        // Eventually we should probably parse it into useful objects
        public XElement XML;

        public string DiffuseColour
        {
            get
            {
                var file = from el in XML.Descendants("Texture")
                where (string)el.Attribute("Alias") == "DiffuseColour"
                select el.Attribute("FileName").Value;

                return file.First();
            }
        }

        public static MT2 Load(string Path)
        {
            MT2 mt2 = new MT2();
            mt2.XML = XElement.Load(Path);
            return mt2;
        }
    }
}
