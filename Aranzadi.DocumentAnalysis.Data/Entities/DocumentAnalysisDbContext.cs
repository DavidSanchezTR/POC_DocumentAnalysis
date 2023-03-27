
using Microsoft.EntityFrameworkCore;

namespace Aranzadi.DocumentAnalysis.Data
{
    public class DocumentAnalysisDbContext : DbContext
    {
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