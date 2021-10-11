using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class AnyRealtySiteParser : BaseSiteAgent, IDisposable
    {
        protected Thread _linksProcessingThread;
        protected Thread _apartmentProcessingThread;

        protected EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        protected EFGenericRepo<ServiceStage, RealtyParserContext> _serviceStageRepo;

        protected StateLogger _stateLogger;

        protected int _maxRetryAmountForSingleRequest;
        protected int _maxRequestTimeoutInMs;
        protected int _tablePagesToSkip;
        protected string _siteName;

        public AnyRealtySiteParser(Action<string> writeToLogDelegate, AppSettings settings)
            : base(settings.IsUseProxies, settings.RequestDelayInMs, writeToLogDelegate)
        {
            _isUseProxy = settings.IsUseProxies;
            _maxRetryAmountForSingleRequest = settings.MaxRetryAmountForSingleRequest;
            _maxRequestTimeoutInMs = settings.MaxRequestTimeoutInMs;

            // Берем прокси либо из списка, либо из сети (если они нужны)
            if (settings.IsUseProxies && !settings.IsGetProxiesListFromGithub && settings.ProxyList != null && settings.ProxyList.Count != 0)
            {
                using (var realtyContext = new RealtyParserContext())
                {
                    OnlineProxyProvider proxyProvider = new OnlineProxyProvider(realtyContext, writeToLogDelegate);
                    proxyProvider.GetProxiesFromIps(settings.ProxyList);
                }
            }
            if (settings.IsUseProxies && settings.IsGetProxiesListFromGithub && settings.IsUpdateProxiesBeforeRun)
            {
                using (var realtyContext = new RealtyParserContext())
                {
                    OnlineProxyProvider proxyProvider = new OnlineProxyProvider(realtyContext, writeToLogDelegate);
                    proxyProvider.GetAliveProxiesList();
                }
            }
        }

        public virtual void StartLinksGatheringInSeparateThread(bool isRentFlats = false) { }
        public virtual void StartApartmentGatheringInSeparateThread(bool isRentFlats = false) { }

        /// <summary>
        /// Вызывать при первом запуске процессинга агентом. По запомненному ServiceStage выбирает,
        /// с какого метода начать работу
        /// </summary>
        public void StartWorkingFromMemorizedStage()
        {
            setInitialCurrentActionFromDb();

            if (_currentActionName == Const.ParsingStatusDescriptionGatheringLinks && !_isProcessingCompleted)
                StartLinksGatheringInSeparateThread(false); // Продолжаем сбор ссылок
            else if (_currentActionName == Const.ParsingStatusDescriptionGatheringLinks)
                StartApartmentGatheringInSeparateThread(false); // Начинаем сбор квартир

            if (_currentActionName == Const.ParsingStatusDescriptionObservingFlats && !_isProcessingCompleted)
                StartApartmentGatheringInSeparateThread(false); // Продолжаем сбор квартир
            else if (_currentActionName == Const.ParsingStatusDescriptionObservingFlats)
                StartLinksGatheringInSeparateThread(true); // Начинаем сбор ссылок (аренда)

            if (_currentActionName == Const.ParsingStatusDescriptionGatheringLinksRent && !_isProcessingCompleted)
                StartLinksGatheringInSeparateThread(true); // Продолжаем сбор ссылок (аренда)
            else if (_currentActionName == Const.ParsingStatusDescriptionGatheringLinksRent)
                StartApartmentGatheringInSeparateThread(true); // Начинаем сбор квартир (аренда)

            if (_currentActionName == Const.ParsingStatusDescriptionObservingFlatsRent && !_isProcessingCompleted)
                StartApartmentGatheringInSeparateThread(true); // Продолжаем сбор квартир (аренда)
            else if (_currentActionName == Const.ParsingStatusDescriptionObservingFlatsRent)
                StartLinksGatheringInSeparateThread(false); // К началу жизненного цикла - снова начинаем сбор ссылок
        }

        protected override void initializeRepositories(RealtyParserContext context)
        {
            _pageLinkRepo = new EFGenericRepo<PageLink, RealtyParserContext>(context);
            _serviceStageRepo = new EFGenericRepo<ServiceStage, RealtyParserContext>(context);
        }

        public void Dispose()
        {
            _writeToLogDelegate("(ВНИМАНИЕ) Агент удален!");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Отмечает ссылку на квартиру как "Мертвую" (после того, как выдало 404).
        /// Если к странице уже привязана квартира, то обновляет у нее RemovalDate
        /// </summary>
        protected virtual void markLinkAsDead(PageLink pageLink)
        {
            // Нужно отдельно достать ссылку из БД, т.к. изначально все они доставались как AsNoTracking
            PageLink trackedPageLink = _pageLinkRepo.Get(pageLink.Id);

            trackedPageLink.LastCheckDateTime = DateTime.Now;
            trackedPageLink.IsDead = true;
            try
            {
                _pageLinkRepo.Save();
            }
            catch (Exception ex)
            {
                _stateLogger.LogApartmentMarkedAsDeadError(pageLink.Href);
                _writeToLogDelegate(string.Format("Не удалось отметить ссылку {0} как \"мертвую\": {1}", pageLink.Href, ex.Message));
            }
        }

        /// <summary>
        /// Пытается загрузить веб-страницу по ссылке с использованием прокси (если задано) за несколько попыток
        /// </summary>
        protected async Task<string> downloadString(string uri, string targetEncoding)
        {
            int triesCount = 0;
            string currentProxyAddress = "";
            while (triesCount < _maxRetryAmountForSingleRequest)
            {
                try
                {
                    using (HttpClient wc = createHttpClient())
                    {
                        currentProxyAddress = _currentProxy?.Ip.ToString();
                        using (HttpResponseMessage response = await wc.GetAsync(uri))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = response.Content;
                                byte[] contentBytes = responseContent.ReadAsByteArrayAsync().Result;
                                string downloadedString = Encoding.GetEncoding(targetEncoding).GetString(contentBytes);

                                findProxyInDbAndAddSuccessAmount();

                                return downloadedString;
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                                return "NotFound";
                            else
                            {
                                markProxyAsNotResponding();
                                triesCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("(404) Not Found"))
                        return "NotFound";

                    triesCount++;
                    markProxyAsNotResponding();
                    _writeToLogDelegate(string.Format("Не удалось загрузить ссылку {0}, попытка {1}, прокси {2}",
                        uri, triesCount, currentProxyAddress));
                }
                finally
                {
                    triesCount++;
                }
            }
            return "LoadingFailed";
        }

        protected async Task<string> downloadStringWithHttpRequest(string uri, string targetEncoding)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 1600;

            int triesCount = 0;
            string currentProxyAddress = "";
            while (triesCount < _maxRetryAmountForSingleRequest)
            {
                HttpWebRequest request = null;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                }
                catch (Exception ex)
                {
                    triesCount++;
                    _writeToLogDelegate(string.Format("Ошибка при создании запроса {0} - неправильная ссылка: {1}", uri, ex.Message));
                }

                if (_isUseProxy)
                {
                    _currentProxy = getRandomWebProxy();
                    request.Proxy = _currentProxy?.WebProxy;
                }
                currentProxyAddress = _currentProxy?.Ip.ToString();
                request.Method = "GET";
                request.Timeout = 60 * 1000;
                request.ServicePoint.ConnectionLimit = 1600;
                request.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                try
                {
                    triesCount++;
                    string pageStr = null;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(targetEncoding)))
                    {
                        pageStr = reader.ReadToEnd();
                        findProxyInDbAndAddSuccessAmount();

                        request.Abort();
                    }
                    return pageStr;
                }
                catch (Exception ex)
                {
                    request.Abort();
                    if (ex.Message.Contains("(404) Not Found"))
                        return "NotFound";

                    triesCount++;
                    markProxyAsNotResponding();
                    _writeToLogDelegate(string.Format("Не удалось загрузить ссылку {0}, попытка {1}, прокси {2}",
                        uri, triesCount, currentProxyAddress));
                }
            }
            return "LoadingFailed";
        }

        private void findProxyInDbAndAddSuccessAmount()
        {
            WebProxyInfo foundProxy = _currentProxy;

            OnlineProxyProvider proxyProvider = new OnlineProxyProvider(_dbContext, _writeToLogDelegate);
            proxyProvider.AddSuccessAmountToProxyInDb(foundProxy);
        }

        /// <summary>
        /// После неудачной попытки загрузить страницу через прокси отмечает проксю как NotResponding,
        /// чтобы больше ее не использовать
        /// </summary>
        private void markProxyAsNotResponding()
        {
            WebProxyInfo foundProxy = _currentProxy;
            OnlineProxyProvider proxyProvider = new OnlineProxyProvider(_dbContext, _writeToLogDelegate);
            proxyProvider.AddFailureAmountToProxyInDb(foundProxy);
        }

        /// <summary>
        /// Вспоминает по ServiceStage, на какой странице в прошлый раз остановился сервис при сборе ссылок
        /// </summary>
        protected void setSkipPages()
        {
            if (_tablePagesToSkip != 0)
                return;

            ServiceStage foundStage = _serviceStageRepo.GetAllWithoutTracking()
                .FirstOrDefault(x => x.Name == _siteName &&
                (x.CurrentStage == Const.ParsingStatusDescriptionGatheringLinks ||
                x.CurrentStage == Const.ParsingStatusDescriptionGatheringLinksRent));

            if (foundStage?.PageNumber == null)
                return;

            _tablePagesToSkip = foundStage.PageNumber.Value;
        }

        /// <summary>
        /// Вспоминает по ServiceStage, на каком этапе остановилась обработка
        /// </summary>
        protected void setInitialCurrentActionFromDb()
        {
            ServiceStage foundStage = _serviceStageRepo.GetAllWithoutTracking()
                .FirstOrDefault(x => x.Name == _siteName);

            if(string.IsNullOrEmpty(foundStage?.CurrentStage))
            {
                _currentActionName = Const.ParsingStatusDescriptionGatheringLinks;
                _isProcessingCompleted = false;
                return;
            }

            _currentActionName = foundStage.CurrentStage;
            _isProcessingCompleted = foundStage.IsComplete.GetValueOrDefault(false);
        }
    }
}
