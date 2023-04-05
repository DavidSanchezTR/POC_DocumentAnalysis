using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test
{
    [TestClass]
    public class DocumentAnalysisServiceTests
    {
        private ILogger<DocumentAnalysisService> logger;

        [TestInitialize]
        public void Initialize()
        {
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole());

           logger = loggerFactory.CreateLogger<DocumentAnalysisService>();
        }


       [TestMethod]
        public async Task GetAnalysisAsync_ValidValues_ReturnsDocumentAnalysisResult()
        {
            var documentAnalysisResult = GetDocumentAnalysisResult();
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResult);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, logger);

            var result = await documentAnalysisService.GetAnalysisAsync("122","22", documentAnalysisResult.DocumentId);
            Assert.AreEqual(documentAnalysisResult.Analysis,result.Analysis);
        } 
        
        [TestMethod]
        public async Task GetAnalysisAsync_StringEmpty_ReturnsException()
        {    
            var documentAnalysisResult = GetDocumentAnalysisResult();
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResult);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, logger);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync("", "22", documentAnalysisResult.DocumentId), "Debería haber lanzado una excepción  por parametro vacío");
        }

        private DocumentAnalysisResult GetDocumentAnalysisResult()
        {
            return new DocumentAnalysisResult
            {
                Analysis = "Esto es un analisis",
                DocumentId = Guid.NewGuid(),
                Status = DocumentAnalysisResult.StatusResult.Pendiente
            };
        }

        private Mock<IDocumentAnalysisRepository> GetIDocumentAnalysisRepositoryOKMock(DocumentAnalysisResult documentAnalysisResult)
        {
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = new();
            documentAnalysisRepositoryMock.Setup(e => e.GetAnalysisAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(documentAnalysisResult));
            return documentAnalysisRepositoryMock;
        }
    }
}