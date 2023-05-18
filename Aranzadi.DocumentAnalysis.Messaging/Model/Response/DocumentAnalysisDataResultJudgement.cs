using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Response
{
    public class DocumentAnalysisDataResultJudgement
    {
        public string Nombre { get; set; }
        public string Jurisdiccion { get; set; }

        [JsonProperty("tipo tribunal")]
        public string TipoTribunal { get; set; }
        public string Ciudad { get; set; }
        public string Numero { get; set; }

    }
}