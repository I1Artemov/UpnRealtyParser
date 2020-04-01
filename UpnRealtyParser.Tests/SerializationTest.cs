using System.Collections.Generic;
using UpnRealtyParser.Business;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class SerializationTest
    {
        [Fact]
        public void SerializeAppSettings_Test()
        {
            AppSettingsSerializer serializer = new AppSettingsSerializer();
            AppSettings settings = new AppSettings
            {
                IsUseProxies = true,
                RequestDelayInMs = 400000,
                ProxyList = new List<string>
                {
                    "139.59.62.255:8080",
                    "192.241.245.207:3128",
                    "212.220.216.70:8080"
                }
            };
            //serializer.SaveSettingsToFile(settings, Const.AppSettingsFileName);

            AppSettings loadedSettings = serializer.GetAppSettingsFromFile(Const.AppSettingsFileName);
            Assert.NotNull(loadedSettings);
        }
    }
}
