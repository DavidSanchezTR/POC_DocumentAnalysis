using Newtonsoft.Json;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda
{
    public class DocumentAnalysisAnaconda
    {
        public string Id { get; set; }

        public IEnumerable<ApiLinkAnaconda> Links { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Preview { get; set; }

        public string Type { get; set; }

        public IEnumerable<CategoryAnaconda> Categories { get; set; }

        public IEnumerable<EntityAnaconda> Entities { get; set; }

        public IEnumerable<FragmentAnaconda> Fragments { get; set; }
    }
}
