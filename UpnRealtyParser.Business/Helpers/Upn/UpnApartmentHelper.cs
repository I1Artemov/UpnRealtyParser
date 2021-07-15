using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnApartmentHelper : BaseApartementHelper<UpnHouseInfo, UpnFlat, UpnRentFlat, UpnAgency, UpnFlatPhoto>
    {
        private readonly EFGenericRepo<UpnFlatVmForTable, RealtyParserContext> _upnFlatVmRepo;

        public UpnApartmentHelper(EFGenericRepo<UpnHouseInfo, RealtyParserContext> houseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<UpnAgency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<UpnFlatPhoto, RealtyParserContext> photoRepo,
            EFGenericRepo<UpnFlatVmForTable, RealtyParserContext> upnFlatVmRepo = null) 
            : base(houseRepo, subwayStationRepo, agencyRepo, pageLinkRepo, photoRepo)
        {
            _upnFlatVmRepo = upnFlatVmRepo;
        }

        /// <summary>
        /// Заполняет квартиры под аренду информацией, взятой из данных о доме и об агентстве, для отображения в таблице
        /// </summary>
        public void FillRentApartmentsWithAdditionalInfo(List<UpnRentFlat> upnFlats)
        {
            if (upnFlats == null || upnFlats.Count == 0)
                return;

            foreach (UpnRentFlat upnFlat in upnFlats)
            {
                FillSingleApartmentWithAdditionalInfo(upnFlat);
            }
        }

        /// <summary>
        /// Применяет сортировку и фильтрацию ко всем квартирам УПН на продажу
        /// </summary>
        public IQueryable<UpnFlatVmForTable> GetFilteredAndOrderedUpnSellFlats(bool? isShowArchived, bool? isExcludeFirstFloor,
            bool? isExcludeLastFloor, int? minPrice, int? maxPrice, int? minBuildYear, int? maxSubwayDistance,
            int? closestSubwayStationId, string addressPart, string sortField, string sortOrder)
        {
            IQueryable<UpnFlatVmForTable> allSellFlats = _upnFlatVmRepo.GetAllWithoutTracking();

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

            allSellFlats = applySorting(allSellFlats, sortField, sortOrder);

            return allSellFlats;
        }

        private IQueryable<UpnFlatVmForTable> applySorting(IQueryable<UpnFlatVmForTable> allSellFlats, string sortField, string sortOrder)
        {
            if(string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortOrder))
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
    }
}