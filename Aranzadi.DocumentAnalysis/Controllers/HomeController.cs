using Microsoft.AspNetCore.Mvc;
using System.Reflection;
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
			var vDocumentAnalysisMessagingPackage = Assembly.GetAssembly(typeof(Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations.MessagingConfiguration))?.GetName().Version?.ToString();
			string scrubbedEndpoint = Regex.Replace(documentAnalysisOptions.Messaging.Endpoint, @"SharedAccessKey=.*", "SharedAccessKey=[Oculto]");
			string queueName = documentAnalysisOptions.Messaging.Queue;
			var message = $"El servicio de análisis de documentos está activo y escuchando en la cola {queueName} del serviceBus {scrubbedEndpoint}. " +
				$"DocumentAnalysis.Messaging (versión {vDocumentAnalysisMessagingPackage})";
			
			return Content(message);
		}
	}
}
