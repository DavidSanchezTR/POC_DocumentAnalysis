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
				Tenant = AssemblyApp.TenantId,
				Owner = AssemblyApp.UserId
			};
			string serviceBusMessage = generateJson(data);
			await sendMessageToQueueServiceBus(serviceBusMessage);

			wait();

			//Check result 
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant={data.Tenant}&DocumentId={data.IdAnalysis}");
			using HttpClient client = new HttpClient();
			var response = await client.GetAsync(uri);
			var result = await response.Content.ReadAsStringAsync();
			IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

			// Assert
			Assert.IsTrue(documentAnalysisResponse.Count() == 1, $"La coleccion de respuesta debería tener 1 analisis con ID: '{data.IdAnalysis.ToString()}', pero hay {documentAnalysisResponse.Count()}.");
			Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data.IdAnalysis) != null, $"No ha encontrado en la coleccion de respuesta el analisis con Id {data.IdAnalysis}");
			//Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data.IdAnalysis).Status == Messaging.Model.Enums.AnalysisStatus.Done, $"El analisis con Id {data.IdAnalysis} no está en estado 'Done', si no '{documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data.IdAnalysis).Status}'");

		}

		[TestMethod]
		public async Task SendTwoMessagesAndRetrieveSeveralAnalysisByTenantAndOwner_OK()
		{
			string tenant = AssemblyApp.TenantId;
			string owner = AssemblyApp.UserId;
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


			await sendMessageToQueueServiceBus(serviceBusMessage1);
			Thread.Sleep(30000);
			await sendMessageToQueueServiceBus(serviceBusMessage2);

			wait();

			//Check result 
			string documentsIds = data1.IdAnalysis.ToString() + ";" + data2.IdAnalysis.ToString();
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysisList?Tenant={tenant}&documentIdList={documentsIds}");
			using HttpClient client = new HttpClient();
			var response = await client.GetAsync(uri);
			var result = await response.Content.ReadAsStringAsync();
			IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

			// Assert
			Assert.IsTrue(documentAnalysisResponse.Count() == 2, $"La coleccion de respuesta debería tener 2 analisis con IDs: '{data1.IdAnalysis.ToString()}' y '{data2.IdAnalysis.ToString()}', pero hay {documentAnalysisResponse.Count()}.");
			Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data1.IdAnalysis) != null, $"No ha encontrado en la coleccion de respuesta el analisis con Id {data1.IdAnalysis}");
			Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data2.IdAnalysis) != null, $"No ha encontrado en la coleccion de respuesta el analisis con Id {data2.IdAnalysis}");
			//Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data1.IdAnalysis).Status == Messaging.Model.Enums.AnalysisStatus.Done, $"El analisis con Id {data1.IdAnalysis} no está en estado 'Done', si no '{documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data1.IdAnalysis).Status}'");
			//Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data2.IdAnalysis).Status == Messaging.Model.Enums.AnalysisStatus.Done, $"El analisis con Id {data1.IdAnalysis} no está en estado 'Done', si no '{documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data2.IdAnalysis).Status}'");


		}

		[TestMethod]
		public async Task SendMessageSasTokenNotExist_AddInCosmos()
		{
			//Send Message
			JsonData data = new JsonData()
			{
				IdAnalysis = Guid.NewGuid().ToString(),
				ZipName = "test.zip",
				SasToken = AssemblyApp.sasToken + "urlErronea",
				Tenant = AssemblyApp.TenantId,
				Owner = AssemblyApp.UserId
			};
			string serviceBusMessage = generateJson(data);
			await sendMessageToQueueServiceBus(serviceBusMessage);

			wait();

			//Check result 
			Uri baseUri = new Uri(AssemblyApp.urlBaseDocumentAnalysisService);
			var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant={data.Tenant}&DocumentId={data.IdAnalysis}");
			using HttpClient client = new HttpClient();
			var response = await client.GetAsync(uri);
			var result = await response.Content.ReadAsStringAsync();
			IEnumerable<DocumentAnalysisResponse> documentAnalysisResponse = JsonConvert.DeserializeObject<IEnumerable<DocumentAnalysisResponse>>(result);

            // Assert
			Assert.IsTrue(documentAnalysisResponse.Count() == 1, $"La colección de respuesta solo debería tener un analisis para el ID: '{data.IdAnalysis.ToString()}', hay {documentAnalysisResponse.Count()}.");
			Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data.IdAnalysis) != null, $"No ha encontrado en la coleccion de respuesta el analisis con Id {data.IdAnalysis}");
			Assert.IsTrue(documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data.IdAnalysis).Status == Messaging.Model.Enums.AnalysisStatus.Error, $"El analisis con Id {data.IdAnalysis} no está en estado 'Error', si no '{documentAnalysisResponse.FirstOrDefault(o => o.DocumentUniqueRefences == data.IdAnalysis).Status}'");

		}


		[Ignore]
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
					Tenant = AssemblyApp.TenantId,
					Owner = AssemblyApp.UserId
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
				var uri = new Uri(baseUri, $"api/DocumentAnalysis/GetAnalysis?Tenant={AssemblyApp.TenantId}&DocumentId={id}");
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