using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1ApartmentParser : BaseHttpParser
    {
		/// <summary> Предварительное получение квартир прямо со страницы с их перечнем </summary>
        public List<N1Flat> GetN1SellFlatsFromTablePage(string webPageText, N1HouseParser houseParser)
        {
            IDocument pageHtmlDoc = getPreparedHtmlDocument(webPageText).Result;

            List<N1Flat> flats = new List<N1Flat>();

            List<IElement> flatCards = pageHtmlDoc.All
                .Where(x => x.LocalName == "tr" && x.ClassList.Contains("re-search-result-table__body-row"))
                .ToList();

            foreach (IElement flatCard in flatCards)
            {
                N1Flat n1Flat = getN1SellFlatFromSingleHtmlCard(flatCard);
                N1HouseInfo n1House = houseParser.GetBasicN1HouseFromSingleApartmentCard(flatCard);
                n1Flat.ConnectedHouseForAddition = n1House;

                if (n1Flat != null)
                    flats.Add(n1Flat);
            }

            return flats;
        }

        private N1Flat getN1SellFlatFromSingleHtmlCard(IElement flatCard)
        {
            N1Flat flat = new N1Flat();

            fillApartmentAreaFromSingleCard(flat, flatCard);
            fillApartmentPriceFromSingleCard(flat, flatCard);
            fillApartmentFloorFromSingleCard(flat, flatCard);
            fillRoomAmountFromSingleCard(flat, flatCard);
            fillApartmentPageHref(flat, flatCard);
            fillApartmentPublishingDateFromSingleCard(flat, flatCard);

            return flat;
        }

        private void fillApartmentAreaFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string totalAreaStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_square")?.TextContent;

            if (string.IsNullOrEmpty(totalAreaStr))
                return;

            List<string> squareList = totalAreaStr.Split("/").ToList();
            if (squareList.Count < 3)
                return;

            string totalStr = squareList.ElementAt(0).Substring(0, squareList.ElementAt(0).Length - 1).Replace(".", ",");
            bool isParsed = double.TryParse(totalStr, out double totalArea);
            if (isParsed)
                flat.SpaceSum = totalArea;

            string livingStr = squareList.ElementAt(1).Substring(1, squareList.ElementAt(1).Length - 2).Replace(".", ",");
            bool isLivingParsed = double.TryParse(livingStr, out double livingArea);
            if (isLivingParsed)
                flat.SpaceLiving = livingArea;

            string kitchenStr = squareList.ElementAt(2).Substring(1, squareList.ElementAt(2).Length - 1).Replace(".", ",");
            bool isKitchenParsed = double.TryParse(kitchenStr, out double kitchenArea);
            if (isKitchenParsed)
                flat.SpaceKitchen = kitchenArea;
        }

        private void fillApartmentPriceFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string priceStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_price")?.TextContent;

            if (string.IsNullOrEmpty(priceStr))
                return;

            priceStr = priceStr.Replace(" ", "");
            priceStr = priceStr.Substring(0, priceStr.Length - 5);

            bool isParsed = int.TryParse(priceStr, out int price);
            if(isParsed)
                flat.Price = price;
        }

        private void fillApartmentFloorFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string allFloorStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_floor span")?.TextContent;

            if (string.IsNullOrEmpty(allFloorStr))
                return;

            int slashIndex = allFloorStr.IndexOf('/');
            if (slashIndex <= 0)
                return;

            allFloorStr = allFloorStr.Substring(0, slashIndex - 1);

            bool isParsed = int.TryParse(allFloorStr, out int floor);
            if (isParsed)
                flat.FlatFloor = floor;
        }

        private void fillRoomAmountFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string roomStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_type span")?.TextContent;

            if (string.IsNullOrEmpty(roomStr))
                return;

            if(roomStr == "ком")
            {
                flat.RoomAmount = 0;
                return;
            }

            bool isParsed = int.TryParse(roomStr.Replace("+", ""), out int roomAmount);
            if (isParsed)
                flat.RoomAmount = roomAmount;
        }

        private void fillApartmentPublishingDateFromSingleCard(N1FlatBase flat, IElement flatCard)
        {
            string dateStr = flatCard.QuerySelector("td.re-search-result-table__body-cell_date span")?.TextContent;

            if (string.IsNullOrEmpty(dateStr))
                return;

            DateTime? parsedDate = Utils.TryGetDateTimeFromString(dateStr, "dd.MM.yy");
            if (parsedDate != null)
                flat.PublishingDateTime = parsedDate;
        }

        /// <summary>
        /// Заполнение ссылки на страницу с квартирой нужно только для связывания квартиры и PageLink при вставке в БД
        /// </summary>
        private void fillApartmentPageHref(N1FlatBase flat, IElement flatCard)
        {
            string hrefStr = flatCard.QuerySelector("div.card-title.living-list-card__inner-block a.link")?.GetAttribute("href");

            if (string.IsNullOrEmpty(hrefStr))
                return;

            flat.Href = hrefStr;
        }

	    public N1Flat GetN1FlatFromPageText(string webPageText)
        {
            IDocument pageHtmlDoc = getPreparedHtmlDocument(webPageText).Result;
            return GetN1FlatFromPageText(pageHtmlDoc, webPageText);
        }

        public N1Flat GetN1FlatFromPageText(IDocument pageHtmlDoc, string webPageText)
        {
            N1Flat flat = new N1Flat();

            fillPlanningType(flat, pageHtmlDoc);
            fillBathroomType(flat, pageHtmlDoc);
            fillFlatCondition(flat, pageHtmlDoc);
            fillBalconyAmount(flat, pageHtmlDoc);
            fillPropertyType(flat, pageHtmlDoc);
            fillDescription(flat, pageHtmlDoc);

            return flat;
        }

        /// <summary>
        /// Выбирает из текста веб-страницы все ссылки на фотографии квартир. Возварщает список HREF-ов на jpg
        /// </summary>
        public List<string> GetPhotoHrefsFromPage(string pageText)
        {
            // a data-v-1aa2889c="" href="https://n1st.ru/cache/realty/photo/e1ae19f41edfd70cc5f0092af780e733_1200_900_p.jpg"
            var htmlDocument = getPreparedHtmlDocument(pageText).Result;
            List<IElement> anchorElements = htmlDocument.QuerySelectorAll("div.offer-card-gallery a.link")
                .ToList();

            return anchorElements
                .Select(x => x.GetAttribute("href"))
                .ToList();
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
            string balconyAmountStr = getValueFromLivingContentParamsList("Лоджия", pageHtmlDoc);
            if (string.IsNullOrEmpty(balconyAmountStr))
                return;

            int spaceIndex = balconyAmountStr.IndexOf(" ");
            if (spaceIndex < 0)
                return;

            balconyAmountStr = balconyAmountStr.Substring(0, spaceIndex);
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
            string descriptionText = pageHtmlDoc.QuerySelector(".card__comments-section p")?.TextContent;
            if (string.IsNullOrEmpty(descriptionText))
                return;

            descriptionText = descriptionText.Replace("<br>", "").Replace("\n", "").Replace("\t", "");
            flat.Description = descriptionText;
        }
    }
}