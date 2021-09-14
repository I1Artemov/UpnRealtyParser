using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class ApartmentStatisticsTest
    {
        [Fact(Skip = "Real data")]
        public async void CalculateUpnFlatsPaybackPeriodTest()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo =
                    new EFGenericRepo<UpnFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo =
                    new EFGenericRepo<UpnRentFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<N1RentFlat, RealtyParserContext> n1RentFlatRepo =
                    new EFGenericRepo<N1RentFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<SimilarHouse, RealtyParserContext> similarHouseRepo =
                    new EFGenericRepo<SimilarHouse, RealtyParserContext>(realtyContext);
                EFGenericRepo<ApartmentPayback, RealtyParserContext> apartmentPaybackRepo =
                    new EFGenericRepo<ApartmentPayback, RealtyParserContext>(realtyContext);

                PaybackApartmentCalculator<UpnFlat> calculator = new PaybackApartmentCalculator<UpnFlat>(Const.SiteNameUpn,
                    upnFlatRepo, upnRentFlatRepo, n1RentFlatRepo, similarHouseRepo, apartmentPaybackRepo, null);

                await calculator.CalculateAllUpnPaybackPeriods();
            }
        }

        [Fact]
        public async void CalculateN1FlatsPaybackPeriodTest()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<N1Flat, RealtyParserContext> n1FlatRepo =
                    new EFGenericRepo<N1Flat, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo =
                    new EFGenericRepo<UpnRentFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<N1RentFlat, RealtyParserContext> n1RentFlatRepo =
                    new EFGenericRepo<N1RentFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<SimilarHouse, RealtyParserContext> similarHouseRepo =
                    new EFGenericRepo<SimilarHouse, RealtyParserContext>(realtyContext);
                EFGenericRepo<ApartmentPayback, RealtyParserContext> apartmentPaybackRepo =
                    new EFGenericRepo<ApartmentPayback, RealtyParserContext>(realtyContext);

                PaybackApartmentCalculator<N1Flat> calculator = new PaybackApartmentCalculator<N1Flat>(Const.SiteNameN1,
                    n1FlatRepo, upnRentFlatRepo, n1RentFlatRepo, similarHouseRepo, apartmentPaybackRepo, null);

                await calculator.CalculateAllN1PaybackPeriods();
            }
        }
    }
}
