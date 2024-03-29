﻿namespace UpnRealtyParser.Business.Models
{
    public class HouseStatistics
    {
        // Средние цены продажи квартир
        public double? AverageSingleRoomSellPrice { get; set; }
        public double? AverageTwoRoomSellPrice { get; set; }
        public double? AverageThreeRoomSellPrice { get; set; }
        public double? AverageFourRoomSellPrice { get; set; }

        // Средние цены аренды квартир
        public double? AverageSingleRoomRentPrice { get; set; }
        public double? AverageTwoRoomRentPrice { get; set; }
        public double? AverageThreeRoomRentPrice { get; set; }

        // Средний метраж квартир
        public double? AverageSingleRoomSpace { get; set; }
        public double? AverageTwoRoomSpace { get; set; }
        public double? AverageThreeRoomSpace { get; set; }
        public double? AverageFourRoomSpace { get; set; }

        // Средние цены продажи за метр
        public double? AverageSingleRoomMeterPrice { get; set; }
        public double? AverageTwoRoomMeterPrice { get; set; }
        public double? AverageThreeRoomMeterPrice { get; set; }
        public double? AverageFourRoomMeterPrice { get; set; }

        /// <summary>
        /// Рассчитывает средние цены за квадратный метр. Вызывать после вычисления
        /// средних цен и средней площади
        /// </summary>
        public void SetAverageMeterPrices()
        {
            AverageSingleRoomMeterPrice =
                AverageSingleRoomSpace == null || AverageSingleRoomSpace == 0 ? null :
                AverageSingleRoomSellPrice / AverageSingleRoomSpace;

            AverageTwoRoomMeterPrice =
            AverageTwoRoomSpace == null || AverageTwoRoomSpace == 0 ? null :
            AverageTwoRoomSellPrice / AverageTwoRoomSpace;

            AverageThreeRoomMeterPrice =
            AverageThreeRoomSpace == null || AverageThreeRoomSpace == 0 ? null :
            AverageThreeRoomSellPrice / AverageThreeRoomSpace;

            AverageFourRoomMeterPrice =
            AverageFourRoomSpace == null || AverageFourRoomSpace == 0 ? null :
            AverageFourRoomSellPrice / AverageFourRoomSpace;
        }
    }
}
