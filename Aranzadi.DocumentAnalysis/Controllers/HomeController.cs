using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace Aranzadi.DocumentAnalysis.Controllers
{
	[Route("")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		private readonly DocumentAnalysisOptions documentAnalysisOptions;

		public HomeController(DocumentAnalysisOptions documentAnalysisOptions)
        {
			this.documentAnalysisOptions = documentAnalysisOptions;
		}
        [HttpGet]
		public IActionResult Get()
		{
			string scrubbedEndpoint = Regex.Replace(documentAnalysisOptions.ServiceBus.ConnectionString, @"SharedAccessKey=.*", "SharedAccessKey=[Oculto]");
			string queueName = documentAnalysisOptions.ServiceBus.Queue;
			var message = $"El servicio de análisis de documentos está activo y escuchando en la cola {queueName} del serviceBus {scrubbedEndpoint}";
			
			return Content(message);
		}
	}
}
