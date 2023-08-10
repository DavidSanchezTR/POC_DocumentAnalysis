using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Test.Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test
{
	[TestClass]
	public class QueuedHostedServiceTest
	{

		[TestMethod]
		public async Task GetHashFromFile_streamValid_OK()
		{
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

			using FileStream stream = new FileStream(@"Resources\Test.pdf", FileMode.Open);
			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);
			
			var hashString = await hostedService.GetHashFromFile(stream);

			Assert.IsNotNull(hashString);
		}

		[TestMethod]
		public async Task GetHashFromFile_RepeatGetHash_GetSameHashOK()
		{
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

			using FileStream stream = new FileStream(@"Resources\Test.pdf", FileMode.Open);
			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);
			
			var hashString = await hostedService.GetHashFromFile(stream);
			var hashString2 = await hostedService.GetHashFromFile(stream);

			Assert.AreEqual(hashString, hashString2);
		}

		[TestMethod]
		public async Task GetHashFromFile_StreamNull_ThrowNullReferenceException()
		{
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

			using Stream? stream = null;
			var hostedService = new QueuedHostedService(serviceProviderMock.Object, documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);

			await Assert.ThrowsExceptionAsync<NullReferenceException>(async () => await hostedService.GetHashFromFile(stream));

		}

		[TestMethod]
		public async Task ProccessMessage_OK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();
			documentAnalysisOptions.CheckIfExistsHashFileInCosmos = false; //EDIT

			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

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
			var hostedService = new QueuedHostedService(serviceProvider.Object, documentAnalysisOptions, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);
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
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D"
			};


			//Act
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);


			//Assert
			Assert.IsTrue(result);

		}

		[TestMethod]
		public async Task ProccessMessage_GetSasTokenInternalServerError_ThrowHttpRequestException()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();
			documentAnalysisOptions.CheckIfExistsHashFileInCosmos = true; //EDIT

			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

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
					StatusCode = System.Net.HttpStatusCode.InternalServerError, //EDIT
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
			var hostedService = new QueuedHostedService(serviceProvider.Object, documentAnalysisOptions, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);
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
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D"
			};

			//Act Assert
			await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest));
			
		}

		[TestMethod]
		public async Task ProccessMessage_CheckIfExistsHashFileInCosmos_AnalysisNotExistsAndGetNewAnalysis()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();
			documentAnalysisOptions.CheckIfExistsHashFileInCosmos = true; //EDIT

			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

			//******* Mock ServiceScope
			var service = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = null; //EDIT
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
			var hostedService = new QueuedHostedService(serviceProvider.Object, documentAnalysisOptions, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);
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
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D"
			};


			//Act
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);


			//Assert
			Assert.IsTrue(result);

		}

		[TestMethod]
		public async Task ProccessMessage_CheckIfExistsHashFileInCosmos_AnalysisExistsAndGetExistingAnalysis()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();
			documentAnalysisOptions.CheckIfExistsHashFileInCosmos = true; //EDIT

			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

			//******* Mock ServiceScope
			var service = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "Existing analysis in Cosmos";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
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
			var hostedService = new QueuedHostedService(serviceProvider.Object, documentAnalysisOptions, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);
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
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D"
			};


			//Act
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);


			//Assert
			Assert.IsTrue(result);

		}

		[TestMethod]
		public async Task ProccessMessage_ErrorResponseFromAnalysisProvider_ThrowInternalServerError()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();
			documentAnalysisOptions.CheckIfExistsHashFileInCosmos = false; //EDIT

			Mock<ILogAnalysis> logAnalysisService = new Mock<ILogAnalysis>();

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
			responseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
			responseMessage.Content = new StringContent("");
			AnalysisJobResponse responseJob = new AnalysisJobResponse();  //EDIT
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			analysisProviderServiceMock.Setup(o => o.SendAnalysisJob(It.IsAny<DocumentAnalysisData>()))
				.Returns(Task.FromResult((responseMessage, responseJob)));

			using Stream? stream = null;
			var hostedService = new QueuedHostedService(serviceProvider.Object, documentAnalysisOptions, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object);
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
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D"
			};


			//Act
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);


			//Assert
			Assert.IsTrue(result);

		}

	}
}
