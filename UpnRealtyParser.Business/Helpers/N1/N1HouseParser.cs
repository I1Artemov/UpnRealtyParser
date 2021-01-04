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

            fillBuildYear(house, pageHtmlDoc);
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
            string streetHouseStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_address")?.TextContent;

            if (string.IsNullOrEmpty(streetHouseStr))
                return;

            string cityStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_city")?.TextContent;

            if (!string.IsNullOrEmpty(cityStr))
                streetHouseStr = cityStr + ", " + streetHouseStr;

            house.Address = streetHouseStr;
        }

        private void fillWallMaterialFromSingleApartmentCard(N1HouseInfo house, IElement flatCard)
        {
            string wallMaterialStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_floor")?.TextContent;
            if (string.IsNullOrEmpty(wallMaterialStr))
                return;

            int spaceIndex = wallMaterialStr.LastIndexOf((char)160);

            if (spaceIndex <= 0)
                return;
            wallMaterialStr = wallMaterialStr.Substring(spaceIndex + 1, wallMaterialStr.Length - spaceIndex - 1);

            switch (wallMaterialStr)
            {
                case "п": house.WallMaterial = "Панельный"; break;
                case "км": house.WallMaterial = "Кирпич-монолит"; break;
                case "бтб": house.WallMaterial = "Бетонные блоки"; break;
                case "м": house.WallMaterial = "Монолитный"; break;
                case "к": house.WallMaterial = "Кирпичный"; break;
                case "д": house.WallMaterial = "Деревянный"; break;
                case "ш": house.WallMaterial = "Шлакоблочный"; break;
                case "др": house.WallMaterial = "Другой"; break;
                default: house.WallMaterial = wallMaterialStr; break;
            }
        }

        private void fillMaxFloorFromSingleApartmentCard(N1HouseInfo house, IElement flatCard)
        {
            string allFloorStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_floor span")?.TextContent;

            if (string.IsNullOrEmpty(allFloorStr))
                return;

            int slashIndex = allFloorStr.IndexOf('/');
            if (slashIndex <= 0)
                return;

            int spaceIndex = allFloorStr.LastIndexOf((char)160);
            string lastFloorStr = allFloorStr.Substring(slashIndex + 2, spaceIndex - slashIndex - 2);

            bool isParsed = int.TryParse(lastFloorStr, out int maxFloor);
            if (isParsed)
                house.MaxFloor = maxFloor;
        }

        private void fillBuildYear(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string buildYearStr = getValueFromLivingContentParamsList("Год постройки", pageHtmlDoc);
            if (string.IsNullOrEmpty(buildYearStr))
                return;

            buildYearStr = buildYearStr.Substring(0, 4);
            bool isParsed = int.TryParse(buildYearStr, out int buildYear);

            if (isParsed)
                house.BuildYear = buildYear;
        }

        private void fillHouseType(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string houseTypeStr = getValueFromLivingContentParamsList("Тип дома", pageHtmlDoc);
            if (string.IsNullOrEmpty(houseTypeStr))
                return;

            house.HouseType = houseTypeStr;
        }

        private void fillBuilderCompany(N1HouseInfo house, IDocument pageHtmlDoc)
        {
            string builderCompanyText = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "p" &&
                     m.ClassName == "card-living-content-declaration")?.InnerHtml;

            if(string.IsNullOrEmpty(builderCompanyText))
                return;

            int startIndex = "Застройщик -".Length;
            int endIndex = builderCompanyText.IndexOf(". ");

            if (startIndex < endIndex)
                builderCompanyText = builderCompanyText.Substring(startIndex + 1, endIndex - startIndex - 1);

            house.BuilderCompany = builderCompanyText;
        }

        private void fillLatitudeAndLongitude(N1HouseInfo house, string webPageText)
        {
            //string template = "141992221\",\"longitude\":\"60.629808407400000\",\"latitude\":\"56.799732576900000\""
            int endIndex = webPageText.IndexOf(",\"utm_source\":\"realty_similar_card\"");
            int startIndex = endIndex - 75;

            if (startIndex <= 0 || endIndex <= 0)
                return;

            string latLonPartText = webPageText.Substring(startIndex, endIndex - startIndex);

            int lonStartIndex = latLonPartText.IndexOf("longitude");
            int lonEndIndex = latLonPartText.IndexOf("\",\"lati");
            if (lonStartIndex < lonEndIndex)
            {
                string longitudeStr = latLonPartText.Substring(lonStartIndex + "longitude\":\"".Length, lonEndIndex - lonStartIndex - "longitude\":\"".Length);
                bool isParsedLon = double.TryParse(longitudeStr.Replace('.', ','), out double longitude);
                if (isParsedLon)
                    house.Longitude = longitude;
            }

            int latStartIndex = latLonPartText.IndexOf("latitude\":\"");
            int latEndIndex = latLonPartText.Length - 1;
            if (latStartIndex < latEndIndex)
            {
                string latitudeStr = latLonPartText.Substring(latStartIndex + "latitude\":\"".Length, latEndIndex - latStartIndex - "latitude\":\"".Length);
                bool isParsedLat = double.TryParse(latitudeStr.Replace('.', ','), out double latitude);
                if (isParsedLat)
                    house.Latitude = latitude;
            }
        }
    }
}