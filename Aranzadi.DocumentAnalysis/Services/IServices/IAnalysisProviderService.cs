using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Models;

namespace Aranzadi.DocumentAnalysis.Services.IServices
{
	public interface IAnalysisProviderService
	{
		Task<(HttpResponseMessage, AnalysisJobResponse?)> SendAnalysisJob(DocumentAnalysisData data);
	}
}
