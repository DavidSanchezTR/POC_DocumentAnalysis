
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;
using ThomsonReuters.BackgroundOperations.Messaging.Models;

namespace Aranzadi.DocumentAnalysis.Services
{
	public class QueuedHostedService : BackgroundService
	{
		static string objLock = "";
		static IConsumer consumer = null;
		public static AnalysisContext Contexto = null;



		private readonly ILogger<QueuedHostedService> _logger;
		private readonly DocumentAnalysisOptions configuration;
		private IDocumentAnalysisRepository documentAnalysisRepository;
		private DocumentAnalysisDbContext documentAnalysisDbContext;

		public IServiceProvider serviceProvider { get; }

		public QueuedHostedService(ILogger<QueuedHostedService> logger, IServiceProvider serviceProvider, DocumentAnalysisOptions configuration)
		{
			_logger = logger;
			this.serviceProvider = serviceProvider;
			this.configuration = configuration;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			_logger.LogInformation($"{nameof(QueuedHostedService)} running at: {DateTimeOffset.Now}");

			if (consumer == null)
			{
				lock (objLock)
				{

					var conf = new MessagingConfiguration();
					conf.ServicesBusConnectionString = configuration.ServiceBus.ConnectionString;
					conf.ServicesBusCola = configuration.ServiceBus.Queue;
					conf.URLServicioAnalisisDoc = new Uri("https://localhost:44323/api/FalsoApi");
					conf.URLOrquestador = new Uri("https://noseusaSepuedeBorrar.com");
					conf.Source = "Fusion";
					conf.Type = "AnalisisDocumentos";

					var factory = new AnalisisDocumentosDefaultFactory(conf);

					if (consumer == null)
					{
						consumer = factory.GetConsumer();
						consumer.StartProcess(TratarMensaje);
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
			var data = new DocumentAnalysisData
			{
				App = "Infolex",
				DocumentName = "PruebaAA3.pdf",				
				Analysis = "Esto es un análisis",
				AccessUrl = "www.prueba.com",
				Sha256 = "HasCode",
				Status = 1,
				TenantId = "122",
				UserId = "22",
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = "La Ley"
			};
			using (IServiceScope scope = serviceProvider.CreateScope())
			{
				IDocumentAnalysisRepository documentAnalysisRepository =
					scope.ServiceProvider.GetRequiredService<IDocumentAnalysisRepository>();

				await documentAnalysisRepository.AddAnalysisDataAsync(data);
			}

			await Task.Delay(0);
			return true;
		}

	}
}
