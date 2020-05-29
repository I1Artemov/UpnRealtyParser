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

            ThreadStart threadMethod = delegate { watchdogProcessingAndLogging(loadedSettings); };
            Thread watchdogThread = new Thread(threadMethod);
            watchdogThread.IsBackground = true;
            watchdogThread.Start();

            Console.ReadLine();
        }

        protected static void watchdogProcessingAndLogging(AppSettings loadedSettings)
        {
            UpnSiteAgent upnAgent = new UpnSiteAgent(WriteDebugLog, loadedSettings);

            // Начинаем со сбора ссылок, на сбор квартир переключит watchdog
            Console.WriteLine("Начат сбор ссылок");
            upnAgent.OpenConnection();
            upnAgent.StartLinksGatheringInSeparateThread();

            Thread.Sleep(700000);
            int previouslyProcessedAmount = 0;

            while (true)
            {
                int currentlyProcessedAmount = upnAgent.GetProcessedRecordsAmount();
                WriteDebugLog(string.Format("Проверка состояния. Обработано {0} записей.", currentlyProcessedAmount));

                if(currentlyProcessedAmount == previouslyProcessedAmount)
                {
                    if(upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionGatheringLinks)
                    {
                        // Если завершился сбор ссылок, то начинаем сбор квартир
                        WriteDebugLog("Переключение на сбор квартир.");
                        previouslyProcessedAmount = 0;

                        upnAgent.CloseConnection();
                        upnAgent = new UpnSiteAgent(WriteDebugLog, loadedSettings);
                        upnAgent.OpenConnection();
                        upnAgent.StartApartmentGatheringInSeparateThread();
                    }
                    else if(upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionObservingFlat)
                    {
                        // Если завершился сбор квартир, то выходим из цикла
                        WriteDebugLog("Обработка полностью завершена. Остановка цикла.");
                        break;
                    }
                    else
                    { 
                        // Если флаг завершения сбора не выставлен, то в любом случае перезапускаем нужный тип процессинга
                        if (!upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionGatheringLinks)
                        {
                            WriteDebugLog("Возможно, поток завис.");

                        }

                        if (!upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionObservingFlat)
                        {
                            upnAgent.StartApartmentGatheringInSeparateThread();
                            WriteDebugLog("Поток завис. Перезапуск...");
                        }
                    }
                }

                previouslyProcessedAmount = currentlyProcessedAmount;
                Thread.Sleep(300000);
            }

            upnAgent.CloseConnection();
        }
    }
}
