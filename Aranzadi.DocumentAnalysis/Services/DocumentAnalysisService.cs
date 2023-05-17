using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Newtonsoft.Json;

public class DocumentAnalysisService : IDocumentAnalysisService
{
	private readonly IDocumentAnalysisRepository _documentAnalysisRepository;
	private readonly ILogger<DocumentAnalysisService> _logger;

	public DocumentAnalysisService(IDocumentAnalysisRepository documentAnalysisRepository, ILogger<DocumentAnalysisService> logger)
	{
		_documentAnalysisRepository = documentAnalysisRepository;
		_logger = logger;
	}

	public async Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(string tenantId, string userId, string? documentId = null)
	{
		if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
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
