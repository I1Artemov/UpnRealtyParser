namespace UpnRealtyParser.Business.Models
{
    public class N1Agency : AgencyCore
    {
        public string AgentName { get; set; }

        /// <summary>
        /// Если true - агентство, иначе - частный риэлтор
        /// </summary>
        public bool? IsCompany { get; set; }
    }
}