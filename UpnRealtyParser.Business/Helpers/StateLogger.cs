using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class StateLogger
    {
        protected EFGenericRepo<ParsingState, RealtyParserContext> _parsingStateRepo;

        public StateLogger(RealtyParserContext context)
        {
            _parsingStateRepo = new EFGenericRepo<ParsingState, RealtyParserContext>(context);
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
        public void LogFirstPageLoading(int recordsAmount, int pagesAmount)
        {
            LogAnyMessage(Const.SiteNameUpn,
                Const.ParsingStatusGettingRowsAndPagesAmount,
                string.Format("Records={0} Pages={1}", recordsAmount, pagesAmount),
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// Не удалось загрузить данные по кол-ву страниц и строк с 1-ой страницы при сборе ссылок по причине...
        /// </summary>
        public void LogFirstPageLoadingFailure(string reason)
        {
            LogAnyMessage(Const.SiteNameUpn,
                Const.ParsingStatusGettingRowsAndPagesAmount,
                reason,
                Const.StatusTypeFailure);
        }

        /// <summary>
        /// "Сбор ссылок завершен"
        /// </summary>
        public void LogLinksGatheringCompletion()
        {
            LogAnyMessage(Const.SiteNameUpn,
                Const.ParsingStatusDescriptionGatheringLinks,
                "Completed",
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// При сборе ссылок после обработки каждой страницы. "Обработана страница {0}: вставлено {1} записей, обновлено {2}."
        /// </summary>
        public void LogLinksPageProcessingResult(int pageNumber, int addedRecordsNumber, int updatedRecordsNumber)
        {
            string detailsMessage = string.Format("PageNumber={0} AddedRows={1} UpdatedRows={2}",
                pageNumber, addedRecordsNumber, updatedRecordsNumber);

            LogAnyMessage(Const.SiteNameUpn,
                Const.ParsingStatusMainTableSinglePageProcessing,
                detailsMessage,
                Const.StatusTypeSuccess);
        }

        /// <summary>
        /// Если не удалось скачать очередную страницу при загрузке ссылок
        /// </summary>
        public void LogLinksPageLoadingFailure(int pageNumber)
        {
            LogAnyMessage(Const.SiteNameUpn,
                Const.ParsingStatusMainTableSinglePageProcessing,
                string.Format("PageNumber={0}", pageNumber),
                Const.StatusTypeFailure);
        }
    }
}
