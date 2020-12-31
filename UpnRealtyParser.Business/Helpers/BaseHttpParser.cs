﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace UpnRealtyParser.Business.Helpers
{
    public class BaseHttpParser
    {
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

        /// <summary>
        /// Возвращает текст внутри элемента, если в нем нет дочерних элементов, 
        /// или текст дочернего элемента, если такой элемент есть
        /// </summary>
        protected string getNodeValueOrFirstChildValue(IElement element)
        {
            if (element == null)
                return "";

            string result = element.ChildElementCount == 0 ?
                    element.InnerHtml : element.FirstElementChild?.InnerHtml;

            return result;
        }

        /// <summary>
        /// Достает значение параметра квартиры из блока под фотографиями формата "Имя параметра ... значение".
        /// Ищет тот параметр, название которого содержит подстроку titleContains
        /// </summary>
        protected string getValueFromLivingContentParamsList(string titleContains, IDocument pageHtmlDoc)
        {
            var livingContentBlock = pageHtmlDoc.All.FirstOrDefault(
                m => m.LocalName == "dl" &&
                     m.InnerHtml != null && m.InnerHtml.Contains(titleContains));

            string livingContentBlockStr = livingContentBlock?.LastElementChild?.TextContent;
            if (string.IsNullOrEmpty(livingContentBlockStr))
                return null;

            livingContentBlockStr = livingContentBlockStr.Replace("   ", "").Replace("\n", "").Trim();
            return livingContentBlockStr;
        }
    }
}