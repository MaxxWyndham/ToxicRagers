using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ToxicRagers.CarmageddonMobile.Formats
{
	public class TextXml
	{
		public enum TextLanguage : int
		{
			Comment = 1,
			Master,
			French,
			Spanish,
			German,
			Italian,
			Japanese,
			Russian,
			PortugueseBrazil,
			Turkish,
			ChineseSimplified,
			Polish
		}

		private XmlDocument textXml;

		public TextXml(string filename)
		{
			textXml = new XmlDocument();
			textXml.Load(filename);
		}

		public void Save(string filename)
		{
			textXml.Save(filename);
		}
		public bool KeyExists(string key)
		{
			var rows = textXml.GetElementsByTagName("Row");
			
			if(rows == null || rows.Count < 1)
			{
				return false;
			}

			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].FirstChild.FirstChild.InnerText.ToUpper() == key.ToUpper())
				{
					return true;
				}
			}

			return false;
		}

		public string GetText(string key)
		{
			return GetText(key, TextLanguage.Master);
		}
		public string GetText(string key, TextLanguage lang)
		{
			var rows = textXml.GetElementsByTagName("Row");
			
			if(rows == null || rows.Count < 1)
			{
				return null;
			}

			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].FirstChild.FirstChild.InnerText.ToUpper() == key.ToUpper())
				{
					return rows[i].ChildNodes[(int)lang].FirstChild.InnerText;
				}
			}

			return null;
		}
		public void SetText(string key, string value, TextLanguage lang = TextLanguage.Master)
		{
			var rows = textXml.GetElementsByTagName("Row");
			
			if(rows == null || rows.Count < 1)
			{
				return;
			}

			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].FirstChild.FirstChild.InnerText == key)
				{
					rows[i].ChildNodes[(int)lang].FirstChild.InnerText = value;
				}
			}
		}
		public void SetAllText(string key, string value, TextLanguage lang = TextLanguage.Master)
		{
			var rows = textXml.GetElementsByTagName("Row");
			
			if(rows == null || rows.Count < 1)
			{
				return;
			}

			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].FirstChild.FirstChild.InnerText == key)
				{
					for (int l = 2; l < rows[i].ChildNodes.Count; l++) {
						rows[i].ChildNodes[l].FirstChild.InnerText = value;
					}
				}
			}
		}
	}
}
