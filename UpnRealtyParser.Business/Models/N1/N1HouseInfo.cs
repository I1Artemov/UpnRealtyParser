namespace UpnRealtyParser.Business.Models
{
    public class N1HouseInfo : HouseInfoCore
    {
        /// <summary>
        /// Название фирмы-застройщика
        /// </summary>
        public string BuilderCompany { get; set; }

        public bool? IsFilledCompletely { get; set; }
    }
}