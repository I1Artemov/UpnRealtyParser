using AngleSharp.Dom;
using System.Collections.Generic;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnAgencyParser
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
    }
}
