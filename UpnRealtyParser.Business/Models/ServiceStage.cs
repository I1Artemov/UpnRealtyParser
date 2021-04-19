using System;

namespace UpnRealtyParser.Business.Models
{
    public class ServiceStage
    {
        public int Id { get; set; }

        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Название сервиса, например, "Upn" или "N1"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Название этапа обработки, например, ObservingFlats или GatheringLinks
        /// </summary>
        public string CurrentStage { get; set; }

        public bool? IsComplete { get; set; }

        /// <summary>
        /// Номер страницы (для режима GatheringLinks)
        /// </summary>
        public int? PageNumber { get; set; }
    }
}