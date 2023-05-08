using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultRequirement
    {
        public string Requirement { get; set; }
        public string Synthesis { get; set; }
        [JsonProperty("fecha requerimiento")]
        public string DateRequest { get; set; }
        [JsonProperty("tipo fecha")]
        public string DateType { get; set; }
        public string Deadline { get; set; }
        public string Part { get; set; }
        [JsonProperty("tipo requerimiento")]
        public string RequirementType { get; set; }
        public string Hall { get; set; }
    }
}
