using Microsoft.AspNetCore.Mvc;
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
    public class UpnSellFlatController : Controller
    {
        private readonly EFGenericRepo<UpnFlat, RealtyParserContext> _upnFlatRepo;
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        private readonly EFGenericRepo<SubwayStation, RealtyParserContext> _subwayStationRepo;
        private readonly EFGenericRepo<UpnAgency, RealtyParserContext> _agencyRepo;

        public UpnSellFlatController(EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo,
            EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<UpnAgency, RealtyParserContext> agencyRepo)
        {
            _upnFlatRepo = upnFlatRepo;
            _upnHouseRepo = upnHouseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllFlats(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<UpnFlat> allSellFlats = _upnFlatRepo.GetAllWithoutTracking();
            int totalCount = allSellFlats.Count();

            List<UpnFlat> filteredFlats = allSellFlats
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            UpnApartmentHelper apartmentHelper = new UpnApartmentHelper(_upnHouseRepo, _subwayStationRepo, _agencyRepo);
            apartmentHelper.FillApartmentsWithAdditionalInfo(filteredFlats);

            return Json(new {flatsList = filteredFlats, totalCount = totalCount});
        }
    }
}