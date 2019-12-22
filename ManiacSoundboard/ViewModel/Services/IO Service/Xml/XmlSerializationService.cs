using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Global application's service of xml serialization.
    /// </summary>
    public static class XmlSerializationService
    {

        static XmlSerializationService()
        {
        }

        /// <summary>
        /// Serializes specified object. When succeeded file is saved in the current directory or in specified path. 
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize.</typeparam>
        /// <param name="toSerialize">Object to be serialized.</param>
        /// <param name="fileName">Name of the xml file where object will be serialized or full path of the mentioned file.</param>
        /// <param name="isFileNameFullPath">Indicates if filename parameter is a name of the file or a full path. The default value of it is false.</param>
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

        /// <summary>
        /// Deserializes specified object.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize.</typeparam>
        /// <param name="toSerialize">Object to be deserialized.</param>
        /// <param name="fileName">Name of the xml file where object will be deserialized from or full path of the mentioned file.</param>
        /// <param name="isFileNameFullPath">Indicates if filename parameter is a name of the file or a full path. The default value of it is false.</param>
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
