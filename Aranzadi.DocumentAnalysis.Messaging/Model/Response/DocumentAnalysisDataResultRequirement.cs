
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Response
{
    public class DocumentAnalysisDataResultRequirement
    {
        public string Requerimiento { get; set; }
        public string Sintesis { get; set; }
        [JsonProperty("fecha requerimiento")]
        public string FechaRequerimiento { get; set; }
        [JsonProperty("tipo fecha")]
        public string TipoFecha { get; set; }
        public string Plazo { get; set; }
        public string Parte { get; set; }
        [JsonProperty("tipo requerimiento")]
        public string TipoRequerimiento { get; set; }
        public string Sala { get; set; }
    }
}