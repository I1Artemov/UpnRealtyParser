using System;
using System.ComponentModel.DataAnnotations.Schema;
using UpnRealtyParser.Business.Interfaces;

namespace UpnRealtyParser.Business.Models
{
    public class FlatTableVmBase : IdInfo, ISortableFlat, IFilterableFlat
    {
        public DateTime? RemovalDate { get; set; }
        public double? SpaceSum { get; set; }
        public double? SpaceLiving { get; set; }
        public double? SpaceKitchen { get; set; }
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

        public DateTime? LastCheckDate { get; set; }
        public int? FlatFloor { get; set; }
        public virtual int? HouseMaxFloor { get; set; }
        public virtual string ClosestSubwayName { get; set; }
        public double? ClosestSubwayStationRange { get; set; }
        public string Description { get; set; }
        public string FirstPhotoFile { get; set; }


        /* ------------------------ Свойства для отображения -------------------------- */

        public string LastCheckDatePrintable =>
            LastCheckDate == null ? "" : LastCheckDate.Value.ToString("dd.MM.yyyy");

        /// <summary>
        /// Этаж = "(этаж квартиры) / (этажей в доме)"
        /// </summary>
        public string FloorSummary
        {
            get
            {
                string flatFloorStr = FlatFloor?.ToString() ?? "?";
                string maxFloorStr = HouseMaxFloor?.ToString() ?? "?";
                return flatFloorStr + "/" + maxFloorStr;
            }
        }

        /// <summary>
        /// До метро: "(станция метро) ((расстояние) м.)"
        /// </summary>
        public string SubwaySummary =>
            Helpers.Utils.GetFormattedSubwaySummaryString(ClosestSubwayName, ClosestSubwayStationRange);

        /// <summary>
        /// Даты создания и проверки наличия на сайте через тире
        /// </summary>
        public string CreatedCheckedDatesSummary =>
            string.Format("{0} - {1}", CreationDatePrintable, LastCheckDatePrintable);

        public string ShortenedDescription =>
            (string.IsNullOrEmpty(Description) || Description.Length <= 150) ? Description
                : Description.Substring(0, 147) + "...";
    }
}
