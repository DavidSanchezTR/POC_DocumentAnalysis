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

namespace SampleClientApp.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult SendMessageOld()
        {
            // TODO: Enviar mensaje  de analisis de documentos al orquestador

            try
            {

                string backgroundOrchestratorEndpoint = "Endpoint=sb://uksouth-iflx-blue-orch-dev-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OzIgYihRXFO389WSagVM4MidAetFjoy7G72kgQxtOwg=";
                var sender = BackgroundOperationsFactory
                    .MessageFactory.GetSender(new ThomsonReuters.BackgroundOperations.Messaging.Models.ConnectionSettings(backgroundOrchestratorEndpoint), null);

                var chunks = new List<MessageDataChunk<DocumentAnalysisRequest>>();
                List<KeyValuePair<string, byte[]>> files2Attach = new List<KeyValuePair<string, byte[]>>(); // emailService.GetOutlookMailFilesForAttach(attachmentMode, myOutlookMailModel, ref actionName, false);
                files2Attach.Add(new KeyValuePair<string, byte[]>("fichero1.pdf", null));
                files2Attach.Add(new KeyValuePair<string, byte[]>("fichero2.pdf", null));

                int lawfirmID = 100;
                string account = "DESPACHO";
                int UserId = 101;
                string fromEmail = "prueba@gmail.com";

                foreach (KeyValuePair<string, byte[]> file in files2Attach)
                {
                    Guid guid = Guid.NewGuid();
                    string originalFileName = "FileName_" + guid.ToString();
                    string ext = ".pdf";
                    string mailMessageId = guid.ToString();
                    string conversationMailMessageId = guid.ToString();
                    var hash = HashCode.Combine(guid).ToString();
                    string fileName = hash + ext;
                    string tokenUrlAttachment = "https://www.tokenUrl.es/" + guid.ToString();

                    chunks.Add(new MessageDataChunk<DocumentAnalysisRequest>(new DocumentAnalysisRequest
                    {
                        EmailId = mailMessageId,
                        Subject = "Subject " + guid.ToString(),
                        ConversationID = conversationMailMessageId,
                        FromEmail = fromEmail,
                        Document = new DocumentAnalysisFile
                        {
                            Name = originalFileName,
                            Path = tokenUrlAttachment,
                            Hash = hash
                        },
                        UserAnalysis = new UserAnalysis
                        {
                            LawFirmId = lawfirmID,
                            UserId = UserId
                        },                        
                        Source = Source.LaLey,
                        Analysis = "Descripcion del analisis"
                        
                    }));
                }

                var message = new Message<DocumentAnalysisRequest>(BackgroundOperationsFactory.MESSAGE_SOURCE_FUSION,
                    BackgroundOperationsFactory.MESSAGE_TYPE_DOCUMENT_ANALYSIS, UserId.ToString(), chunks);

                message.AdditionalData = Newtonsoft.Json.JsonConvert.SerializeObject(new AuthData
                {
                    Account = account,
                    LawFirmID = lawfirmID,
                    UserDataID = UserId,
                    App = "Fusion",
                    Owner = UserId,
                    Tenant = lawfirmID.ToString()
                });

                sender.Send(MessageDestinations.BackgroundOrchestrator, message).Wait();

                ViewBag.Message = "Mensaje enviado";

                return View();

            }
            catch (Exception ex)
            {
                throw;
            }

        }


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

                var factory = new AnalisisDocumentosDefaultFactory(conf);
                IClient client = factory.GetClient();

                

                
                
                
                PackageRequest theRequest = new PackageRequest();
                theRequest.Context = new Aranzadi.DocumentAnalysis.DTO.AnalysisContext()
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
                    string ext = ".pdf";
                    string mailMessageId = guid.ToString();
                    string conversationMailMessageId = guid.ToString();
                    var hash = HashCode.Combine(guid).ToString();
                    //string fileName = hash + ext;
                    string tokenUrlAttachment = "https://www.tokenUrl.es/" + guid.ToString();

                    var docAnalysisRequest = new DocumentAnalysisRequest
                    {
                        EmailId = mailMessageId,
                        Subject = "Subject " + guid.ToString(),
                        ConversationID = conversationMailMessageId,
                        FromEmail = fromEmail,
                        Document = new DocumentAnalysisFile
                        {
                            Name = originalFileName,
                            Path = tokenUrlAttachment,
                            Hash = hash
                        },
                        UserAnalysis = new UserAnalysis
                        {
                            LawFirmId = lawfirmID,
                            UserId = UserId
                        },
                        Source = Source.LaLey,
                        Analysis = "Descripcion del analisis"

                    };

                    documents.Add(docAnalysisRequest);
                }

                theRequest.Documents = documents;
                client.SendRequestAsync(theRequest);

                ViewBag.Message = "Mensaje enviado";

                return View();

            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}