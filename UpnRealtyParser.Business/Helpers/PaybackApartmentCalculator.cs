using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    /// <summary>
    /// Хэлпер для расчета предполагаемой окупаемости отдельных квартир на продажу (не средней по дому!)
    /// </summary>
    public class PaybackApartmentCalculator<TSellFlat>
        where TSellFlat : FlatCore
    {
        protected readonly EFGenericRepo<TSellFlat, RealtyParserContext> _sellFlatRepo;
        protected readonly EFGenericRepo<UpnRentFlat, RealtyParserContext> _upnRentFlatRepo;
        protected readonly EFGenericRepo<N1RentFlat, RealtyParserContext> _n1RentFlatRepo;
        protected readonly EFGenericRepo<SimilarHouse, RealtyParserContext> _similarHouseRepo;
        protected readonly EFGenericRepo<ApartmentPayback, RealtyParserContext> _apartmentPaybackRepo;
        protected readonly string _site;
        protected readonly Action<string> _writeToLogDelegate;

        public PaybackApartmentCalculator(string site, EFGenericRepo<TSellFlat, RealtyParserContext> sellFlatRepo,
            EFGenericRepo<UpnRentFlat, RealtyParserContext> upnRentFlatRepo,
            EFGenericRepo<N1RentFlat, RealtyParserContext> n1RentFlatRepo,
            EFGenericRepo<SimilarHouse, RealtyParserContext> similarHouseRepo,
            EFGenericRepo<ApartmentPayback, RealtyParserContext> apartmentPaybackRepo,
            Action<string> writeToLogDelegate)
        {
            _site = site;
            _sellFlatRepo = sellFlatRepo;
            _upnRentFlatRepo = upnRentFlatRepo;
            _n1RentFlatRepo = n1RentFlatRepo;
            _similarHouseRepo = similarHouseRepo;
            _apartmentPaybackRepo = apartmentPaybackRepo;
            _writeToLogDelegate = writeToLogDelegate;
        }

        /// <summary>
        /// Подсчет окупаемости для всех квартир УПН на продажу
        /// </summary>
        public async Task CalculateAllUpnPaybackPeriods()
        {
            await calculateAllAnyPaybackPeriods(Const.SiteNameUpn, Const.SiteNameN1, _upnRentFlatRepo, _n1RentFlatRepo);
        }

        /// <summary>
        /// Подсчет окупаемости для всех квартир N1 на продажу
        /// </summary>
        public async Task CalculateAllN1PaybackPeriods()
        {
            await calculateAllAnyPaybackPeriods(Const.SiteNameN1, Const.SiteNameUpn, _n1RentFlatRepo, _upnRentFlatRepo);
        }

        protected async Task calculateAllAnyPaybackPeriods<TRentFlatNative, TRentFlatOther>(
            string nativeSite, string otherSite, EFGenericRepo<TRentFlatNative, RealtyParserContext> nativeRentRepo,
            EFGenericRepo<TRentFlatOther, RealtyParserContext> otherRentRepo)
            where TRentFlatNative : FlatCore
            where TRentFlatOther : FlatCore
        {
            int flatsCount = await _sellFlatRepo.GetAllWithoutTracking().CountAsync();
            const int pageSize = 100;

            // Квартиры обрабатываем по 100 штук
            for (int page = 0; page < (flatsCount / pageSize) + 1; page++)
            {
                List<TSellFlat> sellFlats = await _sellFlatRepo.GetAllWithoutTracking()
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                List<ApartmentPayback> paybacksForAddition = new List<ApartmentPayback>(100);
                foreach (TSellFlat sellFlat in sellFlats)
                {
                    bool isAlreadyHaveStatistics = await _apartmentPaybackRepo.GetAllWithoutTracking()
                        .AnyAsync(x => x.FlatId == sellFlat.Id && x.Site == nativeSite);

                    if (isAlreadyHaveStatistics)
                        continue;

                    ApartmentPayback calculatedStats = await getSingleFlatPaybackPeriodAsync(sellFlat, nativeSite, otherSite,
                        nativeRentRepo, otherRentRepo);
                    if (calculatedStats != null)
                        paybacksForAddition.Add(calculatedStats);
                }

                await _apartmentPaybackRepo.AddRangeAsync(paybacksForAddition);
                await _apartmentPaybackRepo.SaveAsync();

                if (_writeToLogDelegate != null)
                    _writeToLogDelegate("Обработано " + (page + 1) * pageSize + " квартир");
            }
        }

        /// <summary>
        /// Считает и возвращает окупаемость по одной квартире УПН на продажу.
        /// Если нет данных по арендным квартирам УПН, ищет арендные квартиры на N1 по похожему дому
        /// </summary>
        /// <typeparam name="TRentFlatNative">Тип арендных квартир с "родного" сайта исходной квартиры</typeparam>
        /// <typeparam name="TRentFlatOther">Тип арендных квартир с другого сайта</typeparam>
        /// <param name="sellFlat">Исходная квартира на продажу</param>
        /// <param name="nativeSite">"Родной" сайт исходной квартиры на продажу</param>
        /// <param name="otherSite">Другой сайт</param>
        /// <param name="otherRentRepo">Репозиторий арендных квартир с "родного" сайта исходной квартиры</param>
        /// <param name="otherRentRepo">Репозиторий арендных квартир с другого сайта</param>
        protected async Task<ApartmentPayback> getSingleFlatPaybackPeriodAsync<TRentFlatNative, TRentFlatOther>(TSellFlat sellFlat, string nativeSite, string otherSite,
            EFGenericRepo<TRentFlatNative, RealtyParserContext> nativeRentRepo, EFGenericRepo<TRentFlatOther, RealtyParserContext> otherRentRepo)
            where TRentFlatNative : FlatCore
            where TRentFlatOther : FlatCore
        {
            if (!sellFlat.Price.HasValue) return null;

            double? averageRent = await nativeRentRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == sellFlat.HouseInfoId && x.RoomAmount == sellFlat.RoomAmount)
                    .AverageAsync(x => x.Price);

            if (averageRent.HasValue)
            {
                return new ApartmentPayback
                {
                    Site = nativeSite,
                    FlatId = sellFlat.Id.GetValueOrDefault(0),
                    PaybackPeriod = sellFlat.Price.Value / (12.0f * averageRent.Value),
                    CalculatedFromSite = nativeSite
                };
            }

            return await getApartmentPaybackFromSimilarHouseAsync(sellFlat, nativeSite, otherSite, otherRentRepo);
        }

        /// <summary>
        /// Пытаемся посчитать среднюю аренду по квартирам похожего дома с другого сайта
        /// </summary>
        private async Task<ApartmentPayback> getApartmentPaybackFromSimilarHouseAsync<TRentFlatOther>(TSellFlat sellFlat, string nativeSite, string otherSite,
            EFGenericRepo<TRentFlatOther, RealtyParserContext> otherRentRepo)
            where TRentFlatOther : FlatCore
        {
            int? similarHouseId = null;
            if (nativeSite == Const.SiteNameUpn)
            {
                SimilarHouse foundSimilarHouse = await _similarHouseRepo.GetAllWithoutTracking()
                    .FirstOrDefaultAsync(x => x.UpnHouseId == sellFlat.HouseInfoId);
                similarHouseId = foundSimilarHouse?.N1HouseId;
            }
            else
            {
                SimilarHouse foundSimilarHouse = await _similarHouseRepo.GetAllWithoutTracking()
                    .FirstOrDefaultAsync(x => x.N1HouseId == sellFlat.HouseInfoId);
                similarHouseId = foundSimilarHouse?.UpnHouseId;
            }

            if (!similarHouseId.HasValue) return null;

            double? averageRent = await otherRentRepo.GetAllWithoutTracking()
                    .Where(x => x.HouseInfoId == similarHouseId.Value && x.RoomAmount == sellFlat.RoomAmount)
                    .AverageAsync(x => x.Price);

            if (averageRent.HasValue)
            {
                return new ApartmentPayback
                {
                    Site = nativeSite,
                    FlatId = sellFlat.Id.GetValueOrDefault(0),
                    PaybackPeriod = sellFlat.Price.Value / (12.0f * averageRent.Value),
                    CalculatedFromSite = otherSite
                };
            }

            return null;
        }
    }
}
