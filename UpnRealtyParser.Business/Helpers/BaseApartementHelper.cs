using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class BaseApartementHelper<THouse, TSellFlat, TRentFlat, TAgency, TFlatPhoto>
        where THouse : HouseInfoCore
        where TSellFlat : FlatCore
        where TRentFlat : FlatCore
        where TAgency : AgencyCore
        where TFlatPhoto : FlatPhotoBase
    {
        private readonly EFGenericRepo<THouse, RealtyParserContext> _houseRepo;
        private readonly EFGenericRepo<SubwayStation, RealtyParserContext> _subwayStationRepo;
        private readonly EFGenericRepo<TAgency, RealtyParserContext> _agencyRepo;
        private readonly EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        private readonly EFGenericRepo<TFlatPhoto, RealtyParserContext> _photoRepo;

        public BaseApartementHelper(EFGenericRepo<THouse, RealtyParserContext> houseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<TAgency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<TFlatPhoto, RealtyParserContext> photoRepo)
        {
            _houseRepo = houseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
            _pageLinkRepo = pageLinkRepo;
            _photoRepo = photoRepo;
        }

        /// <summary>
        /// Заполняет квартиры на продажу информацией, взятой из данных о доме и об агентстве, для отображения в таблице
        /// </summary>
        public void FillSellApartmentsWithAdditionalInfo(List<TSellFlat> flats)
        {
            if (flats == null || flats.Count == 0)
                return;

            foreach (FlatCore flat in flats)
            {
                FillSingleApartmentWithAdditionalInfo(flat);
            }
        }

        /// <summary>
        /// Заполняет одну квартиру информацией, взятой из данных о доме и об агентстве, для отображения в таблице
        /// </summary>
        public void FillSingleApartmentWithAdditionalInfo(FlatCore flat)
        {
            if (flat == null)
                return;

            fillHouseRelatedFields(flat);

            if (flat.AgencyId.HasValue)
            {
                TAgency agency = _agencyRepo.GetWithoutTracking(x => x.Id == flat.AgencyId);
                flat.AgencyName = agency?.Name;
            }

            PageLink foundLink = _pageLinkRepo.GetWithoutTracking(x => x.Id == flat.PageLinkId);
            if (foundLink != null) { 
                flat.SiteUrl = foundLink.Href;
            }
            if(flat.RemovalDate.HasValue || (DateTime.Now - flat.LastCheckDate.GetValueOrDefault(DateTime.MinValue)).Days > 7 ||
                (foundLink != null && foundLink.IsDead.GetValueOrDefault(false)))
            {
                flat.IsArchived = true;
            }

            flat.PhotoCount = _photoRepo
                .GetAllWithoutTracking()
                .Count(x => x.FlatId == flat.Id);
        }

        /// <summary>
        /// Заполняет квартиру списком ссылок на фото по ней
        /// </summary>
        /// <param name="upnFlat">Квартира для заполнения</param>
        /// <param name="relationType">SellFlat или RentFlat</param>
        public void FillSingleApartmentWithPhotoHrefs(FlatCore upnFlat, string relationType = Const.LinkTypeSellFlat)
        {
            List<string> photoHrefs = _photoRepo.GetAllWithoutTracking()
                .Where(x => x.FlatId == upnFlat.Id && x.RelationType == relationType)
                .Select(x => x.Href)
                .ToList();

            upnFlat.PhotoHrefs = photoHrefs;
        }

        private void fillHouseRelatedFields(FlatCore singleFlat)
        {
            if (!singleFlat.HouseInfoId.HasValue)
                return;

            THouse foundHouse = _houseRepo.GetWithoutTracking(x => x.Id == singleFlat.HouseInfoId);
            if (foundHouse == null)
                return;

            singleFlat.HouseAddress = foundHouse.Address;
            singleFlat.HouseBuildYear = foundHouse.BuildYear;
            singleFlat.HouseMaxFloor = foundHouse.MaxFloor;
            singleFlat.HouseWallMaterial = foundHouse.WallMaterial;
            singleFlat.HouseType = foundHouse.HouseType;
            singleFlat.HouseLatitude = foundHouse.Latitude;
            singleFlat.HouseLongitude = foundHouse.Longitude;

            if (!foundHouse.ClosestSubwayStationId.HasValue)
                return;

            SubwayStation foundStation = _subwayStationRepo
                .GetWithoutTracking(x => x.Id == foundHouse.ClosestSubwayStationId);

            if (foundStation == null)
                return;

            singleFlat.ClosestSubwayName = foundStation.Name;
            singleFlat.ClosestSubwayRangeStr = Math.Round(foundHouse.ClosestSubwayStationRange.Value).ToString();
        }
    }
}
