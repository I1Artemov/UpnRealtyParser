namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// для хранения статистики со средними ценами на квартиры в доме в определенные месяца года
    /// </summary>
    public class AveragePriceStat
    {
        public int Id { get; set; }
        public string Site { get; set; }
        public int HouseId { get; set; }
        public int RoomAmount { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public double? Price { get; set; }
    }
}
