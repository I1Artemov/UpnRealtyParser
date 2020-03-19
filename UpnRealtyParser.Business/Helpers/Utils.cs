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
    }
}
