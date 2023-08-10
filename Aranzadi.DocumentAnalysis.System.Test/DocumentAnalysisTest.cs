using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Models;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.DependencyModel.Resolution;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;

namespace Aranzadi.DocumentAnalysis.System.Test
{
	[TestClass]
	public class DocumentAnalysisTest
	{
		[TestMethod]
		public async Task SendMessageAndRetrieveOneAnalysis_OK()
		{
			//Send Message
			JsonData data = new JsonData()
			{
				IdAnalysis = Guid.NewGuid().ToString(),
				ZipName = "test.zip",
				SasToken = AssemblyApp.sasToken,
				Tenant = "5600",
				Owner = "98"
			};
			string serviceBusMessage = generateJson(data);
			await sendMessageToQueueServiceBus(serviceBusMessage);

			wait();

			//Check result 
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant={data.Tenant}&Owner={data.Owner}&DocumentId={data.IdAnalysis}");
			using HttpClient client = new HttpClient();
			var response = await client.GetAsync(uri);
			var result = await response.Content.ReadAsStringAsync();
			IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

			// Assert
			Assert.IsTrue(documentAnalysisResponse.Count() == 1);
		}

		[TestMethod]
		public async Task SendTwoMessagesAndRetrieveSeveralAnalysisByTenantAndOwner_OK()
		{
			string tenant = "5600";
			string owner = "101";
			//Send message
			JsonData data1 = new JsonData()
			{
				IdAnalysis = Guid.NewGuid().ToString(),
				ZipName = "test1.zip",
				SasToken = AssemblyApp.sasToken,
				Tenant = tenant,
				Owner = owner
			};
			string serviceBusMessage1 = generateJson(data1);
			await sendMessageToQueueServiceBus(serviceBusMessage1);

			//Send another message
			JsonData data2 = new JsonData()
			{
				IdAnalysis = Guid.NewGuid().ToString(),
				ZipName = "test2.zip",
				SasToken = AssemblyApp.sasToken,
				Tenant = tenant,
				Owner = owner
			};
			string serviceBusMessage2 = generateJson(data2);
			await sendMessageToQueueServiceBus(serviceBusMessage2);

			wait();

			//Check result 
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant={tenant}&Owner={owner}");
			using HttpClient client = new HttpClient();
			var response = await client.GetAsync(uri);
			var result = await response.Content.ReadAsStringAsync();
			IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

			// Assert
			Assert.IsTrue(documentAnalysisResponse.Count() > 1);
		}

		[TestMethod]
		public async Task SendMessageJsonBadFormatted_FailMessageInServiceBusQueue()
		{
			//Send Message
			JsonData data = new JsonData()
			{
				IdAnalysis = Guid.NewGuid().ToString(),
				ZipName = "test.zip",
				SasToken = AssemblyApp.sasToken,
				Tenant = "5600",
				Owner = "98"
			};
			string serviceBusMessage = generateJsonBadFormatted(data);
			await sendMessageToQueueServiceBus(serviceBusMessage);

			wait();

			//Check result 
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant={data.Tenant}&Owner={data.Owner}&DocumentId={data.IdAnalysis}");
			using HttpClient client = new HttpClient();
			var response = await client.GetAsync(uri);
			var result = await response.Content.ReadAsStringAsync();
			IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

			// Assert
			Assert.IsTrue(documentAnalysisResponse.Count() == 0);
		}

		[TestMethod]
		public async Task SendMessageSasTokenNotExist_NoAddInCosmos()
		{
			//Send Message
			JsonData data = new JsonData()
			{
				IdAnalysis = Guid.NewGuid().ToString(),
				ZipName = "test.zip",
				SasToken = AssemblyApp.sasToken + "urlErronea",
				Tenant = "5600",
				Owner = "98"
			};
			string serviceBusMessage = generateJson(data);
			await sendMessageToQueueServiceBus(serviceBusMessage);

			wait();

			//Check result 
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant={data.Tenant}&Owner={data.Owner}&DocumentId={data.IdAnalysis}");
			using HttpClient client = new HttpClient();
			var response = await client.GetAsync(uri);
			var result = await response.Content.ReadAsStringAsync();
			IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

			// Assert
			Assert.IsTrue(documentAnalysisResponse.Count() == 0);
		}


