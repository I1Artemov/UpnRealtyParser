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
            }
        }
    }
}
