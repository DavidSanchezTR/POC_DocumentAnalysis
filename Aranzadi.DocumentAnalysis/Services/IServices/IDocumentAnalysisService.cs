using Aranzadi.DocumentAnalysis.Messaging.Model.Response;

public interface IDocumentAnalysisService
{
	Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(string tenantId, string userId, string? documentId = null);

}
