namespace UpnRealtyParser.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для квартир, по которым можно осуществлять поиск по критериям
    /// </summary>
    public interface IFilterableFlat
    {
        int? Price { get; set; }
        int? HouseBuildYear { get; set; }
        int? IsArchived { get; set; }
        int? FlatFloor { get; set; }
        int? HouseMaxFloor { get; set; }
        double? ClosestSubwayStationRange { get; set; }
        string ClosestSubwayName { get; set; }
        string HouseAddress { get; set; }
    }
}
