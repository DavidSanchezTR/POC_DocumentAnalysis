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

	public async Task<string> GetAnalysisAsync(DocumentAnalysisData data, int LawfirmId)
	{
		//2.Consultar si existe el analysis del documento
		var resultAnalysis = await _documentAnalysisRepository.GetAnalysisAsync(data.TenantId, data.UserId, data.Id);
		if (resultAnalysis != null)
		{
			data.Status = resultAnalysis.Status;
			data.Analysis = resultAnalysis.Analysis;
			//3.Guardar peticion analisis con resultado en cosmos
			await _documentAnalysisRepository.UpdateAnalysisDataAsync(data);
		}
		else
		{
			//3.Guardar peticion analisis pendiente en cosmos
			await _documentAnalysisRepository.AddAnalysisDataAsync(data);
		}
		/*TODO
         * 
            1.Recuperar la url del documento.            
            4.Crear petición de análisis al proveedor de análisis.
            5.Modificar el registro de CosmosDB con el resultado del análisis.
            6.Devolver resultado.                  
         */
		throw new NotImplementedException();
	}

	public Task<byte[]> GetBytesAsync(DocumentAnalysisData data, int LawfirmId)
	{
		/*
            Leer documento.
         */
		throw new NotImplementedException();
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
