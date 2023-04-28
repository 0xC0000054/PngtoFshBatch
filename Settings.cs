﻿/*
* This file is part of PngtoFshBatch, a tool for batch converting images
* to FSH.
*
* Copyright (C) 2009-2017, 2023 Nicholas Hayes
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*/

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
