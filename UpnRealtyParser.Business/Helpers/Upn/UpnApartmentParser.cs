﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnApartmentParser : BaseHttpParser
    {
        private const string FlatTypeHeaderText = "Объект:";
        private const string RoomAmountHeaderText = "Количество комнат:";
        private const string SpaceComponentsHeaderText = "Площадь (общая/жилая/кухни):";
        private const string FloorComponentsHeaderText = "Этаж:";
        private const string BathroomComponentsHeaderText = "Сан. узлы (совмещенных/раздельных):";
        private const string RenovationHeaderText = "Ремонт:";
        private const string RedevelopmentHeaderIndex = "Перепланировка:";
        private const string WindowsTypeHeaderText = "Стеклопакеты:";
        private const string FurnitureHeaderText = "Мебель:";
        private const string SellConditionHeaderText = "Условия продажи:";
        private const string PriceHeaderText = "Цена:";
        private const string MinimalRentPeriodHeaderText = "Минимальный срок аренды:";

        private int? roomAmountIndex;
        private int? flatTypeIndex;
        private int? spaceComponentsIndex;
        private int? floorComponentsIndex;
        private int? bathroomComponentsIndex;
        private int? renovationTypeIndex;
        private int? redevelopmentTypeIndex;
        private int? windowsTypeIndex;
        private int? furnitureIndex;
        private int? sellConditionIndex;
        private int? priceIndex;
        private int? descriptionIndex;
        private int? minimalRentPeriodIndex;

        public void GetSingleSellFlatInfoFromUrl(string flatUrl)
        {
            IBrowsingContext context = BrowsingContext.New(Configuration.Default);
            var htmlDocument = context.OpenAsync(req => req.Content(
                "<ul><li>First item<li>Second item<li class='blue'>Third item!<li class='blue red'>Last item!</ul>"));

            var blueListItemsLinq = htmlDocument.Result.All
                .Where(m => m.LocalName == "li" && m.ClassList.Contains("blue"));
        }

        /// <summary>
        /// Находит строки в table с нужным текстом и определяет, строки с какими номерами дадут информацию для полей
        /// </summary>
        protected void fillFieldIndexes(List<IElement> tdElements, bool isRentFlats)
        {
            roomAmountIndex = tdElements.FindIndex(x => x.InnerHtml== string.Format("<b>{0}</b>", RoomAmountHeaderText)) + 1;
            flatTypeIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", FlatTypeHeaderText)) + 1;
            spaceComponentsIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", SpaceComponentsHeaderText)) + 1;
            floorComponentsIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", FloorComponentsHeaderText)) + 1;
            bathroomComponentsIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", BathroomComponentsHeaderText)) + 1;
            renovationTypeIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", RenovationHeaderText)) + 1;
            redevelopmentTypeIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", RedevelopmentHeaderIndex)) + 1;
            windowsTypeIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", WindowsTypeHeaderText)) + 1;
            furnitureIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", FurnitureHeaderText)) + 1;
            if(!isRentFlats)
                sellConditionIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", SellConditionHeaderText)) + 1;
            else
                minimalRentPeriodIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", MinimalRentPeriodHeaderText)) + 1;
            priceIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", PriceHeaderText)) + 1;
            if(priceIndex.HasValue && priceIndex > 0)
                descriptionIndex = priceIndex + 1;
        }

        public UpnFlat GetUpnSellFlatFromPageText(List<IElement> fieldValueElements)
        {
            UpnFlat upnSellFlat = new UpnFlat { CreationDateTime = DateTime.Now };
            fillAnyFlatFromPageTextAndGetValueElements(upnSellFlat, fieldValueElements, false);

            if (sellConditionIndex.HasValue && sellConditionIndex != 0)
                upnSellFlat.SellCondition = fieldValueElements.ElementAtOrDefault(sellConditionIndex.Value)?.InnerHtml;

            return upnSellFlat;
        }

        public UpnRentFlat GetUpnRentFlatFromPageText(List<IElement> fieldValueElements)
        {
            UpnRentFlat upnRentFlat = new UpnRentFlat { CreationDateTime = DateTime.Now };
            fillAnyFlatFromPageTextAndGetValueElements(upnRentFlat, fieldValueElements, true);

            if (minimalRentPeriodIndex.HasValue && minimalRentPeriodIndex != 0)
                upnRentFlat.MinimalRentPeriod = fieldValueElements.ElementAtOrDefault(minimalRentPeriodIndex.Value)?.InnerHtml;

            return upnRentFlat;
        }
        private void fillAnyFlatFromPageTextAndGetValueElements(UpnFlatBase upnFlat, List<IElement> fieldValueElements, bool isRentFlats)
        {
            fillFieldIndexes(fieldValueElements, isRentFlats);

            if(flatTypeIndex.HasValue && flatTypeIndex != 0)
            { 
                upnFlat.FlatType = fieldValueElements.ElementAtOrDefault(flatTypeIndex.Value)?.InnerHtml;
                upnFlat.FlatType = upnFlat.FlatType.Split("<").FirstOrDefault();
            }
            if (redevelopmentTypeIndex.HasValue && redevelopmentTypeIndex != 0)
                upnFlat.RedevelopmentType = fieldValueElements.ElementAtOrDefault(redevelopmentTypeIndex.Value)?.InnerHtml;
            if (renovationTypeIndex.HasValue && renovationTypeIndex != 0)
                upnFlat.RenovationType = fieldValueElements.ElementAtOrDefault(renovationTypeIndex.Value)?.InnerHtml;
            if (windowsTypeIndex.HasValue && windowsTypeIndex != 0)
                upnFlat.WindowsType = fieldValueElements.ElementAtOrDefault(windowsTypeIndex.Value)?.InnerHtml;
            if (furnitureIndex.HasValue && furnitureIndex != 0)
                upnFlat.Furniture = fieldValueElements.ElementAtOrDefault(furnitureIndex.Value)?.InnerHtml;

            if (roomAmountIndex.HasValue && roomAmountIndex != 0) { 
                bool hasRoomAmount = Int32.TryParse(fieldValueElements.ElementAtOrDefault(roomAmountIndex.Value)?.InnerHtml, out int roomAmount);
                if(hasRoomAmount) upnFlat.RoomAmount = roomAmount;
            }

            fillFloorNumber(upnFlat, fieldValueElements);
            fillFlatSpaceComponents(upnFlat, fieldValueElements);
            fillFlatBathroomComponents(upnFlat, fieldValueElements);
            fillPrice(upnFlat, fieldValueElements);
            fillAndClearDescription(upnFlat, fieldValueElements);
        }

        /// <summary>
        /// Выбирает из текста веб-страницы все ссылки на фотографии квартир. Возварщает список HREF-ов на jpg
        /// </summary>
        public List<string> GetPhotoHrefsFromPage(string pageText)
        {
            // meta property="og:image" content ="http://upn.ru/getpic.ashx?gid=488521d9-c076-4f7c-b903-ef8ebce1a05b&filename=2923_20000161_1_286974337.jpg&t=false"
            var htmlDocument = getPreparedHtmlDocument(pageText);
            var anchorElements = htmlDocument.Result.All
                .Where(m => m.LocalName == "meta" &&
                    m.OuterHtml.Contains("property=\"og:image\""))
                .Select(x => x as IHtmlMetaElement)
                .ToList();

            return anchorElements
                .Select(x => x.Content)
                .ToList();
        }

        /// <summary>
        /// Заполняет номер этажа квартиры. Записаны через дробь (этаж/всего)
        /// </summary>
        private void fillFloorNumber(UpnFlatBase upnFlat, List<IElement> fieldValueElements)
        {
            if (!floorComponentsIndex.HasValue || floorComponentsIndex == 0)
                return;

            string floorsText = fieldValueElements.ElementAtOrDefault(floorComponentsIndex.Value)?.InnerHtml;
            if (string.IsNullOrEmpty(floorsText))
                return;

            List<string> floorParts = floorsText.Split("/").ToList();

            bool hasFloor= Int32.TryParse(floorParts.ElementAtOrDefault(0), out int flatFloor);
            if (hasFloor) upnFlat.FlatFloor = flatFloor;
        }

        /// <summary>
        /// Заполнение полей с площадью (общая/жилая/кухни)
        /// </summary>
        private void fillFlatSpaceComponents(UpnFlatBase upnFlat, List<IElement> fieldValueElements)
        {
            if (!spaceComponentsIndex.HasValue || spaceComponentsIndex == 0)
                return;

            string spaceText = fieldValueElements.ElementAtOrDefault(spaceComponentsIndex.Value)?.InnerHtml;
            if (string.IsNullOrEmpty(spaceText))
                return;

            List<string> spaceParts = spaceText.Split("/").ToList();
            for (int i = 0; i < spaceParts.Count; i++)
                spaceParts[i] = spaceParts[i].Replace(".", ",");

            bool hasSpaceSum = double.TryParse(spaceParts.ElementAtOrDefault(0), out double spaceSum);
            if (hasSpaceSum) upnFlat.SpaceSum = spaceSum;

            bool hasSpaceLiving = double.TryParse(spaceParts.ElementAtOrDefault(1), out double spaceLiving);
            if (hasSpaceLiving) upnFlat.SpaceLiving = spaceLiving;

            string trimmedSpaceKitchen = spaceParts.ElementAtOrDefault(2);
            if (!string.IsNullOrEmpty(trimmedSpaceKitchen))
                trimmedSpaceKitchen = trimmedSpaceKitchen.Replace("кв, м", ""); // Запятая из-за замены

            bool hasSpaceKitchen = double.TryParse(trimmedSpaceKitchen, out double spaceKitchen);
            if (hasSpaceKitchen) upnFlat.SpaceKitchen = spaceKitchen;
        }

        /// <summary>
        /// Заполнение информации о санузлах (совмещенных/раздельных)
        /// </summary>
        private void fillFlatBathroomComponents(UpnFlatBase upnFlat, List<IElement> fieldValueElements)
        {
            if (!bathroomComponentsIndex.HasValue || bathroomComponentsIndex == 0)
                return;

            string bathroomsText = fieldValueElements.ElementAtOrDefault(bathroomComponentsIndex.Value)?.InnerHtml;
            if (string.IsNullOrEmpty(bathroomsText))
                return;
            List<string> bathroomParts = bathroomsText.Split("/").ToList();

            bool hasJointBaths = Int32.TryParse(bathroomParts.ElementAtOrDefault(0), out int jointBaths);
            if (hasJointBaths) upnFlat.JointBathrooms = jointBaths;
            bool hasSeparateBaths = Int32.TryParse(bathroomParts.ElementAtOrDefault(1), out int separateBaths);
            if (hasSeparateBaths) upnFlat.SeparateBathrooms = separateBaths;
        }

        /// <summary>
        /// Цена квартиры может быть в баннере "Оформить заявку на кредит"
        /// </summary>
        private void fillPrice(UpnFlatBase upnFlat, List<IElement> fieldValueElements)
        {
            if (!priceIndex.HasValue || priceIndex == 0)
                return;

            string priceText = fieldValueElements.ElementAtOrDefault(priceIndex.Value)?.InnerHtml;
            if (string.IsNullOrEmpty(priceText))
                return;

            int pFrom = priceText.IndexOf("<font style=\"font-size: 1.1em;\">") + "<font style=\"font-size: 1.1em;\">".Length;
            int pTo = priceText.LastIndexOf(" руб.</font>");

            if (pFrom < 0 || pTo < 0 || pFrom >= pTo)
                return;

            string priceValueText = priceText.Substring(pFrom, pTo - pFrom);
            bool isPriceParsed = Int32.TryParse(priceValueText.Replace(".", ""), out int priceValue);
            if (isPriceParsed)
                upnFlat.Price = priceValue;
        }

        private void fillAndClearDescription(UpnFlatBase upnFlat, List<IElement> fieldValueElements)
        {
            if (descriptionIndex.HasValue && descriptionIndex != 0)
                upnFlat.Description = fieldValueElements.ElementAtOrDefault(descriptionIndex.Value)?.InnerHtml;

            // Очистка описания
            if (!string.IsNullOrEmpty(upnFlat.Description))
            {
                upnFlat.Description = upnFlat.Description.Replace("<div bis_skin_checked=\"1\">", "");
                upnFlat.Description = upnFlat.Description.Replace("<br>", "");
                upnFlat.Description = upnFlat.Description.Replace("<div>", "");
                upnFlat.Description = upnFlat.Description.Replace("</div>", "");
                upnFlat.Description = upnFlat.Description.Replace("\n", "");
                upnFlat.Description = upnFlat.Description.Trim();
            }
        }
    }
}