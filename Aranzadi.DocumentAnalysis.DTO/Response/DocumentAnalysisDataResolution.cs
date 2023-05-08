using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResolution
    {
        [JsonProperty("tipo resolucion")]
        public string tiporesolucion { get; set; }
        [JsonProperty("subtipo resolucion")]
        public string subtiporesolucion { get; set; }
        [JsonProperty("numero resolucion")]
        public string numeroresolucion { get; set; }
        [JsonProperty("fecha resolucion")]
        public string fecharesolucion { get; set; }
        [JsonProperty("fecha notificacion")]
        public string fechanotificacion { get; set; }
        public string hito { get; set; }
        public string hito_origin { get; set; }
        public string cuantia { get; set; }
        [JsonProperty("resumen escrito")]
        public string resumenescrito { get; set; }
        public DocumentAnalysisDataResultRequirement[] requerimientos { get; set; }
        public DocumentAnalysisDataResultResource[] recurso { get; set; }

    }
}
