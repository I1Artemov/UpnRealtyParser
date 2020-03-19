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
                            m.GetAttribute("bis_skin_checked") != null)
                .Select(m => m.InnerHtml)
                .FirstOrDefault();

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
        public List<string> GetLinksFromSinglePage(string pageText)
        {
            List<IElement> anchorElements = GetApartmentAnchorElementsFromWebPage(pageText);

            List<string> hrefs = anchorElements
                .Select(x => x.Attributes.GetNamedItem("href").Value)
                .ToList();

            return hrefs;
        }

        /// <summary>
        /// Со страницы с перечнем квартир берет ссылку на 1-ую страницу таблицы и в ней id=1 заменяет на id={0}
        /// </summary>
        public string GetTablePageSwitchLinkTemplate(string pageText)
        {
            Task<IDocument> htmlDocument = getPreparedHtmlDocument(pageText);
            string firstPageHref = htmlDocument.Result.All
                .Where(m => m.LocalName == "a" &&
                    m.Attributes.GetNamedItem("href")?.Value != null &&
                    m.Attributes.GetNamedItem("href").Value.Contains("index.aspx?page=realty_eburg_flat_sale&ofdays=&sid="))
                .Select(x => x.Attributes.GetNamedItem("href").Value)
                .FirstOrDefault();

            string pageSwitchUrlTemplate = Utils.ReplaceStrBetweenTwoStrings(firstPageHref, "&id=", "&scn=", "&id={0}");
            pageSwitchUrlTemplate = pageSwitchUrlTemplate
                .Replace("index.aspx?page=realty_eburg_flat_sale&", "https://upn.ru/realty_eburg_flat_sale.htm?");
            return pageSwitchUrlTemplate;
        }
    }
}