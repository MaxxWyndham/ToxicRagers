using System;
using System.Linq;
using System.Xml.Linq;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class MT2: Material
    {
        // We'll cheat for now and just expose the XML;
        // Eventually we should probably parse it into useful objects
        public XElement XML;

        public string DiffuseColour
        {
            get
            {
                // OMGHAX
                var file = from el in XML.Descendants("Texture")
                           where (string)el.Attribute("Alias") == "DiffuseColour"
                           select el.Attribute("FileName").Value;

                if (file.Any())
                {
                    return file.First();
                }
                else
                {
                    if (XML.Descendants("DoubleSided").Where(e => e.Attribute("Value").Value == "TRUE").Any())
                    {
                        file = from el in XML.Descendants("Texture")
                               where (string)el.Attribute("Alias") == "Side1_DiffuseColour2"
                               select el.Attribute("FileName").Value;

                        return file.First();
                    }

                    file = from el in XML.Descendants("Texture")
                            where (string)el.Attribute("Alias") == "Decals"
                            select el.Attribute("FileName").Value;

                    if (file.Any()) { return file.First(); }
                }

                return null;
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