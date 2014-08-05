using System;
using System.Xml;

namespace ToxicRagers.CarmageddonReincarnation.Helpers
{
    public class XMLParser : IDisposable
    {
        XmlDocument xmlDoc;
        string rootNode;

        public XMLParser(string path, string rootNode)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            this.rootNode = rootNode;
        }

        public XmlNode GetNode(string name)
        {
            return xmlDoc[rootNode].GetElementsByTagName(name)[0];
        }

        public void Dispose()
        {
            xmlDoc = null;
        }
    }
}
