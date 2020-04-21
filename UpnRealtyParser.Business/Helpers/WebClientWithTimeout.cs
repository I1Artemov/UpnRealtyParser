using System;
using System.Net;

namespace UpnRealtyParser.Business.Helpers
{
    public class WebClientWithTimeout : WebClient
    {
        public int? RequestTimeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest wr = base.GetWebRequest(address);
            wr.Timeout = RequestTimeout.GetValueOrDefault(10000); // timeout in milliseconds (ms)
            return wr;
        }
    }
}
