using System.Collections.Generic;
using System.Net;
using UpnRealtyParser.Business.Helpers;
using Xunit;

namespace UpnRealtyParser.Tests
{
    public class OnlineProxyProviderTest : BaseWebParsingTest
    {
        [Fact]
        public void LoadProxiesFromGithubTest()
        {
            OnlineProxyProvider proxyProvider = new OnlineProxyProvider(null);
            List<WebProxy> webProxies = proxyProvider.GetAliveProxiesList();

            Assert.NotEmpty(webProxies);
        }
    }
}
