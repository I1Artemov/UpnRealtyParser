using AngleSharp.Dom;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1AgencyParser : BaseHttpParser
    {
        public N1Agency GetN1AgencyFromPageText(string webPageText)
        {
            IDocument pageHtmlDoc = getPreparedHtmlDocument(webPageText).Result;
            return GetN1AgencyFromPageText(pageHtmlDoc, webPageText);
        }

        public N1Agency GetN1AgencyFromPageText(IDocument pageHtmlDoc, string webPageText)
        {
            N1Agency agency = new N1Agency();

            return agency;
        }

        private void fillAgencyName(N1Agency agency, IDocument pageHtmlDoc)
        {

        }

        private void fillAgentName()
        {

        }
    }
}