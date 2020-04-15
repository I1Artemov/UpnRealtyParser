using System;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Service
{
    public class Program
    {
        public static void WriteDebugLog(string text)
        {
            string dateStr = DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
            Console.WriteLine(string.Format("[{0}] {1}", dateStr, text));
        }

        static void Main(string[] args)
        {
            AppSettingsSerializer serializer = new AppSettingsSerializer();
            AppSettings loadedSettings = serializer.GetAppSettingsFromFile(Const.AppSettingsFileName);
            if(loadedSettings == null)
            {
                Console.WriteLine("Не найден файл настройки приложения");
                return;
            }

            UpnSiteAgent upnAgent = new UpnSiteAgent(WriteDebugLog, loadedSettings);

            //Console.WriteLine("Начат сбор ссылок");
            //upnAgent.StartLinksGatheringInSeparateThread();
            Console.WriteLine("Начат сбор квартир");
            upnAgent.StartApartmentGatheringInSeparateThread();
            Console.ReadLine();
        }
    }
}
