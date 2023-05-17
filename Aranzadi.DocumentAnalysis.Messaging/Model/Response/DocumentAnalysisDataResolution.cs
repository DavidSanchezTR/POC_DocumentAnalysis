using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Response
{
    public class DocumentAnalysisDataResolution
    {
        [JsonProperty("tipo resolucion")]
        public string TipoResolucion { get; set; }
        [JsonProperty("subtipo resolucion")]
        public string SubtipoResolucion { get; set; }
        [JsonProperty("numero resolucion")]
        public string NumeroResolucion { get; set; }
        [JsonProperty("fecha resolucion")]
        public string FechaResolucion { get; set; }
        [JsonProperty("fecha notificacion")]
        public string FechaNotificacion { get; set; }
        public string Hito { get; set; }
        public string Hito_Origin { get; set; }
        public string Cuantia { get; set; }
        [JsonProperty("resumen escrito")]
        public string ResumenEscrito { get; set; }
        public DocumentAnalysisDataResultRequirement[] Requerimientos { get; set; }
        public DocumentAnalysisDataResultResource[] Recurso { get; set; }

    }
}
