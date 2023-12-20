using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
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

        public async Task<DocumentAnalysisResponse> GetAnalysisAsync(string tenantId, string documentId)
        {
            if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentNullException();
            }

            Data.Entities.DocumentAnalysisResult analisis = await _documentAnalysisRepository.GetAnalysisAsync(tenantId, documentId);

            DocumentAnalysisResponse documentAnalysisResponse = new DocumentAnalysisResponse()
            {
                DocumentUniqueRefences = analisis.DocumentId.ToString(),
                Status = analisis?.Status ?? AnalysisStatus.Unknown
            };

            if (analisis?.Type == AnalysisTypes.Demand)
            {
                documentAnalysisResponse.ResultAppeal = new AppealsNotification(new AppealsProvider(string.IsNullOrWhiteSpace(analisis?.Analysis)
                    ? new DocumentAnalysisAnaconda()
                    : JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(analisis.Analysis)));
            }
            else
            {
                documentAnalysisResponse.Result = new JudicialNotification(new JudicialNotificationProvider(string.IsNullOrWhiteSpace(analisis?.Analysis)
                    ? new DocumentAnalysisAnaconda()
                    : JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(analisis.Analysis)));
            }

            return documentAnalysisResponse;
        }

        public async Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisListAsync(string tenantId, string documentIdList)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException();
            }

            IEnumerable<Data.Entities.DocumentAnalysisResult> listaAnalisis =
                await _documentAnalysisRepository.GetAnalysisListAsync(tenantId, documentIdList);

            List<DocumentAnalysisResponse> listDocumentAnalysisResponse = new List<DocumentAnalysisResponse>();
            foreach (Data.Entities.DocumentAnalysisResult singleAnalisis in listaAnalisis)
            {
                DocumentAnalysisResponse documentAnalysisResponse = new DocumentAnalysisResponse()
                {   
                    DocumentUniqueRefences = singleAnalisis.DocumentId.ToString(),
                    Status = singleAnalisis?.Status ?? AnalysisStatus.Unknown,
                };

                if (singleAnalisis?.Type == AnalysisTypes.Demand)
                {
                    documentAnalysisResponse.ResultAppeal = new AppealsNotification(new AppealsProvider(string.IsNullOrWhiteSpace(singleAnalisis?.Analysis)
                        ? new DocumentAnalysisAnaconda()
                        : JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(singleAnalisis.Analysis)));
				}
                else
                {
                    documentAnalysisResponse.Result = new JudicialNotification(new JudicialNotificationProvider(string.IsNullOrWhiteSpace(singleAnalisis?.Analysis)
                        ? new DocumentAnalysisAnaconda()
                        : JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(singleAnalisis.Analysis)));
				}

                listDocumentAnalysisResponse.Add(documentAnalysisResponse);
            }

            return listDocumentAnalysisResponse;
        }
    }
}