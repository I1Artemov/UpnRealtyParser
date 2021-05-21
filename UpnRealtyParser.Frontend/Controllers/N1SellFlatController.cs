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

        public N1SellFlatController(EFGenericRepo<N1Flat, RealtyParserContext> n1FlatRepo,
            EFGenericRepo<N1HouseInfo, RealtyParserContext> n1HouseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<N1Agency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<N1FlatPhoto, RealtyParserContext> upnPhotoRepo)
        {
            _n1FlatRepo = n1FlatRepo;
            _n1HouseRepo = n1HouseRepo;
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

        [Route("getsingle")]
        [HttpGet]
        public IActionResult GetSingle(int? id)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID квартиры");

            N1Flat foundFlat = _n1FlatRepo.GetWithoutTracking(x => x.Id == id.Value);
            if (foundFlat == null)
                return makeErrorResult(string.Format("не найдена квартира с ID = {0}", id.Value));

            N1ApartmentHelper apartmentHelper = new N1ApartmentHelper(_n1HouseRepo, _subwayStationRepo, _agencyRepo,
                _pageLinkRepo, _n1PhotoRepo);
            apartmentHelper.FillSingleApartmentWithAdditionalInfo(foundFlat);
            apartmentHelper.FillSingleApartmentWithPhotoHrefs(foundFlat);

            return Json(new { flatInfo = foundFlat });
        }
    }
}