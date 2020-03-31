using System;
using System.Collections.Generic;
using AngleSharp.Dom;
using UpnRealtyParser.Business.Helpers;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class SampleWebPageParsingTest : BaseWebParsingTest
    {
        private const string WorkingSellFlatUrl = "https://upn.ru/realty_eburg_flat_sale_info/20003330-2841.htm";
        private const string TestDataPath = "\\UpnRealtyParser.Tests\\TestData\\01_WebPages";

        [Fact]
        public void SingleFlatFillingTest()
        {
            string webPageText = getTextFromFile(TestDataPath, "01_RealFlatForSell2Rooms_webpage.txt");

            UpnApartmentParser parser = new UpnApartmentParser();
            List<IElement> fieldValueElements = parser.GetTdElementsFromWebPage(webPageText);
            UpnFlat upnFlat = parser.GetUpnSellFlatFromPageText(fieldValueElements);

            Assert.Equal(2, upnFlat.RoomAmount);
            Assert.Equal(42, (int)Math.Floor(upnFlat.SpaceSum.Value));
            Assert.Equal(31, upnFlat.SpaceLiving);
            Assert.Equal(6, upnFlat.SpaceKitchen);
            Assert.Equal(1, upnFlat.FlatFloor);
            Assert.Equal(2, upnFlat.JointBathrooms);
            Assert.Null(upnFlat.SeparateBathrooms);
            Assert.Equal("���", upnFlat.Furniture);
            Assert.Equal("������ �������", upnFlat.SellCondition);
            Assert.Equal(1690000, upnFlat.Price);
            Assert.StartsWith("��������� ������������� �������� �� ������ �����", upnFlat.Description);
        }

        [Fact]
        public void SingleFlat_HouseFilling_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "01_RealFlatForSell2Rooms_webpage.txt");

            UpnHouseParser houseParser = new UpnHouseParser();
            List<IElement> fieldValueElements = houseParser.GetTdElementsFromWebPage(webPageText);
            UpnHouseInfo house = houseParser.GetUpnHouseFromPageText(fieldValueElements, webPageText);

            Assert.Equal("������������, ������, ���������� 26", house.Address);
            Assert.Equal("��������", house.HouseType);
            Assert.Equal("������", house.WallMaterial);
            Assert.Equal(1964, house.BuildYear);
            Assert.Equal(2, house.MaxFloor);

            Assert.Equal(56, (int)Math.Floor(house.Latitude.Value));
            Assert.Equal(60, (int)Math.Floor(house.Longitude.Value));
        }

        [Fact]
        public void GetSellFlatsLinksList_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "02_FlatsSellTable.txt");
            UpnFlatLinksCollector linksCollector = new UpnFlatLinksCollector();

            List<string> hrefs = linksCollector.GetLinksFromSinglePage(webPageText);
            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(webPageText);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));
            string pageUrlTemplate = linksCollector.GetTablePageSwitchLinkTemplate(webPageText);

            Assert.Equal(31, hrefs.Count);
            Assert.Equal(8384, totalApartmentsAmount);
            Assert.Equal(280, totalTablePages);
            Assert.EndsWith("sid=5f94fedb5e9a42b9b9aef2fa1db71986&ag=0&vm=1&id={0}&scn=6", pageUrlTemplate);
        }

        [Fact(Skip = "�������� � ��������� �������")]
        //[Fact]
        public void UpnSiteAgent_LinksGatheringTest()
        {
            UpnSiteAgent upnAgent = new UpnSiteAgent(null, null, 0);
            upnAgent.GatherLinksAndInsertInDb();
        }

        [Fact]
        public void UpnSiteAgent_ApartmentGatheringTest()
        {
            UpnSiteAgent upnAgent = new UpnSiteAgent(null, null, 0);
            List<string> apartmentHrefs = new List<string> {
                "/realty_eburg_flat_sale_info/30125886-2171.htm",
                "/realty_eburg_flat_sale_info/20000573-2851.htm"
            };
            string errorMessage = upnAgent.ProcessAllApartmentsFromLinksInDb(apartmentHrefs, true);
        }
    }
}
