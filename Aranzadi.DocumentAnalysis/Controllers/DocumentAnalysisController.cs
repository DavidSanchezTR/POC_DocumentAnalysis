using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Aranzadi.DocumentAnalysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentAnalysisController : ControllerBase
    {
        private readonly IDocumentAnalysisService _documentAnalysisService;
        private readonly IConfiguration _config;
        private readonly ILogger<DocumentAnalysisController> _logger;
        private readonly DocumentAnalysisOptions _documentAnalysisOptions;

        public DocumentAnalysisController(IDocumentAnalysisService documentAnalysisService, ILogger<DocumentAnalysisController> logger,
            IConfiguration config, DocumentAnalysisOptions documentAnalysisOptions)
        {
            _documentAnalysisService = documentAnalysisService;
            _logger = logger;  
            _config = config;
            _documentAnalysisOptions = documentAnalysisOptions;
        }

        [HttpGet()]
		[Route("GetAnalysis")]
		public async Task<IActionResult> Get(string Tenant, string Owner, string? DocumentId = null)
        {
            if (string.IsNullOrEmpty(Tenant) || string.IsNullOrEmpty(Owner))
            {
                return BadRequest("TenantId and UserId is required");
            }

            var listAnalisis = await _documentAnalysisService.GetAnalysisAsync(Tenant, Owner, DocumentId);
            
			return Ok(JsonConvert.SerializeObject(listAnalisis));

        }

        //[HttpGet("GetAnalysis/{tenantId}/{userId}/")]
        //public async Task<IActionResult> Get(string tenantId, string userId)
        //{
        //	if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
        //	{
        //		return BadRequest("TenantId and UserId is required");
        //	}

        //	var listAnalisis = await _documentAnalysisService.GetAnalysisAsync(tenantId, userId);

        //	return Ok(listAnalisis);
        //}

        // GET: api/<DocumentAnalysisController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // DELETE api/<DocumentAnalysisController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
