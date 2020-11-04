﻿using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1ApartmentParser : BaseHttpParser
    {
		/// <summary> Предварительное получение квартир прямо со страницы с их перечнем </summary>
        public List<N1FlatBase> GetN1ApartmentsFromTablePage(string webPageText, N1HouseParser houseParser)
        {
            IDocument pageHtmlDoc = getPreparedHtmlDocument(webPageText).Result;
            return GetN1ApartmentsFromTablePage(pageHtmlDoc, webPageText, houseParser);
        }

        public List<N1FlatBase> GetN1ApartmentsFromTablePage(IDocument pageHtmlDoc, string webPageText, N1HouseParser houseParser)
        {
            List<N1FlatBase> flats = new List<N1FlatBase>();

            List<IElement> flatCards = pageHtmlDoc.All
                .Where(x => x.LocalName == "div" && x.ClassName == "living-list-card__main-container")
                .ToList();

            foreach(IElement flatCard in flatCards)
            {
                N1FlatBase n1Flat = getN1FlatFromSingleHtmlCard(flatCard);
                if (n1Flat != null)
                    flats.Add(n1Flat);

                N1HouseInfo n1House = houseParser.GetBasicN1HouseFromSingleApartmentCard(flatCard);
            }

            return flats;
        }

        private N1FlatBase getN1FlatFromSingleHtmlCard(IElement flatCard)
        {
            N1FlatBase flat = new N1FlatBase();

            fillApartmentAreaFromSingleCard(flat, flatCard);
            fillApartmentPriceFromSingleCard(flat, flatCard);
            fillApartmentFloorFromSingleCard(flat, flatCard);
            fillRoomAmountFromSingleCard(flat, flatCard);

            return flat;
        }

        private void fillApartmentAreaFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string totalAreaStr = flatCard.QuerySelector("div.living-list-card-area")?.InnerHtml;

            if (string.IsNullOrEmpty(totalAreaStr))
                return;

            int meterIndex = totalAreaStr.IndexOf('м');
            if (meterIndex <= 0)
                return;

            totalAreaStr = totalAreaStr.Substring(0, meterIndex);

            bool isParsed = int.TryParse(totalAreaStr, out int totalArea);
            if (isParsed)
                flat.SpaceSum = totalArea;
        }

        private void fillApartmentPriceFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string priceStr = flatCard.QuerySelector("div.living-list-card-price__item")?.GetAttribute("Title");

            if (string.IsNullOrEmpty(priceStr))
                return;

            priceStr = priceStr.Replace(" руб", "");
            priceStr = priceStr.Replace(" ", "");

            bool isParsed = int.TryParse(priceStr, out int price);
            if(isParsed)
                flat.Price = price;
        }

        private void fillApartmentFloorFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string allFloorStr = flatCard.QuerySelector("span.living-list-card-floor__item")?.InnerHtml;

            if (string.IsNullOrEmpty(allFloorStr))
                return;

            int slashIndex = allFloorStr.IndexOf('/');
            if (slashIndex <= 0)
                return;

            allFloorStr = allFloorStr.Substring(0, slashIndex);

            bool isParsed = int.TryParse(allFloorStr, out int floor);
            if (isParsed)
                flat.FlatFloor = floor;
        }

        private void fillRoomAmountFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string roomStr = flatCard.QuerySelector(".living-list-card__location .link-text")?.InnerHtml;

            if (string.IsNullOrEmpty(roomStr))
                return;

            int roomLetterIndex = roomStr.IndexOf("-к");
            if (roomLetterIndex <= 0)
                return;

            roomStr = roomStr.Substring(0, roomLetterIndex);

            bool isParsed = int.TryParse(roomStr, out int roomAmount);
            if (isParsed)
                flat.RoomAmount = roomAmount;
        }

	public N1FlatBase GetN1FlatFromPageText(string webPageText)
        {
            IDocument pageHtmlDoc = getPreparedHtmlDocument(webPageText).Result;
            return GetN1FlatFromPageText(pageHtmlDoc, webPageText);
        }

        public N1FlatBase GetN1FlatFromPageText(IDocument pageHtmlDoc, string webPageText)
        {
            N1FlatBase flat = new N1FlatBase();

            fillPriceAndRoomAmount(flat, webPageText);
            fillSpace(flat, pageHtmlDoc);
            fillPlanningType(flat, pageHtmlDoc);
            fillBathroomType(flat, pageHtmlDoc);
            fillFlatCondition(flat, pageHtmlDoc);
            fillBalconyAmount(flat, pageHtmlDoc);
            fillPropertyType(flat, pageHtmlDoc);
            fillDescription(flat, pageHtmlDoc);

            return flat;
        }

        private void fillPriceAndRoomAmount(N1FlatBase flat, string webPageText)
        {
            // Продам - руб. - N1
            int startIndex = webPageText.IndexOf("Продам");
            int endIndex = webPageText.IndexOf("руб. — N1");

            if (startIndex <= 0 || endIndex <= 0 || startIndex >= endIndex)
                return;

            string priceAndRoomContainingStr = webPageText.Substring(startIndex, endIndex - startIndex);

            string roomAmountStr = priceAndRoomContainingStr.Substring(7, 1);
            bool isParsedRoomAmount = int.TryParse(roomAmountStr, out int roomAmount);
            if (isParsedRoomAmount)
                flat.RoomAmount = roomAmount;
            // TODO: студии!

            int priceStartIndex = priceAndRoomContainingStr.LastIndexOf(", ");
            if (priceStartIndex <= 0)
                return;

            string priceStr = priceAndRoomContainingStr.Substring(priceStartIndex + 1,
                priceAndRoomContainingStr.Length - priceStartIndex - 1);

            bool isParsedPrice = int.TryParse(priceStr.Replace(" ", ""), out int price);
            if (isParsedPrice)
                flat.Price = price;
        }

        private void fillSpace(N1FlatBase flat, IDocument pageHtmlDoc)
        {
            string spaceSumStr = getValueFromLivingContentParamsList("Общая площадь", pageHtmlDoc);
            if (!string.IsNullOrEmpty(spaceSumStr))
                spaceSumStr = spaceSumStr.Substring(0, spaceSumStr.Length - 3);

            bool isSpaceSumParsed = double.TryParse(spaceSumStr.Replace('.', '.'), out double spaceSum);
            if (isSpaceSumParsed)
                flat.SpaceSum = spaceSum;
        }

        private void fillPlanningType(N1FlatBase flat, IDocument pageHtmlDoc)
        {
            string planningBlockStr = getValueFromLivingContentParamsList("Планировка", pageHtmlDoc);
            if (!string.IsNullOrEmpty(planningBlockStr))
                flat.PlanningType = planningBlockStr;
        }

        private void fillBathroomType(N1FlatBase flat, IDocument pageHtmlDoc)
        {
            string bathroomTypeStr = getValueFromLivingContentParamsList("Санузел", pageHtmlDoc);
            if (!string.IsNullOrEmpty(bathroomTypeStr))
                flat.BathroomType = bathroomTypeStr;
        }

        private void fillFlatCondition(N1FlatBase flat, IDocument pageHtmlDoc)
        {
            string conditionStrStr = getValueFromLivingContentParamsList("Состояние", pageHtmlDoc);
            if (!string.IsNullOrEmpty(conditionStrStr))
                flat.Condition = conditionStrStr;
        }

        private void fillBalconyAmount(N1FlatBase flat, IDocument pageHtmlDoc)
        {
            string balconyAmountStr = getValueFromLivingContentParamsList("Количество лоджий", pageHtmlDoc);
            if (string.IsNullOrEmpty(balconyAmountStr))
                return;

            bool isParsed = int.TryParse(balconyAmountStr, out int balconyAmount);
            if (isParsed)
                flat.BalconyAmount = balconyAmount;
        }

        private void fillPropertyType(N1FlatBase flat, IDocument pageHtmlDoc)
        {
            string propertyTypeStr = getValueFromLivingContentParamsList("Тип собственности", pageHtmlDoc);
            if (!string.IsNullOrEmpty(propertyTypeStr))
                flat.PropertyType = propertyTypeStr;
        }

        private void fillDescription(N1FlatBase flat, IDocument pageHtmlDoc)
        {
            string descriptionText = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "div" &&
                     m.ClassName == "text" &&
                     m.Attributes.GetNamedItem("_v-36737051")?.Value != null)?.InnerHtml;

            if (string.IsNullOrEmpty(descriptionText))
                return;

            descriptionText = descriptionText.Replace("<br>", "");
            descriptionText = descriptionText.Replace("\n", "");

            flat.Description = descriptionText;
        }

        /// <summary>
        /// Достает значение параметра квартиры из блока под фотографиями формата "Имя параметра ... значение".
        /// Ищет тот параметр, название которого содержит подстроку titleContains
        /// </summary>
        private string getValueFromLivingContentParamsList(string titleContains, IDocument pageHtmlDoc)
        {
            var livingContentBlock = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "li" &&
                     m.ClassName == "card-living-content-params-list__item" &&
                     m.InnerHtml != null && m.InnerHtml.Contains(titleContains));

            string livingContentBlockStr = livingContentBlock?.LastChild?.TextContent;
            return livingContentBlockStr;
        }
    }
}