using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class UpnApartmentHelper
    {
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        private readonly EFGenericRepo<SubwayStation, RealtyParserContext> _subwayStationRepo;
        private readonly EFGenericRepo<UpnAgency, RealtyParserContext> _agencyRepo;
        private readonly EFGenericRepo<PageLink, RealtyParserContext> _pageLinkRepo;
        private readonly EFGenericRepo<UpnFlatPhoto, RealtyParserContext> _upnPhotoRepo;

        public UpnApartmentHelper(EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<UpnAgency, RealtyParserContext> agencyRepo,
            EFGenericRepo<PageLink, RealtyParserContext> pageLinkRepo,
            EFGenericRepo<UpnFlatPhoto, RealtyParserContext> upnPhotoRepo)
        {
            _upnHouseRepo = upnHouseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
            _pageLinkRepo = pageLinkRepo;
            _upnPhotoRepo = upnPhotoRepo;
        }

        /// <summary>
        /// Заполняет квартиры информацией, взятой из данных о доме и об агентстве, для отображения в таблице
        /// </summary>
        public void FillApartmentsWithAdditionalInfo(List<UpnFlat> upnFlats)
        {
            if (upnFlats == null || upnFlats.Count == 0)
                return;

            foreach (UpnFlat upnFlat in upnFlats)
            {
                FillSingleApartmentWithAdditionalInfo(upnFlat);
            }
        }

        /// <summary>
        /// Заполняет одну квартиру информацией, взятой из данных о доме и об агентстве, для отображения в таблице
        /// </summary>
        public void FillSingleApartmentWithAdditionalInfo(UpnFlat upnFlat)
        {
            if (upnFlat == null)
                return;

            fillHouseRelatedFields(upnFlat);

            if (upnFlat.UpnAgencyId.HasValue)
            {
                UpnAgency agency = _agencyRepo.GetWithoutTracking(x => x.Id == upnFlat.UpnAgencyId);
                upnFlat.AgencyName = agency?.Name;
            }

            PageLink foundLink = _pageLinkRepo.GetWithoutTracking(x => x.Id == upnFlat.Id);
            if(foundLink != null)
                upnFlat.SiteUrl = foundLink.Href;

            upnFlat.PhotoCount = _upnPhotoRepo
                .GetAllWithoutTracking()
                .Count(x => x.FlatId == upnFlat.Id);
        }

        /// <summary>
        /// Заполняет квартиру списком ссылок на фото по ней
        /// </summary>
        /// <param name="upnFlat">Квартира для заполнения</param>
        /// <param name="relationType">SellFlat или RentFlat</param>
        public void FillSingleApartmentWithPhotoHrefs(UpnFlat upnFlat, string relationType = Const.LinkTypeSellFlat)
        {
            List<string> photoHrefs = _upnPhotoRepo.GetAllWithoutTracking()
                .Where(x => x.FlatId == upnFlat.Id && x.RelationType == relationType)
                .Select(x => x.Href)
                .ToList();

            upnFlat.PhotoHrefs = photoHrefs;
        }

        private void fillHouseRelatedFields(UpnFlat upnFlat)
        {
            if (!upnFlat.UpnHouseInfoId.HasValue)
                return;

            UpnHouseInfo foundHouse = _upnHouseRepo.GetWithoutTracking(x => x.Id == upnFlat.UpnHouseInfoId);
            if (foundHouse == null)
                return;

            upnFlat.HouseAddress = foundHouse.Address;
            upnFlat.HouseBuildYear = foundHouse.BuildYear;
            upnFlat.HouseMaxFloor = foundHouse.MaxFloor;
            upnFlat.HouseWallMaterial = foundHouse.WallMaterial;
            upnFlat.HouseType = foundHouse.HouseType;
            upnFlat.HouseLatitude = foundHouse.Latitude;
            upnFlat.HouseLongitude = foundHouse.Longitude;

            if (!foundHouse.ClosestSubwayStationId.HasValue)
                return;

            SubwayStation foundStation = _subwayStationRepo
                .GetWithoutTracking(x => x.Id == foundHouse.ClosestSubwayStationId);

            if (foundStation == null)
                return;

            upnFlat.ClosestSubwayName = foundStation.Name;
            upnFlat.ClosestSubwayRangeStr = Math.Round(foundHouse.ClosestSubwayStationRange.Value).ToString();
        }
    }
}