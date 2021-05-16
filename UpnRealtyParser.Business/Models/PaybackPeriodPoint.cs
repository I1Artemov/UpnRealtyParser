namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Точка с информацией об окупаемости и местоположении дома
    /// </summary>
    public class PaybackPeriodPoint
    {
        public int UpnHouseId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? PaybackYears { get; set; }
    }
}