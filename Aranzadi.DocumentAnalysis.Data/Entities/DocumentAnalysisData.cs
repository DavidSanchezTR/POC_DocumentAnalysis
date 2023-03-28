using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Data.Entities
{
    public class DocumentAnalysisData
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [Required]
        public string App { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string DocumentName { get; set; }
        [Required]
        public string AccessUrl { get; set; }
        [Required]
        public Guid NewGuid { get; set; }
        [Required]
        public string Sha256 { get; set; }
        [Required]
        public DateTimeOffset FechaCreacion { get; set; }
        [Required]
        public DateTimeOffset FechaAnalisis { get; set; }        
        public string? Analisis { get; set; }
        [Required]
        public string Origen { get; set; }
        [Required]
        public string Estado { get; set; }
    }
}
