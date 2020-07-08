using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class SubwayDistanceTest
    {
        [Fact]
        public void HousesToClosestSubway_InDb_Test()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                DistanceCalculator calculator = new DistanceCalculator(realtyContext);
                calculator.CalculateDistanceFromHousesToClosestSubway();
            }
        }
    }
}
