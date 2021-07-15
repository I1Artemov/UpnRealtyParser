using System;
using UpnRealtyParser.Business.Interfaces;

namespace UpnRealtyParser.Business.Models
{
    public class UpnFlatVmForTable : FlatTableVmBase, ISortableFlat
    {
        public DateTime? RemovalDate { get; set; }
        public double? SpaceSum { get; set; }
        public double? SpaceLiving { get; set; }
        public double? SpaceKitchen { get; set; }
        public string FlatType { get; set; }
        public int? Price { get; set; }
        public int? RoomAmount { get; set; }
        public string HouseAddress { get; set; }
        public double? HouseLatitude { get; set; }
        public double? HouseLongitude { get; set; }
        public string HouseWallMaterial { get; set; }
        public int? HouseBuildYear { get; set; }
        public string HouseType { get; set; }
        public string AgencyName { get; set; }
        public string SellerPhone { get; set; }
        public int? IsArchived { get; set; }
    }
}
