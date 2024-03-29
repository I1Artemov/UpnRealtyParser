﻿using System;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class HouseStatisticsCalculator<TFlat, TRentFlat, THouse> 
        where TFlat : FlatCore
        where TRentFlat : FlatCore
        where THouse : HouseInfoCore
    {
        /// <summary>
        /// Дата самых ранних данных в приложении (начало существования)
        /// </summary>
        protected readonly DateTime AppExistStartDate = new DateTime(2020, 2, 1);

        protected EFGenericRepo<TFlat, RealtyParserContext> _flatRepo;
        protected EFGenericRepo<TRentFlat, RealtyParserContext> _rentFlatRepo;
        protected EFGenericRepo<THouse, RealtyParserContext> _houseRepo;
        protected EFGenericRepo<AveragePriceStat, RealtyParserContext> _statsRepo;
        protected EFGenericRepo<PaybackPeriodPoint, RealtyParserContext> _paybackPointsRepo;

        public HouseStatisticsCalculator(EFGenericRepo<TFlat, RealtyParserContext> flatRepo,
            EFGenericRepo<TRentFlat, RealtyParserContext> rentFlatRepo,
            EFGenericRepo<THouse, RealtyParserContext> houseRepo,
            EFGenericRepo<AveragePriceStat, RealtyParserContext> statsRepo,
            EFGenericRepo<PaybackPeriodPoint, RealtyParserContext> paybackPointsRepo = null)
        {
            _flatRepo = flatRepo;
            _rentFlatRepo = rentFlatRepo;
            _houseRepo = houseRepo;
            _statsRepo = statsRepo;
            _paybackPointsRepo = paybackPointsRepo;
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
            calculateAverageRentPrices(houseId, result);

            result.SetAverageMeterPrices();
            return result;
        }

        private void calculateAverageRentPrices(int houseId, HouseStatistics statistics)
        {
            List<TRentFlat> rentFlats = _rentFlatRepo.GetAllWithoutTracking()
                .Where(x => x.HouseInfoId == houseId).ToList();

            double? oneRoomRentPrice = rentFlats.Where(x => x.RoomAmount == null || x.RoomAmount == 1).Average(x => x.Price);
            double? twoRoomRentPrice = rentFlats.Where(x => x.RoomAmount == 2).Average(x => x.Price);
            double? threeRoomRentPrice = rentFlats.Where(x => x.RoomAmount == 3).Average(x => x.Price);

            statistics.AverageSingleRoomRentPrice = oneRoomRentPrice;
            statistics.AverageTwoRoomRentPrice = twoRoomRentPrice;
            statistics.AverageThreeRoomRentPrice = threeRoomRentPrice;
        }

        /// <summary>
        /// Возвращает точки для графика "Средняя цена от месяца" для указанного дома
        /// </summary>
        /// <param name="houseId">ID дома УПН</param>
        /// <param name="startDt">Начальная дата анализа цен</param>
        /// <param name="endDt">Конечная дата анализа цен</param>
        /// <param name="roomAmount">Для квартир с этим количеством комнат</param>
        public List<PointDateTimeWithValue> GetAveragePriceForMonthsPoints(int houseId, DateTime startDt, DateTime endDt,
            string site)
        {
            startDt = new DateTime(startDt.Year, startDt.Month, 1);
            List<PointDateTimeWithValue> allRoomPoints = new List<PointDateTimeWithValue>();

            for (int roomAmount = 1; roomAmount <= 4; roomAmount++)
            {
                List<AveragePriceStat> foundStats = _statsRepo.GetAllWithoutTracking()
                .Where(x => x.HouseId == houseId && x.Site == site && x.RoomAmount == roomAmount && x.Price != null &&
                       new DateTime(x.Year, x.Month, 1) <= endDt && new DateTime(x.Year, x.Month, 1) >= startDt)
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

                foreach(AveragePriceStat stat in foundStats)
                {
                    PointDateTimeWithValue existingPoint = allRoomPoints
                        .FirstOrDefault(x => x.XAxis.Year == stat.Year && x.XAxis.Month == stat.Month);

                    if (existingPoint == null)
                    {
                        PointDateTimeWithValue pointForAddition = 
                            new PointDateTimeWithValue { XAxis = new DateTime(stat.Year, stat.Month, 1)};
                        pointForAddition.YLayers[roomAmount - 1] = stat.Price;
                        allRoomPoints.Add(pointForAddition);
                    }
                    else
                    {
                        existingPoint.YLayers[roomAmount - 1] = stat.Price;
                    }
                }
            }

            return allRoomPoints.OrderBy(x => x.XAxis).ToList();
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
        public void CalculateAllUpnHouseAvgPricesAndSaveToDb(string site, Action<string> writeToLogDelegate)
        {
            List<int> houseIds = _houseRepo.GetAllWithoutTracking()
                .Where(x => x.Id != null)
                .Select(x => x.Id.Value).ToList();
            int housesCount = houseIds.Count;
            int processedCounter = 0;

            foreach (int houseId in houseIds)
            {
                DateTime currentStartDt = AppExistStartDate;
                DateTime endDt = DateTime.Now.AddMonths(-1); // чтобы не считалась статистика за еще не завершенный текущий месяц

                while (currentStartDt < endDt)
                {
                    for(int roomAmount = 1; roomAmount <= 4; roomAmount++)
                        calculateStatsAndAddToDbIfNeeded(site, houseId, roomAmount, currentStartDt);

                    currentStartDt = currentStartDt.AddMonths(1);
                }

                processedCounter++;
                _statsRepo.Save();

                if (processedCounter % 10 == 0 && writeToLogDelegate != null)
                    writeToLogDelegate(string.Format("Обработано {0} домов из {1}", processedCounter, housesCount));
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

        /// <summary>
        /// Вычисляет и возвращает точки для карты окупаемости по всем домам, собранным с УПН
        /// </summary>
        public void CalculateAllPaybackPeriodPoints(string siteName)
        {
            List<PaybackPeriodPoint> paybackPoints = new List<PaybackPeriodPoint>();
            List<int?> houseIds = _houseRepo.GetAllWithoutTracking().Select(x => x.Id).ToList();

            foreach (int? houseId in houseIds)
            {
                var avgSingleSellPrice = _flatRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == houseId && x.RoomAmount == 1).Average(x => x.Price);
                var avgTwoSellPrice = _flatRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == houseId && x.RoomAmount == 2).Average(x => x.Price);
                var avgSingleRentPrice = _rentFlatRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == houseId && x.RoomAmount == 1).Average(x => x.Price);
                var avgTwoRentPrice = _rentFlatRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == houseId && x.RoomAmount == 2).Average(x => x.Price);

                bool hasSingleInfo = avgSingleSellPrice != null && avgSingleRentPrice != null;
                bool hasTwoInfo = avgTwoSellPrice != null && avgTwoRentPrice != null;
                if (!hasSingleInfo && !hasTwoInfo) continue;

                var singlePaybackYears = avgSingleRentPrice.GetValueOrDefault(0) == 0 ? 
                    0 : avgSingleSellPrice / (avgSingleRentPrice * 12);
                var twoPaybackYears = avgTwoRentPrice.GetValueOrDefault(0) == 0 ?
                    0 : avgTwoSellPrice / (avgTwoRentPrice * 12);

                double? totalPayback = null;
                if (hasSingleInfo && hasTwoInfo) totalPayback = Math.Min(singlePaybackYears.Value, twoPaybackYears.Value);
                if (hasSingleInfo && !hasTwoInfo) totalPayback = singlePaybackYears;
                if (!hasSingleInfo && hasTwoInfo) totalPayback = twoPaybackYears;

                THouse houseInfo = _houseRepo.GetAllWithoutTracking().FirstOrDefault(x => x.Id == houseId);
                PaybackPeriodPoint paybackPoint = new PaybackPeriodPoint
                {
                    HouseId = houseId.GetValueOrDefault(0),
                    Latitude = houseInfo?.Latitude,
                    Longitude = houseInfo?.Longitude,
                    PaybackYears = totalPayback,
                    HouseAddress = houseInfo.Address,
                    SiteName = siteName
                };
                paybackPoints.Add(paybackPoint);
            }

            saveAllPaybackMapPointsToDb(paybackPoints);
        }

        private void saveAllPaybackMapPointsToDb(List<PaybackPeriodPoint> points)
        {
            foreach(var point in points)
            {
                PaybackPeriodPoint foundPoint = _paybackPointsRepo.GetAll()
                    .FirstOrDefault(x => x.HouseId == point.HouseId && x.SiteName == point.SiteName);

                if(foundPoint != null)
                {
                    foundPoint.PaybackYears = point.PaybackYears;
                    foundPoint.CreationDateTime = DateTime.Now;
                    _paybackPointsRepo.Update(foundPoint);
                } else
                {
                    _paybackPointsRepo.Add(point);
                }
            }
            _paybackPointsRepo.Save();
        }
    }
}
