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
        public string tipoprocedimiento { get; set; }
        [JsonProperty("subtipo procedimiento")]
        public string subtipoprocedimiento { get; set; }
        [JsonProperty("numero autos")]
        public string numeroautos { get; set; }
        public DocumentAnalysisDataResultProcedureParts[] partes { get; set; }
        [JsonProperty("procedimiento inicial")]
        public DocumentAnalysisDataResultProcedureInitialProcedure procedimientoinicial { get; set; }
    }
}