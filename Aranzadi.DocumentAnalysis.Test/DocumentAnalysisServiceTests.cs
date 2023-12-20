using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;

namespace Aranzadi.DocumentAnalysis.Test
{
	[TestClass]
	public class DocumentAnalysisServiceTests
	{
		[TestMethod]
		public async System.Threading.Tasks.Task GetAnalysisAsync_StringEmptyNotifications_ReturnsException()
		{
			DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultNotifications();
            
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResult);

			var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync("", Guid.Empty.ToString()), "Debería haber lanzado una excepción  por parametro vacío");
		}

		[TestMethod]
		public async System.Threading.Tasks.Task GetAnalysisAsync_StringEmptyDemands_ReturnsException()
		{
			DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultDemands();
            
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResult);

			var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync("", Guid.Empty.ToString()), "Debería haber lanzado una excepción  por parametro vacío");
		}

		[TestMethod]
		public async System.Threading.Tasks.Task GetAnalysisListAsync_StringEmptyNotifications_ReturnsException()
		{
			DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultNotifications();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>
            {
                documentAnalysisResult
            };

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryListOKMock(documentAnalysisResultList);
            DocumentAnalysisService documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisListAsync(string.Empty, Guid.Empty.ToString()), "Debería haber lanzado una excepción  por parametro vacío");
		}
        
		[TestMethod]
		public async System.Threading.Tasks.Task GetAnalysisListAsync_StringEmptyDemands_ReturnsException()
		{
			DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultDemands();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>
            {
                documentAnalysisResult
            };

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryListOKMock(documentAnalysisResultList);
            DocumentAnalysisService documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisListAsync(string.Empty, Guid.Empty.ToString()), "Debería haber lanzado una excepción  por parametro vacío");
		}

        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisAsync_ValidDocumentIdNotifications_ReturnsDocumentAnalysisResult()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultNotifications();
            
            DocumentAnalysisResponse documentAnalysisResponse = GetDocumentAnalysisResponse(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResult);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            var result = await documentAnalysisService.GetAnalysisAsync("122", documentAnalysisResponse.DocumentUniqueRefences);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.DocumentUniqueRefences, documentAnalysisResponse.DocumentUniqueRefences);
        }
        
        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisAsync_ValidDocumentIdDemands_ReturnsDocumentAnalysisResult()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultDemands();
            
            DocumentAnalysisResponse documentAnalysisResponse = GetDocumentAnalysisResponse(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResult);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            var result = await documentAnalysisService.GetAnalysisAsync("122", documentAnalysisResponse.DocumentUniqueRefences);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.DocumentUniqueRefences, documentAnalysisResponse.DocumentUniqueRefences);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisAsync_InvalidDocumentIdNotifications_ReturnsEmty()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultNotifications();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            DocumentAnalysisResponse documentAnalysisResponse = GetDocumentAnalysisResponse(documentAnalysisResult);
            List<DocumentAnalysisResponse> documentAnalysisResponseList = new List<DocumentAnalysisResponse>();
            documentAnalysisResponseList.Add(documentAnalysisResponse);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryEmtyMock();

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            var result = await documentAnalysisService.GetAnalysisAsync("122", "22");
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.DocumentUniqueRefences, "22");
        }
        
        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisAsync_InvalidDocumentIdDemands_ReturnsEmty()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultDemands();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            DocumentAnalysisResponse documentAnalysisResponse = GetDocumentAnalysisResponse(documentAnalysisResult);
            List<DocumentAnalysisResponse> documentAnalysisResponseList = new List<DocumentAnalysisResponse>();
            documentAnalysisResponseList.Add(documentAnalysisResponse);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryEmtyMock();

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            var result = await documentAnalysisService.GetAnalysisAsync("122", "22");
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.DocumentUniqueRefences, "22");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisListAsync_ValidValuesNotifications_ReturnsDocumentAnalysisResultList()
        {
            DocumentAnalysisResult documentAnalysisResult1 = GetDocumentAnalysisResultNotifications();
            DocumentAnalysisResult documentAnalysisResult2 = GetDocumentAnalysisResultNotifications();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>
            {
                documentAnalysisResult1,
                documentAnalysisResult2
            };

            DocumentAnalysisResponse documentAnalysisResponse1 = GetDocumentAnalysisResponse(documentAnalysisResult1);
            DocumentAnalysisResponse documentAnalysisResponse2 = GetDocumentAnalysisResponse(documentAnalysisResult2);
            List<DocumentAnalysisResponse> documentAnalysisResponseList = new List<DocumentAnalysisResponse>
            {
                documentAnalysisResponse1,
                documentAnalysisResponse2
            };

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryListOKMock(documentAnalysisResultList);
            DocumentAnalysisService documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            IEnumerable<DocumentAnalysisResponse> result = await documentAnalysisService.GetAnalysisListAsync("122",
                string.Join(";", documentAnalysisResponseList.Select(x => x.DocumentUniqueRefences)));
            Assert.AreEqual(documentAnalysisResponseList.Count, result.Count());
            Assert.AreEqual(documentAnalysisResponseList[0].DocumentUniqueRefences, result.FirstOrDefault().DocumentUniqueRefences);
            CollectionAssert.AreEqual(documentAnalysisResponseList, result.ToList());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisListAsync_ValidValuesDemands_ReturnsDocumentAnalysisResultList()
        {
            DocumentAnalysisResult documentAnalysisResult1 = GetDocumentAnalysisResultDemands();
            DocumentAnalysisResult documentAnalysisResult2 = GetDocumentAnalysisResultDemands();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>
            {
                documentAnalysisResult1,
                documentAnalysisResult2
            };

            DocumentAnalysisResponse documentAnalysisResponse1 = GetDocumentAnalysisResponse(documentAnalysisResult1);
            DocumentAnalysisResponse documentAnalysisResponse2 = GetDocumentAnalysisResponse(documentAnalysisResult2);
            List<DocumentAnalysisResponse> documentAnalysisResponseList = new List<DocumentAnalysisResponse>
            {
                documentAnalysisResponse1,
                documentAnalysisResponse2
            };

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryListOKMock(documentAnalysisResultList);
            DocumentAnalysisService documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            IEnumerable<DocumentAnalysisResponse> result = await documentAnalysisService.GetAnalysisListAsync("122",
                string.Join(";", documentAnalysisResponseList.Select(x => x.DocumentUniqueRefences)));
            Assert.AreEqual(documentAnalysisResponseList.Count, result.Count());
            Assert.AreEqual(documentAnalysisResponseList[0].DocumentUniqueRefences, result.FirstOrDefault().DocumentUniqueRefences);
            CollectionAssert.AreEqual(documentAnalysisResponseList, result.ToList());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisListAsync_InvalidValuesNotifications_ReturnsEmptyList()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultNotifications();
            DocumentAnalysisResponse documentAnalysisResponse = GetDocumentAnalysisResponse(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryListEmptyListMock();
            DocumentAnalysisService documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            IEnumerable<DocumentAnalysisResponse> result = await documentAnalysisService.GetAnalysisListAsync("122", string.Empty);
            Assert.AreEqual(0, result.Count());
        }
        
        [TestMethod]
        public async System.Threading.Tasks.Task GetAnalysisListAsync_InvalidValuesDemands_ReturnsEmptyList()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResultDemands();
            DocumentAnalysisResponse documentAnalysisResponse = GetDocumentAnalysisResponse(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryListEmptyListMock();
            DocumentAnalysisService documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object);

            IEnumerable<DocumentAnalysisResponse> result = await documentAnalysisService.GetAnalysisListAsync("122", string.Empty);
            Assert.AreEqual(0, result.Count());
        }

        #region Métodos privados
		private DocumentAnalysisResponse GetDocumentAnalysisResponse(DocumentAnalysisResult documentAnalysisResult)
		{
			JudicialNotification analisisContentResponse = JsonConvert.DeserializeObject<JudicialNotification>(documentAnalysisResult.Analysis);
			return new DocumentAnalysisResponse
			{
				DocumentUniqueRefences = documentAnalysisResult.DocumentId.ToString(),
				Result = analisisContentResponse,
				Status = documentAnalysisResult.Status,
			};
		}

        private DocumentAnalysisResult GetDocumentAnalysisResultNotifications()
		{
			string json = JsonConvert.SerializeObject(new JudicialNotification(new JudicialNotificationProvider(new DocumentAnalysisAnaconda())));

			return new DocumentAnalysisResult
			{
				Analysis = json,
				DocumentId = Guid.NewGuid(),
				Status = AnalysisStatus.Pending
			};
		}

        private DocumentAnalysisResult GetDocumentAnalysisResultDemands()
		{
			string json = JsonConvert.SerializeObject(new AppealsNotification(new AppealsProvider(new DocumentAnalysisAnaconda())));

			return new DocumentAnalysisResult
			{
				Analysis = json,
				DocumentId = Guid.NewGuid(),
				Status = AnalysisStatus.Pending
			};
		}

        private Mock<IDocumentAnalysisRepository> GetIDocumentAnalysisRepositoryOKMock(DocumentAnalysisResult documentAnalysisResult)
		{
			Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = new Mock<IDocumentAnalysisRepository>();
			documentAnalysisRepositoryMock.Setup(e => e.GetAnalysisAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(documentAnalysisResult));
			return documentAnalysisRepositoryMock;
		}

        private Mock<IDocumentAnalysisRepository> GetIDocumentAnalysisRepositoryEmtyMock()
        {
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = new Mock<IDocumentAnalysisRepository>();
            documentAnalysisRepositoryMock.Setup(e => e.GetAnalysisAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new DocumentAnalysisResult());
            return documentAnalysisRepositoryMock;
        }

        private Mock<IDocumentAnalysisRepository> GetIDocumentAnalysisRepositoryListOKMock(List<DocumentAnalysisResult> documentAnalysisResultList)
        {
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = new Mock<IDocumentAnalysisRepository>();
            documentAnalysisRepositoryMock.Setup(e => e.GetAnalysisListAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(System.Threading.Tasks.Task.FromResult(documentAnalysisResultList.AsEnumerable()));
            return documentAnalysisRepositoryMock;
        }

        private Mock<IDocumentAnalysisRepository> GetIDocumentAnalysisRepositoryListEmptyListMock()
        {
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = new Mock<IDocumentAnalysisRepository>();
            documentAnalysisRepositoryMock.Setup(e => e.GetAnalysisListAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<DocumentAnalysisResult>());
            return documentAnalysisRepositoryMock;
        }
        #endregion
    }
}