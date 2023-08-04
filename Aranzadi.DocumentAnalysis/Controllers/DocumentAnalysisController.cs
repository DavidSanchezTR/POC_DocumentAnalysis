using Aranzadi.DocumentAnalysis.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

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
		public async Task<IActionResult> Get(string Tenant, string Owner, string? DocumentId = null)
        {
			if (string.IsNullOrEmpty(Tenant) || string.IsNullOrEmpty(Owner))
            {
                Log.Error("TenantId and UserId is required");
                return BadRequest("TenantId and UserId is required");
            }

            var listAnalisis = await _documentAnalysisService.GetAnalysisAsync(Tenant, Owner, DocumentId);
            
			return Ok(JsonConvert.SerializeObject(listAnalisis));

        }

        // DELETE api/<DocumentAnalysisController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
