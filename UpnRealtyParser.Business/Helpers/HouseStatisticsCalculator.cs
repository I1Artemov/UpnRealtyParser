using System;
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

        /// <summary>
        /// Подсчитывает средние цены (в т.ч. - за квадратный метр) по всем 1, 2, 3 и 4-комнатным квартирам в доме
        /// </summary>
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

            HouseStatistics result = new HouseStatistics
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

            result.SetAverageMeterPrices();
            return result;
        }

        /// <summary>
        /// Возвращает точки для графика "Средняя цена от месяца" для указанного дома
        /// </summary>
        /// <param name="houseId">ID дома УПН</param>
        /// <param name="startDt">Начальная дата анализа цен</param>
        /// <param name="endDt">Конечная дата анализа цен</param>
        /// <param name="roomAmount">Считать для квартир с этим количеством комнат</param>
        public List<PointDateTimeWithValue> GetAveragePriceForMonthsPoints(int houseId,DateTime startDt, DateTime endDt,
            int roomAmount)
        {
            startDt = new DateTime(startDt.Year, startDt.Month, 1);
            DateTime currentStartDt = startDt;
            List<PointDateTimeWithValue> points = new List<PointDateTimeWithValue>();

            while (currentStartDt < endDt)
            {
                DateTime currentEndDt = currentStartDt.AddMonths(1);

                // Ищем не те квартиры, которые были созданы за анализируемый период (CurrentStart <= CreationDate <= CurrentEnd),
                // а те, которые были еще актуальны в текущем периоде (созданы не позже CurrentEnd и удалены не раньше CurrentStart)
                double? averagePrice = _flatRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == houseId &&
                                x.CreationDateTime <= currentEndDt && x.LastCheckDate >= currentStartDt)
                    .Average(x => x.Price);

                if (averagePrice.HasValue)
                    points.Add(new PointDateTimeWithValue(currentStartDt, averagePrice.Value));

                currentStartDt = currentStartDt.AddMonths(1);
            }

            return points;
        }
    }
}
