using Aranzadi.DocumentAnalysis.Models;
using System;

public interface IDocumentAnalysisService
{

	Task<string> GetSingleAnalysisAsync(DocumentAnalysisData data, int LawfirmId);	

	Task<byte[]> GetBytesAsync(DocumentAnalysisData data, int LawfirmId);
}
