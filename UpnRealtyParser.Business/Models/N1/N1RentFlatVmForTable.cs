using System.ComponentModel.DataAnnotations.Schema;

namespace UpnRealtyParser.Business.Models.N1
{
    public class N1RentFlatVmForTable : FlatTableVmBase
    {
        /// <summary>
        /// Для обобщения с арендными квартирами UPN
        /// </summary>
        public string MinimalRentPeriod => "6 месяцев";

        [NotMapped]
        public override double? PaybackYears => null;
    }
}
