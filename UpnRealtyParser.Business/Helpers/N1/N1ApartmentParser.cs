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
                .Where(x => x.LocalName == "div" && x.ClassList.Contains("professional-offers-table-row"))
                .ToList();

            return flats;
        }

        private N1FlatBase getN1FlatFromSingleHtmlCard(IElement flatCard)
        {
            N1FlatBase flat = new N1FlatBase();

            return flat;
        }
    }
}