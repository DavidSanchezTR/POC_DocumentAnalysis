using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultJudgement
    {
        public string Name { get; set; }
        public string Jurisdiction { get; set; }

        [JsonProperty("tipo tribunal")]
        public string CourtType { get; set; }
        public string City { get; set; }
        public string Number { get; set; }

    }
}
