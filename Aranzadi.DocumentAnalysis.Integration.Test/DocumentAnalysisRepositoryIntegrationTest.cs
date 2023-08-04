using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Services;
using Microsoft.Extensions.DependencyInjection;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Azure.Core;
using Aranzadi.DocumentAnalysis.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Aranzadi.DocumentAnalysis.Data.IRepository;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	[TestClass]
	public class DocumentAnalysisRepositoryIntegrationTest
	{

		[TestMethod]
		public async Task AddAnalysisDataAsync_ValidValues_ReturnsOK()
		{
			//Arrange
			DocumentAnalysisDbContext context = AssemblyApp.app.Services.GetService<DocumentAnalysisDbContext>();
			DocumentAnalysisRepository documentAnalysisRepository = new DocumentAnalysisRepository(context);
			var data = new DocumentAnalysisData
			{
				Id = Guid.NewGuid(),
				App = "Fusion",
				TenantId = "5600",
				UserId = "98",
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
			var result = await documentAnalysisRepository.AddAnalysisDataAsync(data);

			//Assert
			Assert.AreEqual(result, 1);
		}

		[TestMethod]
		public async Task AddAnalysisDataAsync_ConnectionStringInvalid_ThrowException()
		{
			//Arrange	
			string connectionString = AssemblyApp.documentAnalysisOptions.ConnectionStrings.DefaultConnection;
			connectionString = "connectionString no valida";
			var contextOptions = new DbContextOptionsBuilder<DocumentAnalysisDbContext>()
									.UseCosmos(connectionString
											, AssemblyApp.documentAnalysisOptions.CosmosDatabaseName ?? "documentAnalisis"
											, (cosmosOptions) => { cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(5)); })
									.Options;
			DocumentAnalysisDbContext context = new DocumentAnalysisDbContext(contextOptions);

			DocumentAnalysisRepository documentAnalysisRepository = new DocumentAnalysisRepository(context);
			var data = new DocumentAnalysisData
			{
				Id = Guid.NewGuid(),
				App = "Fusion",
				TenantId = "5600",
				UserId = "98",
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
			await Assert.ThrowsExceptionAsync<DbUpdateException>(async () => await documentAnalysisRepository.AddAnalysisDataAsync(data));
		}

		[TestMethod]
		public async Task UpdateAnalysisDataAsync_ValidValues_ReturnsOK()
		{
			//Arrange
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
			var data = new DocumentAnalysisData
			{
				Id = Guid.NewGuid(),
				App = "Fusion",
				TenantId = "5600",
				UserId = "98",
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
			var result = await documentAnalysisRepository.AddAnalysisDataAsync(data);
			data.AnalysisProviderResponse = "UPDATE";
			result = await documentAnalysisRepository.UpdateAnalysisDataAsync(data);

			//Assert
			Assert.AreEqual(result, 1);
		}

		[TestMethod]
		public async Task GetAnalysisDoneAsync_ValidValues_ReturnsOK()
		{
			//Arrange
			var randomHash = Guid.NewGuid().ToString();
			AnalysisStatus statusAnalysis = AnalysisStatus.Done;
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
			var data = new DocumentAnalysisData
			{
				Id = Guid.NewGuid(),
				App = "Fusion",
				TenantId = "5600",
				UserId = "98",
				Analysis = null,
				Status = statusAnalysis,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = randomHash,
				AnalysisProviderId = null,
				AnalysisProviderResponse = "Pending"
			};

			//Act
			await documentAnalysisRepository.AddAnalysisDataAsync(data);
			var result = await documentAnalysisRepository.GetAnalysisDoneAsync(randomHash);

			//Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task GetAnalysisDoneAsync_ValidValues_ReturnsNull()
		{
			//Arrange
			var randomHash = Guid.NewGuid().ToString();
			AnalysisStatus statusAnalysis = AnalysisStatus.Pending;
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
			var data = new DocumentAnalysisData
			{
				Id = Guid.NewGuid(),
				App = "Fusion",
				TenantId = "5600",
				UserId = "98",
				Analysis = null,
				Status = statusAnalysis,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = randomHash,
				AnalysisProviderId = null,
				AnalysisProviderResponse = "Pending"
			};

			//Act
			await documentAnalysisRepository.AddAnalysisDataAsync(data);
			var result = await documentAnalysisRepository.GetAnalysisDoneAsync(randomHash);

			//Assert
			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task GetAnalysisAsync_ValidValues_ReturnsOneAnalysis()
		{
			//Arrange
			var tenantId = "5600";
			var userId = "98";
			var documentId = Guid.NewGuid();
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
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
			var result = await documentAnalysisRepository.GetAnalysisAsync(tenantId, userId, documentId.ToString());

			//Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task GetAnalysisAsync_ValidValues_ReturnsAnalysis()
		{
			//Arrange
			var tenantId = "5600";
			var userId = "98";
			IDocumentAnalysisRepository documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
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
			var result = await documentAnalysisRepository.GetAnalysisAsync(tenantId, userId);

			//Assert
			Assert.IsTrue(result.Count() > 1);
		}

	}
}
