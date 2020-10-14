using System;

namespace UpnRealtyParser.Business.Models
{
    public class FlatCore : IdInfo
    {
        public DateTime? RemovalDate { get; set; }

        public DateTime? LastCheckDate { get; set; }

        public double? SpaceSum { get; set; }

        public double? SpaceLiving { get; set; }

        public double? SpaceKitchen { get; set; }

        public int? FlatFloor { get; set; }

        public int? Price { get; set; }

        public int? RoomAmount { get; set; }

        public string Description { get; set; }

        public int PageLinkId { get; set; }
    }
}