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
    public class HouseController : BaseController
    {
        private readonly EFGenericRepo<HouseSitelessVM, RealtyParserContext> _unitedHouseRepo;
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        private readonly EFGenericRepo<N1HouseInfo, RealtyParserContext> _n1HouseRepo;
        private readonly EFGenericRepo<UpnFlat, RealtyParserContext> _upnSellFlatRepo;
        private readonly EFGenericRepo<UpnRentFlat, RealtyParserContext> _upnRentFlatRepo;
        private readonly EFGenericRepo<AveragePriceStat, RealtyParserContext> _statsRepo;
        private readonly EFGenericRepo<PaybackPeriodPoint, RealtyParserContext> _paybackPointsRepo;

        public HouseController(EFGenericRepo<HouseSitelessVM, RealtyParserContext> unitedHouseRepo,
            EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<N1HouseInfo, RealtyParserContext> n1HouseRepo,
            EFGenericRepo<UpnFlat, RealtyParserContext> upnSellFlatRepo,
            EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo,
            EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo,
            EFGenericRepo<PaybackPeriodPoint, RealtyParserContext> paybackPointsRepo)
        {
            _unitedHouseRepo = unitedHouseRepo;
            _upnHouseRepo = upnHouseRepo;
            _n1HouseRepo = n1HouseRepo;
            _upnSellFlatRepo = upnSellFlatRepo;
            _upnRentFlatRepo = upnRentFlatRepo;
            _statsRepo = statsRepo;
            _paybackPointsRepo = paybackPointsRepo;
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
        public IActionResult GetSingleHouse(int? id, string siteName)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID дома");

            if(string.IsNullOrEmpty(siteName) || siteName.ToLower() == "upn")
            {
                UpnHouseInfo upnFoundHouse = _upnHouseRepo.GetWithoutTracking(x => x.Id == id.Value);
                if (upnFoundHouse == null)
                    return makeErrorResult(string.Format("не найден дом UPN с ID = {0}", id.Value));
                return Json(new { houseInfo = upnFoundHouse });
            }

            UpnHouseInfo n1FoundHouse = _upnHouseRepo.GetWithoutTracking(x => x.Id == id.Value);
            if (n1FoundHouse == null)
                return makeErrorResult(string.Format("не найден дом N1 с ID = {0}", id.Value));
            return Json(new { houseInfo = n1FoundHouse });
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

            List<PaybackPeriodPoint> points = _paybackPointsRepo
                .GetAllWithoutTracking().ToList();

            return Json(new { points });
        }
    }
}