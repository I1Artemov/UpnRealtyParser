using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1HouseParser : BaseHttpParser
    {
        public N1HouseInfo GetN1HouseFromPageText(string webPageText)
        {
            IDocument pageHtmlDoc = getPreparedHtmlDocument(webPageText).Result;
            return GetN1HouseFromPageText(pageHtmlDoc, webPageText);
        }

        public N1HouseInfo GetN1HouseFromPageText(IDocument pageHtmlDoc, string webPageText)
        {
            N1HouseInfo house = new N1HouseInfo();

            fillAddress(house, pageHtmlDoc);
            fillBuildYear(house, pageHtmlDoc);
            fillMaxFloor(house, pageHtmlDoc);
            fillWallMaterial(house, pageHtmlDoc);
            fillHouseType(house, pageHtmlDoc);
            fillBuilderCompany(house, pageHtmlDoc);
            fillLatitudeAndLongitude(house, webPageText);

            return house;
        }

        /// <summary>
        /// При предварительном заполнении дома из таблицы с квартирами
        /// </summary>
        public N1HouseInfo GetBasicN1HouseFromSingleApartmentCard(IElement flatCard)
        {
            N1HouseInfo house = new N1HouseInfo();

            fillAddressFromSingleApartmentCard(house, flatCard);
            fillWallMaterialFromSingleApartmentCard(house, flatCard);
            fillMaxFloorFromSingleApartmentCard(house, flatCard);

            return house;
        }

        private void fillAddressFromSingleApartmentCard(N1HouseInfo house, IElement flatCard)
        {
            string streetHouseStr = flatCard.QuerySelector(".living-list-card__location .link-text")?.InnerHtml;

            if (string.IsNullOrEmpty(streetHouseStr))
                return;

            streetHouseStr = streetHouseStr.Replace("1-к, ", "");
            streetHouseStr = streetHouseStr.Replace("2-к, ", "");
            streetHouseStr = streetHouseStr.Replace("3-к, ", "");
            streetHouseStr = streetHouseStr.Replace("4-к, ", "");
            streetHouseStr = streetHouseStr.Replace("5-к, ", "");

            string cityStr = flatCard.QuerySelector(".living-list-card-city-with-estate__item")?.TextContent;
            cityStr = cityStr.Replace(",", "");

            if (!string.IsNullOrEmpty(cityStr))
                streetHouseStr = cityStr + ", " + streetHouseStr;

            house.Address = streetHouseStr;
        }

        private void fillWallMaterialFromSingleApartmentCard(N1HouseInfo house, IElement flatCard)
        {
            string wallMaterialStr = flatCard.QuerySelector(".living-list-card__material")?.InnerHtml;

            if (!string.IsNullOrEmpty(wallMaterialStr))
                house.WallMaterial = wallMaterialStr;
        }

        private void fillMaxFloorFromSingleApartmentCard(N1HouseInfo house, IElement flatCard)
        {
            string allFloorStr = flatCard.QuerySelector(".living-list-card-floor__item")?.TextContent;

            if (string.IsNullOrEmpty(allFloorStr))
                return;

            int slashIndex = allFloorStr.IndexOf('/');
            if (slashIndex <= 0)
                return;

            int wordFloorIndex = allFloorStr.IndexOf("этаж");
            string lastFloorStr = allFloorStr.Substring(slashIndex + 2, wordFloorIndex - slashIndex - 2);

            bool isParsed = int.TryParse(lastFloorStr, out int maxFloor);
            if (isParsed)
                house.MaxFloor = maxFloor;
        }

        private void fillAddress(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string citySpan = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "a" &&
                     m.Attributes.GetNamedItem("data-v-7f258678")?.Value != null &&
                     m.Attributes.GetNamedItem("href")?.Value == "https://ekaterinburg.n1.ru/kupit/kvartiry/")?.InnerHtml;

            if (!string.IsNullOrEmpty(citySpan))
            {
                int startIndex = citySpan.IndexOf("\">");
                int endIndex = citySpan.IndexOf("</span>");
                if (startIndex < endIndex)
                    citySpan = citySpan.Substring(startIndex + 2, endIndex - startIndex - 2);
            }

            string street = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "div" && m.ClassName == "address")?.InnerHtml;

            if (!string.IsNullOrEmpty(street))
                street = street.Replace(", ", "");

            string houseNumber = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "div" && m.ClassName == "house-number")?.InnerHtml;

            if (!string.IsNullOrEmpty(houseNumber))
                houseNumber = houseNumber.Replace(", ", "");

            if (string.IsNullOrEmpty(citySpan))
                citySpan = "Екатеринбург"; // Город не заполняется у квартир в архиве

            List<string> addressElements = new List<string> { citySpan, street, houseNumber };

            if (!string.IsNullOrEmpty(citySpan) || !string.IsNullOrEmpty(street) || !string.IsNullOrEmpty(houseNumber))
                house.Address = string.Join(", ", addressElements);
        }

        private void fillBuildYear(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string buildYearStr = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "span" &&
                     m.ClassName == "card-living-content-params-list__value" &&
                     m.InnerHtml != null && m.InnerHtml.Contains(" г."))?.InnerHtml;

            if (string.IsNullOrEmpty(buildYearStr))
                return;

            buildYearStr = buildYearStr.Substring(0, 4);
            bool isParsed = int.TryParse(buildYearStr, out int buildYear);

            if (isParsed)
                house.BuildYear = buildYear;
        }

        private void fillMaxFloor(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string floorsStr = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "span" &&
                     m.ClassName == "card-living-content-params-list__value" &&
                     m.InnerHtml != null && m.InnerHtml.Contains(" из "))?.InnerHtml;

            if (string.IsNullOrEmpty(floorsStr))
                return;

            int startIndex = floorsStr.IndexOf("из ");
            floorsStr = floorsStr.Substring(startIndex + 3, floorsStr.Length - startIndex - 3);
            bool isParsed = int.TryParse(floorsStr, out int maxFloor);

            if (isParsed)
                house.MaxFloor = maxFloor;
        }

        private void fillWallMaterial(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string wallStr = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "span" &&
                     m.Attributes.GetNamedItem("data-test")?.Value == "offer-card-param-house-material-type")?.InnerHtml;

            if (string.IsNullOrEmpty(wallStr))
                return;

            house.WallMaterial = wallStr;
        }

        private void fillHouseType(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            var houseTypeBlock = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "li" &&
                     m.ClassName == "card-living-content-params-list__item" &&
                     m.InnerHtml != null && m.InnerHtml.Contains("Тип дома"));

            string houseTypeStr = houseTypeBlock?.LastChild?.TextContent;

            if (string.IsNullOrEmpty(houseTypeStr))
                return;

            house.HouseType = houseTypeStr;

        }

        private void fillBuilderCompany(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string builderCompanyText = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "p" &&
                     m.ClassName == "card-living-content-declaration")?.InnerHtml;

            if (string.IsNullOrEmpty(builderCompanyText))
                return;

            int startIndex = "Застройщик -".Length;
            int endIndex = builderCompanyText.IndexOf(". ");

            if (startIndex < endIndex)
                builderCompanyText = builderCompanyText.Substring(startIndex + 1, endIndex - startIndex - 1);

            house.BuilderCompany = builderCompanyText;
        }

        private void fillLatitudeAndLongitude(N1HouseInfo house, string webPageText)
        {
            //string template = "location\":{\"latitude\":56.795919,\"longtitude\":60.765082,\"precision\":\"exact\"}";
            int endIndex = webPageText.IndexOf(",\"precision\":\"exact\"}");
            int startIndex = endIndex - 75;

            if (startIndex <= 0 || endIndex <= 0)
                return;

            string latLonPartText = webPageText.Substring(startIndex, endIndex - startIndex);

            int latStartIndex = latLonPartText.IndexOf("latitude");
            int latEndIndex = latLonPartText.IndexOf(",\"long");
            if (latStartIndex < latEndIndex)
            {
                string latitudeStr = latLonPartText.Substring(latStartIndex + "latitude\":".Length, latEndIndex - latStartIndex - "latitude\":".Length);
                bool isParsedLat = double.TryParse(latitudeStr.Replace('.', ','), out double latitude);
                if (isParsedLat)
                    house.Latitude = latitude;
            }

            int lonStartIndex = latLonPartText.IndexOf("longtitude");
            int lonEndIndex = latLonPartText.Length;
            if (lonStartIndex < lonEndIndex)
            {
                string longitudeStr = latLonPartText.Substring(lonStartIndex + "longtitude\":".Length, lonEndIndex - lonStartIndex - "longtitude\":".Length);
                bool isParsedLon = double.TryParse(longitudeStr.Replace('.', ','), out double longitude);
                if (isParsedLon)
                    house.Longitude = longitude;
            }
        }
    }
}