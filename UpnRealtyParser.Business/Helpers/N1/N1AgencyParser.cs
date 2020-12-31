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

            fillAgencyName(agency, pageHtmlDoc);
            fillAgentName(agency, pageHtmlDoc);
            fillAgentPhone(agency, webPageText);
            fillAgencyUrl(agency, webPageText);

            return agency;
        }

        private void fillAgencyName(N1Agency agency, IDocument pageHtmlDoc)
        {
            string agencyName = pageHtmlDoc.QuerySelector(".organization-informer__title-link")?.TextContent;
            if (string.IsNullOrEmpty(agencyName))
                return;

            agency.Name = agencyName;
        }

        private void fillAgentName(N1Agency agency, IDocument pageHtmlDoc)
        {
            string agentName = pageHtmlDoc.QuerySelector(".card__author-name")?.TextContent;

            if (string.IsNullOrEmpty(agentName))
                return;

            agency.AgentName = agentName.Replace("\n", "").Trim();
        }

        private void fillAgentPhone(N1Agency agency, string webPageText)
        {
            // ","contact_phone":"+7 902 870-05-43","
            int startIndex = webPageText.IndexOf("\"contact_phone\":\"");
            if (startIndex <= 0)
                return;

            string fullPhoneStr = webPageText.Substring(startIndex + "\"contact_phone\":\"".Length, 16);

            fullPhoneStr = fullPhoneStr.Replace("+7", "7").Replace(" ", "").Replace("-", "");
            agency.AgentPhone = fullPhoneStr;
        }

        private void fillAgencyUrl(N1Agency agency, string webPageText)
        {
            // ","contact_email":"mmir.ekb@yandex.ru","
            int startIndex = webPageText.IndexOf("\"contact_email\":\"");
            if (startIndex <= 0)
                return;

            string emailString = webPageText.Substring(startIndex + "\"contact_email\":\"".Length, 128);
            int endIndex = emailString.IndexOf("\",\"");
            if (endIndex <= 0)
                return;

            agency.SiteUrl = emailString.Substring(0, endIndex);
        }
    }
}