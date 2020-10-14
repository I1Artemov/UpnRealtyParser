using System;
using System.IO;
using System.Text;

namespace UpnRealtyParser.Tests
{
    public class BaseWebParsingTest
    {
        protected string getTextFromFile(string dataPath, string fileName, string encodingName = "windows-1251")
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string contents = File.ReadAllText(string.Format("{0}\\..\\..\\..\\..\\{1}\\{2}",
                AppDomain.CurrentDomain.BaseDirectory, dataPath, fileName), Encoding.GetEncoding(encodingName));
            return contents;
        }
    }
}