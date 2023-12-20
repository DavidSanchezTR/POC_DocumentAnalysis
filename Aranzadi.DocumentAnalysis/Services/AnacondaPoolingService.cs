using Aranzadi.HttpPooling.Models;
using Aranzadi.HttpPooling.Services;
using Aranzadi.HttpPooling;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Aranzadi.DocumentAnalysis.Util;
using Aranzadi.DocumentAnalysis.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Azure.Core;
using System.Linq.Expressions;
using Aranzadi.DocumentAnalysis.Services.IServices;
using System.Configuration;

namespace Aranzadi.DocumentAnalysis.Services
{
	public class AnacondaPoolingService : ServiceBusPoolingService
	{
		private readonly DocumentAnalysisOptions configuration;
		private readonly PoolingConfiguration poolingConfiguration;
		private readonly IAnalysisProviderService analysisProviderService;
		private readonly IServiceProvider serviceProvider;
		private readonly ICreditsConsumptionClient creditsConsumptionClient;

		public AnacondaPoolingService(
			  DocumentAnalysisOptions configuration
			, PoolingConfiguration poolingConfiguration
			, IHttpClientFactory factory
			, IAnalysisProviderService analysisProviderService
			, IServiceProvider serviceProvider
			, ICreditsConsumptionClient creditsConsumptionClient) : base(poolingConfiguration, factory)
		{
			this.configuration = configuration;
			this.poolingConfiguration = poolingConfiguration;			
			this.analysisProviderService = analysisProviderService;
			this.serviceProvider = serviceProvider;
			this.creditsConsumptionClient = creditsConsumptionClient;
		}

		protected override string GetClientId()
		{
			return "Anaconda";
		}

		protected override string GetUrlAnalysisJob()
		{
			return "Anaconda";
		}

		protected override void PrepareRequest(HttpPoolingRequest polledRequest, HttpRequestMessage req)
		{
			base.PrepareRequest(polledRequest, req);
			//req.Headers.Add("ocp-apim-subscription-key", poolingConfiguration.AnalysisProvider.ApiKey);
			req.Headers.Add("ocp-apim-subscription-key", configuration.AnalysisProvider.ApiKey);
			req.Headers.Add("Accept", "application/json");
		}

		public override async Task<bool> Retry(HttpResponseMessage responseService, PooledRequestDTO polledRequest)
		{
			bool retry;
			switch (responseService.StatusCode)
			{
				case System.Net.HttpStatusCode.OK:
				case System.Net.HttpStatusCode.Created:
					{
						//Anaconda da una respuesta 200 de tipo PENDING, RUNNING, PARTIAL_SUCCESS
						var response = await responseService.Content.ReadAsStringAsync();
						AnalysisJobResponse analisisJobResponse = JsonConvert.DeserializeObject<AnalysisJobResponse>(response);

						string status = analisisJobResponse?.ExecutionStatus?.State?.ToLower() ?? "";
						switch (status)
						{
							case "pending":
							case "running":
							case "partial_success":
								retry = true;
								break;
							case "succeeded":
							case "failed":
								retry = false;
								break;
							default:
								Log.Warning(@$"ExecutionStatus.State no controlado: {status} del analisis con id {polledRequest.Request.ExternalIdentificator}");
								retry = true;
								break;
						}
					}
					break;
				case System.Net.HttpStatusCode.NoContent:
				case System.Net.HttpStatusCode.NotModified:
				case System.Net.HttpStatusCode.BadRequest:
				case System.Net.HttpStatusCode.Unauthorized:
				case System.Net.HttpStatusCode.NotFound:
					{
						retry = false;
					}
					break;
				case System.Net.HttpStatusCode.TooManyRequests:
				case System.Net.HttpStatusCode.InternalServerError:
					{
						retry = true;
					}
					break;
				default:
					Log.Warning(@$"StatusCode {responseService.StatusCode} no controlado del analisis con id {polledRequest.Request.ExternalIdentificator}");
					retry = false;
					break;
			}

			return retry;
		}

