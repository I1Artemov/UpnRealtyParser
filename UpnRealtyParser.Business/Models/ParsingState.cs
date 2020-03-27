using System;

namespace UpnRealtyParser.Business.Models
{
    public class ParsingState
    {
        public long Id { get; set; }

        public DateTime? CreationDateTime { get; set; }

        public string SiteName { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        public string Status { get; set; }
    }
}
