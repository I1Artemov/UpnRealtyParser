using System;

namespace UpnRealtyParser.Business.Models
{
    public class FlatTableVmBase : IdInfo
    {
        public DateTime? LastCheckDate { get; set; }
        public int? FlatFloor { get; set; }
        public virtual int? HouseMaxFloor { get; set; }
        public virtual string ClosestSubwayName { get; set; }
        public double? ClosestSubwayStationRange { get; set; }
        public string Description { get; set; }


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
        public string SubwaySummary
        {
            get
            {
                if (string.IsNullOrEmpty(ClosestSubwayName) || !ClosestSubwayStationRange.HasValue)
                    return "н/у";

                return string.Format("{0} ({1} м.)", ClosestSubwayName, ClosestSubwayStationRange);
            }
        }

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
