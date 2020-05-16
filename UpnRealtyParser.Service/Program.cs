using System;
using System.Threading;
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

            ThreadStart threadMethod = delegate { watchdogProcessingAndLogging(upnAgent); };
            Thread watchdogThread = new Thread(threadMethod);
            watchdogThread.IsBackground = true;
            watchdogThread.Start();

            Console.ReadLine();
        }

        protected static void watchdogProcessingAndLogging(UpnSiteAgent upnAgent)
        {
            Thread.Sleep(800000);
            int previouslyProcessedAmount = 0;

            while (true)
            {
                int currentlyProcessedAmount = upnAgent.GetProcessedRecordsAmount();
                WriteDebugLog(string.Format("Проверка состояния. Обработано {0} записей.", currentlyProcessedAmount));
                if(currentlyProcessedAmount == previouslyProcessedAmount)
                {
                    WriteDebugLog("Поток завис. Перезапуск...");
                    //upnAgent.StartLinksGatheringInSeparateThread();
                    upnAgent.StartApartmentGatheringInSeparateThread();
                }
                previouslyProcessedAmount = currentlyProcessedAmount;
                Thread.Sleep(800000);
            }
        }
    }
}
