using System;
using System.Threading;
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
            UpnSiteAgent upnAgent = new UpnSiteAgent(WriteDebugLog, null, 20000);

            Console.WriteLine("Начат сбор ссылок");
            upnAgent.StartLinksGatheringInSeparateThread();
            Console.ReadLine();
        }
    }
}
