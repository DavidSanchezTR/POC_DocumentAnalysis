using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.DTO.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test
{
    [TestClass]
    public class DocumentAnalysisServiceTests
    {    

       [TestMethod]
        public async Task GetAnalysisAsync_ValidValues_ReturnsDocumentAnalysisResult()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResult();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

            var result = await documentAnalysisService.GetAnalysisAsync("Hash");
            Assert.AreEqual(documentAnalysisResult.Analysis,result.Analysis);
            Assert.AreEqual(documentAnalysisResult.DocumentId,result.DocumentId);
            Assert.AreEqual(documentAnalysisResult.Status,result.Status);
        } 
        
        [TestMethod]
        public async Task GetAnalysisAsync_StringEmpty_ReturnsException()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResult();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            Mock <IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync(""), "Debería haber lanzado una excepción  por parametro vacío");
        }

        [TestMethod]
        public async Task GetAllAnalysisAsync_ValidValues_ReturnsDocumentAnalysisResultList()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResult();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

            var result = await documentAnalysisService.GetAllAnalysisAsync("122", "22");
            Assert.AreEqual(documentAnalysisResultList.Count, result.Count());
            Assert.AreSame(documentAnalysisResultList, result);
            CollectionAssert.AreEqual(documentAnalysisResultList, result.ToList());
        }


        [TestMethod]
        public async Task GetAllAnalysisAsync_StringEmpty_ReturnsException()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResult();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAllAnalysisAsync("", "22"), "Debería haber lanzado una excepción  por parametro vacío");
        }


        private DocumentAnalysisResult GetDocumentAnalysisResult()
        {
            return new DocumentAnalysisResult
            {
                Analysis = "Esto es un analisis",
                DocumentId = Guid.NewGuid(),
                Status = StatusResult.Pendiente
            };
        }

        private Mock<IDocumentAnalysisRepository> GetIDocumentAnalysisRepositoryOKMock(List<DocumentAnalysisResult> documentAnalysisResultList)
        {
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = new Mock<IDocumentAnalysisRepository>();
            documentAnalysisRepositoryMock.Setup(e => e.GetAnalysisAsync(It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResultList[0])!);
            documentAnalysisRepositoryMock.Setup(e => e.GetAllAnalysisAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResultList.AsEnumerable()));
            return documentAnalysisRepositoryMock;
        }

    }
}