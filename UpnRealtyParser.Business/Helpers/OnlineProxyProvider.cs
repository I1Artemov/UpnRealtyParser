using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class OnlineProxyProvider
    {
        public const string ProxyListUrl = "https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-raw.txt";
        public const string ProxyStatusesListUrl = "https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-status.txt";

        protected Action<string> _writeToLogDelegate;
        protected EFGenericRepo<WebProxyInfo, RealtyParserContext> _proxyRepo;

        public OnlineProxyProvider(RealtyParserContext dbContext, Action<string> writeToLogDelegate)
        {
            _writeToLogDelegate = writeToLogDelegate;
            if(dbContext != null)
                _proxyRepo = new EFGenericRepo<WebProxyInfo, RealtyParserContext>(dbContext);
        }

        /// <summary>
        /// Получает с гитхаба список всех прокси, затем - список статусов (живая или нет).
        /// Возвращает только живые прокси
        /// </summary>
        public List<WebProxyInfo> GetAliveProxiesList(StateLogger stateLogger = null)
        {
            List<string> rawProxies = getAllProxiesRaw(stateLogger);
            if(_writeToLogDelegate != null)
                _writeToLogDelegate(string.Format("Загружен список из {0} прокси с Github", rawProxies.Count));

            List<string> aliveProxyIps = getSuccessfullProxyIpsList(stateLogger);
            if (_writeToLogDelegate != null)
                _writeToLogDelegate(string.Format("Загружен список из {0} живых прокси", aliveProxyIps.Count));

            List<string> aliveProxiesWithPorts = rawProxies.Where(
                x => aliveProxyIps.Contains(x.Split(':').ToList().FirstOrDefault()))
                .ToList();

            List<WebProxyInfo> webProxies = GetProxiesFromIps(rawProxies); // TODO: Второй файл игнорируется!
            insertOrUpdateProxiesInfoInDb(webProxies);
            return webProxies;
        }

        protected List<string> getAllProxiesRaw(StateLogger stateLogger)
        {
            string contents;
            List<string> proxyStrs = new List<string>();
            using (var wc = new System.Net.WebClient())
            {
                try { 
                    contents = wc.DownloadString(ProxyListUrl);
                    proxyStrs = contents.Split('\n').ToList();
                }
                catch (Exception ex)
                {
                    _writeToLogDelegate("Не удалось загрузить новые прокси, продолжение со старым списком. Ошибка: " + ex.Message);
                    if (stateLogger != null)
                        stateLogger.LogProxiesDownloadError(Const.SiteNameUpn, ex.Message);
                }
            }
            return proxyStrs;
        }

        protected List<string> getSuccessfullProxyIpsList(StateLogger stateLogger)
        {
            string contents;
            List<string> proxyStatusStrs = new List<string>();
            List<string> aliveProxyIps = new List<string>();
            using (var wc = new System.Net.WebClient())
            {
                try { 
                    contents = wc.DownloadString(ProxyStatusesListUrl);
                    proxyStatusStrs = contents.Split('\n').ToList();
                }
                catch (Exception ex)
                {
                    _writeToLogDelegate("Не удалось загрузить новые статусы прокси. Ошибка: " + ex.Message);
                    if (stateLogger != null)
                        stateLogger.LogProxiesDownloadError(Const.SiteNameUpn, ex.Message);
                }

                foreach (string proxyStatusStr in proxyStatusStrs)
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
        public List<WebProxyInfo> GetProxiesFromIps(List<string> proxyStrList)
        {
            List<WebProxyInfo> proxyInfoList = new List<WebProxyInfo>();

            if (proxyStrList == null || proxyStrList.Count == 0)
                return proxyInfoList;

            foreach (string proxyStr in proxyStrList)
            {
                List<string> separatedStrs = proxyStr.Split(':').ToList();
                if (separatedStrs.Count < 2)
                    continue;

                int port = Int32.Parse(separatedStrs[1]);

                WebProxyInfo currentProxyInfo = new WebProxyInfo(new WebProxy(separatedStrs[0], port));
                currentProxyInfo.WebProxy.BypassProxyOnLocal = false;
                proxyInfoList.Add(currentProxyInfo);
            }

            if(_writeToLogDelegate != null)
                _writeToLogDelegate("Инициализация прокси-серверов завершена");

            return proxyInfoList;
        }

        /// <summary>
        /// Находит прокси, соответствующую используемой, в БД и увеличивает у нее счетчик неудачных коннектов
        /// </summary>
        public void AddFailureAmountToProxyInDb(WebProxyInfo usedProxy)
        {
            if (usedProxy == null)
                return;

            WebProxyInfo proxyFromDb = _proxyRepo.GetAll().FirstOrDefault(x => x.Ip == usedProxy.Ip && x.Port == usedProxy.Port);
            if (proxyFromDb == null)
                return;

            proxyFromDb.LastUseDateTime = DateTime.Now;
            if (proxyFromDb.FailureAmount == null)
                proxyFromDb.FailureAmount = 0;
            proxyFromDb.FailureAmount++;

            try
            {
                _proxyRepo.Update(proxyFromDb);
                _proxyRepo.Save();
            }
            catch (Exception ex)
            {
                _writeToLogDelegate(string.Format(
                    "Ошибка обновления числа неудачных коннектов для прокси {0}: {1}", proxyFromDb.Ip, ex.Message));
            }
        }

        public WebProxyInfo GetRandomWebProxy(Random random)
        {
            List<WebProxyInfo> notCheckedProxies = _proxyRepo
                .GetAllWithoutTracking()
                .Where(x => x.LastUseDateTime == null)
                .ToList();

            List<WebProxyInfo> workingProxies = _proxyRepo.GetAllWithoutTracking()
                .Where(x => x.LastSuccessDateTime != null)
                .ToList();

            // Фильтруем по дате последнего успешного подключения уже не в БД (иначе - ошибка)
            workingProxies = workingProxies
                .Where(x => (x.LastSuccessDateTime - DateTime.Now).Value.Days < 31
                    && x.SuccessRate >= 0.32d)
                .ToList();
            if(workingProxies.Count == 0)
                workingProxies = _proxyRepo.GetAllWithoutTracking()
                    .Where(x => x.LastSuccessDateTime != null)
                    .Where(x => (x.LastSuccessDateTime - DateTime.Now).Value.Days < 31
                        && x.SuccessRate >= 0.20d)
                    .ToList();

            List<WebProxyInfo> targetProxylist = notCheckedProxies == null || notCheckedProxies.Count == 0 ?
                workingProxies : notCheckedProxies;

            int count = targetProxylist.Count;
            int randomIndex = random.Next(0, count - 1);
            var chosenProxy = targetProxylist[randomIndex];
            chosenProxy.InitializeWebProxy();
            return chosenProxy;
        }

        /// <summary>
        /// Находит прокси, соответствующую используемой, в БД и увеличивает у нее счетчик успешных коннектов
        /// </summary>
        public void AddSuccessAmountToProxyInDb(WebProxyInfo usedProxy)
        {
            if (usedProxy == null)
                return;

            WebProxyInfo proxyFromDb = _proxyRepo.GetAll().FirstOrDefault(x => x.Ip == usedProxy.Ip && x.Port == usedProxy.Port);
            if (proxyFromDb == null)
                return;

            proxyFromDb.LastUseDateTime = DateTime.Now;
            proxyFromDb.LastSuccessDateTime = DateTime.Now;
            if (proxyFromDb.SuccessAmount == null)
                proxyFromDb.SuccessAmount = 0;
            proxyFromDb.SuccessAmount++;

            try { 
                _proxyRepo.Update(proxyFromDb);
                _proxyRepo.Save();
            }
            catch (Exception ex)
            {
                _writeToLogDelegate(string.Format(
                    "Ошибка обновления числа успешных коннектов для прокси {0}: {1}", proxyFromDb.Ip, ex.Message));
            }
        }

        protected bool insertOrUpdateProxiesInfoInDb(List<WebProxyInfo> proxyInfoList)
        {
            if (_proxyRepo == null)
                return false;

            foreach(WebProxyInfo webProxyInfo in proxyInfoList)
            {
                webProxyInfo.FillFieldsFromWebInfo();

                var foundProxy = _proxyRepo
                    .GetWithoutTracking(x => x.Ip == webProxyInfo.Ip && x.Port == webProxyInfo.Port);
                if (foundProxy == null)
                    _proxyRepo.Add(webProxyInfo);
            }

            _proxyRepo.Save();
            return true;
        }
    }
}
