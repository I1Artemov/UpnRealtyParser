using System.Collections.Generic;
using UpnRealtyParser.Business.Helpers;
using Xunit;

namespace UpnRealtyParser.Tests.TestData
{
    public class N1FromHtmlParsingTest : BaseWebParsingTest
    {
        private const string TestDataPath = "\\UpnRealtyParser.Tests\\TestData\\02_N1WebPages";

        [Fact]
        public void GetSellFlatsLinksList_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "01_SampleN1FlatsTable.txt", "utf-8");
            N1FlatLinksCollector linksCollector = new N1FlatLinksCollector();

            List<string> hrefs = linksCollector.GetLinksFromSinglePage(webPageText);
            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(webPageText);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            //string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(webPageText);

            Assert.Equal(100, hrefs.Count);
            Assert.Equal(11686, totalApartmentsAmount);
            Assert.Equal(117, totalTablePages);
            //Assert.EndsWith("sid=5f94fedb5e9a42b9b9aef2fa1db71986&ag=0&vm=1&id={0}&scn=6", pageUrlTemplate);
        }
    }
}