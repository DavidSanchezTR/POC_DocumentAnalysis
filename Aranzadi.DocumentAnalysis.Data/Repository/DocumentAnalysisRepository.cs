﻿using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
            Log.Information($"Add analysis with guid {data.Id} and file {data.DocumentName}");
            try
            {
				dbContext.Analysis.Add(data);
				return await dbContext.SaveChangesAsync();

			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error:{ex.Message}");
				throw;
			}
		}
		#endregion


		#region Update

		public async Task<int> UpdateAnalysisDataAsync(DocumentAnalysisData data)
		{
            Log.Information($"Update analysis with guid {data.Id} and file {data.DocumentName}");
			try
			{
				var item = await dbContext.Analysis.Where(e => e.Id == data.Id).FirstOrDefaultAsync();

				if (item != null)
				{
					item.Status = data.Status;
					item.Analysis = data.Analysis;
					item.AnalysisProviderId = data.AnalysisProviderId;
					item.AnalysisProviderResponse = data.AnalysisProviderResponse;
				}

				return await dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error:{ex.Message}");
				throw;
			}
            

		}

		#endregion

		#region Get

		public async Task<DocumentAnalysisResult?> GetAnalysisDoneAsync(string sha256)
        {
			try
			{
				Log.Information($"Get analysis with hash {sha256}");
				var analysis = await dbContext.Analysis.Where(e => e.Sha256 == sha256 && e.Status == AnalysisStatus.Done).Select(a => new DocumentAnalysisResult { Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis }).FirstOrDefaultAsync();
				return analysis;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error:{ex.Message}");
				throw;
			}
            
		}

		public async Task<IEnumerable<DocumentAnalysisResult>> GetAnalysisAsync(string tenantId, string userId, string? documentId = null)
		{
            Log.Information($"Get analysis for tenantId: {tenantId}, userId: {userId}, documentId: {documentId}");
            List<DocumentAnalysisResult> items = new List<DocumentAnalysisResult>();
			try
			{
				if (string.IsNullOrWhiteSpace(documentId))
				{
					var query = dbContext.Analysis.Where(e => e.TenantId == tenantId && e.UserId == userId).Select(a => new DocumentAnalysisResult { Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis });
					items = await query.ToListAsync();
				}
				else
				{
					Guid guid = new Guid(documentId);
					var analysis = await dbContext.Analysis.Where(e => e.TenantId == tenantId && e.UserId == userId && e.Id == guid).Select(a => new DocumentAnalysisResult { Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis }).FirstOrDefaultAsync();
					if (analysis != null)
						items.Add(analysis);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error:{ex.Message}");
				throw;
			}
			return items;

		}

		#endregion

	}
}
