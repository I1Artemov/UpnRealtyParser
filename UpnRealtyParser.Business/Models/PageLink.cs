using System;

namespace UpnRealtyParser.Business.Models
{
    public class PageLink
    {
        public long Id { get; set; }

        public DateTime? CreationDateTime { get; set; }

        public DateTime? LastCheckDateTime { get; set; }

        public string Href { get; set; }

        public string LinkType { get; set; }

        public string SiteName { get; set; }
    }
}
