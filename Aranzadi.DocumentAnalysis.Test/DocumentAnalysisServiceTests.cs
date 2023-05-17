using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Microsoft.Extensions.Logging;
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
            content.Juzgado = new DocumentAnalysisDataResultJudgement()
            {
                Ciudad = "ciudad sample",
                Jurisdiccion = "jurisdiccion sample",
                Nombre = "nombre sample",
                Numero = "numero sample",
                TipoTribunal = "tribunal sample"
            };
            content.Review = new DocumentAnalysisDataResultReview()
            {
                Cause = new string[] { "cause 1", "cause 2" },
                Review = "review sample"
            };
            content.Procedimiento = new DocumentAnalysisDataResultProcedure()
            {
                NIG = "NIG sample",
                NumeroAutos = "numero autos sample",
                TipoProcedimiento = "procedimiento sample",
                SubtipoProcedimiento = "subprocedimiento sample",
            };

            var lista = new List<DocumentAnalysisDataResultProcedureParts>
        {
            new DocumentAnalysisDataResultProcedureParts()
            {
                Letrados = "letrado sample",
                Naturaleza = "naturaleza sample",
                Nombre = "nombre sample",
                Procurador = "procurador sample",
                TipoParte = "tipo parte sample",
                TipoParteRecurso = "tipo parte recurso sample"

            },
            new DocumentAnalysisDataResultProcedureParts()
            {
                Letrados = "letrado sample 2",
                Naturaleza = "naturaleza sample 2",
                Nombre = "nombre sample 2",
                Procurador = "procurador sample 2",
                TipoParte = "tipo parte sample 2",
                TipoParteRecurso = "tipo parte recurso sample 2"

            }
        };
            content.Procedimiento.Partes = lista.ToArray();
            content.Procedimiento.ProcedimientoInicial = new DocumentAnalysisDataResultProcedureInitialProcedure()
            {
                Juzgado = "juzgado sample",
                NumeroAutos = "numero autos"
            };

            content.Resolucion = new DocumentAnalysisDataResolution()
            {
                Cuantia = "",
                FechaNotificacion = DateTime.Today.AddDays(-100).ToString(),
                FechaResolucion = DateTime.Today.ToString(),
                Hito = "hito sample",
                NumeroResolucion = "num resolucion sample",
                ResumenEscrito = "resumen",

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