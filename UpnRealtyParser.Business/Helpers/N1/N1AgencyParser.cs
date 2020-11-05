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
            fillAgentPhone(agency, pageHtmlDoc);
            fillAgencyUrl(agency, pageHtmlDoc);

            return agency;
        }

        private void fillAgencyName(N1Agency agency, IDocument pageHtmlDoc)
        {
            string agencyName = pageHtmlDoc.QuerySelector("a._agency-name span")?.InnerHtml;
            if (string.IsNullOrEmpty(agencyName))
                return;

            agency.Name = agencyName;
        }

        private void fillAgentName(N1Agency agency, IDocument pageHtmlDoc)
        {
            string agentName = pageHtmlDoc.QuerySelector(".offer-card-contacts__agency-owner-name")?.InnerHtml;
            if (string.IsNullOrEmpty(agentName))
                return;

            agency.AgentName = agentName;

            string sellerTypeStr = pageHtmlDoc.QuerySelector(".offer-card-contacts__person._type")?.InnerHtml;
            if(sellerTypeStr != "Частный риелтор")
                agency.IsCompany = true;
        }

        private void fillAgentPhone(N1Agency agency, IDocument pageHtmlDoc)
        {
            string agentPhone = pageHtmlDoc.QuerySelector("a.offer-card-contacts-phones__phone")?.GetAttribute("href");
            if (string.IsNullOrEmpty(agentPhone))
                return;

            agentPhone = agentPhone.Replace("tel:+", "");
            agency.AgentPhone = agentPhone;
        }

        private void fillAgencyUrl(N1Agency agency, IDocument pageHtmlDoc)
        {
            string agencyUrl = pageHtmlDoc.QuerySelector("a._agency-name")?.GetAttribute("href");
            if (string.IsNullOrEmpty(agencyUrl))
                return;

            agency.SiteUrl = agencyUrl;
        }
    }
}