using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.DTO.Response;
using System;

public interface IDocumentAnalysisService
{

	Task<IEnumerable<DocumentAnalysisResponse>> GetAllAnalysisAsync(string TenantId, string UserId);

	Task<DocumentAnalysisResponse> GetAnalysisAsync(string TenantId, string UserId, Guid Guid);
	
}
