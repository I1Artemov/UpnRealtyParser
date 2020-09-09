using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnSiteAgent : IDisposable
    {
        protected HttpClient _webClient;
        protected WebProxyInfo _currentProxy;
        protected RealtyParserContext _dbContext;
        protected Thread _LinksProcessingThread;
        protected Thread _apartmentProcessingThread;
        protected bool _isConnectionOpen;

        protected EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        protected EFGenericRepo<UpnHouseInfo, RealtyParserContext> _houseRepo;
        protected EFGenericRepo<UpnFlat, RealtyParserContext> _sellFlatRepo;
        protected EFGenericRepo<UpnRentFlat, RealtyParserContext> _rentFlatRepo;
        protected EFGenericRepo<UpnAgency, RealtyParserContext> _agencyRepo;
        protected EFGenericRepo<UpnFlatPhoto, RealtyParserContext> _photoRepo;

        protected StateLogger _stateLogger;
        protected Action<string> _writeToLogDelegate;
        protected bool _isUseProxy;
        protected Random _random;
        protected int _requestDelayInMs;
        protected int _upnTablePagesToSkip;
        protected int _maxRetryAmountForSingleRequest;
        protected int _maxRequestTimeoutInMs;

        protected int _processedObjectsCount;
        protected bool _isProcessingCompleted;
        protected string _currentActionName;

        public UpnSiteAgent(Action<string> writeToLogDelegate, AppSettings settings)
        {
            _writeToLogDelegate = writeToLogDelegate;
            _requestDelayInMs = settings.RequestDelayInMs;
            _isUseProxy = settings.IsUseProxies;
            _upnTablePagesToSkip = settings.UpnTablePagesToSkip;
            _maxRetryAmountForSingleRequest = settings.MaxRetryAmountForSingleRequest;
            _maxRequestTimeoutInMs = settings.MaxRequestTimeoutInMs;

            // Берем прокси либо из списка, либо из сети (если они нужны)
            if(settings.IsUseProxies && !settings.IsGetProxiesListFromGithub && settings.ProxyList != null && settings.ProxyList.Count != 0)
            {
                using (var realtyContext = new RealtyParserContext())
                {
                    OnlineProxyProvider proxyProvider = new OnlineProxyProvider(realtyContext, writeToLogDelegate);
                    proxyProvider.GetProxiesFromIps(settings.ProxyList);
                }
            }
            if(settings.IsUseProxies && settings.IsGetProxiesListFromGithub)
            {
                using (var realtyContext = new RealtyParserContext())
                {
                    OnlineProxyProvider proxyProvider = new OnlineProxyProvider(realtyContext, writeToLogDelegate);
                    proxyProvider.GetAliveProxiesList();
                }
            }

            _random = new Random();
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
            return _LinksProcessingThread.ThreadState;
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

        protected void initializeRepositories(RealtyParserContext context)
        {
            _pageLinkRepo = new EFGenericRepo<PageLink, RealtyParserContext>(context);
            _houseRepo = new EFGenericRepo<UpnHouseInfo, RealtyParserContext>(context);
            _sellFlatRepo = new EFGenericRepo<UpnFlat, RealtyParserContext>(context);
            _rentFlatRepo = new EFGenericRepo<UpnRentFlat, RealtyParserContext>(context);
            _agencyRepo = new EFGenericRepo<UpnAgency, RealtyParserContext>(context);
            _photoRepo = new EFGenericRepo<UpnFlatPhoto, RealtyParserContext>(context);
            _stateLogger = new StateLogger(context);
        }

        public void StartLinksGatheringInSeparateThread(bool isRentFlats = false)
        {
            _isProcessingCompleted = false;
            _currentActionName = isRentFlats ? Const.ParsingStatusDescriptionGatheringLinksRent :
                Const.ParsingStatusDescriptionGatheringLinks;

            ThreadStart threadMethod = delegate { this.GatherLinksAndInsertInDb(isRentFlats); };
            _LinksProcessingThread = new Thread(threadMethod);
            _LinksProcessingThread.IsBackground = true; // Для корректного завершения при закрытии окна
            _LinksProcessingThread.Start();
        }

        public void StartApartmentGatheringInSeparateThread(bool isRentFlats = false)
        {
            _isProcessingCompleted = false;
            _currentActionName = isRentFlats ? Const.ParsingStatusDescriptionObservingFlatsRent :
                Const.ParsingStatusDescriptionObservingFlats;

            ThreadStart threadMethod = delegate { this.GetApartmentLinksFromDbAndProcessApartments(isRentFlats); };
            _apartmentProcessingThread = new Thread(threadMethod);
            _apartmentProcessingThread.IsBackground = true;
            _apartmentProcessingThread.Start();
        }

        public void StopProcessingInThreads()
        {
            if (_LinksProcessingThread != null)
                _LinksProcessingThread.Abort();

            if(_apartmentProcessingThread != null)
                _apartmentProcessingThread.Abort();

            _writeToLogDelegate("Обработка остановлена");
        }

        /// <summary>
        /// Все, что касается сбора ссылок на квартиры и их сохранения в базу данных.
        /// Заходит на главную страницу с переченм квартир, получает общее кол-во квартир,
        /// собирает из таблицы все ссылки на просмотр страниц с описанием квартир
        /// </summary>
        /// <param name="isRentFlats">Обработка квартир под аренду</param>
        public void GatherLinksAndInsertInDb(bool isRentFlats = false)
        {
            string mainTableUrl = isRentFlats ? "https://upn.ru/realty_eburg_flat_rent.htm" : "https://upn.ru/realty_eburg_flat_sale.htm";
            string rentLogMessageAddition = isRentFlats ? " (аренда)" : "";

            string firstTablePageHtml = DownloadString(mainTableUrl);
            if (string.IsNullOrEmpty(firstTablePageHtml))
            {
                _writeToLogDelegate?.Invoke("Не удалось загрузить веб-страницу с перечнем квартир" + rentLogMessageAddition);
                _stateLogger.LogFirstPageLoadingFailure("Не удалось загрузить страницу" + rentLogMessageAddition);
                return;
            }
            UpnFlatLinksCollector linksCollector = new UpnFlatLinksCollector();

            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(firstTablePageHtml);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(firstTablePageHtml, isRentFlats);

            if(totalApartmentsAmount.GetValueOrDefault(0) <= 0 || totalTablePages <= 0 || string.IsNullOrEmpty(pageUrlTemplate))
            {
                _writeToLogDelegate?.Invoke("Ошибка: не удалось обработать первую страницу сайта" + rentLogMessageAddition);
                _stateLogger.LogFirstPageLoadingFailure("Не удалось обработать страницу" + rentLogMessageAddition);
                return;
            }

            _writeToLogDelegate?.Invoke(string.Format("Всего {0} записей на {1} страницах таблицы{2}",
                totalApartmentsAmount.GetValueOrDefault(0), totalTablePages, rentLogMessageAddition));
            _stateLogger.LogFirstPageLoading(totalApartmentsAmount.GetValueOrDefault(0), totalTablePages, isRentFlats);

            _processedObjectsCount = 0;
            for (int currentPageNumber = _upnTablePagesToSkip; currentPageNumber <= totalTablePages; currentPageNumber++)
            {
                string currentTablePageUrl = string.Format(pageUrlTemplate, currentPageNumber);
                string currentTablePageHtml = DownloadString(currentTablePageUrl);

                if (string.IsNullOrEmpty(currentTablePageHtml)) {
                    _stateLogger.LogLinksPageLoadingFailure(currentPageNumber, isRentFlats);
                    continue;
                }

                List<string> currentTablePageHrefs = linksCollector.GetLinksFromSinglePage(currentTablePageHtml, isRentFlats);
                insertHrefsIntoDb(currentTablePageHrefs, Const.SiteNameUpn, currentPageNumber, isRentFlats);

                if(_requestDelayInMs >= 0)
                    Thread.Sleep(_requestDelayInMs);

                _processedObjectsCount++;
            }

            _stateLogger.LogLinksGatheringCompletion(isRentFlats);
            CloseConnection();
            _writeToLogDelegate?.Invoke("Сбор ссылок завершен" + rentLogMessageAddition);
            _isProcessingCompleted = true;
        }

        /// <summary>
        /// Вставка (или обновление существующих) пачки ссылок в БД. Обработка идет по одной записи
        /// </summary>
        private void insertHrefsIntoDb(List<string> hrefs, string siteName, int pageNumber, bool isRentFlats)
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
                    updateExistingFlatByExistingPageLinkOrGetError(foundLink);
                }
                else
                {
                    string linkType = isRentFlats ? Const.LinkTypeRentFlat : Const.LinkTypeSellFlat;
                    PageLink linkForAddition = new PageLink
                    {
                        Href = href,
                        SiteName = siteName,
                        LinkType = linkType
                    };
                    _pageLinkRepo.Add(linkForAddition);
                    insertedAmount++;
                }
            }
            _pageLinkRepo.Save();
            _writeToLogDelegate(string.Format("Обработана страница {0}: вставлено {1} записей, обновлено {2}.",
                pageNumber, insertedAmount, updatedAmount));
            _stateLogger.LogLinksPageProcessingResult(pageNumber, insertedAmount, updatedAmount);
        }

        /// <summary>
        /// Обновляет дату последней проверки у квартиры, соответствующей ссылке на страницу
        /// </summary>
        private void updateExistingFlatByExistingPageLinkOrGetError(PageLink foundLink)
        {
            UpnRentFlat foundRentFlat = _rentFlatRepo.GetAll().FirstOrDefault(x => x.PageLinkId == foundLink.Id);
            if(foundRentFlat != null)
            {
                try
                {
                    foundRentFlat.LastCheckDate = DateTime.Now;
                    _rentFlatRepo.Update(foundRentFlat);
                    _rentFlatRepo.Save();
                }
                catch(Exception ex)
                {
                    _writeToLogDelegate( "Ошибка обновления арендной квартиры " + foundRentFlat.Id + " по ссылке на страницу: " + ex.Message);
                }
            }

            UpnFlat foundSellFlat = _sellFlatRepo.GetAll().FirstOrDefault(x => x.PageLinkId == foundLink.Id);
            if (foundSellFlat != null)
            {
                try
                {
                    foundSellFlat.LastCheckDate = DateTime.Now;
                    _sellFlatRepo.Update(foundSellFlat);
                    _sellFlatRepo.Save();
                }
                catch (Exception ex)
                {
                    _writeToLogDelegate("Ошибка обновления квартиры в продаже " + foundSellFlat.Id + " по ссылке на страницу: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Считывает из БД ссылки на квартиры порциями по N штук и запускает обработку квартир по каждой ссылке
        /// </summary>
        public void GetApartmentLinksFromDbAndProcessApartments(bool isRentFlats)
        {
            string targetLinkType = isRentFlats ? Const.LinkTypeRentFlat : Const.LinkTypeSellFlat;
            IQueryable<PageLink> linksFilterQuery = _pageLinkRepo.GetAllWithoutTracking()
                .Where(x => x.LinkType == targetLinkType && x.SiteName == Const.SiteNameUpn
                        && (x.IsDead == null || x.IsDead.Value == false));

            int totalLinksCount = linksFilterQuery.Count();

            List<PageLink> apartmentHrefs = linksFilterQuery
                .ToList();
            _stateLogger.LogApartmentsParsingStart(apartmentHrefs.Count, isRentFlats);

            ProcessAllApartmentsFromLinks(apartmentHrefs, true, isRentFlats);

            _isProcessingCompleted = true;
        }

        /// <summary>
        /// Переходит на страницы с описанием квартир (по списку ссылок на них), собирает информацию о каждой квартире,
        /// о ее доме и о риэлторе со страницы и добавляет всё в БД
        /// </summary>
        public void ProcessAllApartmentsFromLinks(List<PageLink> apartmentHrefs, bool isAddSiteHref, bool isRentFlats)
        {
            string rentLogMessageAddition = isRentFlats ? " (аренда)" : "";
            if (apartmentHrefs == null || apartmentHrefs.Count == 0)
                _writeToLogDelegate("Перечень ссылок на квартиры пуст" + rentLogMessageAddition);

            _processedObjectsCount = 0;
            foreach(PageLink apartmentLink in apartmentHrefs)
            {
                if (isFlatAlreadyInDb(apartmentLink.Id, isRentFlats)) // TODO: Пока что не обрабатываем обновление данных в уже распарсенных квартирах
                {
                    continue;
                }

                string fullApartmentHref = isAddSiteHref ? "https://upn.ru" + apartmentLink.Href : apartmentLink.Href;
                string apartmentPageHtml = DownloadString(fullApartmentHref);

                if(apartmentPageHtml == "NotFound")
                {
                    _writeToLogDelegate(string.Format("Страница по ссылке {0} не найдена (квартира продана){1}",
                        apartmentLink.Href, rentLogMessageAddition));
                    markLinkAsDead(apartmentLink);
                    _stateLogger.LogApartmentParsingNotFoundOnSite(apartmentLink.Href, isRentFlats);
                    _processedObjectsCount++;
                    if (_requestDelayInMs >= 0) Thread.Sleep(_requestDelayInMs);
                    continue;
                }

                if (string.IsNullOrEmpty(apartmentPageHtml) || apartmentPageHtml == "LoadingFailed")
                {
                    _writeToLogDelegate(string.Format("Проблема при загрузке страницы {0}, переход к следующей{1}",
                        apartmentLink.Href, rentLogMessageAddition));
                    _stateLogger.LogApartmentPageLoadingProplem(apartmentLink.Href, isRentFlats);
                    _processedObjectsCount++;
                    continue;
                }

                processAllDataAboutSingleApartmentAndUpdateDb(apartmentPageHtml, apartmentLink, isRentFlats);

                if (_requestDelayInMs >= 0)
                    Thread.Sleep(_requestDelayInMs);

                _processedObjectsCount++;
            }
            _writeToLogDelegate("Обработка квартир завершена" + rentLogMessageAddition);
        }

        protected void processAllDataAboutSingleApartmentAndUpdateDb(string apartmentPageHtml, PageLink apartmentLink, bool isRentFlats)
        {
            // Сбор сведений о доме
            UpnHouseParser houseParser = new UpnHouseParser();
            List<IElement> fieldValueElements = houseParser.GetTdElementsFromWebPage(apartmentPageHtml);
            UpnHouseInfo house = houseParser.GetUpnHouseFromPageText(fieldValueElements, apartmentPageHtml);
            bool isHouseCreatedSuccessfully = false;
            if (_houseRepo != null)
            {
                DistanceCalculator distanceCalc = new DistanceCalculator(_dbContext);
                distanceCalc.FindClosestSubwayForSingleHouse(house);
                isHouseCreatedSuccessfully = updateOrAddHouse(house);
            }
            if (!isHouseCreatedSuccessfully)
            {
                _stateLogger.LogErrorProcessingHouse(apartmentLink.Href);
                return;
            }

            // Сбор сведений об агентстве
            UpnAgencyParser agencyParser = new UpnAgencyParser();
            UpnAgency agency = agencyParser.GetAgencyFromPageText(fieldValueElements);
            updateOrAddAgency(agency);

            // Сбор сведений о квартире
            UpnApartmentParser apartmentParser = new UpnApartmentParser();
            int flatId;
            if(isRentFlats)
            {
                UpnRentFlat upnRentFlat = apartmentParser.GetUpnRentFlatFromPageText(fieldValueElements);
                updateOrAddRentFlat(upnRentFlat, house.Id.GetValueOrDefault(1), apartmentLink.Id, agency.Id.GetValueOrDefault(1));
                flatId = upnRentFlat.Id.GetValueOrDefault(0);
            }
            else
            {
                UpnFlat upnSellFlat = apartmentParser.GetUpnSellFlatFromPageText(fieldValueElements);
                updateOrAddSellFlat(upnSellFlat, house.Id.GetValueOrDefault(1), apartmentLink.Id, agency.Id.GetValueOrDefault(1));
                flatId = upnSellFlat.Id.GetValueOrDefault(0);
            }

            // Сбор ссылок на фотографии квартиры
            List<string> photoHrefs = apartmentParser.GetPhotoHrefsFromPage(apartmentPageHtml);
            updateOrAddPhotoHrefs(photoHrefs, flatId, isRentFlats);
        }

        /// <summary>
        /// Проверяет, существует ли уже дом с таким адресом в базе данных.
        /// Если нет, то добавляет в БД с сохранением. Если да, то объекту присваивает Id существующего дома
        /// </summary>
        private bool updateOrAddHouse(UpnHouseInfo house)
        {
            var existingHouse = _houseRepo.GetAllWithoutTracking()
                .FirstOrDefault(x => x.Address == house.Address);

            if (existingHouse != null)
            {
                house.Id = existingHouse.Id;
                _writeToLogDelegate(string.Format("Дом с адресом {1} уже существует (Id {0})", house.Id, house.Address));
            }
            else
            {
                _houseRepo.Add(house);
                try { 
                    _houseRepo.Save(); // TODO: Починить
                    _stateLogger.LogHouseAddition(house.Id.Value, house.Address);
                    _writeToLogDelegate(string.Format("Добавлен дом: Id {0}, адрес {1}", house.Id, house.Address));
                }
                catch(Exception ex)
                {
                    _writeToLogDelegate(string.Format("Не удалось добавить дом с адресом {0}. Ошибка: {1}", house.Address, ex.Message));
                    return false;
                }
            }
            return true;
        }

        private void updateOrAddSellFlat(UpnFlat upnSellFlat, int houseId, int pageLinkId, int agencyId)
        {
            upnSellFlat.UpnHouseInfoId = houseId;
            upnSellFlat.PageLinkId = pageLinkId;
            upnSellFlat.UpnAgencyId = agencyId;
            upnSellFlat.LastCheckDate = DateTime.Now;

            UpnFlat existingFlat = _sellFlatRepo.GetAllWithoutTracking()
                    .Where(x => x.PageLinkId == pageLinkId)
                    .FirstOrDefault();

            if (existingFlat != null)
                return;

            _sellFlatRepo.Add(upnSellFlat);
            _sellFlatRepo.Save();
            
            _stateLogger.LogApartmentAddition(upnSellFlat.Id.Value, upnSellFlat.UpnHouseInfoId.Value, false);
            _writeToLogDelegate(string.Format("Добавлена квартира: Id {0}, Id дома {1}, Id ссылки {2}, Аренда=false",
                upnSellFlat.Id, houseId, pageLinkId));
        }

        private void updateOrAddRentFlat(UpnRentFlat upnRentFlat, int houseId, int pageLinkId, int agencyId)
        {
            upnRentFlat.UpnHouseInfoId = houseId;
            upnRentFlat.PageLinkId = pageLinkId;
            upnRentFlat.UpnAgencyId = agencyId;
            upnRentFlat.LastCheckDate = DateTime.Now;

            UpnRentFlat existingFlat = _rentFlatRepo.GetAll()
                    .Where(x => x.PageLinkId == pageLinkId)
                    .FirstOrDefault();

            if (existingFlat != null)
                return;

            _rentFlatRepo.Add(upnRentFlat);
            _rentFlatRepo.Save();

            _stateLogger.LogApartmentAddition(upnRentFlat.Id.Value, upnRentFlat.UpnHouseInfoId.Value, true);
            _writeToLogDelegate(string.Format("Добавлена квартира: Id {0}, Id дома {1}, Id ссылки {2}, Аренда=true",
                upnRentFlat.Id, houseId, pageLinkId));
        }

        private bool isFlatAlreadyInDb(int pageLinkId, bool isRentFlats)
        {
            var isExisting = isRentFlats ?
                _rentFlatRepo.GetAllWithoutTracking().Any(x => x.PageLinkId == pageLinkId) :
                _sellFlatRepo.GetAllWithoutTracking().Any(x => x.PageLinkId == pageLinkId);
            return isExisting;
        }

        private void updateOrAddAgency(UpnAgency agency)
        {
            UpnAgency existingAgency = _agencyRepo.GetAllWithoutTracking().FirstOrDefault(
                x => x.Name == agency.Name &&
                x.AgentPhone == agency.AgentPhone &&
                x.CompanyPhone == agency.CompanyPhone &&
                x.Email == agency.Email);

            if(existingAgency != null)
            {
                agency.Id = existingAgency.Id;
            }
            else
            {
                _agencyRepo.Add(agency);
                _agencyRepo.Save();
                _stateLogger.LogAgencyAddition(agency.Id.Value);
                _writeToLogDelegate(string.Format("Добавлено агентство: Id {0}, телефон агента {1}", agency.Id, agency.AgentPhone));
            }
        }

        protected void updateOrAddPhotoHrefs(List<string> hrefs, int apartmentId, bool isRentFlats)
        {
            string targetRelationType = isRentFlats ? Const.LinkTypeRentFlat : Const.LinkTypeSellFlat;
            foreach(string href in hrefs)
            {
                UpnFlatPhoto photo = new UpnFlatPhoto {
                    CreationDateTime = DateTime.Now,
                    RelationType = targetRelationType,
                    FlatId = apartmentId,
                    Href = href
                };
                _photoRepo.Add(photo);
            }
            _photoRepo.Save();
            _stateLogger.LogApartmentPhotoHrefsAddition(apartmentId, hrefs.Count);
            _writeToLogDelegate(string.Format("Обнаружено {0} ссылок на фото для квартиры (Id = {1} Rent = {2})",
                hrefs.Count, apartmentId, isRentFlats));
        }

        /// <summary>
        /// Отмечает ссылку на квартиру как "Мертвую" (после того, как выдало 404).
        /// Если к странице уже привязана квартира, то обновляет у нее RemovalDate
        /// </summary>
        private void markLinkAsDead(PageLink pageLink)
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

            UpnFlat linkedFlat = _sellFlatRepo.GetAll()
                .FirstOrDefault(x => x.PageLinkId == pageLink.Id);

            if(linkedFlat != null)
            {
                linkedFlat.RemovalDate = DateTime.Now;
                _sellFlatRepo.Update(linkedFlat);
                _stateLogger.LogApartmentMarkedAsDead(pageLink.Href);
                _writeToLogDelegate(string.Format("Квартира (Id {0}) отмечена как удаленная", linkedFlat.Id));
            }
        }

        private HttpClient createHttpClient()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            HttpClientHandler clientHandler = new HttpClientHandler();
            if (_isUseProxy) { 
                _currentProxy = getRandomWebProxy();
                clientHandler.Proxy = _currentProxy.WebProxy;
                clientHandler.UseProxy = true;
            }

            HttpClient httpClient = new HttpClient(clientHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(20); // TODO: В параметры
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
        string DownloadString(string uri) {
            int triesCount = 0;
            string currentProxyAddress = "";
            while(triesCount < _maxRetryAmountForSingleRequest) { 
                try {
                    using (HttpClient wc = createHttpClient()) {
                        currentProxyAddress = _currentProxy?.Ip.ToString();
                        using (HttpResponseMessage response = wc.GetAsync(uri).Result)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = response.Content;
                                byte[] contentBytes = responseContent.ReadAsByteArrayAsync().Result;
                                string downloadedString = Encoding.GetEncoding("windows-1251").GetString(contentBytes);

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
        /// Почле неудачной попытки загрузить страницу через прокси отмечает проксю как NotResponding,
        /// чтобы больше ее не использовать
        /// </summary>
        private void markProxyAsNotResponding()
        {
            WebProxyInfo foundProxy = _currentProxy;
            OnlineProxyProvider proxyProvider = new OnlineProxyProvider(_dbContext, _writeToLogDelegate);
            proxyProvider.AddFailureAmountToProxyInDb(foundProxy);
        }

        public void Dispose()
        {
            _writeToLogDelegate("(ВНИМАНИЕ) Агент удален!");
            throw new NotImplementedException();
        }
    }
}
