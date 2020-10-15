using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1FlatLinksCollector : BaseHttpParser
    {
        /// <summary>
        /// Количество записей на страницу таблицы с перечнем квартир (устанавливается в запросе)
        /// </summary>
        public const int EntriesPerTablePage = 100;

        /// <summary>
        /// Всего записей в таблице с перечнем квартир
        /// </summary>
        public int? GetTotalEntriesInTable(string pageText)
        {
            var htmlDocument = getPreparedHtmlDocument(pageText);
            string totalEntriesText = htmlDocument.Result.All
                .Where(m => m.LocalName == "span" &&
                            m.InnerHtml != null && m.InnerHtml.Contains(" объявлений"))
                .Select(m => m.InnerHtml)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(totalEntriesText))
                return 0;

            int pLast = totalEntriesText.IndexOf(" объявлений");
            if (pLast < 0) return null;

            string totalValueStr = totalEntriesText.Substring(0, pLast);
            bool totalParsed = Int32.TryParse(totalValueStr, out int totalValue);
            if (!totalParsed) return null;

            return totalValue;
        }

        /// <summary>
        /// Общее число страниц таблицы с перечнем квартир
        /// </summary>
        public int GetMaxPagesInTable(int totalEntries) =>
            (int)Math.Ceiling((decimal)totalEntries / (decimal)EntriesPerTablePage);

        /// <summary>
        /// Собирает все URL на квартиры с текущей страницы таблицы с перечнем квартир
        /// </summary>
        public List<string> GetLinksFromSinglePage(string pageText, bool isRentFlats = false)
        {
            List<IElement> anchorElements = getApartmentAnchorElementsFromWebPage(pageText);

            List<string> hrefs = anchorElements
                .Select(x => x.Attributes.GetNamedItem("href").Value)
                .ToList();

            return hrefs;
        }

        protected List<IElement> getApartmentAnchorElementsFromWebPage(string pageText)
        {
            Task<IDocument> htmlDocument = getPreparedHtmlDocument(pageText);
            List<IElement> anchorElements = htmlDocument.Result.All
                .Where(m => m.LocalName == "a" &&
                            m.Attributes.GetNamedItem("href")?.Value != null &&
                            m.ClassName == "first-line__link" &&
                            m.Attributes.GetNamedItem("href").Value
                                .Contains("ekaterinburg.n1.ru/view/"))
                .ToList();

            return anchorElements;
        }

        public string GetTablePageSwitchLinkTemplate(string pageText, bool isRentFlats = false)
        {
            string rentOrSaleText = isRentFlats ? "snyat/dolgosrochno" : "kupit";

            return "https://ekaterinburg.n1.ru/" + rentOrSaleText + "/kvartiry/?limit=100&sort=-date&page={0}";
        }
    }
}