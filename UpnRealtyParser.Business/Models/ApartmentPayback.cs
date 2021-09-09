namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Объект с информацией о предполагаемой окупаемости одной квартиры на продажу
    /// </summary>
    public class ApartmentPayback : IdInfo
    {
        /// <summary>
        /// С какого сайта была взята сама квартира на продажу
        /// </summary>
        public string Site { get; set; }

        public int FlatId { get; set; }

        /// <summary>
        /// Посчитанный период окупаемости = (цена кв.) / ((12 мес) * (средняя аренда по дому))
        /// </summary>
        public double? PaybackPeriod { get; set; }

        /// <summary>
        /// По квартире N1 может быть посчитана статистика из арендных квартир UPN (и наоборот)
        /// </summary>
        public string CalculatedFromSite { get; set; }
    }
}
