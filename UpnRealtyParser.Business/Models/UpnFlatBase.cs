using System;

namespace UpnRealtyParser.Business.Models
{
    public class UpnFlatBase : IdInfo
    {
        // NEW
        public DateTime? RemovalDate { get; set; }

        // NEW
        public DateTime? LastCheckDate { get; set; }

        public int? UpnHouseInfoId { get; set; }

        public int? UpnAgencyId { get; set; }

        public string FlatType { get; set; }

        public int RoomAmount { get; set; }

        public double? SpaceSum { get; set; }

        public double? SpaceLiving { get; set; }

        public double? SpaceKitchen { get; set; }

        public int? FlatFloor { get; set; }

        public int? JointBathrooms { get; set; }

        public int? SeparateBathrooms { get; set; }

        public string RenovationType { get; set; }

        public string RedevelopmentType { get; set; }

        public string WindowsType { get; set; }

        public string Furniture { get; set; }

        public int? Price { get; set; }

        public string Description { get; set; }

        public int PageLinkId { get; set; }
    }
}