using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Business.Helpers
{
    public class N1SiteAgent : AnyRealtySiteParser
    {
        public N1SiteAgent(Action<string> writeToLogDelegate, AppSettings settings) :
            base(writeToLogDelegate, settings)
        { }
    }
}