		[TestMethod]
		public async Task Send100Messages_OK()
		{
			List<string> listaIDs = new List<string>();
			for (int i = 0; i < 100; i++)
			{
				//Send Message
				JsonData data = new JsonData()
				{
					IdAnalysis = Guid.NewGuid().ToString(),
					ZipName = "test.zip",
					SasToken = AssemblyApp.sasToken,
					Tenant = "5600",
					Owner = "98"
				};
				listaIDs.Add(data.IdAnalysis);
				string serviceBusMessage = generateJson(data);
				await sendMessageToQueueServiceBus(serviceBusMessage);

			}


			Thread.Sleep(60 * 1000);


			//Check result 
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			foreach (var id in listaIDs)
			{
				var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant=5600&Owner=98&DocumentId={id}");
				using HttpClient client = new HttpClient();
				var response = await client.GetAsync(uri);
				var result = await response.Content.ReadAsStringAsync();
				IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

				// Assert
				Assert.IsTrue(documentAnalysisResponse.Count() == 1);

			}


		}




		private void wait()
		{
			Thread.Sleep(AssemblyApp.waitingSeconds * 1000);
		}

		private string generateJson(JsonData data)
		{
			string message = @$"{{
					""id"": ""{Guid.NewGuid()}"",
					""source"": ""BackgroundOrchestrator"",
					""type"": ""DocumentAnalysis"",
					""data"": {{
						""OriginalMessageId"": ""{Guid.NewGuid()}"",
						""Ordered"": false,
						""OrderedSessionID"": null,
						""Tenant"": ""{data.Tenant}"",
						""DataChunks"": [
							{{
								""Success"": true,
								""Detail"": null,
								""ID"": ""{Guid.NewGuid()}"",
								""Data"": {{
									""Guid"": ""{data.IdAnalysis}"",
									""Name"": ""{data.ZipName}"",
									""Path"": ""{data.SasToken}""
								}}
							}}
						],
						""AdditionalData"": {{
							""Owner"": ""{data.Owner}"",
							""Tenant"": ""{data.Tenant}"",
							""Account"": ""{data.Tenant}"",
							""App"": ""Fusion""
						}},
						""BackgroundOrchestratorScheduledEnqueueTimeUtc"": null
					}},
					""time"": ""{DateTime.Now.ToString("o")}"",
					""specversion"": ""1.0""
				}}";

			return message;
		}

		private string generateJsonBadFormatted(JsonData data)
		{
			string message = @$"{{
					Esto esta mal formado
					""id"": ""{Guid.NewGuid()}"",
					""source"": ""BackgroundOrchestrator"",
					""type"": ""DocumentAnalysis"",
					""data"": {{
						""OriginalMessageId"": ""{Guid.NewGuid()}"",
						""Ordered"": false,
						""OrderedSessionID"": null,
						""Tenant"": ""{data.Tenant}"",
						""DataChunks"": [
							{{
								""Success"": true,
								""Detail"": null,
								""ID"": ""{Guid.NewGuid()}"",
								""Data"": {{
									""Guid"": ""{data.IdAnalysis}"",
									""Name"": ""{data.ZipName}"",
									""Path"": ""{data.SasToken}""
								}}
							}}
						],
						""AdditionalData"": {{
							""Owner"": ""{data.Owner}"",
							""Tenant"": ""{data.Tenant}"",
							""Account"": ""{data.Tenant}"",
							""App"": ""Fusion""
						}},
						""BackgroundOrchestratorScheduledEnqueueTimeUtc"": null
					}},
					""time"": ""{DateTime.Now.ToString("o")}"",
					""specversion"": ""1.0""
				}}";

			return message;
		}

		private async Task sendMessageToQueueServiceBus(string message)
		{
			string queueName = AssemblyApp.documentAnalysisOptions.Messaging.Queue;
			string connectionString = AssemblyApp.documentAnalysisOptions.Messaging.Endpoint;

			ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString, new ServiceBusClientOptions()
			{
				TransportType = ServiceBusTransportType.AmqpWebSockets
			});
			ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);
			await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message));
		}

	}
}