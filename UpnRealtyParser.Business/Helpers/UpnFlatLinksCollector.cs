using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnFlatLinksCollector : BaseHttpParser
    {
        /// <summary>
        /// Количество записей на страницу таблицы с перечнем квартир
        /// </summary>
        public const int EntriesPerTablePage = 30;

        /// <summary>
        /// Всего записей в таблице с перечнем квартир
        /// </summary>
        public int? GetTotalEntriesInTable(string pageText)
        {
            var htmlDocument = getPreparedHtmlDocument(pageText);
            string totalEntriesText = htmlDocument.Result.All
                .Where(m => m.LocalName == "div" && 
                            m.ClassName == "middle" &&
                            m.InnerHtml != null && m.InnerHtml.Contains("Объекты"))
                .Select(m => m.InnerHtml)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(totalEntriesText))
                return 0;

            int pFrom = totalEntriesText.IndexOf("из ") + "из ".Length;
            if (pFrom < 0) return null;

            string totalValueStr = totalEntriesText.Substring(pFrom);
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
            List<IElement> anchorElements = getApartmentAnchorElementsFromWebPage(pageText, isRentFlats);

            List<string> hrefs = anchorElements
                .Select(x => x.Attributes.GetNamedItem("href").Value)
                .ToList();

            return hrefs;
        }

        protected List<IElement> getApartmentAnchorElementsFromWebPage(string pageText, bool isRentFlats)
        {
            string saleOrRentText = isRentFlats ? "rent" : "sale";

            Task<IDocument> htmlDocument = getPreparedHtmlDocument(pageText);
            List<IElement> anchorElements = htmlDocument.Result.All
                .Where(m => m.LocalName == "a" &&
                            m.Attributes.GetNamedItem("href")?.Value != null &&
                            m.Attributes.GetNamedItem("href").Value
                                .Contains(string.Format("realty_eburg_flat_{0}_info", saleOrRentText)))
                .ToList();

            return anchorElements;
        }

        /// <summary>
        /// Со страницы с перечнем квартир берет ссылку на 1-ую страницу таблицы и в ней id=1 заменяет на id={0}
        /// </summary>
        public string GetTablePageSwitchLinkTemplate(string pageText, bool isRentFlats = false)
        {
            string rentOrSaleText = isRentFlats ? "rent" : "sale";

            Task<IDocument> htmlDocument = getPreparedHtmlDocument(pageText);
            string firstPageHref = htmlDocument.Result.All
                .Where(m => m.LocalName == "a" &&
                    m.Attributes.GetNamedItem("href")?.Value != null &&
                    m.Attributes.GetNamedItem("href").Value.Contains(
                        string.Format("index.aspx?page=realty_eburg_flat_{0}&ofdays=&sid=", rentOrSaleText)))
                .Select(x => x.Attributes.GetNamedItem("href").Value)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(firstPageHref))
                return null;

            int sidFrom = firstPageHref.IndexOf("&sid=") + "&sid=".Length;
            int sidTo = firstPageHref.LastIndexOf("&ag=");

            string sidValue = firstPageHref.Substring(sidFrom, sidTo - sidFrom);

            string pageSwitchUrlTemplate = 
                "https://upn.ru/index.aspx?page=realty_eburg_flat_" + rentOrSaleText + "&ofdays=&sid=" + sidValue + "&ag=0&vm=1&id={0}&scn=6";
            return pageSwitchUrlTemplate;
        }
    }
}