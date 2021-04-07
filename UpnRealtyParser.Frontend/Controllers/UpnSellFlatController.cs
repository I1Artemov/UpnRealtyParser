using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpnSellFlatController : BaseController
    {
        private readonly EFGenericRepo<UpnFlat, RealtyParserContext> _upnFlatRepo;
        private readonly EFGenericRepo<UpnFlatVmForTable, RealtyParserContext> _upnFlatVmRepo;
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        private readonly EFGenericRepo<SubwayStation, RealtyParserContext> _subwayStationRepo;
        private readonly EFGenericRepo<UpnAgency, RealtyParserContext> _agencyRepo;
        private readonly EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        private readonly EFGenericRepo<UpnFlatPhoto, RealtyParserContext> _upnPhotoRepo;

        public UpnSellFlatController(EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo,
            EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<UpnAgency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<UpnFlatPhoto, RealtyParserContext> upnPhotoRepo,
            EFGenericRepo<UpnFlatVmForTable, RealtyParserContext> upnFlatVmRepo)
        {
            _upnFlatRepo = upnFlatRepo;
            _upnHouseRepo = upnHouseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
            _pageLinkRepo = pageLinkRepo;
            _upnPhotoRepo = upnPhotoRepo;
            _upnFlatVmRepo = upnFlatVmRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllFlats(int? page, int? pageSize, bool? isShowArchived, bool? isExcludeFirstFloor,
            bool? isExcludeLastFloor, int? minPrice, int? maxPrice, int? minBuildYear, int? maxSubwayDistance,
            int? closestSubwayStationId)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

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
                SubwayStation foundStation = _subwayStationRepo.GetAllWithoutTracking().FirstOrDefault(x => x.Id == closestSubwayStationId.Value);
                string stationName = foundStation?.Name;
                allSellFlats = allSellFlats.Where(x => x.ClosestSubwayName == stationName);
            }
            int totalCount = allSellFlats.Count();

            List<UpnFlatVmForTable> filteredFlats = allSellFlats
                .OrderBy(x => x.Id)
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new {flatsList = filteredFlats, totalCount = totalCount});
        }

        [Route("getsingle")]
        [HttpGet]
        public IActionResult GetSingle(int? id)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID квартиры");

            UpnFlat foundFlat = _upnFlatRepo.GetWithoutTracking(x => x.Id == id.Value);
            if (foundFlat == null)
                return makeErrorResult(string.Format("не найдена квартира с ID = {0}", id.Value));

            UpnApartmentHelper apartmentHelper = new UpnApartmentHelper(_upnHouseRepo, _subwayStationRepo, _agencyRepo,
                _pageLinkRepo, _upnPhotoRepo);
            apartmentHelper.FillSingleApartmentWithAdditionalInfo(foundFlat);
            apartmentHelper.FillSingleApartmentWithPhotoHrefs(foundFlat);

            return Json(new {flatInfo = foundFlat});
        }
    }
}