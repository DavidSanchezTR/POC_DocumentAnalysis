
using Aranzadi.DocumentAnalysis.Data.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace Aranzadi.DocumentAnalysis.Data
{
    public class DocumentAnalysisDbContext : DbContext
    {
        public DbSet<DocumentAnalysisData> Analysis => Set<DocumentAnalysisData>();

        public DocumentAnalysisDbContext(DbContextOptions<DocumentAnalysisDbContext> options)
            : base(options)
        {
        }        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DocumentAnalysisData>();

        }
    }
}