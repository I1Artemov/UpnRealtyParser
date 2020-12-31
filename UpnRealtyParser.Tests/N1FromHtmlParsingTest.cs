using System;
using System.Collections.Generic;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using Xunit;

namespace UpnRealtyParser.Tests.TestData
{
    public class N1FromHtmlParsingTest : BaseWebParsingTest
    {
        private const string TestDataPath = "\\UpnRealtyParser.Tests\\TestData\\02_N1WebPages";

        [Fact]
        public void GetSellFlatsLinksList_AsTable_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "04_N1FlatsTable_OldVersion.txt", "utf-8");
            N1FlatLinksCollector linksCollector = new N1FlatLinksCollector();

            List<string> hrefs = linksCollector.GetLinksFromSinglePage(webPageText);
            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(webPageText);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));

            Assert.Equal(100, hrefs.Count);
            Assert.Equal(7397, totalApartmentsAmount);
            Assert.Equal(74, totalTablePages);
        }

        [Fact]
        public void SingleFlat_HouseFilling_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "05_N1Apartment_OldVersion.txt", "utf-8");

            N1HouseParser houseParser = new N1HouseParser();
            N1HouseInfo house = houseParser.GetN1HouseFromPageText(webPageText);

            Assert.Equal("Екатеринбург, Шевченко, 20", house.Address);
            Assert.Equal("спецпроект", house.HouseType);
            Assert.Equal("бетонные блоки", house.WallMaterial);
            Assert.Equal(2010, house.BuildYear);
            Assert.Equal(20, house.MaxFloor);
            Assert.Equal("Стройкапитал, ООО", house.BuilderCompany);

            Assert.Equal(56, (int)Math.Floor(house.Latitude.Value));
            Assert.Equal(60, (int)Math.Floor(house.Longitude.Value));
        }

        [Fact]
        public void GetBasicFilledFlats_FromTablePage_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "04_N1FlatsTable_OldVersion.txt", "utf-8");
            N1ApartmentParser flatParser = new N1ApartmentParser();
            N1HouseParser houseParser = new N1HouseParser();
            List<N1Flat> flats = flatParser.GetN1SellFlatsFromTablePage(webPageText, houseParser);

            Assert.NotEmpty(flats);
        }

	    [Fact]
        public void SingleFlat_FlatFilling_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "05_N1Apartment_OldVersion.txt", "utf-8");

            N1ApartmentParser flatParser = new N1ApartmentParser();
            N1FlatBase flat = flatParser.GetN1FlatFromPageText(webPageText);

            Assert.Equal("Комнаты изолированные", flat.PlanningType);
            Assert.Equal("Несколько", flat.BathroomType);
            Assert.Equal("В отличном состоянии", flat.Condition);
            Assert.Equal(1, flat.BalconyAmount);
            Assert.Contains("без дополнительных вложений", flat.Description);
        }

        [Fact]
        public void AgencyFilling_FromSingleFlatPage_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "02_SampleN1SingleFlatView.txt", "utf-8");

            N1AgencyParser agencyParser = new N1AgencyParser();
            N1Agency agency = agencyParser.GetN1AgencyFromPageText(webPageText);

            Assert.Equal("Ориентир.Недвижимость", agency.Name);
            Assert.Equal("Мария", agency.AgentName);
            Assert.Equal("79126009432", agency.AgentPhone);
            Assert.Equal("https://ekaterinburg.n1.ru/an/40191/", agency.SiteUrl);
            Assert.True(agency.IsCompany);
        }

        [Fact]
        public void N1_ApartmentPhotoHrefs_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "02_SampleN1SingleFlatView.txt", "utf-8");

            N1ApartmentParser parser = new N1ApartmentParser();
            List<string> hrefs = parser.GetPhotoHrefsFromPage(webPageText);
            Assert.Equal(27, hrefs.Count);
        }
    }
}