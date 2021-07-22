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
            EFGenericRepo<UpnFlatPhoto, RealtyParserContext> photoRepo) 
            : base(houseRepo, subwayStationRepo, agencyRepo, pageLinkRepo, photoRepo)
        {
        }

        /// <summary>
        /// Заполняет квартиры под аренду информацией, взятой из данных о доме и об агентстве, для отображения в таблице
        /// </summary>
        public void FillRentApartmentsWithAdditionalInfo(List<UpnRentFlat> upnFlats)
        {
            if (upnFlats == null || upnFlats.Count == 0)
                return;

            foreach (UpnRentFlat upnFlat in upnFlats)
            {
                FillSingleApartmentWithAdditionalInfo(upnFlat);
            }
        }
    }
}