
using Aranzadi.DocumentAnalysis.Data.Entities;
using Microsoft.EntityFrameworkCore;
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
            builder.Entity<DocumentAnalysisData>().HasPartitionKey(x => x.NewGuid).Property(y => y.Analisis).HasConversion(
                        j => ToJson(j),
                        j => FromJson<string>(j)
            );
            base.OnModelCreating(builder);            
        }
        public static string ToJson<T>(T item) => JsonSerializer.Serialize(item);
        public static T? FromJson<T>(string json) => JsonSerializer.Deserialize<T>(json);
        public string? GetCosmosContainerName(Type type)
        {
            var entityType = Model.FindEntityType(type);
            return entityType?.GetContainer();
        }
    }
}