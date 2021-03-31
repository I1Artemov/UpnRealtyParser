using System;

namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Для отображения на графике значений "Дата - число" (например, "Цена от времени")
    /// </summary>
    public class PointDateTimeWithValue
    {
        public DateTime XAxis{ get; set; }
        public double?[] YLayers { get; set; }

        public double? YFirstLayer => YLayers[0];
        public double? YSecondLayer => YLayers[1];
        public double? YThirdLayer => YLayers[2];
        public double? YFourthLayer => YLayers[3];

        public PointDateTimeWithValue() {
            YLayers = new double?[4];
        }
    }
}