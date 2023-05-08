using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResolution
    {
        [JsonProperty("tipo resolucion")]
        public string ResolutionType { get; set; }
        [JsonProperty("subtipo resolucion")]
        public string SubTypeResolution { get; set; }
        [JsonProperty("numero resolucion")]
        public string ResolutionNumber { get; set; }
        [JsonProperty("fecha resolucion")]
        public string ResolutionDate { get; set; }
        [JsonProperty("fecha notificacion")]
        public string NotificationDate { get; set; }
        public string Landmark { get; set; }
        public string LandmarkOrigin { get; set; }
        public string Amount { get; set; }
        [JsonProperty("resumen escrito")]
        public string WrittenSummary { get; set; }
        public DocumentAnalysisDataResultRequirement[] Requirements { get; set; }
        public DocumentAnalysisDataResultResource[] Resource { get; set; }

    }
}
