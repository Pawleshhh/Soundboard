using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ManiacSoundboard.ViewModel
{
    public static class XmlSerializationService
    {

        static XmlSerializationService()
        {
        }

        public static void Serialize<T>(T toSerialize, string fileName, bool isFileNameFullPath = false)
        {
            string fullPath;
            if (!isFileNameFullPath)
                fullPath = Environment.CurrentDirectory + "\\" + fileName;
            else
                fullPath = fileName;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (Stream s = File.Create(fullPath))
                xmlSerializer.Serialize(s, toSerialize);
        }

        public static T Deserialize<T>(string fileName, bool isFileNameFullPath = false)
        {
            string fullPath;
            if (!isFileNameFullPath)
                fullPath = Environment.CurrentDirectory + "\\" + fileName;
            else
                fullPath = fileName;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            T deserialized;

            using (XmlReader r = XmlReader.Create(fullPath, settings))
                deserialized = (T)xmlSerializer.Deserialize(r);

            return deserialized;
        }

        public static Dictionary<string, string> ReadDataFromFile(string filename, string configureElement, bool fileNameIsFullPath = false)
        {
            string filePath;
            if (!fileNameIsFullPath)
                filePath = Environment.CurrentDirectory + "\\" + filename;
            else
                filePath = filename;

            if (!File.Exists(filePath))
                return new Dictionary<string, string>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            Dictionary<string, string> result = new Dictionary<string, string>();

            using (XmlReader reader = XmlReader.Create(filePath, settings))
            {
                string elementName = configureElement;
                reader.ReadStartElement(elementName);

                string currentElement = string.Empty;
                do
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        currentElement = reader.LocalName;
                        result.Add(currentElement, string.Empty);
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                        result[currentElement] = reader.Value;
                    else if (reader.LocalName == elementName && reader.NodeType == XmlNodeType.EndElement)
                        break;
                } while (reader.Read());
            }

            return result;
        }

        public static Dictionary<string, string> ReadDataFromXml(string xmlText, string configureElement)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            Dictionary<string, string> result = new Dictionary<string, string>();

            ASCIIEncoding myEncoder = new ASCIIEncoding();
            byte[] bytes = myEncoder.GetBytes(xmlText);

            using (MemoryStream ms = new MemoryStream(bytes))
            using (XmlReader reader = XmlReader.Create(ms, settings))
            {
                string elementName = configureElement;
                reader.ReadStartElement(elementName);

                string currentElement = string.Empty;
                do
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        currentElement = reader.LocalName;
                        result.Add(currentElement, string.Empty);
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                        result[currentElement] = reader.Value;
                    else if (reader.LocalName == elementName && reader.NodeType == XmlNodeType.EndElement)
                        break;
                } while (reader.Read());
            }

            return result;
        }

        public static void WriteData(IXmlSerialization xmlSettings, string filename, string configureElement, bool filenameIsFullPath = false)
        {
            var dict = xmlSettings.LoadData();
            string filePath;
            if (!filenameIsFullPath)
                filePath = Environment.CurrentDirectory + "\\" + filename;
            else
                filePath = filename;

            if (!File.Exists(filePath))
                XmlHelper.CreateXMLDocument(filePath);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                writer.WriteStartElement("Settings");
                foreach (var pair in dict)
                    writer.WriteElementString(pair.Key, pair.Value);
                writer.WriteEndElement();
            }
        }


    }
}
