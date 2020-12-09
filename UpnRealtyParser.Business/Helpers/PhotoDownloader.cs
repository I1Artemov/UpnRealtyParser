using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class PhotoDownloader : BaseSiteAgent
    {
        protected const int PhotoPortionAmount = 1000;
        protected const int MaxRetryAmountForSingleRequest = 100;
        protected EFGenericRepo<UpnFlatPhoto, RealtyParserContext> _photoRepo;

        public PhotoDownloader(bool isUseProxy, Action<string> writeToLogDelegate)
            : base(isUseProxy, writeToLogDelegate)
        { }

        protected override void initializeRepositories(RealtyParserContext context)
        {
            _photoRepo = new EFGenericRepo<UpnFlatPhoto, RealtyParserContext>(context);
        }

        public void DownloadSinglePhotoForAllUpnFlats()
        {
            int photosCount = _photoRepo.GetAllWithoutTracking().Count();

            IQueryable<UpnFlatPhoto> allPhotos = _photoRepo.GetAllWithoutTracking();
            List<FlatIdAndTypeVM> flatInfos = allPhotos
                .Select(x => new FlatIdAndTypeVM { Id = x.FlatId, FlatType = x.RelationType })
                .Distinct().ToList();

            foreach(FlatIdAndTypeVM flatInfo in flatInfos)
            {
                bool isAlreadyHasAnyPhotos = allPhotos.Any(x => x.FlatId == flatInfo.Id &&
                    x.RelationType == flatInfo.FlatType && !string.IsNullOrEmpty(x.FileName));
                if (isAlreadyHasAnyPhotos)
                    continue;

                UpnFlatPhoto firstPhotoInfo = _photoRepo.GetAll()
                    .FirstOrDefault(x => x.FlatId == flatInfo.Id && x.RelationType == flatInfo.FlatType);

                if (string.IsNullOrEmpty(firstPhotoInfo?.Href))
                    continue;

                DownloadAndSaveUpnPhoto(firstPhotoInfo);
            }
        }

        public void DownloadAndSaveUpnPhoto(UpnFlatPhoto photoInfo)
        {
            int fileNameStartIndex = photoInfo.Href.IndexOf("filename=");
            int fileNameEndIndex = photoInfo.Href.IndexOf(".jpg");

            if (fileNameStartIndex < 0 || fileNameEndIndex < 0 || fileNameStartIndex >= fileNameEndIndex)
                return;

            string fileName = photoInfo.Href.Substring(fileNameStartIndex + "filename=".Length, 
                fileNameEndIndex - (fileNameStartIndex + "filename=".Length));
            fileName = fileName + ".jpg";

            string downloadResult = downloadFileWithTries(photoInfo, fileName).Result;
        }

        protected async Task<string> downloadFileWithTries(UpnFlatPhoto photoInfo, string fileName)
        {
            int triesCount = 0;
            string currentProxyAddress = "";
            while (triesCount < MaxRetryAmountForSingleRequest)
            {
                try
                {
                    using (HttpClient wc = createHttpClient())
                    {
                        currentProxyAddress = _currentProxy?.Ip.ToString();
                        var response = await wc.GetAsync(photoInfo.Href);
                        using (var fs = new FileStream(Const.UpnPhotoFolder + fileName, FileMode.CreateNew))
                        {
                            await response.Content.CopyToAsync(fs);
                        }

                        photoInfo.FileName = fileName;
                        if (_photoRepo != null)
                        {
                            _photoRepo.Update(photoInfo);
                            _photoRepo.Save();
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("(404) Not Found"))
                        return "NotFound";

                    triesCount++;
                    if(_writeToLogDelegate != null)
                    {
                        _writeToLogDelegate(string.Format("Не удалось загрузить ссылку {0}, попытка {1}, прокси {2}",
                        photoInfo.Href, triesCount, currentProxyAddress));
                    }
                }
                finally
                {
                    triesCount++;
                }
            }
            return "LoadingFailed";
        }
    }
}
