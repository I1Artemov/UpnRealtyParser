using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        /* ---------------------- NotMapped-свойства для таблицы ---------------------- */

        [NotMapped]
        public string HouseAddress { get; set; }

        [NotMapped]
        public double? HouseLatitude { get; set; }

        [NotMapped]
        public double? HouseLongitude { get; set; }

        [NotMapped]
        public string HouseWallMaterial { get; set; }

        [NotMapped]
        public int? HouseMaxFloor { get; set; }

        [NotMapped]
        public int? HouseBuildYear { get; set; }

        [NotMapped]
        public string HouseType { get; set; }

        [NotMapped]
        public string ClosestSubwayName { get; set; }

        [NotMapped]
        public double? ClosestSubwayStationRange { get; set; }

        [NotMapped]
        public string AgencyName { get; set; }

        [NotMapped]
        public string SiteUrl { get; set; }

        [NotMapped]
        public int PhotoCount { get; set; }

        [NotMapped]
        public List<string> PhotoHrefs { get; set; }

        /// <summary>
        /// Ссылка на первое попавшееся фото, которое было скачано с сайта на сервер (если есть)
        /// </summary>
        [NotMapped]
        public string DownloadedPhotoHref { get; set; }

        [NotMapped]
        public bool? IsArchived { get; set; }

        [NotMapped]
        public virtual int? AgencyId => null;

        [NotMapped]
        public virtual int? HouseInfoId => null;

        // TODO: Удалить, когда будет вьюшка для N1
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