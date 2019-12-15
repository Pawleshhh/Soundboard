using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ManiacSoundboard.ViewModel
{
    public static class XmlHelper
    {

        public static void CreateXMLDocument(string path)
        {
            File.Create(path).Close();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
        }

        public static T Get<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value cannot be empty or null");

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)(converter.ConvertFromInvariantString(value));
        }
    }
}
