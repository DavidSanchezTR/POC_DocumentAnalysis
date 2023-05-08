
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultRequirement
    {
        public string requerimiento { get; set; }
        public string sintesis { get; set; }
        [JsonProperty("fecha requerimiento")]
        public string fecharequerimiento { get; set; }
        [JsonProperty("tipo fecha")]
        public string tipofecha { get; set; }
        public string plazo { get; set; }
        public string parte { get; set; }
        [JsonProperty("tipo requerimiento")]
        public string tiporequerimiento { get; set; }
        public string sala { get; set; }
    }
}