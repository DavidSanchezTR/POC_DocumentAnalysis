using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;

namespace Aranzadi.DocumentAnalysis.Configuration
{
    public static class SeedDatabase
    {
        public static void Seed(DocumentAnalysisDbContext context)
        {
            bool save = false;
            string analysisId = Guid.NewGuid().ToString();
            if (context.Analysis.Count() == 0)
            {
                context.Analysis.Add(new DocumentAnalysisData 
                    { 
                        Id = analysisId,
                        App = "Infolex",
                        DocumentName = "Prueba.pdf",
                        NewGuid = Guid.NewGuid(),
                        Analisis = "Esto es un análisis",
                        AccessUrl = "www.prueba.com",
                        Sha256 = "HasCode",
                        Estado = "Pendiente",
                        TenantId = 122,
                        UserId = 22,
                        Origen = "La Ley",
                        FechaAnalisis = DateTimeOffset.Now,
                        FechaCreacion = DateTimeOffset.Now,
                });
                save = true;
            }
            if (save)
                context.SaveChanges();
        }
    }
}
