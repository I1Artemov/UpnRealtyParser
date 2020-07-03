using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace UpnRealtyParser.Business.Models
{
    public class WebProxyInfo : IdInfo
    {
        public DateTime? LastUseDateTime { get; set; }

        public DateTime? LastSuccessDateTime { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public int? SuccessAmount { get; set; }

        public int? FailureAmount { get; set; }

        public WebProxyInfo()
        {
            CreationDateTime = DateTime.Now;
        }

        public WebProxyInfo(WebProxy webProxy)
        {
            WebProxy = webProxy;
            CreationDateTime = DateTime.Now;
        }

        [NotMapped]
        public WebProxy WebProxy { get; set; }

        /// <summary>
        /// Устанавливается в true, если с прокси не удалось соединиться (уже в ходе работы, а не на
        /// этапе загрузки прокси с гитхаба)
        /// </summary>
        [NotMapped]
        public bool IsHasNotResponded { get; set; }

        public void FillFieldsFromWebInfo()
        {
            if (this.WebProxy == null)
                return;

            Ip = this.WebProxy.Address.Host;
            Port = this.WebProxy.Address.Port.ToString();
        }
    }
}
