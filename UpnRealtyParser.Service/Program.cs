using System;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;

namespace UpnRealtyParser.Service
{
    public class Program
    {
        public static void WriteDebugLog(string text)
        {
            string dateStr = DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
            Console.WriteLine(string.Format("[{0}] {1}\r\n", dateStr, text));
        }

        static void Main(string[] args)
        {
            using (RealtyParserContext dbContext = new RealtyParserContext()) {
                UpnSiteAgent upnAgent = new UpnSiteAgent(WriteDebugLog);
                upnAgent.InitializeRepositories(dbContext);

                Console.WriteLine("Начат сбор ссылок");
                upnAgent.GatherLinksAndInsertInDb();
            }
        }
    }
}
