using Aranzadi.DocumentAnalysis.Models;
using System;

public class DocumentAnalysisService : IDocumentAnalysisService
{
	public DocumentAnalysisService()
	{
	}

    public Task<string> GetSingleAnalysisAsync(DocumentAnalysisData data, int LawfirmId)
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

    
}
