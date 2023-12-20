using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
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
				throw;
			}
		}

		#endregion Insert

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
				throw;
			}
		}

		#endregion Update

		#region Get

		public async Task<DocumentAnalysisData?> GetAnalysisDataAsync(string documentId)
		{
			Log.Information($"Get analysis for documentId:{documentId}");
			try
			{
				Guid guid = new Guid(documentId);
				var analysis = await dbContext.Analysis.Where(e => e.Id == guid).FirstOrDefaultAsync();
				return analysis;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<DocumentAnalysisResult?> GetAnalysisDoneAsync(string sha256)
        {
			try
			{
				Log.Information($"Get analysis with hash {sha256}");
				var analysis = await dbContext.Analysis.Where(e => e.Sha256 == sha256 && e.Status == AnalysisStatus.Done).Select(a => new DocumentAnalysisResult { Type = a.AnalysisType ?? AnalysisTypes.Undefined, Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis }).FirstOrDefaultAsync();
				return analysis;
			}
			catch (Exception ex)
			{
				throw;
			}			
		}

		public async Task<DocumentAnalysisResult> GetAnalysisAsync(string tenantId, string documentId)
		{
            Log.Information($"Get analysis for tenantId: {tenantId}, documentId: {documentId}");
            DocumentAnalysisResult item = new DocumentAnalysisResult();
            try
			{
				if (!string.IsNullOrEmpty(documentId))
				{
                    Guid guid = new Guid(documentId);

					var analysis = await dbContext.Analysis.Where(e => e.TenantId == tenantId && e.Id == guid).Select(a => new DocumentAnalysisResult { Type = a.AnalysisType ?? AnalysisTypes.Undefined, Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis }).FirstOrDefaultAsync();
					if (analysis != null)
                        item = analysis;
                }
			}
			catch (Exception ex)
			{
				throw;
			}
			return item;

		}

		public async Task<IEnumerable<DocumentAnalysisResult>> GetAnalysisListAsync(string tenantId, string documentIdList)
		{
            Log.Information($"Get analysis for tenantId: {tenantId}, documentIdList: {documentIdList}");
            List<DocumentAnalysisResult> analysis = new List<DocumentAnalysisResult>();

            try
			{
                if (!string.IsNullOrEmpty(documentIdList))
                {
					List<Guid> listaDocsGuid = documentIdList.Split(';').Select(x => new Guid(x)).ToList();
                    analysis = await dbContext.Analysis
						.Where(e => e.TenantId == tenantId && listaDocsGuid.Contains(e.Id) && !e.Status.Equals(AnalysisStatus.Pending))
						.Select(a => new DocumentAnalysisResult { Type = a.AnalysisType ?? AnalysisTypes.Undefined, Status = a.Status, DocumentId = a.Id, Analysis = a.Analysis }).ToListAsync();
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			
			return analysis;
		}

		#endregion Get

	}
}
