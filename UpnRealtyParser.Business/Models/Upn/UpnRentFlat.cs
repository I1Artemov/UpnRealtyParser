using UpnRealtyParser.Business.Interfaces;

namespace UpnRealtyParser.Business.Models
{
    public class UpnRentFlat : UpnFlatBase, ISortableFlat
    {
        public string MinimalRentPeriod { get; set; }
    }
}