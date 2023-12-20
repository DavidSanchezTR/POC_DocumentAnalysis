using Aranzadi.DocumentAnalysis.Messaging.Model.Response;

namespace Aranzadi.DocumentAnalysis.Services.IServices
{
	public interface IDocumentAnalysisService
	{
		Task<DocumentAnalysisResponse> GetAnalysisAsync(string tenantId, string documentId);

		Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisListAsync(string tenantId, string documentIdList);
    }
}
