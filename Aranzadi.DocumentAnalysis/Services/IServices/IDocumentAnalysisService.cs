using Aranzadi.DocumentAnalysis.Models;
using System;

public interface IDocumentAnalysisService
{

	Task<string> GetSingleAnalysis(DocumentAnalysisData data, int LawfirmId);	

	Task<byte[]> GetBytes(DocumentAnalysisData data, int LawfirmId);
}
