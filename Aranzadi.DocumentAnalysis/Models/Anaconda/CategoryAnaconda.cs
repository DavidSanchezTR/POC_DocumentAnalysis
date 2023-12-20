using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda
{
    public class CategoryAnaconda
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> AcceptedByClauseType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Confidence { get; set; }

        public string Label { get; set; }

        public string Type { get; set; }
    }
}
