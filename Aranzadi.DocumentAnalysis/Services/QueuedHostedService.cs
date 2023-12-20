using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Models.CreditReservations;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Util;
using Aranzadi.HttpPooling.Interfaces;
using Aranzadi.HttpPooling.Models;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Policy;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using ThomsonReuters.BackgroundOperations.Messaging.Models;
using Aranzadi.DocumentAnalysis.Models.CreditConsumption;
using Serilog.Context;

namespace Aranzadi.DocumentAnalysis.Services
{
	public class QueuedHostedService : BackgroundService
	{
		private const string ORIGIN = "Document Analysis";
		public const string MESSAGE_SOURCE_FUSION = "Fusion";
		public const string MESSAGE_TYPE_DOCUMENT_ANALYSIS = "DocumentAnalysis";
		private const string DAV_VAL_TOKEN_HEADER = "fusion_session_id";
		private const string CREDIT_TEMPLATE_ID = "analisisdocumental";

		static string objLock = "";
		static IConsumer consumer = null;
		public static AnalysisContext Contexto = null;

		private readonly DocumentAnalysisOptions configuration;
		private readonly IHttpClientFactory httpClientFactory;
		private readonly IAnalysisProviderService analysisProviderService;
		private readonly ILogAnalysis logAnalysisService;
		private readonly IHttpPoolingServices serviceBusPoolingService;
		private readonly ICreditsConsumptionClient creditsConsumptionClient;

		public IServiceProvider serviceProvider { get; }

