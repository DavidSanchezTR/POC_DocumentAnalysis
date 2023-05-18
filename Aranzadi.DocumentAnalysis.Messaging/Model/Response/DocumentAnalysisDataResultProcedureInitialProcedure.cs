using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Response
{
    public class DocumentAnalysisDataResultProcedureInitialProcedure
    {
        public string Juzgado { get; set; }
        [JsonProperty("numero autos")]
        public string NumeroAutos { get; set; }
    }
}