namespace UpnRealtyParser.Business.Models
{
    public class N1FlatPhoto : FlatPhotoBase
    {
        public string GetFilePath()
        {
            return string.Format("N1Photos\\{0}\\{1}.jpg", RelationType, FileName);
        }
    }
}
