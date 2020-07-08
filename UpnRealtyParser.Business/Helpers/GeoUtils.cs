using System;

namespace UpnRealtyParser.Business.Helpers
{
    public static class GeoUtils
    {
        /// <summary>
        /// Получает расстояние в метрах между двумя точками с заданной широтой и долготой
        /// </summary>
        public static double GetDistanceBetweenTwoGeoPoints (double firstLat, double firstLon, double secondLat, double secondLon)
        {
            double theta = firstLon - secondLon;
            double dist = Math.Sin(deg2rad(firstLat)) * Math.Sin(deg2rad(secondLat)) +
                Math.Cos(deg2rad(firstLat)) * Math.Cos(deg2rad(secondLat)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60.0d * 1.1515d * 1609.344d; // Сначала переводим в мили, а затем из миль - в метры
            return dist;
        }

        private static double deg2rad(double deg)
        {
            return deg * Math.PI / 180.0d;
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI) * 180.0d;
        }
    }
}
