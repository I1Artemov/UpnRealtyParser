using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;
using UpnRealtyParser.Business;

namespace UpnRealtyParser.Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpnHouseController : BaseController
    {
        private readonly EFGenericRepo<HouseSitelessVM, RealtyParserContext> _unitedHouseRepo;
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        private readonly EFGenericRepo<UpnFlat, RealtyParserContext> _upnSellFlatRepo;
        private readonly EFGenericRepo<UpnRentFlat, RealtyParserContext> _upnRentFlatRepo;
        private readonly EFGenericRepo<AveragePriceStat, RealtyParserContext> _statsRepo;

        public UpnHouseController(EFGenericRepo<HouseSitelessVM, RealtyParserContext> unitedHouseRepo,
            EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<UpnFlat, RealtyParserContext> upnSellFlatRepo,
            EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo,
            EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo)
        {
            _unitedHouseRepo = unitedHouseRepo;
            _upnHouseRepo = upnHouseRepo;
            _upnSellFlatRepo = upnSellFlatRepo;
            _upnRentFlatRepo = upnRentFlatRepo;
            _statsRepo = statsRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllHouses(int? page, int? pageSize, int? minBuildYear, bool? isShowUpn,
            bool? isShowN1, string addressPart)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(20);

            IQueryable<HouseSitelessVM> allHouses = _unitedHouseRepo.GetAllWithoutTracking();
            if (minBuildYear.HasValue)
                allHouses = allHouses.Where(x => x.BuildYear >= minBuildYear.Value);
            if (!isShowUpn.GetValueOrDefault(true))
                allHouses = allHouses.Where(x => x.SourceSite != Const.SiteNameUpn);
            if (!isShowN1.GetValueOrDefault(true))
                allHouses = allHouses.Where(x => x.SourceSite != Const.SiteNameN1);
            if (!string.IsNullOrEmpty(addressPart))
                allHouses = allHouses.Where(x => x.Address.Contains(addressPart));

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

            HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> calculator = 
                new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(
                    _upnSellFlatRepo, _upnRentFlatRepo, _upnHouseRepo, _statsRepo);
            HouseStatistics houseStatistics = calculator.GetStatisticsForHouse(id.Value);

            return Json(new { houseStatistics });
        }

        [Route("getsingle/averageprice/points")]
        [HttpGet]
        public IActionResult GetSingleHouseAveragePricePlotPoints(int? id)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID дома");

            HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> calculator =
                new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(
                    _upnSellFlatRepo, _upnRentFlatRepo, _upnHouseRepo, _statsRepo);

            List<PointDateTimeWithValue> points = 
                calculator.GetAveragePriceForMonthsPoints(
                    id.Value, new DateTime(2020, 01, 01), DateTime.Now, Const.SiteNameUpn);

            return Json(new { points });
        }

        [Route("payback/points")]
        [HttpGet]
        public IActionResult GetAllPaybackMapPoints()
        {
            HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> calculator =
                new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(
                    _upnSellFlatRepo, _upnRentFlatRepo, _upnHouseRepo, _statsRepo);

            List<PaybackPeriodPoint> points = calculator.GetPaybackPeriodPoints();

            return Json(new { points });
        }
    }
}