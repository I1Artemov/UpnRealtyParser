using System;

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

        public string BalconyAmount { get; set; }

        public string Condition { get; set; }

        /// <summary>
        /// Тип собственности: частная, ...
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// Счетчик просмотров с сайта
        /// </summary>
        public int? UserViewsCount { get; set; }

        // TODO: Можно обновлять цену из сводной таблицы с квартирами, чтобы не залазить в каждую
    }
}