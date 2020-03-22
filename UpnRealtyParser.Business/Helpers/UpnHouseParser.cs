using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnHouseParser : BaseHttpParser
    {
        private const string HouseAddressHeaderText = "Адрес:";
        private const string HouseTypeHeaderText = "Дом";
        private const string BuildYearHeaderText = "Год постройки:";
        private const string WallMaterialHeaderText = "Материал стен:";
        private const string FloorComponentsHeaderText = "Этаж:";

        private int? addressIndex;
        private int? houseTypeIndex;
        private int? buildYearIndex;
        private int? wallMaterialIndex;
        private int? floorComponentsIndex;

        protected void fillFieldIndexes(List<IElement> tdElements)
        {
            addressIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", HouseAddressHeaderText)) + 1;
            houseTypeIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", HouseTypeHeaderText)) + 1;
            buildYearIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", BuildYearHeaderText)) + 1;
            wallMaterialIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", WallMaterialHeaderText)) + 1;
            floorComponentsIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", FloorComponentsHeaderText)) + 1;
        }

        public UpnHouseInfo GetUpnHouseFromPageText(List<IElement> fieldValueElements, string pageText)
        {
            const string beforeAddressPart = "<a class=\"showmap\" href=\"#map\">";
            const string afterAddressPart = "</a> <a class=\"showmap\" style=\"border-bottom: 1px dotted; text-transform: none;\"" +
                                            " href=\"#map\">(посмотреть на карте)</a>";

            UpnHouseInfo house = new UpnHouseInfo { CreationDateTime = DateTime.Now };
            fillFieldIndexes(fieldValueElements);

            if (addressIndex.HasValue && addressIndex != 0)
                house.Address = fieldValueElements.ElementAtOrDefault(addressIndex.Value)?.InnerHtml;

            // Очистка адреса от лишних тегов
            if (!string.IsNullOrEmpty(house.Address))
            {
                house.Address = house.Address.Replace(beforeAddressPart, "");
                house.Address = house.Address.Replace(afterAddressPart, "");
            }

            if (houseTypeIndex.HasValue && houseTypeIndex != 0)
                house.HouseType = fieldValueElements.ElementAtOrDefault(houseTypeIndex.Value)?.InnerHtml;
            if (wallMaterialIndex.HasValue)
                house.WallMaterial = fieldValueElements.ElementAtOrDefault(wallMaterialIndex.Value)?.InnerHtml;

            fillHouseBuildYear(house, fieldValueElements);
            fillMaxFloor(house, fieldValueElements);
            fillLatitudeAndLongitudeFromJs(house, pageText);

            return house;
        }

        private void fillHouseBuildYear(UpnHouseInfo house, List<IElement> fieldValueElements)
        {
            if (!buildYearIndex.HasValue || buildYearIndex == 0)
                return;

            string buildYearText = fieldValueElements.ElementAtOrDefault(buildYearIndex.Value)?.InnerHtml;
            if (string.IsNullOrEmpty(buildYearText))
                return;

            bool hasBuildYear = Int32.TryParse(buildYearText, out int buildYear);
            if (hasBuildYear) house.BuildYear = buildYear;
        }

        private void fillMaxFloor(UpnHouseInfo house, List<IElement> fieldValueElements)
        {
            if (!floorComponentsIndex.HasValue || floorComponentsIndex == 0)
                return;

            string floorsText = fieldValueElements.ElementAtOrDefault(floorComponentsIndex.Value)?.InnerHtml;
            if (string.IsNullOrEmpty(floorsText))
                return;

            List<string> floorParts = floorsText.Split("/").ToList();

            bool hasMaxFloor = Int32.TryParse(floorParts.ElementAtOrDefault(1), out int maxFloor);
            if (hasMaxFloor) house.MaxFloor = maxFloor;
        }

        /// <summary>
        /// Заполнение широты и долготы из текста встроенного джаваскрипта
        /// </summary>
        private void fillLatitudeAndLongitudeFromJs(UpnHouseInfo house, string pageText)
        {
            int pFrom = pageText.IndexOf("var point = [") + "var point = [".Length;
            int pTo = pageText.LastIndexOf("];\r\n\r\n        ymaps.ready");

            string latAndLonText = pageText.Substring(pFrom, pTo - pFrom);
            List<string> latAndLonParts = latAndLonText.Split(",").ToList();

            bool hasLatitude = double.TryParse(
                latAndLonParts.ElementAtOrDefault(0)?.Replace('.', ','), out double latitude);
            if (hasLatitude) { 
                house.Latitude = latitude;
            }

            bool hasLongitude = double.TryParse(
                latAndLonParts.ElementAtOrDefault(1)?.Replace('.', ','), out double longitude);
            if (hasLongitude)
            {
                house.Longitude = longitude;
            }
        }
    }
}