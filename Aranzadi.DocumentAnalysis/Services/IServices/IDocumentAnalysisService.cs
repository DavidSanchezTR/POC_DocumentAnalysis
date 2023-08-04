using Aranzadi.DocumentAnalysis.Messaging.Model.Response;

namespace Aranzadi.DocumentAnalysis.Services.IServices
{
	public interface IDocumentAnalysisService
	{
		Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(string tenantId, string userId, string? documentId = null);

	}
}
