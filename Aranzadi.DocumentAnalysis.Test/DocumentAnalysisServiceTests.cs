using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Microsoft.Azure.Amqp.Framing;
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

			Mock<IDocumentAnalysisRepository> documentAnalysisRepositoryMock = GetIDocumentAnalysisRepositoryOKMock(documentAnalysisResultList);

			var documentAnalysisService = new DocumentAnalysisService(documentAnalysisRepositoryMock.Object, Mock.Of<ILogger<DocumentAnalysisService>>());

			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => documentAnalysisService.GetAnalysisAsync("", "", Guid.Empty.ToString()), "Debería haber lanzado una excepción  por parametro vacío");
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
			content.Court = new DocumentAnalysisDataResultCourt()
			{
				City = "ciudad sample",
				Jurisdiction = "jurisdiccion sample",
				Name = "nombre sample",
				Number = "numero sample",
				CourtType = "tribunal sample"
			};
			content.Review = new DocumentAnalysisDataResultReview()
			{
				Cause = new string[] { "cause 1", "cause 2" },
				Review = "review sample"
			};
			content.Proceeding = new DocumentAnalysisDataResultProceeding()
			{
				NIG = "NIG sample",
				MilestonesNumber = "numero autos sample",
				ProceedingType = "procedimiento sample",
				ProceedingSubtype = "subprocedimiento sample",
			};

			var lista = new List<DocumentAnalysisDataResultProceedingParts>
			{
				new DocumentAnalysisDataResultProceedingParts()
				{
					Lawyers = "letrado sample",
					Source = "naturaleza sample",
					Name = "nombre sample ",
					Attorney = "procurador sample",
					PartType = "tipo parte sample",
					AppealPartType = "tipo parte recurso sample"

				},
				new DocumentAnalysisDataResultProceedingParts()
				{
					Lawyers = "letrado sample 2",
					Source = "naturaleza sample 2",
					Name = "nombre sample 2",
					Attorney = "procurador sample 2",
					PartType = "tipo parte sample 2",
					AppealPartType = "tipo parte recurso sample 2"

				}
			};
			content.Proceeding.Parts = lista.ToArray();
			content.Proceeding.InitialProceeding = new DocumentAnalysisDataResultProceedingInitialProceeding()
			{
				Court = "juzgado sample ",
				MilestonesNumber = "numero autos"
			};


			content.CourtDecision = new DocumentAnalysisDataCourtDecision()
			{
				Amount = "",
				CommunicationDate = DateTime.Now.AddDays(-100).ToString("O"),
				CourtDecisionDate = DateTime.Now.ToString("O"),
				Milestone = "hito sample",
				CourtDecisionNumber = "num resolucion sample",
				WrittenSummary = "resumen",

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