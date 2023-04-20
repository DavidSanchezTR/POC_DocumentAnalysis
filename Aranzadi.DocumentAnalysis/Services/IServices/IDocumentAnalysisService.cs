using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.DTO.Response;
using System;

public interface IDocumentAnalysisService
{

	Task<string> GetAnalysisAsync(DocumentAnalysisData data, int TenantId);	

	Task<byte[]> GetBytesAsync(DocumentAnalysisData data, int LawfirmId);

	Task<IEnumerable<DocumentAnalysisResponse>> GetAllAnalysisAsync(string TenantId, string UserId);

	Task<DocumentAnalysisResponse> GetAnalysisAsync(string TenantId, string UserId, Guid Guid);
	
}
