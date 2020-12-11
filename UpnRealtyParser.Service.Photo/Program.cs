using System;
using System.Threading;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Service.Photo
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
            PhotoDownloader photoLoader = 
                new PhotoDownloader(loadedSettings.IsUseProxies, loadedSettings.RequestDelayInMs, WriteDebugLog);

            // Начинаем со сбора ссылок, на сбор квартир переключит watchdog
            Console.WriteLine("Начат сбор фото с UPN");
            photoLoader.OpenConnection();
            photoLoader.StartUpnPhotosGatheringInSeparateThread();

            Thread.Sleep(700000);
            int previouslyProcessedAmount = 0;

            while (true)
            {
                int currentlyProcessedAmount = photoLoader.GetProcessedRecordsAmount();
                WriteDebugLog(string.Format("Проверка состояния. Обработано {0} записей.", currentlyProcessedAmount));

                if (currentlyProcessedAmount == previouslyProcessedAmount)
                {
                    // Если флаг завершения сбора не выставлен
                    if (!photoLoader.CheckIfProcessingCompleted())
                    {
                        WriteDebugLog("Возможно, поток завис. Перезапуск отключен!");
                    }
                    else
                    {
                        WriteDebugLog("Обработка полностью завершена. Остановка цикла.");
                        break;
                    }
                }

                previouslyProcessedAmount = currentlyProcessedAmount;
                Thread.Sleep(300000);
            }

            photoLoader.CloseConnection();
        }
    }
}
