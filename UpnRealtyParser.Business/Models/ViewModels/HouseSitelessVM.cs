using System.ComponentModel.DataAnnotations.Schema;
using UpnRealtyParser.Business.Helpers;

namespace UpnRealtyParser.Business.Models
{
    public class HouseSitelessVM : HouseInfoCore
    {
        public string SourceSite { get; set; }
        public string ClosestSubwayName { get; set; }
        public int? SimilarIdentity { get; set; }

        [NotMapped]
        public string ClosestSubwayRangeInfo => 
            Utils.GetFormattedSubwayDistanceString(ClosestSubwayStationRange);
    }
}