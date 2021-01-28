using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class HouseStatisticsCalculator<T> where T :  FlatCore
    {
        protected EFGenericRepo<T, RealtyParserContext> _flatRepo;

        public HouseStatisticsCalculator(EFGenericRepo<T, RealtyParserContext> flatRepo)
        {
            _flatRepo = flatRepo;
        }

        public HouseStatistics GetStatisticsForHouse(int houseId)
        {
            List<T> houseFlats = _flatRepo.GetAllWithoutTracking()
                .Where(x => x.HouseInfoId == houseId).ToList();

            double? oneRoomPrice = houseFlats.Where(x => x.RoomAmount == null || x.RoomAmount == 1).Average(x => x.Price);
            double? twoRoomPrice = houseFlats.Where(x => x.RoomAmount == 2).Average(x => x.Price);
            double? threeRoomPrice = houseFlats.Where(x => x.RoomAmount == 3).Average(x => x.Price);
            double? fourRoomPrice = houseFlats.Where(x => x.RoomAmount > 3).Average(x => x.Price);

            double? oneRoomSpace = houseFlats.Where(x => x.RoomAmount == null || x.RoomAmount == 1).Average(x => x.SpaceSum);
            double? twoRoomSpace = houseFlats.Where(x => x.RoomAmount == 2).Average(x => x.SpaceSum);
            double? threeRoomSpace = houseFlats.Where(x => x.RoomAmount == 3).Average(x => x.SpaceSum);
            double? fourRoomSpace = houseFlats.Where(x => x.RoomAmount > 3).Average(x => x.SpaceSum);

            return new HouseStatistics
            {
                AverageSingleRoomSellPrice = oneRoomPrice,
                AverageTwoRoomSellPrice = twoRoomPrice,
                AverageThreeRoomSellPrice = threeRoomPrice,
                AverageFourRoomSellPrice = fourRoomPrice,

                AverageSingleRoomSpace = oneRoomSpace,
                AverageTwoRoomSpace = twoRoomSpace,
                AverageThreeRoomSpace = threeRoomSpace,
                AverageFourRoomSpace = fourRoomSpace
            };
        }
    }
}
