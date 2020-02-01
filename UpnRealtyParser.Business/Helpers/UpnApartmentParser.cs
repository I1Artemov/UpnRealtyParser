using System.Linq;
using AngleSharp;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnApartmentParser : BaseHttpParser
    {
        public void GetSingleSellFlatInfoFromUrl(string flatUrl)
        {
            IBrowsingContext context = BrowsingContext.New(Configuration.Default);
            var htmlDocument = context.OpenAsync(req => req.Content(
                "<ul><li>First item<li>Second item<li class='blue'>Third item!<li class='blue red'>Last item!</ul>"));

            var blueListItemsLinq = htmlDocument.Result.All
                .Where(m => m.LocalName == "li" && m.ClassList.Contains("blue"));
        }
    }
}