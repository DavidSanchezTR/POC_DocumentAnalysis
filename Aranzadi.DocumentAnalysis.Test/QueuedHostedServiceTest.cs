using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Models.CreditConsumption;
using Aranzadi.DocumentAnalysis.Models.CreditReservations;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Test.Moq;
using Aranzadi.HttpPooling.Interfaces;
using Aranzadi.HttpPooling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test
{
    [TestClass]
	public class QueuedHostedServiceTest
	{

		[TestMethod]
		public async Task ProccessMessage_OK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();

			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();
			Mock<IHttpPoolingServices> serviceBusPoolingServiceMock = new Mock<IHttpPoolingServices>();
			serviceBusPoolingServiceMock.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);

			//******* Mock ServiceScope
			var service = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			service.Setup(o => o.GetAnalysisDoneAsync(It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResult));
			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(service.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProvider
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);
			//******* Mock response Sas token
			var response = "Lo que sea";
			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
			{
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK, //EDIT
					Content = new StringContent(JsonConvert.SerializeObject(response))
				};
			});
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

			////******* Mock response analysis provider (Anaconda)
			HttpResponseMessage responseMessage = new HttpResponseMessage();  //EDIT
			responseMessage.StatusCode = System.Net.HttpStatusCode.OK;
			responseMessage.Content = new StringContent("");
			AnalysisJobResponse responseJob = new AnalysisJobResponse();  //EDIT
			responseJob.Guid = Guid.NewGuid().ToString();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			analysisProviderServiceMock.Setup(o => o.SendAnalysisJob(It.IsAny<DocumentAnalysisData>()))
				.Returns(Task.FromResult((responseMessage, responseJob)));

            using Stream? stream = null;
            Mock<ICreditsConsumptionClient> creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};

			var tenantResult = new TenantCredit()
			{
			  TenantId = "5600",
			   UserId = "98",
			   CreditTemplateId = "analisisdocumental",
			    FreeReservations = 1,				 
            };

			List<TenantCredit> list = new List<TenantCredit>() { tenantResult };

			var creaditResult = new CreditResponse()
			{
				TenantCredits = list,
				Count = 1,
            };

			var tenantCreditReservation = new TenantCreditReservation()
			{
				CreditTemplateID = "analisisdocumental",
				UserID = "98",
				TenantCreditID = "5600",
				Completed = true
			};


            creditsConsumptionClient.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });
            creditsConsumptionClient.Setup(x => x.GetTenantCredits(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(OperationResult<CreditResponse>.Success(creaditResult)); });
            creditsConsumptionClient.Setup(x => x.CreateReservation(It.IsAny<NewTenantCreditReservation>())).Returns(() => { return Task.FromResult(OperationResult<TenantCreditReservation>.Success(tenantCreditReservation)); });


            var hostedService = new QueuedHostedService(serviceProvider.Object, documentAnalysisOptions, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingServiceMock.Object, creditsConsumptionClient.Object);
			AnalysisContext analysisContext = new AnalysisContext()
			{
				Account = "5600",
				App = "Fusion",
				Owner = "98",
				Tenant = "5600"
			};
			DocumentAnalysisRequest documentAnalysisRequest = new DocumentAnalysisRequest()
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "test.zip",
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D",
				AnalysisType = Messaging.Model.Enums.AnalysisTypes.Undefined
			};

           
            //Act
            var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);


			//Assert
			Assert.IsTrue(result);

		}

		[TestMethod]
		public async Task ProccessMessage_ErrorResponseFromAnalysisProvider_ReturnFalse()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();

			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();
			Mock<IHttpPoolingServices> serviceBusPoolingService = new Mock<IHttpPoolingServices>();

			//******* Mock ServiceScope
			var service = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			service.Setup(o => o.GetAnalysisDoneAsync(It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResult));
			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(service.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProvider
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);
			//******* Mock response Sas token
			var response = "Lo que sea";
			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
			{
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK, //EDIT
					Content = new StringContent(JsonConvert.SerializeObject(response))
				};
			});
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));


            Mock<ICreditsConsumptionClient> creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();
            var operationResult = new OperationResult()
            {
                Code = OperationResult.ResultCode.Success
            };

            var tenantResult = new TenantCredit()
            {
                TenantId = "5600",
                UserId = "98",
                CreditTemplateId = "analisisdocumental",
                FreeReservations = 1,
            };

            List<TenantCredit> list = new List<TenantCredit>() { tenantResult };

            var creaditResult = new CreditResponse()
            {
                TenantCredits = list,
                Count = 1,
            };

            var tenantCreditReservation = new TenantCreditReservation()
            {
                CreditTemplateID = "analisisdocumental",
                UserID = "98",
                TenantCreditID = "5600",
                Completed = true
            };


            creditsConsumptionClient.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });
            creditsConsumptionClient.Setup(x => x.GetTenantCredits(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(OperationResult<CreditResponse>.Success(creaditResult)); });
            creditsConsumptionClient.Setup(x => x.CreateReservation(It.IsAny<NewTenantCreditReservation>())).Returns(() => { return Task.FromResult(OperationResult<TenantCreditReservation>.Success(tenantCreditReservation)); });


            ////******* Mock response analysis provider (Anaconda)
            HttpResponseMessage responseMessage = new HttpResponseMessage();  //EDIT
			responseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
			responseMessage.Content = new StringContent("");
			AnalysisJobResponse responseJob = new AnalysisJobResponse();  //EDIT
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			analysisProviderServiceMock.Setup(o => o.SendAnalysisJob(It.IsAny<DocumentAnalysisData>()))
				.Returns(Task.FromResult((responseMessage, responseJob)));

            using Stream? stream = null;
			var hostedService = new QueuedHostedService(serviceProvider.Object, documentAnalysisOptions, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClient.Object);
			AnalysisContext analysisContext = new AnalysisContext()
			{
				Account = "5600",
				App = "Fusion",
				Owner = "98",
				Tenant = "5600"
			};
			DocumentAnalysisRequest documentAnalysisRequest = new DocumentAnalysisRequest()
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "test.zip",
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D",
				AnalysisType = Messaging.Model.Enums.AnalysisTypes.Undefined
			};


			//Act
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);


			//Assert
			Assert.IsFalse(result);

		}

		[TestMethod]
		public async Task SendMessageToPoolingQueue_OK()
		{
			//Arrange
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();
			Mock<IHttpPoolingServices> serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);
			var creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();

			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClient.Object);
			DocumentAnalysisData data = new DocumentAnalysisData();
			data.Id = Guid.NewGuid();
			data.AnalysisProviderId = Guid.NewGuid();

			//Act Assert
			try
			{
				await hostedService.SendMessageToPoolingQueue(data);
			}
			catch (Exception ex)
			{
				Assert.Fail($"No se esperaba excepción, pero aqui está: {ex.Message}");
			}

		}

		[TestMethod]
		public async Task SendMessageToPoolingQueue_DataAnalysisProviderIdEmpty_ThrowArgumentNullException()
		{
			//Arrange
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();
			Mock<IHttpPoolingServices> serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);
			var creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();

			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClient.Object);
			DocumentAnalysisData data = new DocumentAnalysisData();
			data.Id = Guid.NewGuid();
			data.AnalysisProviderId = null;
			
			//Act Assert
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await hostedService.SendMessageToPoolingQueue(data));
		}

		[TestMethod]
		public async Task SendMessageToPoolingQueue_DataIdIsEmpty_ThrowArgumentNullException()
		{
			//Arrange
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();
			Mock<IHttpPoolingServices> serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);
			var creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();

			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClient.Object);
			DocumentAnalysisData data = new DocumentAnalysisData();
			//data.Id = new Guid();
			data.AnalysisProviderId = Guid.NewGuid();

			//Act Assert
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await hostedService.SendMessageToPoolingQueue(data));
		}

		[TestMethod]
		public async Task SendMessageToPoolingQueue_DataIsNull_ThrowArgumentNullException()
		{
			//Arrange
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();
			Mock<IHttpPoolingServices> serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);
			var creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();

			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClient.Object);
			DocumentAnalysisData data = null;

			//Act Assert
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await hostedService.SendMessageToPoolingQueue(data));
		}

		[TestMethod]
		public async Task SendMessageToPoolingQueue_AddRequestFail_ThrowException()
		{
			//Arrange
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();
			Mock<IHttpPoolingServices> serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Throws(new Exception());
			var creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();

			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClient.Object);
			DocumentAnalysisData data = new DocumentAnalysisData();
			data.Id = Guid.NewGuid();
			data.AnalysisProviderId = Guid.NewGuid();

			//Act Assert
			await Assert.ThrowsExceptionAsync<Exception>(async () => await hostedService.SendMessageToPoolingQueue(data));
		}		

	}
}
