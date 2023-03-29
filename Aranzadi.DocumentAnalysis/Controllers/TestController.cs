using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Aranzadi.DocumentAnalysis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        //private static readonly SHA256 sHA256;
        private readonly DocumentAnalysisData datos = new DocumentAnalysisData
        {
            App = "Infolex",
            DocumentName = "Prueba.pdf",
            NewGuid = new Guid(Guid.NewGuid().ToString()),
            Analisis = "Esto es un análisis",
            AccessUrl = "www.prueba.com",
            Sha256 = "HasCode",
            Estado = "Pendiente",
            TenantId = 122,
            UserId = 22,
            FechaAnalisis = new DateTimeOffset().ToLocalTime(),
            FechaCreacion = new DateTimeOffset().ToLocalTime(),
        };

        private readonly IDocumentAnalysisRepository documentAnalysisRepository;

        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger, IDocumentAnalysisRepository documentAnalysisRepository)
        {
            _logger = logger;
            this.documentAnalysisRepository = documentAnalysisRepository;
        }

        [HttpGet(Name = "GetTestController")]
        public IEnumerable<Test> Get()
        {
            Task.Run(() => documentAnalysisRepository.AddAnalysisDataAsync(datos));
            return Enumerable.Range(1, 5).Select(index => new Test
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();            
        }       
    }
}