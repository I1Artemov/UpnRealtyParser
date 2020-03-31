using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnSiteAgent
    {
        protected EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        protected Action<string> _writeToLogDelegate;

        public UpnSiteAgent(Action<string> writeToLogDelegate)
        {
            _writeToLogDelegate = writeToLogDelegate;
        }

        public void InitializeRepositories(RealtyParserContext context)
        {
            _pageLinkRepo = new EFGenericRepo<PageLink, RealtyParserContext>(context);
        }

        /// <summary>
        /// Все, что касается сбора ссылок на квартиры и их сохранения в базу данных.
        /// Заходит на главную страницу с переченм квартир, получает общее кол-во квартир,
        /// собирает из таблицы все ссылки на просмотр страниц с описанием квартир
        /// </summary>
        public void GatherLinksAndInsertInDb()
        {
            const string mainTableUrl = "https://upn.ru/realty_eburg_flat_sale.htm";

            string firstTablePageHtml;
            using (WebClient client = createWebClient())
            {
                firstTablePageHtml = client.DownloadString(mainTableUrl);
            }
            if (string.IsNullOrEmpty(firstTablePageHtml))
            {
                if (_writeToLogDelegate != null) _writeToLogDelegate("Не удалось загрузить веб-страницу с перечнем квартир");
                return;
            }
            UpnFlatLinksCollector linksCollector = new UpnFlatLinksCollector();

            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(firstTablePageHtml);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(firstTablePageHtml);
            _writeToLogDelegate(string.Format("Всего {0} записей на {1} страницах таблицы", totalApartmentsAmount.Value, totalTablePages));

            for(int currentPageNumber = 1; currentPageNumber <= totalTablePages; currentPageNumber++)
            {
                string currentTablePageUrl = string.Format(pageUrlTemplate, currentPageNumber);
                string currentTablePageHtml;
                using (WebClient client = createWebClient())
                {
                    currentTablePageHtml = client.DownloadString(currentTablePageUrl);
                }

                if (string.IsNullOrEmpty(currentTablePageHtml))
                    continue;

                List<string> currentTablePageHrefs = linksCollector.GetLinksFromSinglePage(currentTablePageHtml);
                insertHrefsIntoDb(currentTablePageHrefs, Const.SiteNameUpn, currentPageNumber);
            }
            return;
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
                using (WebClient client = createWebClient())
                {
                    apartmentPageHtml = client.DownloadString(fullApartmentHref);
                }

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
            return new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
        }
    }
}
