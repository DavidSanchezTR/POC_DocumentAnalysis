using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using ThomsonReuters.BackgroundOperations.Messaging.Models;
using ThomsonReuters.BackgroundOperations.Messaging;
using Azure.Messaging.ServiceBus;
using Azure.Core;
using System.Text.Json;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Aranzadi.DocumentAnalysis.DTO;
using Polly;

namespace Aranzadi.DocumentAnalysis.Controllers
{
	[ApiController]
	[Route("api/test")]
	public class TestController : ControllerBase
	{

		private readonly IDocumentAnalysisRepository documentAnalysisRepository;
        private readonly IConfiguration config;
        private readonly ILogger<TestController> _logger;
		private readonly DocumentAnalysisOptions configuration;

		public TestController(ILogger<TestController> logger
			, DocumentAnalysisOptions configuration
			, IDocumentAnalysisRepository documentAnalysisRepository)
		{
			_logger = logger;
			this.configuration = configuration;
			this.documentAnalysisRepository = documentAnalysisRepository;
        }

		[HttpGet]
		[Route("SavedocumentAnalysisTest")]
		public IEnumerable<Test> Save()
		{
			string[] Summaries = new[]
				{
				"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
				};
						
			return Enumerable.Range(1, 5).Select(index => new Test
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}

		[HttpGet]
		[Route("SendMessageToServiceBusTest")]
		public void SendMessage()
		{
			enviarMensajes();
		}

		private async void enviarMensajes()
		{
			var conf = new MessagingConfiguration();
			conf.ServicesBusConnectionString = configuration.ServiceBus.ConnectionString;
			conf.ServicesBusCola = configuration.ServiceBus.Queue;
			conf.URLServicioAnalisisDoc = new Uri("https://localhost:44323/api/FalsoApi");
			conf.URLOrquestador = new Uri("https://noseusaSepuedeBorrar.com");
			conf.Source = "Fusion";
			conf.Type = "AnalisisDocumentos";

			var factory = new AnalisisDocumentosDefaultFactory(conf);
			IClient client = null;
			client = factory.GetClient();

			var Contexto = new AnalysisContext()
			{
				Aplication = "Infolex",
				Tenant = "Mª Carmen S.L.",
				Owner = "MªCarmen"
			};

			PackageRequest package = new PackageRequest()
			{
				Context = Contexto,
				PackageUniqueRefences = Guid.NewGuid().ToString()
			};

			var l = new List<DocumentAnalysisRequest>();
			for (int i = 0; i < 1; i++)
			{
				string referenciaDoc = ""; // mensajes.ToString("000,00#") + "_" + i.ToString("000,00#") + "_" + DateTimeOffset.UtcNow.UtcDateTime.ToString() + "_" + Guid.NewGuid().ToString() + "_" + idProceso.ToString();
				DocumentAnalysisRequest doc = new DocumentAnalysisRequest()
				{

					DocumentAccesURI = new Uri(@"https:\\loquesea_" + referenciaDoc + ".com"),
					DocumentName = "elNombreDoc_" + referenciaDoc,
					DocumentUniqueRefences = Guid.NewGuid().ToString()

				};
				l.Add(doc);

			}

			package.Documents = l.Select((dc) => dc);
			await client.SendRequestAsync(package);

		}

	}
}