		public override async Task<bool> ProcessResponseAnalysis(HttpResponseMessage responseService, PooledRequestDTO polledRequest)
		{
			bool processResponse = false;
			switch (responseService.StatusCode)
			{
				case System.Net.HttpStatusCode.OK:
				case System.Net.HttpStatusCode.Created:
					{
						string response = await responseService.Content.ReadAsStringAsync();
						AnalysisJobResponse analisisJobResponse = JsonConvert.DeserializeObject<AnalysisJobResponse>(response);
						string status = analisisJobResponse?.ExecutionStatus?.State?.ToLower() ?? "";
						switch (status)
						{
							case "succeeded":
								{
									string analysisProviderId = polledRequest.Request.ExternalIdentificator;
									string clientDocumentId = analisisJobResponse?.SourceDocuments.FirstOrDefault().ClientDocumentId;

									var result = await analysisProviderService.GetAnalysisResult(analysisProviderId, clientDocumentId);
									if (result.Item1.IsSuccessStatusCode)
									{
										DocumentAnalysisData data = new DocumentAnalysisData();
										data.Id = new Guid(polledRequest.Request.ExternalIdentificator);
										data.AnalysisProviderResponse = status;
										data.Status = AnalysisStatus.Done;
										data.Analysis = result.Item2;
										processResponse = await UpdateAnalysis(data);
									}
									else
									{
										await SetRetry(polledRequest);
									}
								}
								break;
							case "failed":
								{
									DocumentAnalysisData data = new DocumentAnalysisData();
									data.Id = new Guid(polledRequest.Request.ExternalIdentificator);
									data.AnalysisProviderResponse = status;
									data.Status = AnalysisStatus.Error;
									data.Analysis = null;
									processResponse = await UpdateAnalysis(data);
								}
								break;
							default:
								{
									Log.Warning(@$"ExecutionStatus.State no controlado: {status} del analisis con id {polledRequest.Request.ExternalIdentificator}");
									throw new KeyNotFoundException(@$"ExecutionStatus.State no controlado: {status} del analisis con id {polledRequest.Request.ExternalIdentificator}");
								}
								break;
						}
					}
					break;
				case System.Net.HttpStatusCode.NoContent:
				case System.Net.HttpStatusCode.NotModified:
				case System.Net.HttpStatusCode.BadRequest:
				case System.Net.HttpStatusCode.Unauthorized:
				case System.Net.HttpStatusCode.NotFound:
				case System.Net.HttpStatusCode.TooManyRequests:
				case System.Net.HttpStatusCode.InternalServerError:
				default:
					{
						DocumentAnalysisData data = new DocumentAnalysisData();
						data.Id = new Guid(polledRequest.Request.ExternalIdentificator);
						data.AnalysisProviderResponse = $"Failed. StatusCode {responseService.StatusCode}. RetriesCount: {polledRequest.NIntento + 1}";
						data.Status = AnalysisStatus.Error;
						data.Analysis = null;
						processResponse = await UpdateAnalysis(data);
					}
					break;
			}

			return processResponse;
		}

		public override async Task<bool> CancelAnalysis(HttpResponseMessage responseService, PooledRequestDTO polledRequest)
		{
			DocumentAnalysisData data = new DocumentAnalysisData();
			data.Id = new Guid(polledRequest.Request.ExternalIdentificator);
			data.AnalysisProviderResponse = $"Cancelled. StatusCode {responseService.StatusCode}. RetriesCount: {polledRequest.NIntento + 1}";
			data.Status = AnalysisStatus.Error;
			data.Analysis = null;
			return await UpdateAnalysis(data);
		}

		private async Task<bool> UpdateAnalysis(DocumentAnalysisData data)
		{
			bool update = false;
			try
			{
				using (IServiceScope scope = serviceProvider.CreateScope())
				{
					IDocumentAnalysisRepository documentAnalysisRepository =
						scope.ServiceProvider.GetRequiredService<IDocumentAnalysisRepository>();

					DocumentAnalysisData analysis = await documentAnalysisRepository.GetAnalysisDataAsync(data.Id.ToString());

					if (analysis != null)
					{
						analysis.AnalysisProviderResponse = data.AnalysisProviderResponse;
						analysis.Status = data.Status;
						analysis.Analysis = data.Analysis;

						if (analysis.Status == AnalysisStatus.Done)
						{
							if (((creditsConsumptionClient.CompleteReservation(analysis.TenantCreditID, analysis.ReservationCreditID))).Result.Code != OperationResult.ResultCode.Success)
							{
								Log.Error($"Error in analysis job with guid {data.Id}, message: Error in credits consumption Service");
								//throw new Exception();
								data.Status = AnalysisStatus.DoneWithErrors;
								await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
								return false;
							}
						}

						update = await documentAnalysisRepository.UpdateAnalysisDataAsync(analysis) == 1;

						string message = $"Analysis job with guid {data.Id} finalized with status {data.AnalysisProviderResponse}";
						if (data.Status == AnalysisStatus.Error)
						{
							Log.Error(message);
						}
						else
						{
							Log.Information(message);
						}
					}
					else
					{
						Log.Warning($"Analysis not found with id {data.Id.ToString()}");
						update = false;
					}
				}
				return update;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Exception in UpdateAnalysis => {ex.Message}");
				throw;
			}
		}				

	}
}
