namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Информация между ближайшими домами из N1 для каждого дома с УПН
    /// </summary>
    public class SimilarHouse : IdInfo
    {
        public int? UpnHouseId { get; set; }
        public int? N1HouseId { get; set; }
        public double Distance { get; set; }
    }
}