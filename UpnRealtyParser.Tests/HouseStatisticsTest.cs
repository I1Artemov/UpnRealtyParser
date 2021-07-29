using System;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class HouseStatisticsTest
    {
        [Fact]
        public void CountStatistics_ForUpnHouse_Test()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo = 
                    new EFGenericRepo<UpnFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo =
                    new EFGenericRepo<UpnRentFlat, RealtyParserContext>(realtyContext);

                EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo =
                    new EFGenericRepo<AveragePriceStat, RealtyParserContext>(realtyContext);

                HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> calculator =
                    new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(upnFlatRepo, upnRentFlatRepo, null, statsRepo);

                var stats = calculator.GetStatisticsForHouse(1695);
                Assert.NotNull(stats);
            }
        }

        [Fact]
        public void GetPoints_ForPriceForMonth_Test()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo =
                    new EFGenericRepo<UpnFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo =
                    new EFGenericRepo<UpnRentFlat, RealtyParserContext>(realtyContext);

                EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo =
                    new EFGenericRepo<AveragePriceStat, RealtyParserContext>(realtyContext);

                HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> calculator =
                    new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(upnFlatRepo, upnRentFlatRepo, null, statsRepo);

                var points = calculator.GetAveragePriceForMonthsPoints(1695, new DateTime(2020, 01, 01), new DateTime(2021, 04, 01),
                    Const.SiteNameUpn);
                Assert.Equal(12, points.Count);
            }
        }

        [Fact (Skip = "Долгое вычисление")]
        public void CountStatistics_ForAllUpnHouses_Test()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo =
                    new EFGenericRepo<UpnFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo =
                    new EFGenericRepo<UpnRentFlat, RealtyParserContext>(realtyContext);

                EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo =
                    new EFGenericRepo<UpnHouseInfo, RealtyParserContext>(realtyContext);

                EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo =
                    new EFGenericRepo<AveragePriceStat, RealtyParserContext>(realtyContext);

                HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo> calculator =
                    new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(upnFlatRepo, upnRentFlatRepo, null, statsRepo);

                calculator.CalculateAllUpnHouseAvgPricesAndSaveToDb(Const.SiteNameUpn);
            }
        }

        [Fact]
        public void PaybackPeriodMap_AllStatisticsCount_Test()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                calculateUpnPaybackPeriods(realtyContext);
                calculateN1PaybackPeriods(realtyContext);
            }
        }

        private void calculateUpnPaybackPeriods(RealtyParserContext realtyContext)
        {
            var upnFlatRepo = new EFGenericRepo<UpnFlat, RealtyParserContext>(realtyContext);
            var upnRentFlatRepo = new EFGenericRepo<UpnRentFlat, RealtyParserContext>(realtyContext);

            var upnHouseRepo = new EFGenericRepo<UpnHouseInfo, RealtyParserContext>(realtyContext);

            var statsRepo = new EFGenericRepo<AveragePriceStat, RealtyParserContext>(realtyContext);
            var paybackPointsRepo = new EFGenericRepo<PaybackPeriodPoint, RealtyParserContext>(realtyContext);

            var calculator = new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(upnFlatRepo, upnRentFlatRepo,
                    upnHouseRepo, statsRepo, paybackPointsRepo);

            calculator.CalculateAllPaybackPeriodPoints(Const.SiteNameUpn.ToLower());
        }

        private void calculateN1PaybackPeriods(RealtyParserContext realtyContext)
        {
            var upnFlatRepo = new EFGenericRepo<N1Flat, RealtyParserContext>(realtyContext);
            var upnRentFlatRepo = new EFGenericRepo<N1RentFlat, RealtyParserContext>(realtyContext);

            var upnHouseRepo = new EFGenericRepo<N1HouseInfo, RealtyParserContext>(realtyContext);

            var statsRepo = new EFGenericRepo<AveragePriceStat, RealtyParserContext>(realtyContext);
            var paybackPointsRepo = new EFGenericRepo<PaybackPeriodPoint, RealtyParserContext>(realtyContext);

            var calculator = new HouseStatisticsCalculator<N1Flat, N1RentFlat, N1HouseInfo>(upnFlatRepo, upnRentFlatRepo,
                    upnHouseRepo, statsRepo, paybackPointsRepo);

            calculator.CalculateAllPaybackPeriodPoints(Const.SiteNameN1.ToLower());
        }
    }
}
