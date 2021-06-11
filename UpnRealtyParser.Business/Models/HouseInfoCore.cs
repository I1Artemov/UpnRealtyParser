using System.ComponentModel.DataAnnotations.Schema;

namespace UpnRealtyParser.Business.Models
{
    public class HouseInfoCore : IdInfo
    {
        public string Address { get; set; }

        public string HouseType { get; set; }

        public int? BuildYear { get; set; }

        public string WallMaterial { get; set; }

        public int? MaxFloor { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int? ClosestSubwayStationId { get; set; }

        public double? ClosestSubwayStationRange { get; set; }

        /// <summary>
        /// ID дома с таким же адресом, но собранного с другого сайта
        /// </summary>
        [NotMapped]
        public int? SimilarHouseFromDifferentSiteId { get; set; }
    }
}