		public QueuedHostedService(IServiceProvider serviceProvider
			, DocumentAnalysisOptions configuration
			, IHttpClientFactory httpClientFactory
			, IAnalysisProviderService analysisProviderService
			, ILogAnalysis logAnalysisService
			, IHttpPoolingServices serviceBusPoolingService
			, ICreditsConsumptionClient creditsConsumptionClient
			)
		{
			this.serviceProvider = serviceProvider;
			this.configuration = configuration;
			this.httpClientFactory = httpClientFactory;
			this.analysisProviderService = analysisProviderService;
			this.logAnalysisService = logAnalysisService;
			this.serviceBusPoolingService = serviceBusPoolingService;
			this.creditsConsumptionClient = creditsConsumptionClient;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				Log.Information($"{nameof(QueuedHostedService)} running at: {DateTimeOffset.Now}");

				if (consumer == null)
				{
					lock (objLock)
					{

						var conf = new MessagingConfiguration();
						conf.ServicesBusConnectionString = configuration.Messaging.Endpoint;
						conf.ServicesBusCola = configuration.Messaging.Queue;
						conf.Source = MESSAGE_SOURCE_FUSION;
						conf.Type = MESSAGE_TYPE_DOCUMENT_ANALYSIS;

						var factory = new AnalisisDocumentosDefaultFactory(conf, logAnalysisService);

						if (consumer == null)
						{
							consumer = factory.GetConsumer();
							consumer.StartProcess(Process);
						}

					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
			}
		}

		public async Task<bool> Process(AnalysisContext context, DocumentAnalysisRequest request)
		{
			try
			{
				return await ProcessMessage(context, request);
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> ProcessMessage(AnalysisContext context, DocumentAnalysisRequest request)
		{
			try
			{
				LogContext.PushProperty("Origin", ORIGIN);

				var data = new DocumentAnalysisData
				{
					Id = new Guid(request.Guid),
					App = context.App,
					TenantId = context.Tenant.ToString(),
					UserId = context.Owner,
					Analysis = null,
					Status = AnalysisStatus.Pending,
					AnalysisDate = DateTimeOffset.Now,
					CreateDate = DateTimeOffset.Now,
					Source = Source.LaLey,
					DocumentName = request.Name,
					AccessUrl = request.Path,
					Sha256 = null,
					AnalysisProviderId = null,
					AnalysisProviderResponse = "Pending",
					TenantCreditID = null,
					ReservationCreditID = null,
					AnalysisType = request.AnalysisType
				};

				LogContext.PushProperty("DocumentId", data.Id);
				LogContext.PushProperty("LawfirmId", data.TenantId);
				LogContext.PushProperty("UserId", data.UserId);
				LogContext.PushProperty("DocumentName", data.DocumentName);

				using (IServiceScope scope = serviceProvider.CreateScope())
				{
					IDocumentAnalysisRepository documentAnalysisRepository =
						scope.ServiceProvider.GetRequiredService<IDocumentAnalysisRepository>();

					await documentAnalysisRepository.AddAnalysisDataAsync(data);

					OperationResult<TenantCreditReservation> reservation = new OperationResult<TenantCreditReservation>
					{
						Code = OperationResult.ResultCode.Success
					};
					OperationResult<CreditResponse> freeReservation = new OperationResult<CreditResponse>
					{
						Code = OperationResult.ResultCode.Success
					};					

					reservation = creditsConsumptionClient.CreateReservation(new NewTenantCreditReservation
					{
						CreditTemplateID = CREDIT_TEMPLATE_ID,
						TenantID = context.Tenant,
						UserID = context.Owner
					}).Result;
					if (reservation.Code != OperationResult.ResultCode.Success)
					{
						data.Status = AnalysisStatus.Error;
						Log.Error($"Error in analysis job with guid {data.Id}, message: Error in credits reservation,{reservation.Detail}");
						await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
						return false;
					}
					else
					{
						data.TenantCreditID = reservation.Result.TenantCreditID;
						data.ReservationCreditID = reservation.Result.ID;
						await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
					}

					var result = await analysisProviderService.SendAnalysisJob(data);
					if (result.Item1.IsSuccessStatusCode)
					{
						// 200 OK, 201 Created
						if (result.Item2?.Guid == null)
						{
							throw new ArgumentNullException($"AnalysisProviderId is null");
						}
						data.AnalysisProviderId = new Guid(result.Item2.Guid);
						data.AnalysisProviderResponse = result.Item2.ExecutionStatus?.State;
						await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
						await SendMessageToPoolingQueue(data);

						return true;
					}
					else
					{
						// ERROR
						data.Status = AnalysisStatus.Error;
						List<string> listMessages = new List<string>();
						if (result.Item1 != null)
						{
							listMessages.Add(result.Item1.StatusCode.ToString());
						}
						if (result.Item2 != null)
						{
							listMessages.Add(result.Item2.ExecutionStatus?.State ?? "");
							if (result.Item2.ExecutionStatus?.Errors != null)
							{
								foreach (var err in result.Item2.ExecutionStatus.Errors)
								{
									listMessages.Add($"{err.Message} {err.Reason} {err.Link}");
								}
							}
						}
						var message = string.Join(Environment.NewLine, listMessages.Where(o => !string.IsNullOrWhiteSpace(o)));
						data.AnalysisProviderResponse = message;
						await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
						Log.Error($"Error in analysis job with guid {data.Id}, message: {message}");

						return false;
					}

				}

			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
				throw;
			}
		}

		public async Task SendMessageToPoolingQueue(DocumentAnalysisData data)
		{
			try
			{
				if (data == null)
				{
					throw new ArgumentNullException("Data is null");
				}
				if (data.Id == Guid.Empty)
				{
					throw new ArgumentNullException("Data.Id is null or empty");
				}
				if (string.IsNullOrWhiteSpace(data?.AnalysisProviderId?.ToString()))
				{
					throw new ArgumentNullException("Data.AnalysisProviderId is null or empty");
				}

				HttpPoolingRequest poolingRequest = new HttpPoolingRequest();
				//poolingRequest.Id = data.Id.ToString();
				poolingRequest.ExternalIdentificator = data.Id.ToString();
				poolingRequest.Url = $"{configuration.AnalysisProvider.UrlApiJobs.Trim().TrimEnd('/')}/{data.AnalysisProviderId.ToString()}";
				await serviceBusPoolingService.AddRequest(poolingRequest);
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Exception to send message to pooling queue => {ex.Message}");
				throw;
			}

		}

	}
}
