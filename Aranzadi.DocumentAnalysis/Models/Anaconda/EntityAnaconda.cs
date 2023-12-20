using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda
{
    public class EntityAnaconda
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> AcceptedByClauseType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DataSource { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsLinkedEntity { get; set; }

        public IEnumerable<MentionAnaconda> Mentions { get; set; }

        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        public string Value { get; set; }

        public IEnumerable<EntityAnaconda> Children { get; set; }
    }
}
