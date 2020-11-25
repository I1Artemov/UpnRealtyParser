using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpnRealtyParser.Business.Models
{
    public class UpnFlatBase : FlatCore
    {
        public int? UpnHouseInfoId { get; set; }

        public int? UpnAgencyId { get; set; }

        public string FlatType { get; set; }

        public int? JointBathrooms { get; set; }

        public int? SeparateBathrooms { get; set; }

        public string RenovationType { get; set; }

        public string RedevelopmentType { get; set; }

        public string WindowsType { get; set; }

        public string Furniture { get; set; }
    }
}