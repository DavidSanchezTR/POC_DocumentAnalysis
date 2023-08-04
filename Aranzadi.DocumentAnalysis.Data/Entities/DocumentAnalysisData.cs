using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Aranzadi.DocumentAnalysis.Data.Entities
{
	public class DocumentAnalysisData
    {
        [Key]        
        public Guid Id { get; set; }
        [Required]
        public string? App { get; set; }
        [Required]
        public string? TenantId { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? DocumentName { get; set; }
        [Required]
        public string? AccessUrl { get; set; }
        [Required]
        public string? Sha256 { get; set; }
        [Required]
        public DateTimeOffset CreateDate { get; set; }
        [Required]
        public DateTimeOffset AnalysisDate { get; set; }        
        public string? Analysis { get; set; }
        [Required]
        public Source? Source { get; set; }
        [Required]
        public AnalysisStatus Status { get; set; }
		public Guid? AnalysisProviderId { get; set; }
        public string? AnalysisProviderResponse { get; set; }

	}
}
