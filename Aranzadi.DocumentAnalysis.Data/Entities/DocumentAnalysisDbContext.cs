
using Aranzadi.DocumentAnalysis.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Aranzadi.DocumentAnalysis.Data
{
    public class DocumentAnalysisDbContext : DbContext
    {
        public DbSet<DocumentAnalysisData> AnalysisData => Set<DocumentAnalysisData>();

        public DocumentAnalysisDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseCosmos(
                    "https://uksouth-das-cosmos-dev.mongo.cosmos.azure.com:10255/",
                    "SlA98NPnfxekWsecVxydj3J3BTGtcfzWljltyNyaRAIRmsJqjIPLZfItRGZ9rsmT0nx9qcrwZVTCpeBaU12CKw==",
                    databaseName: "AnalysisService",
                    optionsBuilder =>
                    {
                        optionsBuilder.RequestTimeout(TimeSpan.FromMinutes(5));
                    });        
    }
}