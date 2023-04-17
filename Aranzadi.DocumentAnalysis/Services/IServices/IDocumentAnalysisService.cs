using Aranzadi.DocumentAnalysis.Data.Entities;
using System;

public interface IDocumentAnalysisService
{

	Task<string> GetAnalysisAsync(DocumentAnalysisData data, int TenantId);	

	Task<byte[]> GetBytesAsync(DocumentAnalysisData data, int LawfirmId);

	Task<IEnumerable<DocumentAnalysisResult>> GetAllAnalysisAsync(string TenantId, string UserId);

	Task<DocumentAnalysisResult?> GetAnalysisAsync(String Sha256);
	
}
