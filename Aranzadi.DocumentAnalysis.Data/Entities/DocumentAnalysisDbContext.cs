
using Aranzadi.DocumentAnalysis.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aranzadi.DocumentAnalysis.Data
{
    public class DocumentAnalysisDbContext : DbContext
    {

		public static string DatabaseName = "documentAnalysis";

		public DbSet<DocumentAnalysisData> DocumentAnalysisData => Set<DocumentAnalysisData>();

        public DocumentAnalysisDbContext(DbContextOptions<DocumentAnalysisDbContext> options)
            : base(options) 
        {            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);            
        } 
        
    }
}