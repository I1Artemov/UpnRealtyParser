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
    public class N1SiteAgent : AnyRealtySiteParser
    {
        protected EFGenericRepo<N1HouseInfo, RealtyParserContext> _houseRepo;
        protected EFGenericRepo<N1Flat, RealtyParserContext> _sellFlatRepo;
        protected EFGenericRepo<N1Agency, RealtyParserContext> _agencyRepo;
        // TODO: RentFlat, Photo

        public N1SiteAgent(Action<string> writeToLogDelegate, AppSettings settings) :
            base(writeToLogDelegate, settings)
        { }

        protected override void initializeRepositories(RealtyParserContext context)
        {
            base.initializeRepositories(context);
            _houseRepo = new EFGenericRepo<N1HouseInfo, RealtyParserContext>(context);
            _sellFlatRepo = new EFGenericRepo<N1Flat, RealtyParserContext>(context);
            _agencyRepo = new EFGenericRepo<N1Agency, RealtyParserContext>(context);
            _stateLogger = new StateLogger(context, Const.SiteNameN1);
            // TODO: RentFlat, Photo
        }

        public void StartLinksGatheringInSeparateThread(bool isRentFlats = false)
        {
            _isProcessingCompleted = false;
            _currentActionName = isRentFlats ? Const.ParsingStatusDescriptionGatheringLinksRent :
                Const.ParsingStatusDescriptionGatheringLinks;

            ThreadStart threadMethod = delegate { this.GatherLinksAndInsertInDb(isRentFlats); };
            _linksProcessingThread = new Thread(threadMethod);
            _linksProcessingThread.IsBackground = true; // Для корректного завершения при закрытии окна
            _linksProcessingThread.Start();
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

        /// <summary>
        /// Все, что касается сбора ссылок на квартиры и их сохранения в базу данных.
        /// Заходит на главную страницу с переченм квартир, получает общее кол-во квартир,
        /// собирает из таблицы все ссылки на просмотр страниц с описанием квартир
        /// </summary>
        /// <param name="isRentFlats">Обработка квартир под аренду</param>
        public void GatherLinksAndInsertInDb(bool isRentFlats = false)
        {
            string mainTableUrl = isRentFlats ? "https://ekaterinburg.n1.ru/snyat/dolgosrochno/kvartiry?page=1&limit=100" :
                "https://ekaterinburg.n1.ru/kupit/kvartiry?page=1&limit=100";
            string rentLogMessageAddition = isRentFlats ? " (аренда)" : "";

            string firstTablePageHtml = downloadStringWithHttpRequest(mainTableUrl, "utf-8").Result;
            if (string.IsNullOrEmpty(firstTablePageHtml))
            {
                _writeToLogDelegate?.Invoke("Не удалось загрузить веб-страницу с перечнем квартир" + rentLogMessageAddition);
                _stateLogger.LogFirstPageLoadingFailure("Не удалось загрузить страницу" + rentLogMessageAddition);
                return;
            }
            N1FlatLinksCollector linksCollector = new N1FlatLinksCollector();

            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(firstTablePageHtml);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(firstTablePageHtml, isRentFlats);

            if (totalApartmentsAmount.GetValueOrDefault(0) <= 0 || totalTablePages <= 0 || string.IsNullOrEmpty(pageUrlTemplate))
            {
                _writeToLogDelegate?.Invoke("Ошибка: не удалось обработать первую страницу сайта" + rentLogMessageAddition);
                _stateLogger.LogFirstPageLoadingFailure("Не удалось обработать страницу" + rentLogMessageAddition);
                return;
            }

            _writeToLogDelegate?.Invoke(string.Format("Всего {0} записей на {1} страницах таблицы{2}",
                totalApartmentsAmount.GetValueOrDefault(0), totalTablePages, rentLogMessageAddition));
            _stateLogger.LogFirstPageLoading(totalApartmentsAmount.GetValueOrDefault(0), totalTablePages, isRentFlats);

            _processedObjectsCount = 0;
            for (int currentPageNumber = 1; currentPageNumber <= totalTablePages; currentPageNumber++)
            {
                string currentTablePageUrl = string.Format(pageUrlTemplate, currentPageNumber);
                string currentTablePageHtml = downloadStringWithHttpRequest(currentTablePageUrl, "utf-8").Result;

                List<string> currentTablePageHrefs = linksCollector.GetLinksFromSinglePage(currentTablePageHtml, isRentFlats);
                insertHrefsIntoDb(currentTablePageHrefs, Const.SiteNameN1, currentPageNumber, isRentFlats);

                // Со страницы с перечнем квартир предварительно заполняем и сами квартиры
                N1ApartmentParser flatParser = new N1ApartmentParser();
                N1HouseParser houseParser = new N1HouseParser();
                List<N1Flat> prefilledFlats = flatParser.GetN1SellFlatsFromTablePage(currentTablePageHtml, houseParser);
                insertPrefilledFlatsIntoDb(prefilledFlats);

                if (string.IsNullOrEmpty(currentTablePageHtml))
                {
                    _stateLogger.LogLinksPageLoadingFailure(currentPageNumber, isRentFlats);
                    continue;
                }

                if (_requestDelayInMs >= 0)
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
            foreach (string href in hrefs)
            {
                // Проверяем, есть ли уже такая ссылка в базе
                PageLink foundLink = _pageLinkRepo.GetAll()
                    .FirstOrDefault(x => x.Href == href && x.SiteName == siteName);

                // Если нашлась, то обновляем дату проверки
                if (foundLink != null)
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
            // TODO: rent flats
            /*N1RentFlat foundRentFlat = _rentFlatRepo.GetAll().FirstOrDefault(x => x.PageLinkId == foundLink.Id);
            if (foundRentFlat != null)
            {
                try
                {
                    foundRentFlat.LastCheckDate = DateTime.Now;
                    _rentFlatRepo.Update(foundRentFlat);
                    _rentFlatRepo.Save();
                }
                catch (Exception ex)
                {
                    _writeToLogDelegate("Ошибка обновления арендной квартиры " + foundRentFlat.Id + " по ссылке на страницу: " + ex.Message);
                }
            }*/
            // TODO: sell flats
            /*
            N1Flat foundSellFlat = _sellFlatRepo.GetAll().FirstOrDefault(x => x.PageLinkId == foundLink.Id);
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
            }*/

        }

        private void insertPrefilledFlatsIntoDb(List<N1Flat> flats)
        {
            foreach(N1Flat flat in flats)
            {
                // Обработка дома
                if(flat.ConnectedHouseForAddition != null)
                {
                    bool isHouseCreatedSuccessfully = false;
                    if (_houseRepo != null)
                    {
                        DistanceCalculator distanceCalc = new DistanceCalculator(_dbContext);
                        distanceCalc.FindClosestSubwayForSingleHouse(flat.ConnectedHouseForAddition);
                        isHouseCreatedSuccessfully = updateOrAddHouse(flat.ConnectedHouseForAddition);
                    }
                    if (!isHouseCreatedSuccessfully)
                    {
                        _stateLogger.LogErrorProcessingHouse(flat.Href);
                        return;
                    }
                }

                // Обработка самой квартиры (агентства нет)
                PageLink foundLink = _pageLinkRepo.GetAll()
                    .FirstOrDefault(x => x.Href == flat.Href && x.SiteName == Const.SiteNameN1);

                if(foundLink == null || flat.ConnectedHouseForAddition.Id == null)
                {
                    _stateLogger.LogAnyMessage(Const.SiteNameN1, "AddingPrefilledFlats", "Не найдена ссылка или дом в БД",
                        Const.StatusTypeFailure);
                    continue;
                }

                updateOrAddSellFlat(flat, flat.ConnectedHouseForAddition.Id.GetValueOrDefault(-1), foundLink.Id, null);
            }
        }

        /// <summary>
        /// Считывает из БД ссылки на квартиры порциями по N штук и запускает обработку квартир по каждой ссылке
        /// </summary>
        public void GetApartmentLinksFromDbAndProcessApartments(bool isRentFlats)
        {
            string targetLinkType = isRentFlats ? Const.LinkTypeRentFlat : Const.LinkTypeSellFlat;
            IQueryable<PageLink> linksFilterQuery = _pageLinkRepo.GetAllWithoutTracking()
                .Where(x => x.LinkType == targetLinkType && x.SiteName == Const.SiteNameN1
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
            foreach (PageLink apartmentLink in apartmentHrefs)
            {
                if (isFlatAlreadyInDb(apartmentLink.Id, isRentFlats)) // TODO: Пока что не обрабатываем обновление данных в уже распарсенных квартирах
                {
                    continue;
                }

                string fullApartmentHref = isAddSiteHref ? "https://ekaterinburg.n1.ru" + apartmentLink.Href : apartmentLink.Href;
                string apartmentPageHtml = downloadString(fullApartmentHref, "utf-8").Result;

                if (apartmentPageHtml == "NotFound")
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

                // TODO: insert
                //processAllDataAboutSingleApartmentAndUpdateDb(apartmentPageHtml, apartmentLink, isRentFlats);

                if (_requestDelayInMs >= 0)
                    Thread.Sleep(_requestDelayInMs);

                _processedObjectsCount++;
            }
            _writeToLogDelegate("Обработка квартир завершена" + rentLogMessageAddition);
        }

        private bool isFlatAlreadyInDb(int pageLinkId, bool isRentFlats)
        {
            // TODO: Rent flats
            /*var isExisting = isRentFlats ?
                _rentFlatRepo.GetAllWithoutTracking().Any(x => x.PageLinkId == pageLinkId) :
                _sellFlatRepo.GetAllWithoutTracking().Any(x => x.PageLinkId == pageLinkId);*/
            var isExisting = 
                _sellFlatRepo.GetAllWithoutTracking().Any(x => x.PageLinkId == pageLinkId);

            return isExisting;
        }

        /// <summary>
        /// Проверяет, существует ли уже дом с таким адресом в базе данных.
        /// Если нет, то добавляет в БД с сохранением. Если да, то объекту присваивает Id существующего дома
        /// </summary>
        private bool updateOrAddHouse(N1HouseInfo house)
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
                try
                {
                    _houseRepo.Save(); // TODO: Починить
                    _stateLogger.LogHouseAddition(house.Id.Value, house.Address);
                    _writeToLogDelegate(string.Format("Добавлен дом: Id {0}, адрес {1}", house.Id, house.Address));
                }
                catch (Exception ex)
                {
                    _writeToLogDelegate(string.Format("Не удалось добавить дом с адресом {0}. Ошибка: {1}", house.Address, ex.Message));
                    return false;
                }
            }
            return true;
        }

        private void updateOrAddSellFlat(N1Flat sellFlat, int houseId, int pageLinkId, int? agencyId)
        {
            sellFlat.N1HouseInfoId = houseId;
            sellFlat.PageLinkId = pageLinkId;
            sellFlat.LastCheckDate = DateTime.Now;

            if (agencyId.HasValue)
                sellFlat.N1AgencyId = agencyId;

            N1Flat existingFlat = _sellFlatRepo.GetAllWithoutTracking()
                    .Where(x => x.PageLinkId == pageLinkId)
                    .FirstOrDefault();

            if (existingFlat != null)
                return;

            _sellFlatRepo.Add(sellFlat);
            _sellFlatRepo.Save();

            _stateLogger.LogApartmentAddition(sellFlat.Id.Value, sellFlat.N1HouseInfoId.Value, false);
            _writeToLogDelegate(string.Format("Добавлена квартира: Id {0}, Id дома {1}, Id ссылки {2}, Аренда=false",
                sellFlat.Id, houseId, pageLinkId));
        }
    }
}
