

using Aranzadi.DocumentAnalysis.DTO.Enums;
using Aranzadi.DocumentAnalysis.DTO.Response;

namespace Aranzadi.DocumentAnalysis.Data.Entities
{
    public class DocumentAnalysisResult
    {
        public Guid DocumentId { get; set; }

        public string? Analysis { get; set; }

        public AnalysisStatus Status { get; set; }

	}
}