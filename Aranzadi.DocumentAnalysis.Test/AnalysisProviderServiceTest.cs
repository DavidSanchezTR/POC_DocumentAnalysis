using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Test.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;

namespace Aranzadi.DocumentAnalysis.Test
{
	[TestClass]
	public class AnalysisProviderServiceTest
	{
		[TestMethod]
		public async Task SendAnalysisJob_ValidData_ReturnsOK()
		{
			//Arrange
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			AnalysisProviderClass config = new AnalysisProviderClass();
			config.ApiKey = "ApiKey";
			config.UrlApiJobs = "https://UrlApiJobs";
			documentAnalysisOptionsMock.Object.AnalysisProvider = config;

			var response = new AnalysisJobResponse();
			response.Guid = Guid.NewGuid().ToString();
			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
			{
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(JsonConvert.SerializeObject(response))
				};
			});
		
			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

			DocumentAnalysisData data = new DocumentAnalysisData();
			data.DocumentName = "prueba.zip";
			data.Id = new Guid();
			data.AccessUrl = "http://wwww.urlSasToken.es";

			//Act
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
			var result = await analysisProviderService.SendAnalysisJob(data);

			//Assert
			Assert.IsTrue(result.Item1.IsSuccessStatusCode);
		}

		[TestMethod]
		public async Task SendAnalysisJob_InvalidResponse_ThrowSerializationException()
		{
			//Arrange
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			AnalysisProviderClass config = new AnalysisProviderClass();
			config.ApiKey = "ApiKey";
			config.UrlApiJobs = "https://UrlApiJobs";
			documentAnalysisOptionsMock.Object.AnalysisProvider = config;

			var response = "Bad request To Serialize";
			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
			{
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(JsonConvert.SerializeObject(response))
				};
			});

			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

			DocumentAnalysisData data = new DocumentAnalysisData();
			data.DocumentName = "prueba.zip";
			data.Id = new Guid();
			data.AccessUrl = "http://wwww.urlSasToken.es";

			//Act
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);

			//Assert			
			await Assert.ThrowsExceptionAsync<JsonSerializationException>(async () => await analysisProviderService.SendAnalysisJob(data));

		}

		[TestMethod]
		public async Task SendAnalysisJob_EmptyResponse_ReturnNull()
		{
			//Arrange
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			AnalysisProviderClass config = new AnalysisProviderClass();
			config.ApiKey = "ApiKey";
			config.UrlApiJobs = "https://UrlApiJobs";
			documentAnalysisOptionsMock.Object.AnalysisProvider = config;

			var response = string.Empty;
			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
			{
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Content = new StringContent(JsonConvert.SerializeObject(response))
				};
			});

			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

			DocumentAnalysisData data = new DocumentAnalysisData();
			data.DocumentName = "prueba.zip";
			data.Id = new Guid();
			data.AccessUrl = "http://wwww.urlSasToken.es";

			//Act
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
			var result = await analysisProviderService.SendAnalysisJob(data);

			//Assert			
			Assert.IsNull(result.Item2);

		}

		[TestMethod]
		public async Task SendAnalysisJob_ResponseInternalServerError_ReturnNull()
		{
			//Arrange
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			AnalysisProviderClass config = new AnalysisProviderClass();
			config.ApiKey = "ApiKey";
			config.UrlApiJobs = "https://UrlApiJobs";
			documentAnalysisOptionsMock.Object.AnalysisProvider = config;

			var response = string.Empty;
			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
			{
				return new HttpResponseMessage()
				{
					StatusCode = System.Net.HttpStatusCode.InternalServerError,
					Content = new StringContent(JsonConvert.SerializeObject(response))
				};
			});

			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

			DocumentAnalysisData data = new DocumentAnalysisData();
			data.DocumentName = "prueba.zip";
			data.Id = new Guid();
			data.AccessUrl = "http://wwww.urlSasToken.es";

			//Act
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
			var result = await analysisProviderService.SendAnalysisJob(data);

			//Assert			
			Assert.IsNull(result.Item2);

		}

		[TestMethod]
		public async Task SendAnalysisJob_RetryPolicyOnce_OK()
		{
			//Arrange
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			AnalysisProviderClass config = new AnalysisProviderClass();
			config.ApiKey = "ApiKey";
			config.UrlApiJobs = "https://UrlApiJobs";
			documentAnalysisOptionsMock.Object.AnalysisProvider = config;

			var response = new AnalysisJobResponse();
			response.Guid = Guid.NewGuid().ToString();
			int errors = 0;
			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
			{
				try
				{
					if (errors == 0) //provocar un error para usar el polly retry policy
					{
						return new HttpResponseMessage()
						{							
							StatusCode = System.Net.HttpStatusCode.InternalServerError,
							Content = new StringContent(JsonConvert.SerializeObject(response))
						};
					}
					else
					{
						return new HttpResponseMessage()
						{
							StatusCode = System.Net.HttpStatusCode.OK,
							Content = new StringContent(JsonConvert.SerializeObject(response))
						};
					}
				}
				finally
				{
					errors++;
				}

			});

			Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

			DocumentAnalysisData data = new DocumentAnalysisData();
			data.DocumentName = "prueba.zip";
			data.Id = new Guid();
			data.AccessUrl = "http://wwww.urlSasToken.es";

			//Act
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
			var result = await analysisProviderService.SendAnalysisJob(data);

			//Assert
			Assert.IsTrue(result.Item1.IsSuccessStatusCode);
		}

	}
}
