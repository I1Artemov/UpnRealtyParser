using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class UnitTest1
    {
        private const string WorkingSellFlatUrl = "https://upn.ru/realty_eburg_flat_sale_info/20003330-2841.htm";

        [Fact]
        public void Test1()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Получаем контент страницы в виде текста
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Encoding = Encoding.GetEncoding("windows-1251");
            string webPageText = wc.DownloadString(WorkingSellFlatUrl);

            // Создаем объект AngleSharp html-разметки
            IBrowsingContext context = BrowsingContext.New(Configuration.Default);
            var htmlDocument = context.OpenAsync(req => req.Content(webPageText));

            List<IElement> fieldValueTds = htmlDocument.Result.All
                .Where(m => m.LocalName == "td" && m.OuterHtml.Contains("style=\"\""))
                .ToList();
        }
    }
}
