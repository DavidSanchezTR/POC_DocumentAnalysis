using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Aranzadi.DocumentAnalysis.Services
{
	public class QueuedHostedService : BackgroundService
	{
		public const string MESSAGE_SOURCE_FUSION = "Fusion";
		public const string MESSAGE_TYPE_DOCUMENT_ANALYSIS = "DocumentAnalysis";

		static string objLock = "";
		static IConsumer consumer = null;
		public static AnalysisContext Contexto = null;

		private readonly ILogger<QueuedHostedService> _logger;
		private readonly DocumentAnalysisOptions configuration;
		private readonly TelemetryClient telemetryClient;

		public IServiceProvider serviceProvider { get; }

		public QueuedHostedService(ILogger<QueuedHostedService> logger
			, IServiceProvider serviceProvider
			, DocumentAnalysisOptions configuration
			, TelemetryClient telemetryClient)
		{
			_logger = logger;
			this.serviceProvider = serviceProvider;
			this.configuration = configuration;
			this.telemetryClient = telemetryClient;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				_logger.LogInformation($"{nameof(QueuedHostedService)} running at: {DateTimeOffset.Now}");

				if (consumer == null)
				{
					lock (objLock)
					{

						var conf = new MessagingConfiguration();
						conf.ServicesBusConnectionString = configuration.ServiceBus.ConnectionString;
						conf.ServicesBusCola = configuration.ServiceBus.Queue;
						conf.Source = MESSAGE_SOURCE_FUSION;
						conf.Type = MESSAGE_TYPE_DOCUMENT_ANALYSIS;

						var factory = new AnalisisDocumentosDefaultFactory(conf);

						if (consumer == null)
						{
							consumer = factory.GetConsumer();
							consumer.StartProcess(ProcessMessage);
						}

					}
				}
			}
			catch (Exception ex)
			{
				telemetryClient.TrackException(ex);
				await telemetryClient.FlushAsync(stoppingToken);
				throw;
			}
		}


		private async Task<bool> ProcessMessage(AnalysisContext context, DocumentAnalysisRequest request)
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
					Sha256 = "" // Calcular el Hash aqui en el paquete
				};
				using (IServiceScope scope = serviceProvider.CreateScope())
				{
					IDocumentAnalysisRepository documentAnalysisRepository =
						scope.ServiceProvider.GetRequiredService<IDocumentAnalysisRepository>();

					// Obtener el codigo Hash del fichero. EN PRUEBAS
					try
					{
						using (SHA256 crypto = SHA256.Create())
						{
							var req = System.Net.WebRequest.Create(data.AccessUrl);
							Stream stream = req.GetResponse().GetResponseStream();
							//using (var fileStream = new FileStream(req.GetResponse().GetResponseStream(), FileMode.Open, FileAccess.Read))
							{
								var hash = crypto.ComputeHash(stream);
								var hashString = Convert.ToBase64String(hash);

								data.Sha256 = hashString;
							}
						}
					}
					catch (Exception ex)
					{
						telemetryClient.TrackException(ex);
						telemetryClient.Flush();
						throw;
					}
					////////////////////

					DocumentAnalysisResult? resultAnalysis = null;
					if (configuration.CheckIfExistsHashFileInCosmos)
					{
						resultAnalysis = await documentAnalysisRepository.GetAnalysisDoneAsync(data.Sha256);
					}

					if (resultAnalysis != null)
					{
						data.Status = resultAnalysis.Status;
						data.Analysis = resultAnalysis.Analysis;
					}
					else
					{                        
						//TODO: Rellenar la respuesta del analysis aqui en modo de pruebas y marcar como Done
						string json = JsonConvert.SerializeObject(Get_DocumentAnalysisDataResultContent(data));
						data.Analysis = json;
						data.Status = AnalysisStatus.Done;
						////////


						
						
					}
					await documentAnalysisRepository.AddAnalysisDataAsync(data);

					/*TODO
					 *        
						Crear petición de análisis al proveedor de análisis.
						Modificar el registro de CosmosDB con el resultado del análisis.
						Devolver resultado.                  
					 */

				}

				await Task.Delay(0);
				return true;
			}
			catch (Exception ex)
			{
				telemetryClient.TrackException(ex);
				telemetryClient.Flush();
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
			};

			return content;
		}


	}
}
