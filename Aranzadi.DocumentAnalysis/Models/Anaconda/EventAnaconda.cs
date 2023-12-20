namespace Aranzadi.DocumentAnalysis.Models.Anaconda
{
    public class EventAnaconda
    {
        /// <summary>
        /// Indioca si el evento es considerado principal
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// Tipo de evento
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Datos Categoria asignados al Evento
        /// </summary>
        public IEnumerable<CategoryAnaconda> Categories { get; set; }

        /// <summary>
        /// Datos Entidad asignados al evento
        /// </summary>
        public IEnumerable<EntityAnaconda> Entities { get; set; }
    }
}
