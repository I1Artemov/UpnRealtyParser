using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnSiteAgent
    {
        protected WebClient _webClient;
        protected RealtyParserContext _dbContext;
        protected Thread _dbProcessingThread;
        protected bool _isConnectionOpen;

        protected EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        protected Action<string> _writeToLogDelegate;
        protected bool _isUseProxy;
        protected List<WebProxy> _proxyList;
        protected Random _random;
        protected int _requestDelayInMs;
        protected int _upnTablePagesToSkip;
        protected int _maxRetryAmountForSingleRequest;

        public UpnSiteAgent(Action<string> writeToLogDelegate, List<string> proxyStrList, bool isUseProxy, int requestDelayInMs,
            int upnTablePagesToSkip, int maxRetryAmountForSingleRequest)
        {
            _writeToLogDelegate = writeToLogDelegate;
            _requestDelayInMs = requestDelayInMs;
            _isUseProxy = isUseProxy;
            _upnTablePagesToSkip = upnTablePagesToSkip;
            _maxRetryAmountForSingleRequest = maxRetryAmountForSingleRequest;

            if(proxyStrList != null && proxyStrList.Count != 0)
                _proxyList = getProxiesFromIps(proxyStrList);

            _random = new Random();
        }

        protected void openConnection()
        {
            if (_isConnectionOpen)
                return;

            _dbContext = new RealtyParserContext();
            _webClient = createWebClient();

            initializeRepositories(_dbContext);

            _isConnectionOpen = true;
        }

        protected void closeConnection()
        {
            if (!_isConnectionOpen)
                return;

            _dbContext.Dispose();
            _webClient.Dispose();

            _isConnectionOpen = false;
        }

        protected void initializeRepositories(RealtyParserContext context)
        {
            _pageLinkRepo = new EFGenericRepo<PageLink, RealtyParserContext>(context);
        }

        public void StartLinksGatheringInSeparateThread()
        {
            ThreadStart threadMethod =
                delegate { this.GatherLinksAndInsertInDb(); };
            _dbProcessingThread = new Thread(threadMethod);
            _dbProcessingThread.IsBackground = true; // Для корректного завершения при закрытии окна
            _dbProcessingThread.Start();
        }

        public void StopProcessingInThread()
        {
            if (_dbProcessingThread == null)
                return;

            _dbProcessingThread.Abort();
            _writeToLogDelegate("Обработка остановлена");
        }

        /// <summary>
        /// Все, что касается сбора ссылок на квартиры и их сохранения в базу данных.
        /// Заходит на главную страницу с переченм квартир, получает общее кол-во квартир,
        /// собирает из таблицы все ссылки на просмотр страниц с описанием квартир
        /// </summary>
        public void GatherLinksAndInsertInDb()
        {
            const string mainTableUrl = "https://upn.ru/realty_eburg_flat_sale.htm";

            openConnection();

            string firstTablePageHtml = DownloadString(mainTableUrl);
            if (string.IsNullOrEmpty(firstTablePageHtml))
            {
                if (_writeToLogDelegate != null) _writeToLogDelegate("Не удалось загрузить веб-страницу с перечнем квартир");
                return;
            }
            UpnFlatLinksCollector linksCollector = new UpnFlatLinksCollector();

            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(firstTablePageHtml);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(firstTablePageHtml);

            if(totalApartmentsAmount.GetValueOrDefault(0) <= 0 || totalTablePages <= 0 || string.IsNullOrEmpty(pageUrlTemplate))
            {
                _writeToLogDelegate("Ошибка: не удалось обработать первую страницу сайта.");
                return;
            }

            _writeToLogDelegate(string.Format("Всего {0} записей на {1} страницах таблицы", totalApartmentsAmount.Value, totalTablePages));

            for(int currentPageNumber = _upnTablePagesToSkip; currentPageNumber <= totalTablePages; currentPageNumber++)
            {
                string currentTablePageUrl = string.Format(pageUrlTemplate, currentPageNumber);
                string currentTablePageHtml;
                currentTablePageHtml = DownloadString(currentTablePageUrl);

                if (string.IsNullOrEmpty(currentTablePageHtml))
                    continue;

                List<string> currentTablePageHrefs = linksCollector.GetLinksFromSinglePage(currentTablePageHtml);
                insertHrefsIntoDb(currentTablePageHrefs, Const.SiteNameUpn, currentPageNumber);

                if(_requestDelayInMs >= 0)
                    Thread.Sleep(_requestDelayInMs);
            }

            closeConnection();
        }

        /// <summary>
        /// Вставка (или обновление существующих) пачки ссылок в БД. Обработка идет по одной записи
        /// </summary>
        private void insertHrefsIntoDb(List<string> hrefs, string siteName, int pageNumber)
        {
            if (hrefs == null || hrefs.Count == 0 || _pageLinkRepo == null)
                return;

            int insertedAmount = 0;
            int updatedAmount = 0;
            foreach(string href in hrefs)
            {
                // Проверяем, есть ли уже такая ссылка в базе
                PageLink foundLink = _pageLinkRepo.GetAll()
                    .FirstOrDefault(x => x.Href == href && x.SiteName == siteName);

                // Если нашлась, то обновляем дату проверки
                if(foundLink != null)
                {
                    foundLink.LastCheckDateTime = DateTime.Now;
                    _pageLinkRepo.Update(foundLink);
                    updatedAmount++;
                }
                else
                {
                    PageLink linkForAddition = new PageLink
                    {
                        Href = href,
                        SiteName = siteName,
                        LinkType = Const.LinkTypeSellFlat
                    };
                    _pageLinkRepo.Add(linkForAddition);
                    insertedAmount++;
                }
            }
            _pageLinkRepo.Save();
            _writeToLogDelegate(string.Format("Обработана страница {0}: вставлено {1} записей, обновлено {2}.",
                pageNumber, insertedAmount, updatedAmount));
        }

        /// <summary>
        /// Переходит на страницы с описанием квартир (по списку ссылок на них), собирает информацию о каждой квартире
        /// и о ее доме со страницы и добавляет всё в БД
        /// </summary>
        public string ProcessAllApartmentsFromLinksInDb(List<string> apartmentHrefs, bool isAddSiteHref)
        {
            if (apartmentHrefs == null || apartmentHrefs.Count == 0)
                return "Перечень ссылок на квартиры пуст";

            foreach(string apartmentHref in apartmentHrefs)
            {
                string fullApartmentHref = isAddSiteHref ? "https://upn.ru" + apartmentHref : apartmentHref;

                string apartmentPageHtml;
                apartmentPageHtml = DownloadString(fullApartmentHref);

                if (string.IsNullOrEmpty(apartmentPageHtml))
                    continue;

                // Сбор сведений о доме
                UpnHouseParser houseParser = new UpnHouseParser();
                List<IElement> fieldValueElements = houseParser.GetTdElementsFromWebPage(apartmentPageHtml);
                UpnHouseInfo house = houseParser.GetUpnHouseFromPageText(fieldValueElements, apartmentPageHtml);

                // Сбор сведений о квартире
                UpnApartmentParser parser = new UpnApartmentParser();
                UpnFlat upnFlat = parser.GetUpnSellFlatFromPageText(fieldValueElements);
            }
            return null;
        }

        private WebClient createWebClient()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WebClient webClient = new WebClient
            {
                Encoding = Encoding.GetEncoding("windows-1251"),
            };

            if(_isUseProxy) { 
                webClient.Proxy = getRandomWebProxy();
            }

            return webClient;
        }

        protected WebProxy getRandomWebProxy()
        {
            int count = _proxyList.Count;
            int randomIndex = _random.Next(0, count - 1);

            return _proxyList[randomIndex];
        }

        /// <summary>
        /// Превращает список адресов прокси формата "(IP):(порт)" в объекты WebProxy
        /// </summary>
        protected List<WebProxy> getProxiesFromIps(List<string> proxyStrList)
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

            _writeToLogDelegate("Инициализация прокси-серверов завершена");
            return proxyList;
        }

        string DownloadString(string uri) {
            int triesCount = 0;
            string currentProxyAddress = "";
            while(triesCount < _maxRetryAmountForSingleRequest) { 
                try {
                    using (var wc = createWebClient()) {
                        currentProxyAddress = ((WebProxy)wc.Proxy)?.Address.ToString();
                        return wc.DownloadString(uri);
                    }
                }
                catch (Exception ex)
                {
                    triesCount++;
                    _writeToLogDelegate(string.Format("Не удалось загрузить ссылку {0}, попытка {1}, прокси {2}",
                        uri, triesCount, currentProxyAddress));
                }
            }
            return "LoadingFailed";
        }
    }
}
