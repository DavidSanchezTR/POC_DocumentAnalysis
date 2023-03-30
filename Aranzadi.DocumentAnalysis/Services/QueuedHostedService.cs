
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using ThomsonReuters.BackgroundOperations.Messaging.Models;

namespace Aranzadi.DocumentAnalysis.Services
{
	public class QueuedHostedService : BackgroundService
	{

		static string conString = "Endpoint=sb://uksouth-iflx-blue-orch-dev-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OzIgYihRXFO389WSagVM4MidAetFjoy7G72kgQxtOwg=";
		public static IClient client = null;
		static IConsumer consumer = null;
		//public static ProtoSAD bds = null;
		public static AnalysisContext Contexto = null;



		private readonly ILogger<QueuedHostedService> _logger;
		private IDocumentAnalysisRepository documentAnalysisRepository;
		private DocumentAnalysisDbContext documentAnalysisDbContext;

		public IServiceProvider serviceProvider { get; }

		public QueuedHostedService(ILogger<QueuedHostedService> logger, IServiceProvider serviceProvider)
		{
			_logger = logger;
			this.serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			_logger.LogInformation($"{nameof(QueuedHostedService)} running at: {DateTimeOffset.Now}");
			//return ProcessTaskQueueAsync(stoppingToken);


			if (client == null || consumer == null)// || bds == null)
			{
				lock (conString)
				{

					var conf = new MessagingConfiguration();
					conf.ServicesBusConnectionString = conString;
					conf.ServicesBusCola = "documentanalysistest"; // MessageDestinations.BackgroundOrchestrator;
					conf.URLServicioAnalisisDoc = new Uri("https://localhost:44323/api/FalsoApi");
					conf.URLOrquestador = new Uri("https://noseusaSepuedeBorrar.com");
					conf.Source = "Fusion";
					conf.Type = "AnalisisDocumentos";

					var factory = new AnalisisDocumentosDefaultFactory(conf);


					if (client == null)
					{
						client = factory.GetClient();
					}
					if (consumer == null)
					{
						consumer = factory.GetConsumer();
						consumer.StartProcess(TratarMensaje);
					}
					//if (bds == null)
					{
						//bds = new ProtoSAD();

					}
					Contexto = new AnalysisContext()
					{
						Aplication = "Infolex",
						Tenant = "Mª Carmen S.L.",
						Owner = "MªCarmen"
					};
				}
			}



		}


		private async Task<bool> TratarMensaje(AnalysisContext context, DocumentAnalysisRequest request)
		{
			//lock (bds)
			//{
			//	bds.AddRequest(context, request);
			//}

			var data = new DocumentAnalysisData
			{
				App = "Infolex",
				DocumentName = "PruebaAA.pdf",
				NewGuid = Guid.NewGuid(),
				Analisis = "Esto es un análisis",
				AccessUrl = "www.prueba.com",
				Sha256 = "HasCode",
				Estado = "Pendiente",
				TenantId = 122,
				UserId = 22,
				FechaAnalisis = DateTimeOffset.Now,
				FechaCreacion = DateTimeOffset.Now,
				Origen = "La Ley"
			};
			using (IServiceScope scope = serviceProvider.CreateScope())
			{
				IDocumentAnalysisRepository documentAnalysisRepository =
					scope.ServiceProvider.GetRequiredService<IDocumentAnalysisRepository>();

				await documentAnalysisRepository.AddAnalysisDataAsync(data);
			}


			//var con = this.serviceProvider.GetService<DocumentAnalysisDbContext>();

			//var a = serviceProvider.GetService<IDocumentAnalysisRepository>();
			//await a.AddAnalysisDataAsync(data);


			//var a = new DocumentAnalysisRepository(con).AddAnalysisDataAsync(data);

			//await documentAnalysisRepository.AddAnalysisDataAsync(data);
			//await new DocumentAnalysisRepository(documentAnalysisDbContext).AddAnalysisDataAsync(data);

			await Task.Delay(0);
			return true;
		}


		private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
		{

			//var connectionSettings = new ConnectionSettings(options.Value.Messaging.Endpoint);
			//var receiver = messageFactory.GetReceiver(connectionSettings, logger);

			//await receiver.OnReceiveMessage<object, IMessageFacadeService>(MessageDestinations.BackgroundOrchestrator, async (msg, ct, facade) =>
			//{
			//	try
			//	{
			//		await facade.SaveMessageAsOperation(msg);
			//		await facade.SendOperation(connectionSettings, msg.Message, ct);
			//	}
			//	catch
			//	{
			//		await facade.MarkAsFailed(msg.Message);
			//	}
			//	finally
			//	{
			//		await msg.Complete();
			//	}
			//}, new MessageReceptionOptions { LimitMaxConcurrentCalls = true, MaxConcurrentCalls = 10 });

			//await receiver.OnReceiveMessage<WorkDone<object>, IMessageFacadeService>(MessageDestinations.WorkDone, async (msg, ct, facade) =>
			//{
			//	await facade.ProcessWorkDone(msg.Message);
			//	await msg.Complete();
			//});


			/////////////////////////////////////////////////
			/////////////////////////////////////////////////


			//while (!stoppingToken.IsCancellationRequested)
			//{
			//	try
			//	{
			//		using var scope = serviceProvider.CreateScope();
			//		Func<CancellationToken, IServiceScope, ValueTask>? workItem =
			//			await _taskQueue.DequeueAsync(stoppingToken);

			//		await workItem(stoppingToken, scope);
			//	}
			//	catch (OperationCanceledException)
			//	{
			//		// Prevent throwing if stoppingToken was signaled
			//	}
			//	catch (Exception ex)
			//	{
			//		_logger.LogError(ex, "Error occurred executing task work item.");
			//	}
			//}
		}


	}
}
