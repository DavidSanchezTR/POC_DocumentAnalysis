using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Aranzadi.DocumentAnalysis.Data.Repository;
using System.Threading;
using Util.TestingMockAsyncMethods;
using Microsoft.Extensions.Logging;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;

namespace Aranzadi.DocumentAnalysis.Data.Test
{
    [TestClass]
    public class DocumentAnalysisRepositoryTests
    {
        [TestMethod]
        public async Task AddAnalysisDataAsync_ValidValues_ReturnsOK()
        {
            DocumentAnalysisData data = GetDocumentAnalysisData(); 
            
            var mockSet = new Mock<DbSet<DocumentAnalysisData>>();
            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(mockSet.Object);
            mockDocumentAnalysisDbContext.Setup(sp => sp.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.AddAnalysisDataAsync(data);

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public async Task AddAnalysisDataAsync_InvalidValues_ReturnsEmptyFromException()
        {
            DocumentAnalysisData data = GetDocumentAnalysisData();

            var mockSet = new Mock<DbSet<DocumentAnalysisData>>();
            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(mockSet.Object);
            mockDocumentAnalysisDbContext.Setup(sp => sp.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws<Exception>();

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);

            var result = await analysisRepository.AddAnalysisDataAsync(data);
            Assert.AreEqual(result, 0);
        }


        [TestMethod]
        public async Task UpdateAnalysisDataAsync_ValidValues_Returns1()
        {            
            DocumentAnalysisData data = GetDocumentAnalysisData();
            var lista = new List<DocumentAnalysisData>
            {
                data,
                data
            };

            var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);
            mockDocumentAnalysisDbContext.Setup(sp => sp.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.UpdateAnalysisDataAsync(data);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public async Task UpdateAnalysisDataAsync_ValidValues_Returns0()
        {
            DocumentAnalysisData data = GetDocumentAnalysisData();
            var lista = new List<DocumentAnalysisData>
            {
                data,
                data
            };

            var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.UpdateAnalysisDataAsync(new DocumentAnalysisData());
            Assert.AreEqual(result, 0);
        }


        [TestMethod]
        public async Task GetAllAnalysisAsync_ValidValues_ReturnsDocumentAnalysisResultList()
        {
            var lista = new List<DocumentAnalysisData>
            {
                GetDocumentAnalysisData()
            };

            var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();  
            
            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.GetAnalysisAsync(lista[0].TenantId, lista[0].UserId);
            Assert.AreEqual(result.Count(), 1);
        }

         [TestMethod]
        public async Task GetAllAnalysisAsync_ValidValues_ReturnsDocumentAnalysisResultList2()
        {
            DocumentAnalysisData data = GetDocumentAnalysisData();
            var lista = new List<DocumentAnalysisData>
            {
                data,
                data
            };

            var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();  
            
            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.GetAnalysisAsync(lista[0].TenantId, lista[0].UserId);
            Assert.AreEqual(result.Count(), 2);
        }

        [TestMethod]
        public async Task GetAllAnalysisAsync_InvalidValues_ReturnsEmptyFromException()
        {
            var dbSetMock = new Mock<DbSet<DocumentAnalysisData>>();

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();  
            
            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.GetAnalysisAsync("","");
            Assert.AreEqual(result.Count(), 0);
        }


        [TestMethod]
        public async Task GetAnalysisAsync_ValidValues_ReturnsDocumentAnalysisResultOK()
        {
             var lista = new List<DocumentAnalysisData>
             {
              GetDocumentAnalysisData(AnalysisStatus.Done)
             };

            var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.GetAnalysisAsync(lista[0].TenantId, lista[0].UserId, lista[0].Id.ToString());
            Assert.AreEqual(result.First().DocumentId, lista[0].Id);
            Assert.AreEqual(result.First().Status, lista[0].Status);
            Assert.AreEqual(result.First().Analysis, lista[0].Analysis);
        }

        [TestMethod]
       
        public async Task GetAnalysisAsync_InvalidData_ReturnsDocumentAnalysisResultEmpty()
        {
            var lista = new List<DocumentAnalysisData>
             {
              GetDocumentAnalysisData(AnalysisStatus.Done)
             };

            var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.GetAnalysisAsync("", "", Guid.Empty.ToString());
			Assert.AreEqual(result.Count(), 0);
		}

		[TestMethod]
		public async Task GetAnalysisDoneAsync_ValidValues_ReturnsDocumentAnalysisResultOK()
		{
			var lista = new List<DocumentAnalysisData>
			 {
			  GetDocumentAnalysisData(AnalysisStatus.Done)
			 };

			var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

			Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

			mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

			DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
			var result = await analysisRepository.GetAnalysisDoneAsync(lista[0].Sha256);
			Assert.AreEqual(result?.DocumentId, lista[0].Id);
			Assert.AreEqual(result?.Status, lista[0].Status);
			Assert.AreEqual(result?.Analysis, lista[0].Analysis);
		}

		[TestMethod]
		public async Task GetAnalysisDoneAsync_PendingStatus_ReturnsDocumentAnalysisResultEmpty()
		{
			var lista = new List<DocumentAnalysisData>
			{
				GetDocumentAnalysisData()
			};
			var dbSetMock = CreateDbSetMock<DocumentAnalysisData>(lista.AsQueryable());

			Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();

			mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

			DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
			var result = await analysisRepository.GetAnalysisDoneAsync(lista[0].Sha256);
			Assert.IsNull(result);
		}

		private DocumentAnalysisData GetDocumentAnalysisData(
            AnalysisStatus status = AnalysisStatus.Pending)
        {
            return new DocumentAnalysisData
            {
                Id = Guid.NewGuid(),
                App = "infolex",
                DocumentName = "name.pdf",
                AccessUrl = "https://example.es",
                Analysis = "string analysis example",
                Status = status,// 1,
                Source = Source.LaLey, // "source",
                Sha256 = "Hash",
                TenantId = "1234",
                UserId = "22",
                AnalysisDate = DateTimeOffset.Now,
                CreateDate = DateTimeOffset.Now
            };
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<T>(items.GetEnumerator()));
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(items.Provider));
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            return dbSetMock;
        }

      




    }
}