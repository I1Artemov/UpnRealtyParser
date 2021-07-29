namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Точка с информацией об окупаемости и местоположении дома
    /// </summary>
    public class PaybackPeriodPoint : IdInfo
    {
        public int HouseId { get; set; }
        public string SiteName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? PaybackYears { get; set; }
        public string HouseAddress { get; set; }
    }
}