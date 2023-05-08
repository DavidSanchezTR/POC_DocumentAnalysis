using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultProcedureInitialProcedure
    {
        public string Court { get; set; }
        [JsonProperty("numero autos")]
        public string CourtOrders { get; set; }
    }
}
