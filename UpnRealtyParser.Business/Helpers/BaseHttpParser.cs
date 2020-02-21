using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace UpnRealtyParser.Business.Helpers
{
    public class BaseHttpParser
    {
        public List<IElement> GetAnchorElementsFromWebPage(string pageText)
        {
            Task<IDocument> htmlDocument = getPreparedHtmlDocument(pageText);
            List<IElement> anchorElements = htmlDocument.Result.All
                .Where(m => m.LocalName == "a" &&
                    m.Attributes.GetNamedItem("href")?.Value != null &&
                    m.Attributes.GetNamedItem("href").Value.Contains("realty_eburg_flat_sale_info"))
                .ToList();

            return anchorElements;
        }

        public List<IElement> GetTdElementsFromWebPage(string pageText)
        {
            Task<IDocument> htmlDocument = getPreparedHtmlDocument(pageText);

            List<IElement> fieldValueElements = htmlDocument.Result.All
                .Where(m => m.LocalName == "td")
                .ToList();

            return fieldValueElements;
        }

        protected Task<IDocument> getPreparedHtmlDocument(string pageText)
        {
            pageText = pageText.Replace("windows-1251", "utf-8");

            // Создаем объект AngleSharp для html-разметки
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IConfiguration browsingConfig = Configuration.Default.WithCulture("ru-RU").WithLocaleBasedEncoding();
            IBrowsingContext context = BrowsingContext.New(browsingConfig);
            var htmlDocument = context.OpenAsync(req => req.Content(pageText));

            return htmlDocument;
        }
    }
}