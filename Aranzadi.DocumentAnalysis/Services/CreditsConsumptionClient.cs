using Aranzadi.DocumentAnalysis.Models.CreditReservations;
using Aranzadi.DocumentAnalysis.Models;
using Azure.Core;
using Azure.Identity;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Models.CreditConsumption;
using Aranzadi.HttpPooling;
using static Aranzadi.DocumentAnalysis.Models.OperationResult;

namespace Aranzadi.DocumentAnalysis.Services
{
    public class CreditsConsumptionClient : ICreditsConsumptionClient
	{
        private readonly DocumentAnalysisOptions configuration;

        private static HttpClient client;

        private static string BaseUrl;

        private const string authorizationHeader = "Authorization";
        private const string bearer = "Bearer ";

        public CreditsConsumptionClient(DocumentAnalysisOptions configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            //var socketsHttpHandler = new SocketsHttpHandler()
            //{
            //    PooledConnectionLifetime = TimeSpan.FromMinutes(1),
            //};
            client = httpClientFactory.CreateClient();//new HttpClient(socketsHttpHandler);
            BaseUrl = configuration.CreditsConsumption.CreditsConsumptionService ?? "https://localhost:44344";
            //BaseUrl = !string.IsNullOrEmpty(configUrl) ? configUrl : "https://localhost:44344";
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.BaseAddress = new Uri(BaseUrl);
            //ServicePointManager.FindServicePoint(new Uri(BaseUrl)).ConnectionLeaseTimeout = 60 * 1000;
        }

        public async Task<OperationResult<TenantCreditReservation>> CreateReservation(NewTenantCreditReservation reservation)
        {
            if (configuration.CheckIfActiveCreditsConsumption)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("/api/TenantCredits/Reservations", UriKind.Relative),
                    Content = new StringContent(JsonConvert.SerializeObject(reservation), Encoding.UTF8, "application/json"),
                };
                request.Headers.Add(authorizationHeader, bearer + await RequestAuthToken());
                HttpResponseMessage response = await client.SendAsync(request);
                return await response.ToOperationResult<TenantCreditReservation>();
            }
            else
            {
                TenantCreditReservation credit = new TenantCreditReservation() { ID = String.Empty, TenantCreditID = String.Empty };
                HttpResponseMessage response = new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(credit))
                };
                return await response.ToOperationResult<TenantCreditReservation>();
            }
        }

        public async Task<OperationResult> CompleteReservation(string tenantCreditId, string reservationId)
        {
            if (configuration.CheckIfActiveCreditsConsumption)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri("/api/TenantCredits/" + tenantCreditId + "/Reservations/" + reservationId + "/Complete",
                    UriKind.Relative),
                };
                request.Headers.Add(authorizationHeader, bearer + await RequestAuthToken());
                HttpResponseMessage response = await client.SendAsync(request);
                return await response.ToOperationResult();
            }
            else
            {
                var operation = new OperationResult() { Code = ResultCode.Success };
                HttpResponseMessage response = new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(operation))
                };
                return await response.ToOperationResult();
            }

        }

        public async Task<OperationResult<CreditResponse>> GetTenantCredits(string userAccountId, string creditTemplateID)
        {
            if (configuration.CheckIfActiveCreditsConsumption)
            {
                string parameters = "?TenantID=" + userAccountId + "&CreditTemplateID=" + creditTemplateID;

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("/api/TenantCredits" + parameters,
                    UriKind.Relative),
                };

                request.Headers.Add(authorizationHeader, bearer + await RequestAuthToken());

                HttpResponseMessage response = await client.SendAsync(request);
                return await response.ToOperationResult<CreditResponse>();
            }
            else
            {
                var response = new CreditResponse();
                TenantCredit tenantCredit = new TenantCredit() { FreeReservations = 2 };
                List<TenantCredit> tenantCreditList = new List<TenantCredit>();
                tenantCreditList.Add(tenantCredit);
                response.Count = 1;
                response.TenantCredits = tenantCreditList;
                HttpResponseMessage responsee = new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(response))
                };
                return await responsee.ToOperationResult<CreditResponse>();
            }
        }


        private async Task<string> RequestAuthToken()
        {
            string scope = configuration.CreditsConsumption.AzureADCreditsConsumptionScope ?? string.Empty;
            string tenantId = configuration.CreditsConsumption.AzureADCreditsConsumptionTenant ?? string.Empty;
            AccessToken tokenRes;
            //Uri? localUri;
            if (configuration.Environment == "Test")
            {
                return String.Empty;
            }
            else if (configuration.Environment == "Development") { 
                tokenRes = await new VisualStudioCredential(new VisualStudioCredentialOptions { TenantId = tenantId })
                   .GetTokenAsync(new TokenRequestContext(new string[] { scope }), CancellationToken.None);
            }
            else
            {
                tokenRes = await new DefaultAzureCredential(new DefaultAzureCredentialOptions { TenantId = tenantId })
                    .GetTokenAsync(new TokenRequestContext(new string[] { scope }));
            }
            return tokenRes.Token;
        }
    }
}
