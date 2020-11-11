using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class AnyRealtySiteParser : IDisposable
    {
        protected HttpClient _webClient;
        protected WebProxyInfo _currentProxy;
        protected RealtyParserContext _dbContext;
        protected Thread _linksProcessingThread;
        protected Thread _apartmentProcessingThread;
        protected bool _isConnectionOpen;

        protected EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        
        protected StateLogger _stateLogger;
        protected bool _isUseProxy;
        protected Random _random;
        protected int _requestDelayInMs;
        protected Action<string> _writeToLogDelegate;

        protected int _maxRetryAmountForSingleRequest;
        protected int _maxRequestTimeoutInMs;

        protected int _processedObjectsCount;
        protected bool _isProcessingCompleted;
        protected string _currentActionName;

        public AnyRealtySiteParser(Action<string> writeToLogDelegate, AppSettings settings)
        {
            _writeToLogDelegate = writeToLogDelegate;
            _requestDelayInMs = settings.RequestDelayInMs;
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
            if (settings.IsUseProxies && settings.IsGetProxiesListFromGithub)
            {
                using (var realtyContext = new RealtyParserContext())
                {
                    OnlineProxyProvider proxyProvider = new OnlineProxyProvider(realtyContext, writeToLogDelegate);
                    proxyProvider.GetAliveProxiesList();
                }
            }

            _random = new Random();
        }

        protected virtual void initializeRepositories(RealtyParserContext context)
        {
            _pageLinkRepo = new EFGenericRepo<PageLink, RealtyParserContext>(context);
        }

        public string GetCurrentActionName() => _currentActionName;

        public bool CheckIfProcessingCompleted() => _isProcessingCompleted;

        public int GetProcessedRecordsAmount() => _processedObjectsCount;

        public ThreadState GetApartmentThreadState()
        {
            return _apartmentProcessingThread.ThreadState;
        }

        public ThreadState GetLinksThreadState()
        {
            return _linksProcessingThread.ThreadState;
        }

        public void OpenConnection()
        {
            if (_isConnectionOpen)
                return;

            _dbContext = new RealtyParserContext();
            _webClient = createHttpClient();

            initializeRepositories(_dbContext);

            _isConnectionOpen = true;
        }

        public void CloseConnection()
        {
            if (!_isConnectionOpen)
                return;

            _dbContext.Dispose();
            _webClient.Dispose();

            _isConnectionOpen = false;
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

        private HttpClient createHttpClient()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            HttpClientHandler clientHandler = new HttpClientHandler();
            if (_isUseProxy)
            {
                _currentProxy = getRandomWebProxy();
                clientHandler.Proxy = _currentProxy.WebProxy;
                //clientHandler.Proxy = new System.Net.WebProxy("127.0.0.1:8888");
                //clientHandler.Proxy = new System.Net.WebProxy("217.172.122.4:8080");
                clientHandler.UseProxy = true;
            } else
            {
                clientHandler.UseProxy = false;
            }

            HttpClient httpClient = new HttpClient(clientHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(400); // TODO: В параметры
            return httpClient;
        }


        /// <summary>
        /// Возвращает случайно выбранную прокси. Игнорирует прокси, у которых ранее был установлен признак IsNotResponding
        /// </summary>
        protected WebProxyInfo getRandomWebProxy()
        {
            OnlineProxyProvider proxyProvider = new OnlineProxyProvider(_dbContext, _writeToLogDelegate);
            return proxyProvider.GetRandomWebProxy(_random);
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
    }
}
