using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultProcedure
    {
        [JsonProperty("N.I.G.")]
        public string NIG { get; set; }
        [JsonProperty("tipo procedimiento")]
        public string ProcedureType { get; set; }
        [JsonProperty("subtipo procedimiento")]
        public string ProcedureSubtype { get; set; }
        [JsonProperty("numero autos")]
        public string CourtOrders { get; set; }
        public DocumentAnalysisDataResultProcedureParts[] Parts { get; set; }
        [JsonProperty("procedimiento inicial")]
        public DocumentAnalysisDataResultProcedureInitialProcedure InitialProcedure { get; set; }
    }
}
