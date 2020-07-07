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

        [NotMapped]
        public double SuccessRate
        {
            get
            {
                if (SuccessAmount.GetValueOrDefault(0) == 0 && FailureAmount.GetValueOrDefault(0) == 0)
                    return 0;
                return SuccessAmount.GetValueOrDefault(0) / (SuccessAmount.GetValueOrDefault(0) + FailureAmount.GetValueOrDefault(0));
            }
        }
            

        public void InitializeWebProxy()
        {
            this.WebProxy = new WebProxy(Ip, Int32.Parse(Port));
        }

        public void FillFieldsFromWebInfo()
        {
            if (this.WebProxy == null)
                return;

            Ip = this.WebProxy.Address.Host;
            Port = this.WebProxy.Address.Port.ToString();
        }
    }
}
