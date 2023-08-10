using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Util;
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

namespace Aranzadi.DocumentAnalysis.Services
{
	public class QueuedHostedService : BackgroundService
	{
		public const string MESSAGE_SOURCE_FUSION = "Fusion";
		public const string MESSAGE_TYPE_DOCUMENT_ANALYSIS = "DocumentAnalysis";
		private const string DAV_VAL_TOKEN_HEADER = "fusion_session_id";

		static string objLock = "";
		static IConsumer consumer = null;
		public static AnalysisContext Contexto = null;

		private readonly DocumentAnalysisOptions configuration;
		private readonly IHttpClientFactory httpClientFactory;
		private readonly IAnalysisProviderService analysisProviderService;
		private readonly ILogAnalysis logAnalysisService;

		public IServiceProvider serviceProvider { get; }

		public QueuedHostedService(IServiceProvider serviceProvider
			, DocumentAnalysisOptions configuration
			, IHttpClientFactory httpClientFactory
			, IAnalysisProviderService analysisProviderService
			, ILogAnalysis logAnalysisService
			)
		{
			this.serviceProvider = serviceProvider;
			this.configuration = configuration;
			this.httpClientFactory = httpClientFactory;
			this.analysisProviderService = analysisProviderService;
			this.logAnalysisService = logAnalysisService;
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
					AnalysisProviderResponse = "Pending"
				};
				using (IServiceScope scope = serviceProvider.CreateScope())
				{
					IDocumentAnalysisRepository documentAnalysisRepository =
						scope.ServiceProvider.GetRequiredService<IDocumentAnalysisRepository>();

					await documentAnalysisRepository.AddAnalysisDataAsync(data);

					DocumentAnalysisResult? resultAnalysis = null;
					if (configuration.CheckIfExistsHashFileInCosmos)
					{		
						Stream stream = null;
						try
						{
							stream = await GetStreamFromSasToken(data.AccessUrl);							
						}
						catch (Exception ex)
						{
							data.Status = AnalysisStatus.NotFound;
							await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
							throw;
						}

						try
						{
							data.Sha256 = await GetHashFromFile(stream);
						}
						catch (Exception)
						{
							data.Status = AnalysisStatus.Error;
							await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
							throw;
						}

						resultAnalysis = await documentAnalysisRepository.GetAnalysisDoneAsync(data.Sha256);
					}

					if (resultAnalysis != null)
					{
                        Log.Information($"The document is already analized with status: {data.Status}");
                        data.Status = resultAnalysis.Status;
						data.Analysis = resultAnalysis.Analysis;
					}
					else
					{
						var result = await analysisProviderService.SendAnalysisJob(data);
						if (result.Item1.IsSuccessStatusCode)
						{
							// 200 OK, 201 Created
							if (result.Item2?.Guid == null)
							{
								throw new ArgumentNullException($"AnalysisProviderId is null");
							}
							data.AnalysisProviderId = new Guid(result.Item2.Guid);
							//Estado del job. Valores validos: 'PENDING', 'RUNNING', 'SUCEEDED','PARTIAL_SUCCESS' y 'FAILED'
							data.AnalysisProviderResponse = result.Item2.ExecutionStatus?.State;
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
							Log.Information($"Error in analysis job with guid {data.Id}, message: {message}");
                        }
					}

					//////////////////////////////////
					//TODO: Rellenar temporalmente la respuesta del analysis aqui en modo de pruebas y marcar como Done
					string json = JsonConvert.SerializeObject(Get_DocumentAnalysisDataResultContent(data));
					data.Analysis = json;
					data.Status = AnalysisStatus.Done;
					/////////////////////////////////

					await documentAnalysisRepository.UpdateAnalysisDataAsync(data);
				}

				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
				throw;
			}
		}

		public async Task<string> GetHashFromFile(Stream stream)
		{
			try
			{
				using (SHA256 crypto = SHA256.Create())
				{
					stream.Seek(0, SeekOrigin.Begin);
					var hash = await crypto.ComputeHashAsync(stream);
					var hashString = Convert.ToBase64String(hash);

					return hashString;
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Exception to get Hash from file stream => {ex.Message}");
				throw;
			}
		}

		private async Task<Stream> GetStreamFromSasToken(string url)
		{
			try
			{
				using HttpClient httpCli = httpClientFactory.CreateClient();
				var resp = await Utils.GetRetryPolicy().ExecuteAsync(async () =>
				{
					Log.Information($"Request file from SASToken: {url}");
					return await httpCli.GetAsync(url);
				});
				resp.EnsureSuccessStatusCode();
				var stream = await resp.Content.ReadAsStreamAsync();
				return stream;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Exception to get SasToken from {url} => {ex.Message}");
				throw;
			}
		}		

		private DocumentAnalysisDataResultContent Get_DocumentAnalysisDataResultContent(DocumentAnalysisData data)
		{
			DocumentAnalysisDataResultContent content = new DocumentAnalysisDataResultContent();
			content.Court = new DocumentAnalysisDataResultCourt()
			{
				City = "ciudad sample",
				Jurisdiction = "jurisdiccion sample",
				Name = "nombre sample " + data.DocumentName,
				Number = "numero sample",
				CourtType = "tribunal sample"
			};
			content.Review = new DocumentAnalysisDataResultReview()
			{
				Cause = new string[] { "cause 1", "cause 2" },
				Review = "review sample " + data.DocumentName
			};
			content.Proceeding = new DocumentAnalysisDataResultProceeding()
			{
				NIG = "NIG sample",
				MilestonesNumber = "numero autos sample",
				ProceedingType = "procedimiento sample",
				ProceedingSubtype = "subprocedimiento sample",
				Topic = "materia sample",
				PrincipalQuoted = "1111.11"
            };

			var lista = new List<DocumentAnalysisDataResultProceedingParts>
			{
				new DocumentAnalysisDataResultProceedingParts()
				{
					Lawyers = "letrado sample",
					Source = "naturaleza sample",
					Name = "nombre sample " + data.DocumentName,
					Attorney = "procurador sample",
					PartType = "tipo parte sample",
					AppealPartType = "tipo parte recurso sample"

				},
				new DocumentAnalysisDataResultProceedingParts()
				{
					Lawyers = "letrado sample 2",
					Source = "naturaleza sample 2",
					Name = "nombre sample 2",
					Attorney = "procurador sample 2",
					PartType = "tipo parte sample 2",
					AppealPartType = "tipo parte recurso sample 2"

				}
			};
			content.Proceeding.Parts = lista.ToArray();
			content.Proceeding.InitialProceeding = new DocumentAnalysisDataResultProceedingInitialProceeding()
			{
				Court = "juzgado sample " + data.DocumentName,
				MilestonesNumber = "numero autos"
			};

			var requerimientos = new List<DocumentAnalysisDataResultNotification>();
			requerimientos.Add(new DocumentAnalysisDataResultNotification()
			{
				NotificationDate = DateTime.UtcNow.AddDays(5).ToString("O"),
				Term = "5",
				Notification = "requerimiento sample 1 " + data.DocumentName,

			});
			requerimientos.Add(new DocumentAnalysisDataResultNotification()
			{
				NotificationDate = DateTime.UtcNow.AddDays(10).ToString("O"),
				Term = "10",
				Notification = "requerimiento sample 2 " + data.DocumentName,

			});

			var recursos = new List<DocumentAnalysisDataResultAppeal>();
			recursos.Add(new DocumentAnalysisDataResultAppeal()
			{
				Term = "6",
				Appeal = "Recurso sample 1 " + data.DocumentName,
			});
			recursos.Add(new DocumentAnalysisDataResultAppeal()
			{
				Term = "20",
				Appeal = "Recurso sample 2 " + data.DocumentName,
			});

			content.CourtDecision = new DocumentAnalysisDataCourtDecision()
			{
				Amount = "",
				CommunicationDate = DateTime.UtcNow.AddDays(-100).ToString("O"),
				CourtDecisionDate = DateTime.UtcNow.ToString("O"),
				Milestone = "hito sample " + data.DocumentName,
				CourtDecisionNumber = "num resolucion sample",
				WrittenSummary = "resumen",
				Notifications = requerimientos.ToArray(),
				Appeal = recursos.ToArray(),
				SitingDate = DateTime.UtcNow.ToString("O"),
			};

			return content;
		}


	}
}
