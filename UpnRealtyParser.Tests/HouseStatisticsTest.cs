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
        public void paybackPeriodMap_StatisticsCount_Test()
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
                    new HouseStatisticsCalculator<UpnFlat, UpnRentFlat, UpnHouseInfo>(upnFlatRepo, upnRentFlatRepo, upnHouseRepo, statsRepo);

                var paybackPoints = calculator.GetPaybackPeriodPoints();
            }
        }
    }
}
