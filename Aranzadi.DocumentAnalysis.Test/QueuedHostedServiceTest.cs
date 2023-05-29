using Aranzadi.DocumentAnalysis.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Aranzadi.DocumentAnalysis.Test
{
	[TestClass]
	public class QueuedHostedServiceTest
	{

		[TestMethod]
		public async Task GetHashFromFile_streamValid_OK()
		{
			Mock<ILogger<QueuedHostedService>> loggerMock = new Mock<ILogger<QueuedHostedService>>();
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			var configuration = new TelemetryConfiguration();
			List<ITelemetry> sendItems = new List<ITelemetry>();
			configuration.TelemetryChannel = new TelemetryChannelMock { OnSend = item => sendItems.Add(item) };
			configuration.ConnectionString = $"InstrumentationKey={Guid.NewGuid().ToString()};IngestionEndpoint=https://test/;LiveEndpoint=https://test/";
			configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
			TelemetryClient telemetryClient = new TelemetryClient(configuration);
			//using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("cualquier cosa"));
			using FileStream stream = new FileStream(@"Resources\Test.pdf", FileMode.Open);
			var hostedService = new QueuedHostedService(loggerMock.Object, serviceProviderMock.Object, documentAnalysisOptionsMock.Object, telemetryClient);
			
			var hashString = await hostedService.GetHashFromFile(stream);

			Assert.IsNotNull(hashString);
		}

		[TestMethod]
		public async Task GetHashFromFile_RepeatGetHash_GetSameHashOK()
		{
			Mock<ILogger<QueuedHostedService>> loggerMock = new Mock<ILogger<QueuedHostedService>>();
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
			var configuration = new TelemetryConfiguration();
			List<ITelemetry> sendItems = new List<ITelemetry>();
			configuration.TelemetryChannel = new TelemetryChannelMock { OnSend = item => sendItems.Add(item) };
			configuration.ConnectionString = $"InstrumentationKey={Guid.NewGuid().ToString()};IngestionEndpoint=https://test/;LiveEndpoint=https://test/";
			configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
			TelemetryClient telemetryClient = new TelemetryClient(configuration);
			//using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("cualquier cosa"));
			using FileStream stream = new FileStream(@"Resources\Test.pdf", FileMode.Open);
			var hostedService = new QueuedHostedService(loggerMock.Object, serviceProviderMock.Object, documentAnalysisOptionsMock.Object, telemetryClient);
			
			var hashString = await hostedService.GetHashFromFile(stream);
			var hashString2 = await hostedService.GetHashFromFile(stream);

			Assert.AreEqual(hashString, hashString2);
		}

		[TestMethod]
		public async Task GetHashFromFile_StreamNull_ThrowNullReferenceException()
		{
			Mock<ILogger<QueuedHostedService>> loggerMock = new Mock<ILogger<QueuedHostedService>>();
			Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
			Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();

			var configuration = new TelemetryConfiguration();
			List<ITelemetry> sendItems = new List<ITelemetry>();
			configuration.TelemetryChannel = new TelemetryChannelMock { OnSend = item => sendItems.Add(item) };
			configuration.ConnectionString = $"InstrumentationKey={Guid.NewGuid().ToString()};IngestionEndpoint=https://test/;LiveEndpoint=https://test/";
			configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
			TelemetryClient telemetryClient = new TelemetryClient(configuration);
			using Stream? stream = null;
			var hostedService = new QueuedHostedService(loggerMock.Object, serviceProviderMock.Object, documentAnalysisOptionsMock.Object, telemetryClient);

			await Assert.ThrowsExceptionAsync<NullReferenceException>(async () => await hostedService.GetHashFromFile(stream));

		}








	}
}
