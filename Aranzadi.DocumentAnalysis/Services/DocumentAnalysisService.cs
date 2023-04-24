using Aranzadi.DocumentAnalysis.Data.IRepository;
using System;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.DTO.Response;
using System.Reflection.Metadata.Ecma335;
using Aranzadi.DocumentAnalysis.DTO.Enums;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

public class DocumentAnalysisService : IDocumentAnalysisService
{
	private readonly IDocumentAnalysisRepository _documentAnalysisRepository;
	private readonly ILogger<DocumentAnalysisService> _logger;

	public DocumentAnalysisService(IDocumentAnalysisRepository documentAnalysisRepository, ILogger<DocumentAnalysisService> logger)
	{
		_documentAnalysisRepository = documentAnalysisRepository;
		_logger = logger;
	}

	public async Task<IEnumerable<DocumentAnalysisResponse>> GetAllAnalysisAsync(string tenantId, string userId)
	{
		if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
		{
			throw new ArgumentNullException();
		}

		var listaAnalisis = await _documentAnalysisRepository.GetAllAnalysisAsync(tenantId, userId);

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

	public async Task<DocumentAnalysisResponse> GetAnalysisAsync(string tenantId, string userId, Guid documentId)
	{
		if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId) || documentId == Guid.Empty)
		{
			throw new ArgumentNullException();
		}

		var singleAnalisis = await _documentAnalysisRepository.GetAnalysisAsync(tenantId, userId, documentId);

		if (singleAnalisis == null)
		{
			throw new NullReferenceException($"Analysis not found with tenand {tenantId}, user {userId}, guid {documentId.ToString()}");
		}
		else
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
			return documentAnalysisResponse;
		}
	}
}
