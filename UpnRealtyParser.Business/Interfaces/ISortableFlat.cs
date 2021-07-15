using System;

namespace UpnRealtyParser.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для картир, по которым можно выполнить сортировку в Antd-таблице
    /// </summary>
    public interface ISortableFlat
    {
        int? Id { get; set; }
        DateTime? LastCheckDate { get; set; }
        int? HouseBuildYear { get; set; }
        int? Price { get; set; }
        double? SpaceSum { get; set; }
        double? ClosestSubwayStationRange { get; set; }
    }
}
