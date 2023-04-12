﻿using Aranzadi.DocumentAnalysis.Data.Entities;
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
        private readonly DocumentAnalysisDbContext dbContext;

        public DocumentAnalysisRepository(DocumentAnalysisDbContext context)
        {
            dbContext = context;
        }
        public async Task<int> AddAnalysisDataAsync(DocumentAnalysisData data)
        {
            try
            {
                data.Id = Guid.NewGuid();

                dbContext.Add(data);

                return await dbContext.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return 0;
            }            
        }

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

        public async Task<DocumentAnalysisResult> GetAnalysisAsync(string TenantId, string UserId, Guid DocumentId)
        {
            DocumentAnalysisResult analysis = new DocumentAnalysisResult();
            try
            {
               var analysisResult = await dbContext.Analysis.Where(e => e.TenantId == TenantId && e.UserId == UserId && e.Id == DocumentId).Select(a => new DocumentAnalysisResult { Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis }).FirstAsync();

                if (analysisResult == null)
                {
                    throw new NullReferenceException();
                }
            }
            
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return analysis;
        }
    }
}
