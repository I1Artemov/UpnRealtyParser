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

        /// <summary>
        /// Не актуально! Сбор ссвлок при типе отображения "таблица"
        /// </summary>
        [Fact]
        public void GetSellFlatsLinksList_AsTable_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "01_SampleN1FlatsTable.txt", "utf-8");
            N1FlatLinksCollector linksCollector = new N1FlatLinksCollector();

            List<string> hrefs = linksCollector.GetLinksFromSinglePage(webPageText);
            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(webPageText);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));

            Assert.Equal(100, hrefs.Count);
            Assert.Equal(11686, totalApartmentsAmount);
            Assert.Equal(117, totalTablePages);
        }

        /// <summary>
        /// Сбор ссылок при типе отображения "Список"
        /// </summary>
        [Fact]
        public void GetSellFlatsLinksList_AsCards_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "03_SampleN1FlatsListCards.txt", "utf-8");
            N1FlatLinksCollector linksCollector = new N1FlatLinksCollector();

            List<string> hrefs = linksCollector.GetLinksFromSinglePage(webPageText);
            int? totalApartmentsAmount = linksCollector.GetTotalEntriesInTable(webPageText);
            int totalTablePages = linksCollector.GetMaxPagesInTable(totalApartmentsAmount.GetValueOrDefault(0));

            Assert.Equal(100, hrefs.Count);
            Assert.Equal(11581, totalApartmentsAmount);
            Assert.Equal(116, totalTablePages);
        }

        [Fact]
        public void SingleFlat_HouseFilling_Test()
        {
            string webPageText = getTextFromFile(TestDataPath, "02_SampleN1SingleFlatView.txt", "utf-8");

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
            string webPageText = getTextFromFile(TestDataPath, "03_SampleN1FlatsListCards.txt", "utf-8");
            N1ApartmentParser flatParser = new N1ApartmentParser();
            List<N1FlatBase> flats = flatParser.GetN1ApartmentsFromTablePage(webPageText);

            Assert.NotEmpty(flats);
        }
    }
}