using System;

namespace UpnRealtyParser.Business.Helpers
{
    public static class Utils
    {
        /// <summary>
        /// Делает такую замену: original = abcdef...{leftSubstring}some_text{rightSubstring}...ghijk
        /// result = abcdef...{leftSubstring}newText{rightSubstring}...ghijk
        /// </summary>
        public static string ReplaceStrBetweenTwoStrings(string original, string leftSubstring, string rightSubstring, string newText)
        {
            if (string.IsNullOrEmpty(original))
                return null;

            int start = original.IndexOf(leftSubstring);
            int end = original.IndexOf(rightSubstring);
            string textBefore = original.Substring(0, start - 1);
            string textAfter = original.Substring(end);
            string replacedText = textBefore + newText + textAfter;

            return replacedText;
        }

        /// <summary>
        /// Преобразует строку в дату по формату. Если не удалось, вернет NULL
        /// </summary>
        public static DateTime? TryGetDateTimeFromString(string dateTimeStr, string format)
        {
            bool isParsed = DateTime.TryParseExact(dateTimeStr, format, System.Globalization.CultureInfo.InvariantCulture,
                 System.Globalization.DateTimeStyles.None, out DateTime parsedDate);

            if (isParsed)
                return parsedDate;

            return null;
        }

        /// <summary>
        /// Возвращает строку вида "До метро: "(станция метро)" ((расстояние) м./км.)
        /// </summary>
        public static string GetFormattedSubwaySummaryString(string stationName, double? distance)
        {
            if (string.IsNullOrEmpty(stationName) || !distance.HasValue)
                return "н/у";

            string subwayRangeStr = GetFormattedSubwayDistanceString(distance);
            return string.Format("{0} ({1})", stationName, subwayRangeStr);
        }

        /// <summary>
        /// расстояние переводит в строку вида "N м." или "N км."
        /// </summary>
        public static string GetFormattedSubwayDistanceString(double? distance)
        {
            if (!distance.HasValue)
                return "";

            string subwayRangeStr = distance.Value > 1000 ?
                (Math.Round(distance.Value / 100) / 10).ToString() + " км."
                : Math.Round(distance.Value).ToString() + " м.";

            return subwayRangeStr;
        }
    }
}
