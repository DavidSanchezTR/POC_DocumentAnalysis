using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Microsoft.Extensions.DependencyInjection;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Services.IServices;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	[TestClass]
	public class DocumentAnalysisServiceIntegrationTest
	{

		[TestMethod]
		public async Task GetAnalysisAsync_ValidValues_ReturnsOneAnalysis()
		{
			//Arrange
			var tenantId = "5600";
			var userId = "98";
			var documentId = Guid.NewGuid();
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
			IDocumentAnalysisService documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			var data = new DocumentAnalysisData
			{
				Id = documentId,
				App = "Fusion",
				TenantId = tenantId,
				UserId = userId,
				Analysis = null,
				Status = AnalysisStatus.Pending,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = "HashTest",
				AnalysisProviderId = null,
				AnalysisProviderResponse = "Pending"
			};

			//Act
			await documentAnalysisRepository.AddAnalysisDataAsync(data);
			var result = await documentAnalysisService.GetAnalysisAsync(tenantId, userId, documentId.ToString());

			//Assert
			Assert.IsTrue(result.Count() > 0);
		}

		[TestMethod]
		public async Task GetAnalysisAsync_ValidValues_ReturnsAllAnalysisOfTenantAndUser()
		{
			//Arrange
			var tenantId = "5600";
			var userId = "98";
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
			IDocumentAnalysisService documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			var data = new DocumentAnalysisData
			{
				Id = Guid.NewGuid(),
				App = "Fusion",
				TenantId = tenantId,
				UserId = userId,
				Analysis = null,
				Status = AnalysisStatus.Pending,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = "HashTest",
				AnalysisProviderId = null,
				AnalysisProviderResponse = "Pending"
			};
			var data1 = new DocumentAnalysisData
			{
				Id = Guid.NewGuid(),
				App = "Fusion",
				TenantId = tenantId,
				UserId = userId,
				Analysis = null,
				Status = AnalysisStatus.Pending,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = "HashTest",
				AnalysisProviderId = null,
				AnalysisProviderResponse = "Pending"
			};

			//Act
			await documentAnalysisRepository.AddAnalysisDataAsync(data);
			await documentAnalysisRepository.AddAnalysisDataAsync(data1);
			var result = await documentAnalysisService.GetAnalysisAsync(tenantId, userId);

			//Assert
			Assert.IsTrue(result.Count() > 1);
		}

		[TestMethod]
		[DataRow(null, null, DisplayName = "null values")]
		[DataRow("", "", DisplayName = "empty values")]
		[DataRow("   ", "    ", DisplayName = "whitespaces values")]
		public async Task GetAnalysisAsync_InvalidValues_ThrowArgumentNullException(string tenant, string user)
		{
			//Arrange
			var tenantId = tenant;
			var userId = user;
			var documentId = Guid.NewGuid();
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
			IDocumentAnalysisService documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			var data = new DocumentAnalysisData
			{
				Id = documentId,
				App = "Fusion",
				TenantId = tenantId,
				UserId = userId,
				Analysis = null,
				Status = AnalysisStatus.Pending,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = "HashTest",
				AnalysisProviderId = null,
				AnalysisProviderResponse = "Pending"
			};

			//Act Assert
			//await documentAnalysisRepository.AddAnalysisDataAsync(data);
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await documentAnalysisService.GetAnalysisAsync(tenantId, userId));
			
		}

	}
}
