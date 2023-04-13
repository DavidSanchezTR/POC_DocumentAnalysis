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
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.DTO.Response;
using Aranzadi.DocumentAnalysis.DTO;
using Microsoft.Extensions.Azure;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Polly;
using System.Diagnostics;
using Polly.Retry;

namespace Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations
{
    /// <summary>
    /// 
    /// </summary>
    internal class MessagingClient : IClient
    {

        private readonly IMessageSender messageSender;
        private readonly MessagingConfiguration confi;

        private readonly IHttpClientFactory httpCliFact;

        internal static readonly string CLIENT_ID = "SAD";

        /// <summary>
        /// In the unit test, we don't want to wait some minutes, so we decrease the Max_time_polly_Retry to 1 second,       
        ///  
        /// </summary>
        internal static int MAX_TIME_POLLY_RETRY = 60;

        internal static int N_TIMES_POLLY_RETRY = 5;

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
            try
            {
                Message<DocumentAnalysisRequest> message = PrepareMessage(theRequest);

                await messageSender.Send(this.confi.ServicesBusCola, message);

                return CalculateTrack(message); ;
            }
            catch (Exception ex)
            {

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
                DocumentUniqueRefences = chu.Data.Document.Hash
            });
            track.DocumentAnalysysRequestTracks = tracDetail;
            return track;
        }

        public async Task<DocumentResponse> GetAnalysisAsync(AnalysisContext context, string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentNullException(nameof(documentId));
            }            
            var list = await RequestStatusList(context, documentId);
            if (list.Count>1)
            {
                throw new DocumentAnalysisException("There are more than one documento with id: " + documentId);
            }
            return list.FirstOrDefault();

        }       
              

        public async Task<IEnumerable<DocumentResponse>> GetAnalysisAsync(AnalysisContext context)
        {            

            return await RequestStatusList(context,string.Empty);
        }

        private async Task<List<DocumentResponse>> RequestStatusList(AnalysisContext context, string docREf)
        {
            ValidateGetAlalysisContext(context);            
           
            using (var httpCli = this.httpCliFact.CreateClient(CLIENT_ID))
            {
                try
                {
                    StatusRequest theStatusRequest = new StatusRequest()
                    {
                        App = context.App,
                        Tenant = context.Tenant,
                        UserDataId = context.Owner,
                        Hash = docREf,
                    };
                    List<DocumentResponse> doc = new List<DocumentResponse>();
                    Uri theUri = GetUri(theStatusRequest);
                    AsyncRetryPolicy policy = GetRetryPolicy(theStatusRequest);
                    string jsonDocAnalisis = await policy.ExecuteAsync(async () =>
                    {
                        return await httpCli.GetStringAsync(theUri);
                    });

                    if (!string.IsNullOrEmpty(jsonDocAnalisis))
                    {
                        doc = JsonConvert.DeserializeObject<List<DocumentResponse>>(
                                       jsonDocAnalisis ?? String.Empty);
                    }
                    return doc;
                }
                catch (Exception ex)
                {
                    throw new DocumentAnalysisException("Error recuperando Status", ex);
                }                
            }
        }

        private static void ValidateGetAlalysisContext(AnalysisContext context)
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

        private Uri GetUri(StatusRequest re)
        {
            NameValueCollection queryString;
           
            queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(nameof(re.App), re.App);
            queryString.Add(nameof(re.UserDataId), re.UserDataId);
            queryString.Add(nameof(re.Tenant), re.Tenant);
            queryString.Add(nameof(re.Hash), re.Hash);

            UriBuilder ur = new UriBuilder(confi.URLServicioAnalisisDoc)
            {
                Query = queryString.ToString()
            };
            return ur.Uri;
        }

        private Polly.Retry.AsyncRetryPolicy GetRetryPolicy(StatusRequest theStatusRequest)
        {
            if (N_TIMES_POLLY_RETRY < 0)
                throw new DocumentAnalysisException("BAD POLLY Configuration, Nº Time Polly Retry: " + N_TIMES_POLLY_RETRY);

            var policy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(
                retryCount: N_TIMES_POLLY_RETRY,
                sleepDurationProvider: (attemptNum) => {                
                    return TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attemptNum), MAX_TIME_POLLY_RETRY));
                },
                onRetry: (ex, ts) =>
                {
                    if (!String.IsNullOrEmpty(theStatusRequest.Hash))
                    {
                        Debug.WriteLine(ex, "Repetimos el documento: " + theStatusRequest.Hash);
                    }
                    else
                    {
                        Debug.WriteLine(ex, "Repetimos sin documento: ");
                    }
                });
            return policy;
        }


    }

}
