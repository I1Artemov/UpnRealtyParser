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
    public class UpnHouseController : BaseController
    {
        private readonly EFGenericRepo<HouseSitelessVM, RealtyParserContext> _unitedHouseRepo;
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        private readonly EFGenericRepo<UpnFlat, RealtyParserContext> _upnSellFlatRepo;

        public UpnHouseController(EFGenericRepo<HouseSitelessVM, RealtyParserContext> unitedHouseRepo,
            EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<UpnFlat, RealtyParserContext> upnSellFlatRepo)
        {
            _unitedHouseRepo = unitedHouseRepo;
            _upnHouseRepo = upnHouseRepo;
            _upnSellFlatRepo = upnSellFlatRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllHouses(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<HouseSitelessVM> allHouses = _unitedHouseRepo.GetAllWithoutTracking();
            int totalCount = allHouses.Count();

            List<HouseSitelessVM> filteredHouses = allHouses
                .OrderByDescending(x => x.SimilarIdentity)
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { housesList = filteredHouses, totalCount = totalCount });
        }

        [Route("getsingle")]
        [HttpGet]
        public IActionResult GetSingleHouse(int? id)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID дома");

            UpnHouseInfo foundHouse = _upnHouseRepo.GetWithoutTracking(x => x.Id == id.Value);
            if (foundHouse == null)
                return makeErrorResult(string.Format("не найден дом с ID = {0}", id.Value));

            return Json(new { houseInfo = foundHouse });
        }

        [Route("getsinglestatistics")]
        [HttpGet]
        public IActionResult GetSingleHouseStatistics(int? id)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID дома");

            HouseStatisticsCalculator<UpnFlat> calculator = new HouseStatisticsCalculator<UpnFlat>(_upnSellFlatRepo);
            HouseStatistics houseStatistics = calculator.GetStatisticsForHouse(id.Value);

            return Json(new { houseStatistics });
        }
    }
}