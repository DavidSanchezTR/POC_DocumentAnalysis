using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("GetAllAnalysis{tenantId}/{userId}")]
        public async Task<IActionResult> Obtener(string tenantId, string userId)
        {
            if(string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
            {
                return BadRequest("TenantId and UserId is required");
            }            

            var allAnalisis = await _documentAnalysisService.GetAllAnalysisAsync(tenantId, userId);

            return Ok(allAnalisis);
        }        

        // GET api/<DocumentAnalysisController>/5
        [HttpGet("GetAnalysis{tenantId}/{userId}/{documentId}")]
        public async Task<IActionResult> Get(string tenantId, string userId, Guid documentId)
        {
            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId) || documentId == Guid.Empty)
            {
                return BadRequest("TenantId and UserId is required");
            }

            var singleAnalisis = await _documentAnalysisService.GetAnalysisAsync(tenantId, userId, documentId);

            return Ok(singleAnalisis);
        }

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
