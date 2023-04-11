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
using static System.Net.Mime.MediaTypeNames;
using System.Text;

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


        [HttpPost]
        [Route("SendMessageToServiceBusTest2")]
        public async void SendMessage2()
        {

            var requestContent = "";
            this.Request.EnableBuffering();
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestContent = await reader.ReadToEndAsync();
            }
            Request.Body.Position = 0;


            ServiceBusClient client = new ServiceBusClient(configuration.ServiceBus.ConnectionString);
            ServiceBusSender sender = client.CreateSender(configuration.ServiceBus.Queue);
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            var message = new ServiceBusMessage(requestContent);
            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(message))
            {
                // if it is too large for the batch
                throw new Exception($"The message is too large to fit in the batch.");
            }
            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        /////////////////////////////////////////////////////////

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
                Tenant = "Tenant",
                Owner = "Owner"
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
                    AccessUrl = new Uri(@"https:\\loquesea_" + referenciaDoc + ".com").ToString(),
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