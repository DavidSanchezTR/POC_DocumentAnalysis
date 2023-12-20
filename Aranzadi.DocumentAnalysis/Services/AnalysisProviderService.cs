using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Util;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Aranzadi.DocumentAnalysis.Services
{
	public class AnalysisProviderService : IAnalysisProviderService
	{

		private readonly DocumentAnalysisOptions configuration;
		private readonly IHttpClientFactory httpClientFactory;

		public AnalysisProviderService(DocumentAnalysisOptions configuration
			, IHttpClientFactory httpClientFactory)
		{
			this.configuration = configuration;
			this.httpClientFactory = httpClientFactory;
		}

		public async Task<(HttpResponseMessage, AnalysisJobResponse?)> SendAnalysisJob(DocumentAnalysisData data)
		{
			try
			{
				if (data == null)
				{
					throw new ArgumentNullException(nameof(data));
				}

				Log.Information($"Send analysis job with guid {data.Id} and file {data.DocumentName}");

				var content = GetStringContent(data);
				using HttpClient client = httpClientFactory.CreateClient();
				var resp = await Utils.GetRetryPolicy().ExecuteAsync(async () =>
				{
					client.DefaultRequestHeaders.Add("ocp-apim-subscription-key", configuration.AnalysisProvider.ApiKey);
					return await client.PostAsync(configuration.AnalysisProvider.UrlApiJobs, content);
				});

				var response = await resp.Content.ReadAsStringAsync();
				AnalysisJobResponse analisisJobResponse = JsonConvert.DeserializeObject<AnalysisJobResponse>(response);
				return (resp, analisisJobResponse);
			}
			catch (Exception ex)
			{
				Log.Error($"Error al enviar una solicitud de análisis con id '{data?.Id}'", $"Error:{ex.Message}");
				throw;
			}
		}

		private StringContent GetStringContent(DocumentAnalysisData data)
		{
			try
			{
				AnalysisJobRequest request = new AnalysisJobRequest();
				request.Name = data.DocumentName;
				request.Tasks = new List<JobTask>
				{
					new JobTask()
					{
						Type = "NotificationAnalysis",
						Parameters = new TaskParameter()
						{
							 NotificationType = MapAnalysisTypeAnaconda(data.AnalysisType)
						}

					}
				};
				request.SourceDocuments = new List<SourceDocument>();
				SourceDocument sourceDocument = new SourceDocument();
				sourceDocument.ClientDocumentId = Guid.NewGuid().ToString();
				sourceDocument.Format = "Zip";
				sourceDocument.Uri = data.AccessUrl;
				request.SourceDocuments.Add(sourceDocument);

				var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				return content;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error en GetStringContent: {ex.Message}");
				throw;
			}			
		}

		private string MapAnalysisTypeAnaconda(AnalysisTypes? analysisTypes)
		{
			switch(analysisTypes)
			{
				case AnalysisTypes.Undefined:
					return "Generic";
				case AnalysisTypes.Demand:
					return "Claim";
				default:
					return "Claim";
			}
		}

		public async Task<(HttpResponseMessage, string)> GetAnalysisResult(string analysisId, string clientDocumentId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(analysisId))
				{
					throw new ArgumentNullException(nameof(analysisId));
				}
				if (string.IsNullOrWhiteSpace(clientDocumentId))
				{
					throw new ArgumentNullException(nameof(clientDocumentId));
				}

				Log.Information($"Get analysis result with guid {analysisId} and clientDocumentId {clientDocumentId}");

				using HttpClient client = httpClientFactory.CreateClient();
				var resp = await Utils.GetRetryPolicy().ExecuteAsync(async () =>
				{
					client.DefaultRequestHeaders.Add("ocp-apim-subscription-key", configuration.AnalysisProvider.ApiKey);
					var url = $"{configuration.AnalysisProvider.UrlApiDocuments.Trim().TrimEnd('/')}/{clientDocumentId}";
					return await client.GetAsync(url);
				});

				var result = await resp.Content.ReadAsStringAsync();
				return (resp, result);
			}
			catch (Exception ex)
			{
				Log.Error($"Error al recuperar el resultado del análisis con id '{analysisId}' y clientDocumentId '{clientDocumentId}'", $"Error:{ex.Message}");
				throw;
			}
		}

	}
}
