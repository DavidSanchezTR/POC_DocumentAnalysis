using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Response
{
    public class DocumentAnalysisDataResultProcedureParts
    {
        public string Nombre { get; set; }
        public string Naturaleza { get; set; }
        [JsonProperty("tipo parte")]
        public string TipoParte { get; set; }
        [JsonProperty("tipo parte recurso")]
        public string TipoParteRecurso { get; set; }
        public string Procurador { get; set; }
        public string Letrados { get; set; }

    }
}