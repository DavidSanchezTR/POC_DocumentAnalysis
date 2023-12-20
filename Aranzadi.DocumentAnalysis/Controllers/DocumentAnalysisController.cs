using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System.Reflection.Metadata;
using System.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Aranzadi.DocumentAnalysis.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentAnalysisController : ControllerBase
	{
		private readonly IDocumentAnalysisService _documentAnalysisService;
		private readonly DocumentAnalysisOptions _documentAnalysisOptions;

		public DocumentAnalysisController(IDocumentAnalysisService documentAnalysisService
			, DocumentAnalysisOptions documentAnalysisOptions)
		{
			_documentAnalysisService = documentAnalysisService;
			_documentAnalysisOptions = documentAnalysisOptions;
		}

		[HttpGet()]
		[Route("GetAnalysis")]
		public async Task<IActionResult> Get(string Tenant, string DocumentId)
		{
			try
			{
				LogContext.PushProperty("Origin", "Request");
				LogContext.PushProperty("DocumentId", DocumentId);
				LogContext.PushProperty("LawfirmId", Tenant);

				if (string.IsNullOrEmpty(Tenant) || string.IsNullOrEmpty(DocumentId))
				{
					Log.Error("TenantId and DocumentId are required");
					return BadRequest("TenantId and DocumentId are required");
				}

				DocumentAnalysisResponse analisis = await _documentAnalysisService.GetAnalysisAsync(Tenant, DocumentId);
				IEnumerable<DocumentAnalysisResponse> listAnalisis = new List<DocumentAnalysisResponse>() { analisis };

				return Ok(JsonConvert.SerializeObject(listAnalisis, new JsonSerializerSettings()
				{
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					Formatting = Formatting.Indented,
				}));
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error:{ex.Message}");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet()]
		[Route("GetAnalysisList")]
		public async Task<IActionResult> Get_List(string tenant, string documentIdList)
		{
			try
			{
				LogContext.PushProperty("Origin", "Request");
				LogContext.PushProperty("DocumentIdList", documentIdList);
				LogContext.PushProperty("LawfirmId", tenant);

				if (string.IsNullOrEmpty(tenant))
				{
					Log.Error("TenantId is required");
					return BadRequest("TenantId is required");
				}

				IEnumerable<DocumentAnalysisResponse> listAnalisis = await _documentAnalysisService.GetAnalysisListAsync(tenant, documentIdList);

				return Ok(JsonConvert.SerializeObject(listAnalisis, new JsonSerializerSettings()
				{
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					Formatting = Formatting.Indented,
				}));
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Error:{ex.Message}");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		// DELETE api/<DocumentAnalysisController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}

	}
}