namespace UpnRealtyParser.Business.Models.N1
{
    public class N1FlatVmForTable : FlatTableVmBase
    {
        public string HousePhotoFile { get; set; }

        public string PhotoUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstPhotoFile))
                    return "/images/n1photos/" + FirstPhotoFile;

                if (!string.IsNullOrEmpty(HousePhotoFile))
                    return "/images/housephotos/" + HousePhotoFile;

                return null;
            }
        }
    }
}
