namespace UpnRealtyParser.Business.Models
{
    public class UpnFlatVmForTable : FlatTableVmBase
    {
        public string FlatType { get; set; }
        public double? PaybackYears { get; set; }
        public string HousePhotoFile { get; set; }

        public string PhotoUrl
        { 
            get {
                if (!string.IsNullOrEmpty(FirstPhotoFile))
                    return "/images/upnphotos/" + FirstPhotoFile;

                if (!string.IsNullOrEmpty(HousePhotoFile))
                    return "/images/housephotos/" + HousePhotoFile;

                return null;
            }
        } 
    }
}
