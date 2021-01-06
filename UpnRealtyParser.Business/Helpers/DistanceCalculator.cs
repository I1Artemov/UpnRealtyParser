using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class DistanceCalculator
    {
        protected EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;
        protected EFGenericRepo<N1HouseInfo, RealtyParserContext> _n1HouseRepo;
        protected EFGenericRepo<SubwayStation, RealtyParserContext> _subwayRepo;
        protected EFGenericRepo<SimilarHouse, RealtyParserContext> _similarHouseRepo;
        List<SubwayStation> _subwayStations;

        public DistanceCalculator(RealtyParserContext dbContext)
        {
            _upnHouseRepo = new EFGenericRepo<UpnHouseInfo, RealtyParserContext>(dbContext);
            _n1HouseRepo = new EFGenericRepo<N1HouseInfo, RealtyParserContext>(dbContext);
            _subwayRepo = new EFGenericRepo<SubwayStation, RealtyParserContext>(dbContext);
            _similarHouseRepo = new EFGenericRepo<SimilarHouse, RealtyParserContext>(dbContext);
            _subwayStations = _subwayRepo.GetAllWithoutTracking()
                .ToList();
        }

        /// <summary>
        /// Берет все дома из БД, для каждого находит ближайшую станцию метро и расстояние до нее (если еще не заполнено).
        /// Вычисления сохраняет в БД
        /// </summary>
        public void CalculateDistanceFromUpnHousesToClosestSubway()
        {
            int housesPerGathering = 1000;
            int skipAmount = 0;
            int totalHouses = _upnHouseRepo.GetAllWithoutTracking()
                .Where(x => x.ClosestSubwayStationId == null)
                .Count();

            for (skipAmount = 0; skipAmount < totalHouses; skipAmount += housesPerGathering)
            {
                List<UpnHouseInfo> houses = _upnHouseRepo.GetAll()
                    .Where(x => x.ClosestSubwayStationId == null)
                    .Skip(skipAmount)
                    .Take(housesPerGathering)
                    .ToList();

                foreach(var house in houses)
                {
                    FindClosestSubwayForSingleHouse(house);
                    _upnHouseRepo.Update(house);
                }
                _upnHouseRepo.Save();
            }
        }

        public void FindClosestSubwayForSingleHouse(HouseInfoCore house)
        {
            if (house.Latitude == null || house.Longitude == null)
                return;

            List<double> distances = new List<double>(_subwayStations.Count);
            double minDistance = double.MaxValue;
            int? closestSubwayId = null;

            foreach(var station in _subwayStations)
            {
                double distance = GeoUtils.GetDistanceBetweenTwoGeoPoints(
                    house.Latitude.Value, house.Longitude.Value, station.Latitude.Value, station.Longitude.Value);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestSubwayId = station.Id;
                }
            }
            house.ClosestSubwayStationId = closestSubwayId;
            house.ClosestSubwayStationRange = minDistance;
        }

        /// <summary>
        /// Проходится по всем домам УПН и находит для них похожие по местоположению и году постройки дома N1
        /// </summary>
        public void FindSimilarN1ForAllUpnHouses()
        {
            List<int?> alreadyFilledUpnHouses = _similarHouseRepo.GetAllWithoutTracking()
                .Where(x => x.UpnHouseId != null && x.N1HouseId != null)
                .Select(x => x.UpnHouseId)
                .ToList();

            var upnHouses = _upnHouseRepo.GetAllWithoutTracking()
                .Where(x => !alreadyFilledUpnHouses.Contains(x.Id))
                .ToList();

            foreach (UpnHouseInfo upnHouse in upnHouses)
            {
                if (!upnHouse.Latitude.HasValue || !upnHouse.Longitude.HasValue)
                    continue;

                N1HouseInfo foundN1House =
                    getSimilarN1House(upnHouse.Latitude.Value, upnHouse.Longitude.Value, upnHouse.BuildYear, upnHouse.MaxFloor);
                if (foundN1House == null)
                    continue;

                double quadRange = (upnHouse.Latitude.Value - foundN1House.Latitude.Value) *
                                   (upnHouse.Latitude.Value - foundN1House.Latitude.Value)
                                   + (upnHouse.Longitude.Value - foundN1House.Longitude.Value) *
                                   (upnHouse.Longitude.Value - foundN1House.Longitude.Value);

                double range = Math.Sqrt(quadRange);

                if (range >= 0.015)
                    continue;

                addSimilarHouseRelationToDb(upnHouse.Id.Value, foundN1House.Id.Value, range);
            }

            _similarHouseRepo.Save();
        }

        protected void addSimilarHouseRelationToDb(int upnHouseId, int n1HouseId, double range)
        {
            SimilarHouse similarHouse = new SimilarHouse
            {
                UpnHouseId = upnHouseId,
                N1HouseId = n1HouseId,
                Distance = range
            };

            _similarHouseRepo.Add(similarHouse);
        }

        protected N1HouseInfo getSimilarN1House(double latitude, double longitude, int? buildYear, int? maxFloor)
        {
            N1HouseInfo foundHouse = _n1HouseRepo.GetAllWithoutTracking()
                .Where(x => x.Latitude != null && x.Longitude != null)
                .OrderBy(x => (x.Latitude - latitude) * (x.Latitude - latitude) + (x.Longitude - longitude) * (x.Longitude - longitude))
                .First();

            if (foundHouse.BuildYear.HasValue && buildYear.HasValue && foundHouse.BuildYear != buildYear)
                return null;

            if (foundHouse.MaxFloor.HasValue && maxFloor.HasValue && foundHouse.MaxFloor != maxFloor)
                return null;

            return foundHouse;
        }
    }
}
