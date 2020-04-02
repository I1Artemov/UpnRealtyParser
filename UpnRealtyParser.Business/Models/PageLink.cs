using System;

namespace UpnRealtyParser.Business.Models
{
    public class PageLink
    {
        public PageLink()
        {
            CreationDateTime = DateTime.Now;
            LastCheckDateTime = DateTime.Now;
        }

        public int Id { get; set; }

        public DateTime? CreationDateTime { get; set; }

        public DateTime? LastCheckDateTime { get; set; }

        public string Href { get; set; }

        public string LinkType { get; set; }

        public string SiteName { get; set; }

        public bool? IsDead { get; set; }
    }
}
