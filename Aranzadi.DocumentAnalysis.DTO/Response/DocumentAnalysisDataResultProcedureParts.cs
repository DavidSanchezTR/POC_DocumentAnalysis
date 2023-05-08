using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultProcedureParts
    {
        public string Name { get; set; }
        public string Nature { get; set; }
        [JsonProperty("tipo parte")]
        public string PartType { get; set; }
        [JsonProperty("tipo parte recurso")]
        public string ResourcePartType { get; set; }
        public string Procurator { get; set; }
        public string Lawyers { get; set; }

    }
}
