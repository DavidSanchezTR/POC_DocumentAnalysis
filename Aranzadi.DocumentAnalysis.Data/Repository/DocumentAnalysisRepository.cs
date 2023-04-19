using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Microsoft.Azure.Cosmos;
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
        private readonly DocumentAnalysisDbContext dbContext;

        public DocumentAnalysisRepository(DocumentAnalysisDbContext context)
        {
            dbContext = context;
        }
        #region Insert

        public async Task<int> AddAnalysisDataAsync(DocumentAnalysisData data)
        {
            try
            {
                data.Id = Guid.NewGuid();

                dbContext.Analysis.Add(data);

                return await dbContext.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return 0;
            }            
        }
        #endregion


        #region Update

        public async Task<int> UpdateAnalysisDataAsync(DocumentAnalysisData data)
        {
            var item = await dbContext.Analysis.Where(e => e.Id == data.Id).FirstOrDefaultAsync();
            
            if (item != null)
            {
                item.Status = data.Status;
                item.Analysis = data.Analysis;
            }

            return await dbContext.SaveChangesAsync();

        }

        #endregion

        #region Get
        public async Task<IEnumerable<DocumentAnalysisResult>> GetAllAnalysisAsync(string tenantId, string userId)
        {
            List<DocumentAnalysisResult> items = new List<DocumentAnalysisResult>();
            try
            {
                var query = dbContext.Analysis.Where(e => e.TenantId == tenantId && e.UserId == userId).Select(a => new DocumentAnalysisResult { Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis });
                items = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return items;
        }
       

        public async Task<DocumentAnalysisResult?> GetAnalysisAsync(string sha256)
        {
           var analysis = await dbContext.Analysis.Where(e => e.Sha256 == sha256 && e.Status == DTO.Enums.StatusResult.Disponible).Select(a => new DocumentAnalysisResult { Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis }).FirstOrDefaultAsync();
           return analysis;
           
            
        }

        #endregion

    }
}
