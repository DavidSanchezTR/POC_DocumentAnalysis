using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
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
				Log.Error(ex, $"Error:{ex.Message}");
				throw;
			}
		}

		private StringContent GetStringContent(DocumentAnalysisData data)
		{
			try
			{
				AnalysisJobRequest request = new AnalysisJobRequest();
				request.Name = data.DocumentName;
				request.SourceDocuments = new List<SourceDocument>();
				SourceDocument sourceDocument = new SourceDocument();
				sourceDocument.ClientDocumentId = data.Id.ToString();
				sourceDocument.AzureBlobUri = data.AccessUrl;
				request.SourceDocuments.Add(sourceDocument);

				var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				return content;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error:{ex.Message}");
				throw;
			}
			
		}

	}
}
