using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Newtonsoft.Json;

namespace Aranzadi.DocumentAnalysis.Services
{

	public class DocumentAnalysisService : IDocumentAnalysisService
	{
		private readonly IDocumentAnalysisRepository _documentAnalysisRepository;

		public DocumentAnalysisService(IDocumentAnalysisRepository documentAnalysisRepository)
		{
			_documentAnalysisRepository = documentAnalysisRepository;
		}

		public async Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(string tenantId, string userId, string? documentId = null)
		{
			if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentNullException();
			}

			var listaAnalisis = await _documentAnalysisRepository.GetAnalysisAsync(tenantId, userId, documentId);

			var listDocumentAnalysisResponse = new List<DocumentAnalysisResponse>();
			foreach (var singleAnalisis in listaAnalisis)
			{
				var documentAnalysisResponse = new DocumentAnalysisResponse();
				documentAnalysisResponse.DocumentUniqueRefences = singleAnalisis.DocumentId.ToString();
				documentAnalysisResponse.Status = singleAnalisis?.Status ?? AnalysisStatus.Unknown;

				if (string.IsNullOrWhiteSpace(singleAnalisis?.Analysis))
				{
					documentAnalysisResponse.Result = new DocumentAnalysisDataResultContent();
				}
				else
				{
					documentAnalysisResponse.Result = JsonConvert.DeserializeObject<DocumentAnalysisDataResultContent>(singleAnalisis.Analysis);
				}
				listDocumentAnalysisResponse.Add(documentAnalysisResponse);
			}

			return listDocumentAnalysisResponse;

		}
	}

}
