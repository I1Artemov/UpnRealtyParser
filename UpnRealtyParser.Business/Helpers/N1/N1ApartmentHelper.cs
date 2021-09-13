using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1ApartmentHelper : BaseApartementHelper<N1HouseInfo, N1Flat, N1Flat, N1Agency, N1FlatPhoto>
    {
        public N1ApartmentHelper(EFGenericRepo<N1HouseInfo, RealtyParserContext> houseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<N1Agency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<N1FlatPhoto, RealtyParserContext> photoRepo,
            EFGenericRepo<ApartmentPayback, RealtyParserContext> apartmentPaybackRepo) :
            base(houseRepo, subwayStationRepo, agencyRepo, pageLinkRepo, photoRepo, apartmentPaybackRepo)
        {
        }
    }
}
