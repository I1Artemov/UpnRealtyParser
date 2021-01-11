namespace UpnRealtyParser.Business.Models
{
    public class AgencySitelessVM : AgencyCore
    {
        public string SourceSite { get; set; }
        public string WorkTime { get; set; }
        public string CompanyPhone { get; set; }
        public string Email { get; set; }
        public string AgentName { get; set; }
        public bool? IsCompany { get; set; }
    }
}
