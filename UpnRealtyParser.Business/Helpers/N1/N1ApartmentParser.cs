using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1ApartmentParser : BaseHttpParser
    {
		/// <summary> Предварительное получение квартир прямо со страницы с их перечнем </summary>
        public List<N1FlatBase> GetN1ApartmentsFromTablePage(string webPageText)
        {
            IDocument pageHtmlDoc = getPreparedHtmlDocument(webPageText).Result;
            return GetN1ApartmentsFromTablePage(pageHtmlDoc, webPageText);
        }

        public List<N1FlatBase> GetN1ApartmentsFromTablePage(IDocument pageHtmlDoc, string webPageText)
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
            }

            return flats;
        }

        private N1FlatBase getN1FlatFromSingleHtmlCard(IElement flatCard)
        {
            N1FlatBase flat = new N1FlatBase();

            fillApartmentAreaFromSingleCard(flat, flatCard);
            fillApartmentPriceFromSingleCard(flat, flatCard);
            fillApartmentFloorFromSingleCard(flat, flatCard);

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
    }
}