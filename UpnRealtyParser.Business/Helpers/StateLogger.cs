using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class StateLogger
    {
        protected EFGenericRepo<ParsingState, RealtyParserContext> _parsingStateRepo;
        protected string _siteName;

        public StateLogger(RealtyParserContext context, string siteName)
        {
            _parsingStateRepo = new EFGenericRepo<ParsingState, RealtyParserContext>(context);
            _siteName = siteName;
        }

        public void LogAnyMessage(string siteName, string description, string details, string status, bool isSaveChanges = true)
        {
            ParsingState entry = new ParsingState
            {
                SiteName = siteName,
                Description = description,
                Details = details,
                Status = status
            };

            _parsingStateRepo.Add(entry);

            if(isSaveChanges)
                _parsingStateRepo.Save();
        }

        /// <summary>
        /// При загруке информации с первой страницы при сборе ссылок. "Всего {0} записей на {1} страницах таблицы"
        /// </summary>
        public void LogFirstPageLoading(int recordsAmount, int pagesAmount, bool isRentFlats)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusGettingRowsAndPagesAmount,
                string.Format("Records={0} Pages={1} Rent={2}", recordsAmount, pagesAmount, isRentFlats),
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// Не удалось загрузить данные по кол-ву страниц и строк с 1-ой страницы при сборе ссылок по причине...
        /// </summary>
        public void LogFirstPageLoadingFailure(string reason)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusGettingRowsAndPagesAmount,
                reason,
                Const.StatusTypeFailure);
        }

        /// <summary>
        /// "Сбор ссылок завершен"
        /// </summary>
        public void LogLinksGatheringCompletion(bool isRentFlats)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusDescriptionGatheringLinks,
                string.Format("Completed Rent={0}", isRentFlats),
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// При сборе ссылок после обработки каждой страницы. "Обработана страница {0}: вставлено {1} записей, обновлено {2}."
        /// </summary>
        public void LogLinksPageProcessingResult(int pageNumber, int addedRecordsNumber, int updatedRecordsNumber)
        {
            string detailsMessage = string.Format("PageNumber={0} AddedRows={1} UpdatedRows={2}",
                pageNumber, addedRecordsNumber, updatedRecordsNumber);

            LogAnyMessage(_siteName,
                Const.ParsingStatusMainTableSinglePageProcessing,
                detailsMessage,
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// Если не удалось скачать очередную страницу при загрузке ссылок
        /// </summary>
        public void LogLinksPageLoadingFailure(int pageNumber, bool isRentFlats)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusMainTableSinglePageProcessing,
                string.Format("PageNumber={0} Rent={1}", pageNumber, isRentFlats),
                Const.StatusTypeFailure);
        }

        /// <summary>
        /// При начале обработки квартир (указываем число новых ссылок для обработки)
        /// </summary>
        public void LogApartmentsParsingStart(int apartmentsAmount, bool isRentFlats)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusDescriptionObservingFlats,
                string.Format("Started LinksToProcess={0} Rent={1}", apartmentsAmount, isRentFlats),
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// Если квартира по ссылке не обнаружена (объявление снято / продана)
        /// </summary>
        public void LogApartmentParsingNotFoundOnSite(string pageHref, bool isRentFlats)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusDescriptionObservingFlats,
                string.Format("NotFound Href={0} Rent={1}", pageHref, isRentFlats),
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// Если страница с информацией о квартире не была загружена по какой-то причине (например, слишком много попыток)
        /// </summary>
        public void LogApartmentPageLoadingProplem(string pageHref, bool isRentFlats)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusDescriptionObservingFlats,
                string.Format("NotLoaded Href={0} Rent={1}", pageHref, isRentFlats),
                Const.StatusTypeFailure);
        }

        public void LogApartmentMarkedAsDead(string pageHref)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusProcessingSingleFlat,
                string.Format("MarkedAsDead Href={0}", pageHref),
                Const.StatusTypeSuccess);
        }

        public void LogApartmentMarkedAsDeadError(string pageHref)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusProcessingSingleFlat,
                string.Format("ErrorMarkedAsDead Href={0}", pageHref),
                Const.StatusTypeFailure);
        }

        /// <summary>
        /// Если не удалось обработать информацию о доме со страницы с квартирой
        /// </summary>
        public void LogErrorProcessingHouse(string pageHref)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusProcessingHouse,
                string.Format("NotProcessed Href={0}", pageHref),
                Const.StatusTypeFailure);
        }

        /// <summary>
        /// При добавлении дома
        /// </summary>
        public void LogHouseAddition(int id, string address)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusProcessingHouse,
                string.Format("Added Id={0} Address={1}", id, address),
                Const.StatusTypeSuccess);
        }

        public void LogApartmentAddition(int id, int houseId, bool isRentFlat)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusProcessingSingleFlat,
                string.Format("Added Id={0} HouseId={1} Rent={2}", id, houseId, isRentFlat),
                Const.StatusTypeSuccess);
        }

        public void LogApartmentRefilling(int id, int houseId, bool isRentFlat)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusProcessingSingleFlat,
                string.Format("Refilled Id={0} HouseId={1} Rent={2}", id, houseId, isRentFlat),
                Const.StatusTypeSuccess);
        }

        public void LogAgencyAddition(int id)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusProcessingAgency,
                string.Format("Added Id={0}", id),
                Const.StatusTypeSuccess);
        }

        public void LogApartmentPhotoHrefsAddition(int apartmentId, int linksAmount)
        {
            LogAnyMessage(_siteName,
                Const.ParsingStatusGettingApartmentPhotoLinks,
                string.Format("Added for Id={0} Amount={1}", apartmentId, linksAmount),
                Const.StatusTypeSuccess);
        }

        public void LogProxiesDownloadError(string siteName, string errorMessage)
        {
            LogAnyMessage(siteName,
                Const.ParsingStatusGettingProxies,
                string.Format("No proxies get, ", errorMessage),
                Const.StatusTypeFailure);
        }
    }
}
