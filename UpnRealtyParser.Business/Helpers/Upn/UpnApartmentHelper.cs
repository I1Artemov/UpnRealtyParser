using System.Collections.Generic;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnApartmentHelper : BaseApartementHelper<UpnHouseInfo, UpnFlat, UpnRentFlat, UpnAgency, UpnFlatPhoto>
    {
        public UpnApartmentHelper(EFGenericRepo<UpnHouseInfo, RealtyParserContext> houseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<UpnAgency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<UpnFlatPhoto, RealtyParserContext> photoRepo,
            EFGenericRepo<ApartmentPayback, RealtyParserContext> apartmentPaybackRepo) 
            : base(houseRepo, subwayStationRepo, agencyRepo, pageLinkRepo, photoRepo, apartmentPaybackRepo)
        {
        }
    }
}