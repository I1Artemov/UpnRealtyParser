using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class BaseHouseHelper
    {
        private readonly EFGenericRepo<HouseSitelessVM, RealtyParserContext> _unitedHouseRepo;
        private readonly EFGenericRepo<HousePhoto, RealtyParserContext> _housePhotoRepo;
        private readonly EFGenericRepo<SimilarHouse, RealtyParserContext> _similarHouseRepo;

        public BaseHouseHelper(EFGenericRepo<HouseSitelessVM, RealtyParserContext> unitedHouseRepo,
            EFGenericRepo<HousePhoto, RealtyParserContext> housePhotoRepo,
            EFGenericRepo<SimilarHouse, RealtyParserContext> similarHouseRepo)
        {
            _unitedHouseRepo = unitedHouseRepo;
            _housePhotoRepo = housePhotoRepo;
            _similarHouseRepo = similarHouseRepo;
        }

        /// <summary>
        /// Применяет сортировку и фильтрацию ко всем домам (с УПН и с N1)
        /// </summary>
        public IQueryable<HouseSitelessVM> GetFilteredAndOrderedHouses(int? minBuildYear, bool? isShowUpn,
            bool? isShowN1, string addressPart, string sortField, string sortOrder)
        {
            IQueryable<HouseSitelessVM> allHouses = _unitedHouseRepo.GetAllWithoutTracking();
            if (minBuildYear.HasValue)
                allHouses = allHouses.Where(x => x.BuildYear >= minBuildYear.Value);
            if (!isShowUpn.GetValueOrDefault(true))
                allHouses = allHouses.Where(x => x.SourceSite != Const.SiteNameUpn);
            if (!isShowN1.GetValueOrDefault(true))
                allHouses = allHouses.Where(x => x.SourceSite != Const.SiteNameN1);
            if (!string.IsNullOrEmpty(addressPart))
                allHouses = allHouses.Where(x => x.Address.Contains(addressPart));

            if (!string.IsNullOrEmpty(sortField) && !string.IsNullOrEmpty(sortOrder))
            {
                if (sortField == "id" && sortOrder == "descend") allHouses = allHouses.OrderByDescending(x => x.Id);
                else if (sortField == "id") allHouses = allHouses.OrderBy(x => x.Id);

                if (sortField == "creationDatePrintable" && sortOrder == "descend") allHouses = allHouses.OrderByDescending(x => x.CreationDateTime);
                else if (sortField == "creationDatePrintable") allHouses = allHouses.OrderBy(x => x.CreationDateTime);

                if (sortField == "buildYear" && sortOrder == "descend") allHouses = 
                        allHouses.Where(x => x.BuildYear != null).OrderByDescending(x => x.BuildYear);
                else if (sortField == "buildYear") allHouses = allHouses.Where(x => x.BuildYear != null).OrderBy(x => x.BuildYear);

                if (sortField == "closestSubwayRangeInfo" && sortOrder == "descend") allHouses = 
                        allHouses.Where(x => x.ClosestSubwayStationRange != null).OrderByDescending(x => x.ClosestSubwayStationRange);
                else if (sortField == "closestSubwayRangeInfo") allHouses =
                        allHouses.Where(x => x.ClosestSubwayStationRange != null).OrderBy(x => x.ClosestSubwayStationRange);
            } else
            {
                allHouses = allHouses.OrderByDescending(x => x.SimilarIdentity);
            }

            return allHouses;
        }

        /// <summary>
        /// Заполняет объединенные дома УПН и N1 фотографиями
        /// </summary>
        public void FillHousesWithPhotoInfo(List<HouseSitelessVM> houses)
        {
            foreach(var house in houses)
            {
                FillSingleHouseWithPhotoInfo(house, house.SourceSite);
            }
        }

        public void FillSingleHouseWithPhotoInfo<T>(T house, string sourceSite)
            where T : HouseInfoCore
        {
            HousePhoto foundPhoto = null;
            if (sourceSite == "UPN")
            {
                foundPhoto = _housePhotoRepo.GetAllWithoutTracking()
                    .FirstOrDefault(x => x.UpnHouseId == house.Id);
            }
            else
            {
                SimilarHouse foundSimilar = _similarHouseRepo.GetAllWithoutTracking()
                    .FirstOrDefault(x => x.N1HouseId == house.Id);
                if (foundSimilar != null)
                {
                    foundPhoto = _housePhotoRepo.GetAllWithoutTracking()
                        .FirstOrDefault(x => x.UpnHouseId == foundSimilar.UpnHouseId);
                }
            }

            if (foundPhoto != null)
                house.Photo = "/images/housephotos/" + foundPhoto.FileName;
        }
    }
}
