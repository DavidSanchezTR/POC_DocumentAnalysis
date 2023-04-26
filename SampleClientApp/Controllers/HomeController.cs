using Aranzadi.DocumentAnalysis.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using SampleClientApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ThomsonReuters.BackgroundOperations.Messaging.Models;
using ThomsonReuters.BackgroundOperations.Messaging;
using Aranzadi.DocumentAnalysis.DTO.Enums;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Aranzadi.DocumentAnalysis.Messaging;
using System.Configuration;
using Aranzadi.DocumentAnalysis.DTO;
using Microsoft.Extensions.Logging;
using log4net;
using log4net.Repository.Hierarchy;
using Aranzadi.DocumentAnalysis.DTO.Response;
using System.Net.Http;
using System.Net.Sockets;

namespace SampleClientApp.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
           return View();
        }

        public ActionResult SendMessage()
        {
			log4net.Config.XmlConfigurator.Configure();


			// TODO: Enviar mensaje  de analisis de documentos al orquestador

			string backgroundOrchestratorEndpoint = "Endpoint=sb://uksouth-iflx-blue-orch-dev-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OzIgYihRXFO389WSagVM4MidAetFjoy7G72kgQxtOwg=";
            string Queue = "backgroundorchestrator";
            int lawfirmID = 100;
            string account = "DESPACHO";
            int UserId = 101;
            string fromEmail = "prueba@gmail.com";
            try
            {

                var conf = new MessagingConfiguration();
                conf.ServicesBusConnectionString = backgroundOrchestratorEndpoint;
                conf.ServicesBusCola = Queue;
                conf.Source = BackgroundOperationsFactory.MESSAGE_SOURCE_FUSION;
                conf.Type = BackgroundOperationsFactory.MESSAGE_TYPE_DOCUMENT_ANALYSIS;
                conf.URLOrquestador = new Uri("https://urlorquestador.com");
                conf.URLServicioAnalisisDoc = new Uri("https://urlservicioanalisis.com");


				LogManager.GetLogger("InfolexWebSessionLog");
				ILog logger = LogManager.GetLogger("loggerTest");


				var factory = new AnalisisDocumentosDefaultFactory(conf, logger);
                IClient client = factory.GetClient();	



				PackageRequest theRequest = new PackageRequest();
                theRequest.Context = new AnalysisContext()
                {
                    Account = account,
                    App = "Fusion",
                    Owner = UserId.ToString(),
                    Tenant = lawfirmID.ToString()
                };

                theRequest.PackageUniqueRefences = "uniqueReferences";

                List<string> listaFicheros = new List<string>();
                listaFicheros.Add("fichero1.pdf");
                listaFicheros.Add("fichero2.pdf");

                var documents = new List<DocumentAnalysisRequest>();

                foreach (string fileName in listaFicheros)
                {
                    Guid guid = Guid.NewGuid();
                    string originalFileName = guid.ToString() + "_" + fileName;                    
                    string mailMessageId = guid.ToString();
                    string conversationMailMessageId = guid.ToString();
                    var hash = HashCode.Combine(guid).ToString();
                    //string fileName = hash + ext;
                    string tokenUrlAttachment = "https://www.tokenUrl.es/" + guid.ToString();

                    var docAnalysisRequest = new DocumentAnalysisRequest
                    {
						Name = originalFileName,
						Path = tokenUrlAttachment,
                        Guid = guid.ToString()
                    };

                    documents.Add(docAnalysisRequest);
                }

                theRequest.Documents = documents;
                client.SendRequestAsync(theRequest);

                ViewBag.Message = "Mensaje enviado";

                return View();

            }
            catch (Exception)
            {
                throw;
            }

        }


		private void Metodo()
		{
			try
			{
				var conf = new MessagingConfiguration();
				conf.ServicesBusConnectionString = "Endpoint=sb://uksouth-iflx-blue-orch-dev-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OzIgYihRXFO389WSagVM4MidAetFjoy7G72kgQxtOwg=";
				conf.ServicesBusCola = "documentanalysis";
				conf.Source = "Fusion";
				conf.Type = "DocumentAnalysis";
				conf.URLOrquestador = new Uri("https://urlorquestador.com");
				conf.URLServicioAnalisisDoc = new Uri("https://localhost:7075");
				var factory = new AnalisisDocumentosDefaultFactory(conf);
				IClient client = factory.GetClient();

				var response = client.GetAnalysisAsync(new AnalysisContext()
				{
					Account = "4131",
					App = "Fusion",
					Owner = "98",
					Tenant = "4131"
				});
                var fin = response.Result;
			}
			catch (Exception ex)
			{
				throw;
			}

		}


	}
}