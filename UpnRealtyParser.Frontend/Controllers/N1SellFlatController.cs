using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class N1SellFlatController : BaseController
    {
        private readonly EFGenericRepo<N1Flat, RealtyParserContext> _n1FlatRepo;
        private readonly EFGenericRepo<N1HouseInfo, RealtyParserContext> _n1HouseRepo;
        private readonly EFGenericRepo<SubwayStation, RealtyParserContext> _subwayStationRepo;
        private readonly EFGenericRepo<N1Agency, RealtyParserContext> _agencyRepo;
        private readonly EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        private readonly EFGenericRepo<N1FlatPhoto, RealtyParserContext> _n1PhotoRepo;

        public N1SellFlatController(EFGenericRepo<N1Flat, RealtyParserContext> upnFlatRepo,
            EFGenericRepo<N1HouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<N1Agency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<N1FlatPhoto, RealtyParserContext> upnPhotoRepo)
        {
            _n1FlatRepo = upnFlatRepo;
            _n1HouseRepo = upnHouseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
            _pageLinkRepo = pageLinkRepo;
            _n1PhotoRepo = upnPhotoRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllFlats(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<N1Flat> allSellFlats = _n1FlatRepo.GetAllWithoutTracking();
            int totalCount = allSellFlats.Count();

            List<N1Flat> filteredFlats = allSellFlats
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            N1ApartmentHelper apartmentHelper = new N1ApartmentHelper(_n1HouseRepo, _subwayStationRepo, _agencyRepo,
                _pageLinkRepo, _n1PhotoRepo);
            apartmentHelper.FillSellApartmentsWithAdditionalInfo(filteredFlats);

            return Json(new { flatsList = filteredFlats, totalCount = totalCount });
        }
    }
}