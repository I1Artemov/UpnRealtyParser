namespace UpnRealtyParser.Business.Models
{
    public class FlatPhotoBase : IdInfo
    {
        // RentFlat или SellFlat
        public string RelationType { get; set; }

        public string FileName { get; set; }

        // Может ссылаться на различные таблицы в зависимости от RelationType
        public int FlatId { get; set; }

        public string Href { get; set; }
    }
}
