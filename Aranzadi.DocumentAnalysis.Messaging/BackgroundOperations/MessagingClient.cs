using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThomsonReuters.BackgroundOperations.Messaging;
using ThomsonReuters.BackgroundOperations.Messaging.Models;
using System.Linq;
using System.Transactions;
using Microsoft.Extensions.Azure;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Polly;
using System.Diagnostics;
using Polly.Retry;
using Microsoft.Extensions.Logging;
using log4net;
using Microsoft.Azure.Amqp.Framing;
using Azure;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Messaging.Model;

namespace Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations
{

    internal class MessagingClient : IClient
    {

        private readonly IMessageSender messageSender;
        private readonly MessagingConfiguration confi;

        private readonly IHttpClientFactory httpCliFact;
		private readonly ILog logger;
		internal static readonly string CLIENT_ID = "SAD";

		internal static string GetAnalysisEndPoint = "api/DocumentAnalysis/GetAnalysis";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageSender"></param>
		/// <param name="confi"></param>
		/// <param name="cli"></param>
		/// <exception cref="ArgumentNullException">If some of the parameters is null</exception>
		internal MessagingClient(IMessageSender messageSender, MessagingConfiguration confi, IHttpClientFactory cli)
        {
            ValidateConstructorParameters(messageSender, confi, cli);

            this.messageSender = messageSender;
            this.confi = confi;
            this.httpCliFact = cli;
        }

		internal MessagingClient(IMessageSender messageSender, MessagingConfiguration confi, IHttpClientFactory cli, ILog logger) : this(messageSender, confi, cli)
		{
			this.logger = logger;
		}

		private static void ValidateConstructorParameters(IMessageSender messageSender, MessagingConfiguration confi, IHttpClientFactory cli)
        {
            if (messageSender == null)
            {
                throw new ArgumentNullException(nameof(messageSender));
            }
            if (confi == null)
            {
                throw new ArgumentNullException(nameof(confi));
            }
            if (!confi.Validate())
            {
                throw new ArgumentException(nameof(confi));
            }
            if (cli == null)
            {
                throw new ArgumentNullException(nameof(cli));
            }
        }

        public async Task<PackageRequestTrack> SendRequestAsync(PackageRequest theRequest)
        {
            ValidateRequest(theRequest);

			logger?.Info($"Sending message to {confi.ServicesBusCola} with uid {theRequest.PackageUniqueRefences}");

			try
            {
                Message<DocumentAnalysisRequest> message = PrepareMessage(theRequest);

                await messageSender.Send(this.confi.ServicesBusCola, message);

                return CalculateTrack(message); ;
            }
            catch (Exception ex)
            {
				logger?.Error("Error while sending", ex);
				throw new DocumentAnalysisException("Error Enviando mensaje", ex);
            }

        }


        private static void ValidateRequest(PackageRequest theRequest)
        {
            if (theRequest == null)
            {
                throw new ArgumentNullException(nameof(theRequest));
            }
            if (!theRequest.Validate())
            {
                throw new ArgumentException(nameof(theRequest));
            }
        }

        private Message<DocumentAnalysisRequest> PrepareMessage(PackageRequest theRequest)
        {
            Message<DocumentAnalysisRequest> message;
            var dataChunks = theRequest.Documents.Select(
                            doc => new MessageDataChunk<DocumentAnalysisRequest>(doc));

            message = new Message<DocumentAnalysisRequest>(
               confi.Source, confi.Type, theRequest.Context.Tenant, dataChunks)
            {
                AdditionalData = theRequest.Context
            };
            return message;
        }

        private static PackageRequestTrack CalculateTrack(Message<DocumentAnalysisRequest> message)
        {
            PackageRequestTrack track = new PackageRequestTrack(){
                TrackingNumber = message.ID
            };
            var tracDetail = message.DataChunks.Select(chu => new DocumentAnalysisRequestTrack()
            {
                TrackingNumber = chu.ID,
                DocumentUniqueRefences = chu.Data.Guid
            });
            track.DocumentAnalysysRequestTracks = tracDetail;
            return track;
        }

        public async Task<DocumentAnalysisResponse> GetAnalysisAsync(AnalysisContext context, string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentNullException(nameof(documentId));
            }

			var list = await RequestStatusList(context, documentId);
			if (list.Count() > 1)
			{
				throw new DocumentAnalysisException("There are more than one documento with id: " + documentId);
			}
			return list.FirstOrDefault();
        }       
              
        public async Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(AnalysisContext context)
        {
			return await RequestStatusList(context,string.Empty);
        }

        private async Task<IEnumerable<DocumentAnalysisResponse>> RequestStatusList(AnalysisContext context, string documentId)
        {
            ValidateGetAnalysisContext(context);            
           
            using (HttpClient httpCli = this.httpCliFact.CreateClient(CLIENT_ID))
            {
                try
                {
                    StatusRequest theStatusRequest = new StatusRequest()
                    {
                        App = context.App,
                        Tenant = context.Tenant,
                        Owner = context.Owner,
                        DocumentId = documentId
                    };

					Uri theUri = GetUri(GetAnalysisEndPoint, theStatusRequest);
                    HttpResponseMessage resp = await httpCli.GetAsync(theUri);
                    var res = resp.Content;
                    var resultado = await res.ReadAsStringAsync();
                    var listadoServicioDocumentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(resultado);
                    if (listadoServicioDocumentAnalysisResponse == null)
                    {
                        listadoServicioDocumentAnalysisResponse = new List<DocumentAnalysisResponse>();
                    }

                    return listadoServicioDocumentAnalysisResponse;

				}
                catch (Exception ex)
                {
                    throw new DocumentAnalysisException("Error recuperando Status", ex);
                }                
            }
        }

        private static void ValidateGetAnalysisContext(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (!context.Validate())
            {
                throw new ArgumentException(nameof(context));
            }
        }

        private Uri GetUri(string relativePath, StatusRequest re)
        {
            UriBuilder ur = new UriBuilder(confi.URLServicioAnalisisDoc);
            ur.Path = relativePath;

			NameValueCollection queryString;
			queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
			queryString.Add(nameof(re.App), re.App);
			queryString.Add(nameof(re.Owner), re.Owner);
			queryString.Add(nameof(re.Tenant), re.Tenant);
			queryString.Add(nameof(re.DocumentId), re.DocumentId);

            ur.Query = queryString.ToString();
			return ur.Uri;
        }


    }

}
