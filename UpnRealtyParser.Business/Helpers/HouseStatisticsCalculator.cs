using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class HouseStatisticsCalculator<TFlat, THouse> 
        where TFlat : FlatCore
        where THouse : HouseInfoCore
    {
        /// <summary>
        /// Дата самых ранних данных в приложении (начало существования)
        /// </summary>
        protected readonly DateTime AppExistStartDate = new DateTime(2020, 2, 1);

        protected EFGenericRepo<TFlat, RealtyParserContext> _flatRepo;
        protected EFGenericRepo<THouse, RealtyParserContext> _houseRepo;
        protected EFGenericRepo<AveragePriceStat, RealtyParserContext> _statsRepo;

        public HouseStatisticsCalculator(EFGenericRepo<TFlat, RealtyParserContext> flatRepo,
            EFGenericRepo<THouse, RealtyParserContext> houseRepo,
            EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo)
        {
            _flatRepo = flatRepo;
            _houseRepo = houseRepo;
            _statsRepo = statsRepo;
        }

        /// <summary>
        /// Подсчитывает средние цены (в т.ч. - за квадратный метр) по всем 1, 2, 3 и 4-комнатным квартирам в доме
        /// </summary>
        public HouseStatistics GetStatisticsForHouse(int houseId)
        {
            List<TFlat> houseFlats = _flatRepo.GetAllWithoutTracking()
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
        /// <param name="roomAmount">Для квартир с этим количеством комнат</param>
        public List<PointDateTimeWithValue> GetAveragePriceForMonthsPoints(int houseId, DateTime startDt, DateTime endDt,
            int roomAmount, string site)
        {
            startDt = new DateTime(startDt.Year, startDt.Month, 1);

            List<PointDateTimeWithValue> points = _statsRepo.GetAllWithoutTracking()
                .Where(x => x.HouseId == houseId && x.Site == site&& x.RoomAmount == roomAmount && x.Price != null &&
                       new DateTime(x.Year, x.Month, 1) <= endDt && new DateTime(x.Year, x.Month, 1) >= startDt)
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new PointDateTimeWithValue(new DateTime(x.Year, x.Month, 1), (double)x.Price.Value))
                .ToList();

            return points;
        }

        private double? calculateAveragePriceForDate(int houseId, DateTime currentStartDt, int roomAmount)
        {
            DateTime currentEndDt = currentStartDt.AddMonths(1);

            // Ищем не те квартиры, которые были созданы за анализируемый период (CurrentStart <= CreationDate <= CurrentEnd),
            // а те, которые были еще актуальны в текущем периоде (созданы не позже CurrentEnd и удалены не раньше CurrentStart)
            double? averagePrice = _flatRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == houseId && x.RoomAmount == roomAmount &&
                                x.CreationDateTime <= currentEndDt && x.LastCheckDate >= currentStartDt)
                    .Average(x => x.Price);

            return averagePrice;
        }

        /// <summary>
        /// Берет все собранные дома УПН и для каждого рассчитывает средние цены квартир
        /// за месяцы, за которые статистика еще не была посчитана
        /// </summary>
        public void CalculateAllUpnHouseAvgPricesAndSaveToDb(string site)
        {
            List<int> houseIds = _houseRepo.GetAllWithoutTracking()
                .Where(x => x.Id != null)
                .Select(x => x.Id.Value).ToList();

            foreach (int houseId in houseIds)
            {
                DateTime currentStartDt = AppExistStartDate;
                DateTime endDt = DateTime.Now;

                while (currentStartDt < endDt)
                {
                    for(int roomAmount = 1; roomAmount <= 4; roomAmount++)
                        calculateStatsAndAddToDbIfNeeded(site, houseId, roomAmount, currentStartDt);

                    currentStartDt = currentStartDt.AddMonths(1);
                }

                _statsRepo.Save();
            }
        }

        private void calculateStatsAndAddToDbIfNeeded(string site, int houseId, int roomAmount,
            DateTime currentStartDt)
        {
            bool isAlreadyHaveStats = _statsRepo.GetAllWithoutTracking().Any(
                        x => x.Site == site && x.HouseId == houseId && x.RoomAmount == roomAmount &&
                        x.Year == currentStartDt.Year && x.Month == currentStartDt.Month);

            if (isAlreadyHaveStats)
                return;

            double? averagePrice = calculateAveragePriceForDate(houseId, currentStartDt, roomAmount);

            if (!averagePrice.HasValue)
                return;

            AveragePriceStat stats = new AveragePriceStat
            {
                Site = site,
                HouseId = houseId,
                RoomAmount = roomAmount,
                Year = currentStartDt.Year,
                Month = currentStartDt.Month,
                Price = averagePrice
            };

            _statsRepo.Add(stats);
        }
    }
}
