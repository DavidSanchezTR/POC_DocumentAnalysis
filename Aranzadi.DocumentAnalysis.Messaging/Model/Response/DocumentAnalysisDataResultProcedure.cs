using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Response
{
    public class DocumentAnalysisDataResultProcedure
    {
        [JsonProperty("N.I.G.")]
        public string NIG { get; set; }
        [JsonProperty("tipo procedimiento")]
        public string TipoProcedimiento { get; set; }
        [JsonProperty("subtipo procedimiento")]
        public string SubtipoProcedimiento { get; set; }
        [JsonProperty("numero autos")]
        public string NumeroAutos { get; set; }
        public DocumentAnalysisDataResultProcedureParts[] Partes { get; set; }
        [JsonProperty("procedimiento inicial")]
        public DocumentAnalysisDataResultProcedureInitialProcedure ProcedimientoInicial { get; set; }
    }
}