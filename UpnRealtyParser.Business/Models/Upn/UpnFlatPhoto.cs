namespace UpnRealtyParser.Business.Models
{
    public class UpnFlatPhoto : FlatPhotoBase
    {
        public string GetFilePath()
        {
            return string.Format("UpnPhotos\\{0}\\{1}.jpg", RelationType, FileName);
        }
    }
}