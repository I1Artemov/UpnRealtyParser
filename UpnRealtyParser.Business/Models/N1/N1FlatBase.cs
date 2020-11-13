using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpnRealtyParser.Business.Models
{
    public class N1FlatBase : FlatCore
    {
        public DateTime? PublishingDateTime { get; set; }

        /// <summary>
        /// Планировка: изолированная, ...
        /// </summary>
        public string PlanningType { get; set; }

        public string BathroomType { get; set; }

        public int? BalconyAmount { get; set; }

        public string Condition { get; set; }

        /// <summary>
        /// Тип собственности: частная, ...
        /// </summary>
        public string PropertyType { get; set; }

        // TODO: Можно обновлять цену из сводной таблицы с квартирами, чтобы не залазить в каждую
		public bool? IsFilledCompletely { get; set; }

        public int? N1HouseInfoId { get; set; }

        public int? N1AgencyId { get; set; }

        [NotMapped]
        public N1HouseInfo ConnectedHouseForAddition { get; set; }

        [NotMapped]
        public string Href { get; set; }
    }
}