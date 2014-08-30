using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ToxicRagers.CarmageddonReincarnation.Helpers
{
    public class XMLWriter
    {
        public static void Save(XDocument xml, string path)
        {
            int depth = 0;

            using (var sw = new StreamWriter(path))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

                Write(xml.Root, sw, depth);
            }
        }

        private static void Write(XElement element, StreamWriter sw, int depth)
        {
            var singleindent = new string(' ', 3);
            var indent = new string(' ', depth * 3);

            sw.Write("{0}<{1}", indent, element.Name);
            foreach (var attribute in element.Attributes()) { sw.Write(" {0}=\"{1}\"", attribute.Name, attribute.Value); }
            sw.WriteLine(">");

            var cdata = (element.Nodes().FirstOrDefault(n => n.NodeType == XmlNodeType.CDATA) as XCData);
            if (cdata != null)
            {
                sw.WriteLine("{0}<![CDATA[", singleindent + indent);
                foreach (var centry in cdata.Value.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    sw.WriteLine("{0}{1}", singleindent + singleindent + indent, centry);
                }
                sw.WriteLine("{0}]]>", singleindent + indent);
            }

            foreach (var child in element.Elements())
            {
                Write(child, sw, depth + 1);
            }

            sw.WriteLine("{0}</{1}>", indent, element.Name);
        }
    }
}
