using System;
using System.Collections.Generic;
using AngleSharp.Dom;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
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
            Assert.Equal("Нет", upnFlat.Furniture);
            //Assert.Equal("Чистая продажа", upnFlat.SellCondition);
            Assert.Equal(1690000, upnFlat.Price);
            Assert.StartsWith("Продается двухкомнатная квартира на первом этаже", upnFlat.Description);
        }

        [Fact]
        public void SingleFlat_HouseFilling_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "01_RealFlatForSell2Rooms_webpage.txt");

            UpnHouseParser houseParser = new UpnHouseParser();
            List<IElement> fieldValueElements = houseParser.GetTdElementsFromWebPage(webPageText);
            UpnHouseInfo house = houseParser.GetUpnHouseFromPageText(fieldValueElements, webPageText);

            Assert.Equal("Екатеринбург, Совхоз, Предельная 26", house.Address);
            Assert.Equal("Хрущевка", house.HouseType);
            Assert.Equal("Кирпич", house.WallMaterial);
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

        [Fact]
        public void GetAgencyFromPage_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "01_RealFlatForSell2Rooms_webpage.txt");
            UpnAgencyParser agencyParser = new UpnAgencyParser();

            List<IElement> fieldValueElements = agencyParser.GetTdElementsFromWebPage(webPageText);
            UpnAgency agency = agencyParser.GetAgencyFromPageText(fieldValueElements);

            Assert.Equal("пн.-пт.: с 10.00-21.00, сб.-вс.: с 11.00 до 18.00", agency.WorkTime);
            Assert.Equal("(343) 286-09-14", agency.CompanyPhone);
            Assert.Equal("89086306177", agency.AgentPhone);
            Assert.Equal("http://orientir.pro", agency.SiteUrl);
            Assert.Equal("info@orientir.pro", agency.Email);
        }

        [Fact]
        public void GettingApartmentPhotoHrefs_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "01_RealFlatForSell2Rooms_webpage.txt");

            UpnApartmentParser parser = new UpnApartmentParser();
            List<string> hrefs = parser.GetPhotoHrefsFromPage(webPageText);
            Assert.Equal(15, hrefs.Count);
        }

        [Fact(Skip = "Действия с реальными данными")]
        //[Fact]
        public void UpnSiteAgent_LinksGatheringTest()
        {
            AppSettings settings = new AppSettings(); // null, false, false, 0, 0, 1
            UpnSiteAgent upnAgent = new UpnSiteAgent(null, settings);
            upnAgent.GatherLinksAndInsertInDb();
        }

        [Fact(Skip = "Действия с реальными данными")]
        //[Fact]
        public void UpnSiteAgent_LinksGatheringTest_Rent()
        {
            AppSettings settings = new AppSettings(); // null, false, false, 0, 0, 1
            UpnSiteAgent upnAgent = new UpnSiteAgent(null, settings);
            upnAgent.GatherLinksAndInsertInDb(true);
        }

        [Fact(Skip = "Действия с реальными данными")]
        //[Fact]
        public void UpnSiteAgent_ApartmentGatheringTest()
        {
            AppSettings settings = new AppSettings(); // null, false, false, 0, 0, 1
            UpnSiteAgent upnAgent = new UpnSiteAgent(null, settings);
            List<PageLink> apartmentHrefs = new List<PageLink> {
                new PageLink{ Href = "/realty_eburg_flat_sale_info/30125886-2171.htm" } ,
                new PageLink{ Href = "/realty_eburg_flat_sale_info/20000573-2851.htm" }
            };
            upnAgent.ProcessAllApartmentsFromLinks(apartmentHrefs, true, false);
        }
    }
}
