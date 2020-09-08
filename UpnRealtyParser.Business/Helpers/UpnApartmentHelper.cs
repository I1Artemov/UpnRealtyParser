using System;
using System.Collections.Generic;
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

        public UpnApartmentHelper(EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo,
            EFGenericRepo<SubwayStation, RealtyParserContext> subwayStationRepo,
            EFGenericRepo<UpnAgency, RealtyParserContext> agencyRepo)
        {
            _upnHouseRepo = upnHouseRepo;
            _agencyRepo = agencyRepo;
            _subwayStationRepo = subwayStationRepo;
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