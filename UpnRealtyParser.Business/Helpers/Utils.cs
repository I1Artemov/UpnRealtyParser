using System;
using System.Text;
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
    }
}
