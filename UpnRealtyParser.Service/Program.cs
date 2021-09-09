using System;
using System.Threading;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

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

            upnAgent.OpenConnection();
            upnAgent.StartWorkingFromMemorizedStage();
            Console.WriteLine("Начало работы с предыдущей стадии: " + upnAgent.GetCurrentActionName());

            Thread.Sleep(700000);
            int previouslyProcessedAmount = 0;

            while (true)
            {
                int currentlyProcessedAmount = upnAgent.GetProcessedRecordsAmount();

                if(currentlyProcessedAmount == previouslyProcessedAmount)
                {
                    if (upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionGatheringLinks)
                    {
                        // Если завершился сбор ссылок, то начинаем сбор квартир
                        WriteDebugLog("Переключение на сбор квартир на продажу.");
                        previouslyProcessedAmount = 0;

                        upnAgent.CloseConnection();
                        upnAgent = new UpnSiteAgent(WriteDebugLog, loadedSettings);
                        upnAgent.OpenConnection();
                        upnAgent.StartApartmentGatheringInSeparateThread();
                    }
                    else if (upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionObservingFlats)
                    {
                        WriteDebugLog("Переключение на сбор ссылок по аренде.");
                        previouslyProcessedAmount = 0;

                        upnAgent.CloseConnection();
                        upnAgent = new UpnSiteAgent(WriteDebugLog, loadedSettings);
                        upnAgent.OpenConnection();
                        upnAgent.StartLinksGatheringInSeparateThread(true);
                    }
                    else if (upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionGatheringLinksRent)
                    {
                        WriteDebugLog("Переключение на сбор арендных квартир.");
                        previouslyProcessedAmount = 0;

                        upnAgent.CloseConnection();
                        upnAgent = new UpnSiteAgent(WriteDebugLog, loadedSettings);
                        upnAgent.OpenConnection();
                        upnAgent.StartApartmentGatheringInSeparateThread(true);
                    }
                    else if (upnAgent.CheckIfProcessingCompleted() && upnAgent.GetCurrentActionName() == Const.ParsingStatusDescriptionObservingFlatsRent)
                    {
                        WriteDebugLog("Подсчет новой статистики по домам.");
                        upnAgent.CalculateHousesStatistics();
                        WriteDebugLog("Поиск похожих домов с разных сайтов");
                        upnAgent.CloseConnection();
                        using (var realtyContext = new RealtyParserContext())
                        {
                            DistanceCalculator calculator = new DistanceCalculator(realtyContext);
                            calculator.FindSimilarN1ForAllUpnHouses();
                        }
                        WriteDebugLog("Подсчет окупаемости квартир");
                        calculateFlatPaybackPeriods();
                        // Если завершился сбор арендных квартир, то выходим из цикла
                        WriteDebugLog("Обработка полностью завершена. Остановка цикла.");
                        break;
                    }
                    else
                    {
                        // Если флаг завершения сбора не выставлен, то в любом случае перезапускаем нужный тип процессинга
                        if (!upnAgent.CheckIfProcessingCompleted())
                        {
                            WriteDebugLog("Возможно, поток завис. Перезапуск отключен!");
                        }
                    }
                }

                previouslyProcessedAmount = currentlyProcessedAmount;
                Thread.Sleep(300000);
            }

            upnAgent.CloseConnection();
        }

        private static void calculateFlatPaybackPeriods()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo =
                    new EFGenericRepo<UpnFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo =
                    new EFGenericRepo<UpnRentFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<N1RentFlat, RealtyParserContext> n1RentFlatRepo =
                    new EFGenericRepo<N1RentFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<SimilarHouse, RealtyParserContext> similarHouseRepo =
                    new EFGenericRepo<SimilarHouse, RealtyParserContext>(realtyContext);
                EFGenericRepo<ApartmentPayback, RealtyParserContext> apartmentPaybackRepo =
                    new EFGenericRepo<ApartmentPayback, RealtyParserContext>(realtyContext);

                PaybackApartmentCalculator<UpnFlat> calculator = new PaybackApartmentCalculator<UpnFlat>(Const.SiteNameUpn,
                    upnFlatRepo, upnRentFlatRepo, n1RentFlatRepo, similarHouseRepo, apartmentPaybackRepo, WriteDebugLog);

                calculator.CalculateAllUpnPaybackPeriods().Wait();
            }
        }
    }
}
