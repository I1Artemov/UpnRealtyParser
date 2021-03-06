using System;
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

                HouseStatisticsCalculator<UpnFlat> calculator = new HouseStatisticsCalculator<UpnFlat>(upnFlatRepo);
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

                HouseStatisticsCalculator<UpnFlat> calculator = new HouseStatisticsCalculator<UpnFlat>(upnFlatRepo);
                var points = calculator.GetAveragePriceForMonthsPoints(1695, new DateTime(2020, 01, 01), new DateTime(2021, 04, 01), 1);
                Assert.Equal(12, points.Count);
            }
        }
    }
}
