using AngleSharp.Dom;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnSiteAgent
    {
        /// <summary>
        /// Все, что касается сбора ссылок на квартиры и их сохранения в базу данных.
        /// Заходит на главную страницу с переченм квартир, получает общее кол-во квартир,
        /// собирает из таблицы все ссылки на просмотр страниц с описанием квартир
        /// </summary>
        public string GatherLinksAndInsertInDb()
        {
            const string mainTableUrl = "https://upn.ru/realty_eburg_flat_sale.htm";

            string firstTablePageHtml;
            using (WebClient client = createWebClient())
            {
                firstTablePageHtml = client.DownloadString(mainTableUrl);
            }
            if (string.IsNullOrEmpty(firstTablePageHtml))
                return "Не удалось загрузить главную страницу";

            UpnFlatLinksCollector linksCollector = new UpnFlatLinksCollector();

            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(firstTablePageHtml);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(firstTablePageHtml);

            List<string> allHrefs = new List<string>(totalApartmentsAmount.Value);
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
                allHrefs.AddRange(currentTablePageHrefs);
            }
            return null;
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
