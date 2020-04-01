using System;
using System.IO;
using System.Xml.Serialization;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class AppSettingsSerializer
    {
        private string ConfigFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\..\\";

        public void SaveSettingsToFile(AppSettings appSettings, string fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
            TextWriter tw = new StreamWriter(ConfigFolderPath + fileName);
            xs.Serialize(tw, appSettings);
        }

        public AppSettings GetAppSettingsFromFile(string fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
            AppSettings appSettings = new AppSettings();
            using (var sr = new StreamReader(ConfigFolderPath + fileName))
            {
                appSettings = (AppSettings)xs.Deserialize(sr);
            }
            return appSettings;
        }
    }
}
