using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace SeleniumPhotoService
{
    public class SeleniumPhotoHelper
    {
        protected Action<string> _writeToLogDelegate;
        protected const int DelayBetweenHouses = 50000;

        public SeleniumPhotoHelper(Action<string> writeToLogDelegate)
        {
            _writeToLogDelegate = writeToLogDelegate;
        }

        /// <summary>
        /// Берет необработанные (с точки зрения фото) дома УПН, переходит на страницу с самой новой квартирой
        /// каждого дома и делает скриншот картинки из ссылки "Показать фото дома"
        /// </summary>
        public async Task ProcessHousesBySelenium()
        {
            IWebDriver webDriver = getWebDriver();

            List<UpnFlat> houseFlats = await getLatestFlatsForHouses();
            if(_writeToLogDelegate != null)
                _writeToLogDelegate("Найдено " + houseFlats.Count + " домов для обработки.");
            int processedCount = 0;

            foreach (UpnFlat houseFlat in houseFlats)
            {
                if (houseFlat.SiteUrl == null)
                    continue;

                webDriver.Url = "https://upn.ru" + houseFlat.SiteUrl;
                var showPhotoAnchor = tryGetElementByCss(webDriver, "#address a");
                if (showPhotoAnchor == null)
                {
                    processedCount++;
                    await Task.Delay(DelayBetweenHouses);
                    continue;
                }
                showPhotoAnchor.Click();
                await Task.Delay(10000);

                string diskSaveError = saveElementScreenshotToDisk(webDriver, houseFlat.UpnHouseInfoId.Value);
                if(string.IsNullOrEmpty(diskSaveError))
                    await saveHousePhotoToDb(houseFlat.UpnHouseInfoId.Value, houseFlat.UpnHouseInfoId.Value + "_pic.png");

                processedCount++;
                if (processedCount % 5 == 0 && _writeToLogDelegate != null)
                    _writeToLogDelegate("Обработано " + processedCount + " домов из " + houseFlats.Count);
                await Task.Delay(DelayBetweenHouses);
            }

            disposeWebDriver(webDriver);
        }

        protected string saveElementScreenshotToDisk(IWebDriver webDriver, int houseId)
        {
            try { 
                var photoElement = tryGetElementByCss(webDriver, ".mapBox img");
                Bitmap screenshot = makeElemScreenshot(webDriver, photoElement);
                string fileName = Const.HousePhotoFolder + houseId + "_pic.png";
                if (screenshot != null)
                    screenshot.Save(fileName, ImageFormat.Png);
            }
            catch(Exception ex)
            {
                _writeToLogDelegate("Ошибка сохранения скриншота: " + ex.Message);
                return ex.Message;
            }
            return null;
        }

        protected async Task<List<UpnFlat>> getLatestFlatsForHouses()
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<HousePhoto, RealtyParserContext> housePhotoRepo = new EFGenericRepo<HousePhoto, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnHouseInfo, RealtyParserContext> houseRepo = new EFGenericRepo<UpnHouseInfo, RealtyParserContext>(realtyContext);
                EFGenericRepo<UpnFlat, RealtyParserContext> flatRepo = new EFGenericRepo<UpnFlat, RealtyParserContext>(realtyContext);
                EFGenericRepo<PageLink, RealtyParserContext> linkRepo = new EFGenericRepo<PageLink, RealtyParserContext>(realtyContext);

                List<int> processedHouseIds = await housePhotoRepo.GetAllWithoutTracking()
                    .Select(x => x.UpnHouseId).ToListAsync();

                List<int> requiredHouseIds = await houseRepo.GetAllWithoutTracking()
                    .Select(x => x.Id.GetValueOrDefault(0)).OrderByDescending(x => x).ToListAsync();

                // Отсекаем уже обработанные дома
                requiredHouseIds = requiredHouseIds
                    .Where(x => !processedHouseIds.Contains(x)).ToList();

                // Берем самые "свежие" квартиры, относящиеся к домам
                List<UpnFlat> latestFlats = new List<UpnFlat>(requiredHouseIds.Count);
                foreach(int houseId in requiredHouseIds)
                {
                    UpnFlat latestFlat = await flatRepo.GetAllWithoutTracking()
                        .Where(x => x.RemovalDate == null && x.UpnHouseInfoId == houseId)
                        .OrderByDescending(x => x.LastCheckDate)
                        .FirstOrDefaultAsync();

                    if (latestFlat == null)
                        continue;

                    // Дозаполняем URL'ами из PageLink'ов
                    PageLink foundLink = await linkRepo.GetAllWithoutTracking()
                        .FirstOrDefaultAsync(x => x.Id == latestFlat.PageLinkId);

                    if (foundLink == null)
                        continue;

                    latestFlat.SiteUrl = foundLink.Href;
                    latestFlats.Add(latestFlat);
                }

                return latestFlats;
            }
        }

        protected async Task saveHousePhotoToDb(int houseId, string fileName)
        {
            using (var realtyContext = new RealtyParserContext())
            {
                EFGenericRepo<HousePhoto, RealtyParserContext> housePhotoRepo =
                    new EFGenericRepo<HousePhoto, RealtyParserContext>(realtyContext);

                HousePhoto photo = new HousePhoto { 
                    UpnHouseId = houseId,
                    FileName = fileName,
                };

                housePhotoRepo.Add(photo);
                await housePhotoRepo.SaveAsync();
            }
        }

        private IWebDriver getWebDriver()
        {

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            // Скрывает сообщение "Браузер управляется автоматизированным ПО"
            options.AddExcludedArgument("enable-automation");

            IWebDriver webDriver = new ChromeDriver(Const.ChromeDriverPath, options, TimeSpan.FromMinutes(3));
            webDriver.Manage().Window.Maximize();
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(3);

            return webDriver;
        }

        private void disposeWebDriver(IWebDriver webDriver)
        {
            webDriver.Quit();
        }

        protected IWebElement tryGetElementByCss(IWebDriver webDriver, string querySelector)
        {
            try
            {
                return webDriver.FindElement(By.CssSelector(querySelector));
            }
            catch (Exception ex)
            {
                _writeToLogDelegate("Ошибка поиска элемента: " + ex.Message);
                return null;
            }
        }

        protected Bitmap makeElemScreenshot(IWebDriver webDriver, IWebElement elem, int? yOffset = null)
        {
            try
            {
                Screenshot myScreenShot = ((ITakesScreenshot)webDriver).GetScreenshot();

                using (var screenBmp = new Bitmap(new MemoryStream(myScreenShot.AsByteArray)))
                {
                    Point offsetLocation = elem.Location;
                    if (yOffset.HasValue)
                        offsetLocation = new Point(elem.Location.X, elem.Location.Y + yOffset.Value);

                    Rectangle cropArea = new Rectangle(offsetLocation, elem.Size);
                    return screenBmp.Clone(cropArea, screenBmp.PixelFormat);
                }
            }
            catch (Exception ex)
            {
                _writeToLogDelegate("Ошибка снятия скриншота: " + ex.Message);
                return null;
            }
        }
    }
}
