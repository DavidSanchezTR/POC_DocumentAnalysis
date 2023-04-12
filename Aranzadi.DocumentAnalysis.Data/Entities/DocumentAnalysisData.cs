using Aranzadi.DocumentAnalysis.DTO.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Data.Entities
{
    public class DocumentAnalysisData
    {
        [Key]        
        public Guid Id { get; set; }
        [Required]
        public string App { get; set; }
        [Required]
        public string TenantId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string DocumentName { get; set; }
        [Required]
        public string AccessUrl { get; set; }
        [Required]
        public string Sha256 { get; set; }
        [Required]
        public DateTimeOffset CreateDate { get; set; }
        [Required]
        public DateTimeOffset AnalysisDate { get; set; }        
        public string? Analysis { get; set; }
        [Required]
        public Source Source { get; set; }
        [Required]
        public StatusResult Status { get; set; }
    }
}
