
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;

namespace Aranzadi.DocumentAnalysis.Data.Entities
{
    public class DocumentAnalysisResult
    {
        public Guid DocumentId { get; set; }

        public string? Analysis { get; set; }

        public AnalysisStatus Status { get; set; }

		public AnalysisTypes Type { get; set; }

	}
}