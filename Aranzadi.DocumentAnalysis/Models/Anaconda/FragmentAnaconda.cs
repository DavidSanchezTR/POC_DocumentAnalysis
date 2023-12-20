using Newtonsoft.Json;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda
{
    public class FragmentAnaconda
    {
        /// <summary>
        /// Indica si se ejecutó algun tipo de analisis especifico para este fragmento.
        /// No todos los tipos de fragmentos son admitidos para su analisis
        /// </summary>
        public bool AnalysisAllowed { get; set; }

        public string ContainerName { get; set; }

        public string Content { get; set; }

        public string HtmlElementId { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// Indica si el fragmento es considerado principal dentro de un documento
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// Indica si la información de este fragmento es recomendable que sea revisada manualmente 
        /// (por ejemplo antes de disparar alguna accion automatica en base a algun dato del mismo)
        /// </summary>
        public bool ManualReviewRequired { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OccurrenceOrder { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// Datos Categoria asignados al fragmento
        /// </summary>
        public IEnumerable<CategoryAnaconda> Categories { get; set; }

        /// <summary>
        /// Datos Entidad encontrados en el fragmento
        /// </summary>
        public IEnumerable<EntityAnaconda> Entities { get; set; }

        /// <summary>
        /// Lista de Hitos/Requerimientos relacionados con un proceso 
        /// </summary>
        public IEnumerable<EventAnaconda> Events { get; set; }
    }
}
