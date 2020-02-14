using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;

namespace UpnRealtyParser.Business.Helpers
{
    public class BaseHttpParser
    {
        public List<IElement> GetTdElementsFromWebPage(string pageText)
        {
            pageText = pageText.Replace("windows-1251", "utf-8");

            // Создаем объект AngleSharp для html-разметки
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IConfiguration browsingConfig = Configuration.Default.WithCulture("ru-RU").WithLocaleBasedEncoding();
            IBrowsingContext context = BrowsingContext.New(browsingConfig);
            var htmlDocument = context.OpenAsync(req => req.Content(pageText));

            List<IElement> fieldValueElements = htmlDocument.Result.All
                .Where(m => m.LocalName == "td")
                .ToList();

            return fieldValueElements;
        }
    }
}