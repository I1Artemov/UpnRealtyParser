using System;

namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Для отображения на графике значений "Дата - число" (например, "Цена от времени")
    /// </summary>
    public class PointDateTimeWithValue
    {
        public DateTime XAxis{ get; set; }
        public double YAxis { get; set; }

        public PointDateTimeWithValue() { }

        public PointDateTimeWithValue(DateTime xValue, double yValue)
        {
            XAxis = xValue;
            YAxis = yValue;
        }
    }
}