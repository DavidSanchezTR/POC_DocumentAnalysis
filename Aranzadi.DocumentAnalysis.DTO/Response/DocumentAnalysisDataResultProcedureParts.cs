using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultProcedureParts
    {
        public string nombre { get; set; }
        public string naturaleza { get; set; }
        [JsonProperty("tipo parte")]
        public string tipoparte { get; set; }
        [JsonProperty("tipo parte recurso")]
        public string tipoparterecurso { get; set; }
        public string procurador { get; set; }
        public string letrados { get; set; }

    }
}