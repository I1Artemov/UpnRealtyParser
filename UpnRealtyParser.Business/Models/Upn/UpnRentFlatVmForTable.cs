using System.ComponentModel.DataAnnotations.Schema;

namespace UpnRealtyParser.Business.Models
{
    public class UpnRentFlatVmForTable : FlatTableVmBase
    {
        public string MinimalRentPeriod { get; set; }
        public string FlatType { get; set; }

        [NotMapped]
        public override double? PaybackYears => null;
    }
}
