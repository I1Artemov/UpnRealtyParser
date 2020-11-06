using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnAgencyParser : BaseHttpParser
    {
        private const string nameHeaderText = "Агентство недвижимости:";
        private const string workTimeHeaderText = "Время работы:";
        private const string companyPhoneHeaderText = "Телефон агентства:";
        private const string agentPhoneHeaderText = "Телефон агента:";
        private const string siteUrlHeaderText = "WWW:";
        private const string emailHeaderText = "EMail:";

        private int? nameIndex;
        private int? workTimeIndex;
        private int? companyPhoneIndex;
        private int? agentPhoneIndex;
        private int? siteUrlIndex;
        private int? emailIndex;

        protected void fillFieldIndexes(List<IElement> tdElements)
        {
            nameIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", nameHeaderText)) + 1;
            workTimeIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", workTimeHeaderText)) + 1;
            companyPhoneIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", companyPhoneHeaderText)) + 1;
            agentPhoneIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", agentPhoneHeaderText)) + 1;
            siteUrlIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", siteUrlHeaderText)) + 1;
            emailIndex = tdElements.FindIndex(x => x.InnerHtml == string.Format("<b>{0}</b>", emailHeaderText)) + 1;
        }

        public UpnAgency GetAgencyFromPageText(List<IElement> fieldValueElements)
        {
            fillFieldIndexes(fieldValueElements);
            UpnAgency agency = new UpnAgency { CreationDateTime = DateTime.Now };

            if (nameIndex.HasValue && nameIndex != 0) {
                IElement nameAnchor = fieldValueElements.ElementAtOrDefault(nameIndex.Value);
                agency.Name = getNodeValueOrFirstChildValue(nameAnchor);
            }
            if (workTimeIndex.HasValue && workTimeIndex != 0)
                agency.WorkTime = fieldValueElements.ElementAtOrDefault(workTimeIndex.Value)?.InnerHtml;
            if (companyPhoneIndex.HasValue && companyPhoneIndex != 0)
                agency.CompanyPhone = fieldValueElements.ElementAtOrDefault(companyPhoneIndex.Value)?.InnerHtml;
            if (agentPhoneIndex.HasValue && agentPhoneIndex != 0)
            {
                IElement agentPhoneAnchor = fieldValueElements.ElementAtOrDefault(agentPhoneIndex.Value);
                agency.AgentPhone = getNodeValueOrFirstChildValue(agentPhoneAnchor);
            }
            if (siteUrlIndex.HasValue && siteUrlIndex != 0)
            {
                IElement siteUrlAnchor = fieldValueElements.ElementAtOrDefault(siteUrlIndex.Value);
                agency.SiteUrl = getNodeValueOrFirstChildValue(siteUrlAnchor);
            }
            if (emailIndex.HasValue && emailIndex != 0) {
                IElement emailAnchor = fieldValueElements.ElementAtOrDefault(emailIndex.Value);
                agency.Email = getNodeValueOrFirstChildValue(emailAnchor);
            }

            return agency;
        }
    }
}
