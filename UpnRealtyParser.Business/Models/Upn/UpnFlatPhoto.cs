namespace UpnRealtyParser.Business.Models
{
    public class UpnFlatPhoto : IdInfo
    {
        // RentFlat или SellFlat
        public string RelationType { get; set; }

        public string FileName { get; set; }

        // Может ссылаться на различные таблицы в зависимости от RelationType
        public int FlatId { get; set; }

        public string Href { get; set; }

        public string GetFilePath()
        {
            return string.Format("UpnPhotos\\{0}\\{1}.jpg", RelationType, FileName);
        }
    }
}