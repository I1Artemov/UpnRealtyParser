using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class PhotoDownloadTest
    {
        [Fact]
        public void DownloadSinglePhoto_Upn_Test()
        {
            PhotoDownloader photoLoader = new PhotoDownloader(false, 0, null);
            UpnFlatPhoto flatPhoto = new UpnFlatPhoto {
                Href = "https://upn.ru/getpic.ashx?gid=05674e97-a8c8-47b2-92c5-ea4ff89aba16&filename=1369_30087437_7_312102847.jpg&t=false"
            };
            photoLoader.DownloadAndSaveUpnPhoto(flatPhoto);
        }

        [Fact]
        public void DownloadMultiplePhotos_Upn_Test()
        {
            PhotoDownloader photoLoader = new PhotoDownloader(true, 2000, null);
            photoLoader.OpenConnection();
            photoLoader.DownloadSinglePhotoForAllUpnFlats();
            photoLoader.CloseConnection();
        }
    }
}
