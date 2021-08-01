using System;
using System.IO;
using System.Text;
using System.Xml;

namespace GalaxyWare
{
    public static class SettingsWrapper
    {
        public static void InitSettings()
        {
            //if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/GalaxyWare"))
            //{
            //    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/GalaxyWare");
            //}
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/GalaxyWare/Config";
            //if (File.Exists(path))
            //{
            //    return;
            //}
            //using (StreamWriter streamWriter = new StreamWriter(path))
            //{
            //    StringBuilder stringBuilder = new StringBuilder();
            //    using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
            //    {
            //        xmlWriter.WriteStartDocument();
            //        xmlWriter.WriteStartElement("Settings");
            //        xmlWriter.WriteAttributeString("Key", string.Empty);
            //        xmlWriter.WriteAttributeString("Expires", string.Empty);
            //        xmlWriter.WriteAttributeString("UpdateHash", string.Empty);
            //        xmlWriter.WriteEndElement();
            //        xmlWriter.WriteEndDocument();
            //        xmlWriter.Close();
            //    }
            //    streamWriter.Write(Encoder.Encode(stringBuilder.ToString()));
            //    streamWriter.Close();
            //}
        }

        public static string GetValue(SettingsAttribute attribute)
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/GalaxyWare/Config";
            //if (!File.Exists(path))
            //{
            //    SettingsWrapper.InitSettings();
            //}
            //string value;
            //using (StreamReader streamReader = new StreamReader(path))
            //{
            //    value = new XmlDocument
            //    {
            //        InnerXml = Encoder.Encode(streamReader.ReadToEnd())
            //    }.GetElementsByTagName("Settings")[0].Attributes[attribute.ToString()].Value;
            //    streamReader.Close();
            //}
            //return value;
            return "true";
        }

        public static void SetValue(SettingsAttribute attribute, string value)
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/GalaxyWare/Config";
            //if (!File.Exists(path))
            //{
            //    SettingsWrapper.InitSettings();
            //}
            //XmlDocument xmlDocument = new XmlDocument();
            //using (StreamReader streamReader = new StreamReader(path))
            //{
            //    xmlDocument.InnerXml = Encoder.Encode(streamReader.ReadToEnd());
            //    xmlDocument.GetElementsByTagName("Settings")[0].Attributes[attribute.ToString()].Value = value;
            //    streamReader.Close();
            //}
            //using (StreamWriter streamWriter = new StreamWriter(path))
            //{
            //    string value2 = Encoder.Encode(xmlDocument.InnerXml);
            //    streamWriter.Write(value2);
            //    streamWriter.Close();
            //}
        }

        public static void Check()
        {
            //SettingsWrapper.GetValue(SettingsAttribute.Key);
            //SettingsWrapper.GetValue(SettingsAttribute.Expires);
            //SettingsWrapper.GetValue(SettingsAttribute.UpdateHash);
        }

        public static void Erase()
        {
            //File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/GalaxyWare/Config");
            //SettingsWrapper.InitSettings();
        }
    }
}
