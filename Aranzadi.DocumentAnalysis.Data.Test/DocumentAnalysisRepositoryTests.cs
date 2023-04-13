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
            var result = await analysisRepository.GetAllAnalysisAsync(lista[0].TenantId, lista[0].UserId);
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
            var result = await analysisRepository.GetAllAnalysisAsync(lista[0].TenantId, lista[0].UserId);
            Assert.AreEqual(result.Count(), 2);
        }

        [TestMethod]
        public async Task GetAllAnalysisAsync_InvalidValues_ReturnsEmptyFromException()
        {
            var dbSetMock = new Mock<DbSet<DocumentAnalysisData>>();

            Mock<DocumentAnalysisDbContext> mockDocumentAnalysisDbContext = new Mock<DocumentAnalysisDbContext>();  
            
            mockDocumentAnalysisDbContext.Setup(sp => sp.Analysis).Returns(dbSetMock.Object);

            DocumentAnalysisRepository analysisRepository = new DocumentAnalysisRepository(mockDocumentAnalysisDbContext.Object);
            var result = await analysisRepository.GetAllAnalysisAsync("","");
            Assert.AreEqual(result.Count(), 0);
        }

        private DocumentAnalysisData GetDocumentAnalysisData()
        {
            return new DocumentAnalysisData
            {
                Id = Guid.NewGuid(),
                App = "infolex",
                DocumentName = "name.pdf",
                AccessUrl = "https://example.es",
                Analysis = "string analysis example",
                Status = DTO.Enums.StatusResult.Pendiente,// 1,
                Source = DTO.Enums.Source.LaLey, // "source",
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