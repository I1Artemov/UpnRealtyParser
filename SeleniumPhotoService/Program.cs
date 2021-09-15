using System;
using System.Threading.Tasks;

namespace SeleniumPhotoService
{
    class Program
    {
        public static void WriteDebugLog(string text)
        {
            string dateStr = DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
            Console.WriteLine(string.Format("[{0}] {1}", dateStr, text));
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Started house photo gathering");
            SeleniumPhotoHelper helper = new SeleniumPhotoHelper(WriteDebugLog);
            await helper.ProcessHousesBySelenium();
        }
    }
}
