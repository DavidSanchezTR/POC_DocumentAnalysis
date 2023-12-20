using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Test.Moq;
using Aranzadi.HttpPooling;
using Aranzadi.HttpPooling.Models;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test
{
	[TestClass]
	public class AnacondaPoolingServiceTest
	{
		[TestMethod]
		public async Task CancelAnalysis_AnalysisUpdated_ReturnTrue()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			DocumentAnalysisData data = new DocumentAnalysisData()
			{
				Id = Guid.NewGuid()
			};
			serviceMock.Setup(o => o.GetAnalysisDataAsync(It.IsAny<string>())).Returns(() => { return Task.FromResult(data); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);

			Mock<ICreditsConsumptionClient> creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClient.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClient.Object);
			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Pending"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var result = await anacondaPoolingService.CancelAnalysis(response, pooledRequestDTO);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task CancelAnalysis_AnalysisNotUpdated_ReturnFalse()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(0); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);

			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Pending"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			var result = await anacondaPoolingService.CancelAnalysis(response, pooledRequestDTO);

			//Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public async Task ProcessResponseAnalysis_ResponseCode200SuceededResponseAnalysis_ReturnTrue()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			#region Mock response analysis provider
			//******* Mock response analysis provider (Anaconda)
			HttpResponseMessage responseMessage = new HttpResponseMessage();  //EDIT
			responseMessage.StatusCode = System.Net.HttpStatusCode.OK;
			responseMessage.Content = new StringContent("");
			string responseJson = "test";  //EDIT
			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();
			analysisProviderServiceMock.Setup(o => o.GetAnalysisResult(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((responseMessage, responseJson)));
			#endregion Mock response analysis provider

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			DocumentAnalysisData data = new DocumentAnalysisData()
			{
				Id = Guid.NewGuid()
			};
			serviceMock.Setup(o => o.GetAnalysisDataAsync(It.IsAny<string>())).Returns(() => { return Task.FromResult(data); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""succeeded"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			var result = await anacondaPoolingService.ProcessResponseAnalysis(response, pooledRequestDTO);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task ProcessResponseAnalysis_ResponseCode200FailedResponseAnalysis_ReturnTrue()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			DocumentAnalysisData data = new DocumentAnalysisData()
			{
				Id = Guid.NewGuid()
			};
			serviceMock.Setup(o => o.GetAnalysisDataAsync(It.IsAny<string>())).Returns(() => { return Task.FromResult(data); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Failed"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			var result = await anacondaPoolingService.ProcessResponseAnalysis(response, pooledRequestDTO);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task ProcessResponseAnalysis_ResponseCode200PendingResponseAnalysis_ThrowKeyNotFoundException()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Pending"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act Assert
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await anacondaPoolingService.ProcessResponseAnalysis(response, pooledRequestDTO));

		}

		[TestMethod]
		public async Task ProcessResponseAnalysis_ResponseCode200UnknownResponseAnalysis_ThrowKeyNotFoundException()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""NoControlado"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act Assert
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await anacondaPoolingService.ProcessResponseAnalysis(response, pooledRequestDTO));
		}

		[TestMethod]
		[DataRow(HttpStatusCode.NoContent, DisplayName = "NoContent")]
		[DataRow(HttpStatusCode.NotModified, DisplayName = "NotModified")]
		[DataRow(HttpStatusCode.BadRequest, DisplayName = "BadRequest")]
		[DataRow(HttpStatusCode.Unauthorized, DisplayName = "Unauthorized")]
		[DataRow(HttpStatusCode.NotFound, DisplayName = "NotFound")]
		[DataRow(HttpStatusCode.TooManyRequests, DisplayName = "TooManyRequests")]
		[DataRow(HttpStatusCode.InternalServerError, DisplayName = "InternalServerError")]
		public async Task ProcessResponseAnalysis_ResponseCodeErrorResponseAnalysis_ReturnTrue(HttpStatusCode code)
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			DocumentAnalysisData data = new DocumentAnalysisData()
			{
				Id = Guid.NewGuid()
			};
			serviceMock.Setup(o => o.GetAnalysisDataAsync(It.IsAny<string>())).Returns(() => { return Task.FromResult(data); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Failed"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = code
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			var result = await anacondaPoolingService.ProcessResponseAnalysis(response, pooledRequestDTO);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task Retry_ResponseCode200PendingResponseAnalysis_ReturnTrue()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Pending"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			var result = await anacondaPoolingService.Retry(response, pooledRequestDTO);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task Retry_ResponseCode200FailedResponseAnalysis_ReturnFalse()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Failed"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.OK
			};

			var creditsConsumptionClientMock = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClientMock.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClientMock.Object);
			var result = await anacondaPoolingService.Retry(response, pooledRequestDTO);

			//Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public async Task Retry_ResponseCode500_ReturnTrue()
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Failed"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = System.Net.HttpStatusCode.InternalServerError
			};

			Mock<ICreditsConsumptionClient> creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClient.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClient.Object);
			var result = await anacondaPoolingService.Retry(response, pooledRequestDTO);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		[DataRow(HttpStatusCode.TooManyRequests, DisplayName = "TooManyRequests")]
		[DataRow(HttpStatusCode.InternalServerError, DisplayName = "InternalServerError")]
		public async Task Retry_ResponseCodeError_ReturnTrue(HttpStatusCode code)
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Failed"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = code
			};

			Mock<ICreditsConsumptionClient> creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClient.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClient.Object);
			var result = await anacondaPoolingService.Retry(response, pooledRequestDTO);

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		[DataRow(HttpStatusCode.NoContent, DisplayName = "NoContent")]
		[DataRow(HttpStatusCode.NotModified, DisplayName = "NotModified")]
		[DataRow(HttpStatusCode.BadRequest, DisplayName = "BadRequest")]
		[DataRow(HttpStatusCode.Unauthorized, DisplayName = "Unauthorized")]
		[DataRow(HttpStatusCode.NotFound, DisplayName = "NotFound")]
		public async Task Retry_ResponseCodeError_ReturnFalse(HttpStatusCode code)
		{
			//Arrange
			var configurationMock = new Mock<DocumentAnalysisOptions>();
			var poolingConfigurationMock = new Mock<PoolingConfiguration>();
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();

			Mock<IAnalysisProviderService> analysisProviderServiceMock = new Mock<IAnalysisProviderService>();

			//******* Mock ServiceScope
			var serviceMock = new Mock<IDocumentAnalysisRepository>();
			DocumentAnalysisResult documentAnalysisResult = new DocumentAnalysisResult(); //EDIT
			documentAnalysisResult.Analysis = "";
			documentAnalysisResult.Status = Messaging.Model.Enums.AnalysisStatus.Done;
			documentAnalysisResult.DocumentId = Guid.NewGuid();
			documentAnalysisResult = null; //EDIT
			serviceMock.Setup(o => o.UpdateAnalysisDataAsync(It.IsAny<DocumentAnalysisData>())).Returns(() => { return Task.FromResult(1); });
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IDocumentAnalysisRepository)))
				.Returns(serviceMock.Object);
			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory
				.Setup(x => x.CreateScope())
				.Returns(serviceScope.Object);
			serviceProviderMock
				.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
				.Returns(serviceScopeFactory.Object);


			PooledRequestDTO pooledRequestDTO = new PooledRequestDTO();
			pooledRequestDTO.NIntento = 0;
			pooledRequestDTO.Request = new HttpPoolingRequest()
			{
				ExternalIdentificator = Guid.NewGuid().ToString(),
				Url = "http://loquesea"
			};

			HttpResponseMessage response = new HttpResponseMessage()
			{
				Content = new StringContent(@"{""guid"":""d54acdc5-1b98-4d13-bd52-1a3c05aef350"",""name"":""test.zip"",""sourceDocuments"":[{""clientDocumentId"":""f197cd05-3157-49e2-adc0-b2fec0cbe413"",""azureBlobUri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""uri"":""https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D"",""docType"":"""",""format"":""Auto"",""collectionId"":""default"",""statistics"":null,""executionStatus"":{""state"":""Pending"",""errors"":null}}],""executionStatus"":{""state"":""Failed"",""errors"":null},""links"":null,""resourceType"":""Document"",""features"":null,""tasks"":[{""type"":""DefaultDocumentAnalysis"",""parameters"":{}}]}"),
				StatusCode = code
			};

			Mock<ICreditsConsumptionClient> creditsConsumptionClient = new Mock<ICreditsConsumptionClient>();
			var operationResult = new OperationResult()
			{
				Code = OperationResult.ResultCode.Success
			};
			creditsConsumptionClient.Setup(x => x.CompleteReservation(It.IsAny<string>(), It.IsAny<string>())).Returns(() => { return Task.FromResult(operationResult); });

			//Act
			var anacondaPoolingService = new AnacondaPoolingService(configurationMock.Object, poolingConfigurationMock.Object, httpClientFactoryMock.Object, analysisProviderServiceMock.Object, serviceProviderMock.Object, creditsConsumptionClient.Object);
			var result = await anacondaPoolingService.Retry(response, pooledRequestDTO);

			//Assert
			Assert.IsFalse(result);
		}

	}
}
