
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Enums;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.DTO.Response;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ThomsonReuters.BackgroundOperations.Messaging.Models;

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


		private async Task<bool> ProcessMessage(AnalysisContext context, DTO.Request.DocumentAnalysisRequest request)
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

					// Consultar si existe el analysis del documento por Hash y si está finalizado y disponible
					var resultAnalysis = await documentAnalysisRepository.GetAnalysisDoneAsync(data.Sha256);
					if (resultAnalysis != null)
					{
						data.Status = resultAnalysis.Status;
						data.Analysis = resultAnalysis.Analysis;
					}
					else
					{
						
						//TODO: Rellenar la respuesta del analysis aqui en modo de pruebas y marcar como Done
						string json = JsonConvert.SerializeObject(Get_DocumentAnalysisDataResultContent());
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


		private DocumentAnalysisDataResultContent Get_DocumentAnalysisDataResultContent()
		{
			DocumentAnalysisDataResultContent content = new DocumentAnalysisDataResultContent();
			content.juzgado = new DocumentAnalysisDataResultJudgement()
			{
				ciudad = "ciudad sample",
				jurisdiccion = "jurisdiccion sample",
				nombre = "nombre sample",
				numero = "numero sample",
				tipotribunal = "tribunal sample"
			};
			content.review = new DocumentAnalysisDataResultReview()
			{
				cause = new string[] { "cause 1", "cause 2" },
				review = "review sample"
			};
			content.procedimiento = new DocumentAnalysisDataResultProcedure()
			{
				NIG = "NIG sample",
				numeroautos = "numero autos sample",
				tipoprocedimiento = "procedimiento sample",
				subtipoprocedimiento = "subprocedimiento sample",
			};

			var lista = new List<DocumentAnalysisDataResultProcedureParts>
		{
			new DocumentAnalysisDataResultProcedureParts()
			{
				letrados = "letrado sample",
				naturaleza = "naturaleza sample",
				nombre = "nombre sample",
				procurador = "procurador sample",
				tipoparte = "tipo parte sample",
				tipoparterecurso = "tipo parte recurso sample"

			},
			new DocumentAnalysisDataResultProcedureParts()
			{
				letrados = "letrado sample 2",
				naturaleza = "naturaleza sample 2",
				nombre = "nombre sample 2",
				procurador = "procurador sample 2",
				tipoparte = "tipo parte sample 2",
				tipoparterecurso = "tipo parte recurso sample 2"

			}
		};
			content.procedimiento.partes = lista.ToArray();
			content.procedimiento.procedimientoinicial = new DocumentAnalysisDataResultProcedureInitialProcedure()
			{
				juzgado = "juzgado sample",
				numeroautos = "numero autos"
			};

			content.resolucion = new DocumentAnalysisDataResolution()
			{
				cuantia = "",
				fechanotificacion = DateTime.Today.AddDays(-100).ToString(),
				fecharesolucion = DateTime.Today.ToString(),
				hito = "hito sample",
				numeroresolucion = "num resolucion sample",
				resumenescrito = "resumen",

			};
			return content;
		}


	}
}
