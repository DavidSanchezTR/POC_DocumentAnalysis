using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.DTO.Response;
using System;

public interface IDocumentAnalysisService
{
	Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(string TenantId, string UserId, string DocumentId = null);

}
