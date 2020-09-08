using System;

namespace UpnRealtyParser.Business.Models
{
    public class IdInfo {
        public int? Id {get; set;}

        public DateTime? CreationDateTime {get; set;}

        public string CreationDatePrintable =>
            CreationDateTime == null ? "" : CreationDateTime.Value.ToString("dd.MM.yyyy");
    }
}