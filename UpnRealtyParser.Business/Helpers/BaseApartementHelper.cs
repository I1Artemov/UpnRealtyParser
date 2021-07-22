using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Interfaces;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class BaseApartementHelper<THouse, TSellFlat, TRentFlat, TAgency, TFlatPhoto>
        where THouse : HouseInfoCore
        where TSellFlat : FlatCore
        where TRentFlat : FlatCore
        where TAgency : AgencyCore
        where TFlatPhoto : FlatPhotoBase
    {
        protected readonly EFGenericRepo<THouse, RealtyParserContext> _houseRepo;
        protected readonly EFGenericRepo<SubwayStation, RealtyParserContext> _subwayStationRepo;
        protected readonly EFGenericRepo<TAgency, RealtyParserContext> _agencyRepo;
        protected readonly EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        protected readonly EFGenericRepo<TFlatPhoto, RealtyParserContext> _photoRepo;

        public BaseApartementHelper(EFGenericRepo<THouse, RealtyParserContext> houseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<TAgency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<TFlatPhoto, RealtyParserContext> photoRepo)
        {
            _houseRepo = houseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
            _pageLinkRepo = pageLinkRepo;
            _photoRepo = photoRepo;
        }

        /// <summary>
        /// Заполняет одну квартиру информацией, взятой из данных о доме и об агентстве, для отображения в таблице
        /// </summary>
        public void FillSingleApartmentWithAdditionalInfo(FlatCore flat)
        {
            if (flat == null)
                return;

            fillHouseRelatedFields(flat);

            if (flat.AgencyId.HasValue)
            {
                TAgency agency = _agencyRepo.GetWithoutTracking(x => x.Id == flat.AgencyId);
                flat.AgencyName = agency?.Name;
            }

            PageLink foundLink = _pageLinkRepo.GetWithoutTracking(x => x.Id == flat.PageLinkId);
            if (foundLink != null) { 
                flat.SiteUrl = foundLink.Href;
            }
            if(flat.RemovalDate.HasValue || (DateTime.Now - flat.LastCheckDate.GetValueOrDefault(DateTime.MinValue)).Days > 7 ||
                (foundLink != null && foundLink.IsDead.GetValueOrDefault(false)))
            {
                flat.IsArchived = true;
            }

            flat.PhotoCount = _photoRepo
                .GetAllWithoutTracking()
                .Count(x => x.FlatId == flat.Id);
        }

        /// <summary>
        /// Заполняет квартиру списком ссылок на фото по ней
        /// </summary>
        /// <param name="upnFlat">Квартира для заполнения</param>
        /// <param name="relationType">SellFlat или RentFlat</param>
        public void FillSingleApartmentWithPhotoHrefs(FlatCore upnFlat, string relationType = Const.LinkTypeSellFlat)
        {
            List<string> photoHrefs = _photoRepo.GetAllWithoutTracking()
                .Where(x => x.FlatId == upnFlat.Id && x.RelationType == relationType)
                .Select(x => x.Href)
                .ToList();

            upnFlat.PhotoHrefs = photoHrefs;

            TFlatPhoto downloadedPhoto = _photoRepo.GetAllWithoutTracking()
                .FirstOrDefault(x => x.FileName != null && x.FileName != "ERR" && x.FlatId == upnFlat.Id
                    && x.RelationType == relationType);

            if (downloadedPhoto != null)
                upnFlat.DownloadedPhotoHref = "/images/upnphotos/" + downloadedPhoto.FileName;
        }

        private void fillHouseRelatedFields(FlatCore singleFlat)
        {
            if (!singleFlat.HouseInfoId.HasValue)
                return;

            THouse foundHouse = _houseRepo.GetWithoutTracking(x => x.Id == singleFlat.HouseInfoId);
            if (foundHouse == null)
                return;

            singleFlat.HouseAddress = foundHouse.Address;
            singleFlat.HouseBuildYear = foundHouse.BuildYear;
            singleFlat.HouseMaxFloor = foundHouse.MaxFloor;
            singleFlat.HouseWallMaterial = foundHouse.WallMaterial;
            singleFlat.HouseType = foundHouse.HouseType;
            singleFlat.HouseLatitude = foundHouse.Latitude;
            singleFlat.HouseLongitude = foundHouse.Longitude;

            if (!foundHouse.ClosestSubwayStationId.HasValue)
                return;

            SubwayStation foundStation = _subwayStationRepo
                .GetWithoutTracking(x => x.Id == foundHouse.ClosestSubwayStationId);

            if (foundStation == null)
                return;

            singleFlat.ClosestSubwayName = foundStation.Name;
            singleFlat.ClosestSubwayStationRange = foundHouse.ClosestSubwayStationRange;
        }

        /// <summary>
        /// Применяет сортировку к любой коллекции квартир, поддерживающих интерфейс ISortableFlat
        /// </summary>
        public IQueryable<T> ApplySorting<T>(IQueryable<T> allSellFlats, string sortField, string sortOrder)
            where T : ISortableFlat
        {
            if (string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortOrder))
            {
                allSellFlats = allSellFlats.OrderBy(x => x.Id);
                return allSellFlats;
            }

            if (sortField == "id" && sortOrder == "descend") allSellFlats = allSellFlats.OrderByDescending(x => x.Id);
            else if (sortField == "id") allSellFlats = allSellFlats.OrderBy(x => x.Id);

            if (sortField == "createdCheckedDatesSummary" && sortOrder == "descend") allSellFlats = allSellFlats.OrderByDescending(x => x.LastCheckDate);
            else if (sortField == "createdCheckedDatesSummary") allSellFlats = allSellFlats.OrderBy(x => x.LastCheckDate);

            if (sortField == "houseBuildYear" && sortOrder == "descend") allSellFlats =
                    allSellFlats.Where(x => x.HouseBuildYear != null).OrderByDescending(x => x.HouseBuildYear);
            else if (sortField == "houseBuildYear") allSellFlats = allSellFlats.Where(x => x.HouseBuildYear != null).OrderBy(x => x.HouseBuildYear);

            if (sortField == "price" && sortOrder == "descend") allSellFlats =
                    allSellFlats.Where(x => x.Price != null).OrderByDescending(x => x.Price);
            else if (sortField == "price") allSellFlats = allSellFlats.Where(x => x.Price != null).OrderBy(x => x.Price);

            if (sortField == "spaceSum" && sortOrder == "descend") allSellFlats =
                    allSellFlats.Where(x => x.SpaceSum != null).OrderByDescending(x => x.SpaceSum);
            else if (sortField == "spaceSum") allSellFlats = allSellFlats.Where(x => x.SpaceSum != null).OrderBy(x => x.SpaceSum);

            if (sortField == "subwaySummary" && sortOrder == "descend") allSellFlats =
                    allSellFlats.Where(x => x.ClosestSubwayStationRange != null).OrderByDescending(x => x.ClosestSubwayStationRange);
            else if (sortField == "subwaySummary") allSellFlats =
                    allSellFlats.Where(x => x.ClosestSubwayStationRange != null).OrderBy(x => x.ClosestSubwayStationRange);

            return allSellFlats;
        }

        /// <summary>
        /// Применяет сортировку и фильтрацию ко всем квартирам УПН на продажу
        /// </summary>
        public IQueryable<T> GetFilteredAndOrderedSellFlats<T>(bool? isShowArchived, bool? isExcludeFirstFloor,
            bool? isExcludeLastFloor, int? minPrice, int? maxPrice, int? minBuildYear, int? maxSubwayDistance,
            int? closestSubwayStationId, string addressPart, string sortField, string sortOrder,
            EFGenericRepo<T, RealtyParserContext> flatVmRepo)
            where T : FlatTableVmBase
        {
            IQueryable<T> allSellFlats = flatVmRepo.GetAllWithoutTracking();

            allSellFlats = applyFiltering(allSellFlats, isShowArchived, isExcludeFirstFloor, isExcludeLastFloor, minPrice, maxPrice, minBuildYear,
                maxSubwayDistance, closestSubwayStationId, addressPart);

            allSellFlats = ApplySorting(allSellFlats, sortField, sortOrder);

            return allSellFlats;
        }

        /// <summary>
        /// Применяет к квартирам поиск по пользовательским критериям
        /// </summary>
        protected IQueryable<T> applyFiltering<T>(IQueryable<T> allSellFlats, bool? isShowArchived, bool? isExcludeFirstFloor,
            bool? isExcludeLastFloor, int? minPrice, int? maxPrice, int? minBuildYear, int? maxSubwayDistance,
            int? closestSubwayStationId, string addressPart)
            where T : IFilterableFlat
        {
            if (!isShowArchived.GetValueOrDefault(true))
                allSellFlats = allSellFlats.Where(x => x.IsArchived.GetValueOrDefault(0) == 0);
            if (isExcludeFirstFloor.GetValueOrDefault(false))
                allSellFlats = allSellFlats.Where(x => x.FlatFloor > 1);
            if (isExcludeLastFloor.GetValueOrDefault(false))
                allSellFlats = allSellFlats.Where(x => x.FlatFloor < x.HouseMaxFloor);
            if (minPrice.HasValue)
                allSellFlats = allSellFlats.Where(x => x.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                allSellFlats = allSellFlats.Where(x => x.Price <= maxPrice.Value);
            if (minBuildYear.HasValue)
                allSellFlats = allSellFlats.Where(x => x.HouseBuildYear >= minBuildYear.Value);
            if (maxSubwayDistance.HasValue)
                allSellFlats = allSellFlats.Where(x => x.ClosestSubwayStationRange <= maxSubwayDistance.Value);
            if (closestSubwayStationId.HasValue)
            {
                SubwayStation foundStation = _subwayStationRepo.GetAllWithoutTracking()
                    .FirstOrDefault(x => x.Id == closestSubwayStationId.Value);
                string stationName = foundStation?.Name;
                allSellFlats = allSellFlats.Where(x => x.ClosestSubwayName == stationName);
            }
            if (!string.IsNullOrEmpty(addressPart))
                allSellFlats = allSellFlats.Where(x => x.HouseAddress.Contains(addressPart));

            return allSellFlats;
        }
    }
}
