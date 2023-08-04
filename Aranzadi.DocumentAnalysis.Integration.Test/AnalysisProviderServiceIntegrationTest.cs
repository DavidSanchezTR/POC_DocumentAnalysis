using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Integration.Test.Moq;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	[TestClass]
	public class AnalysisProviderServiceIntegrationTest
	{

		[TestMethod]
		public async Task SendAnalysisJob_SendValidDataToRealApi_OK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var httpClientFactory = AssemblyApp.app.Services.GetService<IHttpClientFactory>();

			DocumentAnalysisData data = new DocumentAnalysisData();
			data.DocumentName = "prueba.zip";
			data.Id = new Guid();
			data.AccessUrl = AssemblyApp.SasToken;

			//Act
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptions, httpClientFactory);
			var result = await analysisProviderService.SendAnalysisJob(data);

			//Assert
			Assert.IsTrue(result.Item1.IsSuccessStatusCode);
			Assert.IsTrue(result.Item2 != null);
			Assert.IsTrue(result.Item2.Guid != null);
			Assert.IsTrue(result.Item2.ExecutionStatus.State == "Pending");
		}

		[TestMethod]
		public async Task SendAnalysisJob_SendAnalysisDataIsNull_ReturnBadRequest()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var httpClientFactory = AssemblyApp.app.Services.GetService<IHttpClientFactory>();
			DocumentAnalysisData data = null;

			//Act Assert
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptions, httpClientFactory);
			await Assert.ThrowsExceptionAsync<ArgumentNullException>( async () => await analysisProviderService.SendAnalysisJob(data));			
		}

		[TestMethod]
		public async Task SendAnalysisJob_SendAnalysisWithoutData_ReturnBadRequest()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			var httpClientFactory = AssemblyApp.app.Services.GetService<IHttpClientFactory>();
			DocumentAnalysisData data = new DocumentAnalysisData();

			//Act
			var analysisProviderService = new AnalysisProviderService(documentAnalysisOptions, httpClientFactory);
			var result = await analysisProviderService.SendAnalysisJob(data);

			//Assert
			Assert.IsTrue(result.Item1.StatusCode == System.Net.HttpStatusCode.BadRequest);
			Assert.IsTrue(result.Item2 == null);
		}

	}
}
