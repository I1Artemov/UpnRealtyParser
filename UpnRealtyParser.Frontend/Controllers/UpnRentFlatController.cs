﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpnRentFlatController : BaseController
    {
        private readonly EFGenericRepo<UpnRentFlat, RealtyParserContext> _upnFlatRepo;
        private readonly EFGenericRepo<UpnRentFlatVmForTable, RealtyParserContext> _upnRentFlatVmRepo;
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        private readonly EFGenericRepo<SubwayStation, RealtyParserContext> _subwayStationRepo;
        private readonly EFGenericRepo<UpnAgency, RealtyParserContext> _agencyRepo;
        private readonly EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        private readonly EFGenericRepo<UpnFlatPhoto, RealtyParserContext> _upnPhotoRepo;
        private readonly EFGenericRepo<ApartmentPayback, RealtyParserContext> _apartmentPaybackRepo;

        public UpnRentFlatController(EFGenericRepo<UpnRentFlat, RealtyParserContext> upnFlatRepo,
            EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<UpnAgency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<UpnFlatPhoto, RealtyParserContext> upnPhotoRepo,
            EFGenericRepo<UpnRentFlatVmForTable, RealtyParserContext> upnRentFlatVmRepo,
            EFGenericRepo<ApartmentPayback, RealtyParserContext> apartmentPaybackRepo)
        {
            _upnFlatRepo = upnFlatRepo;
            _upnHouseRepo = upnHouseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
            _pageLinkRepo = pageLinkRepo;
            _upnPhotoRepo = upnPhotoRepo;
            _upnRentFlatVmRepo = upnRentFlatVmRepo;
            _apartmentPaybackRepo = apartmentPaybackRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllFlats([FromQuery]FlatsFilterOrderParameters filterParams)
        {
            int targetPage = filterParams.Page.GetValueOrDefault(1);
            int targetPageSize = filterParams.PageSize.GetValueOrDefault(10);


            UpnApartmentHelper apartmentHelper = new UpnApartmentHelper(_upnHouseRepo, _subwayStationRepo, _agencyRepo,
                _pageLinkRepo, _upnPhotoRepo);
            IQueryable<UpnRentFlatVmForTable> allRentFlats = apartmentHelper
                .GetFilteredAndOrderedFlats(filterParams, _upnRentFlatVmRepo);

            int totalCount = allRentFlats.Count();

            List<UpnRentFlatVmForTable> filteredFlats = allRentFlats
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { flatsList = filteredFlats, totalCount = totalCount });
        }

        [Route("getsingle")]
        [HttpGet]
        public IActionResult GetSingle(int? id)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID квартиры");

            UpnRentFlat foundFlat = _upnFlatRepo.GetWithoutTracking(x => x.Id == id.Value);
            if (foundFlat == null)
                return makeErrorResult(string.Format("не найдена квартира с ID = {0}", id.Value));

            UpnApartmentHelper apartmentHelper = new UpnApartmentHelper(_upnHouseRepo, _subwayStationRepo, _agencyRepo,
                _pageLinkRepo, _upnPhotoRepo);
            apartmentHelper.FillSingleApartmentWithAdditionalInfo(foundFlat);
            apartmentHelper.FillSingleApartmentWithPhotoHrefs(foundFlat, Const.LinkTypeRentFlat);

            return Json(new { flatInfo = foundFlat });
        }
    }
}