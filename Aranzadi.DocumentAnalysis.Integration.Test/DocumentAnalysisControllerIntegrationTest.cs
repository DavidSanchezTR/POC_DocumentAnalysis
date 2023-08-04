using Aranzadi.DocumentAnalysis.Controllers;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	[TestClass]
	public class DocumentAnalysisControllerIntegrationTest
	{
		[TestMethod]
		public async Task Get_ValidValues_ReturnsOK()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			documentAnalysisOptions.CheckIfExistsHashFileInCosmos = false; //EDIT
			var configuration = documentAnalysisOptions;
			var documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			string tenantId = "5600";
			string userId = "98";

			//Act
			var documentAnalysisController = new DocumentAnalysisController(documentAnalysisService, documentAnalysisOptions);
			var result = await documentAnalysisController.Get(tenantId, userId);

			//Assert
			Assert.IsTrue((result as ObjectResult).StatusCode == (int)HttpStatusCode.OK);

		}

		[TestMethod]
		public async Task Get_ValidValues_ReturnsERROR()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
			documentAnalysisOptions.CheckIfExistsHashFileInCosmos = false; //EDIT
			var configuration = documentAnalysisOptions;
			var documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			string tenantId = null;
			string userId = null;

			//Act
			var documentAnalysisController = new DocumentAnalysisController(documentAnalysisService, documentAnalysisOptions);
			var result = await documentAnalysisController.Get(tenantId, userId);

			//Assert
			Assert.IsTrue((result as ObjectResult).StatusCode == (int)HttpStatusCode.BadRequest);

		}

	}

}
