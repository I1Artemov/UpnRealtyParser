using System.Net;

namespace UpnRealtyParser.Business.Models
{
    public class WebProxyInfo
    {
        public WebProxyInfo(WebProxy webProxy)
        {
            WebProxy = webProxy;
        }

        public WebProxy WebProxy { get; set; }

        /// <summary>
        /// Устанавливается в true, если с прокси не удалось соединиться (уже в ходе работы, а не на
        /// этапе загрузки прокси с гитхаба)
        /// </summary>
        public bool IsHasNotResponded { get; set; }
    }
}
