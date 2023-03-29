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
	[Route("[controller]")]
	public class TestController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};
		//private static readonly SHA256 sHA256;
		private readonly DocumentAnalysisData datos = new DocumentAnalysisData
		{
			App = "Infolex",
			DocumentName = "Prueba.pdf",
			NewGuid = new Guid(Guid.NewGuid().ToString()),
			Analisis = "Esto es un análisis",
			AccessUrl = "www.prueba.com",
			Sha256 = "HasCode",
			Estado = "Pendiente",
			TenantId = 122,
			UserId = 22,
			FechaAnalisis = new DateTimeOffset().ToLocalTime(),
			FechaCreacion = new DateTimeOffset().ToLocalTime(),
		};

		private readonly IDocumentAnalysisRepository documentAnalysisRepository;

		private readonly ILogger<TestController> _logger;

		public TestController(ILogger<TestController> logger, IDocumentAnalysisRepository documentAnalysisRepository)
		{
			_logger = logger;
			this.documentAnalysisRepository = documentAnalysisRepository;
		}

		[HttpGet(Name = "GetTestController")]
		public IEnumerable<Test> Get()
		{

			enviarMensajes();

			return null;



			//Task.Run(() => documentAnalysisRepository.AddAnalysisDataAsync(datos));
			//         return Enumerable.Range(1, 5).Select(index => new Test
			//         {
			//             Date = DateTime.Now.AddDays(index),
			//             TemperatureC = Random.Shared.Next(-20, 55),
			//             Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			//         })
			//         .ToArray();            
		}

		private async void enviarMensajes()
		{
			string conString = "Endpoint=sb://uksouth-iflx-blue-orch-dev-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OzIgYihRXFO389WSagVM4MidAetFjoy7G72kgQxtOwg=";

			var conf = new MessagingConfiguration();
			conf.ServicesBusConnectionString = conString;
			conf.ServicesBusCola = "documentanalysistest"; // MessageDestinations.BackgroundOrchestrator;
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





			//

			//string conString = "Endpoint=sb://uksouth-iflx-blue-orch-dev-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OzIgYihRXFO389WSagVM4MidAetFjoy7G72kgQxtOwg=";
			//ServiceBusClient client = new ServiceBusClient(conString);

			//client = new ServiceBusClient(conString);
			//ServiceBusSender sender = client.CreateSender("documentanalysistest");

			//using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();


			//DocumentAnalysisRequest obj = new DocumentAnalysisRequest();
			//obj.DocumentAccesURI = new Uri("https://wwww.google.es");
			//obj.DocumentName = "DocumentName";
			//obj.DocumentUniqueRefences = "DocumentUniqueRefences";

			//string json = JsonSerializer.Serialize(obj);
			//var message = new ServiceBusMessage(json);

			//MessageEvent<DocumentAnalysisRequest> mensaje = new MessageEvent<DocumentAnalysisRequest>();

			//if (!messageBatch.TryAddMessage(message))
			//{
			//	// if it is too large for the batch
			//	throw new Exception($"ERROR");
			//}

			//try
			//{
			//	await sender.SendMessagesAsync(messageBatch);

			//}
			//finally
			//{

			//	await sender.DisposeAsync();
			//	await client.DisposeAsync();
			//}

		}


	}
}