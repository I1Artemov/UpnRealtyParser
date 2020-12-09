using System;
using System.Net.Http;
using System.Text;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Helpers
{
    public class BaseSiteAgent
    {
        protected WebProxyInfo _currentProxy;
        protected bool _isUseProxy;
        protected Random _random;
        protected RealtyParserContext _dbContext;
        protected Action<string> _writeToLogDelegate;
        protected bool _isConnectionOpen;

        public BaseSiteAgent(bool isUseProxy, Action<string> writeToLogDelegate)
        {
            _isUseProxy = isUseProxy;
            _random = new Random();
            _writeToLogDelegate = writeToLogDelegate;
        }

        public void OpenConnection()
        {
            if (_isConnectionOpen)
                return;

            _dbContext = new RealtyParserContext();
            initializeRepositories(_dbContext);

            _isConnectionOpen = true;
        }

        public void CloseConnection()
        {
            if (!_isConnectionOpen)
                return;

            _dbContext.Dispose();

            _isConnectionOpen = false;
        }

        protected virtual void initializeRepositories(RealtyParserContext context)
        { }

        protected HttpClient createHttpClient()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            HttpClientHandler clientHandler = new HttpClientHandler();
            if (_isUseProxy)
            {
                _currentProxy = getRandomWebProxy();
                clientHandler.Proxy = _currentProxy.WebProxy;
                //clientHandler.Proxy = new System.Net.WebProxy("127.0.0.1:8888");
                //clientHandler.Proxy = new System.Net.WebProxy("217.172.122.4:8080");
                clientHandler.UseProxy = true;
            }
            else
            {
                clientHandler.UseProxy = false;
            }

            HttpClient httpClient = new HttpClient(clientHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(400); // TODO: В параметры
            return httpClient;
        }


        /// <summary>
        /// Возвращает случайно выбранную прокси. Игнорирует прокси, у которых ранее был установлен признак IsNotResponding
        /// </summary>
        protected WebProxyInfo getRandomWebProxy()
        {
            OnlineProxyProvider proxyProvider = new OnlineProxyProvider(_dbContext, _writeToLogDelegate);
            return proxyProvider.GetRandomWebProxy(_random);
        }
    }
}
