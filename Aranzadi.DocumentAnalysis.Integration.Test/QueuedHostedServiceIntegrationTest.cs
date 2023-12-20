using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Aranzadi.DocumentAnalysis.Integration.Test.Moq;
using Newtonsoft.Json;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.Data;
using System.Configuration;
using System.Data;
using System.Reflection.Metadata;
using Aranzadi.HttpPooling.Interfaces;
using Aranzadi.HttpPooling.Models;
using Aranzadi.DocumentAnalysis.Models.CreditConsumption;
using Aranzadi.DocumentAnalysis.Models.CreditReservations;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	[TestClass]
	public class QueuedHostedServiceIntegrationTest
	{

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{

		}

		[ClassCleanup]
		public static void ClassCleanup()
		{

		}

		[TestMethod]
		public async Task ProcessMessage_AllReal_OK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var serviceProvider = AssemblyApp.app.Services.GetService<IServiceProvider>();
			var configuration = documentAnalysisOptions;
			var httpClientFactory = AssemblyApp.app.Services.GetService<IHttpClientFactory>();
			var analysisProviderService = AssemblyApp.app.Services.GetService<IAnalysisProviderService>();
			var logAnalysisService = AssemblyApp.app.Services.GetService<ILogAnalysis>();
			var serviceBusPoolingService = AssemblyApp.app.Services.GetService<IHttpPoolingServices>();
			await serviceBusPoolingService.Start();
			//var creditsConsumptionClient = AssemblyApp.app.Services.GetService<ICreditsConsumptionClient>();
			#region Mock CreditsConsumption
			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResultCompleteReservation = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultCompleteReservation); });

			var operationResultGetTenantCredits = new OperationResult<CreditResponse>()
			{
				Code = OperationResult.ResultCode.Success,
				Result = new CreditResponse()
				{
					Count = 5,
					TenantCredits = new List<TenantCredit>()
					{
						new TenantCredit()
						{
							Id = Guid.NewGuid().ToString(),
							CreditTemplateId = "analisisdocumental",
							TenantId = AssemblyApp.TenantId,
							FreeReservations = 5
						}
					}
				}
			};
			creditsConsumptionClientMock.Setup(x => x.GetTenantCredits(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultGetTenantCredits); });

			var operationResultCreateReservation = new OperationResult<TenantCreditReservation>()
			{
				Code = OperationResult.ResultCode.Success,

				Result = new TenantCreditReservation()
				{
					Completed = false,
					CreditTemplateID = Guid.NewGuid().ToString(),
					ExpiresAt = DateTime.UtcNow.AddMinutes(5),
					ID = Guid.NewGuid().ToString(),
					TenantCreditID = Guid.NewGuid().ToString(),
					UserID = AssemblyApp.UserId
				}
			};
			creditsConsumptionClientMock.Setup(x => x.CreateReservation((It.IsAny<NewTenantCreditReservation>()))).Returns(() => { return Task.FromResult(operationResultCreateReservation); });
			#endregion Mock CreditsConsumption

			var hostedService = new QueuedHostedService(serviceProvider, configuration, httpClientFactory, analysisProviderService, logAnalysisService, serviceBusPoolingService, creditsConsumptionClientMock.Object);
			IDocumentAnalysisService documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			var tenant = AssemblyApp.TenantId;
			var owner = AssemblyApp.UserId;
			var documentId = Guid.NewGuid().ToString();
			AnalysisContext analysisContext = new AnalysisContext()
			{
				Account = tenant,
				App = "Fusion",
				Owner = owner,
				Tenant = tenant
			};
			DocumentAnalysisRequest documentAnalysisRequest = new DocumentAnalysisRequest()
			{
				Guid = documentId,
				Name = "prueba.zip",
				Path = AssemblyApp.SasToken,
                AnalysisType = Messaging.Model.Enums.AnalysisTypes.Undefined
            };

			//Act
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);
            Thread.Sleep(5000);
            var analysis = await documentAnalysisService.GetAnalysisAsync(tenant, documentId);

			//Assert
			Assert.IsTrue(result);
            Assert.IsTrue(analysis.DocumentUniqueRefences == documentId);
            //Assert.IsTrue(analysis.First().Status == Messaging.Model.Enums.AnalysisStatus.Done);
            //Este test choca con el test de sistema, ya que comparte la cola de pooling, y el mensaje que deja en la cola
            //lo procesa el servicio de DAS mock. Puede en algunos casos dar un comportamiento erroneo
        }

		[TestMethod()]
		public async Task ProcessMessage_DBCosmosReal_OK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var serviceProvider = AssemblyApp.app.Services.GetService<IServiceProvider>();
			var configuration = documentAnalysisOptions;
			var httpClientFactory = AssemblyApp.app.Services.GetService<IHttpClientFactory>();
			var analysisProviderService = AssemblyApp.app.Services.GetService<IAnalysisProviderService>();
			var logAnalysisService = new Mock<ILogAnalysis>();

			#region Mock CreditsConsumption
			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResultCompleteReservation = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultCompleteReservation); });

			var operationResultGetTenantCredits = new OperationResult<CreditResponse>()
			{
				Code = OperationResult.ResultCode.Success,
				Result = new CreditResponse()
				{
					Count = 5,
					TenantCredits = new List<TenantCredit>()
					{
						new TenantCredit()
						{
							Id = Guid.NewGuid().ToString(),
							CreditTemplateId = "analisisdocumental",
							TenantId = AssemblyApp.TenantId,
							FreeReservations = 5
						}
					}
				}
			};
			creditsConsumptionClientMock.Setup(x => x.GetTenantCredits(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultGetTenantCredits); });

			var operationResultCreateReservation = new OperationResult<TenantCreditReservation>()
			{
				Code = OperationResult.ResultCode.Success,

				Result = new TenantCreditReservation()
				{
					Completed = false,
					CreditTemplateID = Guid.NewGuid().ToString(),
					ExpiresAt = DateTime.UtcNow.AddMinutes(5),
					ID = Guid.NewGuid().ToString(),
					TenantCreditID = Guid.NewGuid().ToString(),
					UserID = AssemblyApp.UserId
				}
			};
			creditsConsumptionClientMock.Setup(x => x.CreateReservation((It.IsAny<NewTenantCreditReservation>()))).Returns(() => { return Task.FromResult(operationResultCreateReservation); });
			#endregion Mock CreditsConsumption

			var serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);

			#region Mock response Sas token
			//******* Mock response Sas token
			var response = AssemblyApp.SasToken;
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
			#endregion Mock response Sas token

			#region Mock response analysis provider (Anaconda) 
			//******* Mock response analysis provider (Anaconda)
			HttpResponseMessage responseMessage = new HttpResponseMessage();  //EDIT
			responseMessage.StatusCode = System.Net.HttpStatusCode.OK;
			responseMessage.Content = new StringContent("");
			AnalysisJobResponse responseJob = new AnalysisJobResponse();  //EDIT
			responseJob.Guid = Guid.NewGuid().ToString();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			analysisProviderServiceMock.Setup(o => o.SendAnalysisJob(It.IsAny<DocumentAnalysisData>()))
					.Returns(Task.FromResult((responseMessage, responseJob)));
			#endregion Mock response analysis provider (Anaconda) 

			AnalysisContext analysisContext = new AnalysisContext()
			{
				Account = AssemblyApp.TenantId,
				App = "Fusion",
				Owner = AssemblyApp.UserId,
				Tenant = AssemblyApp.TenantId
			};
			DocumentAnalysisRequest documentAnalysisRequest = new DocumentAnalysisRequest()
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "prueba.zip",
				Path = "https://dev.infolexnube.es/Communication/GetDocument?path=&requireSessionAlive=false&valToken=792-DB-97a515c3bf3a4c20aeafbb8a8b0c60ca-sTAmBnO%2BX8Y1GUJEl5XiSw%3D%3D",
                AnalysisType = Messaging.Model.Enums.AnalysisTypes.Undefined
            };

			//Act
			var hostedService = new QueuedHostedService(serviceProvider, configuration, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClientMock.Object);
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task ProcessMessage_SasTokenReal_OK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var configuration = documentAnalysisOptions;
			var httpClientFactory = AssemblyApp.app.Services.GetService<IHttpClientFactory>();
			var logAnalysisService = new Mock<ILogAnalysis>();
			var serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);

			#region Mock CreditsConsumption
			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResultCompleteReservation = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultCompleteReservation); });

			var operationResultGetTenantCredits = new OperationResult<CreditResponse>()
			{
				Code = OperationResult.ResultCode.Success,
				Result = new CreditResponse()
				{
					Count = 5,
					TenantCredits = new List<TenantCredit>()
					{
						new TenantCredit()
						{
							Id = Guid.NewGuid().ToString(),
							CreditTemplateId = "analisisdocumental",
							TenantId = AssemblyApp.TenantId,
							FreeReservations = 5
						}
					}
				}
			};
			creditsConsumptionClientMock.Setup(x => x.GetTenantCredits(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultGetTenantCredits); });

			var operationResultCreateReservation = new OperationResult<TenantCreditReservation>()
			{
				Code = OperationResult.ResultCode.Success,

				Result = new TenantCreditReservation()
				{
					Completed = false,
					CreditTemplateID = Guid.NewGuid().ToString(),
					ExpiresAt = DateTime.UtcNow.AddMinutes(5),
					ID = Guid.NewGuid().ToString(),
					TenantCreditID = Guid.NewGuid().ToString(),
					UserID = AssemblyApp.UserId
				}
			};
			creditsConsumptionClientMock.Setup(x => x.CreateReservation((It.IsAny<NewTenantCreditReservation>()))).Returns(() => { return Task.FromResult(operationResultCreateReservation); });
			#endregion Mock CreditsConsumption

			#region Mock ServiceScope
			//******* Mock ServiceScope
			var service = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			service.Setup(o => o.GetAnalysisDoneAsync(It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResult));
			var serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(service.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);
			#endregion Mock ServiceScope

			#region Mock response analysis provider (Anaconda) 
			//******* Mock response analysis provider (Anaconda)
			HttpResponseMessage responseMessage = new HttpResponseMessage();  //EDIT
			responseMessage.StatusCode = System.Net.HttpStatusCode.OK;
			responseMessage.Content = new StringContent("");
			AnalysisJobResponse responseJob = new AnalysisJobResponse();  //EDIT
			responseJob.Guid = Guid.NewGuid().ToString();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			analysisProviderServiceMock.Setup(o => o.SendAnalysisJob(It.IsAny<DocumentAnalysisData>()))
				.Returns(Task.FromResult((responseMessage, responseJob)));
			#endregion Mock response analysis provider (Anaconda) 

			AnalysisContext analysisContext = new AnalysisContext()
			{
				Account = AssemblyApp.TenantId,
				App = "Fusion",
				Owner = AssemblyApp.UserId,
				Tenant = AssemblyApp.TenantId
			};
			DocumentAnalysisRequest documentAnalysisRequest = new DocumentAnalysisRequest()
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "prueba.zip",
				Path = AssemblyApp.SasToken,
                AnalysisType = Messaging.Model.Enums.AnalysisTypes.Undefined
            };

			//Act
			var hostedService = new QueuedHostedService(serviceProviderMock.Object, configuration, httpClientFactory, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClientMock.Object);
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task ProcessMessage_SasTokenRealUrlErronea_ReturnTrue()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var configuration = documentAnalysisOptions;
			var httpClientFactory = AssemblyApp.app.Services.GetService<IHttpClientFactory>();
			var logAnalysisService = new Mock<ILogAnalysis>();
			var serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);

			#region Mock CreditsConsumption
			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResultCompleteReservation = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultCompleteReservation); });

			var operationResultGetTenantCredits = new OperationResult<CreditResponse>()
			{
				Code = OperationResult.ResultCode.Success,
				Result = new CreditResponse()
				{
					Count = 5,
					TenantCredits = new List<TenantCredit>()
					{
						new TenantCredit()
						{
							Id = Guid.NewGuid().ToString(),
							CreditTemplateId = "analisisdocumental",
							TenantId = AssemblyApp.TenantId,
							FreeReservations = 5
						}
					}
				}
			};
			creditsConsumptionClientMock.Setup(x => x.GetTenantCredits(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultGetTenantCredits); });

			var operationResultCreateReservation = new OperationResult<TenantCreditReservation>()
			{
				Code = OperationResult.ResultCode.Success,

				Result = new TenantCreditReservation()
				{
					Completed = false,
					CreditTemplateID = Guid.NewGuid().ToString(),
					ExpiresAt = DateTime.UtcNow.AddMinutes(5),
					ID = Guid.NewGuid().ToString(),
					TenantCreditID = Guid.NewGuid().ToString(),
					UserID = AssemblyApp.UserId
				}
			};
			creditsConsumptionClientMock.Setup(x => x.CreateReservation((It.IsAny<NewTenantCreditReservation>()))).Returns(() => { return Task.FromResult(operationResultCreateReservation); });
			#endregion Mock CreditsConsumption

			#region Mock ServiceScope
			//******* Mock ServiceScope
			var service = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			service.Setup(o => o.GetAnalysisDoneAsync(It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResult));
			var serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(service.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);
			#endregion Mock ServiceScope

			#region Mock response analysis provider (Anaconda) 
			//******* Mock response analysis provider (Anaconda)
			HttpResponseMessage responseMessage = new HttpResponseMessage();  //EDIT
			responseMessage.StatusCode = System.Net.HttpStatusCode.OK;
			responseMessage.Content = new StringContent("");
			AnalysisJobResponse responseJob = new AnalysisJobResponse();  //EDIT
			responseJob.Guid = Guid.NewGuid().ToString();
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			analysisProviderServiceMock.Setup(o => o.SendAnalysisJob(It.IsAny<DocumentAnalysisData>()))
				.Returns(Task.FromResult((responseMessage, responseJob)));
			#endregion Mock response analysis provider (Anaconda) 

			AnalysisContext analysisContext = new AnalysisContext()
			{
				Account = AssemblyApp.TenantId,
				App = "Fusion",
				Owner = AssemblyApp.UserId,
				Tenant = AssemblyApp.TenantId
			};
			DocumentAnalysisRequest documentAnalysisRequest = new DocumentAnalysisRequest()
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "prueba.zip",
				Path = AssemblyApp.SasToken + "invalidUrl",
                AnalysisType = Messaging.Model.Enums.AnalysisTypes.Undefined
            };

			//Act Assert
			var hostedService = new QueuedHostedService(serviceProviderMock.Object, configuration, httpClientFactory, analysisProviderServiceMock.Object, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClientMock.Object);
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);
			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task ProcessMessage_AnalysisProviderApiReal_OK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var configuration = documentAnalysisOptions;
			var analysisProviderService = AssemblyApp.app.Services.GetService<IAnalysisProviderService>();
			var logAnalysisService = new Mock<ILogAnalysis>();
			var serviceBusPoolingService = new Mock<IHttpPoolingServices>();
			serviceBusPoolingService.Setup(o => o.AddRequest(It.IsAny<HttpPoolingRequest>())).Returns(Task.CompletedTask);
			
			#region Mock CreditsConsumption
			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResultCompleteReservation = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultCompleteReservation); });

			var operationResultGetTenantCredits = new OperationResult<CreditResponse>()
			{
				Code = OperationResult.ResultCode.Success,
				Result = new CreditResponse()
				{
					Count = 5,
					TenantCredits = new List<TenantCredit>()
					{
						new TenantCredit()
						{
							Id = Guid.NewGuid().ToString(),
							CreditTemplateId = "analisisdocumental",
							TenantId = AssemblyApp.TenantId,
							FreeReservations = 5
						}
					}
				}
			};
			creditsConsumptionClientMock.Setup(x => x.GetTenantCredits(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResultGetTenantCredits); });

			var operationResultCreateReservation = new OperationResult<TenantCreditReservation>()
			{
				Code = OperationResult.ResultCode.Success,

				Result = new TenantCreditReservation()
				{
					Completed = false,
					CreditTemplateID = Guid.NewGuid().ToString(),
					ExpiresAt = DateTime.UtcNow.AddMinutes(5),
					ID = Guid.NewGuid().ToString(),
					TenantCreditID = Guid.NewGuid().ToString(),
					UserID = AssemblyApp.UserId
				}
			};
			creditsConsumptionClientMock.Setup(x => x.CreateReservation((It.IsAny<NewTenantCreditReservation>()))).Returns(() => { return Task.FromResult(operationResultCreateReservation); });
			#endregion Mock CreditsConsumption

			#region Mock ServiceScope
			//******* Mock ServiceScope
			var service = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			service.Setup(o => o.GetAnalysisDoneAsync(It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResult));
			var serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(service.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);
			#endregion Mock ServiceScope

			#region Mock response Sas token
			//******* Mock response Sas token
			var response = AssemblyApp.SasToken;
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
			#endregion Mock response Sas token

			AnalysisContext analysisContext = new AnalysisContext()
			{
				Account = AssemblyApp.TenantId,
				App = "Fusion",
				Owner = AssemblyApp.UserId,
				Tenant = AssemblyApp.TenantId
			};
			DocumentAnalysisRequest documentAnalysisRequest = new DocumentAnalysisRequest()
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "prueba.zip",
				Path = AssemblyApp.SasToken,
                AnalysisType = Messaging.Model.Enums.AnalysisTypes.Undefined
            };

			//Act
			var hostedService = new QueuedHostedService(serviceProviderMock.Object, configuration, httpClientFactoryMock.Object, analysisProviderService, logAnalysisService.Object, serviceBusPoolingService.Object, creditsConsumptionClientMock.Object);
			var result = await hostedService.ProcessMessage(analysisContext, documentAnalysisRequest);

			//Assert
			Assert.IsTrue(result);
		}

	}
}