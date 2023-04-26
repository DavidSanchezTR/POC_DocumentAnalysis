using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.DTO.Enums;
using Aranzadi.DocumentAnalysis.DTO.Response;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
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
        public async Task GetAnalysisAsync_StringEmpty_ReturnsException()
        {
            DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResult();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            Mock <IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync("", "" , Guid.Empty.ToString()), "Debería haber lanzado una excepción  por parametro vacío");
        }

        [TestMethod]
        public async Task GetAllAnalysisAsync_ValidValues_ReturnsDocumentAnalysisResultList()
        {
			DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResult();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            DocumentAnalysisResponse documentAnalysisResponse = GetDocumentAnalysisResponse(documentAnalysisResult);
			List<DocumentAnalysisResponse> documentAnalysisResponseList = new List<DocumentAnalysisResponse>();
			documentAnalysisResponseList.Add(documentAnalysisResponse);

			Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

            var result = await documentAnalysisService.GetAnalysisAsync("122", "22");
            Assert.AreEqual(documentAnalysisResponseList.Count, result.Count());
            Assert.AreEqual(documentAnalysisResponseList[0].DocumentUniqueRefences, result.FirstOrDefault().DocumentUniqueRefences);
            CollectionAssert.AreEqual(documentAnalysisResponseList, result.ToList());
        }


        [TestMethod]
        public async Task GetAllAnalysisAsync_StringEmpty_ReturnsException()
        {
			DocumentAnalysisResult documentAnalysisResult = GetDocumentAnalysisResult();
            List<DocumentAnalysisResult> documentAnalysisResultList = new List<DocumentAnalysisResult>();
            documentAnalysisResultList.Add(documentAnalysisResult);

            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

            var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync("", "22"), "Debería haber lanzado una excepción  por parametro vacío");
        }

        private DocumentAnalysisResult GetDocumentAnalysisResult()
        {
            string json = JsonConvert.SerializeObject(Get_DocumentAnalysisDataResultContent());

            return new DocumentAnalysisResult
            {
                Analysis = json,
                DocumentId = Guid.NewGuid(),
                Status = AnalysisStatus.Pending
            };
        }

        private DocumentAnalysisResponse GetDocumentAnalysisResponse(DocumentAnalysisResult documentAnalysisResult)
        {
			DocumentAnalysisDataResultContent analisisContentResponse = JsonConvert.DeserializeObject<DocumentAnalysisDataResultContent>(documentAnalysisResult.Analysis);
			return new DocumentAnalysisResponse
            {
                DocumentUniqueRefences = documentAnalysisResult.DocumentId.ToString(),
                Result = analisisContentResponse,
                Status = documentAnalysisResult.Status,
            };
        }

        private DocumentAnalysisDataResultContent Get_DocumentAnalysisDataResultContent()
        {
			DocumentAnalysisDataResultContent content = new DocumentAnalysisDataResultContent();
			content.juzgado = new DocumentAnalysisDataResultJudgement()
			{
				ciudad = "ciudad sample",
				jurisdiccion = "jurisdiccion sample",
				nombre = "nombre sample",
				numero = "numero sample",
				tipotribunal = "tribunal sample"
			};
			content.review = new DocumentAnalysisDataResultReview()
			{
				cause = new string[] { "cause 1", "cause 2" },
				review = "review sample"
			};
			content.procedimiento = new DocumentAnalysisDataResultProcedure()
			{
				NIG = "NIG sample",
				numeroautos = "numero autos sample",
				tipoprocedimiento = "procedimiento sample",
				subtipoprocedimiento = "subprocedimiento sample",
			};

			var lista = new List<DocumentAnalysisDataResultProcedureParts>
		{
			new DocumentAnalysisDataResultProcedureParts()
			{
				letrados = "letrado sample",
				naturaleza = "naturaleza sample",
				nombre = "nombre sample",
				procurador = "procurador sample",
				tipoparte = "tipo parte sample",
				tipoparterecurso = "tipo parte recurso sample"

			},
			new DocumentAnalysisDataResultProcedureParts()
			{
				letrados = "letrado sample 2",
				naturaleza = "naturaleza sample 2",
				nombre = "nombre sample 2",
				procurador = "procurador sample 2",
				tipoparte = "tipo parte sample 2",
				tipoparterecurso = "tipo parte recurso sample 2"

			}
		};
			content.procedimiento.partes = lista.ToArray();
			content.procedimiento.procedimientoinicial = new DocumentAnalysisDataResultProcedureInitialProcedure()
			{
				juzgado = "juzgado sample",
				numeroautos = "numero autos"
			};

			content.resolucion = new DocumentAnalysisDataResolution()
			{
				cuantia = "",
				fechanotificacion = DateTime.Today.AddDays(-100).ToString(),
				fecharesolucion = DateTime.Today.ToString(),
				hito = "hito sample",
				numeroresolucion = "num resolucion sample",
				resumenescrito = "resumen",

			};
			return content;
		}

		private Mock<IDocumentAnalysisRepository> GetIDocumentAnalysisRepositoryOKMock(List<DocumentAnalysisResult> documentAnalysisResultList)
        {
            Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = new Mock<IDocumentAnalysisRepository>();
			documentAnalysisRepositoryMock.Setup(e => e.GetAnalysisAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(documentAnalysisResultList.AsEnumerable()));
            return documentAnalysisRepositoryMock;
        }

    }
}