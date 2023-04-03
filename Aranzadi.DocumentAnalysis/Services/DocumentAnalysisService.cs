using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Models;
using System;

public class DocumentAnalysisService : IDocumentAnalysisService
{
    private readonly IDocumentAnalysisRepository _documentAnalysisRepository;    
    private readonly ILogger<DocumentAnalysisService> _logger;

    public DocumentAnalysisService(IDocumentAnalysisRepository documentAnalysisRepository, ILogger<DocumentAnalysisService> logger)
	{
        _documentAnalysisRepository = documentAnalysisRepository;
        _logger = logger;   
	}

    public Task<string> GetAnalysisAsync(DocumentAnalysisData data, int LawfirmId)
    {
        /*TODO
         * 
            Recuperar la url del documento.
            Crear petición de análisis al proveedor de análisis.
            Modificar el registro de CosmosDB con el resultado del análisis.
            Devolver resultado.                  
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

    public async Task<IEnumerable<Aranzadi.DocumentAnalysis.Data.Entities.DocumentAnalysisResult>> GetAllAnalysisAsync(string tenantId, string userId)
    {
        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
        {
            throw new NullReferenceException();
        }

        var listaAnalisis = await _documentAnalysisRepository.GetAllAnalysisAsync(tenantId, userId);

        return listaAnalisis;
    }

    public async Task<Aranzadi.DocumentAnalysis.Data.Entities.DocumentAnalysisResult> GetAnalysisAsync(string tenantId, string userId, Guid documentId)
    {
        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId) || documentId == Guid.Empty)
        {
            throw new NullReferenceException();
        }

        var singleAnalisis = await _documentAnalysisRepository.GetAnalysisAsync(tenantId, userId, documentId);

        return singleAnalisis;        
    }
}
