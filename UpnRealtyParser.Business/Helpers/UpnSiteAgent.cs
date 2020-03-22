using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnSiteAgent
    {
        /// <summary>
        /// Все, что касается сбора ссылок на квартиры и их сохранения в базу данных
        /// </summary>
        public void GatherLinksAndInsertInDb()
        {
            const string mainTableUrl = "https://upn.ru/realty_eburg_flat_sale.htm";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string firstTablePageHtml;
            using (WebClient client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") })
            {
                firstTablePageHtml = client.DownloadString(mainTableUrl);
            }

            UpnFlatLinksCollector linksCollector = new UpnFlatLinksCollector();

            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(firstTablePageHtml);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(firstTablePageHtml);

            List<string> allHrefs = new List<string>(totalApartmentsAmount.Value);
            for(int currentPageNumber = 1; currentPageNumber <= totalTablePages; currentPageNumber++)
            {
                string currentTablePageUrl = string.Format(pageUrlTemplate, currentPageNumber);
                string currentTablePageHtml;
                using (WebClient client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") })
                {
                    currentTablePageHtml = client.DownloadString(currentTablePageUrl);
                }
                List<string> currentTablePageHrefs = linksCollector.GetLinksFromSinglePage(currentTablePageHtml);
                allHrefs.AddRange(currentTablePageHrefs);
            }
        }
    }
}
