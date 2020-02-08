using System;
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
            UpnFlat upnFlat = parser.GetUpnSellFlatFromPageText(webPageText);

            Assert.Equal(2, upnFlat.RoomAmount);
            Assert.Equal(42, (int)Math.Floor(upnFlat.SpaceSum.Value));
            Assert.Equal(31, upnFlat.SpaceLiving);
            Assert.Equal(6, upnFlat.SpaceKitchen);
            Assert.Equal(1, upnFlat.FlatFloor);
            Assert.Equal(2, upnFlat.JointBathrooms);
            Assert.Null(upnFlat.SeparateBathrooms);
            Assert.Equal("Нет", upnFlat.Furniture);
            Assert.Equal("Чистая продажа", upnFlat.SellCondition);
            Assert.Equal(1690000, upnFlat.Price);
        }
    }
}
