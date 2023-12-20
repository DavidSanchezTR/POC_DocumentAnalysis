using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Microsoft.Extensions.DependencyInjection;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	[TestClass]
	public class DocumentAnalysisServiceIntegrationTest
	{

		[TestMethod]
		public async Task GetAnalysisAsync_ValidValues_ReturnsAnalysis()
		{
			//Arrange
			var tenantId = AssemblyApp.TenantId;
			var userId = AssemblyApp.UserId;
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
			var result = await documentAnalysisService.GetAnalysisAsync(tenantId, documentId.ToString());

            //Assert
            Assert.IsTrue(result.DocumentUniqueRefences == documentId.ToString());
        }

		[TestMethod]
        [DataRow(AnalysisStatus.Pending, DisplayName = "return one")]
        [DataRow(AnalysisStatus.Done, DisplayName = "return all Done")]
        [DataRow(AnalysisStatus.Unknown, DisplayName = "return all Unknown")]
        [DataRow(AnalysisStatus.DoneWithErrors, DisplayName = "return all DoneWithErrors")]
        [DataRow(AnalysisStatus.Error, DisplayName = "return all Error")]
        [DataRow(AnalysisStatus.NotFound, DisplayName = "return all NotFound")]
        public async Task GetAnalysisListAsync_ValidValues_ReturnsOneAnalysis(AnalysisStatus status)
		{
            //Arrange
            int numDocs = status switch
            {
                AnalysisStatus.Pending => 1,
                _ => 2,
            };

            string tenantId = "5600";
			string userId = "98";
			Guid documentId1 = Guid.NewGuid();
			Guid documentId2 = Guid.NewGuid();

            if (AssemblyApp.app == null) Assert.Fail();

            IDocumentAnalysisRepository? documentAnalysisRepository = AssemblyApp.app.Services.GetService<IDocumentAnalysisRepository>();
			IDocumentAnalysisService? documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();

            DocumentAnalysisData analysis1 = new DocumentAnalysisData
			{
				Id = documentId1,
				App = "Fusion",
				TenantId = tenantId,
				UserId = userId,
				Analysis = null,
				Status = AnalysisStatus.Done,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = "HashTest",
				AnalysisProviderId = null,
				AnalysisProviderResponse = "Done"
			};

            DocumentAnalysisData analysis2 = new DocumentAnalysisData
			{
				Id = documentId2,
				App = "Fusion",
				TenantId = tenantId,
				UserId = userId,
				Analysis = null,
				Status = status,
				AnalysisDate = DateTimeOffset.Now,
				CreateDate = DateTimeOffset.Now,
				Source = Source.LaLey,
				DocumentName = "test.zip",
				AccessUrl = AssemblyApp.SasToken,
				Sha256 = "HashTest",
				AnalysisProviderId = null,
				AnalysisProviderResponse = status.ToString()
			};

            //Act
			if (documentAnalysisRepository == null || documentAnalysisService == null) Assert.Fail();
            
			await documentAnalysisRepository.AddAnalysisDataAsync(analysis1);
			await documentAnalysisRepository.AddAnalysisDataAsync(analysis2);
			string documentsId = documentId1.ToString() + ";" + documentId2.ToString();
            IEnumerable<DocumentAnalysisResponse> result = await documentAnalysisService.GetAnalysisListAsync(tenantId, documentsId);

			//Assert
			Assert.IsTrue(result.Count() == numDocs);
			Assert.IsTrue(result.FirstOrDefault().DocumentUniqueRefences == documentId1.ToString());
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
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(
				async () => await documentAnalysisService.GetAnalysisAsync(tenantId, documentId.ToString()));
			
		}

		[TestMethod]
		[DataRow(null, null, DisplayName = "null values")]
		[DataRow("", "", DisplayName = "empty values")]
		[DataRow("   ", "    ", DisplayName = "whitespaces values")]
		public async Task GetAnalysisListAsync_InvalidValues_ThrowArgumentNullException(string tenantId, string documentIds)
		{
            //Arrange
            Guid documentId = Guid.NewGuid();
            if (AssemblyApp.app == null) Assert.Fail();
			IDocumentAnalysisService? documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();

            //Act
            if (documentAnalysisService == null) Assert.Fail();

            //Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await documentAnalysisService.GetAnalysisListAsync(tenantId, documentIds));
			
		}
	}
}