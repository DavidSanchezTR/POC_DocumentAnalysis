

using Aranzadi.DocumentAnalysis.DTO.Enums;

namespace Aranzadi.DocumentAnalysis.Data.Entities
{
    public class DocumentAnalysisResult
    {
        public Guid DocumentId { get; set; }

        public string? Analysis { get; set; }

        public StatusResult Status { get; set; }

        
    }
}