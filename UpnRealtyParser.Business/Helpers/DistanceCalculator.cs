using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class DistanceCalculator
    {
        protected EFGenericRepo<UpnHouseInfo, RealtyParserContext> _houseRepo;
        protected EFGenericRepo<SubwayStation, RealtyParserContext> _subwayRepo;
        List<SubwayStation> _subwayStations;

        public DistanceCalculator(RealtyParserContext dbContext)
        {
            _houseRepo = new EFGenericRepo<UpnHouseInfo, RealtyParserContext>(dbContext);
            _subwayRepo = new EFGenericRepo<SubwayStation, RealtyParserContext>(dbContext);
            _subwayStations = _subwayRepo.GetAllWithoutTracking()
                .ToList();
        }

        /// <summary>
        /// Берет все дома из БД, для каждого находит ближайшую станцию метро и расстояние до нее (если еще не заполнено).
        /// Вычисления сохраняет в БД
        /// </summary>
        public void CalculateDistanceFromHousesToClosestSubway()
        {
            int housesPerGathering = 1000;
            int skipAmount = 0;
            int totalHouses = _houseRepo.GetAllWithoutTracking()
                .Where(x => x.ClosestSubwayStationId == null)
                .Count();

            for (skipAmount = 0; skipAmount < totalHouses; skipAmount += housesPerGathering)
            {
                List<UpnHouseInfo> houses = _houseRepo.GetAll()
                    .Where(x => x.ClosestSubwayStationId == null)
                    .Skip(skipAmount)
                    .Take(housesPerGathering)
                    .ToList();

                foreach(var house in houses)
                {
                    FindClosestSubwayForSingleHouse(house);
                    _houseRepo.Update(house);
                }
                _houseRepo.Save();
            }
        }

        public void FindClosestSubwayForSingleHouse(UpnHouseInfo house)
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
    }
}
