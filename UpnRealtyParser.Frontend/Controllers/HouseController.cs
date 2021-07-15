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
        private readonly EFGenericRepo<N1Flat, RealtyParserContext> _n1SellFlatRepo;
        private readonly EFGenericRepo<N1RentFlat, RealtyParserContext> _n1RentFlatRepo;
        private readonly EFGenericRepo<AveragePriceStat, RealtyParserContext> _statsRepo;
        private readonly EFGenericRepo<PaybackPeriodPoint, RealtyParserContext> _paybackPointsRepo;

        public HouseController(EFGenericRepo<HouseSitelessVM, RealtyParserContext> unitedHouseRepo,
            EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<N1HouseInfo, RealtyParserContext> n1HouseRepo,
            EFGenericRepo<UpnFlat, RealtyParserContext> upnSellFlatRepo,
            EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo,
            EFGenericRepo<N1Flat, RealtyParserContext> n1SellFlatRepo,
            EFGenericRepo<N1RentFlat, RealtyParserContext> n1RentFlatRepo,
            EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo,
            EFGenericRepo<PaybackPeriodPoint, RealtyParserContext> paybackPointsRepo)
        {
            _unitedHouseRepo = unitedHouseRepo;
            _upnHouseRepo = upnHouseRepo;
            _n1HouseRepo = n1HouseRepo;
            _upnSellFlatRepo = upnSellFlatRepo;
            _upnRentFlatRepo = upnRentFlatRepo;
            _n1SellFlatRepo = n1SellFlatRepo;
            _n1RentFlatRepo = n1RentFlatRepo;
            _statsRepo = statsRepo;
            _paybackPointsRepo = paybackPointsRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllHouses(int? page, int? pageSize, int? minBuildYear, bool? isShowUpn,
            bool? isShowN1, string addressPart, string sortField, string sortOrder)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(20);

            BaseHouseHelper houseHelper = new BaseHouseHelper(_unitedHouseRepo);
            IQueryable<HouseSitelessVM> allHouses = houseHelper
                .GetFilteredAndOrderedHouses(minBuildYear, isShowUpn, isShowN1, addressPart, sortField, sortOrder);
            int totalCount = allHouses.Count();

            List<HouseSitelessVM> filteredHouses = allHouses
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
                findAndSetSimilarHouseId(upnFoundHouse, siteName.ToUpper());
                return Json(new { houseInfo = upnFoundHouse });
            }

            N1HouseInfo n1FoundHouse = _n1HouseRepo.GetWithoutTracking(x => x.Id == id.Value);
            if (n1FoundHouse == null)
                return makeErrorResult(string.Format("не найден дом N1 с ID = {0}", id.Value));
            findAndSetSimilarHouseId(n1FoundHouse, siteName.ToUpper());
            return Json(new { houseInfo = n1FoundHouse });
        }

        [Route("getsinglestatistics")]
        [HttpGet]
        public IActionResult GetSingleHouseStatistics(int? id, string siteName)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID дома");

            HouseStatistics houseStatistics = null;
            if (string.IsNullOrEmpty(siteName) || siteName.ToLower() == "upn")
            {
                HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> upnCalculator =
                    new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(
                        _upnSellFlatRepo, _upnRentFlatRepo, _upnHouseRepo, _statsRepo);
                houseStatistics = upnCalculator.GetStatisticsForHouse(id.Value);
            }
            else
            {
                HouseStatisticsCalculator<N1Flat, N1RentFlat, N1HouseInfo> n1Calculator =
                    new HouseStatisticsCalculator<N1Flat, N1RentFlat, N1HouseInfo>(
                        _n1SellFlatRepo, _n1RentFlatRepo, _n1HouseRepo, _statsRepo);
                houseStatistics = n1Calculator.GetStatisticsForHouse(id.Value);
            }

            return Json(new { houseStatistics });
        }

        [Route("getsingle/averageprice/points")]
        [HttpGet]
        public IActionResult GetSingleHouseAveragePricePlotPoints(int? id, string siteName)
        {
            if (!id.HasValue)
                return makeErrorResult("Не указан ID дома");

            if(siteName != "upn")
                return Json(new { points = new List<PointDateTimeWithValue>() });

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
        public IActionResult GetAllPaybackMapPoints(double? paybackLimit)
        {
            HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> calculator =
                new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(
                    _upnSellFlatRepo, _upnRentFlatRepo, _upnHouseRepo, _statsRepo);

            if (!paybackLimit.HasValue)
                paybackLimit = 80.0f;

            List<PaybackPeriodPoint> points = _paybackPointsRepo
                .GetAllWithoutTracking()
                .Where(x => x.PaybackYears < paybackLimit.Value).ToList();

            return Json(new { points });
        }

        /// <summary>
        /// У дома targetHouse заполняет ID похожего дома, собранного с другого сайта
        /// </summary>
        protected void findAndSetSimilarHouseId(HouseInfoCore targetHouse, string siteName)
        {
            if (_unitedHouseRepo == null || targetHouse == null)
                return;

            HouseSitelessVM targetHouseWithIdentity = _unitedHouseRepo.GetAllWithoutTracking()
                .FirstOrDefault(x => x.Id == targetHouse.Id && x.SourceSite == siteName);

            if (targetHouseWithIdentity?.SimilarIdentity == null)
                return;

            HouseSitelessVM similarHouse = _unitedHouseRepo.GetAllWithoutTracking()
                .FirstOrDefault(x => x.SimilarIdentity == targetHouseWithIdentity.SimilarIdentity
                                && x.SourceSite != siteName);

            targetHouse.SimilarHouseFromDifferentSiteId = similarHouse?.Id;
        }
    }
}