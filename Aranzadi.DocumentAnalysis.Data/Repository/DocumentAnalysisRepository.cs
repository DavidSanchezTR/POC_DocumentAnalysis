using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Data.Repository
{
    public class DocumentAnalysisRepository : IDocumentAnalysisRepository
    {
        public async Task AddAnalysisDataAsync(DocumentAnalysisData data)
        {
            try
            {
                using (var context = new DocumentAnalysisDbContext())
                {
                    var datos = new DocumentAnalysisData
                    {
                        App = data.App,
                        DocumentName = data.DocumentName,
                        AccessUrl = data.AccessUrl,
                        Analisis = data.Analisis,
                        Estado = data.Estado,
                        Id = data.Id,
                        NewGuid = data.NewGuid,
                        Origen = data.Origen,
                        Sha256 = data.Sha256,
                        TenantId = data.TenantId,
                        UserId = data.UserId,
                        FechaAnalisis = data.FechaAnalisis,
                        FechaCreacion = data.FechaCreacion,
                    };

                    context.Add(datos);

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }            
        }

        public Task<IEnumerable<DocumentAnalysisResult>> GetAllAnalysisAsync(int LawfirmId)
        {
            throw new NotImplementedException();
        }
    }
}
