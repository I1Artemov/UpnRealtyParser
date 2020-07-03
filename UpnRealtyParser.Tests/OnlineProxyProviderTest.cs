using System.Collections.Generic;
using System.Net;
using UpnRealtyParser.Business.Helpers;
using UpnRealtyParser.Business.Models;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class OnlineProxyProviderTest : BaseWebParsingTest
    {
        [Fact]
        public void LoadProxiesFromGithubTest()
        {
            OnlineProxyProvider proxyProvider = new OnlineProxyProvider(null, null);
            List<WebProxyInfo> webProxies = proxyProvider.GetAliveProxiesList();

            Assert.NotEmpty(webProxies);
        }
    }
}
