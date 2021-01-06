namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Структура "ID квартиры + тип (SellFlat или RentFlat)"
    /// </summary>
    public class FlatIdAndTypeVM
    {
        public int Id { get; set; }

        public string FlatType { get; set; }
    }
}
