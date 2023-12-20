using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;

namespace Aranzadi.DocumentAnalysis.Configuration
{
    public static class SeedDatabase
    {
        public static void Seed(DocumentAnalysisDbContext context)
        {
            bool save = false;
            var element = context.Analysis.FirstOrDefault();

            if (element is null)
            {
                context.Analysis.Add(new DocumentAnalysisData 
                    { 
                        Id = Guid.NewGuid(),
                        App = "Infolex",
                        DocumentName = "Prueba.pdf",                        
                        Analysis = "Esto es un análisis",
                        AccessUrl = "www.prueba.com",
                        Sha256 = "HasCode",
                        Status = AnalysisStatus.Pending,
                        TenantId = "0",
                        UserId = "22",
                        Source = Source.LaLey,
                        AnalysisDate = DateTimeOffset.Now,
                        CreateDate = DateTimeOffset.Now,
                });
                save = true;
            }
            if (save)
                context.SaveChanges();
        }
    }
}
