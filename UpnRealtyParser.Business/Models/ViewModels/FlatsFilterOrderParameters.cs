namespace UpnRealtyParser.Business.Models
{
    public class FlatsFilterOrderParameters
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public bool? IsShowArchived { get; set; }
        public bool? IsExcludeFirstFloor { get; set; }
        public bool? IsExcludeLastFloor { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? MinBuildYear { get; set; }
        public int? MaxSubwayDistance { get; set; }
        public int? ClosestSubwayStationId { get; set; }
        public string AddressPart { get; set; }
        public bool? IsShowRooms { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
    }
}
