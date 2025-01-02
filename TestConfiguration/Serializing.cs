using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.TestConfiguration {

    public static class Serializing {

        public static T DeserializeFromFile<T>(String xmlFile) {
            if (!File.Exists(xmlFile)) throw new ArgumentException($"XML File '{xmlFile}' does not exist.");
            T t;
            using (FileStream fileStream = new FileStream(xmlFile, FileMode.Open)) {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileStream);
                XmlNamespaceManager XNM = new XmlNamespaceManager(xmlDoc.NameTable);
                XNM.AddNamespace("default", xmlDoc.DocumentElement.NamespaceURI);
                XmlNode xmlNode = xmlDoc.SelectSingleNode($"//default:{typeof(T).Name}", XNM) ?? throw new InvalidOperationException($"Element '{typeof(T).Name}' not found in XML file '{xmlFile}'.");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringReader stringReader = new StringReader(xmlNode.OuterXml)) { t = (T)xmlSerializer.Deserialize(stringReader); }
            }
            return t;
        }

        public static void SerializeToFile<T>(T t, String xmlFile, FileMode fileMode) {
            using (FileStream fileStream = new FileStream(xmlFile, fileMode)) {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(fileStream, t);
            }
        }

        public static String SerializeToString<T>(T t) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter stringWriter = new StringWriter()) {
                xmlSerializer.Serialize(stringWriter, t);
                return stringWriter.ToString();
            }
        }
    }
}
