using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.DTO.Enums;
using Aranzadi.DocumentAnalysis.DTO.Response;
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

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync(""), "Debería haber lanzado una excepción  por parametro vacío");
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

            var result = await documentAnalysisService.GetAllAnalysisAsync("122", "22");
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

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAllAnalysisAsync("", "22"), "Debería haber lanzado una excepción  por parametro vacío");
        }

        private DocumentAnalysisResult GetDocumentAnalysisResult()
        {
            //TODO: json de atomian, de ejemplo
			//string jsonAtomain = "[{\"originalMessageID\":\"eaf22420-c7cb-4ff4-8ee0-dd60c0df8ad5\",\"receivedAt\":\"2022-09-01T12:31:40.3335203+00:00\",\"source\":\"Fusion\",\"status\":2,\"originalMessageType\":\"DocumentAnalysis\",\"tenant\":\"4131\",\"errors\":0,\"totalChunks\":1,\"completedChunks\":1,\"userId\":\"5\",\"chunks\":[{\"id\":\"DbOperationChunk|9ead1d33-0519-46df-974c-9fed8bf0a893\",\"data\":{\"EmailId\":\"<AM8PR09MB5477735F7452CF45BB42834F8B7B9@AM8PR09MB5477.eurprd09.prod.outlook.com>\",\"Subject\":\"pruebaconunasuntodecorreomáslargoquehabráquerecortarparaqueseveabonitoeninfolex\",\"Document\":{\"Hash\":\"03ABED756EBC32AA38DE456E5F34B857\",\"Name\":\"03ABED756EBC32AA38DE456E5F34B857.PDF\",\"Path\":\"/4131/03ABED756EBC32AA38DE456E5F34B857.pdf\"},\"UserAnalysis\":{\"LawFirmId\":4131,\"UserId\":5}},\"detail\":\"{\\\"result\\\":{\\\"status\\\":\\\"OK\\\",\\\"code\\\":450,\\\"error_code\\\":\\\"000\\\",\\\"error_reason\\\":\\\"Noerrors\\\",\\\"external_id\\\":\\\"<20220912134632>\\\",\\\"content\\\":{\\\"juzgado\\\":{\\\"nombre\\\":\\\"JUZGADODEPRIMERAINSTANCIAN\\u00b03DEVALENCIA111\\\",\\\"jurisdiccion\\\":\\\"01civil\\\",\\\"tipotribunal\\\":\\\"07juzgadodeprimerainstancia\\\",\\\"ciudad\\\":\\\"valencia\\\",\\\"numero\\\":\\\"3\\\"},\\\"procedimiento\\\":{\\\"N.I.G.\\\":\\\"\\\",\\\"tipoprocedimiento\\\":\\\"08ejecuciondetitulosjudiciales\\\",\\\"subtipoprocedimiento\\\":\\\"\\\",\\\"numeroautos\\\":\\\"9001094\\/2020\\\",\\\"partes\\\":[{\\\"nombre\\\":\\\"Faa\\\",\\\"naturaleza\\\":\\\"personafisica\\\",\\\"tipoparte\\\":\\\"demandada\\\",\\\"procurador\\\":\\\"FlorentinaPerezSamper\\\",\\\"letrados\\\":\\\"\\\"}]},\\\"resolucion\\\":{\\\"tiporesolucion\\\":\\\"03auto\\\",\\\"numeroresolucion\\\":\\\"\\\",\\\"fecharesolucion\\\":\\\"30\\/09\\/2020\\\",\\\"fechanotificacion\\\":\\\"01\\/10\\/202016:47:00\\\",\\\"hito\\\":\\\"J08110000despachoejecucion\\\",\\\"hito_origin\\\":\\\"despachaejecuci\\u00e9nporimportede13015,65eurosenconceptodeprincipaleinteresesordinariosymoratoriosvencidos,masotros2600eurosquesefijanprovisionalmenteenconceptodeinteresesque,ensucaso,puedandevengarsedurantelaejecucionDESPACHANDOEJECUCION\\\",\\\"requerimientos\\\":[],\\\"recurso\\\":[]},\\\"review\\\":{\\\"review\\\":\\\"no\\\"},\\\"ocr\\\":\\\"yes\\\"}}}\"}]},{\"originalMessageID\":\"660f9731-10de-4077-b207-c47baeaf6d38\",\"receivedAt\":\"2022-09-01T11:41:54.5882887+00:00\",\"source\":\"Fusion\",\"status\":1,\"originalMessageType\":\"DocumentAnalysis\",\"tenant\":\"4131\",\"errors\":0,\"totalChunks\":2,\"completedChunks\":2,\"userId\":\"5\",\"chunks\":[{\"id\":\"DbOperationChunk|08768c7e-a143-4e4f-8ccd-b38564e8fd95\",\"data\":{\"EmailId\":\"<AM9P195MB13483B80E917E5519EEA972ABED79@AM9P195MB1348.EURP195.PROD.OUTLOOK.COM>\",\"Subject\":\"pruebas\",\"Document\":{\"Hash\":\"03ABED756EBC32AA38DE456E5F34B857\",\"Name\":\"03ABED756EBC32AA38DE456E5F34B857.PDF\",\"Path\":\"/4131/D654DA7EFF79D34FD3BF75555D7F4BC2.PDF\"},\"UserAnalysis\":{\"LawFirmId\":4131,\"UserId\":5}},\"detail\":\"{\\\"result\\\":{\\\"status\\\":\\\"OK\\\",\\\"code\\\":450,\\\"error_code\\\":\\\"000\\\",\\\"error_reason\\\":\\\"Noerrors\\\",\\\"external_id\\\":\\\"<20220912134632>\\\",\\\"content\\\":{\\\"juzgado\\\":{\\\"nombre\\\":\\\"JUZGADODEPRIMERAINSTANCIAN\\u00b03DEVALENCIA222\\\",\\\"jurisdiccion\\\":\\\"01civil\\\",\\\"tipotribunal\\\":\\\"07juzgadodeprimerainstancia\\\",\\\"ciudad\\\":\\\"valencia\\\",\\\"numero\\\":\\\"3\\\"},\\\"procedimiento\\\":{\\\"N.I.G.\\\":\\\"\\\",\\\"tipoprocedimiento\\\":\\\"08ejecuciondetitulosjudiciales\\\",\\\"subtipoprocedimiento\\\":\\\"\\\",\\\"numeroautos\\\":\\\"9001094\\/2020\\\",\\\"partes\\\":[{\\\"nombre\\\":\\\"Faa\\\",\\\"naturaleza\\\":\\\"personafisica\\\",\\\"tipoparte\\\":\\\"demandada\\\",\\\"procurador\\\":\\\"FlorentinaPerezSamper\\\",\\\"letrados\\\":\\\"\\\"}]},\\\"resolucion\\\":{\\\"tiporesolucion\\\":\\\"03auto\\\",\\\"numeroresolucion\\\":\\\"\\\",\\\"fecharesolucion\\\":\\\"30\\/09\\/2020\\\",\\\"fechanotificacion\\\":\\\"01\\/10\\/202016:47:00\\\",\\\"hito\\\":\\\"J08110000despachoejecucion\\\",\\\"hito_origin\\\":\\\"despachaejecuci\\u00e9nporimportede13015,65eurosenconceptodeprincipaleinteresesordinariosymoratoriosvencidos,masotros2600eurosquesefijanprovisionalmenteenconceptodeinteresesque,ensucaso,puedandevengarsedurantelaejecucionDESPACHANDOEJECUCION\\\",\\\"requerimientos\\\":[],\\\"recurso\\\":[]},\\\"review\\\":{\\\"review\\\":\\\"no\\\"},\\\"ocr\\\":\\\"yes\\\"}}}\"},{\"id\":\"DbOperationChunk|65343426-30ec-4783-9800-e4140b7969a9\",\"data\":{\"EmailId\":\"<AM9P195MB13483B80E917E5519EEA972ABED79@AM9P195MB1348.EURP195.PROD.OUTLOOK.COM>\",\"Subject\":\"pruebas\",\"Document\":{\"Hash\":\"14E75715471BEC2B0B71DE4FE7430CDC\",\"Name\":\"14E75715471BEC2B0B71DE4FE7430CDC.PDF\",\"Path\":\"/4131/14E75715471BEC2B0B71DE4FE7430CDC.PDF\"},\"UserAnalysis\":{\"LawFirmId\":4131,\"UserId\":5}},\"detail\":\"{\\\"result\\\":{\\\"status\\\":\\\"OK\\\",\\\"code\\\":450,\\\"error_code\\\":\\\"000\\\",\\\"error_reason\\\":\\\"Noerrors\\\",\\\"external_id\\\":\\\"<20220912134632>\\\",\\\"content\\\":{\\\"juzgado\\\":{\\\"nombre\\\":\\\"JUZGADODEPRIMERAINSTANCIAN\\u00b03DEVALENCIA333\\\",\\\"jurisdiccion\\\":\\\"01civil\\\",\\\"tipotribunal\\\":\\\"07juzgadodeprimerainstancia\\\",\\\"ciudad\\\":\\\"valencia\\\",\\\"numero\\\":\\\"3\\\"},\\\"procedimiento\\\":{\\\"N.I.G.\\\":\\\"\\\",\\\"tipoprocedimiento\\\":\\\"08ejecuciondetitulosjudiciales\\\",\\\"subtipoprocedimiento\\\":\\\"\\\",\\\"numeroautos\\\":\\\"9001094\\/2020\\\",\\\"partes\\\":[{\\\"nombre\\\":\\\"Faa\\\",\\\"naturaleza\\\":\\\"personafisica\\\",\\\"tipoparte\\\":\\\"demandada\\\",\\\"procurador\\\":\\\"FlorentinaPerezSamper\\\",\\\"letrados\\\":\\\"\\\"}]},\\\"resolucion\\\":{\\\"tiporesolucion\\\":\\\"03auto\\\",\\\"numeroresolucion\\\":\\\"\\\",\\\"fecharesolucion\\\":\\\"30\\/09\\/2020\\\",\\\"fechanotificacion\\\":\\\"01\\/10\\/202016:47:00\\\",\\\"hito\\\":\\\"J08110000despachoejecucion\\\",\\\"hito_origin\\\":\\\"despachaejecuci\\u00e9nporimportede13015,65eurosenconceptodeprincipaleinteresesordinariosymoratoriosvencidos,masotros2600eurosquesefijanprovisionalmenteenconceptodeinteresesque,ensucaso,puedandevengarsedurantelaejecucionDESPACHANDOEJECUCION\\\",\\\"requerimientos\\\":[],\\\"recurso\\\":[]},\\\"review\\\":{\\\"review\\\":\\\"no\\\"},\\\"ocr\\\":\\\"yes\\\"}}}\"}]}]";
			//DocumentAnalysisDataOK analisisSample1 = JsonConvert.DeserializeObject<DocumentAnalysisDataOK>(jsonAtomain);
			//DocumentAnalysisDataJsonResultOK analisisSample = JsonConvert.DeserializeObject<DocumentAnalysisDataJsonResultOK>(jsonAtomain);



			DocumentAnalysisDataJsonResultOK analisis = new DocumentAnalysisDataJsonResultOK();
            analisis.status = "status";
            analisis.external_id = "1";
            analisis.code = 1;
            string json = JsonConvert.SerializeObject(analisis);

            return new DocumentAnalysisResult
            {
                Analysis = json, // "Esto es un analisis",
                DocumentId = Guid.NewGuid(),
                Status = StatusResult.Pendiente
            };
        }

        private DocumentAnalysisResponse GetDocumentAnalysisResponse(DocumentAnalysisResult documentAnalysisResult)
        {
            DocumentAnalysisDataJsonResultOK analisis = JsonConvert.DeserializeObject<DocumentAnalysisDataJsonResultOK>(documentAnalysisResult.Analysis);
			return new DocumentAnalysisResponse
            {
                DocumentUniqueRefences = documentAnalysisResult.DocumentId.ToString(),
                Result = analisis,
                Status = documentAnalysisResult.Status,
            };
        }

        private DocumentAnalysisDataJsonResultOK Get_DocumentAnalysisDataJsonResultOK()
        {
			DocumentAnalysisDataJsonResultOK analisis = new DocumentAnalysisDataJsonResultOK();
			analisis.status = "status";
			analisis.external_id = "1";
			analisis.code = 1;
            return analisis;
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