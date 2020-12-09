using System;
using System.Threading;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Service.N1
{
    class Program
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
            if (loadedSettings == null)
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
            N1SiteAgent n1Agent = new N1SiteAgent(WriteDebugLog, loadedSettings);

            // Начинаем со сбора ссылок, на сбор квартир переключит watchdog
            Console.WriteLine("Начат сбор ссылок на квартиры на продажу");
            n1Agent.OpenConnection();
            n1Agent.StartLinksGatheringInSeparateThread();

            Thread.Sleep(700000);
            int previouslyProcessedAmount = 0;

            while (true)
            {
                //int currentlyProcessedAmount = upnAgent.GetProcessedRecordsAmount();
                int currentlyProcessedAmount = n1Agent.GetProcessedRecordsAmount();
                WriteDebugLog(string.Format("Проверка состояния. Обработано {0} записей.", currentlyProcessedAmount));

                if(currentlyProcessedAmount == previouslyProcessedAmount)
                {
                    if (n1Agent.CheckIfProcessingCompleted() && n1Agent.GetCurrentActionName() == Const.ParsingStatusDescriptionGatheringLinks)
                    {
                        // Если завершился сбор ссылок, то начинаем сбор квартир
                        WriteDebugLog("Переключение на сбор квартир на продажу.");
                        previouslyProcessedAmount = 0;

                        n1Agent.CloseConnection();
                        n1Agent = new N1SiteAgent(WriteDebugLog, loadedSettings);
                        n1Agent.OpenConnection();
                        n1Agent.StartApartmentGatheringInSeparateThread();
                    }
                    else if(n1Agent.CheckIfProcessingCompleted() && n1Agent.GetCurrentActionName() == Const.ParsingStatusDescriptionObservingFlats)
                    {
                        WriteDebugLog("Обработка полностью завершена. Остановка цикла.");
                        break;
                    }
                    else
                    {
                        // Если флаг завершения сбора не выставлен
                        if (!n1Agent.CheckIfProcessingCompleted())
                        {
                            WriteDebugLog("Возможно, поток завис. Перезапуск отключен!");
                        }
                    }
                }

                previouslyProcessedAmount = currentlyProcessedAmount;
                Thread.Sleep(300000);
            }

            n1Agent.CloseConnection();
        }
    }
}
