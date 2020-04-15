using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UpnRealtyParser.Business.Helpers
{
    public class OnlineProxyProvider
    {
        public const string ProxyListUrl = "https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-raw.txt";
        public const string ProxyStatusesListUrl = "https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-status.txt";

        protected Action<string> _writeToLogDelegate;

        public OnlineProxyProvider(Action<string> writeToLogDelegate)
        {
            _writeToLogDelegate = writeToLogDelegate;
        }

        /// <summary>
        /// Получает с гитхаба список всех прокси, затем - список статусов (живая или нет).
        /// Возвращает только живые прокси
        /// </summary>
        public List<WebProxy> GetAliveProxiesList()
        {
            List<string> rawProxies = getAllProxiesRaw();
            if(_writeToLogDelegate != null)
                _writeToLogDelegate(string.Format("Загружен список из {0} прокси с Github", rawProxies.Count));

            List<string> aliveProxyIps = getSuccessfullProxyIpsList();
            if (_writeToLogDelegate != null)
                _writeToLogDelegate(string.Format("Загружен список из {0} живых прокси", aliveProxyIps.Count));

            List<string> aliveProxiesWithPorts = rawProxies.Where(
                x => aliveProxyIps.Contains(x.Split(':').ToList().FirstOrDefault()))
                .ToList();

            List<WebProxy> webProxies = GetProxiesFromIps(aliveProxiesWithPorts);
            return webProxies;
        }

        protected List<string> getAllProxiesRaw()
        {
            string contents;
            List<string> proxyStrs = new List<string>();
            using (var wc = new System.Net.WebClient())
            {
                contents = wc.DownloadString(ProxyListUrl);
                proxyStrs = contents.Split('\n').ToList();
            }
            return proxyStrs;
        }

        protected List<string> getSuccessfullProxyIpsList()
        {
            string contents;
            List<string> proxyStatusStrs = new List<string>();
            List<string> aliveProxyIps = new List<string>();
            using (var wc = new System.Net.WebClient())
            {
                contents = wc.DownloadString(ProxyStatusesListUrl);
                proxyStatusStrs = contents.Split('\n').ToList();

                foreach(string proxyStatusStr in proxyStatusStrs)
                {
                    if (!proxyStatusStr.Contains("success"))
                        continue;

                    string aliveProxyIp = proxyStatusStr.Split(':').ToList().FirstOrDefault();
                    aliveProxyIps.Add(aliveProxyIp);
                }
            }

            return aliveProxyIps;
        }

        /// <summary>
        /// Превращает список адресов прокси формата "(IP):(порт)" в объекты WebProxy
        /// </summary>
        public List<WebProxy> GetProxiesFromIps(List<string> proxyStrList)
        {
            List<WebProxy> proxyList = new List<WebProxy>();

            if (proxyStrList == null || proxyStrList.Count == 0)
                return proxyList;

            foreach (string proxyStr in proxyStrList)
            {
                List<string> separatedStrs = proxyStr.Split(':').ToList();
                int port = Int32.Parse(separatedStrs[1]);

                WebProxy currentProxy = new WebProxy(separatedStrs[0], port);
                currentProxy.BypassProxyOnLocal = false;
                proxyList.Add(currentProxy);
            }

            if(_writeToLogDelegate != null)
                _writeToLogDelegate("Инициализация прокси-серверов завершена");

            return proxyList;
        }
    }
}
