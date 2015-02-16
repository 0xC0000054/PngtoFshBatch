using System.Globalization;
using System.Xml;

namespace PngtoFshBatchtxt
{
    internal class Settings
    {
        XmlDocument xmlDocument;
        string documentPath;

        public Settings(string path)
        {
            documentPath = path;
            xmlDocument = new XmlDocument();
            xmlDocument.Load(documentPath);
        }

        public string GetSetting(string xPath, string defaultValue)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null) 
            {
                return xmlNode.InnerText; 
            }
                
            return defaultValue;
        }

        public int GetSetting(string xPath, int defaultValue)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null)
            {
                int value;
                if (int.TryParse(xmlNode.InnerText, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value))
                {
                    return value;
                }
            }

            return defaultValue;
        }

        public void PutSetting(string xPath, string value)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode == null) 
            { 
                xmlNode = CreateMissingNode("settings/" + xPath); 
            }
            xmlNode.InnerText = value;
            xmlDocument.Save(documentPath);
        }

        public void PutSetting(string xPath, int value)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode == null)
            {
                xmlNode = CreateMissingNode("settings/" + xPath);
            }
            xmlNode.InnerText = value.ToString(NumberFormatInfo.InvariantInfo);
            xmlDocument.Save(documentPath);
        }

        private XmlNode CreateMissingNode(string xPath)
        {
            string[] xPathSections = xPath.Split('/');
            string currentXPath = "";
            XmlNode testNode = null;
            XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
            foreach (string xPathSection in xPathSections)
            {
                currentXPath += xPathSection;
                testNode = xmlDocument.SelectSingleNode(currentXPath);
                if (testNode == null)
                {
                    currentNode.InnerXml += "<" + xPathSection + "></" + xPathSection + ">";
                }
                currentNode = xmlDocument.SelectSingleNode(currentXPath);
                currentXPath += "/";
            }
            return currentNode;
        }
    }
}
