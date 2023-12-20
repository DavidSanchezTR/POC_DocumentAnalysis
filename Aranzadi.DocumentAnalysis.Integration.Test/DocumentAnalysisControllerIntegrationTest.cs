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
			var configuration = documentAnalysisOptions;
			var documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			string tenantId = AssemblyApp.TenantId;
            string documentId = Guid.NewGuid().ToString();

            //Act
            var documentAnalysisController = new DocumentAnalysisController(documentAnalysisService, documentAnalysisOptions);
			var result = await documentAnalysisController.Get(tenantId, documentId);

			//Assert
			Assert.IsTrue((result as ObjectResult).StatusCode == (int)HttpStatusCode.OK);

		}

		[TestMethod]
		public async Task Get_ValidValues_ReturnsERROR()
		{
			//Arrange
			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;
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

		[TestMethod]
		public async Task Get_List_ValidValues_ReturnsOK()
		{
			//Arrange
			if(AssemblyApp.documentAnalysisOptions == null || AssemblyApp.app == null) Assert.Fail();

			DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;

			IDocumentAnalysisService? documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			string tenantId = "5600";
            string documentsIds = Guid.NewGuid().ToString() + ";" + Guid.NewGuid().ToString(); 

            if (documentAnalysisService == null) Assert.Fail();

            //Act
            DocumentAnalysisController documentAnalysisController = new DocumentAnalysisController(documentAnalysisService, documentAnalysisOptions);
            IActionResult result = await documentAnalysisController.Get_List(tenantId, documentsIds);

			//Assert
			Assert.IsNotNull(result as ObjectResult);
			Assert.IsTrue((result as ObjectResult).StatusCode == (int)HttpStatusCode.OK);

		}

		[TestMethod]
		public async Task Get_List_ValidValues_ReturnsERROR()
		{
            //Arrange
            if (AssemblyApp.documentAnalysisOptions == null || AssemblyApp.app == null) Assert.Fail();

            DocumentAnalysisOptions documentAnalysisOptions = AssemblyApp.documentAnalysisOptions;

            IDocumentAnalysisService? documentAnalysisService = AssemblyApp.app.Services.GetService<IDocumentAnalysisService>();
			string? tenantId = null;
			string? documentsIds = null;

            if (documentAnalysisService == null) Assert.Fail();

            //Act
            DocumentAnalysisController documentAnalysisController = new DocumentAnalysisController(documentAnalysisService, documentAnalysisOptions);
            IActionResult result = await documentAnalysisController.Get_List(tenantId, documentsIds);

            //Assert
            Assert.IsNotNull(result as ObjectResult);
            Assert.IsTrue((result as ObjectResult).StatusCode == (int)HttpStatusCode.BadRequest);
		}
	}
}