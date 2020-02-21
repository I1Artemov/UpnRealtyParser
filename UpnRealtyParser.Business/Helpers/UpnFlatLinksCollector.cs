using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnFlatLinksCollector : BaseHttpParser
    {
        public const int EntriesPerTablePage = 30;

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

        public int GetMaxPagesInTable(int totalEntries) => 
            (int)Math.Ceiling((decimal)totalEntries / (decimal)EntriesPerTablePage);

        public List<string> GetLinksFromSinglePage(string pageText)
        {
            List<IElement> anchorElements = GetAnchorElementsFromWebPage(pageText);

            List<string> hrefs = anchorElements
                .Select(x => x.Attributes.GetNamedItem("href").Value)
                .ToList();

            return hrefs;
        }
    }
}