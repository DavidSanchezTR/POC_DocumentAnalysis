using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Data.Entities;
using System;

public interface IDocumentAnalysisService
{

	Task<string> GetAnalysisAsync(Aranzadi.DocumentAnalysis.Models.DocumentAnalysisData data, int TenantId);	

	Task<byte[]> GetBytesAsync(Aranzadi.DocumentAnalysis.Models.DocumentAnalysisData data, int LawfirmId);

	Task<IEnumerable<DocumentAnalysisResult>> GetAllAnalysisAsync(string TenantId, string UserId);

	Task<DocumentAnalysisResult> GetAnalysisAsync(string TenantId, string UserId, Guid DocumentId);
	
}
